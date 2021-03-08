using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotesMarketPlace.DataEntity
{
    public class ET_user
    {
        public int id { get; set; }
        public int roleID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailID { get; set; }
        public bool isEmailVerified { get; set; }
        public string password { get; set; }
        public Nullable<System.DateTime> createdDate { get; set; }
        public Nullable<int> createdBy { get; set; }
        public Nullable<System.DateTime> modifiedDate { get; set; }
        public Nullable<int> modifiedBy { get; set; }
        public bool isActive { get; set; }
    }
}