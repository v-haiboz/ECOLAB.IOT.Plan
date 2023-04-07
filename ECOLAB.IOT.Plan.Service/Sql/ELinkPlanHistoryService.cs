namespace ECOLAB.IOT.Plan.Service.Sql
{
    using ECOLAB.IOT.Plan.Common.Utilities;
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.ScheduleDtos;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;

    public interface IELinkPlanHistoryService
    {
        public TData WriteErrorMessage(ELinkPlanHistoryDto eLinkPlanHistoryDto);

        public TData WriteInfoMessage(ELinkPlanHistoryDto eLinkPlanHistoryDto);

        public TData WriteWarningMessage(ELinkPlanHistoryDto eLinkPlanHistoryDto);

    }

    public class ELinkPlanHistoryService : IELinkPlanHistoryService
    {

        private readonly IELinkPlanHistoryRepository _eLinkPlanHistoryRepository;

        public ELinkPlanHistoryService(IELinkPlanHistoryRepository eLinkPlanHistoryRepository)
        {
            _eLinkPlanHistoryRepository= eLinkPlanHistoryRepository;
        }

        public TData WriteErrorMessage(ELinkPlanHistoryDto eLinkPlanHistoryDto)
        {
            var result = new TData();
            if (eLinkPlanHistoryDto == null)
            {
                result.Message = "eLinkPlanHistoryDto is null";
                return result;
            }

            var obj = eLinkPlanHistoryDto.ToCovertELinkPlanHistory();
            obj.Type = ELinkPlanHistoryType.Error.ToString();
           
            var bl= _eLinkPlanHistoryRepository.Insert(obj);
            result.Tag = bl ? 1 : 0;

            return result;

        }

        public TData WriteInfoMessage(ELinkPlanHistoryDto eLinkPlanHistoryDto)
        {
            var result = new TData();
            if (eLinkPlanHistoryDto == null)
            {
                result.Message = "eLinkPlanHistoryDto is null";
                return result;
            }

            var obj = eLinkPlanHistoryDto.ToCovertELinkPlanHistory();
            obj.Type = ELinkPlanHistoryType.Info.ToString();
            
            var bl = _eLinkPlanHistoryRepository.Insert(obj);
            result.Tag = bl ? 1 : 0;

            return result;

        }

        public TData WriteWarningMessage(ELinkPlanHistoryDto eLinkPlanHistoryDto)
        {
            var result = new TData();
            if (eLinkPlanHistoryDto == null)
            {
                result.Message = "eLinkPlanHistoryDto is null";
                return result;
            }

            var obj = eLinkPlanHistoryDto.ToCovertELinkPlanHistory();
            obj.Type = ELinkPlanHistoryType.Warning.ToString();
           
            var bl = _eLinkPlanHistoryRepository.Insert(obj);
            result.Tag = bl ? 1 : 0;

            return result;

        }
    }
}
