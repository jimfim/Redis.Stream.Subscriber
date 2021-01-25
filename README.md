# Redis.Stream.Subscriber
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Build Status][build-shield]][build-status]
[![MIT License][license-shield]][license-url]

## Description
The StackExchange.Redis client does not provide the functionality to subscribe a stream without constant polling. 

https://github.com/StackExchange/StackExchange.Redis/issues/1155

The purpose of this library is to provide a more "EventStore" style blocking stream client for redis and where possible/feasible we will attempt to keep this redis client as similar as possible to the latest EventStore client implementation

This project is concerned only with reading streams. It provides no functionality to write to redis at all. If you require this functionality we recommend using the **StackExchange.Redis** package

## Table of Contents
* [Features](#features)
* [Installation](#installation)
* [Methodology](#methodology)
* [Usage](#usage)
* [Building](#building)
* [License](#license)
* [Related Projects](#related-projects)

## Features
- [X] Read stream forward
- [ ] Read stream backwards

## Installation
Install via nuget
```bash
dotnet add PROJECT package Redis.Stream.Subscriber
```

## Methodology

The primary purpose of this library is to avoid  polling redis for stream updates. 

To achieve this we instead use a direct socket connection to the redis server using the dotnet TcpClient, we write the Stream commands directly to redis and then listen for data to be transmitted back on the same socket.
The thread is blocked listening on the socket until there is data consume, at which point it is read and serialized to a "StreamEntry" object and made available to you to process however you see fit. 

## Usage

Initialize connection with redis
```c#
var connection = new RedisConnection();
var connect = connection.Connect(new RedisStreamSettings
{
    host = "localhost",
    Port = 6379
});
```

Start receiving stream entries
```c#
uint startingIndex = 0;
var entries = connect.ReadStreamAsync("mystream", startingIndex);
await foreach (var entry in entries)
{
    await Console.Out.WriteLineAsync(entry.Data);
}
```

## Building
### Windows / Linux / macOS
**Prerequisites**
- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

or
**Docker**
```bash 
docker run -it -v .:/app/:z -w /app mcr.microsoft.com/dotnet/sdk:3.1 bash
```

### Redis
this repo contains a docker-compose file with a sample redis setup you can use to run the example clients in this solution

```bash
docker-compose up 
```

you can then connect to redis-commander to view your stream http://localhost:8081/

### Running the Sample application
Build the project, there are no external dependencies so this should be fast
```c#
dotnet build Redis.Stream.Subscriber.sln
```

Run the stream listener project that will subscribe to a stream
```c#
dotnet run --project src/Redis.Stream.Subscriber.Listener/Redis.Stream.Subscriber.Listener.csproj
```

the publisher application uses the StackExchange.Redis library to publish changes which the subscriber above project will listen
```c#
dotnet run --project ./src/Redis.Stream.Subscriber.Publisher/Redis.Stream.Subscriber.Publisher.csproj
```

## License

Distributed under the MIT License. See `LICENSE` for more information.


## Related Projects
* [EventNet](https://github.com/jimfim/EventNet) - Library to manage an Aggregate lifecycle using Redis as an event store.

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[contributors-url]: https://github.com/jimfim/Redis.Stream.Subscriber/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[forks-url]: https://github.com/jimfim/Redis.Stream.Subscriber/network/members
[stars-shield]: https://img.shields.io/github/stars/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[stars-url]: https://github.com/jimfim/Redis.Stream.Subscriber/stargazers
[issues-shield]: https://img.shields.io/github/issues/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[issues-url]: https://github.com/jimfim/Redis.Stream.Subscriber/issues
[license-shield]: https://img.shields.io/github/license/jimfim/Redis.Stream.Subscriber.svg?style=for-the-badge
[license-url]: https://github.com/jimfim/Redis.Stream.Subscriber/blob/master/LICENSE.txt
[build-shield]: https://img.shields.io/github/workflow/status/jimfim/Redis.Stream.Subscriber/.NET.svg?style=for-the-badge
[build-status]: https://img.shields.io/github/workflow/status/jimfim/Redis.Stream.Subscriber/.NET
