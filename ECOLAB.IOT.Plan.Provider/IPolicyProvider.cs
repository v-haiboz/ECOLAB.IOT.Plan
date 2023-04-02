namespace ECOLAB.IOT.Plan.Provider
{
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos.SqlServer;
    using ECOLAB.IOT.Plan.Entity.SqlServer;

    public interface IPolicyProvider<Return,Policy> where Return : class
    {
        Task<HashSet<Return>> Resolve(Policy policy, ELinkSqlServer? eLinkSqlServer, ClearServer clearServer=null);
    }
}
