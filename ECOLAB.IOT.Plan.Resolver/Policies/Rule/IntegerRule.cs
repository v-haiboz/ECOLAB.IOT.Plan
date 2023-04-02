namespace ECOLAB.IOT.Plan.Resolver.Policies.Rule
{
    using Newtonsoft.Json.Linq;

    public class IntegerRule
    {
        public string GenarateWhereStr(string json)
        {
            var obj = JObject.Parse(json);
            return Travel(obj);
        }
    }
}
