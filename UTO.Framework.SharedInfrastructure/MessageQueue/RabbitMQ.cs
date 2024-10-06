using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using UTO.Framework.Shared.Guards;
using UTO.Framework.Shared.Interfaces;

namespace UTO.Framework.SharedInfrastructure.Queue
{
    public class RabbitMQ : IMessageQueue
    {
        private readonly string _queueName;
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQ(string queueName, IConfiguration configuration)
        {
            ParameterGuard.AgainstNullStringParameter(queueName);
            ParameterGuard.AgainstNullParameter(configuration);

            _queueName = queueName;
            _configuration = configuration;
            _factory = new ConnectionFactory();
            string queuePath = _configuration.GetConfigurationValue("RabbitMQQueuePath");
            _factory.Uri = new Uri(queuePath);
        }

        public bool IsEmpty(string queueName)
        {
            OpenConnection();

            QueueDeclareOk queueDeclareOk = _channel.QueueDeclare(queueName, true, false, false);
            uint count = queueDeclareOk.MessageCount;

            CloseConnection();

            return count > 0 ? false : true;
        }

        public void Enqueue(string strInputMessage)
        {
            OpenConnection();

            _channel.ExchangeDeclare("RightsUAvailsExchange", ExchangeType.Direct, true);
            var bytesInputMessage = Encoding.UTF8.GetBytes(strInputMessage);
            _channel.BasicPublish("RightsUAvailsExchange", _queueName, null, bytesInputMessage);

            CloseConnection();
        }

        public string Dequeue()
        {
            OpenConnection();

            _channel.QueueDeclare(_queueName, true, false, false);
            _channel.QueueBind(_queueName, "RightsUAvailsExchange", _queueName);

            var consumer = new EventingBasicConsumer(_channel);
            BasicGetResult basicGetResult = _channel.BasicGet(_queueName, true);
            string data = String.Empty;
            if (basicGetResult != null)
            {
                data = Encoding.UTF8.GetString(basicGetResult.Body);
            }

            CloseConnection();

            return data;
        }

        private void OpenConnection()
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        private void CloseConnection()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
