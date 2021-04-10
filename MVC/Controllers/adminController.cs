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
    public class adminController : BaseController
    {
        notesmarketplaceEntities3 db = new notesmarketplaceEntities3();
        // GET: admin        

        public ActionResult Dashboard(int? pageindex, string searchTxt, string searchMonth, string SortOrder, string SortBy, string command, string UnpublishForId, string adminComment, string sellerEmail)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                var a = DateTime.Now.AddMonths(-6);
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.Where(m => m.publishedDate >= a).ToList();
                List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();

                if (!String.IsNullOrEmpty(searchMonth))
                {
                    noteValue = noteValue.Where(m => m.publishedDate.Value.ToString("Y").Equals(searchMonth)).ToList();
                }
                else
                {
                    noteValue = noteValue.Where(m => m.publishedDate.Value.ToString("Y").Equals(DateTime.Now.ToString("Y"))).ToList();
                }

                var query = from n in noteValue
                            join nc in categoryValue on n.noteCategory equals nc.id
                            join u in userValue on n.sellerID equals u.id
                            join r in referenceValue on n.status equals r.id
                            where n.status == 6
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r, userViewModel = u };

                query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.noteCategory.ToString().StartsWith(searchTxt.ToLower()));
                }
                var check = db.tblNoteDetails.Where(m => m.id.ToString().Equals(UnpublishForId)).FirstOrDefault();
                if (command == "unpublish")
                {
                    if (check != null)
                    {
                        check.actionedBy = (int)Session["userID"];
                        check.adminRemark = adminComment;
                        check.status = 8;
                        int i = db.SaveChanges();
                        var check2 = db.tblUsers.Where(m => m.emailID.Equals(sellerEmail)).FirstOrDefault();

                        if (check2 != null && i > 0)
                        {
                            string name = check2.firstName;
                            var fromMail = new MailAddress("urvishapatel2311@gmail.com", "Undhad2311");
                            var toMail = new MailAddress(sellerEmail);
                            var frontEmailPassowrd = "cbxycqbqtyubhczs";
                            string subject = "Sorry! We need to remove your notes from our portal.";
                            string body = "Hello " + name + "<br/><br/>We want to inform you that, your note, " + check.noteTitle + " has been removed from the portal.<br/>Please find our remarks as below -<br/>" + adminComment + "<br/><br/>Regards, <br/>Notes MarketPlace";

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
                }
                return View(query.ToPagedList(pageindex ?? 1, 5));
            }
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
                                    query1 = query1.OrderBy(x => x.noteViewModel.createdDate).ToList();
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
                                    query1 = query1.OrderBy(x => x.noteViewModel.noteCategory).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.noteViewModel.noteCategory).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderBy(x => x.noteViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Seller":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.userViewModel.firstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.userViewModel.firstName).ToList();
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
                                    query1 = query1.OrderBy(x => x.userViewModel.firstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.userViewModel.firstName).ToList();
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
                        query1 = query1.OrderBy(x => x.noteViewModel.createdDate).ToList();
                        break;
                    }
            }

            return (query1);
        }

        public ActionResult NotesUnderReview(int? pageindex, string searchTxt, string sellername, string command, string SortOrder, string SortBy, string ApproveForId, string adminComment, string id)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();
                List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();

                var query = from n in noteValue
                            join nc in categoryValue on n.noteCategory equals nc.id
                            join u in userValue on n.sellerID equals u.id
                            join r in referenceValue on n.status equals r.id
                            where n.status == 4 || n.status == 5
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r, userViewModel = u };

                //from Members page
                var b = db.tblUsers.Where(m => m.id.ToString().Equals(id)).FirstOrDefault();
                if (b != null)
                {
                    query = from n in noteValue
                            join nc in categoryValue on n.noteCategory equals nc.id
                            join u in userValue on n.sellerID equals u.id
                            join r in referenceValue on n.status equals r.id
                            where n.sellerID == b.id && (n.status == 4 || n.status == 5)
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r, userViewModel = u };
                }
                query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.noteCategory.ToString().StartsWith(searchTxt.ToLower()));
                }
                if (!string.IsNullOrEmpty(sellername))
                {
                    query = query.Where(m => m.userViewModel.firstName.ToLower().StartsWith(sellername.ToLower())).ToList();
                    if (query.Count() == 0)
                    {
                        ViewBag.record = "No record found for this Seller name.";
                    }
                }
                var check = db.tblNoteDetails.Where(m => m.id.ToString().Equals(ApproveForId)).FirstOrDefault();
                if (command == "approve")
                {
                    if (check != null)
                    {
                        check.actionedBy = (int)Session["userID"];
                        check.status = 6;
                        check.publishedDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                if (command == "inreview")
                {
                    if (check != null)
                    {
                        check.actionedBy = (int)Session["userID"];
                        check.status = 5;
                        db.SaveChanges();
                    }
                }
                if (command == "reject")
                {
                    if (check != null)
                    {
                        check.actionedBy = (int)Session["userID"];
                        check.adminRemark = adminComment;
                        check.status = 7;
                        db.SaveChanges();
                    }
                }
                return View(query.ToPagedList(pageindex ?? 1, 5));
            }
        }

        public ActionResult publishedNote(int? pageindex, string searchTxt, string sellername, string SortOrder, string SortBy, string command, string UnpublishForId, string adminComment, string sellerEmail, string id)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();
                List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();

                var query = from n in noteValue
                            join nc in categoryValue on n.noteCategory equals nc.id
                            join u in userValue on n.sellerID equals u.id
                            join r in referenceValue on n.status equals r.id
                            where n.status == 6
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r, userViewModel = u };

                //from Members page
                var b = db.tblUsers.Where(m => m.id.ToString().Equals(id)).FirstOrDefault();
                if (b != null)
                {
                    query = from n in noteValue
                            join nc in categoryValue on n.noteCategory equals nc.id
                            join u in userValue on n.sellerID equals u.id
                            join r in referenceValue on n.status equals r.id
                            where n.sellerID == b.id && (n.status == 6)
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r, userViewModel = u };
                }
                query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.noteCategory.ToString().StartsWith(searchTxt.ToLower()));
                }
                if (!string.IsNullOrEmpty(sellername))
                {
                    query = query.Where(m => m.userViewModel.firstName.ToLower().StartsWith(sellername.ToLower())).ToList();
                    if (query.Count() == 0)
                    {
                        ViewBag.record = "No record found for this Seller name.";
                    }
                }
                var check = db.tblNoteDetails.Where(m => m.id.ToString().Equals(UnpublishForId)).FirstOrDefault();
                if (command == "unpublish")
                {
                    check.actionedBy = (int)Session["userID"];
                    check.adminRemark = adminComment;
                    check.status = 8;
                    int i = db.SaveChanges();
                    var check2 = db.tblUsers.Where(m => m.emailID.Equals(sellerEmail)).FirstOrDefault();

                    if (check2 != null && i > 0)
                    {
                        string name = check2.firstName;
                        var fromMail = new MailAddress("urvishapatel2311@gmail.com", "Undhad2311");
                        var toMail = new MailAddress(sellerEmail);
                        var frontEmailPassowrd = "cbxycqbqtyubhczs";
                        string subject = "Sorry! We need to remove your notes from our portal.";
                        string body = "Hello " + name + "<br/><br/>We want to inform you that, your note, " + check.noteTitle + " has been removed from the portal.<br/>Please find our remarks as below -<br/>" + adminComment + "<br/><br/>Regards, <br/>Notes MarketPlace";

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
                return View(query.ToPagedList(pageindex ?? 1, 5));
            }
        }

        public ActionResult downloadedNote(int? pageindex, string searchTxt, string sellername, string buyername, string bookname, string SortOrder, string SortBy, string id, string bookid)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();
                List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();

                var query = from d in downloadValue
                            join u in userValue on d.downloader equals u.id
                            join n in noteValue on d.noteID equals n.id
                            join nc in categoryValue on d.noteCategory equals nc.categoryName
                            where d.isAttachmentDownloaded
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };

                //from Members page for total earning
                var b = db.tblUsers.Where(m => m.id.ToString().Equals(id)).FirstOrDefault();
                if (b != null)
                {
                    query = from d in downloadValue
                            join u in userValue on d.downloader equals u.id
                            join n in noteValue on d.noteID equals n.id
                            join nc in categoryValue on d.noteCategory equals nc.categoryName
                            where d.downloader == b.id && d.isAttachmentDownloaded
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };
                }

                //from published note for no of downloads
                var p = db.tblNoteDetails.Where(m => m.id.ToString().Equals(bookid)).FirstOrDefault();
                if (p != null)
                {
                    query = from d in downloadValue
                            join u in userValue on d.downloader equals u.id
                            join n in noteValue on d.noteID equals n.id
                            join nc in categoryValue on d.noteCategory equals nc.categoryName
                            where d.noteID == p.id && d.isAttachmentDownloaded
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };
                }

                //from Members page for total expense
                //var e = db.tblUsers.Where(m => m.id.ToString().Equals(buyerid)).FirstOrDefault();
                //if (e != null)
                //{
                //    query = from d in downloadValue
                //            join u in userValue on d.downloader equals u.id
                //            join n in noteValue on d.noteID equals n.id
                //            join nc in categoryValue on d.noteCategory equals nc.categoryName
                //            where d.downloader == e.id
                //            select new ViewModel { noteViewModel = n, categoryViewModel = nc, downloadViewModel = d, userViewModel = u };
                //}

                query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.noteCategory.ToString().StartsWith(searchTxt.ToLower()));
                }
                if (!string.IsNullOrEmpty(buyername))
                {
                    query = query.Where(m => m.userViewModel.firstName.ToLower().StartsWith(buyername.ToLower())).ToList();
                    if (query.Count() == 0)
                    {
                        ViewBag.record = "No record found for this Seller name.";
                    }
                }
                if (!string.IsNullOrEmpty(sellername))
                {
                    query = query.Where(m => m.noteViewModel.tblUser1.firstName.ToLower().StartsWith(sellername.ToLower())).ToList();
                    if (query.Count() == 0)
                    {
                        ViewBag.record = "No record found for this Buyer name.";
                    }
                }
                if (!string.IsNullOrEmpty(bookname))
                {
                    query = query.Where(m => m.noteViewModel.noteTitle.ToLower().StartsWith(bookname.ToLower())).ToList();
                    if (query.Count() == 0)
                    {
                        ViewBag.record = "No record found for this Seller name.";
                    }
                }
                return View(query.ToPagedList(pageindex ?? 1, 5));
            }
        }

        public ActionResult rejectedNote(int? pageindex, string searchTxt, string sellername, string SortOrder, string SortBy, string PublishForId, string command)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblUser> userValue = db.tblUsers.ToList();
                List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();
                List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();
                List<tblDownload> downloadValue = db.tblDownloads.ToList();

                var query = from n in noteValue
                            join nc in categoryValue on n.noteCategory equals nc.id
                            join u in userValue on n.sellerID equals u.id
                            //join r in referenceValue on n.status equals r.id
                            where n.status == 7
                            select new ViewModel { noteViewModel = n, categoryViewModel = nc, /*referenceViewModel = r,*/ userViewModel = u };

                query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.noteViewModel.noteTitle.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.noteViewModel.noteCategory.ToString().StartsWith(searchTxt.ToLower()));
                }
                if (!string.IsNullOrEmpty(sellername))
                {
                    query = query.Where(m => m.userViewModel.firstName.ToLower().StartsWith(sellername.ToLower())).ToList();
                    if (query.Count() == 0)
                    {
                        ViewBag.record = "No record found for this Seller name.";
                    }
                }

                if (command == "publish")
                {
                    var check = db.tblNoteDetails.Where(m => m.id.ToString().Equals(PublishForId)).FirstOrDefault();
                    if (check != null)
                    {
                        check.actionedBy = (int)Session["userID"];
                        check.adminRemark = null;
                        check.status = 6;
                        db.SaveChanges();
                    }
                }
                return View(query.ToPagedList(pageindex ?? 1, 5));
            }

        }

        public ActionResult members(int? pageindex, string searchTxt, string SortOrder, string SortBy, string DeactivateForId, string command)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblUser> userValue = db.tblUsers.ToList();
                var query = from u in userValue
                            select new ViewModel { userViewModel = u };
                //query = sorting(SortOrder, SortBy, query);
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(ViewModel => ViewModel.userViewModel.firstName.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.userViewModel.lastName.ToString().StartsWith(searchTxt.ToLower()) || ViewModel.userViewModel.emailID.ToString().StartsWith(searchTxt.ToLower()) || ViewModel.userViewModel.createdDate.ToString().StartsWith(searchTxt.ToLower()));
                }
                query = sorting2(SortOrder, SortBy, query);
                var check = db.tblUsers.Where(m => m.id.ToString().Equals(DeactivateForId)).FirstOrDefault();
                var check2 = db.tblNoteDetails.Where(m => m.sellerID.ToString().Equals(DeactivateForId)).ToList();
                if (command == "inActive")
                {
                    if (check != null)
                    {
                        check.isActive = false;
                        check.Confirmpassword = check.password;
                        if (check2.Count() > 0)
                        {
                            foreach (tblNoteDetail item in check2)
                            {
                                item.actionedBy = (int)Session["userID"];
                                item.status = 8;
                            }
                        }
                    }

                    db.SaveChanges();
                }
                return View(query.ToPagedList(pageindex ?? 1, 5));
            }
        }

        public ActionResult memberDetails(string id, int? pageindex, string SortOrder, string SortBy)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                var check = db.tblUserProfiles.Where(m => m.userID.ToString().Equals(id)).FirstOrDefault();
                if (check != null)
                {
                    ViewModel model = new ViewModel
                    {
                        profileViewModel = check
                    };
                    List<tblUser> userValue = db.tblUsers.ToList();
                    List<tblManageNoteCategory> categoryValue = db.tblManageNoteCategories.ToList();
                    List<tblNoteDetail> noteValue = db.tblNoteDetails.ToList();
                    List<tblReferenceData> referenceValue = db.tblReferenceDatas.ToList();
                    List<tblDownload> downloadValue = db.tblDownloads.ToList();

                    var query = from n in noteValue
                                join nc in categoryValue on n.noteCategory equals nc.id
                                join u in userValue on n.sellerID equals u.id
                                join r in referenceValue on n.status equals r.id
                                where n.status == 6
                                select new ViewModel { noteViewModel = n, categoryViewModel = nc, referenceViewModel = r, userViewModel = u };

                    query = sorting(SortOrder, SortBy, query);
                    IEnumerable<ViewModel> notes = query.Where(m => m.noteViewModel.sellerID.ToString().Equals(id) && m.noteViewModel.status != 3);
                    ViewModel details = new ViewModel();
                    ViewBag.notes = notes.ToPagedList(pageindex ?? 1, 5);
                    return View("memberDetails", model);
                }
            }
            return View();
        }

        public ActionResult manageAdministrator(int? pageindex, string searchTxt, string DeactivateForId, string command)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                var ID = Convert.ToInt32(Session["userID"]);
                var check = db.tblUsers.Where(m => m.id.Equals(ID)).FirstOrDefault();
                if (check.roleID == 1002)
                {
                    List<tblUser> userValue = db.tblUsers.ToList();
                    List<tblUserProfile> userProfileValue = db.tblUserProfiles.ToList();

                    var query = from u in userValue
                                join up in userProfileValue on u.id equals up.userID
                                where u.roleID == 1 && u.isActive
                                select new ViewModel { userViewModel = u, profileViewModel = up };

                    if (!string.IsNullOrEmpty(searchTxt))
                    {
                        query = query.Where(ViewModel => ViewModel.userViewModel.firstName.ToLower().StartsWith(searchTxt.ToLower()) || ViewModel.userViewModel.lastName.ToString().StartsWith(searchTxt.ToLower()) || ViewModel.userViewModel.emailID.ToString().StartsWith(searchTxt.ToLower()) || ViewModel.userViewModel.createdDate.ToString().StartsWith(searchTxt.ToLower()));
                    }

                    if (command == "inActive")
                    {
                        var check2 = db.tblUsers.Where(m => m.id.ToString().Equals(DeactivateForId)).FirstOrDefault();
                        if (check2 != null)
                        {
                            check2.isActive = false;
                            check2.Confirmpassword = check2.password;
                            db.SaveChanges();
                        }
                    }

                    return View(query.ToPagedList(pageindex ?? 1, 5));

                }
                else
                {
                    return HttpNotFound();
                }

            }
        }

        public ActionResult EditmanageAdministrator(int id)
        {
            ViewModel model = new ViewModel();

            //get country list from database and send to view
            var countryCodelist = db.tblManageCountries.ToList();
            ViewBag.CountryCodeList = new SelectList(countryCodelist, "id", "countryCode");

            model.userViewModel = db.tblUsers.Where(m => m.id.Equals(id)).SingleOrDefault();
            model.profileViewModel = db.tblUserProfiles.Where(m => m.userID.Equals(id)).SingleOrDefault();
            ViewBag.isPageOfEdit = true;
            return View("addAdmin", model);
        }

        [HttpPost]
        public ActionResult EditmanageAdministrator(ViewModel model, string command)
        {
            //get country list from database and send to view
            var countryCodelist = db.tblManageCountries.ToList();
            ViewBag.CountryCodeList = new SelectList(countryCodelist, "id", "countryCode");

            var oldData = db.tblUsers.Where(m => m.id.Equals(model.userViewModel.id)).SingleOrDefault();
            var oldData2 = db.tblUserProfiles.Where(m => m.userID.Equals(model.userViewModel.id)).SingleOrDefault();
            if (command == "update")
            {
                var ID = Convert.ToInt32(Session["userID"]);
                oldData.firstName = model.userViewModel.firstName;
                oldData.lastName = model.userViewModel.lastName;
                oldData.emailID = model.userViewModel.emailID;
                oldData.Confirmpassword = oldData.password;
                oldData.modifiedDate = DateTime.Now;
                oldData.modifiedBy = ID;
                oldData2.countryCode_phoneNo = model.profileViewModel.countryCode_phoneNo;
                oldData2.phoneNo = model.profileViewModel.phoneNo;
                db.SaveChanges();
            }
            ViewBag.isPageOfEdit = true;
            return RedirectToAction("manageAdministrator");
        }

        public ActionResult myProfile()
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
                ViewBag.isPageOfEdit = false;
                int ID = (int)Session["userID"];
                var check = db.tblUserProfiles.Where(m => m.userID == ID).SingleOrDefault();
                if (check != null && check.isProfileSet)
                {
                    //gender list
                    var genderList2 = db.tblReferenceDatas.Where(m => m.refCategory == "Gender").ToList();
                    ViewBag.GenderList2 = new SelectList(genderList, "id", "value");

                    //get country list from database and send to view
                    var countryCodelist2 = db.tblManageCountries.ToList();
                    ViewBag.CountryCodeList2 = new SelectList(countryCodelist, "id", "countryCode");


                    ViewModel vm = new ViewModel();
                    vm.profileViewModel = check;
                    ViewBag.isPageOfEdit = true;
                    return View("myProfile", vm);
                }
            }
            else
            {
                return RedirectToAction("Login", "user");
            }
            return View();
        }
        [HttpPost]
        public ActionResult myProfile(ViewModel model, string command)
        {
            //gender list
            var genderList = db.tblReferenceDatas.Where(m => m.refCategory == "Gender").ToList();
            ViewBag.GenderList = new SelectList(genderList, "id", "value");

            //get country list from database and send to view
            var countryCodelist = db.tblManageCountries.ToList();
            ViewBag.CountryCodeList = new SelectList(countryCodelist, "id", "countryCode");
            ViewBag.isPageOfEdit = false;

            if (model.profileImage != null)
            {
                //save note picture with random number
                string p = CreateRandomString();
                string filename = Path.GetFileNameWithoutExtension(model.profileImage.FileName);
                string extension = Path.GetExtension(model.profileImage.FileName);
                filename = filename + p + extension;
                model.profileViewModel.profilePicture = "~/Image/profilePicture/" + filename;
                filename = Path.Combine(Server.MapPath("~/Image/profilePicture/"), filename);
                model.profileImage.SaveAs(filename);

            }
            if (model.profileViewModel.phoneNo != null)
            {
                model.profileViewModel.isProfileSet = true;
            }
            if (command == "update")
            {
                int ID = (int)Session["userID"];
                //ViewBag.emailadd = Session["emailID"];
                //gender list
                var genderList2 = db.tblReferenceDatas.Where(m => m.refCategory == "Gender").ToList();
                ViewBag.GenderList = new SelectList(genderList2, "id", "value");

                //get country list from database and send to view
                var countryCodelist2 = db.tblManageCountries.ToList();
                ViewBag.CountryCodeList = new SelectList(countryCodelist2, "id", "countryCode");

                var oldData = db.tblUsers.Where(m => m.id.Equals(ID)).SingleOrDefault();
                var oldData2 = db.tblUserProfiles.Where(m => m.userID.Equals(ID)).SingleOrDefault();

                oldData.firstName = model.fname;
                oldData.lastName = model.lname;
                oldData.emailID = /*model.userViewModel.emailID;*/(string)Session["emailID"];
                oldData.Confirmpassword = oldData.password;
                oldData.modifiedDate = DateTime.Now;
                oldData.modifiedBy = ID;
                oldData2.countryCode_phoneNo = model.profileViewModel.countryCode_phoneNo;
                oldData2.phoneNo = model.profileViewModel.phoneNo;
                oldData2.profilePicture = model.profileViewModel.profilePicture;

                db.SaveChanges();
            }
            ViewBag.isPageOfEdit = true;
            return RedirectToAction("myProfile");
        }

        public ActionResult addAdmin()
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                //get country list from database and send to view
                var countryCodelist = db.tblManageCountries.ToList();
                ViewBag.CountryCodeList = new SelectList(countryCodelist, "id", "countryCode");

                var ID = Convert.ToInt32(Session["userID"]);
                var check = db.tblUsers.Where(m => m.id.Equals(ID)).FirstOrDefault();
                ViewBag.isPageOfEdit = false;
                if (check.roleID == 1002)
                {
                    return View();
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        private static string CreateRandomString2()
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            string validChars2 = "abcdefghijklmnopqrstuvwxyz";
            string validChars3 = "0123456789";
            string validChars4 = "@$!%*?&";
            Random random = new Random();

            char[] chars = new char[8];
            for (int i = 0; i < 8; i++)
            {
                if (i < 2)
                {
                    chars[i] = validChars[random.Next(0, validChars.Length)];
                }
                else if (i > 1 && i < 5)
                {
                    chars[i] = validChars2[random.Next(0, validChars2.Length)];
                }
                else if (i > 4 && i < 7)
                {
                    chars[i] = validChars3[random.Next(0, validChars3.Length)];
                }
                else
                {
                    chars[i] = validChars4[random.Next(0, validChars4.Length)];
                }

            }
            T[] Shuffle<T>(Random rng, T[] array)
            {
                int n = array.Length;
                while (n > 1)
                {
                    int k = rng.Next(n--);
                    T temp = array[n];
                    array[n] = array[k];
                    array[k] = temp;
                }
                return array;
            }
            return new string(Shuffle<char>(random, chars));
        }

        [HttpPost]
        public ActionResult addAdmin(ViewModel addAdmin)
        {
            if (addAdmin.userViewModel.id != 0)
            {
                ViewBag.isPageOfEdit = true;
            }
            else
            {
                ViewBag.isPageOfEdit = false;
            }
            var ID = Convert.ToInt32(Session["userID"]);
            var random = CreateRandomString2();
            addAdmin.userViewModel.roleID = 1;
            addAdmin.userViewModel.createdDate = DateTime.Now;
            addAdmin.userViewModel.createdBy = ID;
            addAdmin.userViewModel.isActive = true;
            addAdmin.userViewModel.isEmailVerified = false;
            addAdmin.userViewModel.password = random;
            addAdmin.userViewModel.Confirmpassword = random;
            ViewBag.isPageOfEdit = false;

            db.tblUsers.Add(addAdmin.userViewModel);
            int j = db.SaveChanges();
            if (j > 0)
            {
                addAdmin.profileViewModel.userID = addAdmin.userViewModel.id;
                addAdmin.profileViewModel.isProfileSet = true;
                db.tblUserProfiles.Add(addAdmin.profileViewModel);
                int i = db.SaveChanges();
                if (i > 0)
                {
                    //send random pwd to Admin
                    var fromMail = new MailAddress("urvishapatel2311@gmail.com");
                    var toMail = new MailAddress(addAdmin.userViewModel.emailID);
                    var frontEmailPassowrd = "cbxycqbqtyubhczs";
                    string subject = "New Temporary Password has been created for you";
                    string body = "Hello " + addAdmin.userViewModel.firstName + ",<br/><br/>We have generated a new password for you<br/>Password: <b>" + random + "</b><br/>You can change your password via your profile settings.<br/><br/>Regards, <br/>Notes MarketPlace";

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
                    return Redirect("manageAdministrator");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }


        }

        public ActionResult manageSystemConfiguration()
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                var ID = Convert.ToInt32(Session["userID"]);
                var check = db.tblUsers.Where(m => m.id.Equals(ID)).FirstOrDefault();
                if (check.roleID == 1002)
                {
                    return View();
                }
                else
                {
                    return HttpNotFound();
                }
            }

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

        [HttpPost]
        public ActionResult manageSystemConfiguration(tblSystemConfig config)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.tblSystemConfigs.FirstOrDefault(s => s.supportEmailID == config.supportEmailID);
                    if (check == null)
                    {
                        config.isActive = true;
                        config.createdDate = DateTime.Now;
                        config.createdBy = (int?)Session["userID"];

                        if (config.noteImgFile != null)
                        {
                            //save note picture with random number
                            string r = CreateRandomString();
                            string filename = Path.GetFileNameWithoutExtension(config.noteImgFile.FileName);
                            string extension = Path.GetExtension(config.noteImgFile.FileName);
                            filename = filename.Replace(" ", "_").Replace("-", "_") + r + extension;
                            config.defaultNoteImage = "~/Image/adminDefaultData/" + filename;
                            filename = Path.Combine(Server.MapPath("~/Image/adminDefaultData/"), filename);
                            config.noteImgFile.SaveAs(filename);
                        }

                        if (config.profileImgFile != null)
                        {
                            //save note picture with random number
                            string r = CreateRandomString();
                            string filename = Path.GetFileNameWithoutExtension(config.profileImgFile.FileName);
                            string extension = Path.GetExtension(config.profileImgFile.FileName);
                            filename = filename.Replace(" ", "_").Replace("-", "_") + r + extension;
                            config.defaultProfilePicture = "~/Image/adminDefaultData/" + filename;
                            filename = Path.Combine(Server.MapPath("~/Image/adminDefaultData/"), filename);
                            config.profileImgFile.SaveAs(filename);
                        }

                        db.tblSystemConfigs.Add(config);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong!!", ex);
            }
            return View();
        }

        public ActionResult addCategory()
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            return View();

        }
        [HttpPost]
        public ActionResult addCategory(tblManageNoteCategory tmc)
        {
            if (ModelState.IsValid)
            {
                tmc.addedBy = (int)Session["userID"];
                tmc.createdDate = DateTime.Now;
                tmc.isActive = true;
                db.tblManageNoteCategories.Add(tmc);
                db.SaveChanges();

            }
            else
            {
                Console.WriteLine("Something went wrong!!");
            }
            return View();

        }

        public ActionResult manageCategory(int? pageindex, string searchTxt, string command, string DeleteForId)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblManageNoteCategory> a = db.tblManageNoteCategories.Where(m => m.isActive).ToList();
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    a = a.Where(m => m.categoryName.ToString().ToLower().StartsWith(searchTxt.ToLower())).ToList();
                }
                if (command == "delete")
                {
                    var check = db.tblManageNoteCategories.Where(m => m.id.ToString().Equals(DeleteForId)).FirstOrDefault();
                    if (check != null)
                    {
                        check.isActive = false;
                        db.SaveChanges();
                    }
                }
                return View(a.ToPagedList(pageindex ?? 1, 5));
            }

        }

        public ActionResult editCategory(int id)
        {
            var data = db.tblManageNoteCategories.Where(m => m.id.Equals(id)).SingleOrDefault();

            return View("addCategory", data);
        }
        [HttpPost]

        public ActionResult editCategory(tblManageNoteCategory tmc, string command)
        {
            var oldData = db.tblManageNoteCategories.Where(m => m.id.Equals(tmc.id)).SingleOrDefault();

            if (command == "update")
            {
                oldData.categoryName = tmc.categoryName;
                oldData.description = tmc.description;
                oldData.addedBy = (int)Session["userID"];
                oldData.createdDate = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("manageCategory");
        }

        public ActionResult addType()
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            return View();
        }
        [HttpPost]
        public ActionResult addType(tblManageNoteType tmt)
        {
            if (ModelState.IsValid)
            {
                tmt.addedBy = (int)Session["userID"];
                tmt.createdDate = DateTime.Now;
                tmt.isActive = true;
                db.tblManageNoteTypes.Add(tmt);
                db.SaveChanges();

            }
            else
            {
                Console.WriteLine("Something went wrong!!");
            }
            return View();
        }

        public ActionResult manageType(int? pageindex, string searchTxt, string command, string DeleteForId)
        {
            ViewBag.alertDownload = string.Format("Are you sure you want to delete this note?");

            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblManageNoteType> a = db.tblManageNoteTypes.Where(m => m.isActive).ToList();
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    a = a.Where(m => m.typeName.ToString().ToLower().StartsWith(searchTxt.ToLower())).ToList();
                }
                if (command == "delete")
                {
                    var check = db.tblManageNoteTypes.Where(m => m.id.ToString().Equals(DeleteForId)).FirstOrDefault();
                    if (check != null)
                    {
                        check.isActive = false;
                        db.SaveChanges();
                    }
                }
                return View(a.ToPagedList(pageindex ?? 1, 5));
            }


        }

        public ActionResult editType(int id)
        {
            var data = db.tblManageNoteTypes.Where(m => m.id.Equals(id)).SingleOrDefault();

            return View("addType", data);
        }
        [HttpPost]
        public ActionResult editType(tblManageNoteType tmt, string command)
        {
            var oldData = db.tblManageNoteTypes.Where(m => m.typeName.Equals(tmt.typeName)).SingleOrDefault();

            if (command == "update")
            {
                oldData.addedBy = (int)Session["userID"];
                oldData.typeName = tmt.typeName;
                oldData.description = tmt.description;

                oldData.createdDate = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("manageType");
        }

        public ActionResult addCountry()
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            return View();
        }
        [HttpPost]
        public ActionResult addCountry(tblManageCountry tmc)
        {
            if (ModelState.IsValid)
            {
                tmc.addedBy = (int)Session["userID"];
                tmc.createdDate = DateTime.Now;
                tmc.isActive = true;
                db.tblManageCountries.Add(tmc);
                db.SaveChanges();

            }
            else
            {
                Console.WriteLine("Something went wrong!!");
            }
            return View();
        }

        public ActionResult manageCountry(int? pageindex, string searchTxt, string command, string DeleteForId)
        {
            ViewBag.alertDownload = string.Format("Are you sure you want to delete this note?");

            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                List<tblManageCountry> a = db.tblManageCountries.Where(m => m.isActive).ToList();
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    a = a.Where(m => m.countryName.ToString().ToLower().StartsWith(searchTxt.ToLower())).ToList();
                }
                if (command == "delete")
                {
                    var check = db.tblManageCountries.Where(m => m.id.ToString().Equals(DeleteForId)).FirstOrDefault();
                    if (check != null)
                    {
                        check.isActive = false;
                        db.SaveChanges();
                    }
                }
                return View(a.ToPagedList(pageindex ?? 1, 5));
            }



        }

        public ActionResult editCountry(int id)
        {
            var data = db.tblManageCountries.Where(m => m.id.Equals(id)).SingleOrDefault();

            return View("addCountry", data);
        }
        [HttpPost]
        public ActionResult editCountry(tblManageCountry tmc, string command, int id)
        {
            var oldData = db.tblManageCountries.Where(m => m.id.Equals(tmc.id)).SingleOrDefault();

            if (command == "update")
            {
                oldData.countryName = tmc.countryName;
                oldData.countryCode = tmc.countryCode;
                oldData.addedBy = (int)Session["userID"];
                oldData.createdDate = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("manageCountry");
        }

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
                var listofreviews = db.tblNoteReviews.Where(m => m.noteID == check.id).ToList();

                model.reviewViewModelList = new List<SingleReview>();

                var count = listofreviews.Count();
                if (count != 0)
                {
                    int sum = 0;
                    for (int i = 0; i < count; i++)
                    {
                        sum = (int)(sum + listofreviews[i].rating);

                        var userId = listofreviews[i].reviewedByID;

                        var reviewerImg = db.tblUserProfiles.FirstOrDefault(m => m.userID == userId).profilePicture;
                        var reviewerName = db.tblUsers.FirstOrDefault(m => m.id == userId).firstName + " " + db.tblUsers.FirstOrDefault(m => m.id == userId).lastName;
                        model.reviewViewModelList.Add(new SingleReview { singlereviewViewModel = listofreviews[i], profileUrl = reviewerImg, userName = reviewerName });

                    }
                    model.rating = sum / count;
                }

                model.reviewCount = db.tblNoteReviews.Where(m => m.noteID == check.id).Count();
                model.inappropriate = db.tblNoteReportedIsuues.Where(m => m.noteID == check.id).Count().ToString();
                return View(model);

            }
            return RedirectToAction("SearchNote","user");
        }

        public ActionResult spamReport(int? pageindex, string searchTxt, string command, int? DeleteForId)
        {
            if (Session["emailID"] == null)
            {
                return RedirectToAction("Login", "user");
            }
            else
            {
                var q = db.tblNoteReportedIsuues.ToList();
                if (command == "delete")
                {
                    tblNoteReportedIsuue reportedissue = db.tblNoteReportedIsuues.Find(DeleteForId);
                    db.tblNoteReportedIsuues.Remove(reportedissue);
                    db.SaveChanges();
                }
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    q = q.Where(m => m.tblNoteDetail.noteTitle.ToString().ToLower().StartsWith(searchTxt.ToLower()) || m.tblNoteDetail.tblManageNoteCategory.categoryName.ToString().ToLower().StartsWith(searchTxt.ToLower()) || m.tblUser.firstName.ToString().ToLower().StartsWith(searchTxt.ToLower()) || m.remarks.ToString().ToLower().StartsWith(searchTxt.ToLower()) || m.createdDate.ToString().ToLower().StartsWith(searchTxt.ToString().ToLower())).ToList();
                }
                return View(q.ToPagedList(pageindex ?? 1, 5));
            }
        }
        public IEnumerable<ViewModel> sorting2(string SortOrder, string SortBy, IEnumerable<ViewModel> query1)
        {
            ViewBag.SortOrder = SortOrder;
            switch (SortBy)
            {
                case "First Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.userViewModel.firstName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.userViewModel.firstName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1 = query1.OrderByDescending(x => x.userViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Last Name":
                    {
                        switch (SortOrder)
                        {
                            case "Asc":
                                {
                                    query1 = query1.OrderBy(x => x.userViewModel.lastName).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    query1 = query1.OrderByDescending(x => x.userViewModel.lastName).ToList();
                                    break;
                                }
                            default:
                                {
                                    query1.OrderByDescending(x => x.userViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                case "Email":
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
                                    query1 = query1.OrderByDescending(x => x.userViewModel.createdDate).ToList();
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        query1 = query1.OrderByDescending(x => x.userViewModel.createdDate).ToList();
                        break;
                    }
            }

            return (query1);
        }
    }
}