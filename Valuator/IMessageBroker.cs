namespace Valuator
{
    public interface IMessageBroker
    {
        void Send(string key, string message);
    }
}