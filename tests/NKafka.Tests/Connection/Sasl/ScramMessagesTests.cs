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
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Text;

using NKafka.Connection.Sasl;
using NKafka.Connection.Sasl.Messages;

namespace NKafka.Tests.Connection.Sasl;

public class ScramMessagesTests
{
    public static readonly IEnumerable<object[]> ValidExtensions = new[]
    {
        new[]
        {
            "ext=val1"
        },
        new[]
        {
            "anotherext=name1=value1 name2=another test value \"\'!$[]()"
        },
        new[]
        {
            "first=val1,second=name1 = value ,third=123"
        }
    };

    public static readonly IEnumerable<object[]> InvalidExtensions = new[]
    {
        new[]
        {
            "ext1=value"
        },
        new[]
        {
            "ext"
        },
        new[]
        {
            "ext=value1,value2"
        },
        new[]
        {
            "ext=,"
        },
        new[]
        {
            "ext =value"
        }
    };

    public static readonly IEnumerable<object[]> ValidReserved = new[]
    {
        new[]
        {
            "m=reserved-value"
        },
        new[]
        {
            "m=name1=value1 name2=another test value \"\'!$[]()"
        }
    };

    public static readonly IEnumerable<object[]> InvalidReserved = new[]
    {
        new[]
        {
            "m"
        },
        new[]
        {
            "m=name,value"
        },
        new[]
        {
            "m=,"
        }
    };

    private readonly ScramFormatter _formatter;

    public ScramMessagesTests()
    {
        _formatter = new ScramFormatter(ScramMechanism.ScramSha256);
    }

    [Theory]
    [InlineData("someuser", "someuser", "someuser", "", false)]
    [InlineData("n,,n=testuser,r={0}", "testuser", "testuser", "", true)]
    [InlineData("n,,n=test=2Cuser,r={0}", "test=2Cuser", "test,user", "", true)]
    [InlineData("n,,n=test=3Duser,r={0}", "test=3Duser", "test=user", "", true)]
    [InlineData("n,a=testauthzid,n=testuser,r={0}", "testuser", "testuser", "testauthzid", true)]
    public void CreateClientFirstMessage_Successful(
        string testString,
        string validSaslName,
        string validUserName,
        string validAuthorizationId,
        bool createAsBytes)
    {
        var nonce = _formatter.SecureRandomString;

        if (createAsBytes)
        {
            var str = string.Format(testString, nonce);
            var cfm = new ClientFirstMessage(Encoding.UTF8.GetBytes(str));
            CheckClientFirstMessage(cfm, validSaslName, nonce, validAuthorizationId);
            ScramFormatter.UserName(cfm.SaslName).Should().Be(validUserName);

            cfm = new ClientFirstMessage(cfm.ToBytes());
            CheckClientFirstMessage(cfm, validSaslName, nonce, validAuthorizationId);
        }
        else
        {
            var cfm = new ClientFirstMessage(testString, nonce, new Dictionary<string, string>());
            CheckClientFirstMessage(cfm, validSaslName, nonce, validAuthorizationId);
        }
    }

    [Fact]
    public void CreateClientFirstMessage_Fail()
    {
        var nonce = _formatter.SecureRandomString;
        var str = $"n,x=something,n=testuser,r={nonce}";

        void CreateClientFirstMessage()
            => _ = new ClientFirstMessage(Encoding.UTF8.GetBytes(str));

        FluentActions.Invoking(CreateClientFirstMessage).Should().Throw<SaslException>();
    }

    [Theory]
    [MemberData(nameof(ValidReserved))]
    public void CreateClientFirstMessage_WithValidReserved_Successful(string reserved)
    {
        var nonce = _formatter.SecureRandomString;
        var str = $"n,,{reserved},n=testuser,r={nonce}";
        var cfm = new ClientFirstMessage(Encoding.UTF8.GetBytes(str));
        CheckClientFirstMessage(cfm, "testuser", nonce, string.Empty);
    }

    [Theory]
    [MemberData(nameof(InvalidReserved))]
    public void CreateClientFirstMessage_WithInvalidReserved_Fail(string reserved)
    {
        var nonce = _formatter.SecureRandomString;
        var str = $"n,,{reserved},n=testuser,r={nonce}";

        void CreateClientFirstMessage()
            => _ = new ClientFirstMessage(Encoding.UTF8.GetBytes(str));

        FluentActions.Invoking(CreateClientFirstMessage).Should().Throw<SaslException>();
    }

    [Theory]
    [MemberData(nameof(ValidExtensions))]
    public void CreateClientFirstMessage_WithValidExtensions_Successful(string extension)
    {
        var nonce = _formatter.SecureRandomString;
        var str = $"n,,n=testuser,r={nonce},{extension}";
        var cfm = new ClientFirstMessage(Encoding.UTF8.GetBytes(str));
        CheckClientFirstMessage(cfm, "testuser", nonce, string.Empty);
    }

    [Fact]
    public void ValidationClientFirstMessage_WithTokenAuth_Successful()
    {
        var nonce = _formatter.SecureRandomString;
        var str = $"n,,n=testuser,r={nonce},tokenauth=true";
        var cfm = new ClientFirstMessage(Encoding.UTF8.GetBytes(str));
        cfm.Extensions.TokenAuthenticated.Should().BeTrue();
    }

    private static void CheckClientFirstMessage(ClientFirstMessage message, string saslName, string nonce, string authorizationId)
    {
        saslName.Should().Be(message.SaslName);
        nonce.Should().Be(message.Nonce);
        authorizationId.Should().Be(message.AuthorizationId);
    }
}