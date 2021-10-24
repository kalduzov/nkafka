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

using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using NKafka.Connection.Sasl.Messages;

namespace NKafka.Connection.Sasl;

internal class ScramFormatter: IDisposable
{
    private static readonly Regex _equal = new("=", RegexOptions.Compiled);
    private static readonly Regex _comma = new(",", RegexOptions.Compiled);
    private static readonly Regex _equalTwoC = new("=2C", RegexOptions.Compiled);
    private static readonly Regex _equalThreeD = new("=3D", RegexOptions.Compiled);
    private readonly KeyedHashAlgorithm _mac;
    private readonly ScramMechanism _mechanism;

    private readonly HashAlgorithm _messageDigest;
    private readonly RandomNumberGenerator _random;

    public string SecureRandomString => GetSecureRandomString(_random);

    public ScramFormatter(ScramMechanism mechanism)
    {
        _mechanism = mechanism;
        _messageDigest = mechanism.GetHashAlgorithm();
        _mac = mechanism.GetMacAlgorithm();
        _random = RandomNumberGenerator.Create();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _messageDigest.Dispose();
        _mac.Dispose();
        _random.Dispose();
    }

    private static string GetSecureRandomString(RandomNumberGenerator generator)
    {
        var buffer = new byte[17];
        generator.GetBytes(buffer);
        var bigInt = new BigInteger(buffer);
        bigInt = BigInteger.Abs(bigInt);

        return bigInt.ToString("x8");
    }

    private byte[] Hmac(byte[] key, ReadOnlySpan<byte> bytes)
    {
        _mac.Key = key;

        var buf = new byte[_mac.HashSize >> 3];
        _mac.TryComputeHash(bytes, buf, out _);

        return buf;
    }

    private byte[] Hash(byte[] str)
    {
        return _messageDigest.ComputeHash(str);
    }

    public static string SaslName(string name)
    {
        var replace = _equal.Replace(name, "=3D");

        return _comma.Replace(replace, "=2C");
    }

    public static string UserName(string saslName)
    {
        var userName = _equalTwoC.Replace(saslName, ",");

        if (_equalThreeD.Replace(userName, "").Contains('='))
        {
            throw new ArgumentException($"Invalid username: {saslName}");
        }

        return _equalThreeD.Replace(userName, "=");
    }

    public byte[] SaltedPassword(string password, byte[] salt, int iterations)
    {
        var hashAlgorithmName = _mechanism.GetHashAlgorithmName();

        var pwd = password.Normalize(NormalizationForm.FormKC);

        return Rfc2898DeriveBytes.Pbkdf2(
            pwd,
            salt,
            iterations,
            hashAlgorithmName,
            hashAlgorithmName == HashAlgorithmName.SHA256 ? 32 : 64);
    }

    public byte[] ServerKey(byte[] saltedPassword)
    {
        var str = "Server Key"u8;

        return Hmac(saltedPassword, str);
    }

    private byte[] ClientKey(byte[] saltedPassword)
    {
        var str = "Client Key"u8;

        return Hmac(saltedPassword, str);
    }

    public byte[] ClientProof(
        byte[] saltedPassword,
        ClientFirstMessage clientFirstMessage,
        ServerFirstMessage serverFirstMessage,
        ClientFinalMessage clientFinalMessage)
    {
        var clientKey = ClientKey(saltedPassword);
        var storedKey = Hash(clientKey);
        var authMessage = AuthMessage(clientFirstMessage, serverFirstMessage, clientFinalMessage);
        var clientSignature = Hmac(storedKey, authMessage);

        return Xor(clientKey, clientSignature);
    }

    private static byte[] Xor(byte[] buffer1, IReadOnlyList<byte> buffer2)
    {
        for (var i = 0; i < buffer1.Length; i++)
        {
            buffer1[i] ^= buffer2[i];
        }

        return buffer1;
    }

    private static ReadOnlySpan<byte> AuthMessage(
        ClientFirstMessage clientFirstMessage,
        ServerFirstMessage serverFirstMessage,
        ClientFinalMessage clientFinalMessage)
    {
        var str =
            $"{clientFirstMessage.ClientFirstMessageBare()},{serverFirstMessage.ToMessage()},{clientFinalMessage.ClientFinalMessageWithoutProof()}";

        return Encoding.UTF8.GetBytes(str);
    }

    public byte[] ServerSignature(
        byte[] serverKey,
        ClientFirstMessage clientFirstMessage,
        ServerFirstMessage serverFirstMessage,
        ClientFinalMessage clientFinalMessage)
    {
        var authMessage = AuthMessage(clientFirstMessage, serverFirstMessage, clientFinalMessage);

        return Hmac(serverKey, authMessage);
    }
}