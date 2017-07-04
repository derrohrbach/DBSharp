using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp
{
    [Flags]
    public enum DistanceClass : byte
    {
        NoneOrUnknown = byte.MinValue,
        D = 1,
        S = 2,
        N = 4,
        F = 8,
        All = byte.MaxValue
    }

    /// <summary>
    /// Holds information about the Train
    /// </summary>
    public class TrainMetaData : IMergeable<TrainMetaData>
    {
        /// <summary>
        /// f = D/N/S/F
        /// </summary>
        public DistanceClass DistanceClass { get; set; } = DistanceClass.NoneOrUnknown;

        /// <summary>
        /// t = p
        /// </summary>
        public string TransportClass { get; set; }

        /// <summary>
        /// c = S/ICE/ABR/...
        /// </summary>
        public string ModelClass { get; set; }

        /// <summary>
        /// n = [NUMBER/NAME]
        /// </summary>
        public string TrainNumber { get; set; }

        /// <summary>
        /// o = 03/80/R2/...
        /// </summary>
        public string TrainOwner { get; set; }

        public virtual void MergeChanges(TrainMetaData changes)
        {
            if (changes != null)
            {
                DistanceClass = (changes.DistanceClass == DistanceClass.NoneOrUnknown ? DistanceClass : changes.DistanceClass);
                ModelClass = changes.ModelClass ?? ModelClass;
                TrainNumber = changes.TrainNumber ?? TrainNumber;
                TransportClass = changes.TransportClass ?? TransportClass;
                TrainOwner = changes.TrainOwner ?? TrainOwner;
            }
        }
    }
}
