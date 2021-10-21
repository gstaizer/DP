using NATS.Client;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = connectionMultiplexer.GetDatabase();

            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                LogMessage(id);

                string textKey = "TEXT-" + id;
                string text = db.StringGet(textKey);
                double rank = 0;
                if (text != null) {
                    rank = ((double)text.Count(ch => !char.IsLetter(ch)) / (double)text.Length);
                }

                string rankKey = "RANK-" + id;
                db.StringSet(rankKey, rank.ToString("0.##"));
            });

            void LogMessage(string message)
            {
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fffffff")} - {message}");
            }

            s.Start();

            Console.WriteLine("Consumer started");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
    }
}