// This is an independent project of an individual developer. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: https://pvs-studio.com

/*
 * Copyright © 2022 Aleksey Kalduzov. All rights reserved
 * 
 * Author: Aleksey Kalduzov
 * Email: alexei.kalduzov@gmail.com
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#pragma warning disable CS1591
namespace NKafka.Protocol.Records;

/// <summary>
/// 
/// </summary>
[Flags]
public enum BitMask: short
{
    Bit0 = 1 << 0, // 0000 0001
    Bit1 = 1 << 1, // 0000 0010
    Bit2 = 1 << 2, // 0000 0100
    Bit3 = 1 << 3, // 0000 1000
    TransactionalBit = 1 << 4, // 0001 0000
    ControlBit = 1 << 5, // 0010 0000
    Bit6 = 1 << 6, // 0100 0000
    Bit7 = 1 << 7, // 1000 0000
    Zero = 0 // 0000 0000
}