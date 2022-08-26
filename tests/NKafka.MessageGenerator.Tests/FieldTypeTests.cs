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

using FluentAssertions;

using NKafka.MessageGenerator.Specifications;

using Xunit;

namespace NKafka.MessageGenerator.Tests;

public class FieldTypeTests
{
    [Theory]
    [InlineData("bool", typeof(IFieldType.BoolFieldType), "bool")]
    [InlineData("string", typeof(IFieldType.StringFieldType), "string")]
    [InlineData("int32", typeof(IFieldType.Int32FieldType), "int")]
    public void ParseTest(string value, Type result, string clrType)
    {
        var parseType = IFieldType.Parse(value);
        parseType.Should().BeOfType(result);
        parseType.ClrName.Should().Be(clrType);
    }
}