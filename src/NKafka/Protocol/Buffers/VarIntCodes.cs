// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// 
//  PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com
// 
//  Copyright ©  2024 Aleksey Kalduzov. All rights reserved
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

namespace NKafka.Protocol.Buffers;

// This class fork from https://github.com/Cysharp/MemoryPack
internal static class VarIntCodes
{
    public const byte MAX_SINGLE_VALUE = 127;
    public const sbyte MIN_SINGLE_VALUE = -120;

    public const sbyte BYTE = -121;
    public const sbyte SBYTE = -122;
    public const sbyte UINT16 = -123;
    public const sbyte INT16 = -124;
    public const sbyte UINT32 = -125;
    public const sbyte INT32 = -126;
    public const sbyte UINT64 = -127;
    public const sbyte INT64 = -128;
}