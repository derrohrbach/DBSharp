using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp
{
    /// <summary>
    /// Holds realtime changes to arrival/departure of a connection
    /// </summary>
    public class RealtimeInfo : ConnectionInfo, IMergeable<RealtimeInfo>
    {
        public RealtimeInfo()
        {
            Messages = new List<InformationMessage>();
        }

        /// <summary>
        /// ps/cs = c/a
        /// </summary>
        public virtual string Status { get; set; }

        /// <summary>
        /// clt = [Timestamp]
        /// </summary>
        public virtual DateTime? StatusSince { get; set; }

        /// <summary>
        /// m
        /// </summary>
        public virtual List<InformationMessage> Messages { get; set; }

        /// <summary>
        /// Was connection caneled today?
        /// </summary>
        public virtual bool IsCanceled => Status == "c";

        /// <summary>
        /// Gets the latest delay message string
        /// </summary>
        /// <returns></returns>
        public virtual string GetDelayReason() => Messages.OrderByDescending(m => m.Time).FirstOrDefault(m => m.Type == "d")?.GetHumanString();

        public virtual void MergeChanges(RealtimeInfo changes)
        {
            if (changes != null)
            {
                base.MergeChanges(changes);
                Status = changes.Status ?? Status;
                StatusSince = changes.StatusSince ?? StatusSince;
                if (changes.Messages != null)
                {
                    foreach (var msg in changes.Messages)
                    {
                        var localEquivalent = Messages.Where(m => m.Uid == msg.Uid).FirstOrDefault();
                        if (localEquivalent != null)
                            localEquivalent.MergeChanges(msg);
                        else
                            Messages.Add(msg);
                    }
                }
            }
        }
    }
}
