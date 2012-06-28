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

Producer represents logical connection to a single broker. 

To create `Producer` that will produce messages to a broker at `brokerAddress` call the following constructor:
```csharp
public Producer(String brokerAddress);
```

To open message stream, use one of the following signatures:
```csharp
public ProducerMessageStream OpenStream(String topic)
public ProducerMessageStream OpenStream(String topic, Int32 numberOfPartitions)
public ProducerMessageStream OpenStream(String topic, Int32 numberOfPartitions, IPartitioner partitioner)
```


