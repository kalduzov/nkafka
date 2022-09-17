// // This is an independent project of an individual developer. Dear PVS-Studio, please check it.
//
// // PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
//
// /*
//  * Copyright © 2022 Aleksey Kalduzov. All rights reserved
//  * 
//  * Author: Aleksey Kalduzov
//  * Email: alexei.kalduzov@gmail.com
//  * 
//  * Licensed under the Apache License, Version 2.0 (the "License");
//  * you may not use this file except in compliance with the License.
//  * You may obtain a copy of the License at
//  * 
//  *     https://www.apache.org/licenses/LICENSE-2.0
//  * 
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */
//
// using NKafka.Protocol.Extensions;
//
// namespace NKafka.Protocol.Requests;
//
// internal class ProduceRequestMessage: RequestBody
// {
//     public ProduceRequestMessage(ApiVersions version, IReadOnlyCollection<TopicProduceData> topicProduceData)
//     {
//         Version = version;
//         ApiKey = ApiKeys.Metadata;
//         Length = CalculateLen();
//         TopicData = topicProduceData;
//     }
//
//     private int CalculateLen()
//     {
//         return 0;
//     }
//
//     /// <summary>
//     /// The transactional ID, or null if the producer is not transactional
//     /// </summary>
//     public string? TransactionalId { get; set; }
//
//     /// <summary>
//     /// The number of acknowledgments the producer requires the leader to have received before considering a request complete.
//     /// Allowed values: 0 for no acknowledgments, 1 for only the leader and -1 for the full ISR
//     /// </summary>
//     public short Acks { get; set; }
//
//     /// <summary>
//     /// The timeout to await a response in milliseconds
//     /// </summary>
//     public int TimeoutMs { get; set; }
//
//     /// <summary>
//     /// Each topic to produce to
//     /// </summary>
//     public IReadOnlyCollection<TopicProduceData> TopicData { get; }
//
//     public override void SerializeToStream(Stream stream)
//     {
//         if (Version is >= ApiVersions.Version3 and < ApiVersions.Version9)
//         {
//             stream.Write(TransactionalId.AsNullableString());
//         }
//
//         if (Version >= ApiVersions.Version9)
//         {
//             stream.Write(TransactionalId.AsCompactNullableString());
//         }
//
//         stream.Write(Acks.ToBigEndian());
//         stream.Write(TimeoutMs.ToBigEndian());
//
//         foreach (var data in TopicData)
//         {
//             stream.Write(data.Serialize());
//         }
//     }
// }

