namespace ECOLAB.IOT.Plan.Provider.HttpClient
{
    using ECOLAB.IOT.Plan.Entity.Dtos.Certification;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using Flurl.Http;

    public interface IDMPFluentHttpProvider
    {
        public Task<TokenInfoDto> GetToken();

        //public Task<Root> GetTableSchema();

    }

    public class DMPFluentHttpProvider : IDMPFluentHttpProvider
    {
        private string _tokenUrl= "https://login.partner.microsoftonline.cn/2c3c280f-2394-453f-944d-6df0dd9c338e/oauth2/v2.0/token";
        private string _schemaUrl= "https://EcolinkDevApi-d.ecolab.com.cn/api/app-infos?pageIndex=1&pageSize=100";

        //public async Task<Root> GetTableSchema()
        //{
        //    var tokenInfo =await GetToken();
        //    var schame= await _schemaUrl.WithHeader("Authorization", $"Bearer {tokenInfo.Access_Token}").GetJsonAsync<Root>();
        //    return schame;
        //}

        public async Task<TokenInfoDto> GetToken()
        {
            var test = await _tokenUrl.PostUrlEncodedAsync(new
            {
                client_id = "dd609f2a-99e0-4c1d-bd9f-58bdd4573384",
                scope = "dd609f2a-99e0-4c1d-bd9f-58bdd4573384/.default",
                client_secret = "i~5x_Gy2UrC1~R-k8h.KbDjM8bA.Z4DbhS",
                grant_type = "client_credentials"
            }).ReceiveString();

            var result = await _tokenUrl.PostUrlEncodedAsync(new
            {
                client_id = "dd609f2a-99e0-4c1d-bd9f-58bdd4573384",
                scope = "dd609f2a-99e0-4c1d-bd9f-58bdd4573384/.default",
                client_secret = "i~5x_Gy2UrC1~R-k8h.KbDjM8bA.Z4DbhS",
                grant_type = "client_credentials"
            }).ReceiveJson<TokenInfoDto>();

            return result;
        }
    }
}
