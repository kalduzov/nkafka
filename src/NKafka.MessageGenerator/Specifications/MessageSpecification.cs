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

using System.Collections.Immutable;

using Newtonsoft.Json;

namespace NKafka.MessageGenerator.Specifications;

/// <summary>
/// Message specification 
/// </summary>
public record MessageSpecification
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly MessageSpecification Empty = new(
        apiKey: -1,
        type: MessageType.None,
        listeners: Array.Empty<RequestListenerType>(),
        name: string.Empty,
        validVersions: string.Empty,
        flexibleVersions: "0+",
        fields: Array.Empty<FieldSpecification>(),
        commonStructs: Array.Empty<StructSpecification>());

    /// <summary>
    /// Message api key 
    /// </summary>
    public short ApiKey { get; }

    /// <summary>
    /// Message type 
    /// </summary>
    public MessageType Type { get; }

    public IReadOnlyCollection<RequestListenerType>? Listeners { get; }

    public Versions ValidVersions => Struct.Versions;

    public Versions FlexibleVersions { get; }

    public IReadOnlyCollection<FieldSpecification> Fields => Struct.Fields;

    /// <summary>
    /// Внутренние структуры данных
    /// </summary>
    public IReadOnlyCollection<StructSpecification> CommonStructs { get; }

    [JsonIgnore]
    public string ClassName
    {
        get
        {
            return Type switch
            {
                MessageType.Request => Struct.Name + "Message",
                MessageType.Response => Struct.Name + "Message",
                MessageType.Header => Struct.Name,
                _ => Struct.Name
            };
        }
    }

    [JsonIgnore]
    public StructSpecification Struct { get; }

    [JsonConstructor]
    public MessageSpecification(
        [JsonProperty("apiKey")] short? apiKey,
        [JsonProperty("type")] MessageType type,
        [JsonProperty("listeners")] IReadOnlyCollection<RequestListenerType>? listeners,
        [JsonProperty("name")] string name,
        [JsonProperty("validVersions")] string validVersions,
        [JsonProperty("flexibleVersions")] string flexibleVersions,
        [JsonProperty("fields")] IReadOnlyCollection<FieldSpecification> fields,
        [JsonProperty("commonStructs")] IReadOnlyCollection<StructSpecification>? commonStructs)
    {
        Struct = new StructSpecification(name, validVersions, fields);
        ApiKey = apiKey ?? -1;
        Type = type;
        CommonStructs = (commonStructs ?? Array.Empty<StructSpecification>()).ToImmutableArray();

        if (string.IsNullOrWhiteSpace(flexibleVersions))
        {
            throw new ArgumentException("You must specify a value for flexibleVersions. Please use 0+ for all new messages.");
        }

        FlexibleVersions = Versions.Parse(flexibleVersions, Versions.None);

        if (FlexibleVersions is { IsEmpty: false, Highest: < short.MaxValue })
        {
            throw new ArgumentException(
                $"Field {name} specifies flexibleVersions {FlexibleVersions}, which is not open-ended. flexibleVersions must be either none, "
                + "or an open-ended range (that ends with a plus sign).");
        }

        if (listeners is not null && listeners.Count != 0 && type != MessageType.Request)
        {
            throw new ArgumentException("The `requestScope` property is only valid for messages with type `request`");
        }
        Listeners = listeners;

    }
}