using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microlibs.Kafka.Clients
{
    public interface IAdminClient 
    {
        Task<CreateTopicsResult> CreateTopicsAsync(IReadOnlyCollection<TopicSpecification> topics, CreateTopicOptions options, CancellationToken token = default);
    }

    public class TopicSpecification
    {
    }

    public class CreateTopicsResult
    {
    }
}