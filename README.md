# Redis.Stream.Subscriber

## Why?
The StackExchange.Redis client does not provide the functionality to subscribe a stream without constant polling. 

https://github.com/StackExchange/StackExchange.Redis/issues/1155

The purpose of this library is to provide a more "EventStore" style redis stream client. 

## Example

this repo contains a docker-compose file with a sample redis setup you can use to run the example clients in this solution

`docker-compose up`

you can then connect to redis-commander on port