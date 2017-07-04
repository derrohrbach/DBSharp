using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBSharp
{
    /// <summary>
    /// Holds a connection (arrival, departure, meta, realtime)
    /// </summary>
    public class TrainConnection
    {
        private static Regex ShowLineNameOnlyRegex = new Regex(@"^[A-Za-z]+\s*\d.*$");
        private static Regex InsertSpaceRegex = new Regex(@"^([A-Za-z]+)(\d.*)$");

        private TrainMetaData _TrainMetaData;

        public TrainConnection(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentException("UID can not be null or empty");
            Uid = uid;
        }

        public string Uid { get; protected set; }

        public TrainMetaData TrainMetaData
        {
            get { return _TrainMetaData ?? RealtimeChangeset?.TrainMetaData; }
            set { _TrainMetaData = value; }
        }

        public ConnectionInfo ArrivalInfo { get; set; }

        public ConnectionInfo DepartureInfo { get; set; }

        public RealtimeChangeset RealtimeChangeset { get; set; }

        public string GetHumanName()
        {
            if(TrainMetaData != null)
            {
                var lineName = DepartureInfo?.LineName ?? ArrivalInfo?.LineName;
                if (lineName != null && ShowLineNameOnlyRegex.IsMatch(lineName))
                {
                    return InsertSpaceRegex.Replace(lineName, "$1 $2") + $" ({TrainMetaData.ModelClass})";
                }
                else
                {
                    StringBuilder sb = new StringBuilder(TrainMetaData.ModelClass ?? "Other");
                    sb.Append(" ");
                    sb.Append(lineName ?? TrainMetaData.TrainNumber ?? "");
                    return sb.ToString();
                }
            }
            else
            {
                return "N/A";
            }
        }

        /// <summary>
        /// Get ArrivalInfo combined with realtime changes
        /// </summary>
        /// <returns>Realtime ArrivalInfo</returns>
        public ConnectionInfo GetRealtimeArrivalInfo()
        {
            var clone = ArrivalInfo?.Clone() as ConnectionInfo;
            if(RealtimeChangeset?.ArrivalInfo != null)
                clone?.MergeChanges(RealtimeChangeset.ArrivalInfo);
            return clone;
        }

        /// <summary>
        /// Get DepartureInfo combined with realtime changes
        /// </summary>
        /// <returns>Realtime DepartureInfo</returns>
        public ConnectionInfo GetRealtimeDepartureInfo()
        {
            var clone = DepartureInfo?.Clone() as ConnectionInfo;
            if (RealtimeChangeset?.DepartureInfo != null)
                clone?.MergeChanges(RealtimeChangeset.DepartureInfo);
            return clone;
        }

        public DateTime? GetMaxTime(bool includeRealtime = false)
        {
            var arrivalTime = (includeRealtime ? RealtimeChangeset?.ArrivalInfo?.Time : null) ?? ArrivalInfo?.Time;
            var departureTime = (includeRealtime ? RealtimeChangeset?.DepartureInfo?.Time : null) ?? DepartureInfo?.Time;
            if (arrivalTime == null || departureTime > arrivalTime)
                return departureTime;
            return arrivalTime;
        }

        public DateTime? GetMinTime(bool includeRealtime = false)
        {
            var arrivalTime = (includeRealtime ? RealtimeChangeset?.ArrivalInfo?.Time : null) ?? ArrivalInfo?.Time;
            var departureTime = (includeRealtime ? RealtimeChangeset?.DepartureInfo?.Time : null) ?? DepartureInfo?.Time;
            if (arrivalTime == null || departureTime < arrivalTime)
                return departureTime;
            return arrivalTime;
        }

        public bool IsBetween(DateTime startTime, DateTime endTime, bool includeRealtime = false)
        {
            var arrivalTime = (includeRealtime ? RealtimeChangeset?.ArrivalInfo?.Time : null) ?? ArrivalInfo?.Time;
            var departureTime = (includeRealtime ? RealtimeChangeset?.DepartureInfo?.Time : null) ?? DepartureInfo?.Time;
            return (arrivalTime >= startTime || departureTime >= startTime) && (arrivalTime <= endTime || departureTime <= endTime);
        }
    }
}
