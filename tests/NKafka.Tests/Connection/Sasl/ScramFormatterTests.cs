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

public class ScramFormatterTests
{
    /// <summary>
    ///     Tests that the formatter implementation produces the same values for the
    ///     example included in <a href="https://tools.ietf.org/html/rfc5802#section-5">RFC 7677</a>
    /// </summary>
    [Fact]
    public void Rfc7677Example()
    {
        var formatter = new ScramFormatter(ScramMechanism.ScramSha256);

        const string password = "pencil";
        const string c1 = "n,,n=user,r=rOprNGfwEbeRWgbNEkqO";
        const string s1 = "r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,s=W22ZaJ0SNY7soEsUEjb6gQ==,i=4096";
        const string c2 = "c=biws,r=rOprNGfwEbeRWgbNEkqO%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0,p=dHzbZapWIk4jUhN+Ute9ytag9zjfMHgsqmmiz7AndVQ=";
        const string s2 = "v=6rriTRBi23WpRR/wtup+mMhUZUn/dB5nLTJRsjl95G4=";

        var clientFirst = new ClientFirstMessage(Encoding.UTF8.GetBytes(c1));
        var serverFirst = new ServerFirstMessage(Encoding.UTF8.GetBytes(s1));
        var clientFinal = new ClientFinalMessage(Encoding.UTF8.GetBytes(c2));
        var serverFinal = new ServerFinalMessage(Encoding.UTF8.GetBytes(s2));

        clientFirst.SaslName.Should().Be("user");
        clientFirst.Nonce.Should().Be("rOprNGfwEbeRWgbNEkqO");

        serverFirst.Nonce[clientFirst.Nonce.Length..].Should().Be("%hvYDpWUa2RaTCAfuxFIlj)hNlF$k0");
        Convert.FromBase64String("W22ZaJ0SNY7soEsUEjb6gQ==").Should().BeEquivalentTo(serverFirst.Salt);
        serverFirst.Iterations.Should().Be(4096);

        Convert.FromBase64String("biws").Should().BeEquivalentTo(clientFinal.ChannelBinding.ToArray());
        Convert.FromBase64String("6rriTRBi23WpRR/wtup+mMhUZUn/dB5nLTJRsjl95G4=").Should().BeEquivalentTo(serverFinal.ServerSignature);

        var saltedPassword = formatter.SaltedPassword(password, serverFirst.Salt, serverFirst.Iterations);
        var serverKey = formatter.ServerKey(saltedPassword);
        var computedProof = formatter.ClientProof(saltedPassword, clientFirst, serverFirst, clientFinal);

        computedProof.Should().BeEquivalentTo(clientFinal.Proof);

        var computedSignature = formatter.ServerSignature(serverKey, clientFirst, serverFirst, clientFinal);
        computedSignature.Should().BeEquivalentTo(serverFinal.ServerSignature);

        ScramMechanism.ScramSha256.MinIterations().Should().Be(4096);
        ScramMechanism.ScramSha512.MinIterations().Should().Be(4096);
    }

    /// <summary>
    ///     Tests encoding of username
    /// </summary>
    [Theory]
    [InlineData("user1")]
    [InlineData("123")]
    [InlineData("1,2")]
    [InlineData("user=A")]
    [InlineData("user==B")]
    [InlineData("user,1")]
    [InlineData("user 1")]
    [InlineData(",")]
    [InlineData("=")]
    [InlineData(",=")]
    [InlineData("==")]
    public void SaslNameTest(string userName)
    {
        var saslName = ScramFormatter.SaslName(userName);
        saslName.IndexOf(',').Should().Be(-1);
        saslName.Replace("=2C", "").Replace("=3D", "").IndexOf('=').Should().Be(-1);

        ScramFormatter.UserName(saslName)
            .Should()
            .Be(userName);
    }
}