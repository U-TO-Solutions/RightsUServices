using System;
using System.Messaging;
using UTO.Framework.Shared.Guards;
using UTO.Framework.Shared.Interfaces;

namespace UTO.Framework.SharedInfrastructure.MSMQ
{
    public class MSMQ : IMessageQueue
    {
        private readonly string _queueName;
        private readonly IConfiguration _configuration;

        public MSMQ(string queueName, IConfiguration configuration)
        {
            ParameterGuard.AgainstNullStringParameter(queueName);
            ParameterGuard.AgainstNullParameter(configuration);

            _queueName = queueName;
            _configuration = configuration;
        }

        public bool IsEmpty(string queueName)
        {
            string path = _configuration.GetConfigurationValue("QueuePath") + queueName;

            bool isQueueEmpty = false;
            var myQueue = new MessageQueue(path);
            try
            {
                myQueue.Peek(new TimeSpan(0));
                isQueueEmpty = false;
            }
            catch (MessageQueueException e)
            {
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    isQueueEmpty = true;
                }
            }
            return isQueueEmpty;
        }

        public void Enqueue(string strInputMessage)
        {
            MessageQueue queue = null;
            try
            {
                string path = _configuration.GetConfigurationValue("QueuePath") + _queueName;
                queue = new MessageQueue(path);
                System.Messaging.Message msg = new Message();
                msg.UseDeadLetterQueue = _configuration.GetConfigurationValue("UseDeadLetterQueue", false);
                msg.Recoverable = _configuration.GetConfigurationValue("Recoverable", true);
                msg.UseJournalQueue = _configuration.GetConfigurationValue("UseJournalQueue", false);
                msg.Body = strInputMessage;
                queue.Send(msg);
            }
            catch
            {
                throw;// if exception occurs then fallback mechanism would be used to store messages
            }
            finally
            {
                queue.Dispose();
            }
        }

        public string Dequeue()
        {
            string strOutputMessage = string.Empty;
            MessageQueue queue = null;
            try
            {
                string path = _configuration.GetConfigurationValue("QueuePath") + _queueName;
                queue = new MessageQueue(path);

                queue.Formatter = new XmlMessageFormatter(new Type[]
                {typeof(string)});

                System.Messaging.Message msg = new Message();
                msg = queue.Receive(new
                    TimeSpan(0, 0, 5));

                strOutputMessage = (string)msg.Body;
            }
            catch (MessageQueueException e)
            {
                // Handle no message arriving in the queue.
                if (e.MessageQueueErrorCode ==
                    MessageQueueErrorCode.IOTimeout)
                {
                    Console.WriteLine("No message arrived in queue.");
                }

                // Handle other sources of a MessageQueueException.
            }
            catch
            {
                throw;// if exception occurs then need to set mechanism to wait and retry or notify
            }
            finally
            {
            }
            return strOutputMessage;
        }
    }
}
