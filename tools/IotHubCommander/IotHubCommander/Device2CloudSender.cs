﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

//using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotHubCommander
{
    internal class Device2CloudSender : IHubModule
    {
        private int commandDelayInSec;
        private string connStr;
        private string evetFile;
        private string templateFile;

        public Device2CloudSender(string connStr, int commandDelayInSec)
        {
            this.connStr = connStr;
            this.commandDelayInSec = commandDelayInSec;
        }

        public Device2CloudSender(string connStr, 
            int commandDelayInSec, 
            string evetFile, 
            string templateFile) : this(connStr, commandDelayInSec)
        {
            this.evetFile = evetFile;
            this.templateFile = templateFile;
        }

        public Task Execute()
        {
            return sendEvent();
        }

        private async Task sendEvent()
        {
            StreamReader readerEventFile = null;
            StreamReader readerTempFile = null;
            try
            {
                readerEventFile = new StreamReader(File.OpenRead(evetFile));
                while (!readerEventFile.EndOfStream)
                {
                    readerTempFile = new StreamReader(File.OpenRead(templateFile));
                    var template = readerTempFile.ReadToEnd();

                    var value = readerEventFile.ReadLine().Split(';');
                    template = template.Replace("@1", value[0]);
                    template = template.Replace("@2", value[1]);
                    template = template.Replace("@3", value[2]);
                    var json = JsonConvert.SerializeObject(template);
                    var message = new Message(Encoding.UTF8.GetBytes(json));
                    message.MessageId = Guid.NewGuid().ToString();
                    DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connStr);

                    await deviceClient.SendEventAsync(message);

                    Console.WriteLine($"{template}{Environment.NewLine} Event has been sent.");
                   // Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex}");
            }
        }
    }
}
