using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketPlace.Models;

namespace NotesMarketPlace.Controllers
{
    public class BaseController : Controller
    {

        notesmarketplaceEntities3 db = new notesmarketplaceEntities3();

        public tblUserProfile userData;
        public BaseController()
        {

            if (CommonViewModel.uId != 0)
            {
                userData = db.tblUserProfiles.Where(e => e.userID == CommonViewModel.uId).SingleOrDefault();
                CommonViewModel.profileViewModel = userData;
                CommonViewModel.configViewModel = db.tblSystemConfigs.FirstOrDefault();
            }

        }

    }
}