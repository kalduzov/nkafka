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

namespace NKafka.MessageGenerator;

public sealed class HeaderGenerator: IHeaderGenerator
{
    private const string _HEADER = @"//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2022 Aleksey Kalduzov. All rights reserved
// 
//  Author: Aleksey Kalduzov
//  Email: alexei.kalduzov@gmail.com
// 
//  Licensed under the Apache License, Version 2.0 (the ""License"");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart";

    private readonly string _namespaceName;
    private readonly SortedSet<string> _using = new();
    private readonly ICodeGenerator _codeGenerator;

    public HeaderGenerator(string namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        _namespaceName = namespaceName;
        _codeGenerator = new CodeGenerator();
    }

    public void AppendUsing(string value)
    {
        _using.Add(value);
    }

    public void Generate()
    {
        _codeGenerator.AppendLine(_HEADER);
        _codeGenerator.AppendLine();

        foreach (var @using in _using)
        {
            _codeGenerator.AppendLine($"using {@using};");
        }

        _codeGenerator.AppendLine();

        _codeGenerator.AppendLine($"namespace {_namespaceName};");
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return _codeGenerator.ToString();
    }
}