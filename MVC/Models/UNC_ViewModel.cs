using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.Models
{
    public class UNC_ViewModel
    {
        public tblNoteDetail noteDetailsViewModel { get; set; }

        public tblUserProfile profileViewModel { get; set; }
        public tblNoteReview reviewViewModel { get; set; }
        public List<SingleReview> reviewViewModelList { get; set; }
        public String contryViewModel { get; set; }

        public String typeViewModel { get; set; }
        public String catViewModel { get; set; }

        public String inappropriate { get; set; }

        public int reviewCount { get; set; }
        public String userViewModel { get; set;  }
        public int rating { get; set; }
    }
    public class SingleReview
    {
        public tblNoteReview singlereviewViewModel { get; set; }
        public string userName { get; set; }
        public string profileUrl { get; set; }
    }
}