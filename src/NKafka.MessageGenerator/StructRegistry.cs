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

using NKafka.MessageGenerator.Specifications;

namespace NKafka.MessageGenerator;

public sealed class StructRegistry
{
    private readonly Dictionary<string, StructInfo> _struct;

    private record StructInfo(StructSpecification Specification, Versions ParentVersion);

    public IEnumerable<StructSpecification> CommonStructs => _struct
        .Where(s => CommonStructNames.Contains(s.Key))
        .Select(x => x.Value.Specification);

    public HashSet<string> CommonStructNames { get; }

    public StructRegistry()
    {
        _struct = new Dictionary<string, StructInfo>();
        CommonStructNames = new HashSet<string>();
    }

    public void Register(MessageSpecification message)
    {
        foreach (var structCommon in message.CommonStructs)
        {
            if (char.IsLower(structCommon.Name[0]))
            {
                throw new ArgumentException($"Can't process structure {structCommon.Name}: the first letter of structure names must be capitalized.");
            }

            if (_struct.ContainsKey(structCommon.Name))
            {
                throw new ArgumentException($"Common struct {structCommon.Name} was specified twice.");
            }

            _struct.Add(structCommon.Name, new StructInfo(structCommon, structCommon.Versions));
            CommonStructNames.Add(structCommon.Name);
        }

        AddStructSpecs(message.ValidVersions, message.Fields);
    }

    private void AddStructSpecs(Versions parentVersions, IReadOnlyCollection<FieldSpecification> fields)
    {
        foreach (var field in fields)
        {
            var typeName = field.Type switch
            {
                IFieldType.ArrayType arrayType => arrayType.IsStructArray ? arrayType.ElementName : null,
                IFieldType.StructType structType => structType.TypeName,
                _ => null
            };

            if (typeName is null)
            {
                continue;
            }

            if (CommonStructNames.Contains(typeName))
            {
                if (field.Fields.Count != 0)
                {
                    throw new ArgumentException($"Can't re-specify the common struct {typeName} as an inline struct.");
                }
            }
            else if (_struct.ContainsKey(typeName))
            {
                throw new ArgumentException($"Struct {typeName} was specified twice.");
            }
            else
            {
                var spec = new StructSpecification(typeName, field.Versions.ToString(), field.Fields);
                _struct.Add(typeName, new StructInfo(spec, parentVersions));
            }

            AddStructSpecs(parentVersions.Intersect(field.Versions), field.Fields);
        }
    }

    public StructSpecification FindStruct(FieldSpecification field)
    {
        string structFieldName;

        if (field.Type.IsArray && field.Type is IFieldType.ArrayType arrayType)
        {
            structFieldName = arrayType.ElementName;
        }
        else if (field.Type.IsStruct && field.Type is IFieldType.StructType type)
        {
            structFieldName = type.TypeName;
        }
        else
        {
            throw new ArgumentException();
        }

        if (!_struct.TryGetValue(structFieldName, out var structInfo))
        {
            throw new Exception();
        }

        return structInfo.Specification;
    }

    public bool IsStructArrayWithKeys(FieldSpecification field)
    {
        if (field.Type is not IFieldType.ArrayType arrayType)
        {
            return false;
        }

        if (!arrayType.IsStructArray)
        {
            return false;
        }

        if (!_struct.TryGetValue(arrayType.ElementName, out var structInfo))
        {
            throw new Exception($"Unable to locate a specification for the structure {arrayType.ElementName}");
        }

        return structInfo.Specification.HasKeys;
    }
}