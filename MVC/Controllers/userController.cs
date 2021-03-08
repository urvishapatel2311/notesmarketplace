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
    public class userController : Controller
    {
        notesmarketplaceEntities db = new notesmarketplaceEntities();
        // GET: user
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tblUser user2)
        {

            try
            {
                var data = db.tblUsers.Where(m => m.emailID.Equals(user2.emailID) && m.password.Equals(user2.password)).SingleOrDefault();

                if (data != null)
                {
                    Session["userID"] = data.id;
                    Session["emailID"] = user2.emailID;
                    Session["Name"] = data.firstName + data.lastName;
                    if (data.isEmailVerified == true)
                    {
                        if (data.roleID == 5)
                        {
                            return RedirectToAction("SearchNote");
                        }
                        else if (data.roleID == 1)
                        {
                            return RedirectToAction("SearchNote");
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
        public ActionResult SignUp(tblUser user)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var check = db.tblUsers.FirstOrDefault(s => s.emailID == user.emailID);
                    if (check == null)
                    {
                        user.roleID = 5;
                        user.isEmailVerified = false;
                        user.isActive = true;
                        user.createdDate = DateTime.Now;
                        db.tblUsers.Add(user);
                        int i = db.SaveChanges();
                        if (i > 0)
                        {
                            VerifyEmail(user.emailID);
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
        public ActionResult ForgotPassword(tblUser user)
        {
            var check = db.tblUsers.Where(m => m.emailID.Equals(user.emailID)).SingleOrDefault();
            if (check != null)
            {

                return View();
            }
            else
            {
                ViewBag.Message = "Please enter valid email address";
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult forgotPassword(tblUser user)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            var data = db.tblUsers.Where(m => m.emailID.Equals(user.emailID)).SingleOrDefault();
            if (data != null)
            {
                forgotpasswordlink(user.emailID);
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
            var pwd = CreateRandomString();

            //update password in db
            var b = db.tblUsers.FirstOrDefault(m => m.emailID == emailId);
            if (b != null)
            {
                b.password = pwd.ToString();
            }
            db.SaveChanges();

            //send random pwd to registered email
            var fromMail = new MailAddress("urvishapatel2311@gmail.com");
            var toMail = new MailAddress(emailId);
            var frontEmailPassowrd = "cbxycqbqtyubhczs";
            string subject = "New Temporary Password has been created for you";
            string body = "Hello, <br/><br/>We have generated a new password for you<br/>Password: " + pwd + "<br/><br/>Regards, <br/>Notes MarketPlace";

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
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@$!%*?&";
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

        // GET: user (Dashboard)
        public ActionResult SellYourNote()
        {
            return View();
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
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddNote(tblNoteDetail noteDetail, String command, String role)
        {
            try
            {
                //if (ModelState.IsValid)

                {
                    var categorylist = db.tblManageNoteCategories.ToList();
                    ViewBag.CategoryList = new SelectList(categorylist, "id", "categoryName");

                    var citylist = db.tblManageNoteTypes.ToList();
                    ViewBag.TypeList = new SelectList(citylist, "id", "typeName");

                    var countrylist = db.tblManageCountries.ToList();
                    ViewBag.CountryList = new SelectList(countrylist, "id", "countryName");

                    //var free_paid =  db.tblReferenceDatas.tol

                    noteDetail.sellerID = (int)Session["userID"];
                    noteDetail.actionedBy = 4;
                    noteDetail.isActive = true;
                    noteDetail.createdDate = DateTime.Now;
                    noteDetail.adminRemark = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Nesciunt et, consequuntur! Tempora molestiae deserunt in culpa, repellendus voluptatum necessitatibus, perferendis eos ad consequuntur consequatur officiis. Alias repellendus sunt, laboriosam inventore";

                    if (noteDetail.imgFile != null)
                    {
                        //save note picture with random number
                        var r = CreateRandomString();
                        string filename = Path.GetFileNameWithoutExtension(noteDetail.imgFile.FileName);
                        string extension = Path.GetExtension(noteDetail.imgFile.FileName);
                        filename = filename + r + extension;
                        noteDetail.notePicture = "~/Image/notePhoto/" + filename;
                        filename = Path.Combine(Server.MapPath("~/Image/notePhoto/"), filename);
                        noteDetail.imgFile.SaveAs(filename);
                    }

                    if (noteDetail.pdfFile != null)
                    {
                        //save note attachment with random number
                        var d = CreateRandomString();
                        string pdffilename = Path.GetFileNameWithoutExtension(noteDetail.pdfFile.FileName);
                        string extensions = Path.GetExtension(noteDetail.pdfFile.FileName);
                        pdffilename = pdffilename + d + extensions;
                        noteDetail.filePath = "~/Image/notePdf/" + pdffilename;
                        pdffilename = Path.Combine(Server.MapPath("~/Image/notePdf/"), pdffilename);
                        noteDetail.pdfFile.SaveAs(pdffilename);
                    }
                    if (noteDetail.notepreviewFile != null)
                    {
                        //save note preview with random number
                        var n = CreateRandomString();
                        string pdffilenames = Path.GetFileNameWithoutExtension(noteDetail.notepreviewFile.FileName);
                        string Extensions = Path.GetExtension(noteDetail.notepreviewFile.FileName);
                        pdffilenames = pdffilenames + n + Extensions;
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

                    db.tblNoteDetails.Add(noteDetail);
                    db.SaveChanges();
                    return RedirectToAction("Login");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View();
        }

        private ActionResult AddNote(string v)
        {
            throw new NotImplementedException();
        }


        // GET: user
        public ActionResult BuyerRequest(string searching, int? buyerId, int? bookId)
        {
            if(Session["emailID"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                var q2 = db.tblNoteDetails.FirstOrDefault(m => m.id == bookId);
                var q3 = db.tblDownloads.FirstOrDefault(m => m.downloader == buyerId);

                if(q3!=null)
                {
                    q3.isSellerHasAllowedDownload = true;
                    db.Entry(q3).State = System.Data.Entity.EntityState.Modified;
                    q3.attachementPath = q2.filePath;
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.temp = "no";
                }
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
                        where d.seller == sID
                        select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };
            

            if(!string.IsNullOrEmpty(searching))
            {
                query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.Contains(searching) || ViewModel.noteViewModel.courseName.Contains(searching));
            }
            return View("BuyerRequest", query);
        }

        // GET: admin
        public ActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(string fullname, string email, string sub, string comment)
        {
            ContactUsEmail(fullname, email, sub, comment);
            return View();
        }

        [NonAction]
        private void ContactUsEmail(string fullname, string email, string sub, string comment)
        {
            //server emailID/support emailID
            var fromMail = new MailAddress("urvishapatel2311@gmail.com", "Undhad2311");
            //admin emailID
            var toMail = new MailAddress("shahipatel229@gmail.com");
            var frontEmailPassowrd = "cbxycqbqtyubhczs";
            string subject = fullname + " - Query";
            string body = "Hello <br/><br/>EmailID: " + email + "<br/>Subject: " + sub + "<br/>Comments/Questions: " + comment + "<br/><br/>Regards,<br/>" + fullname;

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
        public ActionResult SearchNote(int? pageindex, String search, String university, String type, String country, String course, String category, String rating)
        {
            //var countries = db.tblManageCountries.ToList();
            //ViewBag.Countries = new SelectList(countries, "id", "countryName");

            List<UNC_ViewModel> viewModels = new List<UNC_ViewModel>();
            List<tblNoteDetail> mynote = db.tblNoteDetails.ToList();
            foreach (tblNoteDetail nd in mynote)
            {
                UNC_ViewModel tempmodel = new UNC_ViewModel();
                tempmodel.noteDetailsViewModel = nd;
                tblManageCountry countee = db.tblManageCountries.FirstOrDefault(m => m.id.ToString().Equals(nd.country.ToString()));
                tempmodel.contryViewModel = countee.countryName;
                tempmodel.inappropriate = db.tblNoteReportedIsuues.Where(m => m.noteID == nd.id).Count().ToString();
                viewModels.Add(tempmodel);
            }

            //  List<tblNoteDetail> notes = db.tblNoteDetails.Where(m => m.status == 6 && m.isActive).ToList();

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
                viewModels = viewModels.Where(m => m.noteDetailsViewModel.tblManageNoteType.typeName.ToString().ToLower().Equals(type.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(country))
            {
                viewModels = viewModels.Where(m => m.contryViewModel.ToString().ToLower().Equals(country.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(course))
            {
                viewModels = viewModels.Where(m => m.noteDetailsViewModel.courseName.ToLower().Equals(course.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(category))
            {
                viewModels = viewModels.Where(m => m.noteDetailsViewModel.noteCategory.ToString().ToLower().Equals(category.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(rating))
            {
                //notes=notes.Where(m=>m.ra)
            }
            ViewBag.Count = viewModels.Count();
            return View(viewModels.ToPagedList(pageindex ?? 1, 9));
        }

        // GET: admin
        public ActionResult NoteDetails(String id)
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
                return View(model);

            }
            return RedirectToAction("SearchNote");
        }

        public ActionResult DownloadBook([Optional] string sellerEmail, int? bookId)
        {
            var q1 = db.tblNoteDetails.FirstOrDefault(m => m.id == bookId);



            if (Session["emailID"] != null)
            {
                tblDownload d1 = new tblDownload();
                d1.seller = db.tblUsers.SingleOrDefault(m => m.emailID == sellerEmail).id;
                d1.noteID = q1.id;
                string sellm = (string)Session["emailID"];
                d1.downloader = db.tblUsers.SingleOrDefault(m => m.emailID == sellm).id;
                d1.isSellerHasAllowedDownload = false;
                d1.isAttachmentDownloaded = false;
                d1.attachementDownloadDate = q1.createdDate;
                d1.purchasedPrice = q1.sellingPrice;
                d1.noteTitle = q1.noteTitle;
                d1.noteCategory = db.tblManageNoteCategories.SingleOrDefault(m => m.id == q1.noteCategory).categoryName;
                d1.createdDate = DateTime.Now;

                if (q1.isPaid == false)
                {
                    d1.isSellerHasAllowedDownload = true;
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
                    db.tblDownloads.Add(d1);
                    db.SaveChanges();
                    //buyer email
                    var buyerName = Session["Name"].ToString();
                    ViewBag.buyername = buyerName;
                    ViewBag.alertDownload = String.Format("Hello " + buyerName + ".<br/><br/>Your Request is Achieved successfully..");

                    var fromMail = new MailAddress("urvishapatel2311@gmail.com");
                    var toMail = new MailAddress(sellerEmail);
                    var frontEmailPassowrd = "cbxycqbqtyubhczs";
                    string subject = buyerName + " wants to purchse your notes.";
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
                    return RedirectToAction("SellYourNote");
                }


            }
            else
            {
                return RedirectToAction("SignUp", "user");
            }


        }

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