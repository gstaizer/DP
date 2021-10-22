using System;

namespace Lib
{
    public class Constants
    {
        public static readonly string TextKey = "TEXT-";
        public static readonly string SimilarityKey = "SIMILARITY-";
        public static readonly string RankKey = "RANK-";
        public static readonly string ShardKey = "SHARD-";
        public static readonly string TextSetKey = "TEXT-SET-";
        
        public static readonly string SHARD_RUS = "RUS";
        public static readonly string SHARD_EU = "EU";
        public static readonly string SHARD_OTHER = "OTHER";   

        public static readonly string[] Countries = new string[] {
            "Russia", 
            "France",
            "Germany",
            "USA",
            "India"
        };
    }
}
