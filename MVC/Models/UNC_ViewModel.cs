using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class UNC_ViewModel
    {
        public tblNoteDetail noteDetailsViewModel { get; set; }
        public String contryViewModel { get; set; }

        public String inappropriate { get; set; }
        public String userViewModel { get; set;  }
    }
}