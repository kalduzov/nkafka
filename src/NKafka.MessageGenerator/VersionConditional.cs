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

internal sealed class VersionConditional
{
    private readonly Versions _containingVersions;
    private readonly Versions _possibleVersions;

    private Action<Versions>? _ifMember;
    private Action<Versions>? _ifNotMember;
    private bool _allowMembershipCheckAlwaysFalse = true;
    private bool _alwaysEmitBlockScope;

    private VersionConditional(Versions containingVersions, Versions possibleVersions)
    {
        _containingVersions = containingVersions;
        _possibleVersions = possibleVersions;
    }

    internal VersionConditional IfMember(Action<Versions> ifMember)
    {
        _ifMember = ifMember;

        return this;
    }

    internal VersionConditional IfNotMember(Action<Versions> ifMember)
    {
        _ifNotMember = ifMember;

        return this;
    }

    internal VersionConditional AllowMembershipCheckAlwaysFalse(bool allowMembershipCheckAlwaysFalse)
    {
        _allowMembershipCheckAlwaysFalse = allowMembershipCheckAlwaysFalse;

        return this;
    }

    internal VersionConditional AlwaysEmitBlockScope(bool alwaysEmitBlockScope)
    {
        _alwaysEmitBlockScope = alwaysEmitBlockScope;

        return this;
    }

    internal static VersionConditional ForVersions(
        Versions containingVersions,
        Versions possibleVersions)
    {
        return new VersionConditional(containingVersions, possibleVersions);
    }

    public void Generate(CodeBuffer codeBuffer)
    {
        var ifVersions = _possibleVersions.Intersect(_containingVersions);
        var ifNotVersions = _possibleVersions - _containingVersions ?? _possibleVersions;

        if (_possibleVersions.Lowest < _containingVersions.Lowest)
        {
            if (_possibleVersions.Highest > _containingVersions.Highest)
            {
                GenerateFullRangeCheck(ifVersions, ifNotVersions, codeBuffer);
            }
            else if (_possibleVersions.Highest >= _containingVersions.Lowest)
            {
                GenerateLowerRangeCheck(ifVersions, ifNotVersions, codeBuffer);
            }
            else
            {
                GenerateAlwaysFalseCheck(ifNotVersions, codeBuffer);
            }
        }
        else if (_possibleVersions.Highest >= _containingVersions.Lowest && _possibleVersions.Lowest <= _containingVersions.Highest)
        {
            if (_possibleVersions.Highest > _containingVersions.Highest)
            {
                GenerateUpperRangeCheck(ifVersions, ifNotVersions, codeBuffer);
            }
            else
            {
                GenerateAlwaysTrueCheck(ifVersions, codeBuffer);
            }
        }
        else
        {
            GenerateAlwaysFalseCheck(ifNotVersions, codeBuffer);
        }
    }

    private void GenerateAlwaysTrueCheck(Versions ifVersions, ICodeBuffer codeBuffer)
    {
        if (_ifMember is not null)
        {
            if (_alwaysEmitBlockScope)
            {
                codeBuffer.AppendLine("{");
                codeBuffer.IncrementIndent();
            }

            _ifMember(ifVersions);

            if (_alwaysEmitBlockScope)
            {
                codeBuffer.DecrementIndent();
                codeBuffer.AppendLine("}");
            }
        }
    }

    private void GenerateUpperRangeCheck(Versions ifVersions, Versions ifNotVersions, ICodeBuffer codeBuffer)
    {
        if (_ifMember is not null)
        {
            codeBuffer.AppendLine($"if (version <= ApiVersions.Version{_containingVersions.Highest})");
            codeBuffer.AppendLine("{");

            codeBuffer.IncrementIndent();
            _ifMember(ifVersions);
            codeBuffer.DecrementIndent();

            if (_ifNotMember is not null)
            {
                codeBuffer.AppendLine("}");
                codeBuffer.AppendLine("else");
                codeBuffer.AppendLine("{");

                codeBuffer.IncrementIndent();
                _ifNotMember(ifNotVersions);
                codeBuffer.DecrementIndent();
            }

            codeBuffer.AppendLine("}");
        }
        else if (_ifNotMember is not null)
        {
            codeBuffer.AppendLine($"if (version > ApiVersions.Version{_containingVersions.Highest})");
            codeBuffer.AppendLine("{");
            codeBuffer.IncrementIndent();
            _ifNotMember(ifNotVersions);
            codeBuffer.DecrementIndent();
            codeBuffer.AppendLine("}");
        }
    }

    private void GenerateAlwaysFalseCheck(Versions ifNotVersions, ICodeBuffer codeBuffer)
    {
        if (!_allowMembershipCheckAlwaysFalse)
        {
            throw new ArgumentException($"Version ranges {_containingVersions} and {_possibleVersions} have no versions in common.");
        }

        if (_ifNotMember is not null)
        {
            if (_alwaysEmitBlockScope)
            {
                codeBuffer.AppendLine("{");
                codeBuffer.IncrementIndent();
            }

            _ifNotMember(ifNotVersions);

            if (_alwaysEmitBlockScope)
            {
                codeBuffer.DecrementIndent();
                codeBuffer.AppendLine("}");
            }
        }
    }

    private void GenerateLowerRangeCheck(Versions ifVersions, Versions ifNotVersions, ICodeBuffer codeBuffer)
    {
        if (_ifMember is not null)
        {
            codeBuffer.AppendLine($"if (version >= ApiVersions.Version{_containingVersions.Lowest})");
            codeBuffer.AppendLine("{");

            codeBuffer.IncrementIndent();
            _ifMember(ifVersions);
            codeBuffer.DecrementIndent();

            if (_ifNotMember is not null)
            {
                codeBuffer.AppendLine("}");
                codeBuffer.AppendLine("else");
                codeBuffer.AppendLine("{");

                codeBuffer.IncrementIndent();
                _ifNotMember(ifNotVersions);
                codeBuffer.DecrementIndent();
            }

            codeBuffer.AppendLine("}");
        }
        else if (_ifNotMember is not null)
        {
            codeBuffer.AppendLine($"if (version < ApiVersions.Version{_containingVersions.Lowest})");
            codeBuffer.AppendLine("{");
            codeBuffer.IncrementIndent();
            _ifNotMember(ifNotVersions);
            codeBuffer.DecrementIndent();
            codeBuffer.AppendLine("}");
        }
    }

    private void GenerateFullRangeCheck(Versions ifVersions, Versions ifNotVersions, ICodeBuffer codeBuffer)
    {
        if (_ifMember is not null)
        {
            codeBuffer.AppendLine(
                $"if (version >= ApiVersions.Version{_containingVersions.Lowest} && version <= ApiVersions.Version{_containingVersions.Highest})");
            codeBuffer.AppendLine("{");

            codeBuffer.IncrementIndent();
            _ifMember(ifVersions);
            codeBuffer.DecrementIndent();

            if (_ifNotMember is not null)
            {
                codeBuffer.AppendLine("}");
                codeBuffer.AppendLine("else");
                codeBuffer.AppendLine("{");

                codeBuffer.IncrementIndent();
                _ifNotMember(ifNotVersions);
                codeBuffer.DecrementIndent();
            }

            codeBuffer.AppendLine("}");
        }
        else if (_ifNotMember is not null)
        {
            codeBuffer.AppendLine(
                $"if (version < ApiVersions.Version{_containingVersions.Lowest} || version > ApiVersions.Version{_containingVersions.Highest})");
            codeBuffer.AppendLine("{");
            codeBuffer.IncrementIndent();
            _ifNotMember(ifNotVersions);
            codeBuffer.DecrementIndent();
            codeBuffer.AppendLine("}");
        }
    }
}