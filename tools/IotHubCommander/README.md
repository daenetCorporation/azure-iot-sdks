*## Welcome to IotHub Commander   :)

This is .Net Console Application, which send event to Device to Cloud, Cloud to Device and read events from IotHub or EventHub.

### Build Environment

* Create an Environment Variable.

![Alt text](https://github.com/daenetCorporation/azure-iot-sdks/blob/develop/tools/IotHubCommander/Environment%20Variable.PNG?raw=true "Environment Variable creating")

* Run command promt and write commands.

### Send event Device to Cloud 

To send event Device to Cloud, run CommandLien promt and write command -
* --send=Event "for sending event to Cloud from Device".
* --connStr="Connection string for sending event, device connection string".
* --cmdDelay="Delay time to listen".
* --eventFile="csv formated file path with ";" separated value".
* --tempFile="Json formated text file path, format of event".
Example -
* IotHubCommander.exe --send=event --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvH/5HmRTkD8bR/YbEIU9IM= --cmdDelay=5 --eventFile=TextData1.csv --tempFile=JsonTemplate.txt

### Send Event Cloud to Device

To send event Cloud to Device, run CommandLine promt and write command -
* --send=Cloud "for sending event to Device from Cloud".
* --connStr="Connection string for sending event, Service connection string".
* --deviceId="Device ID".
* --eventFile="csv formated file path with ";" separated value".
* --tempFile="Json formated text file path, format of event".
Example -
* IotHubCommander.exe --send=Cloud --connStr=HostName=protadapter-testing.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=J95WJrRRbvZbSAV66CX/MKj66IJ7YnqvaqXSmIg5lY4= --deviceId=daenet-damir --eventFile=C:\GitHub\Azure-Iot-Sdks\tools\IotHubCommander\IotHubCommander\TextData2.csv --tempFile=C:\GitHub\Azure-Iot-Sdks\tools\IotHubCommander\IotHubCommander\JsonTemplate2.txt

### Cloud to Device Listener

You need to run two CommandLine promt, sender and receiver. For sender run Send evet Device to Cloud and for receiver write command -
 * --listen=Device for listening event
 * --connStr="Connection string for reading event"
 * --action="Abandon, Commit or None", for abandon, Commit the message. None is default command and will ask you for abandon or commit.

Example -

* IotHubCommander.exe --listen=Device --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvHHmRTkD8bR/YbEIU9IM= --action=Abandon
* IotHubCommander.exe --listen=Device --connStr=HostName=something.azure-devices.net;DeviceId=123456;SharedAccessKey=2CFsCmqyHvHHmRTkD8bR/YbEIU9IM= --action=Commit


### Read events from IotHub or EventHub
