using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka
{
    public interface IProducer : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="Message"></param>
        /// <typeparam name="T"></typeparam>
        public void Produce<T>(string topicName, T Message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<ProduceResult> ProduceAsync<T>(string topic, T message, CancellationToken token = default)
        {
            var topics = new[]
            {
                topic
            };

            return ProduceAsync(topics, message, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<ProduceResult> ProduceAsync<T>(IReadOnlyList<string> topics, T message, CancellationToken token = default)
        {
            return ProduceAsync(
                topics,
                messages:
                new[]
                {
                    message
                },
                token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<ProduceResult> ProduceAsync<T>(IReadOnlyList<string> topics, IReadOnlyList<T> messages, CancellationToken token = default);
    }

    public class ProduceResult
    {
    }
}