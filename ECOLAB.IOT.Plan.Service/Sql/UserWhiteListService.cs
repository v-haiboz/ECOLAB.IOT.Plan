
namespace ECOLAB.IOT.Plan.Service.Sql
{
    using AutoMapper;
    using ECOLAB.IOT.Plan.Common.Utilities;
    using ECOLAB.IOT.Plan.Entity;
    using ECOLAB.IOT.Plan.Entity.Dtos.Certification;
    using ECOLAB.IOT.Plan.Entity.Dtos.Sql;
    using ECOLAB.IOT.Plan.Entity.Entities.SqlServer;
    using ECOLAB.IOT.Plan.Repository.Repositories.SqlServer;
    using System;
    using System.Collections.Generic;

    public interface IUserWhiteListService
    {
        public TData<SecretDto> Create(UserWhiteListDto userWhiteListDto);

        public TData<SecretDto> Refresh(UserWhiteListDto userWhiteListDto);

        public TData Delete(UserWhiteListDto userWhiteListDto);

        public TDataPage<List<UserWhiteList>> GetUserWhiteList(string email = null, int pageIndex = 1, int pageSize = 50);
    }

    public class UserWhiteListService : IUserWhiteListService
    {
        private readonly IUserWhiteListRepository _userWhiteListRepository;
        private readonly IMapper _mapper;
        public UserWhiteListService(IUserWhiteListRepository userWhiteListRepository, IMapper mapper)
        {
            _userWhiteListRepository = userWhiteListRepository;
            _mapper = mapper;
        }

        public TData<SecretDto> Create(UserWhiteListDto userWhiteListDto)
        {
            var result = new TData<SecretDto>();
            try
            {
                if (userWhiteListDto == null || string.IsNullOrEmpty(userWhiteListDto.Email))
                {
                    result.Message = "Email cannot be empty.";
                    return result;
                }

               var list=_userWhiteListRepository.GetUserWhiteList(userWhiteListDto.Email);
                if (list != null && list.Count > 0)
                {
                    result.Message = "Email exists.";
                    return result;
                }

                var userWhiteList = _mapper.Map<UserWhiteList>(userWhiteListDto);
                userWhiteList.SecretKey = Utility.RandomGenerateString();
                var bl=_userWhiteListRepository.Create(userWhiteList);
                result.Tag = bl ? 1 : 0;
                if (bl)
                {
                    result.Message = "User White List Create successful";
                    result.Data = new SecretDto()
                    {
                        SecretKey = userWhiteList.SecretKey,
                        AbsoluteExpirationTime = userWhiteList.ExpiredAt
                    };
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Create User WhiteList failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TData<SecretDto> Refresh(UserWhiteListDto userWhiteListDto)
        {
            var result = new TData<SecretDto>();
            try
            {
                if (userWhiteListDto == null || string.IsNullOrEmpty(userWhiteListDto.Email))
                {
                    result.Message = "Email cannot be empty.";
                    return result;
                }

                var list = _userWhiteListRepository.GetUserWhiteList(userWhiteListDto.Email);
                if (list != null && list.Count > 0 && list.FirstOrDefault()!=null)
                {
                    var item = list.FirstOrDefault();
                    if (item != null)
                    {
                        item.SecretKey = Utility.RandomGenerateString();
                        item.ExpiredAt = DateTime.UtcNow.AddYears(1);
                        var bl = _userWhiteListRepository.Update(item);
                        result.Tag = bl ? 1 : 0;
                        if (bl)
                        {
                            result.Message = "User White List Create successful";
                            result.Data = new SecretDto()
                            {
                                SecretKey = item.SecretKey,
                                AbsoluteExpirationTime = item.ExpiredAt
                            };
                        }
                    }
                }
               
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Create User WhiteList failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TData Delete(UserWhiteListDto userWhiteListDto)
        {
            var result = new TData();
            try
            {
                if (userWhiteListDto == null || string.IsNullOrEmpty(userWhiteListDto.Email))
                {
                    result.Message = "Email cannot be empty.";
                    return result;
                }

                var list = _userWhiteListRepository.GetUserWhiteList(userWhiteListDto.Email);
                if (list == null || list.Count == 0)
                {
                    result.Message = "Email doesn't exist.";
                    return result;
                }

                var userWhiteList = _mapper.Map<UserWhiteList>(userWhiteListDto);
                var bl = _userWhiteListRepository.Delete(userWhiteList);
                result.Tag = bl ? 1 : 0;
                if (bl)
                    result.Message = "User White List Delete successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Delete User WhiteList failed";
                result.Description = ex.ToString();
                return result;
            }
        }

        public TDataPage<List<UserWhiteList>> GetUserWhiteList(string email = null, int pageIndex = 1, int pageSize = 50)
        {
            var result = new TDataPage<List<UserWhiteList>>();
            try
            {
                result.Tag = 1;
                result.Data= _userWhiteListRepository.GetUserWhiteList(email, pageIndex, pageSize);
                result.Total = _userWhiteListRepository.CountUserWhiteList(email);
                result.PageIndex = pageIndex;
                result.PageSize= pageSize;
                result.Message = "Get UserWhiteList successful";
                return result;
            }
            catch (Exception ex)
            {
                result.Tag = 0;
                result.Message = "Get UserWhiteList failed";
                result.Description = ex.ToString();
                return result;
            }
        }
    }
}
