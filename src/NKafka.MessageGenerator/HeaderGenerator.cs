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

public class HeaderGenerator: IHeaderGenerator
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
//
// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global";

    private readonly string _namespaceName;
    private readonly SortedSet<string> usings = new();

    public HeaderGenerator(string namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            throw new ArgumentNullException(nameof(namespaceName));
        }

        _namespaceName = namespaceName;
    }

    public void AppendUsing(string value)
    {
        usings.Add(value);
    }

    public StringBuilder Generate()
    {
        var builder = new StringBuilder();

        builder.AppendLine(_HEADER);
        builder.AppendLine();

        foreach (var @using in usings)
        {
            builder.AppendLine($"using {@using};");
        }

        builder.AppendLine();

        builder.AppendLine($"namespace {_namespaceName};");
        builder.AppendLine();

        return builder;
    }
}