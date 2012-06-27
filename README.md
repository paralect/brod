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
var producer = new Producer("localhost:5567");
using(var stream = producer.OpenMessageStream("sample-topic"))
{
    producer.Send("Hello world!");
}
```

Your first Brod consumer:

```csharp
var context = new ConsumerContext();
var consumer = context.CreateConsumer("localhost:5568");
using(var stream = consumer.OpenMessageStream("sample-topic"))
{
    foreach (var message in stream.NextString())
        Console.WriteLine(message);
}
```