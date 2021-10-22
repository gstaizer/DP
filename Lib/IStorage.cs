namespace Lib
{
    public interface IStorage
    {
        void Store(string key, string value);
        void Store(string shard, string key, string value);
        string Load(string shard, string key);
        bool IsValueExist(string key, string value);
        void StoreToSet(string shard, string key, string value);
        string GetShardId(string key);
    }
}