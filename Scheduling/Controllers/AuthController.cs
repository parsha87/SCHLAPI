
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Data.GlobalEntities;
using Scheduling.Helpers;
using Scheduling.Services;
using Scheduling.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
/* =============================================================================================================================
Created on  : 30/06/2022

Author      : Kuldeep

File Details: This file contains the all authentication releated REST API

Description : This controller contains apis for login, sitenames, Forgot pasword, Password Hint

Remarks     : 
============================================================================================================================== */

namespace Scheduling.Controllers
{   //Allow specific origin
    [EnableCors("AllowSpecificOrigin")]
    //Controller route/endpoint
    [Route("api/[controller]")]
    [ApiController]
    //Authorize APIs
    [Authorize]
    public class AuthController : ControllerBase
    {
        //System Configuration manager
        private readonly IConfiguration _config;
        //Identity user manager
        private readonly UserManager<ApplicationUser> _userManager;
        //Logger 
        private readonly ILogger<AuthController> _logger;
        //Token JWT factory
        private readonly IJwtFactory _jwtFactory;
        //JWT Options
        private readonly JwtIssuerOptions _jwtOptions;
        //Json serializer
        private readonly JsonSerializerSettings _serializerSettings;
        //Hosting Envionment
        private readonly IWebHostEnvironment _webHostEnvironment;
        //Main DB Context
        private readonly MainDBContext _mainDBContext;
        //SMS Service - not working
        private readonly ISmsService _smsService;
        //Automapper for model mapping
        private readonly IMapper _mapper;
        //Global DB Context
        private GlobalDBContext _globalDBContext;
        //User Service
        private IUserService _userService;
        //Constructor to initialize methods and service
        public AuthController(
            MainDBContext mainDBContext,
            IConfiguration config,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger,
            IJwtFactory jwtFactory, IWebHostEnvironment hostingEnvironment,
            IOptions<JwtIssuerOptions> jwtOptions,
            ISmsService smsService, IMapper mapper,
            IUserService userService,
            GlobalDBContext globalDBContext
            )
        {
            _userService = userService;
            _userManager = userManager;
            _mapper = mapper;
            _mainDBContext = mainDBContext;
            _logger = logger;
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _webHostEnvironment = hostingEnvironment;
            _config = config;
            _globalDBContext = globalDBContext;
            _smsService = smsService;
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        /// <summary>
        /// Get all site names, it is needed to select the db's
        /// </summary>
        /// <returns>List<SiteViewModel></returns>
        [HttpGet("getsitenames")]
        [AllowAnonymous]
        public ActionResult<List<SiteViewModel>> GetSiteNames()
        {
            try
            {   //Get DB Connection
                List<SiteViewModel> siteNames = DbConnectionManager.GetAllConnections().Select(x => new SiteViewModel { Name = x.SiteName }).ToList();
                //Return response
                return Ok(CustomResponse.CreateResponse(true, "", siteNames, 0));
            }
            catch (Exception ex)
            {   //Log error
                _logger.LogError($"[{nameof(AuthController) }.{nameof(GetSiteNames) }]{ ex }");
                //Return Error
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", "", 1));
            }
        }

        /// <summary>
        /// get user register types
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>List<UserRegisterAsTypeViewModel></returns>
        [HttpPost("getuserregistertypes")]
        [AllowAnonymous]
        public async Task<ActionResult<List<UserRegisterAsTypeViewModel>>> GetUserRegisterTypes([FromBody] SiteViewModel siteName)
        {
            try
            {   //Check if sitename id null or empty
                if (string.IsNullOrEmpty(siteName.Name))
                {   //Return response
                    return Ok(CustomResponse.CreateResponse(false, "Site name not selected.", "", 4));
                }
                //Assign sitename to DB manager
                DbManager.SiteName = siteName.Name;
                //Get RegisterAS Type data
                var registerAsType = await _mainDBContext.UserRegisterAsType.ToListAsync();
                //Map datamodel to view model
                List<UserRegisterAsTypeViewModel> registerAsTypes = _mapper.Map<List<UserRegisterAsTypeViewModel>>(registerAsType);
                //Return response
                return Ok(CustomResponse.CreateResponse(true, "", registerAsTypes, 0));

            }
            catch (Exception ex)
            {   //Log error
                _logger.LogError($"[{nameof(AuthController) }.{nameof(GetUserRegisterTypes) }]{ex}");
                //Return response
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", "", 1));
            }
        }

        /// <summary>
        /// get contries
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("getcountries")]
        public async Task<ActionResult<dynamic>> GetCountries([FromBody] SiteViewModel siteName)
        {
            try
            {
                //Check if sitename id null or empty
                if (string.IsNullOrEmpty(siteName.Name))
                {
                    //Return response
                    return Ok(CustomResponse.CreateResponse(false, "Site name not selected.", "", 4));
                }
                //Assign sitename to DB manager
                DbManager.SiteName = siteName.Name;
                //Get All Countries Data
                List<CountryViewModel> countries = await _userService.GetCountries();
                //Return response
                return Ok(CustomResponse.CreateResponse(true, "", countries, 0));
            }
            catch (Exception ex)
            {   //Log Error
                _logger.LogError($"[{nameof(AuthController) }.{nameof(GetCountries) }]{ex}");
                //Return Error response
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// get languages
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("getlanguage")]
        public async Task<ActionResult<dynamic>> GetLanguage([FromBody] SiteViewModel siteName)
        {

            try
            {   
                //Check if sitename id null or empty
                if (string.IsNullOrEmpty(siteName.Name))
                {
                    //Return response
                    return Ok(CustomResponse.CreateResponse(false, "Site name name not selected.", "", 4));
                }
                //Assign sitename to DB manager
                DbManager.SiteName = siteName.Name;
                //Get all languages data
                List<LanguageViewModel> languages = await _userService.GetLanguage();
                //Return response
                return Ok(CustomResponse.CreateResponse(true, "", languages, 0));
            }
            catch (Exception ex)
            {   //Log Error and return response
                _logger.LogError($"[{nameof(AuthController)}.{ nameof(GetLanguage) }]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", null, 1));
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="credentials">CredentialsViewModel</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CredentialsViewModel credentials)
        {
            try
            {
                //Check if model is valid
                if (!ModelState.IsValid)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Bad request.", "", 0));
                }
                if(credentials.Name=="")
                {
                    // set sitename
                    DbManager.SiteName = _config["SiteName"]; 
                }
                else
                {
                    // set sitename
                    DbManager.SiteName = credentials.Name; //_config["SiteName"]; 
                }
                

                // get the user to verify
                var user = await _userManager.FindByNameAsync(credentials.UserName);
                if (user == null)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Invalid username or password.", "", 5));
                }

                // check user password
                if (!await _userManager.CheckPasswordAsync(user, credentials.Password))
                {
                    return Ok(CustomResponse.CreateResponse(false, "Invalid username or password.", "", 6));
                }

                // check user is enabled
                if (!user.IsUserEnabled)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Your account is not approved for login.", "", 7));
                }
                //check user restricted
                if (!user.IsRestrictedUser)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Your account is not approved for login.", "", 7));
                }

                // check restricted user TODO: uncommnet later after check
                //if (!user.IsRestrictedUser)
                //{
                //    return Ok(CustomResponse.CreateResponse(false, "Your access has been restricted.", "", 8));
                //}

                //Get user role and previleges
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.First();
                //Get role id
                var roleid = await _mainDBContext.AspNetRoles.Where(x => x.Name == role).Select(x => x.Id).FirstOrDefaultAsync();
                string privileges = await _userService.GetAspNetRolePrivileges(roleid);
                // set identity
                var identity = _jwtFactory.GenerateClaimsIdentity(credentials.UserName, user.Id, role, roleid, credentials.Name, privileges);
                // get user language
                var language = await _mainDBContext.Language.Where(x => x.Id == user.LanguageId).FirstOrDefaultAsync();
                LanguageViewModel languageViewModel = _mapper.Map<LanguageViewModel>(language);
                //Get gateway list
                List<Gateway> gateways = _mainDBContext.Gateway.ToList();
                //Technician Id
                int techid = user.UserNo;
                //Get project gateway mapping from global DB
                ProjectGatewayMapping projectGatewayMapping = _globalDBContext.ProjectGatewayMapping.Where(x => x.ProjectName == credentials.Name).FirstOrDefault();
                //Create Dynamic response
                dynamic response = new
                {   //Auth Token, Expires, Firstname, Lastname, site,role, language, datetime, gateway list, deciceid, technician id
                    auth_token = await _jwtFactory.GenerateEncodedToken(credentials.UserName, identity, credentials.Name, roleid, privileges),
                    expires_in = (int)_jwtOptions.ValidFor.TotalSeconds,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    site = new SiteViewModel() { Name = credentials.Name, ProjectId = (int)projectGatewayMapping.ProjectId },
                    role = role,
                    language = languageViewModel,
                    datetime = DateTime.Now,
                    gateways = gateways,
                    deviceId = 767,
                    technicianId = user.UserNo //Value from 1 to 65535 (2 bytes) //UserNo column is used as technician id
                };
                //Create Json Response
                var json = JsonConvert.SerializeObject(response, _serializerSettings);
                //return response
                return Ok(CustomResponse.CreateResponse(true, "", response, 0));
                //return new OkObjectResult(json);
            }
            catch (Exception ex)
            {   //Log Error and return response
                _logger.LogError($"[{nameof(AuthController)}.{ nameof(Post) }]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", "", 1));
            }
        }


        // POST: api/auth/ForgotPassword
        /// <summary>
        /// forgot password using sms
        /// </summary>
        /// <param name="model">ForgotPasswordModel</param>
        /// <returns>Task<IHttpActionResult></returns>
        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Bad request.", "", 1));
                }
                DbManager.SiteName = model.Name;

                // check user
                ApplicationUser user = await _userManager.FindByNameAsync(model.EmailId);
                if (user == null)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Invalid username.", "", 5));
                }

                // check user is enabled
                if (!user.IsUserEnabled)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Your account is not approved for login.", "", 7));
                }

                // check restricted user
                if (!user.IsRestrictedUser)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Your access has been restricted.", "", 8));
                }
                //Decrypt password
                string password = EncreptDecreptService.DecryptText(user.EncreptedPassword, GlobalConstants.EncreptDecreptKey);
                string smsMessage = "Your login details are Username:" + user.Email + " and Password:" + password;
                _smsService.SendSMS(user.MobileNo, smsMessage, user.Id, "Forgot Password");

                return Ok(CustomResponse.CreateResponse(true, smsMessage, "", 0));

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AuthController) }.{nameof(ForgotPassword) }]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", "", 1));
            }
        }

        // POST: api/Auth/ResetPassword
        /// <summary>
        /// send password hint using sms
        /// </summary>
        /// <param name="model">ForgotPasswordModel</param>
        /// <returns>Task<IActionResult></returns>
        [HttpPost("passwordhint")]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordHint([FromBody] ForgotPasswordModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Bad Request", "", 1));
                }
                DbManager.SiteName = model.Name;

                // check user
                ApplicationUser user = await _userManager.FindByNameAsync(model.EmailId);
                if (user == null)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Username does not exist.", "", 5));
                }

                // check user is enabled
                if (!user.IsUserEnabled)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Your account is not approved for login.", "", 7));
                }

                // check restricted user
                if (!user.IsRestrictedUser)
                {
                    return Ok(CustomResponse.CreateResponse(false, "Your access has been restricted.", "", 8));
                }

                // check password hint set or not
                if (string.IsNullOrEmpty(user.PasswordHint))
                {
                    return Ok(CustomResponse.CreateResponse(false, "Your password hint is not set.", "", 1));
                }
                //Log Error and return response
                return Ok(CustomResponse.CreateResponse(true, "Your password hint is " + user.PasswordHint,
                    "Your password hint is " + user.PasswordHint, 1));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AuthController) }.{nameof(PasswordHint) }]{ex}");
                return Ok(CustomResponse.CreateResponse(false, "Oop's, something went wrong while performing your request.", "", 1));
            }
        }
    }
}
