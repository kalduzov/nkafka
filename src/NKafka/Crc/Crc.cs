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

using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace NKafka.Crc;

public static class Crc
{
    private static readonly ICrc32C _crc32C;

    static Crc()
    {
        if (Sse42.IsSupported)
        {
            _crc32C = new HardwareX86Crc32C();

            return;
        }

        if (Crc32.Arm64.IsSupported)
        {
            _crc32C = new ArmCrc32C();

            return;
        }

        _crc32C = new NativeCrc32C();
    }

    public static uint Calculate(Span<byte> span)
    {
        return _crc32C.Calculate(span);
    }
}