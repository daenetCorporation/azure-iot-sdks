// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubCommander
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string connStr = "";

                // -send -connStr connection_string -cmdDelay 5 -eventFile c:\temp\eventdata.csv -templateFile c:\jsontemplate.txt 
                //DeviceEventSender devEmu = new DeviceEventSender(connStr: connStr, commandDelayInSec: 10, evetFile:"TestData.csv", templateFile:"JsonTemplate.txt" );
                //devEmu.Execute();

                // 
                // Auto commit all received events as completed.
                // -listen -connStr connection_string -autoCommit true 
                //
                // Abandon all received events automatically
                // -listen -connStr connection_string -autoCommit true 
                //
                // Ask user to complete 'c' or abandon 'a' every received event.
                //  -listen -connStr connection_string 
                DeviceEventListener devListener = new DeviceEventListener(connStr: connStr);
                var t = devListener.Execute();


                t.Wait();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        }
    }
}
