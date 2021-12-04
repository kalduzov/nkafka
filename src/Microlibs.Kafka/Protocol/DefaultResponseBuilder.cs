// using System;
// using Microlibs.Kafka.Protocol.Extensions;
// using Microlibs.Kafka.Protocol.Responses;
//
// namespace Microlibs.Kafka.Protocol
// {
//     internal static class DefaultResponseBuilder
//     {
//         public static ResponseMessage Create(ApiKeys apiKey, Memory<byte> memoryBuffer, int responseLength)
//         {
//             return apiKey switch
//             {
//                 //ApiKeys.ApiVersions => BuildApiVersionResponse(memoryBuffer, responseLength),
//                 ApiKeys.DescribeCluster => BuildDescribeClusterResponse(memoryBuffer, responseLength),
//             };
//         }
//
//         private static ResponseMessage BuildDescribeClusterResponse(Memory<byte> memory, int responseLength)
//         {
//             var trottleTimeMs = memory[..4].ToInt32();
//
//             var errorCode = memory.Slice(3,2).ToInt16();
//
//             if (errorCode != (short)StatusCodes.None)
//             {
//                 throw new ArgumentException("Ошибка получения запроса");
//             }
//
//             var errorMessageLen = memory.Slice(5,2).ToInt16();
//
//             if (errorMessageLen != 0)
//             {
//                 //var errorMessage = 
//             }
//
//             var clusterId = "";//memory.ReadCompactString();
//             var controllerId = 1;// memory.ReadInt32().Swap();
//
//             // var arrayLen = memory.ReadByte();
//             //
//             // if (arrayLen > 1)
//             // {
//             //     for (var i = 0; i < arrayLen - 1; i++)
//             //     {
//             //         var brokerId = memory.ReadInt32().Swap();
//             //         var host = memory.ReadCompactString();
//             //         var port = memory.ReadInt32().Swap();
//             //         var rack = memory.ReadCompactNullableString();
//             //     }
//             // }
//
//             var clusterAuthorizedOperations = 1;// memory.ReadInt32().Swap();
//
//             return new DescribeResponseMessage(trottleTimeMs, clusterId, controllerId, clusterAuthorizedOperations);
//         }
//
//         // private static ApiVersionMessage BuildApiVersionResponse(Memory<byte> memory, int responseLength)
//         // {
//         //     var errorCode = memory.ReadInt16().Swap();
//         //
//         //     if (errorCode != (short)ErrorCodes.None)
//         //     {
//         //         throw new ArgumentException("Ошибка получения запроса");
//         //     }
//         //
//         //     var lenArray = memory.ReadInt32().Swap();
//         //
//         //     var list = new List<ApiVersion>(lenArray);
//         //
//         //     for (var i = 0; i < lenArray; i++)
//         //     {
//         //         var apiKey = memory.ReadInt16().Swap();
//         //         var minVersion = memory.ReadInt16().Swap();
//         //         var maxVersion = memory.ReadInt16().Swap();
//         //
//         //         list.Add(new ApiVersion(apiKey, minVersion, maxVersion));
//         //     }
//         //
//         //     var message = new ApiVersionMessage
//         //     {
//         //         ApiVersions = list
//         //     };
//         //
//         //     //ver 1 and ver 2
//         //     if (memory.BaseStream.Position < responseLength - 1)
//         //     {
//         //         var throttleTimeMs = memory.ReadInt32().Swap();
//         //
//         //         message = message with
//         //         {
//         //             ThrottleTimeMs = throttleTimeMs
//         //         };
//         //
//         //         //ver 3
//         //         if (memory.BaseStream.Position < responseLength - 1)
//         //         {
//         //         }
//         //     }
//         //
//         //     return message;
//         // }
//     }
// }