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
            CommandLineConfigurationProvider cmd = new CommandLineConfigurationProvider(args);
            cmd.Load();

            string connStr;
            if (cmd.TryGet("listen", out connStr))
            {
                string actionStr;
                CommandAction action = CommandAction.None;

                if (cmd.TryGet("action", out actionStr))
                {
                    if (!Enum.TryParse<CommandAction>(actionStr, true, out action))
                    {
                        Console.WriteLine("Failed to parse commit value, will use None as default");
                        action = CommandAction.None;
                    }
                    string connection_string;
                    if (cmd.TryGet("listen", out connection_string))
                    {
                        Console.WriteLine("Enter c to complete or a to abondon the received Events");
                        string userInput = Console.ReadLine();
                        if (userInput == "c")
                        {
                            action = CommandAction.Commit;
                        }
                        else if (userInput == "a")
                        {
                           
                            action =  CommandAction.Abandon;
                        }
                        else
                        {
                            action = CommandAction.None;
                        }
                    }
                }
                    IHubModule devListener = new DeviceEventListener(connStr, action);
                    var t = devListener.Execute();
                    // Send events
                    // -send -connStr connection_string -cmdDelay 5 -eventFile c:\temp\eventdata.csv -templateFile c:\jsontemplate.txt 
                    //IHubModule devEmu = new DeviceEventSender(connStr: connStr, commandDelayInSec: 10, evetFile:"TestData.csv", templateFile:"JsonTemplate.txt" );
                    //devEmu.Execute();

                    // 
                    // Read commands
                    // 
                    // Auto commit all received events as completed.
                    // -listen -connStr connection_string -autoCommit true 
                    ///--listen=conStr --autoCommit=true/false
                    // Abandon all received events automatically
                    // -listen -connStr connection_string -autoAbandon true 

                    // -listen=constr --action=commit
                    // -listen=constr --action=abandon
                    // -listen=constr --action=none
                    //
                    // Ask user to complete 'c' or abandon 'a' every received event.
                    //  -listen -connStr connection_string 


                    //
                    // Read events form IoTHub or EventHub.
                    // -connectTo=EventHub -connStr -startTime=-3h -consumerGroup=$Default
                    // -connectTo=IotHub -connStr -startTime=-5d -consumerGroup=abc
                    // -connectTo=IotHub -connStr -startTime=now -consumerGroup=abc

                    //IHubModule devListener = new DeviceEventListener(connStr: connStr);
                    //var t = devListener.Execute();

                    //IHubModule module = new EventHubListener(connStr, null/* "messages/events"*/, DateTime.UtcNow.AddDays(-2), "daenet2");
                    //  var t = module.Execute();

                    t.Wait(Timeout.Infinite);
                
            }

        }

        // var receivedEvents = 

        private static void SendEvent(CommandLineConfigurationProvider cmdConfig)
        {
            //-send =event -connStr=connection_string -cmdDelay 5 -eventFile c:\temp\eventdata.csv -templateFile c:\jsontemplate.txt
            string connStr;
            if (cmdConfig.TryGet("connStr", out connStr))
            {
                string cmdDelay;
                if (cmdConfig.TryGet("cmdDelay", out cmdDelay))
                {
                    string eventFile;
                    if (cmdConfig.TryGet("eventFile", out eventFile))
                    {
                        string templateFile;
                        if (cmdConfig.TryGet("templateFile", out templateFile))
                        {
                            int commandDelayInSec = int.Parse(cmdDelay);
                            IHubModule devEmu = new DeviceEventSender(connStr, commandDelayInSec, eventFile, templateFile);
                            devEmu.Execute();
                        }
                        else
                        {
                            throw new Exception("\"--templateFile\" command not found.");
                        }
                    }
                    else
                    {
                        throw new Exception("\"--eventFile\" command not found.");
                    }
                }
                else
                {
                    throw new Exception("\"--cmdDelay\" command not found.");
                }
            }
            else
            {
                throw new Exception("\"--connStr\" command not found.");
            }
        }
    }
}
        
       



