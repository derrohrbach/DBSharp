using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace DBSharp
{
    /// <summary>
    /// Holds information about a station
    /// </summary>
    public class StationMetaData : ICloneable
    {
        /// <summary>
        /// name
        /// </summary>
        [BsonIndex]
        public string Name { get; set; }

        /// <summary>
        /// ds100
        /// </summary>
        [BsonIndex]
        public string DS100 { get; set; }

        /// <summary>
        /// eva
        /// </summary>
        [BsonId]
        [BsonIndex]
        public string EVA { get; set; }

        /// <summary>
        /// meta
        /// </summary>
        [BsonIgnore]
        public string Meta { get; set; }

        public object Clone()
        {
            return new StationMetaData() { Name = Name, DS100 = DS100, EVA = EVA, Meta = Meta };
        }
    }
}
