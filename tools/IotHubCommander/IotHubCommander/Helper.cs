using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotHubCommander
{
    public class Helper
    {

        #region Public Methods
        /// <summary>
        /// Help text
        /// </summary>
        public static void getHelp()
        {
            //--send=event --connStr=öoiwerjökj --cmdDelay=5 --eventFile=TextData1.csv --tempFile=JsonTemplate.txt
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
            helpText.AppendLine($"   --tempFile=<txt formated file, format of event>");
            helpText.AppendLine();
            helpText.AppendLine($"- EXAMPLES -");
            helpText.AppendLine($"--send=event --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvH/5HmRTkD8bR/YbEIU9IM= --cmdDelay=5 --eventFile=TextData1.csv --tempFile=JsonTemplate.txt");
            ////--send=cloud --connstr="" enentfile="" --tempFile=""
            helpText.AppendLine();
            helpText.AppendLine($"- Send Event Cloud to Device-");
            helpText.AppendLine($"-----------------------------");
            helpText.AppendLine($"-----------------------------");
            helpText.AppendLine();
            helpText.AppendLine($"   --send=<\"Cloud\" for sending event>");
            helpText.AppendLine($"   --connStr=<Connection string for sending event>");
            helpText.AppendLine($"   --eventFile=<csv formated file with \";\" separated value>");
            helpText.AppendLine($"   --tempFile=<txt formated file, format of event>");
            helpText.AppendLine();
            helpText.AppendLine($"- EXAMPLES -");
            helpText.AppendLine($"--send=Cloud --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvH/5HmRTkD8bR/YbEIU9IM= --eventFile=TextData1.csv --tempFile=JsonTemplate.txt");
            //--listen=Feedback --connStr=""
            helpText.AppendLine();
            helpText.AppendLine($"- Cloud to Device Feedback -");
            helpText.AppendLine($"----------------------------");
            helpText.AppendLine($"----------------------------");
            helpText.AppendLine();
            helpText.AppendLine($"   --listen=<\"Feedback\" for listening event>");
            helpText.AppendLine($"   --connStr=<Connection string for reading event>");
            helpText.AppendLine($"   --action=<\"Abandon, Commit or None\" for abandon, Commit the message. None is default command and will ask you for abandon or commit.>");
            helpText.AppendLine();
            helpText.AppendLine($"- EXAMPLES -");
            helpText.AppendLine($"--listen=Device --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvHHmRTkD8bR/YbEIU9IM= --action=Abandon");
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

        /// <summary>
        /// Formating the DateTime
        /// </summary>
        /// <param name="startTime">Time in string </param>
        /// <returns></returns>
        public static DateTime getTime(string startTime)
        {

            DateTime time;
            if (startTime.ToLower().Contains('h'))
            {
                int hours = int.Parse(startTime.Substring(0, 2));
                time = DateTime.UtcNow.AddHours(hours);

            }
            else if (startTime.ToLower().Contains('s'))
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
        /// Checks args is empty and help is required or not 
        /// </summary>
        /// <param name="args"> args from main function </param>
        /// <returns></returns>
        public static bool isHelpCall(string[] args)
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
        /// Color line print
        /// </summary>
        /// <param name="message">Text for printing</param>
        /// <param name="lineColor">Text Color</param>
        /// <param name="isNewLine">Print in new line</param>
        public static void WriteLine(string message, ConsoleColor lineColor, bool isNewLine = true)
        {
            Console.ForegroundColor = lineColor;
            if (isNewLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
            Console.ResetColor();
        }
        #endregion
    }
}
