using DBSharp;
using DBSharp.IRIS;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBSharp.StationSearch
{
    public class StationSearchDatabase : IDisposable
    {
        public const string DEFAULT_COLLECTION_NAME = "stations";

        private static Regex _DeleteChars = new Regex(@"(?:\W|-|_|\(|\)|,)+");
        private LiteDatabase _DB;
        private LiteCollection<StationMetaData> _Collection;

        public StationSearchDatabase(LiteDatabase db, string collectionName = DEFAULT_COLLECTION_NAME)
        {
            _DB = db;
            _Collection = _DB.GetCollection<StationMetaData>(collectionName);
        }

        public StationSearchDatabase(string path = "./stations.litedb", string collectionName = DEFAULT_COLLECTION_NAME)
            : this(new LiteDatabase(path), collectionName)
        { }

        public long Count() => _Collection.LongCount();

        public string FindEVA(string query)
        {
            query = SimplifyEntry(query);
            var candidates = _Collection.Find(s => s.Name.StartsWith(query) || s.EVA == query || s.DS100 == query);
            return (candidates.FirstOrDefault(s => s.EVA == query)
                ?? candidates.FirstOrDefault(s => s.DS100 == query)
                ?? candidates.OrderBy(s => s.Name.Length).FirstOrDefault(s => s.Name.StartsWith(query)))?.EVA;
        }

        public async Task<string> FindOrFetchEVAAsync(string query)
        {
            var res = FindEVA(query);
            if (res == null)
            {
                IRISStationRequest stationRequest = new IRISStationRequest(query);
                try
                {
                    await stationRequest.DoRequestAsync();
                    if (stationRequest.Successfull)
                    {
                        res = stationRequest.StationData?.EVA;
                        SetStation(stationRequest.StationData);
                    }
                }
                catch { }
            }
            return res;
        }

        public void Write()
        {
            _DB.Engine.Commit();
        }

        public void SetStation(StationMetaData data)
        {
            if(!String.IsNullOrEmpty(data?.EVA)) {
                var insertData = (StationMetaData)data.Clone();
                SimplifyStationMeta(insertData);
                _Collection.Upsert(insertData);
            }
        }

        private void SimplifyStationMeta(StationMetaData data) {
            data.EVA = SimplifyEntry(data.EVA);
            data.Name = SimplifyEntry(data.Name);
            data.DS100 = SimplifyEntry(data.DS100);
        }

        private string SimplifyEntry(string query)
        {
            if(query == null)
                return null;
            return _DeleteChars.Replace(query.Trim().ToUpperInvariant(), "");
        }

        public void Dispose()
        {
            _DB.Dispose();
        }
    }
}
