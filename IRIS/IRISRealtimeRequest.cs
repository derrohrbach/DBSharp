using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBSharp.IRIS
{
    /// <summary>
    /// Requests realtime information for a train station
    /// </summary>
    public class IRISRealtimeRequest : IRISRequest
    {        
        /// <param name="stationEVA">EVA name of station</param>
        /// <param name="full">Perform a full (initial) request or a change request</param>
        public IRISRealtimeRequest(string stationEVA, bool full = true)
        {
            FullRequest = full;
            StationEVA = stationEVA;
        }

        public bool FullRequest { get; set; }

        public string StationEVA { get; set; }

        public RealtimeChangeset[] ConnectionChangesets { get; protected set; }

        public override string GenerateURL()
        {
            return base.GenerateURL() + (FullRequest ? IRISConfig.FULL_CHANGE_URL : IRISConfig.PARTIAL_CHANGE_URL) + $"/{StationEVA}";
        }

        protected override bool ParseResponse(XElement xml)
        {
            if (xml?.Name == "timetable")
            {
                ConnectionChangesets = xml.Elements("s")
                    .Where(e => e.HasElements && e.HasAttributes && !string.IsNullOrWhiteSpace(e.Attribute("id")?.Value))
                    .Select(e => {
                        var con = new RealtimeChangeset(e.Attribute("id").Value);
                        con.TrainMetaData = IRISHelpers.TrainMetaDataFromXElement(e.Element("tl"));
                        con.ArrivalInfo = IRISHelpers.RealtimeInfoFromXElement(e.Element("ar"));
                        con.DepartureInfo = IRISHelpers.RealtimeInfoFromXElement(e.Element("dp"));
                        con.ConnectionMessages = e.Elements("m").Select(m => IRISHelpers.InformationMessageFromXElement(m)).Where(m => m != null).ToList();
                        return con;
                    }).ToArray();
                return true;
            }
            return false;
        }
    }
}
