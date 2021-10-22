using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly IDatabase _db;
        private string host = "localhost";
        private int  port = 6379;

        public RedisStorage() {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(host);
            _db = connectionMultiplexer.GetDatabase();
        }

        public string Load(string key)
        {
            return _db.StringGet(key);
        }

        public void Store(string key, string value)
        {
            _db.StringSet(key, value);
        }

        public bool IsValueExist(string value)
        {
            var keys = ConnectionMultiplexer.Connect(host + ",allowAdmin=true").GetServer(host + ":" + port).Keys(pattern: "TEXT-*");
            foreach (var key in keys) 
            {
                string _value = Load(key);
                if (_value == value) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}