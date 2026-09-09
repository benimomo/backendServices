using System;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using AuthService.Infrastructure; 

namespace AuthService.UnitTests
{
    public class TokenServiceTests
    {
       
        [Fact]
        public void CreateToken_WhenValidInputs_ReturnsValidTokenString()
        {
            // Arrange 
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["Jwt:Key"]).Returns("CIopP9V+QJPzpgdZoy02zxTJXOHjqjQQkktKCKJrMaE=");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("AuthService");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("BookService");

            var tokenService = new TokenService(configMock.Object);

            //  Act
            var token = tokenService.CreateToken("1", "ali",Domain.Role.Admin);

            //  Assert 
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }
    }
}