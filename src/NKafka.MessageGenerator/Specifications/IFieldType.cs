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

namespace NKafka.MessageGenerator.Specifications;

public interface IFieldType
{
    private const string _ARRAY_PREFIX = "[]";

    string ClrName { get; }

    bool IsArray => false;

    bool IsStruct => false;

    bool IsStructArray => false;

    bool IsString => false;

    bool SerializationIsDifferentInFlexibleVersions => false;

    bool IsBytes => false;

    bool IsRecords => false;

    bool IsFloat => false;

    bool CanBeNullable => false;

    int? Size => null;

    bool IsVariableLength => !Size.HasValue;

    string ToString();

    public static IFieldType Parse(string value)
    {
        value = value.Trim();

        switch (value)
        {
            case BoolFieldType.NAME:
                return BoolFieldType.Instance;
            case BytesFieldType.NAME:
                return BytesFieldType.Instance;
            case RecordsFieldType.NAME:
                return RecordsFieldType.Instance;
            case Int8FieldType.NAME:
                return Int8FieldType.Instance;
            case Int16FieldType.NAME:
                return Int16FieldType.Instance;
            case UInt16FieldType.NAME:
                return UInt16FieldType.Instance;
            case Int32FieldType.NAME:
                return Int32FieldType.Instance;
            case UInt32FieldType.NAME:
                return UInt32FieldType.Instance;
            case Int64FieldType.NAME:
                return Int64FieldType.Instance;
            case UuidFieldType.NAME:
                return UuidFieldType.Instance;
            case Float64FieldType.NAME:
                return Float64FieldType.Instance;
            case StringFieldType.NAME:
                return StringFieldType.Instance;
            default:
                {
                    if (!value.StartsWith(_ARRAY_PREFIX))
                    {
                        return new StructType(value);
                    }

                    var elementTypeString = value[2..];

                    if (elementTypeString.Length == 0)
                    {
                        throw new ArgumentException($"Can't parse array type {value}. No element type found.");
                    }

                    var elementType = Parse(elementTypeString);

                    if (elementType.IsArray)
                    {
                        throw new ArgumentException("Can't have an array of arrays. Use an array of structs containing an array instead.");
                    }

                    return new ArrayType(elementType);
                }
        }
    }

    internal sealed class BoolFieldType: IFieldType
    {
        public const string NAME = "bool";

        public static readonly BoolFieldType Instance = new();

        public string ClrName => "bool";

        public int? Size => 1;

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class BytesFieldType: IFieldType
    {
        public const string NAME = "bytes";
        public static readonly BytesFieldType Instance = new();

        public string ClrName => "byte[]";

        public bool SerializationIsDifferentInFlexibleVersions => true;

        public bool IsBytes => true;

        public bool CanBeNullable => true;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class RecordsFieldType: IFieldType
    {
        public const string NAME = "records";

        public static readonly RecordsFieldType Instance = new();

        public string ClrName => "Records";

        public bool SerializationIsDifferentInFlexibleVersions => true;

        public bool IsRecords => true;

        public bool CanBeNullable => true;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class Int8FieldType: IFieldType
    {
        public const string NAME = "int8";

        public static readonly Int8FieldType Instance = new();

        public string ClrName => "sbyte";

        public int? Size => 1;

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class Int16FieldType: IFieldType
    {
        public const string NAME = "int16";

        public static readonly Int16FieldType Instance = new();

        public string ClrName => "short";

        public int? Size => 2;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class UInt16FieldType: IFieldType
    {
        public const string NAME = "uint16";

        public static readonly UInt16FieldType Instance = new();

        public string ClrName => "ushort";

        public int? Size => 2;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class Int32FieldType: IFieldType
    {
        public const string NAME = "int32";

        public static readonly Int32FieldType Instance = new();

        public string ClrName => "int";

        public int? Size => 4;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class UInt32FieldType: IFieldType
    {
        public const string NAME = "uint32";

        public static readonly UInt32FieldType Instance = new();

        public string ClrName => "uint";

        public int? Size => 4;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class Int64FieldType: IFieldType
    {
        public const string NAME = "int64";

        public static readonly Int64FieldType Instance = new();

        public string ClrName => "long";

        public int? Size => 8;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class UuidFieldType: IFieldType
    {
        public const string NAME = "uuid";

        public static readonly UuidFieldType Instance = new();

        public string ClrName => "Guid";

        public int? Size => 16;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class Float64FieldType: IFieldType
    {
        public const string NAME = "float64";

        public static readonly Float64FieldType Instance = new();

        public string ClrName => "double";

        public int? Size => 8;

        public bool IsFloat => true;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class StringFieldType: IFieldType
    {
        public const string NAME = "string";

        public static readonly StringFieldType Instance = new();

        public string ClrName => "string";

        public bool SerializationIsDifferentInFlexibleVersions => true;

        public bool IsString => true;

        public bool CanBeNullable => true;

        public override string ToString()
        {
            return NAME;
        }
    }

    internal sealed class StructType: IFieldType
    {
        private readonly string _type;

        public string TypeName => _type;

        public StructType(string type)
        {
            _type = type;
        }

        public string ClrName => _type;

        public bool IsStruct => true;

        public bool SerializationIsDifferentInFlexibleVersions => true;

        public override string ToString()
        {
            return _type;
        }
    }

    internal sealed class ArrayType: IFieldType
    {
        public IFieldType ElementType { get; }

        public string ElementName => ElementType.ToString();

        public ArrayType(IFieldType elementType)
        {
            ElementType = elementType;
        }

        public string ClrName => throw new NotSupportedException();

        public bool IsArray => true;

        public bool IsStructArray => ElementType.IsStruct;

        public bool CanBeNullable => true;

        public bool SerializationIsDifferentInFlexibleVersions => true;

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"[]{ElementType}";
        }
    }
}