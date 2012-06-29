Brod
====

High-throughput, distributed .NET messaging system for real-time and offline processing, havely 
inspirited by Apache Kafka project, that was originally developed at LinkedIn.

Brod is designed to be able to keep messages persistent on disk without performance degradation, regardless of the 
volume of messages.

Usage
-----

Your first Brod producer:

```csharp
using(var producer = new Producer("localhost:5567"));
using(var stream = producer.OpenStream("sample-topic"))
{
    producer.Send("Hello world!");
}
```

Your first Brod consumer:

```csharp

using(var consumer = new Consumer("localhost:5568"))
using(var stream = consumer.OpenStream("sample-topic"))
{
    foreach (var message in stream.NextString())
        Console.WriteLine(message);
}
```

Producer API
============

`Producer` represents logical connection to a single broker. 

To create `Producer` that will produce messages to a broker at `brokerAddress` call the following constructor:
```csharp
/// <summary>
/// Constructs Producer with specified broker address
/// </summary>
public Producer(String brokerAddress);
```

To start producing messages, open message stream, using one of the following signatures:
```csharp
/// <summary>
/// Open stream for specified topic that has one partition (#0)
/// </summary>
public ProducerMessageStream OpenStream(String topic)

/// <summary>
/// Open stream for specified topic using specified partitioner
/// </summary>
public ProducerMessageStream OpenStream(String topic, IPartitioner partitioner)

/// <summary>
/// Open stream for specified topic and specified partition
/// </summary>
public ProducerMessageStream OpenStream(String topic, Int32 partition)
```

ProducerMessageStream API
-------------------------

`ProducerMessageStream` allows to send messages.

To send single message, use one of the following signatures:
```csharp
/// <summary>
/// Send binary message to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(byte[] payload)

/// <summary>
/// Send binary message with specified key to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(byte[] payload, Object key)

/// <summary>
/// Send binary message to specified partition
/// </summary>
public void Send(byte[] payload, Int32 partition)

/// <summary>
/// Send binary message with key to specified partition
/// </summary>
public void Send(byte[] payload, Object key, Int32 partition)

/// <summary>
/// Send text message with default UTF-8 encoding to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(String message)

/// <summary>
/// Send text message with key using default UTF-8 encoding to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(String message, Object key)

/// <summary>
/// Send text message with default UTF-8 encoding to specified partition
/// </summary>
public void Send(String message, Int32 partition);

/// <summary>
/// Send text message with specified encoding to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(String message, Encoding encoding)

/// <summary>
/// Send text message with specified encoding to specified partition
/// </summary>
public void Send(String message, Encoding encoding, Int32 partition)
```

To send many messages at once, use one of the following overloads:

```csharp
/// <summary>
/// Send binary messages to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(IEnumerable<byte[]> payloads)

/// <summary>
/// Send binary messages to specified partition
/// </summary>
public void Send(IEnumerable<byte[]> payloads, Int32 partition)

/// <summary>
/// Send text messages with default UTF-8 encoding to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(IEnumerable<String> messages)

/// <summary>
/// Send text messages with default UTF-8 encoding to specified partition
/// </summary>
public void Send(IEnumerable<String> messages, Int32 partition)

/// <summary>
/// Send text messages with specified encoding to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(IEnumerable<String> messages, Encoding encoding)

/// <summary>
/// Send text messages with specified encoding to specified partition
/// </summary>
public void Send(IEnumerable<String> messages, Encoding encoding, Int32 partition)
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