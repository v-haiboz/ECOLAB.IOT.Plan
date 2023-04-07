namespace ECOLAB.IOT.Plan.Service.Certifiction
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Certification;
    using ECOLAB.IOT.Plan.Provider.Certification;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;

    public interface ICertificationService
    {
        public TData<TokenInfoDto> GetToken(RequestTokenDto userWhiteListDto);
        public TData ValidateToken(string header);

        public Task<TData> ValidateAccessToken(string authorizationHeader);
    }

    public class CertificationService : ICertificationService
    {
        private readonly IUserWhiteListRepository _userWhiteListRepository;
        private readonly ICertificationProvider _certificationProvider;
        private readonly IConfiguration _config;
        public CertificationService(IUserWhiteListRepository userWhiteListRepository, ICertificationProvider certificationProvider, IConfiguration config)
        {
            _userWhiteListRepository = userWhiteListRepository;
            _certificationProvider = certificationProvider;
            _config = config;
        }
        public TData<TokenInfoDto> GetToken(RequestTokenDto requestTokenDto)
        {
            var result = new TData<TokenInfoDto>();
            try
            {
                if (requestTokenDto == null || string.IsNullOrEmpty(requestTokenDto.Email) || string.IsNullOrEmpty(requestTokenDto.SecretKey))
                {
                    result.Message = "Email And SecretKey cannot be empty.";
                    return result;
                }

                var bl = _userWhiteListRepository.CheckExpiredByEmailAndSecretKey(requestTokenDto.Email, requestTokenDto.SecretKey);
                if (!bl)
                {
                    result.Message = $"This {requestTokenDto.Email} email is illegal or SecretKey's expired.";
                    return result;
                }

                result.Tag = 1;
                result.Data = _certificationProvider.GetToken(requestTokenDto);
                return result;

            }
            catch (Exception ex)
            {
                result.Message = "Get Token failed.";
                result.Message = ex.Message;
                return result;
            }
        }

        public TData ValidateToken(string header)
        {
            var result = new TData();
            try
            {
                if (!string.IsNullOrEmpty(header))
                {
                    //Extracting credentials
                    // Removing "Basic " Substring
                    var certifictionInfo = _certificationProvider.GetDecodeToken(header);
                    if (certifictionInfo == null
                        || certifictionInfo.AbsoluteExpirationtime == default(DateTime)
                        || certifictionInfo.FirstMACAddress == null
                        || certifictionInfo.UserWhiteList == null
                        || string.IsNullOrEmpty(certifictionInfo.UserWhiteList.Email))
                    {
                        result.Message = "Invalidate Token.";
                        return result;
                    }

                    if (certifictionInfo.AbsoluteExpirationtime < DateTime.UtcNow)
                    {
                        result.Message = "Token expired.";
                        return result;
                    }

                    if (!_certificationProvider.IsValidateFirstMACAddress(certifictionInfo.FirstMACAddress))
                    {
                        result.Message = "Token is not for this service.";
                        return result;
                    }

                    var list = _userWhiteListRepository.GetUserWhiteList(certifictionInfo.UserWhiteList.Email);
                    if (list == null || list.Count == 0)
                    {
                        result.Message = $"This {certifictionInfo.UserWhiteList.Email} email is illegal.";
                        return result;
                    }

                    result.Tag = 1;
                    return result;
                }
                else
                {
                    result.Message = "Invalidate Token.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = "Invalidate Token.";
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<TData> ValidateAccessToken(string authorizationHeader) ///ClaimsPrincipal
        {
            var result = new TData();
            try
            {
                var accessToken = authorizationHeader.Substring("Bearer ".Length);

                if (string.IsNullOrEmpty(accessToken) || _config == null)
                {
                    result.Message = "accessToken is null or AAD config is null.";
                    return result;
                }

                var tenantId = _config["AAD:TenantId"];
                var clientId = _config["AAD:ClientId"];

                if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId))
                {
                    result.Message = "TenantId or ClientId is null.";
                    return result;
                }

                ConfigurationManager<OpenIdConnectConfiguration> configManager =
                    new ConfigurationManager<OpenIdConnectConfiguration>(
                        $"https://login.partner.microsoftonline.cn/{tenantId}/v2.0/.well-known/openid-configuration",
                        new OpenIdConnectConfigurationRetriever());

                OpenIdConnectConfiguration config = null;
                config = await configManager.GetConfigurationAsync();

                var tokenValidator = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    RequireSignedTokens = true,
                    ValidAudience = clientId,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKeys = config.SigningKeys,
                    ValidateIssuer = false,
                    //ValidIssuer = config.Issuer
                };

                SecurityToken securityToken;
                var _claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out securityToken);
                result.Tag = 1;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = "Validate AccessToken failed.";
                result.Description = ex.Message;
                return result;
            }
        }
    }
}
