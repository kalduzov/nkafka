﻿//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

using NKafka.Protocol;

namespace NKafka.Messages;

public sealed partial class MetadataResponseMessage
{
    public sealed partial class MetadataResponseBrokerCollection
    {
        internal IReadOnlyDictionary<int, Node> ConvertToNodes()
        {
            var dictionary = new Dictionary<int, Node>(Count);

            foreach (var broker in this)
            {
                var node = new Node(broker.NodeId, broker.Host, broker.Port, broker.Rack);
                dictionary.Add(node.Id, node);
            }

            return dictionary;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    /// <inheritdoc/>
    public bool ShouldClientThrottle(ApiVersion version)
    {
        return version >= ApiVersion.Version6;
    }
}