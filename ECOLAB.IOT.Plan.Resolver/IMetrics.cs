namespace ECOLAB.IOT.Plan.Resolver
{
    using ECOLAB.IOT.Plan.Entity.Entities;
    using Newtonsoft.Json.Linq;

    public interface IMetrics<T> where T : class
    {
        T Resolve(string json);
    }
}
