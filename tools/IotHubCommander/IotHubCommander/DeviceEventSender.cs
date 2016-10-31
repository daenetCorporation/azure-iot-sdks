// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubCommander
{
    internal class DeviceEventSender : IHubModule
    {
        private int commandDelayInSec;
        private string connStr;
        private string evetFile;
        private string templateFile;

        public DeviceEventSender(string connStr, int commandDelayInSec)
        {
            this.connStr = connStr;
            this.commandDelayInSec = commandDelayInSec;
        }

        public DeviceEventSender(string connStr, 
            int commandDelayInSec, 
            string evetFile, 
            string templateFile) : this(connStr, commandDelayInSec)
        {
            this.evetFile = evetFile;
            this.templateFile = templateFile;
        }

        public Task Execute()
        {
            throw new NotImplementedException();
        }
    }
}
