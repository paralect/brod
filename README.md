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
------------

`Producer` represents logical connection to a single broker. 

To create `Producer` that will produce messages to a broker at `brokerAddress` call the following constructor:
```csharp
/// <summary>
/// Constructs Producer with specified broker address
/// </summary>
public Producer(String brokerAddress);
```

To open message stream, use one of the following signatures:
```csharp
/// <summary>
/// Open stream for specified topic that has one partition (#0)
/// </summary>
public ProducerMessageStream OpenStream(String topic)

/// <summary>
/// Open stream for specified topic that has numberOfParitions partitions. DefaultPartitioner will be used.
/// </summary>
public ProducerMessageStream OpenStream(String topic, Int32 numberOfPartitions)

/// <summary>
/// Open stream for specified topic that has numberOfParitions partitions with specified partitioner
/// </summary>
public ProducerMessageStream OpenStream(String topic, Int32 numberOfPartitions, IPartitioner partitioner)
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
/// Send binary message to specified partition
/// </summary>
public void Send(byte[] payload, Int32 partition)

/// <summary>
/// Send text message with default UTF-8 encoding to partition, that will be selected by Partitioner of this stream
/// </summary>
public void Send(String message)

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
