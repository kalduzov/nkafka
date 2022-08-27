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

public sealed class IsNullConditional
{
    private readonly string _name;
    private Versions _nullableVersions = Versions.All;
    private Versions _possibleVersions = Versions.All;
    private bool _alwaysEmitBlockScope;
    private Action? _ifNull;
    private Action? _ifShouldNotBeNull;

    private readonly Func<string, bool, string> _conditionalGenerator = (name, negated) => negated ? $"{name} is not null" : $"{name} is null";

    private IsNullConditional(string name)
    {
        _name = name;
    }

    internal static IsNullConditional ForName(string name)
    {
        return new IsNullConditional(name);
    }

    public IsNullConditional PossibleVersions(Versions possibleVersions)
    {
        _possibleVersions = possibleVersions;

        return this;
    }

    public IsNullConditional NullableVersions(Versions nullableVersions)
    {
        _nullableVersions = nullableVersions;

        return this;
    }

    public IsNullConditional AlwaysEmitBlockScope(bool alwaysEmitBlockScope)
    {
        _alwaysEmitBlockScope = alwaysEmitBlockScope;

        return this;
    }

    public IsNullConditional IfNull(Action ifNull)
    {
        _ifNull = ifNull;

        return this;
    }

    public IsNullConditional IfShouldNotBeNull(Action ifShouldNotBeNull)
    {
        _ifShouldNotBeNull = ifShouldNotBeNull;

        return this;
    }

    public void Generate(ICodeBuffer codeBuffer)
    {
        if (_nullableVersions.Intersect(_possibleVersions).IsEmpty)
        {
            if (_ifShouldNotBeNull is not null)
            {
                if (_alwaysEmitBlockScope)
                {
                    codeBuffer.AppendLine("{");
                    codeBuffer.IncrementIndent();
                }

                _ifShouldNotBeNull();

                if (_alwaysEmitBlockScope)
                {
                    codeBuffer.DecrementIndent();
                    codeBuffer.AppendLine("}");
                }
            }
        }
        else
        {
            if (_ifNull is not null)
            {
                codeBuffer.AppendLine($"if ({_conditionalGenerator(_name, false)})");
                codeBuffer.AppendLine("{");
                codeBuffer.IncrementIndent();
                _ifNull();
                codeBuffer.DecrementIndent();

                if (_ifShouldNotBeNull is not null)
                {
                    codeBuffer.AppendLine("}");
                    codeBuffer.AppendLine("else");
                    codeBuffer.AppendLine("{");
                    codeBuffer.IncrementIndent();
                    _ifShouldNotBeNull();
                    codeBuffer.DecrementIndent();
                }

                codeBuffer.AppendLine("}");
            }
            else if (_ifShouldNotBeNull is not null)
            {
                codeBuffer.AppendLine($"if ({_conditionalGenerator(_name, true)})");
                codeBuffer.AppendLine("{");
                codeBuffer.IncrementIndent();
                _ifShouldNotBeNull();
                codeBuffer.DecrementIndent();
                codeBuffer.AppendLine("}");
            }
        }
    }
}