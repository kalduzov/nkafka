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
//      https://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//
// THIS CODE IS AUTOMATICALLY GENERATED.  DO NOT EDIT.

// ReSharper disable RedundantUsingDirective
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable PartialTypeWithSinglePart

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public sealed class ApiVersionsResponseMessage: ResponseMessage
{
    /// <summary>
    /// The top-level error code.
    /// </summary>
    public short ErrorCode { get; set; } = 0;
    /// <summary>
    /// The APIs supported by the broker.
    /// </summary>
    public List<ApiVersionMessage> ApiKeys { get; set; } = new();
    /// <summary>
    /// Features supported by the broker.
    /// </summary>
    public List<SupportedFeatureKeyMessage>? SupportedFeatures { get; set; } = new();
    /// <summary>
    /// The monotonically increasing epoch for the finalized features information. Valid values are >= 0. A value of -1 is special and represents unknown epoch.
    /// </summary>
    public long? FinalizedFeaturesEpoch { get; set; } = -1;
    /// <summary>
    /// List of cluster-wide finalized features. The information is valid only if FinalizedFeaturesEpoch >= 0.
    /// </summary>
    public List<FinalizedFeatureKeyMessage>? FinalizedFeatures { get; set; } = new();



}
