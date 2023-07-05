using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        //[Required]
        //[EmailAddress]
        //[MaxLength(255)]
        //public string UserName { get; set; }

        [MaxLength(30)]
        public string MobileNo { get; set; }

        [Required]
        [MaxLength(255)]
        [RegularExpression("^[a-zA-ZÀÈÌÒÙàèìòùÁÉÍÓÚÝáéíóúýÂÊÎÔÛâêîôûÃÑÕãñõÄËÏÖÜŸäëïöüŸ¡¿çÇŒœßØøÅåÆæÞþÐð ]*$", ErrorMessage = "First name can only consist letters and spaces.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        [RegularExpression("^[a-zA-ZÀÈÌÒÙàèìòùÁÉÍÓÚÝáéíóúýÂÊÎÔÛâêîôûÃÑÕãñõÄËÏÖÜŸäëïöüŸ¡¿çÇŒœßØøÅåÆæÞþÐð ]*$", ErrorMessage = "Last name can only consist letters and spaces.")]
        public string LastName { get; set; }
        public string Address { get; set; }

        [Required]
        public string RoleId { get; set; }

        public bool IsUserEnabled { get; set; }
        //[EmailAddress]
        //[MaxLength(255)]
        //public string Email { get; set; }
        //Add new properties for new user management requirement 
        public string Designation { get; set; }
        public string WorkAreaLocation { get; set; }
        public int RegisterAs { get; set; }
        public string IfOther { get; set; }
        public string PasswordHint { get; set; }
        public bool IsConfigured { get; set; }
        public string EncreptedPassword { get; set; }
        public bool IsRestrictedUser { get; set; }
        public int UserNo { get; set; }
        public int CountryId { get; set; }
        public int LanguageId { get; set; }
       
    }
}
