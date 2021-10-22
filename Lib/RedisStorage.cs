using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Lib
{
    public class RedisStorage : IStorage
    {
        private readonly IDatabase _db;
        private readonly Dictionary<string, IDatabase> _conns;
        private string host = "localhost";

        public RedisStorage() {
            _conns = new Dictionary<string, IDatabase>();
            _conns.Add(Constants.SHARD_RUS, 
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User)).GetDatabase());
            _conns.Add(Constants.SHARD_EU, 
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User)).GetDatabase());
            _conns.Add(Constants.SHARD_OTHER, 
            ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User)).GetDatabase());

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(host);
            _db = connectionMultiplexer.GetDatabase();
        }

        private IDatabase GetConnection(string shard)
        {
            return _conns[shard];
        }

        public string GetShardId(string key)
        {
            return _db.StringGet(Constants.ShardKey + key);
        }

        public string Load(string shard, string key)
        {
            return GetConnection(shard).StringGet(key);
        }

        public void Store(string shard, string key, string value)
        {
            GetConnection(shard).StringSet(key, value);
        }

         public void Store(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public void StoreToSet(string shard, string key, string value)
        {
            GetConnection(shard).SetAdd(key, value);
        }

        public bool IsValueExist(string key, string value)
        {
            foreach (KeyValuePair<string, IDatabase> _conn in _conns)
            {
                if (_conn.Value.SetContains(key, value))
                {
                    return true;
                }
            }
            return false;        
        }
    }
}