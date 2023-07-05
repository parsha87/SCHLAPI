using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class AspNetRolePrivilegeViewModel
    {
        public int Id { get; set; }      
        public string Role { get; set; }
        public string Privilege { get; set; }       
        public string RoleId { get; set; }
    }
}
