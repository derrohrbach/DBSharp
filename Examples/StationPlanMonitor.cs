using DBSharp;
using DBSharp.IRIS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBSharp.Examples
{
    class StationPlanMonitor
    {
        static void Main(string[] args)
        {
            Console.Write("Bitte DS100-Kürzel eingeben: ");
            var searchString = Console.ReadLine();

            //Find the station EVA
            IRISStationRequest isr = new IRISStationRequest(searchString);
            isr.DoRequestAsync().Wait();

            if(isr.Successfull)
            {
                //Get plan for next 2 hours
                IRISPlanRequest ipr1 = new IRISPlanRequest(DateTime.Now, isr.StationData.EVA);
                IRISPlanRequest ipr2 = new IRISPlanRequest(DateTime.Now + new TimeSpan(1, 0, 0), isr.StationData.EVA);
                //Get realtime info
                IRISRealtimeRequest irr = new IRISRealtimeRequest(isr.StationData.EVA, true);

                Task.WaitAll(ipr1.DoRequestAsync(), ipr2.DoRequestAsync(), irr.DoRequestAsync());

                if (ipr1.Successfull && ipr2.Successfull && irr.Successfull)
                {
                    var plan = ipr1.Plan;
                    plan.Append(ipr2.Plan); //Join the plans together
                    irr.FullRequest = false; //Switch to change requests
                    while (true)
                    {
                        if (irr.Successfull)
                        {
                            plan.ApplyChangesets(irr.ConnectionChangesets); //Load new realtime data into plan
                            Console.Clear();
                            PrintPlan(plan, false); //Print plan (The ugly function below :p)
                        }
                        else
                            Console.WriteLine("Fehler beim Aktualisieren!");

                        //Request new realtime info
                        var refreshTask = irr.DoRequestAsync();
                        var waitTask = Task.Delay(5000);
                        Task.WaitAll(refreshTask, waitTask);
                    }
                }
                else
                    Console.WriteLine("Konnte den aktuellen Plan nicht abrufen!");
            }
            else
                Console.WriteLine("Konnte Stationsabfrage nicht durchführen!");
        }

        private static void PrintPlan(DBSharp.StationPlan plan, bool arrival = false)
        {
            Console.WriteLine($"{(arrival ? "Ankunfts " : "Abfahrts")}plan {plan.StationName}");
            var tmpColor = Console.BackgroundColor;
            Console.BackgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = tmpColor;
            Console.Write($"{"Zeit".PadRight(10)}{"Gleis".PadRight(10)}{"Zug".PadRight(12)}{"Endstelle".PadRight(25)}Weitere Informationen".PadRight(Console.BufferWidth));
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = tmpColor;
            //Warning: Beautiful LINQ statement ahead ;)
            foreach (var con in (from con in plan.Connections
                                 let info = arrival ? con.Value.ArrivalInfo : con.Value.DepartureInfo
                                 let realtimeInfo = arrival ? con.Value.RealtimeChangeset?.ArrivalInfo : con.Value.RealtimeChangeset?.DepartureInfo
                                 let estimatedTime = (realtimeInfo?.Time ?? info?.Time)
                                 where info != null && estimatedTime > DateTime.Now - new TimeSpan(0, 0, 20)
                                 orderby info?.Time ascending
                                 select new {
                                     Plan = info,
                                     Realtime = realtimeInfo,
                                     EstimatedTime = estimatedTime,
                                     Value = con.Value
                                 })) 
            {
                var timeDiff = con.EstimatedTime - con.Plan.Time ?? TimeSpan.Zero;
                var timeString = con.Plan.Time?.ToString("HH:mm") ?? "";
                if (timeDiff != TimeSpan.Zero)
                    timeString += $" {timeDiff.TotalMinutes.ToString("+#;-#")}";
                StringBuilder line = new StringBuilder(timeString.PadRight(10));
                var platformToday = con.Realtime?.Platform ?? con.Plan?.Platform;
                if (platformToday != con.Plan.Platform)
                    line.Append((con.Plan.Platform + " > " + platformToday).PadRight(10));
                else
                    line.Append(platformToday.PadRight(10));
                line.Append(con.Value.GetHumanName().PadRight(12));
                line.Append(arrival ? con.Plan.StartPoint.PadRight(25) : con.Plan.EndPoint.PadRight(25));
                if (con?.Realtime?.IsCanceled ?? false)
                    line.Append("Fällt heute aus!");
                else
                {
                    var wings = (con.Realtime?.Wings ?? con.Plan.Wings);
                    if (wings != null)
                    {
                        var wingsConnection = plan.Connections.FirstOrDefault(q => q.Key.StartsWith(wings)).Value;
                        if (wingsConnection == null)
                            line.Append("Mit weiterem Zug. ");
                        else
                            line.Append("Mit " + wingsConnection.GetHumanName() + " nach " + wingsConnection?.DepartureInfo?.EndPoint);
                    }
                    if (timeDiff != TimeSpan.Zero) {
                        var delayReason = con.Realtime?.GetDelayReason();
                        if(!string.IsNullOrWhiteSpace(delayReason))
                        line.Append("Grund: " + delayReason);
                    }
                }
                line.Append(con?.Realtime?.Status ?? "");
                Console.WriteLine(line.ToString().PadRight(Console.BufferWidth - 1));
            }
        }
    }
}
