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
using Microsoft.Framework.Configuration.CommandLine;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotHubCommander
{
    class Program
    {
        /// <summary>
        /// Main Function 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bool isHelp = isHelpCall(args);
            if (isHelp)
            {
                getHelp();
            }
            else
            {
                CommandLineConfigurationProvider cmdConfig = new CommandLineConfigurationProvider(args);
                try
                {
                    cmdConfig.Load();
                    string connetTo;
                    if (cmdConfig.TryGet("connectTo", out connetTo) || cmdConfig.TryGet("send", out connetTo) || cmdConfig.TryGet("listen", out connetTo))
                    {
                        switch (connetTo.ToLower())
                        {
                            case "eventhub":
                                EventListener(cmdConfig, null);
                                break;

                            case "iothub":
                                EventListener(cmdConfig, "messages/events");
                                break;
                            case "event":
                                SendEventDevice2Cloud(cmdConfig);
                                break;
                            case "device":
                                ReceiveCloud2Device(cmdConfig);
                                break;
                            default:
                                throw new Exception("Command not found. In order to see more details write \"--help\"");
                        }

                    }
                    else
                    {
                        throw new Exception("Command not found. In order to see more details write \"--help\"");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                    Console.ResetColor();
                }
            }
            Console.ReadKey();
        }


        /// <summary>
        /// Send event Device to Cloud
        /// </summary>
        /// <param name="cmdConfig">CommandLineConfigurationProvider</param>
        private static void SendEventDevice2Cloud(CommandLineConfigurationProvider cmdConfig)
        {
            //-send =event -connStr=connection_string -cmdDelay 5 -eventFile c:\temp\eventdata.csv -templateFile c:\jsontemplate.txt
            string connStr = cmdConfig.GetArgument("connStr");
            string cmdDelay = cmdConfig.GetArgument("cmdDelay");
            string eventFile = cmdConfig.GetArgument("eventFile");
            string templateFile = cmdConfig.GetArgument("templateFile");
            int commandDelayInSec = int.Parse(cmdDelay);
            IHubModule devEmu = new Device2CloudSender(connStr, commandDelayInSec, eventFile, templateFile);
            devEmu.Execute();
        }


        /// <summary>
        /// Event Listener for IotHub or EventHub
        /// </summary>
        /// <param name="cmdConfig">CommandLineConfigurationProvider</param>
        /// <param name="path">Path for Event Hub</param>
        private static void EventListener(CommandLineConfigurationProvider cmdConfig, string path = null)
        {
            string connStr = cmdConfig.GetArgument("connStr");
            string startTime = cmdConfig.GetArgument("startTime");
            string consumerGroup = cmdConfig.GetArgument("consumerGroup", false);
            DateTime time = getTime(startTime);
            if (consumerGroup != null)
            {
                IHubModule module = new TelemetryListener(connStr, path, time, consumerGroup);
                var t = module.Execute();

                t.Wait(Timeout.Infinite);
            }
            else
            {
                IHubModule module = new TelemetryListener(connStr, path, time);
                var t = module.Execute();

                t.Wait(Timeout.Infinite);
            }


        }


        /// <summary>
        /// Formating the DateTime
        /// </summary>
        /// <param name="startTime">Time in string </param>
        /// <returns></returns>
        private static DateTime getTime(string startTime)
        {

            DateTime time;
            if (startTime.ToLower().Contains('h'))
            {
                int hours = int.Parse(startTime.Substring(0, 2));
                time = DateTime.UtcNow.AddHours(hours);

            }
            else if(startTime.ToLower().Contains('s'))
            {
                int second = int.Parse(startTime.Substring(0, 2));
                time = DateTime.UtcNow.AddSeconds(second);
            }
            else if (startTime.ToLower().Contains('d'))
            {
                int day = int.Parse(startTime.Substring(0, 2));
                time = DateTime.UtcNow.AddDays(day);
            }
            else if (startTime.ToLower().Contains("now"))
            {
                time = DateTime.Now;
            }
            else
            {
                throw new Exception("Wrong time format.");
            }

            return time;
        }


        /// <summary>
        /// Event Listener Cloud to Device
        /// </summary>
        /// <param name="cmdConfig">CommandLineConfigurationProvider</param>
        private static void ReceiveCloud2Device(CommandLineConfigurationProvider cmdConfig)
        {
            string connStr = cmdConfig.GetArgument("connStr");
            string action = cmdConfig.GetArgument("action",false);
            CommandAction commandAction;
            if(Enum.TryParse<CommandAction>(action,true, out commandAction))
            {
                IHubModule devListener = new Cloud2DeviceListener(connStr, commandAction);
                var t = devListener.Execute();
            }
            else
            {
                commandAction = CommandAction.None;
                IHubModule devListener = new Cloud2DeviceListener(connStr, commandAction);
                var t = devListener.Execute();
            }
            
        }


        /// <summary>
        /// Checks args is empty and help is required or not 
        /// </summary>
        /// <param name="args"> args from main function </param>
        /// <returns></returns>
        private static bool isHelpCall(string[] args)
        {
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("help", StringComparison.OrdinalIgnoreCase) || args[i].StartsWith("--help", StringComparison.OrdinalIgnoreCase) || args[i].StartsWith("-help", StringComparison.OrdinalIgnoreCase) || args[i].StartsWith("/help", StringComparison.OrdinalIgnoreCase))
                    {
                        args[i] = "--help=true";
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Help text
        /// </summary>
        private static void getHelp()
        {
            //--send=event --connStr=öoiwerjökj --cmdDelay=5 --eventFile=TextData1.csv --templateFile=JsonTemplate.txt
            StringBuilder helpText = new StringBuilder();
            helpText.AppendLine();
            helpText.AppendLine($"- Send Event Device to Cloud-");
            helpText.AppendLine($"-----------------------------");
            helpText.AppendLine($"-----------------------------");
            helpText.AppendLine();
            helpText.AppendLine($"   --send=<\"Event\" for sending event>");
            helpText.AppendLine($"   --connStr=<Connection string for sending event>");
            helpText.AppendLine($"   --cmdDelay=<Delay time to listen>");
            helpText.AppendLine($"   --eventFile=<csv formated file with \";\" separated value>");
            helpText.AppendLine($"   --templateFile=<txt formated file, format of event>");
            helpText.AppendLine();
            helpText.AppendLine($"- EXAMPLES -");
            helpText.AppendLine($"--send=event --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvH/5HmRTkD8bR/YbEIU9IM= --cmdDelay=5 --eventFile=TextData1.csv --templateFile=JsonTemplate.txt");
            //--listen=Device --connStr=connection_string --autoCommit=true/false
            helpText.AppendLine();
            helpText.AppendLine($"- Cloud to Device Listener -");
            helpText.AppendLine($"----------------------------");
            helpText.AppendLine($"----------------------------");
            helpText.AppendLine();
            helpText.AppendLine($"   --listen=<\"Device\" for listening event>");
            helpText.AppendLine($"   --connStr=<Connection string for reading event>");
            helpText.AppendLine($"   --action=<\"Abandon, Commit or None\" for abandon, Commit the message. None is default command and will ask you for abandon or commit.>");
            helpText.AppendLine();
            helpText.AppendLine($"- EXAMPLES -");
            helpText.AppendLine($"--listen=Device --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvHHmRTkD8bR/YbEIU9IM= --action=Abandon");
            helpText.AppendLine($"--listen=Device --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvHHmRTkD8bR/YbEIU9IM= --action=Commit");
            //-connectTo=EventHub -connStr=oadölfj -startTime=-3h -consumerGroup=$Default
            helpText.AppendLine();
            helpText.AppendLine($"- Read events form IoTHub or EventHub. -");
            helpText.AppendLine($"----------------------------------------");
            helpText.AppendLine($"----------------------------------------");
            helpText.AppendLine();
            helpText.AppendLine($"   --connectTo=<\"EventHub or IotHub\" for connect with>");
            helpText.AppendLine($"   --connStr=<Connection string for reading event>");
            helpText.AppendLine($"   --startTime=<Starting for reading for event that could be hour, day and now, otherwise it will throw an error \"Wrong time format\">");
            helpText.AppendLine($"   --consumerGroup=<Your consumer group name. It is not mandatory, it will take \"$Default\" consumer group name>");
            helpText.AppendLine();
            helpText.AppendLine($"- EXAMPLES -");
            helpText.AppendLine($"--connectTo=EventHub --connStr=Endpoint=sb://sonethig-myevent-test.servicebus.windows.net/;SharedAccessKeyName=ReaderPolicy;SharedAccessKey=8AKA52124IVqj5eabciWz99UJWpDpQLQzwyLoWVKOTg=;EntityPath=abc -startTime=-3h -consumerGroup=abc");
            helpText.AppendLine($"--connectTo=EventHub --connStr=Endpoint=sb://sonethig-myevent-test.servicebus.windows.net/;SharedAccessKeyName=ReaderPolicy;SharedAccessKey=8AKA52124IVqj5eabciWz99UJWpDpQLQzwyLoWVKOTg=;EntityPath=abc -startTime=-3d -consumerGroup=abc");
            helpText.AppendLine($"--connectTo=EventHub --connStr=Endpoint=sb://sonethig-myevent-test.servicebus.windows.net/;SharedAccessKeyName=ReaderPolicy;SharedAccessKey=8AKA52124IVqj5eabciWz99UJWpDpQLQzwyLoWVKOTg=;EntityPath=abc -startTime=-3s -consumerGroup=abc");
            helpText.AppendLine($"--connectTo=EventHub --connStr=Endpoint=sb://sonethig-myevent-test.servicebus.windows.net/;SharedAccessKeyName=ReaderPolicy;SharedAccessKey=8AKA52124IVqj5eabciWz99UJWpDpQLQzwyLoWVKOTg=;EntityPath=abc -startTime=now -consumerGroup=abc");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(helpText.ToString());
            Console.ResetColor();

        }

    }
}
        
       



