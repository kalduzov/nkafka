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

namespace NKafka.Connection.Sasl;

/// <summary>
/// Performs SASL authentication as a client.
/// </summary>
/// <remarks>This a port SaslClient from java</remarks>
public interface ISaslClient: IDisposable
{
    /// <summary>
    /// Returns the IANA-registered mechanism name of this SASL client.
    /// (e.g. "CRAM-MD5", "GSSAPI").
    /// </summary>
    /// <returns>A non-null string representing the IANA-registered mechanism name.</returns>
    string MechanismName { get; }

    /// <summary>
    /// Determines whether this mechanism has an optional initial response.
    /// If true, caller should call <see cref="EvaluateChallenge"/> with an
    /// empty array to get the initial response.
    /// </summary>
    /// <returns>true if this mechanism has an initial response.</returns>
    bool HasInitialResponse { get; }

    /// <summary>
    /// Determines whether the authentication exchange has completed.
    /// This method may be called at any time, but typically, it
    /// will not be called until the caller has received indication
    /// from the server
    /// (in a protocol-specific manner) that the exchange has completed.
    /// </summary>
    /// <returns>true if the authentication exchange has completed; false otherwise.</returns>
    bool IsComplete { get; }

    /// <summary>
    /// Evaluates the challenge data and generates a response.
    /// If a challenge is received from the server during the authentication
    /// process, this method is called to prepare an appropriate next
    /// response to submit to the server.
    /// </summary>
    /// <param name="challenge">The non-null challenge sent from the server.
    /// The challenge array may have zero length.
    /// </param>
    /// <returns></returns>
    /// <exception cref="SaslException">If an error occurred while processing
    /// the challenge or generating a response.
    /// </exception>
    Span<byte> EvaluateChallenge(Span<byte> challenge);

    /// <summary>
    /// Unwraps a byte array received from the server.
    /// This method can be called only after the authentication exchange has
    /// completed (i.e., when <see cref="IsComplete"/> returns true) and only if
    /// the authentication exchange has negotiated integrity and/or privacy
    /// as the quality of protection; otherwise, an
    /// <see cref="Exception"/> is thrown.
    /// 
    /// <br/><br/>
    /// <b><paramref name="incoming"/></b> is the contents of the SASL buffer as defined in RFC 2222
    /// without the leading four octet field that represents the length.
    /// <b><paramref name="offset"/></b> and <b><paramref name="len"/></b> specify the portion of <b><paramref name="incoming"/></b>
    /// to use.
    /// 
    /// </summary>
    /// <param name="incoming">A non-null byte array containing the encoded bytes from the server.</param>
    /// <param name="offset">The starting position at <b><paramref name="incoming"/></b> of the bytes to use.</param>
    /// <param name="len">The number of bytes from <b><paramref name="incoming"/></b> to use.</param>
    /// <returns>A non-null byte array containing the decoded bytes.</returns>
    /// <exception cref="SaslException">if <b><paramref name="incoming"/></b> cannot be successfully unwrapped.</exception>
    /// <exception cref="Exception"> if the authentication exchange has not completed, or if the negotiated quality of protection
    /// has neither integrity nor privacy.
    /// </exception>
    Span<byte> UnWrap(Span<byte> incoming, int offset, int len);

    /// <summary>
    /// Wraps a byte array to be sent to the server.
    /// This method can be called only after the authentication exchange has
    /// completed (i.e., when <see cref="IsComplete"/> returns true) and only if
    /// the authentication exchange has negotiated integrity and/or privacy
    /// as the quality of protection; otherwise, an <see cref="Exception"/> is thrown.
    /// 
    /// <br/><br/>
    /// The result of this method will make up the contents of the SASL buffer
    /// as defined in RFC 2222 without the leading four octet field that
    /// represents the length.
    /// <b><paramref name="offset"/></b> and <b><paramref name="len"/></b> specify the portion of <b><paramref name="outgoing"/></b>
    /// to use.
    /// 
    /// </summary>
    /// <param name="outgoing">A non-null byte array containing the bytes to encode.</param>
    /// <param name="offset">The starting position at <b><paramref name="outgoing"/></b> of the bytes to use.</param>
    /// <param name="len">The number of bytes from <b><paramref name="outgoing"/></b> to use.</param>
    /// <returns>A non-null byte array containing the encoded bytes.</returns>
    /// <exception cref="SaslException">if <b><paramref name="outgoing"/></b> cannot be successfully wrapped.</exception>
    /// <exception cref="Exception"> if the authentication exchange has not completed, or if the negotiated quality of protection
    /// has neither integrity nor privacy.
    /// </exception>
    Span<byte> Wrap(Span<byte> outgoing, int offset, int len);

    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="propertyName"></param>
    // /// <typeparam name="T"></typeparam>
    // /// <returns></returns>
    // T GetNegotiatedProperty<T>(string propertyName);
}