using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client;

namespace Valuator
{
    public class NatsMessageBroker : IMessageBroker
    {
        private async Task ProduceAsync(string key, string message)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                c.Publish(key, data);
                c.Drain();
                c.Close();
            }
        }
        public async void Send(string key, string message)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => ProduceAsync(key, message), cts.Token);
        }
    }
}