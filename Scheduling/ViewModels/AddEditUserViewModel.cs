using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class AddEditUserViewModel
    {
        public string UserId { get; set; }

        [MaxLength(255)]
        public string UserName { get; set; }

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
        public string Password { get; set; }

        //[Required]
        public string RoleId { get; set; }

        public bool IsUserEnabled { get; set; }

        public string EmailID { get; set; }
        //Add new properties for new user management requirement 
        public string Designation { get; set; }
        public string WorkAreaLocation { get; set; }
        //public int RegisterAs { get; set; }
        public UserRegisterAsTypeViewModel RegisterAs { get; set; } = new UserRegisterAsTypeViewModel();
        public string IfOther { get; set; }
        public string PasswordHint { get; set; }
        public bool IsConfigured { get; set; }
        public string EncreptedPassword { get; set; }
        public bool IsRestrictedUser { get; set; }
        public int UserNo { get; set; }
        public bool IsUserNoEdit { get; set; }
        
        public CountryViewModel Country { get; set; } = new CountryViewModel();
        public LanguageViewModel Language { get; set; } = new LanguageViewModel();
        //public int CountryId { get; set; }
        //public int LanguageId { get; set; }

        [Required]         
        public SiteViewModel Site { get; set; } = new SiteViewModel();

    }
}
