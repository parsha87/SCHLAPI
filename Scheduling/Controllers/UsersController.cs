using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.ViewModels;
using Microsoft.EntityFrameworkCore;
using Scheduling.Auth;
using Scheduling.Helpers;
using System.Security.Claims;
using Scheduling.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Scheduling.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ISmsService _smsService;
        private readonly ApplicationDbContext _appDbContext;
        private readonly MainDBContext _mainDBContext;
        private readonly EventDBContext _eventDBContext;

        private readonly IWebHostEnvironment _webHostEnvironment;
        /// private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private IUserService _userService;

        public UsersController(ISmsService smsService, EventDBContext eventDBContext, MainDBContext mainDBContext, IUserService userService, UserManager<ApplicationUser> userManager, ILogger<UsersController> logger, RoleManager<IdentityRole> roleManager,
            IConfiguration config, ApplicationDbContext appDbContext, IWebHostEnvironment hostingEnvironment, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            _mainDBContext = mainDBContext;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
            _webHostEnvironment = hostingEnvironment;
            _eventDBContext = eventDBContext;
            _smsService = smsService;
        }

        /// <summary>
        /// Get Users
        /// </summary>
        /// <returns>List<AddEditUserViewModel></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<AddEditUserViewModel>>> Get(bool? status)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<AddEditUserViewModel> users = await _userService.GetUsers(status);
                return Ok(CustomResponse.CreateResponse(true, string.Empty, users, 0));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(UsersController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while fetching users.", "", 1));
            }
        }

/// <summary>
/// 
/// </summary>
/// <param name="userId"></param>
/// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                using (var transaction = _appDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        //show proper error message to admin
                        if (user == null)
                        {
                            transaction.Rollback();
                            return NotFound();
                        }
                        //find user's existing role
                        var userRoles = await _userManager.GetRolesAsync(user);
                        if (userRoles.Contains("SuperAdmin"))
                        {
                            transaction.Rollback();
                            ModelState.AddModelError("Error", "Cannot ");
                            return BadRequest(new CustomBadRequest(ModelState));
                        }
                        //delete user roles
                        foreach (var role in userRoles)
                        {
                            var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, role);
                            if (!removeRoleResult.Succeeded)
                            {
                                //rollback
                                transaction.Rollback();
                                ModelState.AddModelError("Error", removeRoleResult.Errors.FirstOrDefault().Description);
                                return BadRequest(new CustomBadRequest(ModelState));
                            }
                            await _appDbContext.SaveChangesAsync();
                        }
                        var removeUserResult = await _userManager.DeleteAsync(user);
                        if (!removeUserResult.Succeeded)
                        {
                            //rollback
                            transaction.Rollback();
                            ModelState.AddModelError("Error", removeUserResult.Errors.FirstOrDefault().Description);
                            return BadRequest(new CustomBadRequest(ModelState));
                        }
                        await _appDbContext.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        string error = $@"[{nameof(UsersController)}.{nameof(DeleteUser)}] 
                                    Exception = {ex}
                                    loggedin user = {User.Identity.Name}
                                    Http Request Details:
                                    id = {userId}";
                        _logger.LogError(error);
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                return Ok(new OperationResult { Succeeded = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UsersController)}.{nameof(Get)}]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while fetching users.", "", 1));
            }
        }

        /// <summary>
        /// Get users by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>AddEditUserViewModel</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AddEditUserViewModel>> Get(string id)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                AddEditUserViewModel user = await _userService.GetUserById(id);
                if (user == null)
                    return Ok(CustomResponse.CreateResponse(false, "User not found.", "", 1));

                return Ok(CustomResponse.CreateResponse(true, string.Empty, user, 1));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(UsersController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while fetching user.", "", 1));
            }
        }


        /// <summary>
        /// Get users by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>AddEditUserViewModel</returns>
        [HttpGet("GetRoles")]
        [Authorize]
        public async Task<ActionResult<AspNetRoles>> GetRoles()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                List<AspNetRoles> roles = await _userService.GetRoles();
                if (roles == null)
                    return Ok(CustomResponse.CreateResponse(false, "User not found.", "", 1));

                return Ok(CustomResponse.CreateResponse(true, string.Empty, roles, 1));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(UsersController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while fetching roles.", "", 1));
            }
        }

        /// <summary>
        /// Get users by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>AddEditUserViewModel</returns>
        [HttpGet("updateuseracess/{id}/{flag}")]
        [Authorize]
        public async Task<ActionResult> Updateuseracess(string id, bool flag)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                var roles = await _userService.UpdateUserAccess(id, flag);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(UsersController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while updating.", "", 1));
            }
        }

        /// <summary>
        /// Get users by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>AddEditUserViewModel</returns>
        [HttpGet("updatetechnician/{id}/{techid}")]
        [Authorize]
        public async Task<ActionResult> updatetechnician(string id, int techid)
        {
            try
            {

                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                bool user = await _userService.CheckTechnicianId(id, techid);
                if (user)
                {
                    return BadRequest();
                }

                var roles = await _userService.UpdateUserTechId(id, techid);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(UsersController) }.{ nameof(Get) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while updating.", "", 1));
            }
        }

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>AddEditUserViewModel</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<OperationResult<AddEditUserViewModel>>> Post([FromBody] AddEditUserViewModel model)
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;
                //
                //AddEditUserViewModel model = new AddEditUserViewModel() ;
                if (!IsModelValid(model))
                {
                    var errorList = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                    return Ok(CustomResponse.CreateResponse(false, "Bad Request", errorList, 4));
                }

                //DbManager.SiteName = model.Site.Name;
                DbManager.SiteName = _config["SiteName"];
                model.IsUserEnabled = true;
                // get users register as type
                //var registerAsType = await _mainDBContext.UserRegisterAsType.Where(x => x.RegisterAsId == model.RegisterAs.RegisterAsId).FirstOrDefaultAsync();
                //if (registerAsType == null)
                //{
                //    return Ok(CustomResponse.CreateResponse(false, "Invalid value for register as type.", string.Empty, 5));
                //}

                int aspNetUserCount = 0;
                var errors = new List<string>();
                // username is not entered then generate username
                if (string.IsNullOrEmpty(model.UserName))
                {
                    List<AspNetUsers> users = await _mainDBContext.AspNetUsers
                    .Where(a => a.FirstName.ToLower() == model.FirstName.ToLower()
                        && a.LastName.ToLower() == model.LastName.ToLower())
                    .ToListAsync();

                    aspNetUserCount = users.Count;
                    if (aspNetUserCount == 0)
                    {
                        model.UserName = string.Format("{0}.{1}{2}@JainIrriCare.com", model.FirstName.ToString().ToLower(), model.LastName.ToString().ToLower(), aspNetUserCount);
                    }
                    else
                    {
                        aspNetUserCount = aspNetUserCount + 1;
                        model.UserName = string.Format("{0}.{1}{2}@JainIrriCare.com", model.FirstName.ToString().ToLower(), model.LastName.ToString().ToLower(), aspNetUserCount);
                    }
                }

                // get farmer role
                var role = await _roleManager.Roles.Where(x => x.Name == "Farmer").FirstOrDefaultAsync();
                if (role == null)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Role not found!", string.Empty, 6));
                }
                
                // new users mapping
                var newUser = _mapper.Map<ApplicationUser>(model);

                //newUser.MobileNo = model.MobileNo;
                //newUser.FirstName = model.MobileNo;
                //newUser.LastName = model.MobileNo;
                //newUser.Address = model.MobileNo;
                //newUser.Password = model.Password;
                //newUser.IsUserEnabled = model.IsUserEnabled;
                //newUser.EmailID = model.MobileNo;
                //newUser.Designation = model.MobileNo;
                //newUser.WorkAreaLocation = model.MobileNo;
                //newUser.IfOther = model.MobileNo;
                //newUser.PasswordHint = model.MobileNo;
                //newUser.IsConfigured = model.MobileNo;
                //newUser.EncreptedPassword = model.MobileNo;
                //newUser.IsRestrictedUser = model.MobileNo;
                //newUser.UserNo = model.MobileNo;
                //newUser.RegisterAs = model.RegisterAs.RegisterAsId;
                //newUser.LanguageId = model.Language.Id;
                //newUser.CountryId = model.Country.CntryId;
                // generate encrypted password
                newUser.EncreptedPassword = EncreptDecreptService.EncryptText(model.Password, GlobalConstants.EncreptDecreptKey);

                // star transaction
                using (var transaction = _appDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // create user
                        var result = await _userManager.CreateAsync(newUser, model.Password);
                        if (!result.Succeeded)
                        {
                            _logger.LogError($"[{nameof(UsersController)}] {nameof(Post)} + User ({newUser.Email}) not created.");
                            // rollback
                            transaction.Rollback();
                            return Ok(CustomResponse.CreateResponse(false, result.Errors.FirstOrDefault().Description, "", 7));
                        }
                        model.UserId = newUser.Id;
                        model.RoleId = role.Id;
                        // assign user to role
                        var addUserToRoleResult = await _userManager.AddToRoleAsync(newUser, role.Name);
                        if (!addUserToRoleResult.Succeeded)
                        {
                            _logger.LogError($"[{nameof(UsersController)}] {nameof(Post)} + User ({newUser.Email}) not created, error in add user to role.");
                            // rollback
                            transaction.Rollback();
                            return Ok(CustomResponse.CreateResponse(false, addUserToRoleResult.Errors.FirstOrDefault().Description, "", 7));
                        }
                        await _appDbContext.SaveChangesAsync();

                        // call admin based action 
                        var adminBasedSectionResult = await _userService.AddAdminBasedAction("Self", newUser.Id, "User Created");
                        if (!adminBasedSectionResult)
                        {
                            _logger.LogError($"[{nameof(UsersController)}] {nameof(Post)} + User ({newUser.Email}) not created, error in add admin based section.");
                            // rollback
                            transaction.Rollback();
                            return Ok(CustomResponse.CreateResponse(false, $"User ({newUser.Email}) not created, error in add admin based section.", "", 7));
                        }

                        if (newUser.IsUserEnabled == false)
                        {
                            // unapproved user
                            var unApprovedUserAccessResult = await _userService.AddUnApproveUserAccessDatas(newUser.Id, "");
                            if (!unApprovedUserAccessResult)
                            {
                                _logger.LogError($"[{nameof(UsersController)}] {nameof(Post)} + User ({newUser.Email}) not created, error in add unapproved user access.");
                                transaction.Rollback();
                                return Ok(CustomResponse.CreateResponse(false, $"User ({newUser.Email}) not created, error in add unapproved user access.", "", 7));
                            }
                        }
                        transaction.Commit();
                        //SMS SERVICE
                        string smsMessage = "JainIrriCare: You have successfully applied with JainIrriCare. Your login details will be sent to your mobile number after approval.";
                        //_smsService.SendSMS(model.MobileNo, smsMessage, newUser.Id, newUser.Id);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError("[" + nameof(UsersController) + "." + nameof(Post) + "]" + ex);
                        return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while registering user.", "", 4));
                    }
                }
                return Ok(CustomResponse.CreateResponse(true, "Registration is sent for approval. On approval you will receive a message on registered mobile number.", string.Empty, 8));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ nameof(UsersController) }.{ nameof(Post) }]{ ex }");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", string.Empty, 1));
            }
        }

        #region private methods     

        /// <summary>
        /// check model is valid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsModelValid(AddEditUserViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "Model cannot be null");
            }

            if (!ModelState.IsValid)
            {
                return false;
            }

            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (allErrors.ToList().Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
