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

using NKafka.Protocol;
using NKafka.Protocol.Extensions;
using NKafka.Protocol.Records;
using System.Text;

namespace NKafka.Messages;

public partial class LeaveGroupRequestMessage: RequestMessage
{
    /// <summary>
    /// The ID of the group to leave.
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// The member ID to remove from the group.
    /// </summary>
    public string MemberId { get; set; }

    /// <summary>
    /// List of leaving member identities.
    /// </summary>
    public IReadOnlyCollection<MemberIdentityMessage> Members { get; set; }

    public LeaveGroupRequestMessage()
    {
        ApiKey = ApiKeys.LeaveGroup;
        LowestSupportedVersion = ApiVersions.Version0;
        HighestSupportedVersion = ApiVersions.Version4;
    }

    public override void Read(BufferReader reader, ApiVersions version)
    {
    }

    public override void Write(BufferWriter writer, ApiVersions version)
    {
    }

    public class MemberIdentityMessage: Message
    {
        /// <summary>
        /// The member ID to remove from the group.
        /// </summary>
        public string MemberId { get; set; }

        /// <summary>
        /// The group instance ID to remove from the group.
        /// </summary>
        public string? GroupInstanceId { get; set; } = null;

        public MemberIdentityMessage()
        {
            LowestSupportedVersion = ApiVersions.Version0;
            HighestSupportedVersion = ApiVersions.Version4;
        }

        public override void Read(BufferReader reader, ApiVersions version)
        {
        }

        public override void Write(BufferWriter writer, ApiVersions version)
        {
        }
    }
}