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


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //-------------------------------------------------------Create------------------------------------------------------------------
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [AuthorizeAccess("VideoPasswordNew")]
        public ActionResult Index(int id)
        {
            var model = new UserGroupVideoCatagoryCredentialModels { UserGroupId = id, VideoCatagoryId = 3, Salt = generateSalt() };
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
                            ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("VideoPasswordEmpty", culture));
                            return View();
                        }

                        if (model.Password.Length <= 7)
                        {
                            ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("VideoPasswordMinimum8", culture));
                            return View();
                        }
                        else
                        {
                            var videoPassword = new UserGroupVideoCatagoryCredential();
                            videoPassword.UserGroupId = model.Id;
                            videoPassword.VideoCatagoryId = model.VideoCatagoryId;
                            videoPassword.Salt = model.Salt;
                            videoPassword.Password = Hash(model.Password, GetBytes(model.Salt));

                            CreateEntity(videoPassword, "Add UserGroupVideoCatagoryCredential Error!!", string.Empty, LogType.DbCreateError);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("VideoPasswordAddPasswordExsists", culture));
                    return View();
                }
            }
            catch (Exception e)
            {
                Logger.Log("UserGroupVideoCatagoryCredintialNotFound Error", e.Message, LogType.DbQueryError, LogEntryType.Error);
            }
            return View(model);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //-------------------------------------------------------EDIT--------------------------------------------------------------------
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [AuthorizeAccess("VideoPasswordEdit")]
        public ActionResult Edit(int id)
        {
            var model = new UserGroupVideoCatagoryCredentialModels { UserGroupId = id, VideoCatagoryId = 3, Salt = generateSalt() };
            return View(model);
        }


        //Edit Video Password for Usergroup.
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
                            ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("VideoPasswordEmpty", culture));
                            return View();
                        }

                        if (model.Password.Length <= 7)
                        {
                            ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("VideoPasswordMinimum8", culture));
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


                            if (!UpdateVideoPassword(videoPassword))
                            {
                                ModelState.AddModelError("", "Der eksistere ikke en kode til denne gruppe!");
                            }

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("VideoPasswordEditPasswordExsists", culture));
                    return View();
                }
            }
            catch (Exception e)
            {
                Logger.Log("UserGroupVideoCatagoryCredintialNotFound Error", e.Message, LogType.DbQueryError, LogEntryType.Error);
            }
            return View(model);
        }

        private bool UpdateVideoPassword(UserGroupVideoCatagoryCredentialData Credential)
        {
            bool updateSucces = false;
            int userGroupId;

            //Resolve id
            if (Credential.UserGroupId != -1) //Not test case
            {
                userGroupId = Credential.UserGroupId; //Get Id from JSON string
            }
            else //Test case, take the last user available
            {
                userGroupId = (from ugvcc in _repository.Get<UserGroup>() select ugvcc.Id).First();
            }

            Logger.Log("UpdatevideoPassword InitOK", "videoPasswordId: " + userGroupId, LogType.Ok, LogEntryType.Info);

            var orginalVideoPassword = _repository.Get<UserGroupVideoCatagoryCredential>().SingleOrDefault(x => x.Id == Credential.Id);

            if (orginalVideoPassword != null)
            {
                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    orginalVideoPassword.Password = Credential.Password;
                    orginalVideoPassword.Salt = Credential.Salt;
                    if (!UpdateEntity(orginalVideoPassword, "UpdatevideoPassword Error", "userGroupId: " + userGroupId, LogType.DbCreateError))
                    {
                        updateSucces = false;
                    }
                    else
                    {
                        updateSucces = true;
                    }

                    transaction.Complete();
                }
            }
            Logger.Log("UpdatevideoPassword FinalOK", "userGroupId: " + userGroupId, LogType.DbCreateOk, LogEntryType.Info);
            return updateSucces;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //-------------------------------------------------------Delete--------------------------------------------------------------------
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            /*
        [AuthorizeAccess("VideoPasswordEdit")]
        public ActionResult Delete(int id)
        {
            var model = new UserGroupVideoCatagoryCredentialModels { UserGroupId = id };
            return View(model);
        }

        //Add new Video Password for Usergroup.
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UserGroupVideoCatagoryCredentialModels model)
        {
            var culture = Session["WDCulture"].ToString();
            var gtgRights = ((MasterUserRightsModel)Session["Rights"]).UserToTemplateViewRights;

            UserGroupVideoCatagoryCredential ugvcc;
            try
            {
                if (model != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Password == null)
                        {
                            ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("VideoPasswordEmpty", culture));
                            return View();
                        }
                        else
                        {
                            ugvcc = _repository.Get<UserGroupVideoCatagoryCredential>().SingleOrDefault(x => x.Id == model.Id);
                            string HashedPassword = Hash(model.Password, GetBytes(model.Salt));
                            if (HashedPassword != ugvcc.Password)
                            {
                                ModelState.AddModelError(string.Empty, "Den indtastede kode er forkert!");
                                return View();
                            }
                            else
                            {
                                if (!DeleteEntity(ugvcc, "DeleteVideoPassword Error", "userGroupId: " + ugvcc.Id, LogType.DbCreateError))
                                {
                                    ModelState.AddModelError("", "Der eksistere ikke en kode til denne gruppe!");
                                }

                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Video kode eksistere ikke!");
                    return View();
                }
            }
            catch (Exception e)
            {
                Logger.Log("UserGroupVideoCatagoryCredintialNotFound Error", e.Message, LogType.DbQueryError, LogEntryType.Error);
            }
            return View(model);
        }
        */

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //-------------------------------------------------------Hash----------------------------------------------------------------------
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
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


        #endregion

    }
}