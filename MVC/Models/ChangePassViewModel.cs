using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NotesMarketPlace.Models
{
    public class ChangePassViewModel
    {

        [Required]
        public string oldPwd { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])(\\S)*[A-Za-z\d@$!%*?&]{6,24}$", ErrorMessage = "Password must be strong")]
        public string NewPwd { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Compare("NewPwd")]
        public string CnfmPwd { get; set; }
    }
}