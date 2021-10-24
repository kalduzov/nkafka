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

using System.Text;

using NKafka.MessageGenerator.Specifications;

namespace NKafka.MessageGenerator;

public class SupportVersionsGenerator
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly IHeaderGenerator _headerGenerator;

    public SupportVersionsGenerator(string ns)
    {
        _headerGenerator = new HeaderGenerator(ns);
        _codeGenerator = new CodeGenerator();
    }

    public StringBuilder Generate(IEnumerable<(string Name, short ApiKey, Versions FlexibleVersions, Versions Versions)> supportVersionsInformation)
    {
        _headerGenerator.AppendUsing("NKafka.Exceptions");
        _headerGenerator.Generate();

        GenerateSupportVersions(supportVersionsInformation);

        var result = new StringBuilder();
        result.Append(_headerGenerator);
        result.Append(_codeGenerator);

        return result;
    }

    private void GenerateSupportVersions(
        IEnumerable<(string Name, short ApiKey, Versions FlexibleVersions, Versions Versions)> supportVersionsInformation)
    {
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine("internal static partial class SupportVersionsExtensions");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();

        var versionsInformation = supportVersionsInformation as (string Name, short ApiKey, Versions FlexibleVersion, Versions versions)[]
                                  ?? supportVersionsInformation.ToArray();

        GenerateRequestHeaderVersions(versionsInformation);
        _codeGenerator.AppendLine();
        GenerateResponseHeaderVersions(versionsInformation);

        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();
    }

    private void GenerateRequestHeaderVersions(
        IEnumerable<(string Name, short ApiKey, Versions FlexibleVersion, Versions versions)> supportVersionsInformation)
    {
        _codeGenerator.AppendLine("public static ApiVersion GetRequestHeaderVersion(this ApiKeys apiKey, ApiVersion version)");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine("return apiKey switch");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();

        foreach (var info in supportVersionsInformation)
        {
            _codeGenerator.Append($"ApiKeys.{info.Name} => ");

            if (info.ApiKey == 7)
            {
                _codeGenerator.Append("ApiVersion.Version0");
            }
            else
            {
                VersionConditional
                    .ForVersions(info.FlexibleVersion, info.versions)
                    .AsTernary(true)
                    .IfMember(_ => { _codeGenerator.AppendWithoutIdent("ApiVersion.Version2"); })
                    .IfNotMember(_ => { _codeGenerator.AppendWithoutIdent("ApiVersion.Version1"); })
                    .Generate(_codeGenerator);
            }

            _codeGenerator.AppendWithoutIdent(",");
            _codeGenerator.AppendLine();
        }

        _codeGenerator.AppendLine("_ => throw new UnsupportedVersionException($\"Unsupported API key {apiKey}\")");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendLine("};");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();
    }

    private void GenerateResponseHeaderVersions(
        IEnumerable<(string Name, short ApiKey, Versions FlexibleVersion, Versions versions)> supportVersionsInformation)
    {
        _codeGenerator.AppendLine("public static ApiVersion GetResponseHeaderVersion(this ApiKeys apiKey, ApiVersion version)");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine("return apiKey switch");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();

        foreach (var info in supportVersionsInformation)
        {
            _codeGenerator.Append($"ApiKeys.{info.Name} => ");

            if (info.ApiKey == 18) //KIP-511
            {
                _codeGenerator.Append("ApiVersion.Version0");
            }
            else
            {
                VersionConditional
                    .ForVersions(info.FlexibleVersion, info.versions)
                    .AsTernary(true)
                    .IfMember(_ => { _codeGenerator.AppendWithoutIdent("ApiVersion.Version1"); })
                    .IfNotMember(_ => { _codeGenerator.AppendWithoutIdent("ApiVersion.Version0"); })
                    .Generate(_codeGenerator);
            }

            _codeGenerator.AppendWithoutIdent(",");
            _codeGenerator.AppendLine();
        }

        _codeGenerator.AppendLine("_ => throw new UnsupportedVersionException($\"Unsupported API key {apiKey}\")");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendLine("};");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();
    }
}