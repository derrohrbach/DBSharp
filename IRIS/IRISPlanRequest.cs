using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBSharp.IRIS
{
    /// <summary>
    /// Class to make requests to IRIS for a station plan for the full hour given
    /// </summary>
    public class IRISPlanRequest : IRISRequest
    {
        public IRISPlanRequest(DateTime startTime, string stationEVA)
        {
            StartTime = startTime;
            StationEVA = stationEVA;
        }

        /// <summary>
        /// Hour and day of plan
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Station of plan
        /// </summary>
        public string StationEVA { get; set; }

        /// <summary>
        /// Last recieved plan
        /// </summary>
        public StationPlan Plan { get; protected set; }

        public override string GenerateURL()
        {
            var dateString = IRISHelpers.GetDateString(StartTime);
            var hourString = IRISHelpers.GetHourString(StartTime);
            return base.GenerateURL() + IRISConfig.PLAN_URL + $"/{StationEVA}/{dateString}/{hourString}";
        }

        protected override bool ParseResponse(XElement xml)
        {
            if(xml?.Name == "timetable")
            {
                var stationName = xml.Attribute("station")?.Value;
                if (stationName == null)
                {
                    Plan = null;
                    return !xml.HasElements && !xml.HasAttributes;
                }
                var connections = xml.Elements("s")
                    .Where(e => e.HasElements && e.HasAttributes && !string.IsNullOrWhiteSpace(e.Attribute("id")?.Value))
                    .Select(e => {
                        var con = new TrainConnection(e.Attribute("id").Value);
                        con.TrainMetaData = IRISHelpers.TrainMetaDataFromXElement(e.Element("tl"));
                        con.ArrivalInfo = IRISHelpers.ConnectionInfoFromXElement(e.Element("ar"));
                        con.DepartureInfo = IRISHelpers.ConnectionInfoFromXElement(e.Element("dp"));
                        return con;
                    });
                Plan = new StationPlan(stationName, connections);
                return true;
            }
            return false;
        }
    }
}
