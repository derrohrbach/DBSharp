using DBSharp.IRIS;
using DBSharp.StationSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp
{
    /// <summary>
    /// Provides information about the arrival/departure of a connection
    /// </summary>
    public class ConnectionInfo : IMergeable<ConnectionInfo>, ICloneable
    {
        protected string _EndPoint;
        private string[] _CachedEVARoute;
        private string[] _Route;

        /// <summary>
        /// pt/ct = [Timestamp]
        /// </summary>
        public virtual DateTime? Time { get; set; }

        /// <summary>
        /// l = RB40/1/...
        /// </summary>
        public virtual string LineName { get; set; }

        /// <summary>
        /// pp/cp = 1/4/...
        /// </summary>
        public virtual string Platform { get; set; }

        /// <summary>
        /// wings = [Start of Connection Uid]
        /// </summary>
        public virtual string Wings { get; set; }

        /// <summary>
        /// ppth/cpth = [Station]|[Station]|...
        /// </summary>
        public virtual string[] Route
        {
            get { return _Route; }
            set
            {
                _Route = value;
                _CachedEVARoute = null;
            }
        }

        /// <summary>
        /// pde/cde = [Station]
        /// Similar to StartPoint. Only difference is the way info is 
        /// interpolated out of the route if no endpoint was specified
        /// (Use this for departures)
        /// </summary>
        public virtual string EndPoint {
            get { return _EndPoint ?? Route?.LastOrDefault(); }
            set { _EndPoint = value; }
        }
        
        /// <summary>
        /// pde/cde = [Station]
        /// Similar to EndPoint. Only difference is the way info is 
        /// interpolated out of the route if no endpoint was specified 
        /// (Use this for arrivals)
        /// </summary>
        public virtual string StartPoint
        {
            get { return _EndPoint ?? Route?.FirstOrDefault(); }
            set { _EndPoint = value; }
        }

        public virtual void MergeChanges(ConnectionInfo changes)
        {
            Time = changes.Time ?? Time;
            LineName = changes.LineName ?? LineName;
            Platform = changes.Platform ?? Platform;
            Route = changes.Route ?? Route;
            Wings = changes.Wings ?? Wings;
            _EndPoint = changes._EndPoint ?? _EndPoint;
        }

        /// <summary>
        /// Gets the route with each stop as EVA value and not as name
        /// </summary>
        /// <param name="searchDatabase">Optional. If specified uses the given database to make station requests. Vastly increases performance!</param>
        /// <returns>Route with each stop as EVA value</returns>
        public async Task<string[]> GetEVARouteAsync(StationSearchDatabase searchDatabase = null)
        {
            if (_CachedEVARoute != null)
                return _CachedEVARoute;
            var route = Route;
            var newEVARoute = new string[route.Length];
            for (int i = 0; i < newEVARoute.Length; i++)
            {
                if (searchDatabase == null)
                {
                    var request = new IRISStationRequest(route[i]);
                    await request.DoRequestAsync();
                    if (request.Successfull)
                        newEVARoute[i] = request.StationData.EVA;
                    else
                        newEVARoute[i] = null;
                }
                else
                    newEVARoute[i] = await searchDatabase.FindOrFetchEVAAsync(route[i]);
            }
            _CachedEVARoute = newEVARoute;
            return newEVARoute;
        }

        public virtual object Clone() => new ConnectionInfo() { Time = Time, LineName = LineName, Platform = Platform, Route = Route, Wings = Wings, _EndPoint = _EndPoint };
    }
}
