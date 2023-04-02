namespace ECOLAB.IOT.Plan.Provider.Certification
{
    using ECOLAB.IOT.Plan.Entity.Dtos.Certification;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using JWT.Exceptions;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using System;
    using System.Net.NetworkInformation;
    using System.Text;

    public interface ICertificationProvider
    {
        public TokenInfoDto GetToken(RequestTokenDto userWhiteListDto);

        public CertificationInfoDto GetDecodeToken(string token);

        public bool IsValidateFirstMACAddress(string mac);
    }

    public class CertificationProvider : ICertificationProvider
    {
        private static MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions() { });
        private static int _slidingExpiration = 5;// Minutes;
        private static string _secret = "ECOLinkGQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private static readonly string _headerBase64Url = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";

        private IECOLABIOTSecurityProvider _eCOLABIOTSecurityProvider;
        public CertificationProvider(IECOLABIOTSecurityProvider eCOLABIOTSecurityProvider)
        {
            _eCOLABIOTSecurityProvider = eCOLABIOTSecurityProvider;
        }

        public TokenInfoDto? GetToken(RequestTokenDto requestTokenDto)
        {
            if (_memoryCache.TryGetValue(requestTokenDto.Email, out var token))
            {
                return (TokenInfoDto)token;
            }

            var absoluteExpirationtime = DateTime.UtcNow.AddMinutes(_slidingExpiration);
            var datetTimeOffSet = new DateTimeOffset(absoluteExpirationtime);
            var newToken = new TokenInfoDto()
            {
                Access_Token = GenarateToken(requestTokenDto, absoluteExpirationtime),
                Expires_In = _slidingExpiration * 60,
                Ext_Expires_In = _slidingExpiration * 60,
                TokenType = "ECOLAB.IOT.Plan.TokenType"
            };
            _memoryCache.Set(requestTokenDto.Email, newToken, datetTimeOffSet);

            return newToken;

        }
        public CertificationInfoDto GetDecodeToken(string token)
        {
            return GetJwtDecode(token);
        }

        private string GenarateToken(RequestTokenDto requestTokenDto, DateTime absoluteExpirationtime)
        {
            var info = new CertificationInfoDto()
            {
                UserWhiteList = requestTokenDto,
                AbsoluteExpirationtime = absoluteExpirationtime,
                FirstMACAddress = GetFirstMACAddress()
            };

            return SetJwtEncode(info);
        }

        public bool IsValidateFirstMACAddress(string mac)
        {
            return string.Compare(mac, GetFirstMACAddress()) == 0;
        }

        public string SetJwtEncode(CertificationInfoDto payload)
        {
            var payloadJsonStr = JsonConvert.SerializeObject(payload);
            string payloadBase64Url = Base64UrlEncode(payloadJsonStr);
            var payloadBase64UrlSign = _eCOLABIOTSecurityProvider.AESEncrypt(payloadBase64Url);
            string sign = _eCOLABIOTSecurityProvider.AESEncrypt($"{_headerBase64Url}.{payloadBase64UrlSign}");//$"{_headerBase64Url}.{payloadBase64Url}".ToHMACSHA256String(secret);
            return $"{Base64UrlEncode(_headerBase64Url)}.{payloadBase64UrlSign}.{sign}";
        }

        public CertificationInfoDto? GetJwtDecode(string token)
        {
            try
            {
                var payload = Base64UrlDecode(_eCOLABIOTSecurityProvider.AESDecrypt(token.Split('.')[1]));
                var certificationInfo = JsonConvert.DeserializeObject<CertificationInfoDto>(payload);
                return certificationInfo;
            }
            catch (TokenExpiredException)
            {
                throw new Exception("expired");
            }
            catch (SignatureVerificationException)
            {
                throw new Exception("invalid");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Base64UrlEncode(string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            var base64 = Convert.ToBase64String(plainTextBytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return base64;
        }

        public string Base64UrlDecode(string base64UrlStr)
        {
            base64UrlStr = base64UrlStr.Replace('-', '+').Replace('_', '/');
            switch (base64UrlStr.Length % 4)
            {
                case 2:
                    base64UrlStr += "==";
                    break;
                case 3:
                    base64UrlStr += "=";
                    break;
            }
            var bytes = Convert.FromBase64String(base64UrlStr);
            return Encoding.UTF8.GetString(bytes);
        }

        private string GetFirstMACAddress()
        {
            var adapter = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();

            if (adapter == null)
            {
                return "FirstMACAddress";
            }

            PhysicalAddress address = adapter.GetPhysicalAddress();
            byte[] bytes = address.GetAddressBytes();
            var list = new List<string>();
            for (int i = 0; i < bytes.Length; i++)
            {
                list.Add(bytes[i].ToString("X2"));
            }

            return string.Join("-", list);
        }
    }
}
