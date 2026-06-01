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

using System.Security.Cryptography;

namespace NKafka.Connection.Sasl;

internal enum ScramMechanism
{
    ScramSha256,
    ScramSha512
}

internal static class ScramMechanismExtensions
{
    extension(ScramMechanism mechanism)
    {
        public string GetName()
        {
            return mechanism switch
            {
                ScramMechanism.ScramSha256 => "SCRAM-SHA-256",
                ScramMechanism.ScramSha512 => "SCRAM-SHA-512",
                _ => throw new ArgumentOutOfRangeException(nameof(mechanism), mechanism, null)
            };
        }

        public int MinIterations()
        {
            return mechanism switch
            {
                ScramMechanism.ScramSha256 => 4096,
                ScramMechanism.ScramSha512 => 4096,
                _ => throw new ArgumentOutOfRangeException(nameof(mechanism), mechanism, null)
            };
        }

        public HashAlgorithm GetHashAlgorithm()
        {
            return mechanism switch
            {
                ScramMechanism.ScramSha256 => SHA256.Create(),
                ScramMechanism.ScramSha512 => SHA512.Create(),
                _ => throw new ArgumentOutOfRangeException(nameof(mechanism), mechanism, null)
            };
        }

        public KeyedHashAlgorithm GetMacAlgorithm()
        {
            return mechanism switch
            {
                ScramMechanism.ScramSha256 => new HMACSHA256(),
                ScramMechanism.ScramSha512 => new HMACSHA512(),
                _ => throw new ArgumentOutOfRangeException(nameof(mechanism), mechanism, null)
            };
        }

        public HashAlgorithmName GetHashAlgorithmName()
        {
            return mechanism switch
            {
                ScramMechanism.ScramSha256 => HashAlgorithmName.SHA256,
                ScramMechanism.ScramSha512 => HashAlgorithmName.SHA512,
                _ => throw new ArgumentOutOfRangeException(nameof(mechanism), mechanism, null)
            };
        }
    }
}
