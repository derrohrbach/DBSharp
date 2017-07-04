using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp
{
    /// <summary>
    /// Connection plan for a station
    /// </summary>
    public class StationPlan
    {
        private readonly string _StationName;
        private readonly ConcurrentDictionary<string, TrainConnection> _Connections;
        public StationPlan(string stationName, IEnumerable<TrainConnection> connections = null)
        {
            _StationName = stationName ?? throw new ArgumentNullException("StationName can not be null");
            if (connections != null)
                _Connections = new ConcurrentDictionary<string, TrainConnection>(connections.ToDictionary(con => con.Uid));
            else
                _Connections = new ConcurrentDictionary<string, TrainConnection>();
        }

        /// <summary>
        /// Human readable name of station
        /// </summary>
        public string StationName => _StationName;

        /// <summary>
        /// Dictionary of connections stored in this plan
        /// Key = Connection UID
        /// </summary>
        public ConcurrentDictionary<string, TrainConnection> Connections => _Connections;

        public void ApplyChangesets(params RealtimeChangeset[] changesets)
        {
            foreach (var changeset in changesets.Where(c => c?.TargetUid != null))
            {
                if (Connections.ContainsKey(changeset.TargetUid))
                {
                    var connection = Connections[changeset.TargetUid];
                    if (connection.RealtimeChangeset != null)
                        connection.RealtimeChangeset.MergeWithNewChangeset(changeset);
                    else
                        connection.RealtimeChangeset = changeset;
                }
                else
                {
                    //var newConnection = new TrainConnection(changeset.TargetUid);
                    //newConnection.RealtimeChangeset = changeset;
                    //Connections.Add(newConnection.Uid, newConnection);
                }
            }
        }
        
        /// <summary>
        /// Appends more plans to this one (for creating a plan of multiple hours using IRISPlanRequest)
        /// </summary>
        /// <param name="plans"></param>
        public void Append(params StationPlan[] plans)
        {
            foreach(var plan in plans)
            {
                if (plan.StationName != this.StationName)
                    throw new ArgumentException("Cannot join plans of different stations");
                foreach(var con in plan.Connections)
                {
                    Connections[con.Key] = con.Value;
                }
            }
        }
    }
}
