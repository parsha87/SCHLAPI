﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class AccountActivationModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
