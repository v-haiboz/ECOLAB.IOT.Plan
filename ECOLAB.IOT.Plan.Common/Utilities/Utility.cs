namespace ECOLAB.IOT.Plan.Common.Utilities
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using XAct;

    public class Utility
    {
        public static JsonSerializerSettings setting = new JsonSerializerSettings() {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static JsonSerializerSettings jsonSettingIgnoreNullValue = new JsonSerializerSettings {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
        };

        public static T JsonReplace<T>(T obj, string key, string replaceChar)
        {
            if (string.IsNullOrEmpty(key))
            {
                return obj;
            }

            var json = JsonConvert.SerializeObject(obj);
            JObject jobj = JObject.Parse(json);
            JToken result = jobj as JToken;
            JSON_SetChildNodes(ref result, key, replaceChar);
            return JsonConvert.DeserializeObject<T>(result.ToString());
        }

        private static void JSON_SetChildNodes(ref JToken jobj, string nodeName, string value)
        {
            try
            {
                JToken result = jobj as JToken;
                JToken result2 = result.DeepClone();
                                                    
                var reader = result.CreateReader();
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        if (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
                        {
                            Regex reg = new Regex(@"" + nodeName + "$");
                            if (reg.IsMatch(reader.Path))
                            {
                                result2.SelectToken(reader.Path).Replace(value);
                            }
                        }
                    }
                }

                jobj = result2;
            }
            catch (Exception ex)
            {
            }
        }


        public static string GetColmons<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return string.Join(",", obj.GetType().GetProperties().Select(p => p.Name).ToList());
        }

        public static string GetValues<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return string.Join(",", obj.GetType().GetProperties().Select(p => string.Format("'{0}'", p.GetValue(obj))).ToArray());
        }

        public static string RandomGenerateString(int length = 32)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(length);
            var strBuilder = new StringBuilder();
            foreach (var randomByt in randomBytes)
            {
                strBuilder.Append(randomByt.ToString("X2"));
            }

            return strBuilder.ToString();
        }

        public static bool ValidateToken(string header)
        {
            //Checking the header
            if (!string.IsNullOrEmpty(header) && header.StartsWith("Basic"))
            {
                //Extracting credentials
                // Removing "Basic " Substring
                string encodedUsernamePassword = header.Substring("Basic ".Length).Trim();
                //Decoding Base64
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));
                //Splitting Username:Password
                int seperatorIndex = usernamePassword.IndexOf(':');
                // Extracting the individual username and password
                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);
                //Validating the credentials
                if (username is "Admin" && password is "ECOLAB123") return true;
                else return false;
            }
            else
            {
                return false;
            }
        }
    }
}
