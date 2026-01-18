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

using System.Security.Authentication;

using FluentAssertions;

using NKafka.Config;
using NKafka.Resources;

using Xunit;

namespace NKafka.Tests.Config;

public sealed class SslSettingsTests
{
    [Fact]
    public void Validate_WhenIsSetIsFalse_ShouldNotThrow()
    {
        // Arrange
        var settings = SslSettings.None;

        // Act & Assert
        FluentActions.Invoking(() => settings.Validate()).Should().NotThrow();
    }

    [Fact]
    public void Validate_WhenProtocolsIsNone_ShouldThrowArgumentException()
    {
        // Arrange
        var settings = new SslSettings
        {
            Protocols = SslProtocols.None
        };

        // Act & Assert
        FluentActions.Invoking(() => settings.Validate())
            .Should()
            .Throw<ArgumentException>()
            .WithMessage(ExceptionMessages.SslProtocolInvalid);
    }

    [Fact]
    public void Validate_WhenRootCertificateIsNullAndTrustServerCertificateIsFalse_ShouldThrowArgumentException()
    {
        // Arrange
        var settings = new SslSettings
        {
            RootCertificate = null,
            TrustServerCertificate = false
        };

        // Act & Assert
        FluentActions.Invoking(() => settings.Validate())
            .Should()
            .Throw<ArgumentException>()
            .WithMessage(ExceptionMessages.SslRootCertificateRequired);
    }

    [Fact]
    public void Validate_WhenRootCertificateIsSet_ShouldNotThrow()
    {
        // Arrange
        var settings = new SslSettings
        {
            RootCertificate = "path/to/cert"
        };

        // Act & Assert
        FluentActions.Invoking(() => settings.Validate()).Should().NotThrow();
    }

    [Fact]
    public void Validate_WhenTrustServerCertificateIsTrue_ShouldNotThrow()
    {
        // Arrange
        var settings = new SslSettings
        {
            TrustServerCertificate = true
        };

        // Act & Assert
        FluentActions.Invoking(() => settings.Validate()).Should().NotThrow();
    }
}