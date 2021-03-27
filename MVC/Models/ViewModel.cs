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

        public tblUserProfile profileViewModel { get; set; }

        public HttpPostedFileBase profileImage { get; set; }

        public tblReferenceData referenceViewModel { get; set; }

        public tblNoteReview NoteReviewModel { get; set; }

        public tblNoteReportedIsuue NoteReportedIsuue { get; set; }

        public int myDownloads { get; set; }
        public int mySoldNote { get; set; }
        public int moneyEarned { get; set; }
        public int buyerRequest { get; set; }
        public int myRejectedNote { get; set; }
    }

    public class CommonViewModel
    {
        static public tblUserProfile profileViewModel { get; set; }

        static public int uId { get; set; }
    }

}