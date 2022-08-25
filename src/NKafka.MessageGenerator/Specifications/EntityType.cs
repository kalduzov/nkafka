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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NKafka.MessageGenerator.Specifications;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityType
{
    [EnumMember(Value = "unknown")]
    Unknown,

    [EnumMember(Value = "transactionalId")]
    TransactionalId,

    [EnumMember(Value = "producerId")]
    ProducerId,

    [EnumMember(Value = "brokerId")]
    BrokerId,

    [EnumMember(Value = "groupId")]
    GroupId,

    [EnumMember(Value = "topicName")]
    TopicName
}