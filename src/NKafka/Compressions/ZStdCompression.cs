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

using ZstdSharp;

namespace NKafka.Compressions;

internal sealed class ZStdCompression(int compressionLevel): ICompression
{
    public byte[] Encode(byte[] data)
    {
        using var compressor = new Compressor(compressionLevel);
        return compressor.Wrap(data).ToArray();
    }

    public byte[] Decode(byte[] data)
    {
        using var decompressor = new Decompressor();
        return decompressor.Unwrap(data).ToArray();
    }

    public Stream Encode(Stream stream)
        => new CompressionStream(stream, compressionLevel);

    public Stream Decode(Stream stream)
        => new DecompressionStream(stream);
}
