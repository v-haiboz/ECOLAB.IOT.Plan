namespace ECOLAB.IOT.Plan.Provider
{
    public interface IMetricsProvider<Return, Metrics> where Return : class
    {
        Return Resolve(Metrics? metrics);
    }
}
