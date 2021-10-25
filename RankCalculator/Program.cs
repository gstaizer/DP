using Lib;
using NATS.Client;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RankCalculator
{
    class Program
    {
        private static double CalculateRank(string text) 
        {
            double rank = 0;
            if (text != null) {
                rank = ((double)text.Count(ch => !char.IsLetter(ch)) / (double)text.Length);
            }
            return rank;
        }

        private static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fffffff")} - {message}");
        }

        private static void PublishRankEvent(IMessageBroker messageBroker, string id, string rank)
        {
            EventContainer eventData = new EventContainer { Name = "rank_calculator.rank_calculated", Id = id, Value = rank };
            messageBroker.Send("events", JsonSerializer.Serialize(eventData));
        }

        static void Main(string[] args)
        {
            IMessageBroker messageBroker = new NatsMessageBroker();
            RedisStorage storage = new RedisStorage();

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                LogMessage(id);

                string shardId = storage.GetShardId(id);

                string text = storage.Load(shardId, Constants.TextKey + id);
                string rank = CalculateRank(text).ToString("0.##");
                
                storage.Store(shardId, Constants.RankKey + id, rank);
                PublishRankEvent(messageBroker, id, rank);
            });

            s.Start();

            Console.WriteLine("Consumer started");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
    }
}