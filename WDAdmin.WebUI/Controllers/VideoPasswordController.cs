using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WDAdmin.Domain;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.WebUI.Infrastructure.ModelOperators;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Controllers
{
    [Authorize]
    public class VideoPasswordController : BaseController
    {

        private readonly IGenericRepository _repository;
        private readonly ModelReflector _reflector;
        private readonly ResourceHandler _handler;
        private readonly MasterRightsModelGenerator _masterGenerator;



        public VideoPasswordController(IGenericRepository repository) : base(repository)
        {
            _repository = repository;
            _reflector = ModelReflector.GetInstance;
            _handler = ResourceHandler.GetInstance;
            _masterGenerator = MasterRightsModelGenerator.GetInstance;
            _masterGenerator.SetRepository(_repository);
            _masterGenerator.SetReflector(_reflector);
        }

        #region

        [AuthorizeAccess("VideoPasswordNew")]
        public ActionResult Index(int id)
        {
            var model = new UserGroupVideoCatagoryCredentialModels { UserGroupId = id, VideoCatagoryId = 3, Salt = generateSalt()};
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserGroupVideoCatagoryCredentialModels model)
        {
            var culture = Session["WDCulture"].ToString();

            var gtgRights = ((MasterUserRightsModel)Session["Rights"]).UserToTemplateViewRights;
            UserGroupVideoCatagoryCredential ugvcc;

            

            try
            {
                ugvcc = (from uvc in _repository.Get<UserGroupVideoCatagoryCredential>() where uvc.UserGroupId == model.Id select uvc).SingleOrDefault();

                if (ugvcc == null)
                {
                    if (ModelState.IsValid)
                    {


                        if (model.Password == null)
                        {
                            ModelState.AddModelError("", "Indtast venligst en kode!");
                            return View();
                        }
                        
                        if (model.Password.Length <= 7){
                            ModelState.AddModelError("", "Koden skal være minimum 8 eller mere tegn!");
                            return View();
                        }else
                        {
                            var videoPassword = new UserGroupVideoCatagoryCredential();
                            videoPassword.UserGroupId = model.Id;
                            videoPassword.VideoCatagoryId = model.VideoCatagoryId;
                            videoPassword.Salt = model.Salt;
                            videoPassword.Password = Hash(model.Password, GetBytes(model.Salt));

                            CreateEntity(videoPassword, "Add UserGroupVideoCatagoryCredential Error!!", string.Empty, LogType.DbCreateError);
                            //TempData["Success"] = "Created";
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Der eksistere allerede en kode til denne gruppe!");
                    return View();
                }
            }
            catch (Exception e)
            {
                Logger.Log("UserGroupVideoCatagoryCredintialNotFound Error", e.Message, LogType.DbQueryError, LogEntryType.Error);
            }
            return View(model);
        }



        [AuthorizeAccess("VideoPasswordEdit")]
        public ActionResult Edit(int id)
        {
            var model = new UserGroupVideoCatagoryCredentialModels { UserGroupId = id, VideoCatagoryId = 3, Salt = generateSalt() };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserGroupVideoCatagoryCredentialModels model)
        {
            var culture = Session["WDCulture"].ToString();

            var gtgRights = ((MasterUserRightsModel)Session["Rights"]).UserToTemplateViewRights;
            UserGroupVideoCatagoryCredential ugvcc;

            try
            {
                ugvcc = (from uvc in _repository.Get<UserGroupVideoCatagoryCredential>() where uvc.UserGroupId == model.Id select uvc).SingleOrDefault();

                if (ugvcc != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Password == null)
                        {
                            ModelState.AddModelError("", "Indtast venligst den nye kode!");
                            return View();
                        }

                        if (model.Password.Length <= 7)
                        {
                            ModelState.AddModelError("", "Koden skal være minimum 8 eller mere tegn!");
                            return View();
                        }
                        else
                        {
                            var videoPassword = new UserGroupVideoCatagoryCredentialData();
                            videoPassword.Id = ugvcc.Id;
                            videoPassword.UserGroupId = model.Id;
                            videoPassword.VideoCatagoryId = model.VideoCatagoryId;
                            videoPassword.Salt = model.Salt;
                            videoPassword.Password = Hash(model.Password, GetBytes(model.Salt));


                            UpdateVideoPassword(videoPassword);
                            return RedirectToAction("Index");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Der eksistere ikke en kode til denne gruppe!");
                    return View();
                }
            }
            catch (Exception e)
            {
                Logger.Log("UserGroupVideoCatagoryCredintialNotFound Error", e.Message, LogType.DbQueryError, LogEntryType.Error);
            }
            return View(model);
        }





        public string Hash(string password, byte[] hashBytes)
        {
            //Console.WriteLine("Password: " + password);
            //Console.WriteLine("Salt: " + GetString(hashBytes));
            var bytes = new UTF8Encoding().GetBytes(password);
            using (var algorithm = new System.Security.Cryptography.SHA512Managed())
            {
                hashBytes = algorithm.ComputeHash(bytes);
            }
            return Convert.ToBase64String(hashBytes);
        }

        private string generateSalt()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[32]);
            return Convert.ToBase64String(salt);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        //
        //Virker ikke helt endnu.. men her skal man kunne opdatere video kode for en gruppe!
        //
        [HttpPut]
        [JsonFilter(Param = "jobject", RootType = typeof(UserGroupVideoCatagoryCredentialData))]
        public bool UpdateVideoPassword(UserGroupVideoCatagoryCredentialData jobject)
        {
            int videoPasswordId;

            //Resolve user id
            if (jobject.Id != -1) //Not test case
            {
                videoPasswordId = jobject.Id; //Get Id from JSON string
            }
            else //Test case, take the last user available
            {
                videoPasswordId = (from ugvcc in _repository.Get<UserGroupVideoCatagoryCredential>() select ugvcc.Id).First();
            }
            var videoPassword = new UserGroupVideoCatagoryCredential
            {
                Id = jobject.Id,
                VideoCatagoryId = jobject.VideoCatagoryId,
                UserGroupId = jobject.UserGroupId,
                Password = jobject.Password,
                Salt = jobject.Salt
            };
            Logger.Log("UpdatevideoPassword InitOK", "videoPasswordId: " + videoPasswordId, LogType.Ok, LogEntryType.Info);

            using (var transaction = TransactionScopeUtils.CreateTransactionScope())
            {
                if (!UpdateEntity(videoPassword, "UpdatevideoPassword Error", "videoPasswordId: " + videoPasswordId, LogType.DbCreateError))
                {
                    return false;
                }

                transaction.Complete();
            }

            Logger.Log("UpdatevideoPassword FinalOK", "videoPasswordId: " + videoPasswordId, LogType.DbCreateOk, LogEntryType.Info);
            return true;
        }


        #endregion

    }
}