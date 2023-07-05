using AutoMapper;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    public interface IUserService
    {
        Task<List<AddEditUserViewModel>> GetUsers(bool? status);
        Task<AddEditUserViewModel> GetUserById(string userId);
        Task<bool> CheckTechnicianId(string userId, int techid);
        Task<List<AspNetRoles>> GetRoles();
        Task<string> GetAspNetRolePrivileges(string roleId);
        //Task<List<AddEditUserViewModel>> GetContractorStaffUsers(bool? status);
        Task<List<AddEditUserViewModel>> GetFarmerUsers(bool? status);
        Task<UserProfileViewModel> GetUserProfileById(string userId);
        Task<bool> AddAdminBasedAction(string UserId, string ActionOnUserId, string Actions);
        Task<bool> AddUnApproveUserAccessDatas(string UserId, string leastAccessArea);
        Task<List<CountryViewModel>> GetCountries();
        Task<bool> UpdateUserAccess(string userid, bool flag);
        Task<bool> UpdateUserTechId(string userid, int techId);
        Task<List<LanguageViewModel>> GetLanguage();
    }
    public class UsersService : IUserService
    {
        private readonly IMapper _mapper;
        private MainDBContext _mainDBContext;
        private readonly ILogger<UsersService> _logger;
        public UsersService(ILogger<UsersService> logger,
                MainDBContext mainDBContext,
                IMapper mapper
            )
        {
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
        }

        /// <summary>
        /// add admin based action
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="actionOnUserId"></param>
        /// <param name="actions"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddAdminBasedAction(string userId, string actionOnUserId, string actions)
        {
            try
            {
                AdminBasedActions abaObj = new AdminBasedActions()
                {
                    UserId = userId,
                    ActionOnUserId = actionOnUserId,
                    Action = actions
                };
                await _mainDBContext.AdminBasedActions.AddAsync(abaObj);
                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(UsersService) }.{ nameof(AddAdminBasedAction) }]{ex}");
                return false;
            }
        }
        /// <summary>
        /// add unapproved user access data
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="leastAccessArea"></param>
        /// <returns>bool</returns>
        public async Task<bool> CheckTechnicianId(string userId, int techid)
        {
            try
            {
                if (_mainDBContext.AspNetUsers.Any(x => x.UserNo == techid && x.Id != userId))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(CheckTechnicianId)}]{ex}");
                return false;
            }
        }
        /// <summary>
        /// add unapproved user access data
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="leastAccessArea"></param>
        /// <returns>bool</returns>
        public async Task<bool> AddUnApproveUserAccessDatas(string UserId, string leastAccessArea)
        {
            try
            {
                UnApproveUserAccessData unapprovedUserObj = _mainDBContext.UnApproveUserAccessData
                    .Where(u => u.UserId == UserId).FirstOrDefault();
                if (unapprovedUserObj == null)
                {
                    unapprovedUserObj = new UnApproveUserAccessData();
                    unapprovedUserObj.UserId = UserId;
                    unapprovedUserObj.AccessArea = leastAccessArea.TrimEnd(' ').TrimEnd(',');
                    await _mainDBContext.UnApproveUserAccessData.AddAsync(unapprovedUserObj);
                }
                else
                {
                    unapprovedUserObj.AccessArea = leastAccessArea.TrimEnd(' ').TrimEnd(',');
                }
                await _mainDBContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(AddUnApproveUserAccessDatas)}]{ex}");
                return false;
            }
        }

        /// <summary>
        /// get privileges by roleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>string</returns>
        public async Task<string> GetAspNetRolePrivileges(string roleId)
        {
            try
            {
                string priviledge = await _mainDBContext.AspNetRolePrivilege
                    .Where(x => x.RoleId == roleId)
                    .Select(x => x.Privilege)
                    .OrderBy(x => x)
                    .FirstOrDefaultAsync();
                List<int> rolePriviledge = priviledge.Split(',').Select(int.Parse).ToList();
                var privileges = await _mainDBContext.Privilege.Select(x => (int)x.ActionKey).ToListAsync();
                bool isEqual = Enumerable.SequenceEqual(rolePriviledge, privileges);
                if (isEqual)
                {
                    return "All";
                }
                else
                {
                    return priviledge;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(GetAspNetRolePrivileges)}]{ex}");
                throw ex;
            }

        }

        public async Task<List<AspNetRoles>> GetRoles()
        {
            try
            {
                var roles = _mainDBContext.AspNetRoles.ToList();
                return roles;
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public async Task<bool> UpdateUserTechId(string userid, int techid)
        {
            try
            {
                var roles = _mainDBContext.AspNetUsers.Where(c => c.Id == userid).FirstOrDefault();
                roles.UserNo = techid;
                _mainDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> UpdateUserAccess(string userid, bool flag)
        {
            try
            {
                var roles = _mainDBContext.AspNetUsers.Where(c => c.Id == userid).FirstOrDefault();
                roles.IsRestrictedUser = flag;
                _mainDBContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// get users by isEnabled
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<List<AddEditUserViewModel>> GetUsers(bool? status)
        {
            try
            {
                var userList = (from user in _mainDBContext.AspNetUsers
                                join userrole in _mainDBContext.AspNetUserRoles on user.Id equals userrole.UserId
                                join role in _mainDBContext.AspNetRoles on userrole.RoleId equals role.Id
                                select new AddEditUserViewModel
                                {
                                    UserId = user.Id,
                                    UserName = user.Email,
                                    MobileNo = user.MobileNo,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    RoleId = user.RoleId,
                                    IsRestrictedUser = (bool)user.IsRestrictedUser,
                                    UserNo = (int)user.UserNo,
                                    IsUserNoEdit = false
                                });
                //if (status != null)
                //{
                //    userList = userList.Where(x => x.IsUserEnabled == status);
                //}
                return await userList.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(GetUsers)}]{ex}");
                throw ex;
            }
        }


        //public async Task<bool> DeleteUser(string userId)
        //{
        //    try
        //    {
              
        //        return await userList.ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"[{nameof(UsersService)}.{nameof(GetUsers)}]{ex}");
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<AddEditUserViewModel> GetUserById(string userId)
        {
            try
            {
                var userInfo = await (from user in _mainDBContext.AspNetUsers
                                      join userrole in _mainDBContext.AspNetUserRoles on user.Id equals userrole.UserId
                                      join role in _mainDBContext.AspNetRoles on userrole.RoleId equals role.Id
                                      where user.Id == userId
                                      select new AddEditUserViewModel
                                      {
                                          UserId = user.Id,
                                          UserName = user.Email,
                                          MobileNo = user.MobileNo,
                                          FirstName = user.FirstName,
                                          LastName = user.LastName,
                                          RoleId = role.Id,
                                      }).FirstOrDefaultAsync();
                return userInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(GetUserById)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// Get farmer users by isEnabled
        /// </summary>
        /// <returns></returns>
        public async Task<List<AddEditUserViewModel>> GetFarmerUsers(bool? status)
        {
            try
            {
                var userList = (from user in _mainDBContext.AspNetUsers
                                join userrole in _mainDBContext.AspNetUserRoles on user.Id equals userrole.UserId
                                join role in _mainDBContext.AspNetRoles on userrole.RoleId equals role.Id
                                where role.Name == "Farmer"
                                select new AddEditUserViewModel
                                {
                                    UserId = user.Id,
                                    UserName = user.Email,
                                    MobileNo = user.MobileNo,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    RoleId = role.Id,
                                });
                if (status != null)
                {
                    userList = userList.Where(x => x.IsUserEnabled == status);
                }
                return await userList.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(GetFarmerUsers)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// get user profile by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>UserProfileViewModel</returns>
        public async Task<UserProfileViewModel> GetUserProfileById(string userId)
        {
            try
            {

                var userProfile = await (from u in _mainDBContext.AspNetUsers
                                         where u.Id == userId
                                         select new UserProfileViewModel
                                         {
                                             UserId = u.Id,
                                             FirstName = u.FirstName,
                                             LastName = u.LastName,
                                             PhoneNumber = u.PhoneNumber

                                         }).FirstOrDefaultAsync();


                return userProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(GetUserProfileById)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// get contry list
        /// </summary>
        /// <returns>List<CountryViewModel></returns>
        public async Task<List<CountryViewModel>> GetCountries()
        {
            try
            {
                var list = await _mainDBContext.Country.ToListAsync();
                List<CountryViewModel> countries = _mapper.Map<List<CountryViewModel>>(list);
                return countries;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(GetCountries)}]{ex}");
                throw ex;
            }
        }

        /// <summary>
        /// get language list
        /// </summary>
        /// <returns>List<LanguageViewModel></returns>
        public async Task<List<LanguageViewModel>> GetLanguage()
        {
            try
            {
                var list = await _mainDBContext.Language.ToListAsync();
                List<LanguageViewModel> languages = _mapper.Map<List<LanguageViewModel>>(list);
                return languages;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersService)}.{nameof(GetLanguage)}]{ex}");
                throw ex;
            }
        }
    }
}
