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
  * [Brokers](/paralect/brod/wiki/Brokers) - set of nodes that store published messages
  * [Producers](/paralect/brod/wiki/Producers) - set of nodes, that produce messages
  * [Consumers](/paralect/brod/wiki/Consumers) - set of nodes, that consume messages

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

Consumer API
============

`Consumer` represents logical connection to a single broker. 

To create `Consumer` that will consume messages from the broker at `brokerAddress`, call the following constructor:
```csharp
public Consumer(String brokerAddress)
```

To start consuming messages, open message stream using one of the following signatures:
```csharp
/// <summary>
/// Open stream for specified topic, that has one partition (#0)
/// </summary>
public ConsumerMessageStream OpenStream(String topic)

/// <summary>
/// Open stream for specified topic, that has numberOfPartitions partitions
/// </summary>
public ConsumerMessageStream OpenStream(String topic, Int32 numberOfPartitions)
```

You can create more than one stream for single topic. In this way you can use several threads to consume messages from
this topic. Of course, number of topic partitions should be many more, than number of streams. 

```csharp
/// <summary>
/// Open numberOfStreams streams for specified topic, that has numberofPartitions partitions.
/// Paritions will be assigned to each stream in such a way, that each stream will consume
/// roughly the same number of partitions.
/// </summary>
public List<ConsumerMessageStream> OpenStreams(String topic, Int32 numberOfPartitions, Int32 numberOfStreams)
```