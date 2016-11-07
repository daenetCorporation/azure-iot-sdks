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

            Task t = null;

            string connStr;
            if (cmd.TryGet("listen", out connStr))
            {
                string actionStr;
                CommandAction action = CommandAction.None;

                if (cmd.TryGet("action", out actionStr))
                {
                    if (!Enum.TryParse<CommandAction>(actionStr, true, out action))
                    {
                        Console.WriteLine("Failed to parse commit value, will use default: None");
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
                    IHubModule devListener = new Cloud2DeviceListener(connStr, action);
                    t = devListener.Execute();
            }
            else if (1 == 1)
            {
                //
                // Read events form IoTHub or EventHub.
                // -connectTo=EventHub -connStr -startTime=-3h -consumerGroup=$Default
                // -connectTo=IotHub -connStr -startTime=-5d -consumerGroup=abc
                // -connectTo=IotHub -connStr -startTime=now -consumerGroup=abc
            }
            else if (2 == 2)
            {
                //IHubModule module = new EventHubListener(connStr, null/* "messages/events"*/, DateTime.UtcNow.AddDays(-2), "daenet2");
                //  var t = module.Execute();
            }
            else if (3 == 3)
            {
                // Send events
                // -send -connStr connection_string -cmdDelay 5 -eventFile c:\temp\eventdata.csv -templateFile c:\jsontemplate.txt 
                //IHubModule devEmu = new DeviceEventSender(connStr: connStr, commandDelayInSec: 10, evetFile:"TestData.csv", templateFile:"JsonTemplate.txt" );
                //devEmu.Execute();
            }

            if (t != null)
                t.Wait(Timeout.Infinite);
        }
    }
}
        
       



