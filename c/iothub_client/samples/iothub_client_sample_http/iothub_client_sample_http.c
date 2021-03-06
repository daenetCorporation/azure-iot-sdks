// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#include <stdio.h>
#include <stdlib.h>

/* This sample uses the _LL APIs of iothub_client for example purposes.
   That does not mean that HTTP only works with the _LL APIs.
   Simply changing the using the convenience layer (functions not having _LL)
   and removing calls to _DoWork will yield the same results. */

#ifdef ARDUINO
#include "AzureIoT.h"
#else
#include "iothub_client_ll.h"
#include "iothub_message.h"
#include "crt_abstractions.h"
#include "iothubtransporthttp.h"
#include "platform.h"
#endif

#ifdef MBED_BUILD_TIMESTAMP
#include "certs.h"
#endif // MBED_BUILD_TIMESTAMP

/*String containing Hostname, Device Id & Device Key in the format:             */
/*  "HostName=<host_name>;DeviceId=<device_id>;SharedAccessKey=<device_key>"    */
static const char* connectionString = "[device connection string]";
static int callbackCounter;

DEFINE_ENUM_STRINGS(IOTHUB_CLIENT_CONFIRMATION_RESULT, IOTHUB_CLIENT_CONFIRMATION_RESULT_VALUES);

typedef struct EVENT_INSTANCE_TAG
{
    IOTHUB_MESSAGE_HANDLE messageHandle;
    int messageTrackingId;  // For tracking the messages within the user callback.
} EVENT_INSTANCE;

static IOTHUBMESSAGE_DISPOSITION_RESULT ReceiveMessageCallback(IOTHUB_MESSAGE_HANDLE message, void* userContextCallback)
{
    int* counter = (int*)userContextCallback;
    const char* buffer;
    size_t size;
	MAP_HANDLE mapProperties;

    if (IoTHubMessage_GetByteArray(message, (const unsigned char**)&buffer, &size) != IOTHUB_MESSAGE_OK)
    {
        printf("unable to retrieve the message data\r\n");
    }
    else
    {
        (void)printf("Received Message [%d] with Data: <<<%.*s>>> & Size=%d\r\n", *counter, (int)size, buffer, (int)size);
    }

    // Retrieve properties from the message
    mapProperties = IoTHubMessage_Properties(message);
    if (mapProperties != NULL)
    {
        const char*const* keys;
        const char*const* values;
        size_t propertyCount = 0;
        if (Map_GetInternals(mapProperties, &keys, &values, &propertyCount) == MAP_OK)
        {
            if (propertyCount > 0)
            {
				size_t index;

                printf("Message Properties:\r\n");
                for (index = 0; index < propertyCount; index++)
                {
                    printf("\tKey: %s Value: %s\r\n", keys[index], values[index]);
                }
                printf("\r\n");
            }
        }
    }

    /* Some device specific action code goes here... */
    (*counter)++;
    return IOTHUBMESSAGE_ACCEPTED;
}

static void SendConfirmationCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* userContextCallback)
{
    EVENT_INSTANCE* eventInstance = (EVENT_INSTANCE*)userContextCallback;
    (void)printf("Confirmation[%d] received for message tracking id = %d with result = %s\r\n", callbackCounter, eventInstance->messageTrackingId, ENUM_TO_STRING(IOTHUB_CLIENT_CONFIRMATION_RESULT, result));
    /* Some device specific action code goes here... */
    callbackCounter++;
    IoTHubMessage_Destroy(eventInstance->messageHandle);
}

static char msgText[1024];
static char propText[1024];
#define MESSAGE_COUNT 5

void iothub_client_sample_http_run(void)
{
    IOTHUB_CLIENT_LL_HANDLE iotHubClientHandle;

    EVENT_INSTANCE messages[MESSAGE_COUNT];
    double avgWindSpeed = 10.0;
    int receiveContext = 0;

    srand((unsigned int)time(NULL));

    callbackCounter = 0;

    if (platform_init() != 0)
    {
        printf("Failed to initialize the platform.\r\n");
    }
    else
    {
        (void)printf("Starting the IoTHub client sample HTTP...\r\n");

        if ((iotHubClientHandle = IoTHubClient_LL_CreateFromConnectionString(connectionString, HTTP_Protocol)) == NULL)
        {
            (void)printf("ERROR: iotHubClientHandle is NULL!\r\n");
        }
        else
        {
            unsigned int timeout = 241000;
            // Because it can poll "after 9 seconds" polls will happen effectively // at ~10 seconds.
            // Note that for scalabilty, the default value of minimumPollingTime
            // is 25 minutes. For more information, see:
            // https://azure.microsoft.com/documentation/articles/iot-hub-devguide/#messaging
            unsigned int minimumPollingTime = 9;
            if (IoTHubClient_LL_SetOption(iotHubClientHandle, "timeout", &timeout) != IOTHUB_CLIENT_OK)
            {
                printf("failure to set option \"timeout\"\r\n");
            }

            if (IoTHubClient_LL_SetOption(iotHubClientHandle, "MinimumPollingTime", &minimumPollingTime) != IOTHUB_CLIENT_OK)
            {
                printf("failure to set option \"MinimumPollingTime\"\r\n");
            }

#ifdef MBED_BUILD_TIMESTAMP
            // For mbed add the certificate information
            if (IoTHubClient_LL_SetOption(iotHubClientHandle, "TrustedCerts", certificates) != IOTHUB_CLIENT_OK)
            {
                printf("failure to set option \"TrustedCerts\"\r\n");
            }
#endif // MBED_BUILD_TIMESTAMP

            /* Setting Message call back, so we can receive Commands. */
            if (IoTHubClient_LL_SetMessageCallback(iotHubClientHandle, ReceiveMessageCallback, &receiveContext) != IOTHUB_CLIENT_OK)
            {
                (void)printf("ERROR: IoTHubClient_LL_SetMessageCallback..........FAILED!\r\n");
            }
            else
            {
                int i;

                (void)printf("IoTHubClient_LL_SetMessageCallback...successful.\r\n");

                /* Now that we are ready to receive commands, let's send some messages */
                for (i = 0; i < MESSAGE_COUNT; i++)
                {
                    sprintf_s(msgText, sizeof(msgText), "{\"deviceId\": \"myFirstDevice\",\"windSpeed\": %.2f}", avgWindSpeed + (rand() % 4 + 2));
                    if ((messages[i].messageHandle = IoTHubMessage_CreateFromByteArray((const unsigned char*)msgText, strlen(msgText))) == NULL)
                    {
                        (void)printf("ERROR: iotHubMessageHandle is NULL!\r\n");
                    }
                    else
                    {
                        MAP_HANDLE propMap;

                        messages[i].messageTrackingId = i;

                        propMap = IoTHubMessage_Properties(messages[i].messageHandle);
                        sprintf_s(propText, sizeof(propText), "PropMsg_%d", i);
                        if (Map_AddOrUpdate(propMap, "PropName", propText) != MAP_OK)
                        {
                            (void)printf("ERROR: Map_AddOrUpdate Failed!\r\n");
                        }

                        if (IoTHubClient_LL_SendEventAsync(iotHubClientHandle, messages[i].messageHandle, SendConfirmationCallback, &messages[i]) != IOTHUB_CLIENT_OK)
                        {
                            (void)printf("ERROR: IoTHubClient_LL_SendEventAsync..........FAILED!\r\n");
                        }
                        else
                        {
                            (void)printf("IoTHubClient_LL_SendEventAsync accepted message [%d] for transmission to IoT Hub.\r\n", i);
                        }

                    }
                }
            }

            /* Wait for Commands. */
            while (1)
            {
                IoTHubClient_LL_DoWork(iotHubClientHandle);
            }

            IoTHubClient_LL_Destroy(iotHubClientHandle);
        }
        platform_deinit();
    }
}
