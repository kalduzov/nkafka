using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microlibs.Kafka.Protocol;

namespace Microlibs.Kafka.Clients;

internal sealed class AdminClient : IAdminClient
{
    public Task<CreateTopicsResult> CreateTopicsAsync(
        IReadOnlyCollection<TopicSpecification> topics,
        CreateTopicOptions options,
        CancellationToken token = default)
    {
        // var kafkaRequest = _requestBuilder.Create(ApiKeys.CreateTopics, topics);
        // var kafkaResponse = await SendAsync(kafkaRequest, token);
        //
        // return DefaultResponseBuilder.Create<CreateTopicsResult>(kafkaResponse);

        return Task.FromResult(new CreateTopicsResult());
    }
}