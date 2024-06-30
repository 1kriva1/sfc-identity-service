using System.Security.Claims;
using System.Text.Json;

using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

using IdentityModel;

namespace SFC.Identity.Infrastructure.Validators;
public class TokenExchangeGrantValidator(ITokenValidator validator) : IExtensionGrantValidator
{
    private readonly ITokenValidator _validator = validator;

    // register for urn:ietf:params:oauth:grant-type:token-exchange
    public string GrantType => OidcConstants.GrantTypes.TokenExchange;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        // default response is error
        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest);

        // the spec allows for various token types, most commonly you return an access token
        Dictionary<string, object> customResponse = new()
        {
            { OidcConstants.TokenResponse.IssuedTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken }
        };

        // read the incoming token
        string? subjectToken = context.Request.Raw.Get(OidcConstants.TokenRequest.SubjectToken);

        // and the token type
        string? subjectTokenType = context.Request.Raw.Get(OidcConstants.TokenRequest.SubjectTokenType);

        // mandatory parameters
        if (string.IsNullOrWhiteSpace(subjectToken))
        {
            return;
        }

        // for delegation scenario we require an access token
        if (!string.Equals(subjectTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken))
        {
            return;
        }

        // validate the incoming access token with the built-in token validator
        TokenValidationResult validationResult = await _validator.ValidateAccessTokenAsync(subjectToken);

        if (validationResult.IsError || validationResult.Claims is null)
        {
            return;
        }

        string sub = validationResult.Claims.First(c => c.Type == JwtClaimTypes.Subject).Value;

        string clientId = validationResult.Claims.First(c => c.Type == JwtClaimTypes.ClientId).Value;

        // set token client_id to original id
        context.Request.ClientId = clientId;

        var actor = new { client_id = context.Request.Client.ClientId };

        Claim actClaim = new(JwtClaimTypes.Actor, JsonSerializer.Serialize(actor), IdentityServerConstants.ClaimValueTypes.Json);

        context.Result = new GrantValidationResult(
            subject: sub,
            authenticationMethod: GrantType,
            claims: [actClaim],
            customResponse: customResponse);
    }
}
