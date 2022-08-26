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
// using System.IO;
//
// using NKafka.Protocol.Extensions;
//
// namespace NKafka.Protocol.Requests;
//
// internal class DescribeClusterRequest: RequestBody
// {
//     public DescribeClusterRequest()
//     {
//         Length = 0x4;
//         ApiKey = ApiKeys.DescribeCluster;
//     }
//
//     public bool IncludeClusterAuthorizedOperations { get; set; }
//
//     public override void SerializeToStream(Stream stream)
//     {
//         // Span<byte> buf = stackalloc byte[4];
//         //
//         // stream.Write((IncludeClusterAuthorizedOperations ? 1 : 0).ToBigEndian());
//     }
// }