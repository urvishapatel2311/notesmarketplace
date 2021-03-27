using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NotesMarketPlace.Models;
using NotesMarketPlace.Controllers;
using System.Net.Mail;
using System.Net;
using System.IO;
using NotesMarketPlace.DataEntity;
using PagedList;
using System.Runtime.InteropServices;

namespace NotesMarketPlace.Controllers
{
    public class userController : BaseController
    {
        notesmarketplaceEntities db = new notesmarketplaceEntities();
        // GET: user
        public ActionResult Login()
        {
            HttpCookie cookie = Request.Cookies["user"];
            tblUser u = new tblUser();

            if (cookie != null)
            {
                u.emailID = cookie["emailID"];
                u.password = cookie["password"];

                if (cookie["emailID"].Length != 0)
                {
                    u.Rememberme = true;
                }
                else
                {
                    u.Rememberme = false;
                }
            }
            return View(u);
        }

        [HttpPost]
        public ActionResult Login(tblUser u)
        {
            HttpCookie cookie = new HttpCookie("user");
            try
            {
                var data = db.tblUsers.Where(m => m.emailID.Equals(u.emailID) && m.password.Equals(u.password)).SingleOrDefault();


                if (data != null)
                {
                    CommonViewModel.uId = data.id;
                    Session["userID"] = data.id;
                    Session["emailID"] = u.emailID;
                    Session["Name"] = data.firstName + data.lastName;
                    Session["fname"] = data.firstName;
                    Session["lname"] = data.lastName;

                    var data2 = db.tblUserProfiles.Where(m => m.userID.Equals(data.id)).SingleOrDefault();

                    if (data.isEmailVerified == true)
                    {
                        if (u.Rememberme == true)
                        {
                            cookie["emailID"] = u.emailID;
                            cookie["password"] = u.password;
                            cookie.Expires = DateTime.Now.AddDays(2);
                            HttpContext.Response.Cookies.Add(cookie);
                        }
                       
                        if (data2!= null && data2.isProfileSet)
                        {
                            if (data.roleID == 5)//user
                            {
                                return RedirectToAction("SearchNote");
                            }
                            else if (data.roleID == 1)//admin
                            {
                                return RedirectToAction("Dashboard", "admin");
                            }
                        }
                        else
                        {
                            return RedirectToAction("UserProfile");
                        }

                    }
                    else
                    {
                        ViewBag.error = "Please verify the email address via clicking on the link we sent you via email.";
                    }
                }
                else
                {
                    ViewBag.error = "Login failed";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong!!", ex);
            }
            return View();

        }

        // GET: user
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SignUp(tblUser su)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var check = db.tblUsers.FirstOrDefault(s => s.emailID == su.emailID);
                    if (check == null)
                    {
                        su.roleID = 5;
                        su.isEmailVerified = false;
                        su.isActive = true;
                        su.createdDate = DateTime.Now;
                        db.tblUsers.Add(su);
                        int i = db.SaveChanges();
                        if (i > 0)
                        {
                            VerifyEmail(su.emailID);
                        }
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.error = "Email already exists";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong!!", ex);
            }
            return View();
        }

        [NonAction]
        private void VerifyEmail(string emailId)
        {
            var varifyUrl = "/user/EmailVerification/?emailID=" + emailId;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, varifyUrl);
            var fromMail = new MailAddress("urvishapatel2311@gmail.com");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "cbxycqbqtyubhczs";
            string subject = "Note MarketPlace - Email Verification";
            string body = "Hello <br/><br/>Thank You for signing up with us. Please click on below link to verify your email address and to do login." +
              " <br/><br/><a href= '" + link + "'>Click here to Verify Your Email</a><br/><br/>Regards, <br/>Notes MarketPlace";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        // GET: user
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult forgotPassword(tblUser fp)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            var forgotpassword = db.tblUsers.Where(m => m.emailID.Equals(fp.emailID)).SingleOrDefault();
            if (forgotpassword != null)
            {
                forgotpasswordlink(fp.emailID);
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.error = "Account not found";
            }
            return View();
        }

        [NonAction]
        private void forgotpasswordlink(string emailId)
        {
            var forgotpwd = CreateRandomString();

            //update password in db
            var b = db.tblUsers.FirstOrDefault(m => m.emailID == emailId);
            if (b != null)
            {
                b.password = forgotpwd.ToString();
            }
            db.SaveChanges();

            //send random pwd to registered email
            var fromMail = new MailAddress("urvishapatel2311@gmail.com");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "cbxycqbqtyubhczs";
            string subject = "New Temporary Password has been created for you";
            string body = "Hello, <br/><br/>We have generated a new password for you<br/>Password: " + forgotpwd + "<br/><br/>Regards, <br/>Notes MarketPlace";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        private static string CreateRandomString()
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            // Minimum size 8. Max size is number of all allowed chars.  
            int size = random.Next(8, 24);

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public ActionResult ChangePassword()
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassViewModel cp)
        {
            if (ModelState.IsValid)
            {
                string sess = (string)Session["emailID"];
                var check = db.tblUsers.FirstOrDefault(m => m.emailID == sess);
                if (check != null)
                {
                    if (check.password == cp.oldPwd)
                    {
                        check.password = cp.NewPwd;
                        check.Confirmpassword = cp.NewPwd;
                        int i = db.SaveChanges();
                        if (i > 0)
                        {
                            ViewBag.change = "Your Password is changed successfully..!!";
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            ViewBag.changes = "Something went wrong..!!";
                        }
                    }
                    else
                    {
                        ViewBag.changepwd = "Please enter your correct old password!";
                    }
                }

            }

            return View();
        }

        public ActionResult UserProfile()
        {
            if (Session["emailID"] != null)
            {
                ViewBag.fname = Session["fname"];
                ViewBag.lname = Session["lname"];
                ViewBag.emailadd = Session["emailID"];

                //gender list
                var genderList = db.tblReferenceDatas.Where(m => m.refCategory == "Gender").ToList();
                ViewBag.GenderList = new SelectList(genderList, "id", "value");

                //get country list from database and send to view
                var countryCodelist = db.tblManageCountries.ToList();
                ViewBag.CountryCodeList = new SelectList(countryCodelist, "id", "countryCode");

                //get country list from database and send to view
                var countrylists = db.tblManageCountries.ToList();
                ViewBag.CountryLists = new SelectList(countrylists, "id", "countryName");
                ViewBag.isPageOfEdit = false;
                int sID = (int)Session["userID"];
                var check = db.tblUserProfiles.Where(m => m.userID == sID).SingleOrDefault();
                if (check!= null && check.isProfileSet)
                {
                    //gender list
                    var genderList2 = db.tblReferenceDatas.Where(m => m.refCategory == "Gender").ToList();
                    ViewBag.GenderList2 = new SelectList(genderList, "id", "value");

                    //get country list from database and send to view
                    var countryCodelist2 = db.tblManageCountries.ToList();
                    ViewBag.CountryCodeList2 = new SelectList(countryCodelist, "id", "countryCode");

                    //get country list from database and send to view
                    var countrylists2 = db.tblManageCountries.ToList();
                    ViewBag.CountryLists2 = new SelectList(countrylists, "id", "countryName");
                    ViewBag.isPageOfEdit = true;

                    ViewModel vm = new ViewModel();
                    vm.profileViewModel = check;
                    return View("UserProfile", vm);
                }
               
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }
                
        [HttpPost]
        public ActionResult UserProfile(ViewModel profile, string command)
        {
            //gender list
            var genderList = db.tblReferenceDatas.Where(m => m.refCategory == "Gender").ToList();
            ViewBag.GenderList = new SelectList(genderList, "id", "value");

            //get country list from database and send to view
            var countryCodelist = db.tblManageCountries.ToList();
            ViewBag.CountryCodeList = new SelectList(countryCodelist, "id", "countryCode");

            //get country list from database and send to view
            var countrylists = db.tblManageCountries.ToList();
            ViewBag.CountryLists = new SelectList(countrylists, "id", "countryName");
            ViewBag.isPageOfEdit = false;
            profile.profileViewModel.userID = (int)Session["userID"];
            profile.profileViewModel.createdDate = DateTime.Now;
            profile.profileViewModel.isProfileSet = false;

            if (profile.profileImage != null)
            {
                //save note picture with random number
                string p = CreateRandomString();
                string filename = Path.GetFileNameWithoutExtension(profile.profileImage.FileName);
                string extension = Path.GetExtension(profile.profileImage.FileName);
                filename = filename + p + extension;
                profile.profileViewModel.profilePicture = "~/Image/profilePicture/" + filename;
                filename = Path.Combine(Server.MapPath("~/Image/profilePicture/"), filename);
                profile.profileImage.SaveAs(filename);

            }
            if (profile.profileViewModel.phoneNo != null)
            {
                profile.profileViewModel.isProfileSet = true;
            }

            if (command == "update")
            {
                ViewBag.fname = Session["fname"];
                ViewBag.lname = Session["lname"];
                ViewBag.emailadd = Session["emailID"];
                //gender list
                var genderList2 = db.tblReferenceDatas.Where(m => m.refCategory == "Gender").ToList();
                ViewBag.GenderList2 = new SelectList(genderList, "id", "value");

                //get country list from database and send to view
                var countryCodelist2 = db.tblManageCountries.ToList();
                ViewBag.CountryCodeList2 = new SelectList(countryCodelist, "id", "countryCode");

                //get country list from database and send to view
                var countrylists2 = db.tblManageCountries.ToList();
                ViewBag.CountryLists2 = new SelectList(countrylists, "id", "countryName");

                var oldData = db.tblUserProfiles.Where(m => m.id.Equals(profile.profileViewModel.id)).SingleOrDefault();
                oldData.gender = profile.profileViewModel.gender;
                oldData.countryCode_phoneNo = profile.profileViewModel.countryCode_phoneNo;
                oldData.phoneNo = profile.profileViewModel.phoneNo;
                oldData.addressLine1 = profile.profileViewModel.addressLine1;
                oldData.addressLine2 = profile.profileViewModel.addressLine2;
                oldData.city = profile.profileViewModel.city;
                oldData.state = profile.profileViewModel.state;
                oldData.zipcode = profile.profileViewModel.zipcode;
                oldData.country = profile.profileViewModel.country;
                oldData.university = profile.profileViewModel.university;
                oldData.college = profile.profileViewModel.college;
                oldData.modifiedDate = DateTime.Now;
                oldData.modifiedBy = (int)Session["userID"];

                if (profile.profileViewModel.dob !=  null)
                {
                    oldData.dob = profile.profileViewModel.dob;
                }
                if (profile.profileImage != null)
                {
                    //save note picture with random number
                    string p = CreateRandomString();
                    string filename = Path.GetFileNameWithoutExtension(profile.profileImage.FileName);
                    string extension = Path.GetExtension(profile.profileImage.FileName);
                    filename = filename + p + extension;
                    oldData.profilePicture = "~/Image/profilePicture/" + filename;
                    filename = Path.Combine(Server.MapPath("~/Image/profilePicture/"), filename);
                    profile.profileImage.SaveAs(filename);

                }
                db.SaveChanges();
                ViewBag.isPageOfEdit = true;
                return View("UserProfile", profile);
            }
            db.tblUserProfiles.Add(profile.profileViewModel);
            db.SaveChanges();
            return RedirectToAction("SearchNote");
        }

        //GET: Homapage
        public ActionResult HomePage()
        {
            return View();
        }

        public ActionResult DeleteNote(int? id)
        {
            ViewBag.alertDownload = string.Format("Are you sure you want to delete this note. ");
            //if (ViewBag.alertDownload)
           
            tblNoteDetail noteDetail = db.tblNoteDetails.Find(id);
            db.tblNoteDetails.Remove(noteDetail);
            db.SaveChanges();
            return RedirectToAction("SellYourNote");
         }

        // GET: user (Dashboard)
        public ActionResult SellYourNote(int? pageindex,int? pageindex2, string searchTxt2, string searchTxt, string SortOrder, string SortBy)
        {
            if (Session["userID"] != null)
            {
                ViewBag.alertDownload = string.Format("Are you sure you want to delete this note?");

                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();
                List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();

                var sID = Convert.ToInt32(Session["userID"]);

                var query1 = from n in noteValue
                             join nc in categoryValue on n.noteCategory equals nc.id
                             join r in referenceValue on n.status equals r.id
                             where n.sellerID == sID
                             select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r };

                IEnumerable<ViewModel> InprogressNotes = query1.Where(m => m.noteViewModel.status != 6).ToList();
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    InprogressNotes = (IEnumerable<ViewModel>)query1.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.categoryViewModel.categoryName.ToLower().StartsWith(searchTxt.ToLower()));
                }
                InprogressNotes = sorting(SortOrder, SortBy, InprogressNotes);
                ViewBag.Inprogress = InprogressNotes.ToPagedList(pageindex ?? 1, 5);

                IEnumerable<ViewModel> PublishedNotes = query1.Where(m => m.noteViewModel.status == 6).ToList();
                if (!string.IsNullOrEmpty(searchTxt2))
                {
                    PublishedNotes = (IEnumerable<ViewModel>)query1.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt2.ToLower()) || ViewModel.noteViewModel.sellingPrice.ToString().StartsWith(searchTxt2.ToLower()));
                }
                PublishedNotes = sorting(SortOrder, SortBy, PublishedNotes);
                ViewBag.Published = PublishedNotes.ToPagedList(pageindex2 ?? 1, 5);

                ViewModel sellYourNote = new ViewModel();
                sellYourNote.myDownloads = downloadValue.Where(e => e.downloader == sID && e.isSellerHasAllowedDownload).ToList().Count;
                sellYourNote.mySoldNote = downloadValue.Where(e => e.seller == sID && e.isSellerHasAllowedDownload).ToList().Count;
                sellYourNote.moneyEarned = (int)downloadValue.Where(e => e.seller == sID && e.isSellerHasAllowedDownload).Select(m => m.purchasedPrice).ToList().Sum();
                sellYourNote.myRejectedNote = noteValue.Where(e => e.sellerID == sID && e.status == 7).ToList().Count;
                sellYourNote.buyerRequest = downloadValue.Where(e => e.seller == sID && e.isSellerHasAllowedDownload == false).ToList().Count;

                return View(sellYourNote);
            }
            return RedirectToAction("SignUp");
        }
               
        public ActionResult EditNoteDetails(int id)
        {
            //get category list from database and send to view
            var categorylist = db.tblManageNoteCategories.ToList();
            ViewBag.CategoryList = new SelectList(categorylist, "id", "categoryName");

            //get type list from database and send to view
            var citylist = db.tblManageNoteTypes.ToList();
            ViewBag.TypeList = new SelectList(citylist, "id", "typeName");

            //get country list from database and send to view
            var countrylist = db.tblManageCountries.ToList();
            ViewBag.CountryList = new SelectList(countrylist, "id", "countryName");

            var data = db.tblNoteDetails.Where(m => m.id.Equals(id)).SingleOrDefault();
            data.SelectedRadioButtonValue = data.isPaid ? "Paid" : "Free";

            ViewBag.isPageOfEdit = true;

            return View("AddNote", data);

        }

        [HttpPost]
        public ActionResult EditNoteDetails(tblNoteDetail noteDetail, string command)
        {
            var categorylist = db.tblManageNoteCategories.ToList();
            ViewBag.CategoryList = new SelectList(categorylist, "id", "categoryName");

            //get type list from database and send to view
            var citylist = db.tblManageNoteTypes.ToList();
            ViewBag.TypeList = new SelectList(citylist, "id", "typeName");

            //get country list from database and send to view
            var countrylist = db.tblManageCountries.ToList();
            ViewBag.CountryList = new SelectList(countrylist, "id", "countryName");

            var oldData = db.tblNoteDetails.Where(m => m.id.Equals(noteDetail.id)).SingleOrDefault();
            if (command == "update")
            {
                oldData.status = 3;
            }
            else
            {
                oldData.status = 6;
            }
            oldData.noteCategory = noteDetail.noteCategory;
            oldData.noteType = noteDetail.noteType;
            oldData.numberOfPage = noteDetail.numberOfPage;
            oldData.description = noteDetail.description;
            oldData.country = noteDetail.country;
            oldData.noteUniversity = noteDetail.noteUniversity;
            oldData.courseName = noteDetail.courseName;
            oldData.courseCode = noteDetail.courseCode;
            oldData.professor = noteDetail.professor;
            oldData.SelectedRadioButtonValue = noteDetail.SelectedRadioButtonValue;
            oldData.sellingPrice = noteDetail.sellingPrice;
            oldData.modifiedDate = DateTime.Now;
            oldData.modifiedBy = (int)Session["userID"];

            if (noteDetail.imgFile != null)
            {
                //save note picture with random number
                string r = CreateRandomString();
                string filename = Path.GetFileNameWithoutExtension(noteDetail.imgFile.FileName);
                string extension = Path.GetExtension(noteDetail.imgFile.FileName);
                filename = filename.Replace(" ", "_").Replace("-", "_") + r + extension;
                oldData.notePicture = "~/Image/notePhoto/" + filename;
                filename = Path.Combine(Server.MapPath("~/Image/notePhoto/"), filename);
                noteDetail.imgFile.SaveAs(filename);
            }

            if (noteDetail.pdfFile != null)
            {
                //save note attachment with random number
                string d = CreateRandomString();
                string pdffilename = Path.GetFileNameWithoutExtension(noteDetail.pdfFile.FileName);
                string extensions = Path.GetExtension(noteDetail.pdfFile.FileName);
                pdffilename = pdffilename.Replace(" ", "_").Replace("-", "_") + d + extensions;
                oldData.filePath = "~/Image/notePdf/" + pdffilename;
                pdffilename = Path.Combine(Server.MapPath("~/Image/notePdf/"), pdffilename);
                noteDetail.pdfFile.SaveAs(pdffilename);
            }
            if (noteDetail.notepreviewFile != null)
            {
                //save note preview with random number
                string n = CreateRandomString();
                string pdffilenames = Path.GetFileNameWithoutExtension(noteDetail.notepreviewFile.FileName);
                string Extensions = Path.GetExtension(noteDetail.notepreviewFile.FileName);
                pdffilenames = pdffilenames.Replace(" ", "_").Replace("-", "_") + n + Extensions;
                oldData.notePreview = "~/Image/notePreview/" + pdffilenames;
                pdffilenames = Path.Combine(Server.MapPath("~/Image/notePreview/"), pdffilenames);
                noteDetail.notepreviewFile.SaveAs(pdffilenames);
            }

            db.SaveChanges();

            ViewBag.isPageOfEdit = true;
            return RedirectToAction("SellYourNote");
        }

        // GET: user
        public ActionResult AddNote()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                //get category list from database and send to view
                var categorylist = db.tblManageNoteCategories.ToList();
                ViewBag.CategoryList = new SelectList(categorylist, "id", "categoryName");

                //get type list from database and send to view
                var citylist = db.tblManageNoteTypes.ToList();
                ViewBag.TypeList = new SelectList(citylist, "id", "typeName");

                //get country list from database and send to view
                var countrylist = db.tblManageCountries.ToList();
                ViewBag.CountryList = new SelectList(countrylist, "id", "countryName");
                ViewBag.isPageOfEdit = false;
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddNote(tblNoteDetail noteDetail, string command, string role)
        {
            try
            {
                if (noteDetail.id != 0)
                {
                    ViewBag.isPageOfEdit = true;
                }
                else
                {
                    ViewBag.isPageOfEdit = false;
                }

                //if (ModelState.IsValid)

                {
                    var categorylist = db.tblManageNoteCategories.ToList();
                    ViewBag.CategoryList = new SelectList(categorylist, "id", "categoryName");

                    var citylist = db.tblManageNoteTypes.ToList();
                    ViewBag.TypeList = new SelectList(citylist, "id", "typeName");

                    var countrylist = db.tblManageCountries.ToList();
                    ViewBag.CountryList = new SelectList(countrylist, "id", "countryName");
                    ViewBag.isPageOfEdit = false;

                    //var free_paid =  db.tblReferenceDatas.tol

                    noteDetail.sellerID = (int)Session["userID"];
                    noteDetail.actionedBy = 14;
                    noteDetail.isNoteDetailSet = false;
                    noteDetail.isActive = true;
                    noteDetail.createdDate = DateTime.Now;
                    noteDetail.adminRemark = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Nesciunt et, consequuntur! Tempora molestiae deserunt in culpa, repellendus voluptatum necessitatibus, perferendis eos ad consequuntur consequatur officiis. Alias repellendus sunt, laboriosam inventore";

                    if (noteDetail.imgFile != null)
                    {
                        //save note picture with random number
                        string r = CreateRandomString();
                        string filename = Path.GetFileNameWithoutExtension(noteDetail.imgFile.FileName);
                        string extension = Path.GetExtension(noteDetail.imgFile.FileName);
                        filename = filename.Replace(" ","_").Replace("-","_") + r + extension;
                        noteDetail.notePicture = "~/Image/notePhoto/" + filename;
                        filename = Path.Combine(Server.MapPath("~/Image/notePhoto/"), filename);
                        noteDetail.imgFile.SaveAs(filename);
                    }

                    if (noteDetail.pdfFile != null)
                    {
                        //save note attachment with random number
                        string d = CreateRandomString();
                        string pdffilename = Path.GetFileNameWithoutExtension(noteDetail.pdfFile.FileName);
                        string extensions = Path.GetExtension(noteDetail.pdfFile.FileName);
                        pdffilename = pdffilename.Replace(" ", "_").Replace("-", "_") + d + extensions;
                        noteDetail.filePath = "~/Image/notePdf/" + pdffilename;
                        pdffilename = Path.Combine(Server.MapPath("~/Image/notePdf/"), pdffilename);
                        noteDetail.pdfFile.SaveAs(pdffilename);
                    }
                    if (noteDetail.notepreviewFile != null)
                    {
                        //save note preview with random number
                        string n = CreateRandomString();
                        string pdffilenames = Path.GetFileNameWithoutExtension(noteDetail.notepreviewFile.FileName);
                        string Extensions = Path.GetExtension(noteDetail.notepreviewFile.FileName);
                        pdffilenames = pdffilenames.Replace(" ", "_").Replace("-", "_") + n + Extensions;
                        noteDetail.notePreview = "~/Image/notePreview/" + pdffilenames;
                        pdffilenames = Path.Combine(Server.MapPath("~/Image/notePreview/"), pdffilenames);
                        noteDetail.notepreviewFile.SaveAs(pdffilenames);
                    }

                    if (role == "Free")
                    {
                        noteDetail.isPaid = false;
                    }
                    else if (role == "Paid")
                    {
                        noteDetail.isPaid = true;
                    }

                    if (command == "save")
                    {
                        noteDetail.status = 3;
                        if (noteDetail.status == 3)
                        {
                            ViewBag.Saved = "Data Saved successfully";
                        }
                        
                    }
                    else if (command == "publish")
                    {
                        noteDetail.status = 6;
                        if (noteDetail.status == 6)
                        {
                            noteDetail.publishedDate = DateTime.Now;
                            ViewBag.data = "Note Published successfully";
                        }
                    }
                    
                    if (noteDetail.noteTitle != null)
                    {
                        noteDetail.isNoteDetailSet = true;
                    }
                    db.tblNoteDetails.Add(noteDetail);
                    db.SaveChanges();
                    return RedirectToAction("SellYourNote");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View();
        }

        public IEnumerable<ViewModel> sorting(string SortOrder, string SortBy, IEnumerable<ViewModel> query1)
        {
            ViewBag.SortOrder = SortOrder;
            switch (SortBy)
            {
                case "Note Title":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.noteViewModel.noteTitle).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.noteTitle).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Title":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.noteViewModel.noteTitle).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.noteTitle).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Status":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.downloadViewModel.isPaid).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.downloadViewModel.isPaid).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Category":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.downloadViewModel.noteCategory).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.downloadViewModel.noteCategory).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Buyer":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.userViewModel.emailID).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.userViewModel.emailID).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Sell Type":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.downloadViewModel.isPaid).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.downloadViewModel.isPaid).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Price":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.noteViewModel.sellingPrice).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.sellingPrice).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        query1 = query1.OrderByDescending(x => x.noteViewModel.createdDate).ToList();
                        break;
                    }
            }

            return (query1);
        }
        // GET: user
        public ActionResult BuyerRequest(int? pageindex, string searchTxt, int? buyerId, int? bookId, string buyerEmail, string SortOrder, string SortBy)
        {

            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var b1 = db.tblNoteDetails.FirstOrDefault(m => m.id == bookId);
                var b2 = db.tblDownloads.FirstOrDefault(m => m.downloader == buyerId && m.noteID == bookId);

                if (b2 != null)
                {
                    b2.isSellerHasAllowedDownload = true;
                    b2.attachementPath = b1.filePath;
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        var sellerName = Session["Name"].ToString();
                        var fromMail = new MailAddress("urvishapatel2311@gmail.com");
                        var toMail = new MailAddress(buyerEmail);
                        var frontEmailPassowrd = "cbxycqbqtyubhczs";
                        string subject = sellerName + " Allows you to download a note.";
                        string body = "Hello <br/><br/>We would like to inform you that, " + sellerName + " Allows you to download a note. Please login and see My Download tabs to download particular note.<br/><br/>Regards, <br/>Notes MarketPlace";

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

                        };
                        using (var message = new MailMessage(fromMail, toMail)
                        {
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        })
                            smtp.Send(message);
                    }
                }
                else
                {
                    ViewBag.temp = "no";
                }

                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();

                var sID = Convert.ToInt32(Session["userID"]);

                var query = from d in downloadValue
                            join u in userValue on d.downloader equals u.id
                            join n in noteValue on d.noteID equals n.id
                            join nc in categoryValue on d.noteCategory equals nc.categoryName
                            where d.seller == sID && d.isSellerHasAllowedDownload == false
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };

                query = sorting(SortOrder, SortBy, query);

                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.sellingPrice.ToString().StartsWith(searchTxt.ToLower()));
                }
                return View(query.ToPagedList(pageindex ?? 1, 10));
            }
        }

        public ActionResult ContactUs()
        {
            if (Session["userID"] != null)
            {
                ViewBag.emailAdd = Session["emailID"];
                ViewBag.fullname = Session["Name"];
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ContactUs(string fullname, string email, string sub, string comment)
        {
            ContactUsEmail(fullname, email, sub, comment);
            ViewBag.success = " Your Query sent successfully to the Adminiistrator..";
            return View();
        }

        [NonAction]
        private void ContactUsEmail(string fullname, string email, string sub, string comment)
        {
            //server emailID/support emailID
            var fromMail = new MailAddress("urvishapatel2311@gmail.com");
            //admin emailID
            var toMail = new MailAddress("shahipatel229@gmail.com");
            var frontEmailPassowrd = "cbxycqbqtyubhczs";
            string subject = fullname + " - Query";
            string body;
            if (Session["emailID"] != null)
            {
                body = "Hello <br/><br/>EmailID: " + Session["emailID"] + "<br/>Subject: " + sub + "<br/>Comments/Questions: " + comment + "<br/><br/>Regards,<br/>" + fullname;
            }
            else
            {
                body = "Hello <br/><br/>EmailID: " + email + "<br/>Subject: " + sub + "<br/>Comments/Questions: " + comment + "<br/><br/>Regards,<br/>" + fullname;
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

            };
            using (var message = new MailMessage(fromMail, toMail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        // GET: 
        public ActionResult EmailVerification()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EmailVerification(string emailID)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            var verification = db.tblUsers.FirstOrDefault(m => m.emailID == emailID);
            if (verification != null)
            {
                verification.isEmailVerified = true;
                ViewBag.Name = verification.firstName + " " + verification.lastName;
                db.SaveChanges();
            }
            return View();
        }

        [HttpPost]
        public ActionResult EmailVerification(tblUser user)
        {
            if (user.isEmailVerified == true)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        // GET: admin
        public ActionResult SearchNote(int? pageindex, string search, string university, string type, string country, string course, string category, string rating)
        {
            List<UNC_ViewModel> viewModels = new List<UNC_ViewModel>();
            List<tblNoteDetail> mynote = db.tblNoteDetails.Where(m => m.status != 3).ToList();
            foreach (tblNoteDetail nd in mynote)
            {
                UNC_ViewModel tempmodel = new UNC_ViewModel();
                tempmodel.noteDetailsViewModel = nd;
                tblManageCountry countee = db.tblManageCountries.FirstOrDefault(m => m.id.ToString().Equals(nd.country.ToString()));
                tempmodel.contryViewModel = countee.countryName;

                tblManageNoteType typee = db.tblManageNoteTypes.FirstOrDefault(m => m.id.ToString().Equals(nd.noteType.ToString()));
                tempmodel.typeViewModel = typee.typeName;

                tblManageNoteCategory catee = db.tblManageNoteCategories.FirstOrDefault(m => m.id.ToString().Equals(nd.noteCategory.ToString()));
                tempmodel.catViewModel = catee.categoryName;
                tempmodel.rating = 0;
                var temp = db.tblNoteReviews.Where(m => m.noteID == nd.id);
                if (temp != null)
                {
                    var listofreviews = temp.ToList();
                    var count = listofreviews.Count();
                    if (count != 0)
                    {
                        int sum = 0;
                        for (int i = 0; i < count; i++)
                        {
                            sum = (int)(sum + listofreviews[i].rating);
                        }
                        tempmodel.rating = sum / count;
                    }
                }
                tempmodel.reviewCount = db.tblNoteReviews.Where(m => m.noteID == nd.id).Count();
                tempmodel.inappropriate = db.tblNoteReportedIsuues.Where(m => m.noteID == nd.id).Count().ToString();
                viewModels.Add(tempmodel);
            }

            // List<tblNoteDetail> notes = db.tblNoteDetails.Where(m => m.status == 6 && m.isActive).ToList();

            if (!String.IsNullOrEmpty(search))
            {
                viewModels = viewModels.Where(m => m.noteDetailsViewModel.noteTitle.ToLower().StartsWith(search.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(university))
            {
                viewModels = viewModels.Where(m => m.noteDetailsViewModel.noteUniversity.ToLower().Equals(university.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(type))
            {
                //viewModels = viewModels.Where(m => m.noteDetailsViewModel.noteType.ToString().ToLower().Equals(type.ToLower())).ToList();
                viewModels = viewModels.Where(m => m.typeViewModel.ToLower().Equals(type.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(country))
            {
                viewModels = viewModels.Where(m => m.contryViewModel.ToLower().Equals(country.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(course))
            {
                viewModels = viewModels.Where(m => m.noteDetailsViewModel.courseName.ToLower().Equals(course.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(category))
            {
                //viewModels = viewModels.Where(m => m.noteDetailsViewModel.noteCategory.ToString().ToLower().Equals(category.ToLower())).ToList();
                viewModels = viewModels.Where(m => m.catViewModel.ToLower().Equals(category.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(rating))
            {
                //notes=notes.Where(m=>m.ra)
                viewModels = viewModels.Where(m => m.rating.ToString().Equals(rating.ToString())).ToList();
            }
            ViewBag.Count = viewModels.Count();
            return View(viewModels.ToPagedList(pageindex ?? 1, 9));
        }

        // GET: admin
        public ActionResult NoteDetails(string id)
        {
            var check = db.tblNoteDetails.FirstOrDefault(m => m.id.ToString().Equals(id));

            if (check != null)
            {
                UNC_ViewModel model = new UNC_ViewModel
                {
                    noteDetailsViewModel = check,
                    contryViewModel = db.tblManageCountries.SingleOrDefault(c => c.id == check.country).countryName,
                    userViewModel = db.tblUsers.SingleOrDefault(c => c.id == check.sellerID).emailID,
                };
                var listofreviews =  db.tblNoteReviews.Where(m => m.noteID == check.id).ToList();

                model.reviewViewModelList = new List<SingleReview>();

                var count = listofreviews.Count();
                if (count != 0)
                {
                    int sum = 0;
                    for (int i = 0; i < count; i++)
                    {
                        sum = (int)(sum + listofreviews[i].rating);

                        var userId = listofreviews[i].reviewedByID;

                        var reviewerImg = db.tblUserProfiles.FirstOrDefault(m=>m.userID == userId).profilePicture;
                        var reviewerName = db.tblUsers.FirstOrDefault(m => m.id == userId).firstName + " " + db.tblUsers.FirstOrDefault(m => m.id == userId).lastName;
                        model.reviewViewModelList.Add(new SingleReview { singlereviewViewModel = listofreviews[i],profileUrl = reviewerImg ,userName = reviewerName });

                    }
                    model.rating = sum / count;
                }

                model.reviewCount = db.tblNoteReviews.Where(m => m.noteID == check.id).Count();
                model.inappropriate = db.tblNoteReportedIsuues.Where(m => m.noteID == check.id).Count().ToString();
                return View(model);

            }
            return RedirectToAction("SearchNote");
        }

        [HttpGet]
        public ActionResult DownloadBook([Optional] string sellerEmail, int? bookId)
        {
            var q1 = db.tblNoteDetails.FirstOrDefault(m => m.id == bookId);
            if (Session["emailID"] != null)
            {
                tblDownload d1 = new tblDownload();
                var a = Convert.ToInt32(Session["userID"]);
                tblDownload d2 = db.tblDownloads.FirstOrDefault(m => m.downloader == a && m.noteID==bookId);
                if (d2 == null)
                {
                    d1.seller = db.tblUsers.SingleOrDefault(m => m.emailID == sellerEmail).id;
                    d1.noteID = q1.id;
                    string sellm = (string)Session["emailID"];
                    d1.downloader = db.tblUsers.SingleOrDefault(m => m.emailID == sellm).id;
                    d1.purchasedPrice = q1.sellingPrice;
                    d1.noteTitle = q1.noteTitle;
                    d1.noteCategory = db.tblManageNoteCategories.SingleOrDefault(m => m.id == q1.noteCategory).categoryName;
                    d1.createdDate = q1.createdDate;

                    if (q1.isPaid == false)
                    {
                        d1.isSellerHasAllowedDownload = true;
                        d1.attachementDownloadDate = DateTime.Now;
                        d1.attachementPath = q1.filePath;
                        d1.isAttachmentDownloaded = true;
                        d1.isPaid = false;
                        db.tblDownloads.Add(d1);
                        db.SaveChanges();

                        string notepath = q1.filePath;
                        var pdffilename = Path.Combine(Server.MapPath(notepath));
                        return File(pdffilename, ".pdf");
                    }
                    else
                    {
                        d1.isPaid = true;
                        d1.attachementDownloadDate = DateTime.Now;
                        d1.isSellerHasAllowedDownload = false;
                        d1.isAttachmentDownloaded = false;
                        db.tblDownloads.Add(d1);
                        db.SaveChanges();

                        var buyerName = Session["Name"].ToString();
                        ViewBag.buyername = buyerName;
                        var fromMail = new MailAddress("urvishapatel2311@gmail.com", "Undhad2311");
                        var toMail = new MailAddress(sellerEmail);
                        var frontEmailPassowrd = "cbxycqbqtyubhczs";
                        string subject = buyerName + " wants to purchase your notes.";
                        string body = "Hello <br/><br/>We would like to inform you that, " + buyerName + " wants to purchase your notes. Please see Buyer Request tab and allow download access to Buyer if you have received the payment from him.<br/><br/>Regards, <br/>Notes MarketPlace";

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromMail.Address, frontEmailPassowrd)

                        };
                        using (var message = new MailMessage(fromMail, toMail)
                        {
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        })
                            smtp.Send(message);
                    }
                }

                
                return RedirectToAction("SellYourNote");

            }
            else
            {
                return RedirectToAction("SignUp", "user");
            }
        }

        public ActionResult DownloadBookfromMyWindow([Optional] int? bookId)
        {
            var q1 = db.tblNoteDetails.FirstOrDefault(m => m.id == bookId);
            string notepath = q1.filePath;
            var pdffilename = Path.Combine(Server.MapPath(notepath));
            return File(pdffilename, ".pdf");

        }
                
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AddReview(int dID, string ratingComment, string rate)
        {
            tblNoteReview reviewOfNote = new tblNoteReview();

            ViewBag.downloadID = dID;
            ViewBag.star = rate;
            ViewBag.value = ratingComment;

            var check = db.tblDownloads.Where(m => m.noteID == dID).FirstOrDefault();

            db.Configuration.ValidateOnSaveEnabled = false;

            var findObj = db.tblNoteReviews.Where(m => m.againstDownloadID == dID).FirstOrDefault();
            if (findObj == null)
            {
                if (check != null)
                {
                    reviewOfNote.noteID = check.noteID;
                    reviewOfNote.rating = Convert.ToDecimal(rate);
                    reviewOfNote.comments = ratingComment;
                    reviewOfNote.againstDownloadID = check.id;
                    reviewOfNote.reviewedByID = (int)Session["userID"];
                    reviewOfNote.createdDate = DateTime.Now;
                    reviewOfNote.createdBy = (int)Session["userID"];
                    reviewOfNote.isActive = true;
                    db.tblNoteReviews.Add(reviewOfNote);
                    db.SaveChanges();
                }

            }
            else
            {
                findObj.rating = Convert.ToDecimal(rate);
                findObj.comments = ratingComment;
                findObj.modifiedDate = DateTime.Now;
                findObj.modifiedBy = (int)Session["userID"];

                db.SaveChanges();
            }
            return RedirectToAction("MyDownload");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ReportAsInappropriate(int dID, string inappropriateComment)
        {
            tblNoteReportedIsuue issueOfNote = new tblNoteReportedIsuue();
            var check = db.tblDownloads.Where(m => m.noteID == dID).SingleOrDefault();

            db.Configuration.ValidateOnSaveEnabled = false;

            var findObj = db.tblNoteReportedIsuues.Where(m => m.againstDownloadID == check.seller).FirstOrDefault();
            if (findObj == null)
            {
                if (check != null)
                {
                    issueOfNote.noteID = check.noteID;
                    issueOfNote.reportedByID = (int)Session["userID"];
                    issueOfNote.againstDownloadID = check.seller;
                    issueOfNote.remarks = inappropriateComment;
                    issueOfNote.createdDate = DateTime.Now;
                    issueOfNote.createdBy = (int)Session["userID"];
                    db.tblNoteReportedIsuues.Add(issueOfNote);
                    db.SaveChanges();
                }

            }
            else
            {
                findObj.remarks = inappropriateComment;
                findObj.modifiedDate = DateTime.Now;
                findObj.modifiedBy = (int)Session["userID"];
                db.SaveChanges();
            }
            return RedirectToAction("MyDownload");
        }

        public ActionResult MyDownload(int? pageindex, string searchTxt, string SortOrder, string SortBy)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var bID = Convert.ToInt32(Session["userID"]);
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();

                var query = from d in downloadValue
                            join u in userValue on d.downloader equals u.id
                            join n in noteValue on d.noteID equals n.id
                            join nc in categoryValue on d.noteCategory equals nc.categoryName
                            where d.downloader == bID && d.isSellerHasAllowedDownload
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };

                query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.sellingPrice.ToString().StartsWith(searchTxt.ToLower()));
                }
                return View(query.ToPagedList(pageindex ?? 1, 10));
            }
        }

        public ActionResult MySoldNotes(int? pageindex, string searchTxt, string SortOrder, string SortBy)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var sID = Convert.ToInt32(Session["userID"]);
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();

                var query = from d in downloadValue
                            join u in userValue on d.downloader equals u.id
                            join n in noteValue on d.noteID equals n.id
                            join nc in categoryValue on d.noteCategory equals nc.categoryName
                            where d.seller == sID && d.isSellerHasAllowedDownload
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };

                query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.sellingPrice.ToString().StartsWith(searchTxt.ToLower()));
                }
                return View(query.ToPagedList(pageindex ?? 1, 10));
            }
        }

        public ActionResult MyRejectedNote(int? pageindex, string searchTxt, string SortOrder, string SortBy)
        {
            if (Session["userID"] != null)
            {
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();
                List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();
                //List<tblDownload> downloadValue = db.tblDownloads.ToList();

                var sID = Convert.ToInt32(Session["userID"]);

                var query1 = from n in noteValue
                             join nc in categoryValue on n.noteCategory equals nc.id
                             //join d in downloadValue on n.id equals d.noteID
                             join r in referenceValue on n.status equals r.id
                             where n.sellerID == sID && n.status == 7
                             select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r/*, downloadViewModel =d*/ };

                query1 = sorting(SortOrder, SortBy, query1);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query1 = query1.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.categoryViewModel.categoryName.ToLower().StartsWith(searchTxt.ToLower()));
                }

                return View(query1.ToPagedList(pageindex ?? 1, 10));
            }
            return RedirectToAction("SignUp");
        }

        //GET: faq
        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["userID"] = null;
            Session.Abandon();
            return RedirectToAction("Login");
        }

    }
}