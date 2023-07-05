using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scheduling.Auth;
using Scheduling.Data;
using Scheduling.Data.Entities;
using Scheduling.Helpers;
using Scheduling.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadConfigurationController : ControllerBase
    {
        private IUploadService _uploadService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly ILogger<UploadConfigurationController> _logger;
        public UploadConfigurationController(IUploadService uploadService,
          ILogger<UploadConfigurationController> logger, IWebHostEnvironment webHostEnvironment,
               IConfiguration config)
        {
            _uploadService = uploadService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }
        /// <summary>
        /// Create estimate
        /// </summary>
        /// <param name="model">EstimateViewModel</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                //Add attachments if any
                IFormFileCollection formFiles = HttpContext.Request.Form.Files;
                var addAttachmentResult = await _uploadService.UploadConfiguration(formFiles);
                if (addAttachmentResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UploadConfigurationController)}.{nameof(Post)}]{ ex }");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(Post), new { id = 0 }, true);

        }

        [HttpGet("UploadTest")]
        [AllowAnonymous]
        public IActionResult Getsd()
        {
            DbManager.SiteName = "JainPort76";
            _uploadService.Createnodes();
            return Ok();
        }

            /// <summary>
            /// Create estimate
            /// </summary>
            /// <param name="model">EstimateViewModel</param>
            /// <returns></returns>
            [HttpGet("project")]
        public async Task<ProjectConfiguration> Get()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                var data = await _uploadService.GetProjectConfiguration();
                return data;

            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UploadConfigurationController)}.{nameof(Get)}]{ ex }");
                return null;
            }
            

        }

        [HttpPost("Sequence")]
        public async Task<IActionResult> PostSequence()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                //Add attachments if any
                IFormFileCollection formFiles = HttpContext.Request.Form.Files;
                var addAttachmentResult = await _uploadService.UploadConfigurationSeq(formFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(UploadConfigurationController)}.{nameof(Post)}]{ ex }");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return CreatedAtAction(nameof(Post), new { id = 0 }, true);

        }

        [HttpGet("filedownload")]
        // //[Authorize(Policy = "Permissions.Operations.Estimate List.ReadOnly,Permissions.Operations.Estimate List.AddUpdateDelete")]
        public IActionResult FileDownload()
        {
            try
            {
                DbManager.SiteName = User.Claims.Single(c => c.Type == CustomClaimTypes.SiteName).Value;

                string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, _config["IRAttachmentsFolderName"], "sequence.xlsx");
                if (filePath != null)
                {
                    // get content type of the attachment
                    var fileProvider = new FileExtensionContentTypeProvider();
                    if (!fileProvider.TryGetContentType(filePath, out string contentType))
                    {
                        throw new ArgumentOutOfRangeException($"Unable to find Content Type for file name sequence.xlsx.");
                    }
                    return PhysicalFile(filePath, contentType, "sequence.xlsx");
                }
                else
                {
                    ModelState.AddModelError("Error", "File not found");
                    return BadRequest(new CustomBadRequest(ModelState));
                }
            }
            catch (Exception ex)
            {
                string error = $@"[{nameof(UploadConfigurationController)}.{nameof(FileDownload)}] 
                    Exception = {ex}
                    loggedin user = {User.Identity.Name}
                    Http Request Details:
                    ";
                _logger.LogError(error);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
