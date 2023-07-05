using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Cannot enter more than 255 characters in first name.")]
        [RegularExpression("^[a-zA-ZÀÈÌÒÙàèìòùÁÉÍÓÚÝáéíóúýÂÊÎÔÛâêîôûÃÑÕãñõÄËÏÖÜŸäëïöüŸ¡¿çÇŒœßØøÅåÆæÞþÐð ]*$", ErrorMessage = "First name can only consist letters and spaces.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Cannot enter more than 255 characters in last name.")]
        [RegularExpression("^[a-zA-ZÀÈÌÒÙàèìòùÁÉÍÓÚÝáéíóúýÂÊÎÔÛâêîôûÃÑÕãñõÄËÏÖÜŸäëïöüŸ¡¿çÇŒœßØøÅåÆæÞþÐð ]*$", ErrorMessage = "Last name can only consist letters and spaces.")]
        public string LastName { get; set; }

        public string Title { get; set; }

        [MaxLength(30, ErrorMessage = "Cannot enter more than 30 characters in phone number.")]
        public string PhoneNumber { get; set; }

    }
}
