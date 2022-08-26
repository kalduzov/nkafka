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

public class StructSpecification
{
    public string Name { get; }

    public Versions Versions { get; }

    public IReadOnlyCollection<FieldSpecification> Fields { get; }

    public bool HasKeys { get; }

    public StructSpecification(
        [JsonProperty("name")] string name,
        [JsonProperty("versions")] string versions,
        [JsonProperty("fields")] IReadOnlyCollection<FieldSpecification>? fields)
    {
        Name = name;
        Versions = Versions.Parse(versions, null!);

        var newFields = new List<FieldSpecification>();

        if (fields is not null)
        {
            var tags = new HashSet<int>();

            foreach (var field in fields)
            {
                if (field.Tag.HasValue)
                {
                    if (tags.Contains(field.Tag.Value))
                    {
                        throw new ArgumentException(
                            $"In {name}, field {field.Name} has a duplicate tag ID {field.Tag.Value}.  All tags IDs must be unique.");
                    }

                    tags.Add(field.Tag.Value);
                }

                newFields.Add(field);
            }

            for (var i = 0; i < tags.Count; i++)
            {
                if (!tags.Contains(i))
                {
                    throw new ArgumentException("In {name}, the tag IDs are not contiguous. Make use of tag {i} before using any higher tag IDs.");
                }
            }
        }

        Fields = newFields.ToImmutableArray();
        HasKeys = Fields.Any(f => f.MapKey);
    }
}