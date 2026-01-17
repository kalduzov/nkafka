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

using System.IO.Compression;

namespace NKafka.Config;

/// <summary>
/// Represents the configuration settings for compression in the Kafka client.
/// </summary>
/// <remarks>
/// This class defines the compression type and optional levels for different compression algorithms.
/// It is used to configure the behavior of message compression for the producer.
/// </remarks>
public record CompressionConfig(CompressionType CompressionType = CompressionType.None)
{
    /// <summary>
    /// Gets the compression level to be used when the Gzip compression type is selected.
    /// </summary>
    /// <remarks>
    /// The level determines the trade-off between compression ratio and performance for Gzip.
    /// It is an optional setting that is applicable only if the <c>CompressionType</c> is set to <c>Gzip</c>.
    /// </remarks>
    /// <value>
    /// A <see cref="System.IO.Compression.CompressionLevel"/> representing the desired Gzip compression level.
    /// </value>
    public CompressionLevel GzipLevel { get; init; }

    /// <summary>
    /// Gets the compression level to be used when the LZ4 compression type is selected.
    /// </summary>
    /// <remarks>
    /// The level determines the trade-off between compression performance and efficiency for LZ4.
    /// It is an optional setting that is applicable only if the <c>CompressionType</c> is set to <c>Lz4</c>.
    /// </remarks>
    /// <value>
    /// An <see cref="int"/> representing the desired LZ4 compression level.
    /// </value>
    public int LZ4Level { get; init; }

    /// <summary>
    /// Gets the compression level to be used when the Zstandard (ZStd) compression type is selected.
    /// </summary>
    /// <remarks>
    /// The level determines the trade-off between compression ratio and performance for Zstandard.
    /// It is an optional setting that is applicable only if the <c>CompressionType</c> is set to <c>ZStd</c>.
    /// </remarks>
    /// <value>
    /// An <see cref="int"/> representing the desired Zstandard compression level.
    /// </value>
    public int ZstdLevel { get; init; }
}