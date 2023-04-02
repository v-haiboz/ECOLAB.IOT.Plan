namespace ECOLAB.IOT.Plan.Service.Certifiction
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Certification;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using ECOLAB.IOT.Plan.Provider.Certification;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using System;
    using System.Net.NetworkInformation;
    using System.Text;
    using XAct;

    public interface ICertificationService
    {
        public TData<TokenInfoDto> GetToken(RequestTokenDto userWhiteListDto);
        public TData ValidateToken(string header);
    }

    public class CertificationService : ICertificationService
    {
        private readonly IUserWhiteListRepository _userWhiteListRepository;
        private readonly ICertificationProvider _certificationProvider;
        public CertificationService(IUserWhiteListRepository userWhiteListRepository, ICertificationProvider certificationProvider)
        {
            _userWhiteListRepository = userWhiteListRepository;
            _certificationProvider = certificationProvider;
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
    }
}
