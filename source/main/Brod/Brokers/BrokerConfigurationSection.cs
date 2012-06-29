using System;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace Brod.Brokers
{
    public class BrokerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("storageDirectory", IsRequired = false)]
        public SimpleElement<String> StorageDirectory
        {
            get { return (SimpleElement<String>) this["storageDirectory"]; }
        }

        [ConfigurationProperty("brokerId", IsRequired = false)]
        public SimpleElement<Int32> BrokerId
        {
            get { return (SimpleElement<Int32>)this["brokerId"]; }
        }

        [ConfigurationProperty("hostName", IsRequired = false)]
        public SimpleElement<String> HostName
        {
            get { return (SimpleElement<String>)this["hostName"]; }
        }

        [ConfigurationProperty("port", IsRequired = false)]
        public SimpleElement<Int32> Port
        {
            get { return (SimpleElement<Int32>)this["port"]; }
        }

        [ConfigurationProperty("pullPort", IsRequired = false)]
        public SimpleElement<Int32> PullPort
        {
            get { return (SimpleElement<Int32>)this["pullPort"]; }
        }

        [ConfigurationProperty("numberOfPartitions", IsRequired = false)]
        public SimpleElement<Int32> NumberOfPartitions
        {
            get { return (SimpleElement<Int32>)this["numberOfPartitions"]; }
        }

        [ConfigurationProperty("numberOfPartitionsPerTopic", IsRequired = false)]
        [ConfigurationCollection(typeof(TopicCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public TopicCollection NumberOfPartitionsPerTopic
        {
            get { return (TopicCollection)this["numberOfPartitionsPerTopic"]; }
        }

        public static BrokerConfigurationSection FromXml(XElement element)
        {
            var section = new BrokerConfigurationSection();
            section.DeserializeSection(element.CreateReader());
            return section;
        }

    }

    public class SimpleElement<TType> : ConfigurationElement
    {
        public TType Value { get; set; }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            Value = (TType) Convert.ChangeType(reader.ReadElementContentAsString(), typeof(TType));
        }
    }

    public class TopicCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TopicElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TopicElement)element).Topic;
        }
    }
    public class TopicElement : ConfigurationElement
    {
        [ConfigurationProperty("topic", IsKey = true, IsRequired = true)]
        public string Topic
        {
            get { return (string)base["topic"]; }
            set { base["topic"] = value; }
        }

        [ConfigurationProperty("partitions", IsRequired = true)]
        public Int32 Partitions
        {
            get { return (Int32) base["partitions"]; }
            set { base["partitions"] = value; }
        }
    }
}