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

using System.Text.Json.Serialization;

using NKafka.MessageGenerator.Converters;

namespace NKafka.MessageGenerator;

[JsonConverter(typeof(VersionsJsonConverter))]
public sealed class Versions
{
    public static readonly Versions None = new();

    public const string NONE_STRING = "none";

    public short Lowest { get; }

    public short Highest { get; }

    public Versions(short lowest, short highest)
    {
        if (lowest < 0 || highest < 0)
        {
            throw new ArgumentException("Invalid version range " + lowest + " to " + highest);
        }

        Lowest = lowest;
        Highest = highest;
    }

    public Versions()
        : this(0, -1)
    {
    }

    public static Versions? Parse(string input, Versions? defaultVersions = null!)
    {
        if (string.IsNullOrEmpty(input))
        {
            return defaultVersions;
        }

        var trimmedInput = input.Trim();

        if (trimmedInput.Length == 0)
        {
            return defaultVersions;
        }

        if (trimmedInput.Equals(NONE_STRING, StringComparison.OrdinalIgnoreCase))
        {
            return None;
        }

        if (trimmedInput.EndsWith('+'))
        {
            var lowest = short.Parse(trimmedInput[..1]);

            return new Versions(lowest, short.MaxValue);
        }
        else
        {
            var dashIndex = trimmedInput.IndexOf('-');

            if (dashIndex < 0)
            {
                var version = short.Parse(trimmedInput);

                return new Versions(version, version);
            }

            var lowest = short.Parse(trimmedInput[..dashIndex]);
            var highest = short.Parse(trimmedInput[(dashIndex + 1)..]);

            return new Versions(lowest, highest);
        }
    }
}