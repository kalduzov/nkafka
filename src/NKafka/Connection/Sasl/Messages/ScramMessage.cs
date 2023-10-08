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

namespace NKafka.Connection.Sasl.Messages;

/// <summary>
/// 
/// </summary>
public abstract class ScramMessage
{
#pragma warning disable CS1591
    private const string _ALPHA = "[A-Za-z]+";
    private const string _BASE64_CHAR = "[a-zA-Z0-9/+]";
    private const string _VALUE = "[\\x01-\\x7F-[,]]+";

    protected const string VALUE_SAFE = "[\\x01-\\x7F-[=,]]+";
    protected const string PRINTABLE = "[\\x21-\\x7E-[,]]+";
    protected const string SASL_NAME = "(?:[\\x01-\\x7F-[=,]]|=2C|=3D)+";
    protected const string BASE64 = $"(?:{_BASE64_CHAR}{{4}})*(?:{_BASE64_CHAR}{{3}}=|{_BASE64_CHAR}{{2}}==)?";
    protected const string RESERVED = $"(m={_VALUE},)?";
    protected const string EXTENSIONS = $"(,{_ALPHA}={_VALUE})*";
#pragma warning restore CS1591

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract string ToMessage();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToMessage());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageBytes"></param>
    /// <returns></returns>
    protected static string ToMessage(Span<byte> messageBytes)
    {
        return Encoding.UTF8.GetString(messageBytes);
    }
}