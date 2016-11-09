using Microsoft.Framework.Configuration.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubCommander
{
    public static class Extensions
    {
        public static string GetArgument(this CommandLineConfigurationProvider provider, string name, bool isMandatory = true)
        {
            string returnValue;

            if (provider.TryGet(name, out returnValue))
                return returnValue;
            else if (isMandatory)
                throw new ArgumentException($"'--{name}' command not found.In order to see more details '--help'");
            else
                return default(String);
        }
    }
}
