using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp
{
    public class RealtimeChangeset
    {
        public RealtimeChangeset(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
                throw new ArgumentException("targetId can not be null or empty");
            TargetUid = targetId;
            ConnectionMessages = new List<InformationMessage>();
        }

        public string TargetUid { get; protected set; }

        public TrainMetaData TrainMetaData { get; set; }

        public RealtimeInfo ArrivalInfo { get; set; }

        public RealtimeInfo DepartureInfo { get; set; }

        public List<InformationMessage> ConnectionMessages { get; set; }

        public void MergeWithNewChangeset(RealtimeChangeset changeset)
        {
            if (changeset.TargetUid != this.TargetUid)
                throw new ArgumentException("Cannot merge changesets for two differen targets!");
            if (TrainMetaData == null)
                TrainMetaData = changeset.TrainMetaData;
            else
                TrainMetaData.MergeChanges(changeset?.TrainMetaData);
            if (ArrivalInfo == null)
                ArrivalInfo = changeset.ArrivalInfo;
            else
                ArrivalInfo.MergeChanges(changeset?.ArrivalInfo);
            if (DepartureInfo == null)
                DepartureInfo = changeset.DepartureInfo;
            else
                DepartureInfo.MergeChanges(changeset?.DepartureInfo);
            if (changeset.ConnectionMessages != null)
            {
                foreach (var msg in changeset.ConnectionMessages)
                {
                    var localEquivalent = ConnectionMessages.Where(m => m.Uid == msg.Uid).FirstOrDefault();
                    if (localEquivalent != null)
                        localEquivalent.MergeChanges(msg);
                    else
                        ConnectionMessages.Add(msg);
                }
            }
        }
    }
}
