//using System.Collections.Specialized;
//using System.Security.Claims;
//using System.Text.Json;

//using Duende.IdentityModel;
//using Duende.IdentityServer.Models;
//using Duende.IdentityServer.Validation;

//using Moq;

//using SFC.Identity.Infrastructure.Validators;

//namespace SFC.Identity.Infrastructure.UnitTests.Validators;
//public class TokenExchangeGrantValidatorTests
//{
//    [Fact]
//    [Trait("Validator", "TokenExchangeGrant")]
//    public void Validator_TokenExchangeGrant_ShouldReturnValidGrantType()
//    {
//        // Arrange
//        Mock<ITokenValidator> validatorMock = new();
//        TokenExchangeGrantValidator validator = new(validatorMock.Object);

//        // Assert
//        Assert.Equal(OidcConstants.GrantTypes.TokenExchange, validator.GrantType);
//    }

//    [Fact]
//    [Trait("Validator", "TokenExchangeGrant")]
//    public async Task Validator_TokenExchangeGrant_ShouldReturnErrorWhenSubjectTokenIsEmpty()
//    {
//        // Arrange
//        Mock<ITokenValidator> validatorMock = new();
//        ExtensionGrantValidationContext context = new()
//        {
//            Request = new ValidatedTokenRequest
//            {
//                Raw = new NameValueCollection() {
//                    { OidcConstants.TokenRequest.SubjectToken, string.Empty }
//                }
//            }
//        };
//        TokenExchangeGrantValidator validator = new(validatorMock.Object);

//        // Act
//        await validator.ValidateAsync(context);

//        // Assert
//        Assert.True(context.Result.IsError);
//        Assert.Equal("invalid_request", context.Result.Error);
//    }

//    [Fact]
//    [Trait("Validator", "TokenExchangeGrant")]
//    public async Task Validator_TokenExchangeGrant_ShouldReturnErrorWhenSubjectTokenTypeIsNotAccessToken()
//    {
//        // Arrange
//        Mock<ITokenValidator> validatorMock = new();
//        ExtensionGrantValidationContext context = new()
//        {
//            Request = new ValidatedTokenRequest
//            {
//                Raw = new NameValueCollection() {
//                    { OidcConstants.TokenRequest.SubjectToken, "subject" },
//                    { OidcConstants.TokenRequest.SubjectTokenType, OidcConstants.TokenTypeIdentifiers.RefreshToken }
//                }
//            }
//        };
//        TokenExchangeGrantValidator validator = new(validatorMock.Object);

//        // Act
//        await validator.ValidateAsync(context);

//        // Assert
//        Assert.True(context.Result.IsError);
//    }

//    [Fact]
//    [Trait("Validator", "TokenExchangeGrant")]
//    public async Task Validator_TokenExchangeGrant_ShouldReturnErrorWhenAccessTokenValidationIsFailed()
//    {
//        // Arrange
//        string subjectToken = "subject";
//        Mock<ITokenValidator> validatorMock = new();
//        ExtensionGrantValidationContext context = new()
//        {
//            Request = new ValidatedTokenRequest
//            {
//                Raw = new NameValueCollection() {
//                    { OidcConstants.TokenRequest.SubjectToken, subjectToken },
//                    { OidcConstants.TokenRequest.SubjectTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken }
//                }
//            }
//        };
//        validatorMock.Setup(um => um.ValidateAccessTokenAsync(subjectToken, null)).ReturnsAsync(new TokenValidationResult { IsError = true });
//        TokenExchangeGrantValidator validator = new(validatorMock.Object);

//        // Act
//        await validator.ValidateAsync(context);

//        // Assert
//        Assert.True(context.Result.IsError);
//    }

//    [Fact]
//    [Trait("Validator", "TokenExchangeGrant")]
//    public async Task Validator_TokenExchangeGrant_ShouldReturnErrorWhenAccessTokenValidationResultHasEmptyClaims()
//    {
//        // Arrange
//        string subjectToken = "subject";
//        Mock<ITokenValidator> validatorMock = new();
//        ExtensionGrantValidationContext context = new()
//        {
//            Request = new ValidatedTokenRequest
//            {
//                Raw = new NameValueCollection() {
//                    { OidcConstants.TokenRequest.SubjectToken, subjectToken },
//                    { OidcConstants.TokenRequest.SubjectTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken }
//                },
//                Client = new Client()
//            }
//        };
//        validatorMock.Setup(um => um.ValidateAccessTokenAsync(subjectToken, null))
//            .ReturnsAsync(new TokenValidationResult
//            {
//                IsError = false,
//                Claims = null
//            });
//        TokenExchangeGrantValidator validator = new(validatorMock.Object);

//        // Act
//        await validator.ValidateAsync(context);

//        // Assert
//        Assert.True(context.Result.IsError);
//    }

//    [Fact]
//    [Trait("Validator", "TokenExchangeGrant")]
//    public async Task Validator_TokenExchangeGrant_ShouldPerformExhangeSuccessfully()
//    {
//        // Arrange
//        string subjectToken = "subject", subjectValue = "subject_value", clientIdValue = "client_id_value";
//        Mock<ITokenValidator> validatorMock = new();
//        ExtensionGrantValidationContext context = new()
//        {
//            Request = new ValidatedTokenRequest
//            {
//                Raw = new NameValueCollection() {
//                    { OidcConstants.TokenRequest.SubjectToken, subjectToken },
//                    { OidcConstants.TokenRequest.SubjectTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken }
//                },
//                Client = new Client { ClientId = clientIdValue }
//            }
//        };
//        validatorMock.Setup(um => um.ValidateAccessTokenAsync(subjectToken, null))
//            .ReturnsAsync(new TokenValidationResult
//            {
//                IsError = false,
//                Claims = [
//                    new Claim(JwtClaimTypes.Subject, subjectValue),
//                    new Claim(JwtClaimTypes.ClientId, clientIdValue)
//                ]
//            });
//        TokenExchangeGrantValidator validator = new(validatorMock.Object);

//        // Act
//        await validator.ValidateAsync(context);

//        // Assert
//        Assert.False(context.Result.IsError);
//        Assert.Equal(subjectValue, context.Result.Subject.FindFirstValue(JwtClaimTypes.Subject));
//        Assert.Equal(new() { { OidcConstants.TokenResponse.IssuedTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken } },
//            context.Result.CustomResponse);
//        Assert.Equal(clientIdValue, context.Request.ClientId);
//        Assert.Equal(validator.GrantType, context.Result.Subject.FindFirstValue(JwtClaimTypes.AuthenticationMethod));
//        Assert.Equal(JsonSerializer.Serialize(new { client_id = clientIdValue }),
//            context.Result.Subject.FindFirstValue(JwtClaimTypes.Actor));
//    }
//}