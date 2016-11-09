// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.using System;
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
                                SendEventToDevice(cmdConfig);
                                break;
                            case "device":
                                DeviceListener(cmdConfig);
                                break;
                            default:
                                throw new Exception("Command not found. In order to see more details write \"--help\"");
                                break;
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
            Console.WriteLine("Main Function");
            Console.ReadKey();
        }


        private static void SendEventToDevice(CommandLineConfigurationProvider cmdConfig)
        {
            //-send =event -connStr=connection_string -cmdDelay 5 -eventFile c:\temp\eventdata.csv -templateFile c:\jsontemplate.txt
            string connStr = cmdConfig.GetArgument("connStr");
            string cmdDelay = cmdConfig.GetArgument("cmdDelay");
            string eventFile = cmdConfig.GetArgument("eventFile");
            string templateFile = cmdConfig.GetArgument("templateFile");
            int commandDelayInSec = int.Parse(cmdDelay);
            IHubModule devEmu = new DeviceEventSender(connStr, commandDelayInSec, eventFile, templateFile);
            devEmu.Execute();
        }


        private static void EventListener(CommandLineConfigurationProvider cmdConfig, string path = null)
        {
            string connStr = cmdConfig.GetArgument("connStr");
            string startTime = cmdConfig.GetArgument("startTime");
            string consumerGroup = cmdConfig.GetArgument("consumerGroup", false);
            DateTime time = getTime(startTime);
            if (consumerGroup != null)
            {
                IHubModule module = new EventHubListener(connStr, path, time, consumerGroup);
                var t = module.Execute();

                t.Wait(Timeout.Infinite);
            }
            else
            {
                IHubModule module = new EventHubListener(connStr, path, time);
                var t = module.Execute();

                t.Wait(Timeout.Infinite);
            }


        }

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

        private static void DeviceListener(CommandLineConfigurationProvider cmdConfig)
        {
            string connStr = cmdConfig.GetArgument("connStr");
            string autoCommit = cmdConfig.GetArgument("autoCommit", false);

            if (autoCommit != null)
            {
                bool isAutoCommit = bool.Parse(autoCommit);
                IHubModule devListener = new DeviceEventListener(connStr, isAutoCommit);
                var t = devListener.Execute();
            }
            else
            {
                IHubModule devListener = new DeviceEventListener(connStr);
                var t = devListener.Execute();
            }
        }



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

        private static void getHelp()
        {
            //--send=event --connStr=öoiwerjökj --cmdDelay=5 --eventFile=TextData1.csv --templateFile=JsonTemplate.txt
            StringBuilder helpText = new StringBuilder();
            helpText.AppendLine();
            helpText.AppendLine($"- Send Event -");
            helpText.AppendLine($"--------------");
            helpText.AppendLine($"--------------");
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
            helpText.AppendLine($"- Read Command -");
            helpText.AppendLine($"----------------");
            helpText.AppendLine($"----------------");
            helpText.AppendLine();
            helpText.AppendLine($"   --listen=<\"Device\" for listening event>");
            helpText.AppendLine($"   --connStr=<Connection string for reading event>");
            helpText.AppendLine($"   --autoCommit=<\"true or false\" all received events as completed>");
            helpText.AppendLine();
            helpText.AppendLine($"- EXAMPLES -");
            helpText.AppendLine($"--listen=Device --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvHHmRTkD8bR/YbEIU9IM= --autoCommit=true");
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
        
       



