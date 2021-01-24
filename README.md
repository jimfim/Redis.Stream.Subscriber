
# Redis.Stream.Subscriber
## Description
<img src="./docs/icon.png" align="right" alt="Redis Stream Logo" width="120" height="178">
The StackExchange.Redis client does not provide the functionality to subscribe a stream without constant polling. 

https://github.com/StackExchange/StackExchange.Redis/issues/1155

The purpose of this library is to provide a more "EventStore" blocking style redis stream client. 

## Table of Contents
* [Installation](#installation)
* [Usage](#usage)
* [Methodology](#methodology)
* [License](#license)

## Installation
Install via nuget
```
dotnet install <project>
```

## Usage

### Redis

this repo contains a docker-compose file with a sample redis setup you can use to run the example clients in this solution

`docker-compose up`

you can then connect to redis-commander on port


## License
TBD...