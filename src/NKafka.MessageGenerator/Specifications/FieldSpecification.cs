using System.Collections.Immutable;

using Newtonsoft.Json;

using NKafka.MessageGenerator.Converters;

namespace NKafka.MessageGenerator.Specifications;

public class FieldSpecification
{
    public string Name { get; }

    public IFieldType Type { get; }

    public Versions Versions { get; }

    public Versions NullableVersions { get; }

    public Versions TaggedVersions { get; }

    public Versions? FlexibleVersions { get; }

    [JsonProperty("about")]
    public string? About { get; }

    public string Default { get; }

    public bool Ignorable { get; }

    public EntityType EntityType { get; }

    public bool MapKey { get; }

    public int? Tag { get; }

    public bool ZeroCopy { get; }

    public IReadOnlyCollection<FieldSpecification> Fields { get; }

    [JsonConstructor]
    public FieldSpecification(
        [JsonProperty("name")] string name,
        [JsonProperty("versions")] string versions,
        [JsonProperty("fields")] IReadOnlyCollection<FieldSpecification>? fields,
        [JsonProperty("type")] string type,
        [JsonProperty("mapKey")] bool mapKey,
        [JsonProperty("nullableVersions")] string nullableVersions,
        [JsonProperty("default")] [JsonConverter(typeof(DefaultJsonConverter))]
        string? defaultValue,
        [JsonProperty("ignorable")] bool ignorable,
        [JsonProperty("entityType")] EntityType? entityType,
        [JsonProperty("about")] string about,
        [JsonProperty("taggedVersions")] string taggedVersions,
        [JsonProperty("flexibleVersions")] string flexibleVersions,
        [JsonProperty("tag")] int? tag,
        [JsonProperty("zeroCopy")] bool zeroCopy)
    {
        Name = name;
        TaggedVersions = Versions.Parse(taggedVersions, Versions.None);
        Versions = Versions.Parse(versions, TaggedVersions.IsEmpty ? null! : TaggedVersions);

        if (Versions == null)
        {
            throw new ArgumentException("You must specify the version of the {name} structure.");
        }

        Fields = (fields ?? Array.Empty<FieldSpecification>()).ToImmutableArray();
        Type = IFieldType.Parse(type);
        MapKey = mapKey;
        NullableVersions = Versions.Parse(nullableVersions, Versions.None);

        if (!NullableVersions.IsEmpty)
        {
            if (!Type.CanBeNullable)
            {
                throw new ArgumentException($"Type {Type} cannot be nullable.");
            }
        }

        Default = string.IsNullOrEmpty(defaultValue) ? string.Empty : defaultValue;
        Ignorable = ignorable;
        EntityType = entityType ?? EntityType.Unknown;

        //EntityType.VerifyTypeMatches(name, Type);

        About = string.IsNullOrEmpty(about) ? "" : about;

        if (Fields.Count != 0)
        {
            if (!Type.IsArray && !Type.IsStruct)
            {
                throw new ArgumentException($"Non-array or Struct field {name} cannot have fields");
            }
        }

        if (string.IsNullOrEmpty(flexibleVersions))
        {
            FlexibleVersions = null;
        }
        else
        {
            FlexibleVersions = Versions.Parse(flexibleVersions, null!);

            if (!(Type.IsString || !Type.IsBytes))
            {
                throw new ArgumentException(
                    $"Invalid flexibleVersions override for {name}. Only fields of type string or bytes can specify a flexibleVersions override.");
            }
        }

        Tag = tag;

        if (Tag.HasValue && mapKey)
        {
            throw new ArgumentException("Tagged fields cannot be used as keys.");
        }

        CheckTagInvariants();
        ZeroCopy = zeroCopy;

        if (ZeroCopy && !Type.IsBytes)
        {
            throw new ArgumentException($"Invalid zeroCopy value for {name}. Only fields of type bytes can use zeroCopy flag.");
        }
    }

    private void CheckTagInvariants()
    {
        if (Tag.HasValue)
        {
            if (Tag.Value < 0)
            {
                throw new ArgumentException($"Field {Name} specifies a tag of {Tag}. Tags cannot be negative.");
            }

            if (TaggedVersions.IsEmpty)
            {
                throw new ArgumentException(
                    $"Field {Name} specifies a tag of {Tag}, but has no tagged versions. "
                    + "If a tag is specified, taggedVersions must be specified as well.");
            }

            var nullableTaggedVersions = NullableVersions.Intersect(TaggedVersions);

            if (!(nullableTaggedVersions.IsEmpty || nullableTaggedVersions.Equals(TaggedVersions)))
            {
                throw new ArgumentException(
                    $"Field {Name} specifies nullableVersions {NullableVersions} and taggedVersions {TaggedVersions}. "
                    + "Either all tagged versions must be nullable, or none must be.");
            }

            if (TaggedVersions.Highest < short.MaxValue)
            {
                throw new ArgumentException(
                    $"Field {Name} specifies taggedVersions {TaggedVersions}, which is not open-ended. taggedVersions must "
                    + "be either none, or an open-ended range (that ends with a plus sign).");
            }

            if (!TaggedVersions.Intersect(Versions).Equals(TaggedVersions))
            {
                throw new ArgumentException(
                    $"Field {Name} specifies taggedVersions {TaggedVersions}, and versions {Versions}. "
                    + "taggedVersions must be a subset of versions.");
            }
        }
        else if (!TaggedVersions.IsEmpty)
        {
            throw new ArgumentException(
                $"Field {Name} does not specify a tag, but specifies tagged versions of {TaggedVersions}. "
                + $"Please specify a tag, or remove the taggedVersions.");
        }
    }

    internal string FieldDefault()
    {
        switch (Type)
        {
            case IFieldType.BoolFieldType:
                {
                    if (string.IsNullOrEmpty(Default))
                    {
                        return "false";
                    }

                    if (Default.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        return "true";
                    }

                    if (Default.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        return "false";
                    }

                    throw new ArgumentException($"Invalid default for boolean field {Name}: {Default}");
                }
            case IFieldType.Int8FieldType:
                {
                    var @base = 10;
                    var defaultString = Default;

                    if (defaultString.StartsWith("0x"))
                    {
                        @base = 16;
                        defaultString = defaultString[2..];
                    }

                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return "0";
                    }

                    var val = Convert.ToSByte(defaultString, @base);

                    return $"{val}";
                }
            case IFieldType.Int16FieldType:
                {
                    var @base = 10;
                    var defaultString = Default;

                    if (defaultString.StartsWith("0x"))
                    {
                        @base = 16;
                        defaultString = defaultString[2..];
                    }

                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return "0";
                    }

                    var val = Convert.ToInt16(defaultString, @base);

                    return $"{val}";
                }
            case IFieldType.UInt16FieldType:
                {
                    var @base = 10;
                    var defaultString = Default;

                    if (defaultString.StartsWith("0x"))
                    {
                        @base = 16;
                        defaultString = defaultString[2..];
                    }

                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return "0";
                    }

                    var val = Convert.ToUInt16(defaultString, @base);

                    return $"{val}";
                }
            case IFieldType.Int32FieldType:
                {
                    var @base = 10;
                    var defaultString = Default;

                    if (defaultString.StartsWith("0x"))
                    {
                        @base = 16;
                        defaultString = defaultString[2..];
                    }

                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return "0";
                    }

                    var val = Convert.ToInt32(defaultString, @base);

                    return $"{val}";
                }
            case IFieldType.UInt32FieldType:
                {
                    var @base = 10;
                    var defaultString = Default;

                    if (defaultString.StartsWith("0x"))
                    {
                        @base = 16;
                        defaultString = defaultString[2..];
                    }

                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return "0";
                    }

                    var val = Convert.ToUInt32(defaultString, @base);

                    return $"{val}";
                }
            case IFieldType.Int64FieldType:
                {
                    var @base = 10;
                    var defaultString = Default;

                    if (defaultString.StartsWith("0x"))
                    {
                        @base = 16;
                        defaultString = defaultString[2..];
                    }

                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return "0";
                    }

                    var val = Convert.ToInt64(defaultString, @base);

                    return $"{val}";
                }
            case IFieldType.UuidFieldType:
                {
                    if (string.IsNullOrEmpty(Default))
                    {
                        return "Guid.Empty";
                    }

                    Guid.Parse(Default);

                    return $"Guid.Parse(\"{Default}\")";
                }
            case IFieldType.Float64FieldType:
                {
                    if (string.IsNullOrEmpty(Default))
                    {
                        return "0.0";
                    }

                    var _ = Convert.ToDouble(Default);

                    return Default;
                }
            case IFieldType.StringFieldType when Default.Equals("null"):
                {
                    ValidateNullDefault();

                    return "null";
                }
            case IFieldType.StringFieldType:
                {
                    return string.IsNullOrWhiteSpace(Default) ? "string.Empty" : $"\"{Default}\"";
                }
            case IFieldType.BytesFieldType:
                {
                    if (Default.Equals("null"))
                    {
                        ValidateNullDefault();

                        return "null";
                    }

                    if (!string.IsNullOrWhiteSpace(Default))
                    {
                        throw new ArgumentException(
                            $"Invalid default for bytes field {Name}. The only valid default for a bytes field is empty or null.");
                    }

                    return ZeroCopy ? "new byte[0]" : "Array.Empty<byte>()";
                }
        }

        if (Type.IsRecords)
        {
            return "null";
        }

        if (Type.IsStruct)
        {
            if (!string.IsNullOrWhiteSpace(Default))
            {
                throw new ArgumentException($"Invalid default for struct field {Name}: custom defaults are not supported for struct fields.");
            }

            return $"new ()";
        }

        if (Type.IsArray)
        {
            if (Default.Equals("null"))
            {
                ValidateNullDefault();

                return "null";
            }

            if (!string.IsNullOrWhiteSpace(Default))
            {
                throw new ArgumentException(
                    $"Invalid default for array field {Name}. The only valid default for an array field is the empty array or null.");
            }

            return $"new ()";
        }

        throw new ArgumentException($"Unsupported field type {Type}");
    }

    internal string FieldAbstractClrType(StructRegistry structRegistry)
    {
        switch (Type)
        {
            case IFieldType.BoolFieldType:
            case IFieldType.Int8FieldType:
            case IFieldType.Int16FieldType:
            case IFieldType.UInt16FieldType:
            case IFieldType.Int32FieldType:
            case IFieldType.UInt32FieldType:
            case IFieldType.Int64FieldType:
            case IFieldType.UuidFieldType:
            case IFieldType.Float64FieldType:
            case IFieldType.StringFieldType:
            case IFieldType.BytesFieldType:
            case IFieldType.RecordsFieldType:
                return Type.ClrName;
            case IFieldType.StructType:
                return Type.ToString() + "Message";
            case IFieldType.ArrayType arrayType:
                {
                    if (structRegistry.IsStructArrayWithKeys(this))
                    {
                        return CollectionType(arrayType.ElementType.ToString());
                    }

                    if (arrayType.IsStructArray)
                    {
                        return $"List<{arrayType.ElementType.ClrName}Message>";
                    }

                    return $"List<{arrayType.ElementType.ClrName}>";
                }
            default:
                {
                    throw new Exception($"Unknown field type {Type}");
                }
        }
    }

    internal static string CollectionType(string baseType)
    {
        return baseType + "Collection";
    }

    private void ValidateNullDefault()
    {
        if (!NullableVersions.Contains(Versions))
        {
            throw new ArgumentException($"null cannot be the default for field {Name}, because not all versions of this field are nullable.");
        }
    }

    public void GenerateNonIgnorableFieldCheck(StructRegistry structRegistry, ICodeGenerator codeGenerator)
    {
        GenerateNonDefaultValueCheck(structRegistry, codeGenerator, NullableVersions);
        codeGenerator.IncrementIndent();
        codeGenerator.AppendLine($"throw new UnsupportedVersionException($\"Attempted to write a non-default {Name} at version {{version}}\");");
        codeGenerator.DecrementIndent();
        codeGenerator.AppendRightBrace();
    }

    public void GenerateNonDefaultValueCheck(
        StructRegistry structRegistry,
        ICodeGenerator codeGenerator,
        Versions nullableVersions)
    {
        var fieldDefault = FieldDefault();

        if (Type.IsArray)
        {
            if (fieldDefault.Equals("null"))
            {
                codeGenerator.AppendLine($"if ({Name} is not null)");
                codeGenerator.AppendLeftBrace();
            }
            else if (nullableVersions.IsEmpty)
            {
                codeGenerator.AppendLine($"if ({Name}.Count != 0)");
                codeGenerator.AppendLeftBrace();
            }
            else
            {
                codeGenerator.AppendLine($"if ({Name} is null || {Name}.Count != 0)");
                codeGenerator.AppendLeftBrace();
            }
        }
        else if (Type.IsBytes)
        {
            if (fieldDefault.Equals("null"))
            {
                codeGenerator.AppendLine($"if ({Name} is not null)");
                codeGenerator.AppendLeftBrace();
            }
            else if (nullableVersions.IsEmpty)
            {
                codeGenerator.AppendLine($"if ({Name}.Length != 0)");
                codeGenerator.AppendLeftBrace();
            }
            else
            {
                codeGenerator.AppendLine($"if ({Name} is null || {Name}.Length != 0)");
                codeGenerator.AppendLeftBrace();
            }
        }
        else if (Type.IsString || Type.IsStruct || Type is IFieldType.UuidFieldType)
        {
            if (fieldDefault.Equals("null"))
            {
                codeGenerator.AppendLine($"if ({Name} is not null)");
                codeGenerator.AppendLeftBrace();
            }
            else if (nullableVersions.IsEmpty)
            {
                codeGenerator.AppendLine($"if (!{Name}.Equals({fieldDefault}))");
                codeGenerator.AppendLeftBrace();
            }
            else
            {
                codeGenerator.AppendLine($"if ({Name} is null || !{Name}.Equals({fieldDefault}))");
                codeGenerator.AppendLeftBrace();
            }
        }
        else if (Type is IFieldType.BoolFieldType)
        {
            codeGenerator.AppendLine($"if ({(fieldDefault.Equals("true") ? "!" : "")}{Name})");
            codeGenerator.AppendLeftBrace();
        }
        else
        {
            codeGenerator.AppendLine($"if ({Name} != {fieldDefault})");
            codeGenerator.AppendLeftBrace();
        }
    }
}