using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBSharp.IRIS
{
    public static class IRISHelpers
    {
        private static int YEAR_PREFIX = (DateTime.Today.Year / 100) * 100;

        public static DateTime? ParseDateTime(string datetime)
        {
            if (datetime == null)
                return null;
            var data = new int[datetime.Length / 2];
            for(int i = 0; i < datetime.Length; i+=2) {
                data[i / 2] = int.Parse(datetime.Substring(i, 2));
            }
            return new DateTime(YEAR_PREFIX + data[0], data[1], data[2], data[3], data[4], 0);
        }

        public static string GetDateString(DateTime datetime) => datetime.ToString("yyMMdd");
        public static string GetDateTimeString(DateTime datetime) => datetime.ToString("yyMMddHHmm");
        public static string GetTimeString(DateTime datetime) => datetime.ToString("HHmm");
        public static string GetHourString(DateTime datetime) => datetime.ToString("HH");

        internal static TrainMetaData TrainMetaDataFromXElement(XElement meta)
        {
            if (meta == null)
                return null;
            return new TrainMetaData()
            {
                DistanceClass = ParseDistanceClass(meta.Attribute("f")?.Value),
                TransportClass = meta.Attribute("t")?.Value,
                ModelClass = meta.Attribute("c")?.Value,
                TrainNumber = meta.Attribute("n")?.Value,
                TrainOwner = meta.Attribute("o")?.Value
            };
        }

        internal static DistanceClass ParseDistanceClass(string distanceClass)
        {
            DistanceClass res = 0;
            if (distanceClass != null)
            {
                distanceClass = distanceClass.ToUpperInvariant();
                if (distanceClass.Contains("F"))
                    res |= DistanceClass.F;
                if (distanceClass.Contains("N"))
                    res |= DistanceClass.N;
                if (distanceClass.Contains("S"))
                    res |= DistanceClass.S;
                if (distanceClass.Contains("D"))
                    res |= DistanceClass.D;
            }
            return res;
        }

        internal static ConnectionInfo ConnectionInfoFromXElement(XElement info)
        {
            if (info == null)
                return null;
            return new ConnectionInfo()
            {
                Time = ParseDateTime(info.Attribute("pt")?.Value),
                Platform = info.Attribute("pp")?.Value,
                Route = info.Attribute("ppth")?.Value?.Split('|'),
                LineName = info.Attribute("l")?.Value,
                EndPoint = info.Attribute("pde")?.Value,
                Wings = info.Attribute("wings")?.Value
            };
        }

        internal static RealtimeInfo RealtimeInfoFromXElement(XElement info)
        {
            if (info == null)
                return null;
            return new RealtimeInfo()
            {
                Time = ParseDateTime(info.Attribute("ct")?.Value ?? info.Attribute("pt")?.Value),
                Platform = info.Attribute("cp")?.Value ?? info.Attribute("pp")?.Value,
                Route = info.Attribute("cpth")?.Value?.Split('|') ?? info.Attribute("ppth")?.Value?.Split('|'),
                LineName = info.Attribute("l")?.Value,
                EndPoint = info.Attribute("cde")?.Value ?? info.Attribute("pde")?.Value,
                Status = info.Attribute("cs")?.Value ?? info.Attribute("ps")?.Value,
                StatusSince = ParseDateTime(info.Attribute("clt")?.Value),
                Messages = info.Elements("m").Select(m => InformationMessageFromXElement(m)).Where(m => m != null).ToList()
            };
        }

        internal static InformationMessage InformationMessageFromXElement(XElement m)
        {
            if (m == null || string.IsNullOrWhiteSpace(m.Attribute("id")?.Value))
                return null;
            return new InformationMessage(m.Attribute("id").Value)
            {
                Type = m.Attribute("t")?.Value,
                Time = ParseDateTime(m.Attribute("ts")?.Value),
                Value = m.Attribute("c")?.Value
            };
        }
    }
}
