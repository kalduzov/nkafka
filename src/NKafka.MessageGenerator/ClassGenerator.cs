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

public abstract class ClassGenerator: IClassGenerator
{
    protected IHeaderGenerator HeaderGenerator { get; }

    protected ICodeGenerator CodeGenerator { get; }

    protected StructRegistry StructRegistry { get; }

    protected ClassGenerator(string ns)
    {
        HeaderGenerator = new HeaderGenerator(ns);
        CodeGenerator = new CodeGenerator();
        StructRegistry = new StructRegistry();
    }

    public StringBuilder Generate(MessageSpecification message)
    {
        InternalGenerate(message);

        HeaderGenerator.Generate();

        var result = new StringBuilder();
        result.Append(HeaderGenerator);
        result.Append(CodeGenerator);

        return result;
    }

    protected abstract void InternalGenerate(MessageSpecification message);
}