using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace Brod.Configuration
{
    public class BrokerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("storageDirectory", IsRequired = false)]
        public SimpleElement<String> StorageDirectory
        {
            get { return (SimpleElement<String>) this["storageDirectory"]; }
        }

        [ConfigurationProperty("producerPort", IsRequired = false)]
        public SimpleElement<Int32> ProducerPort
        {
            get { return (SimpleElement<Int32>)this["producerPort"]; }
        }

        [ConfigurationProperty("consumerPort", IsRequired = false)]
        public SimpleElement<Int32> ConsumerPort
        {
            get { return (SimpleElement<Int32>)this["consumerPort"]; }
        }

        [ConfigurationProperty("numberOfPartitions", IsRequired = false)]
        public SimpleElement<Int32> NumberOfPartitions
        {
            get { return (SimpleElement<Int32>)this["numberOfPartitions"]; }
        }

        [ConfigurationProperty("numberOfPartitionsPerTopic", IsRequired = false)]
        [ConfigurationCollection(typeof(AcmeInstanceCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public AcmeInstanceCollection NumberOfPartitionsPerTopic
        {
            get { return (AcmeInstanceCollection)this["numberOfPartitionsPerTopic"]; }
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

    public class AcmeInstanceCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AcmeInstanceElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AcmeInstanceElement)element).Topic;
        }
    }
    public class AcmeInstanceElement : ConfigurationElement
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