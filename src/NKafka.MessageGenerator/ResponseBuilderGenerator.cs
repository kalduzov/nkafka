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

namespace NKafka.MessageGenerator;

public class ResponseBuilderGenerator
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly IHeaderGenerator _headerGenerator;

    public ResponseBuilderGenerator(string ns)
    {
        _headerGenerator = new HeaderGenerator(ns);
        _codeGenerator = new CodeGenerator();
    }

    public StringBuilder Generate(IEnumerable<(string ApiKeyName, short ApiKey, string ClassName)> responseInformation)
    {
        _headerGenerator.AppendUsing("NKafka.Exceptions");
        _headerGenerator.AppendUsing("NKafka.Messages");
        _headerGenerator.Generate();

        GenerateResponseBuilder(responseInformation);

        var result = new StringBuilder();
        result.Append(_headerGenerator);
        result.Append(_codeGenerator);

        return result;
    }

    private void GenerateResponseBuilder(IEnumerable<(string ApiKeyName, short ApiKey, string ClassName)> responseInformation)
    {
        _codeGenerator.AppendLine();
        _codeGenerator.AppendLine("internal static partial class ResponseBuilder");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();

        var responses = responseInformation as (string ApiKeyName, short ApiKey, string ClassName)[]
                                  ?? responseInformation.ToArray();

        _codeGenerator.AppendLine("public static IResponseMessage Build(ApiKeys apiKey, ApiVersion apiVersion, byte[] span)");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();
        _codeGenerator.AppendLine("var reader = new BufferReader(span);");
        _codeGenerator.AppendLine("var headerVersion = apiKey.GetResponseHeaderVersion(apiVersion);");
        _codeGenerator.AppendLine("ProcessHeader(reader, headerVersion);");
        _codeGenerator.AppendLine("return apiKey switch");
        _codeGenerator.AppendLeftBrace();
        _codeGenerator.IncrementIndent();

        foreach (var (apiKeyName, _, className) in responses)
        {
            _codeGenerator.AppendLine($"ApiKeys.{apiKeyName} => new {className}(reader, apiVersion),");
        }

        _codeGenerator.AppendLine("_ => throw new UnsupportedVersionException($\"Unsupported API key {apiKey}\")");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendLine("};");
        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();

        _codeGenerator.DecrementIndent();
        _codeGenerator.AppendRightBrace();
    }
}