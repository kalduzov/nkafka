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

public class CodeBuffer: ICodeBuffer
{
    private readonly Dictionary<int, string> _indents = new();

    private int IndentValue { get; set; }

    private readonly StringBuilder _builder = new();

    public void AppendLine(string value)
    {
        _builder.AppendLine($"{Indent}{value}");
    }

    public void AppendLine()
    {
        _builder.AppendLine();
    }

    public void Append(string value)
    {
        _builder.Append($"{Indent}{value}");
    }

    protected string Indent
    {
        get
        {
            {
                if (_indents.TryGetValue(IndentValue, out var result))
                {
                    return result;
                }

                result = string.Empty;

                for (var i = 0; i < IndentValue; i++)
                {
                    result += " ";
                }

                _indents.TryAdd(IndentValue, result);

                return result;
            }
        }
    }

    public void IncrementIndent(int value = ICodeBuffer.DEFAULT_INDENT)
    {
        IndentValue += value;
    }

    public void DecrementIndent(int value = ICodeBuffer.DEFAULT_INDENT)
    {
        IndentValue -= value;
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return _builder.ToString();
    }
}