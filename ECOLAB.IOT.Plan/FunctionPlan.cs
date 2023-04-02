using ECOLAB.IOT.Plan.Common.Utilities;
using ECOLAB.IOT.Plan.Entity;
using ECOLAB.IOT.Plan.Entity.Dtos.Certification;
using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
using ECOLAB.IOT.Plan.Entity.ScheduleDtos;
using ECOLAB.IOT.Plan.Entity.SqlServer;
using ECOLAB.IOT.Plan.Service;
using ECOLAB.IOT.Plan.Service.Certifiction;
using ECOLAB.IOT.Plan.Service.Sql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ECOLAB.IOT.Plan
{
    public class Function1
    {
        private readonly IELinkSqlServerService _eLINKSqlServerService;
        private readonly IUserWhiteListService _userWhiteListService;
        private readonly ICertificationService _certificationService;
        private readonly ISqlTableClearScheduleService _sqlTableClearScheduleService;
        private readonly ISqlPlanParserService _sqlPlanParserService;
        private readonly ISqlPlanDispatcherService _sqlPlanDispatcherService;
        private readonly IELinkPlanHistoryService _eLinkPlanHistoryService;
        public Function1(IELinkSqlServerService eLINKSqlServerService, 
            IUserWhiteListService userWhiteListService, 
            ICertificationService certificationService,
            ISqlTableClearScheduleService sqlTableClearScheduleService,
            ISqlPlanParserService sqlPlanParserService,
            ISqlPlanDispatcherService sqlPlanDispatcherService,
            IELinkPlanHistoryService eLinkPlanHistoryService)
        {
            _eLINKSqlServerService = eLINKSqlServerService;
            _userWhiteListService = userWhiteListService;
            _certificationService = certificationService;
            _sqlTableClearScheduleService = sqlTableClearScheduleService;
            _sqlPlanParserService= sqlPlanParserService;
            _sqlPlanDispatcherService = sqlPlanDispatcherService;
            _eLinkPlanHistoryService= eLinkPlanHistoryService;
        }

        [FunctionName("ELINKSqlServerInsert")]
        [OpenApiSecurity("apikeyquery_auth",
                     SecuritySchemeType.ApiKey,
                     In = OpenApiSecurityLocationType.Header,
                     Name = "token")]
        [OpenApiOperation(operationId: "ELINKSqlServer", tags: new[] { "ELINKSqlServer" }, Summary = "Insert Sql Server information.", Description = "Insert Sql Server information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ELINKSqlServerDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> Insert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ELINKSqlServer/Insert")] HttpRequest req,
            ILogger log)
        {
            var headers = req.Headers["token"];
            var result= _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<ELINKSqlServerDto>(requestBody, Utility.setting);
            result = _eLINKSqlServerService.Insert(item);
            return new OkObjectResult(result);
        }

        [FunctionName("ELINKSqlServerUpdate")]
        [OpenApiSecurity("apikeyquery_auth",
                     SecuritySchemeType.ApiKey,
                     In = OpenApiSecurityLocationType.Header,
                     Name = "token")]
        [OpenApiOperation(operationId: "ELINKSqlServer", tags: new[] { "ELINKSqlServer" }, Summary = "Update Sql Server information.", Description = "Update Sql Server information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ELINKSqlServerDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ELINKSqlServer/Update")] HttpRequest req,
            ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<ELINKSqlServerDto>(requestBody, Utility.setting);
            result = _eLINKSqlServerService.Update(item);
            return new OkObjectResult(result);
        }

        [FunctionName("ELINKSqlServerDelete")]
        [OpenApiSecurity("apikeyquery_auth",
                     SecuritySchemeType.ApiKey,
                     In = OpenApiSecurityLocationType.Header,
                     Name = "token")]
        [OpenApiOperation(operationId: "ELINKSqlServer", tags: new[] { "ELINKSqlServer" }, Summary = "Delete Sql Server information.", Description = "Delete Sql Server information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ELINKSqlServerDeleteDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ELINKSqlServer/Delete")] HttpRequest req,
            ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<ELINKSqlServerDeleteDto>(requestBody, Utility.setting);
            result = _eLINKSqlServerService.Delete(item);
            return new OkObjectResult(result);
        }

        [FunctionName("ELINKSqlServerGetList")]
        [OpenApiSecurity("apikeyquery_auth",SecuritySchemeType.ApiKey,In = OpenApiSecurityLocationType.Header,Name = "token")]
        [OpenApiOperation(operationId: "ELINKSqlServer", tags: new[] { "ELINKSqlServer" }, Summary = "ELINKSqlServer GetList information.", Description = "It only get information from Server and DB.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "ServerName", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The ServerName parameter")]
        [OpenApiParameter(name: "DBName", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The DBName parameter")]
        [OpenApiParameter(name: "PageIndex", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageIndex parameter")]
        [OpenApiParameter(name: "PageSize", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageSize parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TDataPage<dynamic>), Summary = "result")]
        public async Task<IActionResult> ELINKSqlServerGetList(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ELINKSqlServer/GetList")] HttpRequest req,
    ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }

            var result_dynamic = new TDataPage<List<ELinkSqlServer>>();
            try
            {
                string ServerName = req.Query["ServerName"];
                string DBName = req.Query["DBName"];
                if (!int.TryParse(req.Query["PageIndex"], out var pageIndex))
                {
                    pageIndex = 1;
                }

                if (!int.TryParse(req.Query["PageSize"], out var pageSize))
                {
                    pageSize = 50;
                }

                result_dynamic = _eLINKSqlServerService.GetList(ServerName, DBName, pageIndex, pageSize);
                result_dynamic = Utility.JsonReplace(result_dynamic, "Password", "******");
                return new OkObjectResult(result_dynamic);
            }
            catch (Exception ex)
            {
                result_dynamic.Message = "ELINKSqlServer  Get  List failed";
                result_dynamic.Description = ex.Message;
                return await Task.FromResult(new OkObjectResult(result_dynamic));
            }
        }

        [FunctionName("ELINKSqlServerGetDetailList")]
        [OpenApiSecurity("apikeyquery_auth",SecuritySchemeType.ApiKey,In = OpenApiSecurityLocationType.Header,Name = "token")]
        [OpenApiOperation(operationId: "ELINKSqlServer", tags: new[] { "ELINKSqlServer" }, Summary = "ELINK SqlServer GetDetailList information.", Description = "It not only get information about Server and DB, but also get information about their corresponding tables.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "ServerName", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The ServerName parameter")]
        [OpenApiParameter(name: "DBName", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The DBName parameter")]
        [OpenApiParameter(name: "PageIndex", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageIndex parameter")]
        [OpenApiParameter(name: "PageSize", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageSize parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TDataPage<dynamic>), Summary = "result")]
        public async Task<IActionResult> ELINKSqlServerGetDetailList(
[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ELINKSqlServer/GetDetailList")] HttpRequest req,
ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }

            var result_dynamic = new TDataPage<List<ELinkSqlServer>>();
            try
            {
                string ServerName = req.Query["ServerName"];
                string DBName = req.Query["DBName"];
                if (!int.TryParse(req.Query["PageIndex"], out var pageIndex))
                {
                    pageIndex = 1;
                }

                if (!int.TryParse(req.Query["PageSize"], out var pageSize))
                {
                    pageSize = 50;
                }

                var list = _eLINKSqlServerService.GetELINKSqlServersByServerNameOrDBName(ServerName, DBName);
                list = Utility.JsonReplace(list, "Password", "******");
                result_dynamic.Tag = list.Tag;
                result_dynamic.Message = list.Message;
                result_dynamic.PageSize=pageSize;
                result_dynamic.PageIndex=pageIndex;
                result_dynamic.Total = list.Data.Count;
                result_dynamic.Data= list.Data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return await Task.FromResult(new OkObjectResult(result));
            }
            catch (Exception ex)
            {
                result.Message = "ELINKSqlServer  Get Detail List failed";
                result.Description = ex.Message;
                return await Task.FromResult(new OkObjectResult(result));
            }
           
        }

        [FunctionName("SqlTableClearScheduleInsert")]
        [OpenApiSecurity("apikeyquery_auth",
                    SecuritySchemeType.ApiKey,
                    In = OpenApiSecurityLocationType.Header,
                    Name = "token")]
        [OpenApiOperation(operationId: "SqlTableClearSchedule", tags: new[] { "SqlTableClearSchedule" }, Summary = "Insert SqlTableClearSchedule information.", Description = "Insert SqlTableClearSchedule information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SqlTableClearScheduleDto<dynamic>), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> SqlTableClearScheduleInsert(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SqlTableClearSchedule/Insert")] HttpRequest req,
           ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var result_dynamic = await _sqlTableClearScheduleService.Insert(requestBody);
            return new OkObjectResult(result_dynamic);
        }

        [FunctionName("SqlTableClearScheduleUpdate")]
        [OpenApiSecurity("apikeyquery_auth",
                    SecuritySchemeType.ApiKey,
                    In = OpenApiSecurityLocationType.Header,
                    Name = "token")]
        [OpenApiOperation(operationId: "SqlTableClearSchedule", tags: new[] { "SqlTableClearSchedule" }, Summary = "Update SqlTableClearSchedule information.", Description = "Update SqlTableClearSchedule information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SqlTableClearScheduleDto<dynamic>), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> SqlTableClearScheduleUpdate(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SqlTableClearSchedule/Update")] HttpRequest req,
           ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var result_dynamic = await _sqlTableClearScheduleService.Update(requestBody);
            return new OkObjectResult(result_dynamic);
        }

        [FunctionName("PolicyEnableOrDisable")]
        [OpenApiSecurity("apikeyquery_auth",
                    SecuritySchemeType.ApiKey,
                    In = OpenApiSecurityLocationType.Header,
                    Name = "token")]
        [OpenApiOperation(operationId: "SqlTableClearSchedule", tags: new[] { "SqlTableClearSchedule" }, Summary = "Enable Or Disable SqlTableClearSchedule.", Description = "Enable Or Disable SqlTableClearSchedule.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SqlTableClearScheduleEnableOrDisableDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> SqlTableClearScheduleEnableOrDisable(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SqlTableClearSchedule/EnableOrDisable")] HttpRequest req,
           ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<SqlTableClearScheduleEnableOrDisableDto>(requestBody, Utility.setting);
            result = await _sqlTableClearScheduleService.EnableOrDisable(item);
            return new OkObjectResult(result);
        }



        [FunctionName("PolicyDelete")]
        [OpenApiSecurity("apikeyquery_auth",
                    SecuritySchemeType.ApiKey,
                    In = OpenApiSecurityLocationType.Header,
                    Name = "token")]
        [OpenApiOperation(operationId: "SqlTableClearSchedule", tags: new[] { "SqlTableClearSchedule" }, Summary = "Insert Sql Server information.", Description = "Insert Sql Server information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SqlTableClearScheduleDeleteDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> SqlTableClearSchedulePolicyDelete(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SqlTableClearSchedule/Delete")] HttpRequest req,
           ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<SqlTableClearScheduleDeleteDto>(requestBody, Utility.setting);
            result = _sqlTableClearScheduleService.Delete(item);
            return new OkObjectResult(result);
        }



        [FunctionName("GetTableClearSchedulesByServerNameOrDBName")]
        [OpenApiSecurity("apikeyquery_auth",SecuritySchemeType.ApiKey,In = OpenApiSecurityLocationType.Header,Name = "token")]
        [OpenApiOperation(operationId: "SqlTableClearSchedulesByServerNameOrDBName", tags: new[] { "SqlTableClearSchedule" }, Summary = "SqlTableClearSchedule.", Description = "SqlTableClearSchedule information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "ServerName", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The ServerName parameter")]
        [OpenApiParameter(name: "DBName", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The DBName parameter")]
        [OpenApiParameter(name: "PageIndex", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageIndex parameter")]
        [OpenApiParameter(name: "PageSize", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageSize parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TDataPage<dynamic>), Summary = "result")]
        public async Task<IActionResult> GetTableClearSchedules(
[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SqlTableClearSchedule/GetList")] HttpRequest req,
ILogger log)
        {
            var headers = req.Headers["token"];
            var result = _certificationService.ValidateToken(headers);
            if (result.Tag == 0)
            {
                return new UnauthorizedResult();
                //return new OkObjectResult(HttpStatusCode.Unauthorized);
            }

            var result_dynamic = new TDataPage<dynamic>();
            try
            {
                string ServerName = req.Query["ServerName"];
                string DBName = req.Query["DBName"];
                if (!int.TryParse(req.Query["PageIndex"], out var pageIndex))
                {
                    pageIndex = 1;
                }

                if (!int.TryParse(req.Query["PageSize"], out var pageSize))
                {
                    pageSize = 100;
                }

                var items = _eLINKSqlServerService.GetELINKSqlServersByServerNameOrDBName(ServerName, DBName);
                var clearPlans = await _sqlPlanParserService.BuildClearPlans(items.Data);
                var list = clearPlans.ToConvertTableClearScheduleRecordDisplays();
                result_dynamic.Data= list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result_dynamic.Tag = 1;
                result_dynamic.PageIndex = pageIndex;
                result_dynamic.PageSize= pageSize;
                result_dynamic.Total = list.Count;
                result_dynamic.Message = "Get SqlTableClearSchedule list sccessful.";
                return new OkObjectResult(result_dynamic);
            }
            catch (Exception ex)
            {
                result_dynamic.Message = "Get SqlTableClearSchedule list failed.";
                result_dynamic.Description = ex.Message;
                return new OkObjectResult(result_dynamic); 
            }
        }


        [FunctionName("UserWhiteListCreate")]
        [OpenApiSecurity("basic_auth",
                     SecuritySchemeType.Http,
                     Scheme = OpenApiSecuritySchemeType.Basic)]
        [OpenApiOperation(operationId: "UserWhiteList", tags: new[] { "UserWhiteList" }, Summary = "Create User WhiteList information.", Description = "Delete Sql Server information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserWhiteListDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData<SecretDto>), Summary = "result")]
        public async Task<IActionResult> UserWhiteListInsert(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UserWhiteList/Create")] HttpRequest req,
          ILogger log)
        {
            var headers = req.Headers["Authorization"];
            if (Utility.ValidateToken(headers))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var item = JsonConvert.DeserializeObject<UserWhiteListDto>(requestBody, Utility.setting);
                var result = _userWhiteListService.Create(item);
                return new OkObjectResult(result);
            }
            else
            {
                string encodedUsernamePassword = headers.ToString().Substring("Basic ".Length).Trim();
                //Decoding Base64
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                //Splitting Username:Password
                int seperatorIndex = usernamePassword.IndexOf(':');
                // Extracting the individual username and password
                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);
                //Validating the credentials
                return new OkObjectResult(new { usernamePassword, password });
            }
        }

        [FunctionName("UserWhiteListRefresh")]
        [OpenApiSecurity("basic_auth",
                     SecuritySchemeType.Http,
                     Scheme = OpenApiSecuritySchemeType.Basic)]
        [OpenApiOperation(operationId: "UserWhiteListRefresh", tags: new[] { "UserWhiteList" }, Summary = "Create User WhiteList information.", Description = "Delete Sql Server information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserWhiteListDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData<SecretDto>), Summary = "result")]
        public async Task<IActionResult> UserWhiteListRefresh(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UserWhiteList/Refresh")] HttpRequest req,
          ILogger log)
        {
            var headers = req.Headers["Authorization"];
            if (Utility.ValidateToken(headers))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var item = JsonConvert.DeserializeObject<UserWhiteListDto>(requestBody, Utility.setting);
                var result = _userWhiteListService.Refresh(item);
                return new OkObjectResult(result);
            }
            else
            {
                return new UnauthorizedResult();
            }
        }

        [FunctionName("UserWhiteListDelete")]
        [OpenApiSecurity("basic_auth",
                     SecuritySchemeType.Http,
                     Scheme = OpenApiSecuritySchemeType.Basic)]
        [OpenApiOperation(operationId: "UserWhiteList", tags: new[] { "UserWhiteList" }, Summary = "Delete User WhiteList information.", Description = "Delete User WhiteList information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserWhiteListDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData), Summary = "result")]
        public async Task<IActionResult> UserWhiteListDelete(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UserWhiteList/Delete")] HttpRequest req,
           ILogger log)
        {
            var headers = req.Headers["Authorization"];
            if (Utility.ValidateToken(headers))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var item = JsonConvert.DeserializeObject<UserWhiteListDto>(requestBody, Utility.setting);
                var result = _userWhiteListService.Delete(item);
                return new OkObjectResult(result);
            }
            else
            {
                return new UnauthorizedResult();
            }
        }


        [FunctionName("UserWhiteListGetListByEmail")]
        //[OpenApiSecurity("basic_auth",
        //             SecuritySchemeType.Http,
        //             Scheme = OpenApiSecuritySchemeType.Basic)]
        [OpenApiOperation(operationId: "UserWhiteList", tags: new[] { "UserWhiteList" }, Summary = "Get User WhiteList information.", Description = "Get User WhiteList information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "Email", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The Email parameter")]
        [OpenApiParameter(name: "PageIndex", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageIndex parameter")]
        [OpenApiParameter(name: "PageSize", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The PageSize parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TDataPage<List<UserWhiteList>>), Summary = "result")]
        public async Task<IActionResult> UserWhiteListGetListByEmail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "UserWhiteList/GetListByEmail")] HttpRequest req,
            ILogger log)
        {
            var result = new TDataPage<List<UserWhiteList>>(); 
            var queryParameters = req.Query;
            string email = null;
            if (queryParameters.TryGetValue("Email", out var email_str))
            {
                email = email_str;
            }

            var _pageIndex = 1;
            if (queryParameters.TryGetValue("PageIndex", out var pageIndex_str))
            {
                if (int.TryParse(pageIndex_str, out var pageIndex))
                {
                    _pageIndex = (int)pageIndex;
                }
                else
                {
                    result.Message = "PageIndex must be int.";
                    return new OkObjectResult(result);
                }
            }
            var _pageSize = 50;
            if (queryParameters.TryGetValue("PageSize", out var pageSize_str))
            {
                if (int.TryParse(pageSize_str, out var pageSize))
                {
                    _pageSize = pageSize;
                }
                else
                {
                    result.Message = "PageSize must be int.";
                    return new OkObjectResult(result);
                }
            }

            result = _userWhiteListService.GetUserWhiteList(email, _pageIndex, _pageSize);
            result = Utility.JsonReplace(result, "Password", "******");
            return await Task.FromResult(new OkObjectResult(result));
        }

        [FunctionName("Test")]
        [OpenApiOperation(operationId: "UserWhiteList", tags: new[] { "Plan" }, Summary = "Get User WhiteList information.", Description = "Get User WhiteList information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TDataPage<dynamic>), Summary = "result")]
        public async Task<IActionResult> Test(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Plan/Test")] HttpRequest req,
            ILogger log)
        {
            var expiryTime = DateTime.UtcNow.AddHours(1);
            var plans=await _sqlPlanParserService.Execute();
            _sqlPlanDispatcherService.Execute(plans, expiryTime);
            return await Task.FromResult(new OkObjectResult(plans));
        }

        [FunctionName("UserWhiteListGetToken")]
        [OpenApiOperation(operationId: "UserWhiteListGetToken", tags: new[] { "UserWhiteList" }, Summary = "Insert Sql Server information.", Description = "Insert Sql Server information.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(RequestTokenDto), Description = "Feed reader request payload")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TData<string>), Summary = "result")]
        public async Task<IActionResult> UserWhiteListGetToken(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UserWhiteList/GetToken")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var item = JsonConvert.DeserializeObject<RequestTokenDto>(requestBody, Utility.setting);
            var result = _certificationService.GetToken(item);
            return new OkObjectResult(result); 
        }

        [Timeout("08:00:00")]
        [FunctionName("TimerTriggerCSharp")]
        public async Task TimerTrigger([TimerTrigger("0 55 */2 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                var traceLog = new ELinkPlanHistoryDto()
                {
                    Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                    Message = $"ExecuteClearPlan Start",
                    TargetRowData = null,
                    SourceRowData = $"StartTime: {DateTime.UtcNow}"
                };

                _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
                var expiryTime = DateTime.UtcNow.AddHours(1);
                var plans = await _sqlPlanParserService.Execute();
                _sqlPlanDispatcherService.Execute(plans, expiryTime);

                traceLog = new ELinkPlanHistoryDto()
                {
                    Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                    Message = $"ExecuteClearPlan Finished",
                    TargetRowData = $"EndTime: {DateTime.UtcNow}",
                    SourceRowData = null
                };

                _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
            }
            catch (Exception ex)
            {
                var traceLog = new ELinkPlanHistoryDto()
                {
                    Category = ELinkPlanHistoryCategory.ExecuteClearPlan,
                    Message = $"ExecuteClearPlan failed",
                    TargetRowData = $"EndTime: {DateTime.UtcNow} {ex.Message}",
                    SourceRowData = null
                };

                _eLinkPlanHistoryService.WriteInfoMessage(traceLog);
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }

}
