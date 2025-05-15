# Azure Service Bus Emulator Setup (Windows & Mac)

## Overview

Use [Azure Service Bus Emulator](https://github.com/Azure/azure-service-bus-emulator) for local development and testing without connecting to Azure.

---

## Prerequisites

- [.NET 8 SDK or later](https://dotnet.microsoft.com/download)
- [Podman](https://podman.io/getting-started) (for containerized setup)

---

## Setup Azure Service Bus emulator using Podman

1. Pull the image:
    ```sh
    podman pull mcr.microsoft.com/azure-messaging/servicebus-emulator
    ```

## Run the Azure Service Bus emulator

- To start the emulator, you should supply configuration for the entities you want to use. Save the below config locally as [Config.json](./Config.json)

- Run below command to run emulator

    ```sh
    podman compose -f docker-compose.yml up -d
    ```

---

## Configuration

- Update your application connection string to point to the emulator endpoint (see emulator docs for details).
- Default endpoints:
  - AMQP: `amqp://localhost:5672`
  - Management: `http://localhost:9354`

---

## Example Connection String in `local.settings.json`

Add the following to your `local.settings.json` file under the `Values` section:

```json
{
    "Values": {
        "ServiceBusConnectionString": "Endpoint=sb://localhost/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=localEmulatorKey"
    }
}
```

Replace `"ServiceBusConnectionString"` with your application's expected key if different.

---

## References

- [Azure Service Bus Emulator GitHub](https://github.com/Azure/azure-service-bus-emulator)
- [Official Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)
