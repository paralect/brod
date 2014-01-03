Brod
====

High-throughput, distributed .NET messaging system for real-time and offline processing, havely 
inspirited by Apache Kafka project, that was originally developed at LinkedIn.

Brod is designed to be able to keep messages persistent on disk without performance degradation, regardless of the 
volume of messages.

Basic Concepts
========

Messages are published to a _topic_ by a _producer_. Each message will be send to a 
server acting as a _broker_. Some number of _consumers_ subscribe to a topic, and each published message is 
delivered to all consumers.

Main Brod components are:
  * [Brokers](https://github.com/paralect/brod/wiki/Brokers) - set of nodes that store published messages
  * [Producers](https://github.com/paralect/brod/wiki/Producers) - set of nodes, that produce messages
  * [Consumers](https://github.com/paralect/brod/wiki/Consumers) - set of nodes, that consume messages

Installation
------------

Brod require .NET Framework 4.0 or higher. Mono support is planned for future releases.

In order to build project, perform the following steps:

  1. Clone project from `https://github.com/paralect/brod.git`
  2. Run `./Build.bat` which will output to `./target` directory

In `./target` you'll find:

  * `Brod.exe` - Brod broker
  * `SampleProducer.exe` - Sample console project, that allows you interractively produce messages.
  * `SampleConsumer.exe` - Sample console project, that allows you to see messages, produced by `SampleProducer`
  * `MassiveProducer.exe` - Sample console project, that produce 100.000 messages, 1kb each.

Usage
-----

Your first Brod producer:

```csharp
using(var producer = new Producer("localhost:5567"));
{
    producer.Send("sample-topic", "Hello world!");
}
```

Your first Brod consumer:

```csharp

using(var consumer = new Consumer("localhost:5567"))
using(var stream = consumer.OpenStream("sample-topic"))
{
    foreach (var message in stream.NextString())
        Console.WriteLine(message);
}
```
