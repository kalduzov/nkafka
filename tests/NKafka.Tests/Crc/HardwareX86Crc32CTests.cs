//  This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2023 Aleksey Kalduzov. All rights reserved
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

using NKafka.Crc;

namespace NKafka.Tests.Crc;

public class HardwareX86Crc32CTests
{
    [SkippableTheory]
    [InlineData("test", 0x86A072C0)]
    [InlineData("Lorem Ipsum is simply dummy text of the printing and typesetting industry.", 0xC4DD7DD6)]
    [InlineData(
        "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's "
        + "standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a "
        + "type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, "
        + "remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing "
        + "Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
        0x43CF3B1D)]
    public void CalculateTest(string text, uint crc)
    {
        Skip.IfNot(System.Runtime.Intrinsics.X86.Sse42.IsSupported, "Sse42 on x86 is not support");

        var data = Encoding.UTF8.GetBytes(text);

        var crc32C = new HardwareX86Crc32C();

        var result = crc32C.Calculate(data);

        result.Should().Be(crc);
    }
}