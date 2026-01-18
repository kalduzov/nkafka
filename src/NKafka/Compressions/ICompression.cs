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

namespace NKafka.Compressions;

/// <summary>
/// Defines the contract for data compression and decompression functionality.
/// Implementations of this interface provide specific compression algorithms for processing both
/// byte arrays and streams.
/// </summary>
public interface ICompression
{
    /// <summary>
    /// Decodes the provided compressed byte array into its original, uncompressed form.
    /// </summary>
    /// <param name="data">The compressed byte array to be decoded.</param>
    /// <returns>A byte array representing the original, uncompressed data.</returns>
    public byte[] Decode(byte[] data)
        => data;

    /// <summary>
    /// Decodes the provided compressed stream into its original, uncompressed form.
    /// </summary>
    /// <param name="stream">The compressed stream to be decoded.</param>
    /// <returns>A stream representing the original, uncompressed data.</returns>
    public Stream Decode(Stream stream)
        => stream;

    /// <summary>
    /// Encodes the provided byte array into a compressed form.
    /// </summary>
    /// <param name="data">The byte array to be compressed.</param>
    /// <returns>A byte array representing the compressed data.</returns>
    public byte[] Encode(byte[] data)
        => data;

    /// <summary>
    /// Encodes the provided stream into a compressed form.
    /// </summary>
    /// <param name="stream">The stream to be compressed.</param>
    /// <returns>A stream representing the compressed data.</returns>
    public Stream Encode(Stream stream)
        => stream;
}