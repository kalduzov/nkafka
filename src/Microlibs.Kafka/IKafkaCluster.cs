using System;
using Microlibs.Kafka.Clients;
using Microlibs.Kafka.Clients.Producer;
using Microlibs.Kafka.Config;

namespace Microlibs.Kafka
{
    /// <summary>
    /// 
    /// </summary>
    public interface IKafkaCluster : IDisposable
    {
        CommonConfig Config { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="producerConfig"></param>
        /// <returns></returns>
        IProducer<TKey, TValue> BuildProducer<TKey, TValue>(string name, ProducerConfig? producerConfig = null);
    }
}