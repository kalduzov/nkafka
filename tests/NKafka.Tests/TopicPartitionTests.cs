//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
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

namespace NKafka.Tests;

public class TopicPartitionTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(3994)]
    [InlineData(int.MaxValue)]
    public void TopicPartitionCreate_Successful(int partition)
    {
        var topicPartition = new TopicPartition("test", partition);
        topicPartition.Topic.Should().Be("test");
        topicPartition.Partition.Should().Be(partition);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TopicPartitionCreate_TopicInvalid_Successful(string? topicName)
    {
        TopicPartition BuildTopicPartition()
        {
            return new TopicPartition(topicName, 1);
        }

        FluentActions
            .Invoking(BuildTopicPartition)
            .Should()
            .Throw<ArgumentNullException>()
            .And.ParamName.Should()
            .Be("topic");
    }

    [Fact]
    public void TopicPartitionCreate_PartitionInvalid_Successful()
    {
        TopicPartition BuildTopicPartition()
        {
            return new TopicPartition("test", -2);
        }

        FluentActions
            .Invoking(BuildTopicPartition)
            .Should()
            .Throw<ArgumentNullException>()
            .And.ParamName.Should()
            .Be("partition");
    }
}