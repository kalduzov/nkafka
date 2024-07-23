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

using NKafka.Config;
using NKafka.Connection.Sasl.Providers;

namespace NKafka.Tests.Connection.Sasl.Providers;

public class SaslPlaintTextProviderTests
{
    [Fact]
    public void CreateProvider_Successful()
    {
        var saslSettings = new SaslSettings();
        _ = new SaslPlaintTextProvider(saslSettings);
    }

    [Theory]
    [InlineData("", "", "AAA=")]
    [InlineData("test", "test", "AHRlc3QAdGVzdA==")]
    [InlineData("test", "test1", "AHRlc3QAdGVzdDE=")]
    public void GetAuthData_Successful(string userName, string password, string val)
    {
        var saslSettings = new SaslSettings
        {
            Password = password,
            UserName = userName,

        };
        var provider = new SaslPlaintTextProvider(saslSettings);
        var data = provider.GetAuthData();
        var value = Convert.FromBase64String(val);

        data.Length.Should().Be(value.Length);
        data.Should().BeEquivalentTo(value);
    }
}