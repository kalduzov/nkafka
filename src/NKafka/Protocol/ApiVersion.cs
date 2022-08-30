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
namespace NKafka.Protocol;

public enum ApiVersion : short
{
    LastVersion = Version13,

    Version0 = 0x0000,
    Version1 = 0x0001,
    Version2 = 0x0002,
    Version3 = 0x0003,
    Version4 = 0x0004,
    Version5 = 0x0005,
    Version6 = 0x0006,
    Version7 = 0x0007,
    Version8 = 0x0008,
    Version9 = 0x0009,
    Version10 = 0x000A,
    Version11 = 0x000B,
    Version12 = 0x000C,
    Version13 = 0x000D
}