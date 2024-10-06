namespace UTO.Framework.Shared.Interfaces
{
    public interface IMessageQueue
    {
        bool IsEmpty(string queueName);

        void Enqueue(string strInputMessage);

        string Dequeue();
    }
}
