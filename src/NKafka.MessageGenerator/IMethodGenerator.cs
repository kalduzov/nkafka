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

public interface IMethodGenerator
{
    void Generate(string className, StructSpecification structSpecification, Versions parentVersions, Versions messageFlexibleVersions);

    public Versions FieldFlexibleVersions(FieldSpecification field)
    {
        if (field.FlexibleVersions is null)
        {
            return MessageFlexibleVersions;
        }

        if (!MessageFlexibleVersions.Intersect(field.FlexibleVersions).Equals(field.FlexibleVersions))
        {
            throw new Exception(
                $"The flexible versions for field {field.Name} are {field.FlexibleVersions}, "
                + $"which are not a subset of the flexible versions for the message as a whole, which are {MessageFlexibleVersions}");
        }

        return field.FlexibleVersions;
    }

    Versions MessageFlexibleVersions { get; set; }
}