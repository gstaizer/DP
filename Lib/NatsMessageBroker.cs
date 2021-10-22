using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client;

namespace Lib
{
    public class NatsMessageBroker : IMessageBroker
    {
        public void Send(string key, string message)
        {
            Task.Factory.StartNew(() =>
            {
                ConnectionFactory cf = new ConnectionFactory();
                IConnection c = cf.CreateConnection();
                byte[] data = Encoding.UTF8.GetBytes(message);
                c.Publish(key, data);
                c.Drain();
                c.Close();
            });
        }
    }
}