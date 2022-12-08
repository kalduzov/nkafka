﻿// // This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
// namespace NKafka.Protocol.Responses;
//
// /// <summary>
// /// И
// /// </summary>
// public class MetadataResponseMessage : ResponseMessage
// {
//     /// <summary>
//     ///     The ID of the controller broker
//     /// </summary>
//     public int ControllerId { get; init; }
//
//     /// <summary>
//     ///     The cluster ID that responding broker belongs to
//     /// </summary>
//     public string? ClusterId { get; init; }
//
//     /// <summary>
//     ///     32-bit bitfield to represent authorized operations for this cluster
//     /// </summary>
//     public int ClusterAuthorizedOperations { get; init; }
//
//     /// <summary>
//     ///     Each broker in the response
//     /// </summary>
//     public IReadOnlyCollection<BrokerInfo> Brokers { get; init; } = Array.Empty<BrokerInfo>();
//
//     /// <summary>
//     ///     Each topic in the response
//     /// </summary>
//     public IReadOnlyCollection<TopicInfo> Topics { get; init; } = Array.Empty<TopicInfo>();
//
//     /// <summary>
//     /// Writes out this message to the given stream.
//     /// </summary>
//     /// 
//     public override void Write(Stream stream, ApiVersion version)
//     {
//     }
//
//     /// <summary>
//     /// Reads this message from the given BufferReader. This will overwrite all relevant fields with information from the byte buffer.
//     /// </summary>
//     public override void Read(IBufferReader reader, ApiVersion version)
//     {
//     }
// }