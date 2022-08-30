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

public class CodeGenerator: ICodeGenerator
{
    private readonly Dictionary<int, string> _indents = new();

    private int _indentValue;

    private readonly StringBuilder _builder = new();

    public void AppendLine(string value)
    {
        _builder.AppendLine($"{Indent}{value}");
    }

    public void AppendLine()
    {
        _builder.AppendLine();
    }

    public void AppendLeftBrace()
    {
        AppendLine("{");
    }

    public void AppendRightBrace()
    {
        AppendLine("}");
    }

    public void Append(string value)
    {
        _builder.Append($"{Indent}{value}");
    }

    public void AppendWithoutIdent(string value)
    {
        _builder.Append(value);
    }

    private string Indent
    {
        get
        {
            {
                if (_indents.TryGetValue(_indentValue, out var result))
                {
                    return result;
                }

                result = string.Empty;

                for (var i = 0; i < _indentValue; i++)
                {
                    result += " ";
                }

                _indents.TryAdd(_indentValue, result);

                return result;
            }
        }
    }

    public void IncrementIndent(int value = ICodeGenerator.DEFAULT_INDENT)
    {
        _indentValue += value;
    }

    public void DecrementIndent(int value = ICodeGenerator.DEFAULT_INDENT)
    {
        _indentValue -= value;
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}