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

public sealed class BrokerRegistrationRequestMessage: RequestMessage
{
    /// <summary>
    /// The broker ID.
    /// </summary>
    public int BrokerId { get; set; } = 0;
    /// <summary>
    /// The cluster id of the broker process.
    /// </summary>
    public string ClusterId { get; set; } = null!;
    /// <summary>
    /// The incarnation id of the broker process.
    /// </summary>
    public Guid IncarnationId { get; set; } = Guid.Empty;
    /// <summary>
    /// The listeners of this broker
    /// </summary>
    public List<ListenerMessage> Listeners { get; set; } = new();
    /// <summary>
    /// The features on this broker
    /// </summary>
    public List<FeatureMessage> Features { get; set; } = new();
    /// <summary>
    /// The rack which this broker is in.
    /// </summary>
    public string Rack { get; set; } = null!;



}
