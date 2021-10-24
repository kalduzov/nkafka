using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka
{
    /// <summary>
    /// 
    /// </summary>
    public interface IKafkaCluster
    {
        async Task<string> CreateTopic(string name, CancellationToken token = default)
        {
            var topics = new[]
            {
                name
            };
            var result = await CreateTopics(topics, token);

            return result.First();
        }

        Task<IEnumerable<string>> CreateTopics(IReadOnlyCollection<string> names, CancellationToken token = default);
    }
}