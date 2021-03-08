using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class ViewModel
    {
        public tblNoteDetail noteViewModel { get; set; }

        public tblUser userViewModel { get; set; }

        public tblDownload downloadViewModel { get; set; }

        public tblManageNoteCategory categoryViewModel { get; set; }
    }
}