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

using NKafka.Exceptions;

namespace NKafka.Clients.Admin;

/// <summary>
/// Represents the result of a delete topics operation.
/// </summary>
/// <remarks>
/// The result contains information about whether the deletion encountered an error and, if so, the associated exception.
/// </remarks>
/// <param name="IsError">
/// Indicates whether an error occurred during the delete operation. A value of <c>true</c> means an error occurred.
/// </param>
/// <param name="Exception">
/// The exception associated with the error, if any. This is set to <c>null</c> if <paramref name="IsError"/> is <c>false</c>.
/// </param>
public record DeleteTopicsResult(bool IsError, KafkaException? Exception = null);