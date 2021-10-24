using System;
using System.Collections.Generic;
using System.IO;
using Microlibs.Kafka.Protocol.Responses;

namespace Microlibs.Kafka.Protocol
{
    internal static class DefaultResponseBuilder
    {
        public static ResponseMessage Create(ApiKeys apiKey, BinaryReader binaryReader, int responseLength)
        {
            return apiKey switch
            {
                ApiKeys.ApiVersions => BuildApiVersionResponse(binaryReader, responseLength),
            };
        }

        private static ApiVersionMessage BuildApiVersionResponse(BinaryReader binaryReader, int responseLength)
        {
            var errorCode = binaryReader.ReadInt16().Swap();

            if (errorCode != (short)ErrorCodes.None)
            {
                throw new ArgumentException("Ошибка получения запроса");
            }

            var lenArray = binaryReader.ReadInt32().Swap();

            var list = new List<ApiVersion>(lenArray);

            for (var i = 0; i < lenArray; i++)
            {
                var apiKey = binaryReader.ReadInt16().Swap();
                var minVersion = binaryReader.ReadInt16().Swap();
                var maxVersion = binaryReader.ReadInt16().Swap();

                list.Add(new ApiVersion(apiKey, minVersion, maxVersion));
            }

            var message = new ApiVersionMessage
            {
                ApiVersions = list
            };

            //ver 1 and ver 2
            if (binaryReader.BaseStream.Position < responseLength - 1)
            {
                var throttleTimeMs = binaryReader.ReadInt32().Swap();

                message = message with
                {
                    ThrottleTimeMs = throttleTimeMs
                };

                //ver 3
                if (binaryReader.BaseStream.Position < responseLength - 1)
                {
                }
            }

            return message;
        }
    }
}