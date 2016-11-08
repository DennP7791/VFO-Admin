using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.WebUI.Infrastructure.ModelOperators;

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
        [AuthorizeAccess("VideoPassword")]
        public ActionResult Index(int id)
        {
            var culture = Session["WDCulture"].ToString();

            UserGroupVideoCatagoryCredential ugvcc;
            try
            {
                ugvcc = (from uvc in _repository.Get<UserGroupVideoCatagoryCredential>() where uvc.UserGroupId == id select uvc).Single();
            }
            catch (Exception e)
            {
                Logger.Log("UserGroupVideoCatagoryCredintialNotFound Error", e.Message, LogType.DbQueryError, LogEntryType.Error);

            }
            return View();
        }
        #endregion
    }
}