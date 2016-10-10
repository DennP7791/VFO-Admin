using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Transactions;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.WebUI.Infrastructure.ModelOperators;
using WDAdmin.WebUI.Infrastructure.Various;
using WDAdmin.WebUI.Models;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;
using WDAdmin.Domain;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class UserController.
    /// </summary>
    [Authorize]
    public class UserController : BaseController
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private readonly IGenericRepository _repository;
        /// <summary>
        /// The _page structure
        /// </summary>
        private readonly PageStructureGenerator _pageStructure;
        /// <summary>
        /// The _reflector
        /// </summary>
        private readonly ModelReflector _reflector;
        /// <summary>
        /// The _master generator
        /// </summary>
        private readonly MasterRightsModelGenerator _masterGenerator;
        /// <summary>
        /// The _pass
        /// </summary>
        private readonly PassGenHash _pass;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public UserController(IGenericRepository repository) : base(repository)
        {
            _repository = repository;
            _pageStructure = PageStructureGenerator.GetInstance;
            _reflector = ModelReflector.GetInstance;
            _masterGenerator = MasterRightsModelGenerator.GetInstance;
            _masterGenerator.SetRepository(_repository);
            _masterGenerator.SetReflector(_reflector);
            _pass = PassGenHash.GetInstance;
        }

        #region /---------------------- User -----------------------------/

        /// <summary>
        /// Userpage index page
        /// </summary>
        /// <returns>UserIndex view</returns>
        [AuthorizeAccess("Group1Page1")]
        public ActionResult UserIndex()
        {
            //Build form model to get user rights
            var fmodel = BuildUserFormModel(null);
            
            //Transfer info to UserViewModel
            var model = new UserViewModel {UserGroups = fmodel.UserGroup};

            //Check for GroupId saved in TempData - if present search for users - little hack to solve problem with pagers
            if (TempData["Selected"] == null)
            {
                TempData["Selected"] = 0;
            }
            else
            {
                model.GroupId = (int)TempData["Selected"]; //Read GroupId from TempData
                model.Users = new List<User>();

                //Get users from a chosen group and put it into model
                var users = from use in _repository.Get<User>() 
                            where use.UserGroupId == model.GroupId && use.IsDeleted == false
                            select use;

                foreach (var us in users)
                {
                    var user = new User { Id = us.Id, Firstname = us.Firstname, Lastname = us.Lastname, Phone = us.Phone, Email = us.Email };
                    model.Users.Add(user);
                }

                TempData["Selected"] = model.GroupId; //Put GroupId again into TempData
            }

            return View(model);
        }

        /// <summary>
        /// Userpage index page - HttpPost after group choice
        /// </summary>
        /// <param name="model">UserViewModel object from view</param>
        /// <returns>UserIndex view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserIndex(UserViewModel model)
        {
            //Build form model to get user rights
            var fmodel = BuildUserFormModel(null);

            //Transfer info to UserViewModel
            model.UserGroups = fmodel.UserGroup;

            if (ModelState.IsValid)
            {
                //Get users from a chosen group and put it into model
                var users = from use in _repository.Get<User>() 
                            where use.UserGroupId == model.GroupId && use.IsDeleted == false 
                            select use;

                //If users found, populate User list, put GroupId into TempData - else add error state to model
                if (users.Any())
                {
                    model.Users = new List<User>();
                    model.NoUsers = false;

                    foreach (var us in users)
                    {
                        var user = new User { Id = us.Id, Firstname = us.Firstname, Lastname = us.Lastname, Phone = us.Phone, Email = us.Email, UserGroupId = us.UserGroupId};
                        model.Users.Add(user);
                    }

                    TempData["Selected"] = model.GroupId;
                }
                else
                {
                    model.NoUsers = true;
                }
            }
            
            return View(model);
        }

        /// <summary>
        /// Create user page
        /// </summary>
        /// <returns>UserAdd view</returns>
        [AuthorizeAccess("Group1Page2")]
        public ActionResult UserAdd()
        {
            var model = BuildUserFormModel(null);
            model.Username = "Username";
            model.SalaryNumber = 0;
            return View(model);
        }

        /// <summary>
        /// Create user page
        /// </summary>
        /// <param name="model">UserFormModel model from view</param>
        /// <returns>UserIndex view, or UserAdd view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAdd(UserFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                //Search DB for user with same email-address and if found show error
                var userExist = from use in _repository.Get<User>() 
                                where use.Email == model.Email && use.IsDeleted == false 
                                select use;

                if (userExist.Any())
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithSameEmailError", culture));
                    model = BuildUserFormModel(model);
                    return View(model);
                }

                //Create new User object with info from model
                var newUser = new User
                                  {
                                      Firstname = model.Firstname,
                                      Lastname = model.Lastname,
                                      Email = model.Email,
                                      Phone = model.Phone,
                                      UserGroupId = model.UserGroupId,
                                      UserTemplateId = model.UserGroupTemplateId,
                                      IsDeleted = false,
                                      CountryId = 1 //Temporary hardcode of Denmark as Country
                                  };
                
                //Generate random pass/salt for the new user
                var randomPass = _pass.CreateRandomPassword();
                var passSalt = _pass.CreateSaltedSHA512Hash(randomPass);
                newUser.Password = passSalt.Item1;
                newUser.Salt = passSalt.Item2;

                //Create user
                if (!CreateEntity(newUser, "UserCreate Error", string.Empty, LogType.DbCreateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    model = BuildUserFormModel(model);
                    return View(model);
                }

                new EmailController().EmailNewUser(model.Email, randomPass).DeliverAsync();
                return RedirectToAction("UserIndex");
            }

            /* Error handling */
            model = BuildUserFormModel(model);
            return View(model);
        }

        /// <summary>
        /// Edit user page
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>UserEdit view</returns>
        [AuthorizeAccess("Group1Page3")]
        public ActionResult UserEdit(int id)
        {
            var culture = Session["WDCulture"].ToString();
            User editedUser;
            //Get the edited user from DB and put info into model
            try
            {
                editedUser = (from use in _repository.Get<User>() where use.Id == id select use).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserNotFoundError", culture));
                Logger.Log("UserEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                var errmodel = BuildUserFormModel(null); //Build UserFormModel
                return View(errmodel);
            }

            var model = new UserFormModel {UserId = id};
            model = BuildUserFormModel(model); //Build UserFormModel

            //Put info into model
            model.Firstname = editedUser.Firstname;
            model.Lastname = editedUser.Lastname;
            model.Phone = editedUser.Phone;
            
            model.UserGroupId = editedUser.UserGroupId;
            model.UserGroupTemplateId = editedUser.UserTemplateId;
            
            if(editedUser.SalaryNumber != null) //VFO user
            {
                model.SalaryNumber = (int)editedUser.SalaryNumber;
                model.Username = editedUser.Username;
                
                if(editedUser.Email != null)
                {
                    model.Email = editedUser.Email;
                }
            }
            else //Non-VFO user
            {
                model.SalaryNumber = -1; //Set -1 as it is rather impossible for salary number to be that
                model.Username = "Username";
                model.Email = editedUser.Email;
                model.Templates.Remove(model.Templates.Last()); //Remove VFO user template to avoid changing to that
            }

            //If CountryId set use it, else set it to 1 (Denmark)
            if(editedUser.CountryId != null)
            {
                model.CountryId = (int)editedUser.CountryId;
            }
            else
            {
                model.CountryId = 1;
            }

            return View(model);
        }

        /// <summary>
        /// Edit user page
        /// </summary>
        /// <param name="model">UserFormModel object from view</param>
        /// <returns>UserIndex view, or UserEdit view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(UserFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                if(model.Email != null)
                {
                    //Search DB for user with same email-address and if found show error
                    var userExist = from use in _repository.Get<User>() 
                                    where use.Email == model.Email && use.Id != model.UserId && use.IsDeleted == false 
                                    select use;

                    if (userExist.Any())
                    {
                        ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithSameEmailError", culture));
                        model = BuildUserFormModel(model); //Build UserFormModel
                        return View(model);
                    }
                }

                if (model.SalaryNumber != -1 && model.Username != null)
                {
                    //Search DB for user with same username and if found show error
                    var userExist = from use in _repository.Get<User>() where use.Username == model.Username && use.Id != model.UserId && use.IsDeleted == false select use;

                    if (userExist.Any())
                    {
                        ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithSameUsernameError", culture));
                        model = BuildUserFormModel(model); //Build UserFormModel
                        return View(model);
                    }
                }
                
                //Create User object with info from DB and update with new data
                var userToEdit = (from dbu in _repository.Get<User>() where dbu.Id == model.UserId select dbu).Single();
                userToEdit.Firstname = model.Firstname;
                userToEdit.Lastname = model.Lastname;
                userToEdit.Email = model.Email;
                userToEdit.Username = model.SalaryNumber != -1 ? model.Username : null;
                userToEdit.SalaryNumber = model.SalaryNumber != -1 ? model.SalaryNumber : (int?)null;
                userToEdit.Phone = model.Phone;
                userToEdit.UserGroupId = model.UserGroupId;
                userToEdit.CountryId = model.CountryId;
                
                //Check if template changed fro VFO user - if so remove the salary number
                if(userToEdit.UserTemplateId != model.UserGroupTemplateId && model.SalaryNumber != -1)
                {
                    userToEdit.SalaryNumber = (int?)null;
                }

                userToEdit.UserTemplateId = model.UserGroupTemplateId;

                //Update user in DB
                if (!UpdateEntity(userToEdit, "UserEdit Error", userToEdit.Id.ToString(), LogType.DbUpdateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    model = BuildUserFormModel(model);
                    return View(model);
                }

                return RedirectToAction("UserIndex");
            }

            /* End Error handling */
            model = BuildUserFormModel(model);
            return View(model);
        }

        /// <summary>
        /// Helper method for bulding UserFormModel
        /// </summary>
        /// <param name="model">UserFormModel object to build</param>
        /// <returns>Builded UserFormModel object</returns>
        private UserFormModel BuildUserFormModel(UserFormModel model)
        {
            if (model == null) //If model is null, create one
            {
                model = new UserFormModel();
            }
            model.UserGroup = new List<UserGroup>();

            //Get current user ID from Session, get user and get the group ID for that user
            var user = (from use in _repository.Get<User>() where use.Id == (int)Session["UserID"] select use).Single();
            var userGroup = (from gr in _repository.Get<UserGroup>() where gr.Id == user.UserGroupId select gr).Single();

            //Select groups allowed to be accessed by this user
            var gtgr = ((MasterUserRightsModel)Session["Rights"]).GroupToGroupViewRights;

            //Put groups into model
            foreach (var ag in gtgr)
            {
                var ug = new UserGroup { Id = ag.Id, GroupName = ag.GroupName };
                model.UserGroup.Add(ug);
            }

            //VFO user template not allowed to create as they self-register, allow to edit
            var allAllowedTemplates = (from at in ((MasterUserRightsModel)Session["Rights"]).UserToTemplateViewRights select at).ToList();
            var allowedTemplates = new List<UserToTemplateRight>();
            
            if(model.UserId == 0) //User create situation
            {
                allowedTemplates = allAllowedTemplates.Take(allAllowedTemplates.Count() - 1).ToList();
            }
            else
            {
                allowedTemplates = allAllowedTemplates;
            }
            
            model.Templates = new List<UserTemplate>();
            foreach (var at in allowedTemplates)
            {
                var template = new UserTemplate { Id = at.Id, TemplateName = at.TemplateName, TemplateLevel = at.TemplateLevel };
                model.Templates.Add(template);
            }

            if (userGroup.CustomerId == null && userGroup.UserGroupParentId != null) //Group is Customer
            {
                model.CustomerId = userGroup.Id;
            }
            else if (userGroup.CustomerId == null && userGroup.UserGroupParentId == null) //TopAdmin situation - needs to be taken care of after group is chosen - set to -1
            {
                model.CustomerId = -1;
            }

            return model;
        }

        /// <summary>
        /// return Json(false, JsonRequestBehavior.AllowGet);
        /// Delete user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Json result of the operation</returns>
        [HttpPost]
        public JsonResult UserDelete(int id)
        {
            //Check user type to established what needs to be removed/deactivated
            //Get user from DB
            var userToDelete = new User();

            try
            {
                userToDelete = (from us in _repository.Get<User>() where us.Id == id select us).Single();
            }
            catch (Exception ex)
            {
                Logger.Log("UserDelete Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return Json(false);
            }

            //Set user's status to deleted
            userToDelete.IsDeleted = true;

            if (!UpdateEntity(userToDelete, "UserDelete Error", id.ToString(), LogType.DbUpdateError))
            {
                return Json(false);
            }

            return Json(true);
        }

        /// <summary>
        /// Recreate password for user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Json result of the operation</returns>
        [HttpPost]
        public JsonResult ResetPassword(int id)
        {
            try
            {

                //Check user type to established what needs to be removed/deactivated
                //Get user from DB
                var userResetPassword = new User();

                try
                {
                    userResetPassword = (from us in _repository.Get<User>() where us.Id == id select us).Single();
                }
                catch (Exception ex)
                {
                    Logger.Log("ResetPassword Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                    return Json(false);
                }

                //Generate new 1random pass/salt for user
                var randomPass = _pass.CreateRandomPassword();
                var passSalt = _pass.CreateSaltedSHA512Hash(randomPass);
                userResetPassword.Password = passSalt.Item1;
                userResetPassword.Salt = passSalt.Item2;

                //Update user in DB
                if (!UpdateEntity(userResetPassword, "ResetPassword Error", id.ToString(), LogType.DbUpdateError))
                {
                    return Json(false);
                }

                //Send email to user
                new EmailController().ForgottenPassEmail(userResetPassword.Email, randomPass).DeliverAsync();

                return Json(true);
            }
            catch (Exception ex)
            {
                (log4net.LogManager.GetLogger(typeof(UserController))).Fatal(ex);
                return Json(false);
            }
        }

        #endregion

        #region //------------------------ User templates ---------------------------/

        /// <summary>
        /// UserGroup templates index page
        /// </summary>
        /// <returns>UserTemplateIndex view</returns>
        [AuthorizeAccess("Group2Page4")]
        public ActionResult UserTemplateIndex()
        {
            var model = new UserTemplateViewModel {UserTemplates = new List<UserTemplate>()};

            //Get allowed user group templates from MasterModel and put them into current model
            var gttRights = ((MasterUserRightsModel)Session["Rights"]).UserToTemplateViewRights;

            foreach (var tp in gttRights)
            {
                var groupTemplate = (from ugrt in _repository.Get<UserTemplate>() where ugrt.Id == tp.Id select ugrt).Single();
                model.UserTemplates.Add(groupTemplate);
            }

            //Create tree of templates to inject into Index view
            var sb = new StringBuilder();
            sb.Append("<ul class=\"list\">");

            foreach (var template in model.UserTemplates)
            {
                if (template.ParentTemplateId == null)
                {
                    sb.Append("<div class=\"treeRoot\">");
                    sb.Append("<a href=\"" + Url.Action("UserTemplateEdit/" + template.Id) + "\" class=\"topAdder\">" + template.TemplateName + "</a>"
                                + " - " + "<a href=\"" + Url.Action("UserTemplateAdd/" + template.Id) + "\" class=\"creator\">" + @LangResources.CreateUndertemplate + "</a>");
                    sb.Append(GetChildrenTemplates(model.UserTemplates, template.Id));
                    sb.Append("</div>");
                    sb.Append("<ul class=\"children\">");
                }
            }

            sb.Append("</ul>");
            model.TemplateTree = sb.ToString();
            return View(model);
        }

        /// <summary>
        /// Helper method for getting children template nodes
        /// </summary>
        /// <param name="templates">List of UserGroupTemplate objects</param>
        /// <param name="parentId">Parent UserGroupTemplate ID</param>
        /// <returns>Formed HTML string</returns>
        private string GetChildrenTemplates(IEnumerable<UserTemplate> templates, int parentId)
        {
            var sb = new StringBuilder();

            //child categories
            var childCategories = from cc in templates where cc.ParentTemplateId == parentId select cc;

            //check if there are nay children
            if (childCategories.Any())
            {
                sb.Append("<ul>");
                foreach (var cc in childCategories)
                {
                    sb.Append("<li>");
                    sb.Append("<a href=\"" + Url.Action("UserTemplateEdit/" + cc.Id) + "\" class=\"adder\">" + cc.TemplateName + "</a>"
                                + " - " + "<a href=\"" + Url.Action("UserTemplateAdd/" + cc.Id) + "\" class=\"creator\">" + @LangResources.CreateUndertemplate + "</a>");
                    sb.Append(this.GetChildrenTemplates(templates, cc.Id)); //Call itself for children
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// User group template add page
        /// </summary>
        /// <param name="id">Template level</param>
        /// <returns>UserTemplateAdd view</returns>
        [AuthorizeAccess("Group2Page5")]
        public ActionResult UserTemplateAdd(int id)
        {
            var model = new UserTemplateFormModel { ParentTemplateId = id };

            if (id != 0)
            {
                model.TemplateLevel = 1 + (from templ in _repository.Get<UserTemplate>() where templ.Id == id select templ).Single().TemplateLevel;
            }
            else
            {
                model.TemplateLevel = 1;
            }

            return View(model);
        }

        /// <summary>
        /// User group template add page
        /// </summary>
        /// <param name="model">UserGroupTemplateFormModel object from view</param>
        /// <returns>UserTemplateIndex view, or UserTemplateAdd view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserTemplateAdd(UserTemplateFormModel model)
        {
            //If ModelState is valid - begin adding process
            if (ModelState.IsValid)
            {
                //Create a new group template in DB
                var userTemplate = new UserTemplate
                                    {
                                        TemplateName = model.TemplateName,
                                        Created = DateTime.Now.Date, //Get date of creation
                                        TemplateLevel = model.TemplateLevel,
                                        IsActive = true
                                    };
                
                if (model.ParentTemplateId != 0)
                {
                    userTemplate.ParentTemplateId = model.ParentTemplateId;
                }

                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    //Create template in DB
                    if (!CreateEntity(userTemplate, "UserTemplateAdd Error", string.Empty, LogType.DbCreateError))
                    {
                        ModelState.AddModelError(string.Empty, string.Empty);
                        return View(model);
                    }

                    //Get the bool properties from the model
                    var properties = _reflector.GetBoolPropertiesFromModel(model);

                    //Check model properties and create UserGroupRights according to values
                    foreach (var prop in properties)
                    {
                        var pageId = _pageStructure.GetPageId(prop.Item1);

                        if (prop.Item2 && pageId != -1) //Create only if page allowed and page exists
                        {
                            var tptRight = new TemplateToPageRight
                                          {
                                              UserTemplateId = userTemplate.Id,
                                              PageId = pageId, //Get pageID from PageStructure dictionary
                                              IsAllowed = prop.Item2
                                          };

                            //Create TemplateToPageRight in DB
                            if (!CreateEntity(tptRight, "UserTemplateAdd TTPRight Error", string.Empty, LogType.DbCreateError))
                            {
                                ModelState.AddModelError(string.Empty, string.Empty);
                                return View(model);
                            }
                        }
                    }

                    transaction.Complete();
                }

                _masterGenerator.GenerateMasterRightsModel(null);
                return RedirectToAction("UserTemplateIndex");
            }

            return View(model);
        }

        /// <summary>
        /// User group template edit page
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>UserTemplateEdit view</returns>
        [AuthorizeAccess("Group2Page6")]
        public ActionResult UserTemplateEdit(int id)
        {
            var model = new UserTemplateFormModel
            {
                Id = id,
                TemplateName = (from temp in _repository.Get<UserTemplate>() where temp.Id == id select temp).Single().TemplateName
            };

            //Get group template name, rights and fields for the chosen group template
            var userGroupRights = from ugr in _repository.Get<TemplateToPageRight>() where ugr.UserTemplateId == id select ugr;

            //Get bool properties from model and set the to values from DB
            var properties = _reflector.GetBoolPropertiesFromModel(model);
            model = (UserTemplateFormModel)_reflector.SetRightsPropertiesInModelDb(model, properties, userGroupRights); //Put group rights into model

            return View(model);
        }

        /// <summary>
        /// User group template edit page
        /// </summary>
        /// <param name="model">UserGroupTemplateFormModel object from view</param>
        /// <returns>UserTemplateIndex view, or UserTemplateEdit on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserTemplateEdit(UserTemplateFormModel model)
        {
            //If Model State is valid - begin Update process
            if (ModelState.IsValid)
            {
                //Get current template data from DB for changes
                UserTemplate template;
                try
                {
                    template = (from temp in _repository.Get<UserTemplate>() where temp.Id == model.Id select temp).Single();
                    template.TemplateName = model.TemplateName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    Logger.Log("UserTemplateEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                    return View(model);
                }

                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    //Update template in DB
                    if (!UpdateEntity(template, "UserTemplateEdit Error", string.Empty, LogType.DbUpdateError))
                    {
                        ModelState.AddModelError(string.Empty, string.Empty);
                        return View(model);
                    }

                    //Get existing group rights for the chosen group
                    var userGroupRights = from ugr in _repository.Get<TemplateToPageRight>()
                                          where ugr.UserTemplateId == model.Id
                                          select ugr;

                    //Get bool properties from model
                    var properties = _reflector.GetBoolPropertiesFromModel(model);

                    //Update DB according to values from model
                    foreach (var prop in properties)
                    {
                        var pageId = _pageStructure.GetPageId(prop.Item1);

                        //Find the correct group right - if not found create one
                        if (pageId != -1) //Check if page exists in page tree
                        {
                            var tptRight = (from gr in userGroupRights where gr.PageId == pageId select gr).FirstOrDefault();
                            
                            if(tptRight == null)
                            {
                                if (prop.Item2) //Create only if page right checked, else no need
                                {
                                    tptRight = new TemplateToPageRight
                                                   {
                                                       UserTemplateId = model.Id,
                                                       PageId = _pageStructure.GetPageId(prop.Item1),
                                                       IsAllowed = true
                                                   };

                                    if (!CreateEntity(tptRight, "UserTemplateEdit TTPRight Error", string.Empty, LogType.DbCreateError))
                                    {
                                        ModelState.AddModelError(string.Empty, string.Empty);
                                        return View(model);
                                    }
                                }
                            }
                            else
                            {
                                //Update only group rights which are changed
                                if (!tptRight.IsAllowed == prop.Item2)
                                {
                                    tptRight.IsAllowed = prop.Item2;

                                    if (!UpdateEntity(tptRight, "UserTemplateEdit TTPRight Error", string.Empty, LogType.DbUpdateError))
                                    {
                                        ModelState.AddModelError(string.Empty, string.Empty);
                                        return View(model);
                                    }
                                }
                            }
                        }
                    }

                    transaction.Complete();
                }

                _masterGenerator.GenerateMasterRightsModel(null);
                return RedirectToAction("UserTemplateIndex");
            }

            ModelState.AddModelError(string.Empty, string.Empty);
            return View(model);
        }

        #endregion
    }
}
