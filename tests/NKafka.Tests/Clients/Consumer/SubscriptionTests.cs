//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NKafka.Clients.Consumer;
using NKafka.Config;
using NKafka.Protocol;

namespace NKafka.Tests.Clients.Consumer;

public class SubscriptionTests
{
    [Theory]
    [InlineData(ApiVersion.Version1, "AAEAAAABAAR0ZXN0/////wAAAAA=")]
    [InlineData(ApiVersion.Version2, "AAIAAAABAAR0ZXN0/////wAAAAD/////")]
    [InlineData(ApiVersion.Version3, "AAMAAAABAAR0ZXN0/////wAAAAD///////8=")]
    public void SubscriptionSerializeTest(ApiVersion version, string data)
    {
        var subscription = new Subscription(new[]
            {
                "test"
            },
            AutoOffsetReset.None,
            new IPartitionAssignor[]
            {
                new RangeAssignor(),
                new RoundRobinAssignor()
            },
            apiVersion: version);

        var result = subscription.SerializeAsMetadata();
        var bytes = Convert.FromBase64String(data);

        result.Should().BeEquivalentTo(bytes);
    }
}