﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Business;
namespace MatchBX.Controllers
{
    public class HomeController : Controller
    {
        MatchBXNotification objNotification = new MatchBXNotification();
        MatchBXNotificationModel objNotiMod = new MatchBXNotificationModel();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Chat()
        {
            return View();
        }
        public ActionResult ChatTest()
        {
            return View();
        }
        public ActionResult ChatPartial(int prmUserId, string from)
        {
           // ViewBag.From = from;
            TempData["FromMsg"] = from;      
            return PartialView("~/Views/Shared/Chat.cshtml");
        }
        public ActionResult ChatForSendMessage(int _prmSendUserId, string from,string _prmJobSeeker,string _prmBidUserProfilePic)
        {
           // ViewBag.From = from;
            MatchBXMessage _obj = new MatchBXMessage();
            _obj.SendUserId = _prmSendUserId;
            _obj.JobSeeker = _prmJobSeeker;
            _obj.BidUserProfilePic = _prmBidUserProfilePic;
            _obj.ReceiverId = Convert.ToInt32(Session["UserId"]);
            return PartialView("~/Views/Shared/Chat.cshtml",_obj);
        }
        public ActionResult ChatFromMail(int _prmSendUserId)
        {
            // ViewBag.From = from;
            UserProfileModel _objUserModel = new UserProfileModel();
            UserProfile _profile = _objUserModel.LoadUserProfile(_prmSendUserId).FirstOrDefault();

            MatchBXMessage _obj = new MatchBXMessage();
            _obj.SendUserId = _prmSendUserId;
            _obj.BidUserProfilePic = _profile.ProfilePic;
            _obj.JobSeeker = _profile.FullName;
            _obj.ReceiverId = Convert.ToInt32(Session["UserId"]);
            Session["mailMessagId"] = 0;
            return PartialView("~/Views/Shared/Chat.cshtml", _obj);
        }
        public ActionResult LoadAllChat(int prmReceiverId, int prmSendUserId)
        {
            var userId = Convert.ToInt32(Session["UserId"]);
            MatchBXMessageModel _objModel = new MatchBXMessageModel();
            List<MatchBXMessage> _list = new List<MatchBXMessage>();
            _list = _objModel.GetChatMessage(prmReceiverId, prmSendUserId);

            if (_list.Where(m => m.ReadStatus == 0).ToList().Count > 0 && prmSendUserId != userId)
            {
                var _obj = new MatchBXMessage();
                _obj.ReceiverId = prmReceiverId;
                _obj.SendUserId = prmSendUserId;
                _obj.ReadStatus = 1;
                _objModel.ChangeReadStatus(_obj);
            }
            return Json(_list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadChatLandingDetails(int prmReceiverId)
        {
            MatchBXMessageModel _objModel = new MatchBXMessageModel();
            List<MatchBXMessage> _list = new List<MatchBXMessage>();
            _list = _objModel.GetAllChatMessage(prmReceiverId);
            if(_list.Where(m => m.ReadStatus == 0).ToList().Count == 0)
            {
                Session["MessageStatus"] = "N";
            }
            return Json(_list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckAllMsgRead(int prmReceiverId)
        {
            MatchBXMessageModel _objModel = new MatchBXMessageModel();
            List<MatchBXMessage> _list = new List<MatchBXMessage>();
            _list = _objModel.GetAllChatMessage(prmReceiverId);
            return Json(_list.Where(m => m.ReadStatus == 0).ToList().Count, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNotification()
        {
            List<MatchBXNotification> notificationList = new List<MatchBXNotification>();
            notificationList = objNotiMod.GetNotificationsForReceiver(Convert.ToInt32(Session["UserId"]));
            var unread = notificationList.Count(n => n.ReadStatus == 0);
            dynamic NotificationObject = new System.Dynamic.ExpandoObject();
            NotificationObject.List = notificationList;
            NotificationObject.Unread = unread;
            return Json(NotificationObject, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetNotificationsRead(List<MatchBXNotification> notifications)
        {
            if (notifications != null)
            {
                foreach (var notification in notifications)
                {
                    var status = objNotiMod.Save(notification);
                }
            
            Session["NotificationStatus"] = "N";
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SetNotificationsReadStatus(int ReceiverId)
        {
            objNotification.ReceiverId = ReceiverId;
            if (objNotiMod.SetNotificationsReadStatus(objNotification))
            {
                Session["NotificationStatus"] = "N";
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("failed", JsonRequestBehavior.AllowGet);
            }            
            
        }

        [HttpGet]
        public ActionResult IsUserApproved(int UserId,string Address)
        {
            var data = "Failed";
            TransactionDetailModel _TransactionDetailModel = new TransactionDetailModel();
            TransactionDetail _TransactionDetail = _TransactionDetailModel.GetList(" IsApproved,Amount ", " UserId = " + UserId + " and Address = '" + Address + "' and IsApproved <> 'F' and TransactionType = 'A'").FirstOrDefault();
            if (_TransactionDetail != null)
            {
                //data = new JavaScriptSerializer().Serialize(_ContractDetail);
                return Json(_TransactionDetail, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }

        }

    }
}