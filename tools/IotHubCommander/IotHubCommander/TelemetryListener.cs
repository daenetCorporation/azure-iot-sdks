//=======================================================================================
// Copyright © daenet GmbH Frankfurt am Main
//
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================
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
    /// <summary>
    /// Event listener from IotHub or EventHub
    /// </summary>
    internal class TelemetryListener : IHubModule
    {
        #region Member Variables
        /// <summary>
        /// Goto IotHub portal and copy Shared Access Policy with name 'service'.
        /// </summary>
        private string m_ConnStr = "";
        private string m_ConsumerGroup;
        private string m_Path = "messages/events";

        private EventHubClient m_EventHubClient;
        private DateTime m_StartTime;
        private ManualResetEvent m_Event = new ManualResetEvent(false);

        #endregion

        #region Public Methods 

        /// <summary>
        /// Listener for event hub
        /// </summary>
        /// <param name="connStr">Connection string</param>
        /// <param name="path">The Path to the Event Hub </param>
        /// <param name="startTime"></param>
        /// <param name="consumerGroup"></param>
        public TelemetryListener(string connStr, string path = "messages/events", DateTime? startTime = null, string consumerGroup = "$Default")
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


        /// <summary>
        /// Execute the Command
        /// </summary>
        /// <returns></returns>
        public Task Execute()
        {
            var t = Task.Run(() =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("Receive messages\n");            

                var d2cPartitions = m_EventHubClient.GetRuntimeInformation().PartitionIds;

                foreach (string partition in d2cPartitions)
                {
                    receiveMessagesAsync(partition).Wait();
                    Console.WriteLine($"Connected to partition {partition}");
                }
            });

            //m_Event.WaitOne();

            return t;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Receiver from IotHub or EventHub
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        private async Task receiveMessagesAsync(string partition)
        {
            try
            {
                var eventHubReceiver = m_EventHubClient.GetConsumerGroup(m_ConsumerGroup).CreateReceiver(partition, m_StartTime);
                int count = 1;
                while (true)
                {
                    EventData eventData = await eventHubReceiver.ReceiveAsync();
                    if (eventData == null)
                    {
                        Console.WriteLine($"Partition: {partition}. No events received ...");
                        continue;
                    }

                    string data = Encoding.UTF8.GetString(eventData.GetBytes());

                    StringBuilder stBuider = new StringBuilder();
                    stBuider.Append($"x-opt-sequence-number : {eventData.SystemProperties["x-opt-sequence-number"]}");
                    stBuider.Append($"x-opt-offset: {eventData.SystemProperties["x-opt-offset"]}");
                    stBuider.Append($"x-opt-enqueued-time: {eventData.SystemProperties["x-opt-enqueued-time"]}");
                    stBuider.Append($"Message received. Partition: {partition} Data: '{data}'");
                    //
                    // Different color
                    if (count % 2 == 0)
                    {
                        Helper.WriteLine(stBuider.ToString(), ConsoleColor.Blue);
                    }
                    else
                    {
                        Helper.WriteLine(stBuider.ToString(), ConsoleColor.DarkBlue);
                    }
                    count++;
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
        #endregion
    }
}
