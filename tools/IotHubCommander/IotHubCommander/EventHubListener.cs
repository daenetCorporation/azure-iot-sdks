using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotHubCommander
{
    internal class EventHubListener : IHubModule
    {
        /// <summary>
        /// Goto IotHub portal and copy Shared Access Policy with name 'service'.
        /// </summary>
        private string m_ConnStr = "";

        private string m_Path = "messages/events";

        private EventHubClient m_EventHubClient;

        private string m_ConsumerGroup;

        private DateTime m_StartTime;

        private ManualResetEvent m_Event = new ManualResetEvent(false);

        public EventHubListener(string connStr, string path = "messages/events", DateTime? startTime = null, string consumerGroup = "$Default")
        {
            this.m_Path = path;
            this.m_ConsumerGroup = consumerGroup;
            this.m_ConnStr = connStr;

            if (startTime.HasValue)
                this.m_StartTime = startTime.Value;
            else
                this.m_StartTime = DateTime.UtcNow;

            if (m_Path != null)
                m_EventHubClient = EventHubClient.CreateFromConnectionString(m_ConnStr, m_Path);
            else
                m_EventHubClient = EventHubClient.CreateFromConnectionString(m_ConnStr);
        }

        public Task Execute()
        {
            var t = Task.Run(() =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("Receive messages\n");
            

                var d2cPartitions = m_EventHubClient.GetRuntimeInformation().PartitionIds;

                foreach (string partition in d2cPartitions)
                {
                    receiveMessagesAsync(partition);
                    Console.WriteLine($"Connected to partition {partition}");
                }
            });

            m_Event.WaitOne();

            return t;
        }

        private async Task receiveMessagesAsync(string partition)
        {
            try
            {
                var eventHubReceiver = m_EventHubClient.GetConsumerGroup(m_ConsumerGroup).CreateReceiver(partition, m_StartTime);

                while (true)
                {
                    EventData eventData = await eventHubReceiver.ReceiveAsync();
                    if (eventData == null)
                    {
                        Console.WriteLine($"Partition: {partition}. No events received ...");
                        continue;
                    }

                    string data = Encoding.UTF8.GetString(eventData.GetBytes());
                    //
                    // Different color
                    Console.WriteLine($"{eventData.SystemProperties["DeviceId"]}");
                    Console.WriteLine($"{eventData.SystemProperties["EnqueedAt//"]}");
                    Console.WriteLine(string.Format("Message received. Partition: {0} Data: '{1}'", partition, data));

                    // readProperties(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
            //m_Event.Set();//TODO. Implement cancelation.
        }

        // Not used yet.
        //private static void readProperties(string data)
        //{
        //    try
        //    {
        //        var sensorEvent = JsonConvert.DeserializeObject<dynamic>(data);
        //        Console.WriteLine($"T={sensorEvent.Temperature} Celsius, I={sensorEvent.Current} A, L={sensorEvent.Location}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(data);
        //    }
        //}
    }
}
