using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBSharp.IRIS
{
    /// <summary>
    /// Requests various names for a station from IRIS
    /// </summary>
    public class IRISStationRequest : IRISRequest
    {
        public IRISStationRequest(string searchRequest)
        {
            SearchRequest = searchRequest;
        }

        /// <summary>
        /// The search parameter (Most likely the DS100 name)
        /// </summary>
        public string SearchRequest { get; set; }
        
        /// <summary>
        /// The data returned by the query
        /// </summary>
        public StationMetaData StationData { get; protected set; }

        public override string GenerateURL()
        {
            return base.GenerateURL() + IRISConfig.STATION_URL + $"/{SearchRequest}";
        }

        protected override bool ParseResponse(XElement xml)
        {
            if (xml?.Name == "stations")
            {
                var station = xml.Element("station");
                var metaData = new StationMetaData()
                {
                    Name = station?.Attribute("name")?.Value,
                    EVA = station?.Attribute("eva")?.Value,
                    DS100 = station?.Attribute("ds100")?.Value,
                    Meta = station?.Attribute("meta")?.Value
                };
                StationData = metaData;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{nameof(IRISStationRequest)} ({StationData?.Name}={StationData?.EVA})";
        }
    }
}
