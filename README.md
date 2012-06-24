Brod
====

High-throughput, distributed .NET messaging system for real-time and offline processing, havely 
inspirited by Apache Kafka project, that was originally developed at LinkedIn.

Brod is designed to be able to keep messages persistent on disk without performance degradation, regardless of the 
volume of messages.

Usage
-----

Your first producer:

```csharp
var context = new ProducerContext();
var producer = context.CreateProducer("localhost:5567");
producer.Send("sample-topic", 0, "Hello world!");
```

Your first consumer:

```csharp
var context = new ConsumerContext();
var consumer = context.CreateConsumer("localhost:5568");
var stream = consumer.CreateMessageStream("sample-topic");

foreach (var message in stream.NextString())
    Console.WriteLine(message);
```