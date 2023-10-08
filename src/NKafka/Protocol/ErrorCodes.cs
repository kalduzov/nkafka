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

namespace NKafka.Protocol;

#pragma warning disable CS1591

/// <summary>
/// 
/// </summary>
public enum ErrorCodes: short
{
    /// <summary>
    ///  An error occurred on the server for which the client doesn't have a corresponding error code. This is generally an unexpected error.
    /// </summary>
    UnknownServerError = -0x1,

    /// <summary>
    /// No errors 
    /// </summary>
    None = 0x0,

    /// <summary>
    /// No reset policy has been defined, and the offsets for these partitions are either larger or smaller than the range of offsets the server has for the given partition.
    /// </summary>
    OffsetOutOfRange = 0x1,

    /// <summary>
    /// This error indicates a record has failed its internal CRC check, this generally indicates network or disk corruption.
    /// </summary>
    CorruptMessage = 0x2,

    /// <summary>
    /// 
    /// </summary>
    UnknownTopicOrPartition = 0x3,

    /// <summary>
    /// 
    /// </summary>
    InvalidFetchSize = 0x4,

    /// <summary>
    /// 
    /// </summary>
    LeaderNotAvailable = 0x5,

    /// <summary>
    /// 
    /// </summary>
    NotLeaderOrFollower = 0x6,

    /// <summary>
    /// 
    /// </summary>
    RequestTimedOut = 0x7,

    /// <summary>
    /// 
    /// </summary>
    BrokerNotAvailable = 0x8,

    /// <summary>
    /// 
    /// </summary>
    ReplicaNotAvailable = 0x9,

    /// <summary>
    /// 
    /// </summary>
    MessageTooLarge = 10,

    /// <summary>
    /// 
    /// </summary>
    StaleControllerEpoch = 11,

    /// <summary>
    /// 
    /// </summary>
    OffsetMetadataTooLarge = 12,

    /// <summary>
    /// 
    /// </summary>
    NetworkException = 13,
    CoordinatorLoadInProgress = 14,
    CoordinatorNotAvailable = 15,
    NotCoordinator = 16,
    InvalidTopicException = 17,
    RecordListTooLarge = 18,
    NotEnoughReplicas = 19,
    NotEnoughReplicasAfterAppend = 20,
    InvalidRequiredAcks = 21,
    IllegalGeneration = 22,
    InconsistentGroupProtocol = 23,
    InvalidGroupId = 24,
    UnknownMemberId = 25,
    InvalidSessionTimeout = 26,
    RebalanceInProgress = 27,
    InvalidCommitOffsetSize = 28,
    TopicAuthorizationFailed = 29,
    GroupAuthorizationFailed = 30,
    ClusterAuthorizationFailed = 31,
    InvalidTimestamp = 32,
    UnsupportedSaslMechanism = 33,
    IllegalSaslState = 34,
    UnsupportedVersion = 35,
    TopicAlreadyExists = 36,
    InvalidPartitions = 37,
    InvalidReplicationFactor = 38,
    InvalidReplicaAssignment = 39,
    InvalidConfig = 40,
    NotController = 41,
    InvalidRequest = 42,
    UnsupportedForMessageFormat = 43,
    PolicyViolation = 44,
    OutOfOrderSequenceNumber = 45,
    DuplicateSequenceNumber = 46,
    InvalidProducerEpoch = 47,
    InvalidTxnState = 48,
    InvalidProducerIdMapping = 49,
    InvalidTransactionTimeout = 50,
    ConcurrentTransactions = 51,
    TransactionCoordinatorFenced = 52,
    TransactionalIdAuthorizationFailed = 53,
    SecurityDisabled = 54,
    OperationNotAttempted = 55,
    KafkaStorageError = 56,
    LogDirNotFound = 57,
    SaslAuthenticationFailed = 58,
    UnknownProducerId = 59,
    ReassignmentInProgress = 60,
    DelegationTokenAuthDisabled = 61,
    DelegationTokenNotFound = 62,
    DelegationTokenOwnerMismatch = 63,
    DelegationTokenRequestNotAllowed = 64,
    DelegationTokenAuthorizationFailed = 65,
    DelegationTokenExpired = 66,
    InvalidPrincipalType = 67,
    NonEmptyGroup = 68,
    GroupIdNotFound = 69,
    FetchSessionIdNotFound = 70,
    InvalidFetchSessionEpoch = 71,
    ListenerNotFound = 72,
    TopicDeletionDisabled = 73,
    FencedLeaderEpoch = 74,
    UnknownLeaderEpoch = 75,
    UnsupportedCompressionType = 76,
    StaleBrokerEpoch = 77,
    OffsetNotAvailable = 78,
    MemberIdRequired = 79,
    PreferredLeaderNotAvailable = 80,
    GroupMaxSizeReached = 81,
    FencedInstanceId = 82,
    EligibleLeadersNotAvailable = 83,
    ElectionNotNeeded = 84,
    NoReassignmentInProgress = 85,
    GroupSubscribedToTopic = 86,
    InvalidRecord = 87,
    UnstableOffsetCommit = 88,
    ThrottlingQuotaExceeded = 89,
    ProducerFenced = 90,
    ResourceNotFound = 91,
    DuplicateResource = 92,
    UnacceptableCredential = 93,
    InconsistentVoterSet = 94,
    InvalidUpdateVersion = 95,
    FeatureUpdateFailed = 96,
    PrincipalDeserializationFailure = 97,
    SnapshotNotFound = 98,
    PositionOutOfRange = 99,
    UnknownTopicId = 100,
    DuplicateBrokerRegistration = 101,
    BrokerIdNotRegistered = 102,
    InconsistentTopicId = 103,
    InconsistentClusterId = 104,
    TransactionalIdNotFound = 105,
    FetchSessionTopicIdError = 106,
    IneligibleReplica = 107,
    NewLeaderElected = 108
}