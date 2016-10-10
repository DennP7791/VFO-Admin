using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WDAdmin.WebUI.Infrastructure.ModelOperators;
using WDAdmin.WebUI.Models;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.Resources;
using System.Configuration;
using WDAdmin.WebUI.Infrastructure.Various;
using System.Collections.Generic;
using WDAdmin.Domain;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class AccountController.
    /// </summary>
    public class AccountController : BaseController
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private readonly IGenericRepository _repository;
        /// <summary>
        /// The _reflector
        /// </summary>
        private readonly ModelReflector _reflector;
        /// <summary>
        /// The _pass
        /// </summary>
        private readonly PassGenHash _pass;
        /// <summary>
        /// The _handler
        /// </summary>
        private readonly ResourceHandler _handler;
        /// <summary>
        /// The _master generator
        /// </summary>
        private readonly MasterRightsModelGenerator _masterGenerator;
        /// <summary>
        /// The _login help file
        /// </summary>
        private readonly string _loginHelpFile = ConfigurationManager.AppSettings["LoginHelpFile"];
        /// <summary>
        /// The _self create help file
        /// </summary>
        private readonly string _selfCreateHelpFile = ConfigurationManager.AppSettings["SelfCreateHelpFile"];

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public AccountController(IGenericRepository repository) : base(repository)
        {
            _repository = repository;
            _pass = PassGenHash.GetInstance;
            _reflector = ModelReflector.GetInstance;
            _handler = ResourceHandler.GetInstance;
            _masterGenerator = MasterRightsModelGenerator.GetInstance;
            _masterGenerator.SetRepository(_repository);
            _masterGenerator.SetReflector(_reflector);
        }

        #region //----------------- LogOn - LogOff - Error - SessionExpired -------------------------// 

        /// <summary>
        /// LogOn page
        /// </summary>
        /// <returns>LogoOn view</returns>
        public ActionResult LogOn()
        {
            //Reset Session variables to avoid showing menu elements if user not logged out properly before
            Session["Rights"] = null;
            Session["User"] = null;
            Session["UserID"] = null;
            
            CheckAndChangeCulture(string.Empty); //Check culture before showing login screen

            Session["Help"] = _loginHelpFile;
            
            return View();
        }

        /// <summary>
        /// LogOn page
        /// </summary>
        /// <param name="model">LogOnModel object from view</param>
        /// <param name="returnUrl">Return url when login successfull</param>
        /// <returns>Home/Index (or returnUrl), or LogOn view on error</returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// </exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var culture = Session["WDCulture"].ToString();
                User user;

                try
                {
                    //Get user from DB
                    user = (from use in _repository.Get<User>()
                            where use.Email == model.Username && use.IsDeleted == false || use.Username == model.Username && use.IsDeleted == false
                            select use).Single();

                    if (user != null) //User found
                    {
                        //Generate salted pass if user has pass and salt
                        string saltedPass = user.Salt;
                        if (!string.IsNullOrEmpty(saltedPass))
                        {
                            saltedPass = _pass.CheckSaltedPass(model.Password, user.Salt);
                        }

                        if (user.Id == 0 || string.IsNullOrEmpty(saltedPass) && !user.Password.Equals(_pass.CreateSHA512Hash(model.Password)) || !string.IsNullOrEmpty(saltedPass) && saltedPass != user.Password)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    }
                    else //User not found
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Logger.Log("InvalidCredentials", model.Username, LogType.LoginError, LogEntryType.Warning);
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("InvalidUsernamePasswordError", culture));
                    return View(model);
                }
                catch (Exception ex)
                {
                    Logger.Log("Login Error", ex.Message, model.Username, LogType.LoginError, LogEntryType.Error);
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("InternalError", culture));
                    return View(model);
                }

                //Generate master rights model
                _masterGenerator.GenerateMasterRightsModel(user.Id);

                //Put UserID/UserGroupID/UserGroupTemplateID and user name in Session
                Session["User"] = user.Firstname + " " + user.Lastname;
                Session["UserID"] = user.Id;
                Session["UserGroupID"] = user.UserGroupId;
                Session["UserTemplateID"] = user.UserTemplateId;

                //Check if user has country specified, if so check session/cookie and correct is neccessary
                if(user.CountryId != null)
                {
                    var country = new Country();

                    try
                    {
                        country = (from ct in _repository.Get<Country>() where ct.Id == user.CountryId select ct).Single();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("PersonalDetails GetCountry Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                        return View();
                    }
                
                    if(Session["WDCulture"] == null || Session["WDCulture"].ToString() != country.CultureCode)
                    {
                        CheckAndChangeCulture(country.CultureCode);
                    }
                }

                //Get info about user's browser
                var brObject = Request.Browser;
                var browser = "Browser Type: " + brObject.Type + ", " + "Browser Version: " + brObject.Version;

                //Log successfull login + browser info
                Logger.Log("LoginOK", model.Username, browser, LogType.LoginOk, LogEntryType.Info);

                //Authenticate and redirect to main page
                FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                return Redirect(returnUrl ?? Url.Action("Index", "Home"));
            }

            return View(model);
        }

        /// <summary>
        /// LogOff action - clean cookie/session/MasterUserRightsModel and redirect to LogOn page
        /// </summary>
        /// <returns>LogOff view</returns>
        public ActionResult LogOff()
        {
            //Perform log off functions
            LogOffFunctions();

            //Redirect to LogOn screen
            return RedirectToAction("LogOn", "Account");
        }

        /// <summary>
        /// General error page
        /// </summary>
        /// <returns>Error view</returns>
        public ActionResult Error()
        {
            //Perform log off functions
            LogOffFunctions();

            return View();
        }

        /// <summary>
        /// Session expired page
        /// </summary>
        /// <returns>SessionExpired view</returns>
        public ActionResult SessionExpired()
        {
            //Perform log off functions
            LogOffFunctions();
            
            return View();
        }

        #endregion

        #region //-------------- VFO user self-registration ----------------//

        /// <summary>
        /// Page for self-registration process of citizens with reference
        /// </summary>
        /// <returns>Register view</returns>
        public ActionResult Register()
        {
            Session["Help"] = _selfCreateHelpFile;

            if (Session["WDCulture"] == null)
            {
                Session.Add("WDCulture", WDAdmin.WebUI.AppSettings.DefaultCulture.Name);
            }

            var culture = Session["WDCulture"].ToString();
            var model = new RegisterModel();

            //Get reference parameter from URL
            var reference = (string)Url.RequestContext.RouteData.Values["id"];

            //Check if reference number present - if not show error and block fields
            if (!string.IsNullOrEmpty(reference))
            {
                //Split & parse reference parts
                var referenceParts = reference.Split('-');
                var refCId = int.Parse(referenceParts[0]);
                var refGId = int.Parse(referenceParts[1]);

                //Get reference info about the customer and check if group and user exist
                var customer = (from cust in _repository.Get<UserGroup>() where cust.Id == refCId select cust).FirstOrDefault();
                var group = (from gr in _repository.Get<UserGroup>() where gr.Id == refGId select gr).FirstOrDefault();

                if(customer == null || group == null)
                {
                    Logger.Log("Register module Error", "One of elements is null", LogType.DbQueryError, LogEntryType.Error);
                    model.Reference = string.Empty;
                    model.UserGroupId = 0;
                    ViewBag.BlockFields = true;
                    ViewBag.IsVisible = false;
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("WrongReferenceError ", culture)+ " " + ResourceHandler.GetInstance.GetResource("ContactVFOResponsible", culture));
                    return View(model);
                }

                //Put reference numbers and info from DB into ViewBag for use in View
                model.Reference = customer.GroupName;
                model.UserGroupId = refGId;
                ViewBag.IsVisible = false;
                ViewBag.BlockFields = false;
            }
            else
            {
                model.Reference = string.Empty;
                model.UserGroupId = 0;
                ViewBag.BlockFields = true;
                ViewBag.IsVisible = false;
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("NoReferenceError", culture));
                return View(model);
            }

            return View(model);
        }

        /// <summary>
        /// Page for self-registration process of citizens with reference - HttpPost
        /// </summary>
        /// <param name="model">RegisterModel object from view</param>
        /// <returns>Register view with the result of operation</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var culture = Session["WDCulture"].ToString();
                //Search DB for user with same username and if found show error
                var usernameExist = from use in _repository.Get<User>() 
                                    where use.IsDeleted == false
                                    where use.Username == model.Username || use.Email == model.Username
                                    select use;

                if (usernameExist.Any())
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithSameUsernameError", culture));
                    ViewBag.IsVisible = false;
                    ViewBag.BlockFields = false;
                    return View(model);
                }

                //If email present, Search DB for user with same email and if found show error
                if(!string.IsNullOrEmpty(model.Email))
                {
                    var emailExist = from use in _repository.Get<User>()
                                    where use.Email == model.Email && use.IsDeleted == false
                                    select use;

                    if (emailExist.Any())
                    {
                        ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithSameEmailError", culture));
                        ViewBag.IsVisible = false;
                        ViewBag.BlockFields = false;
                        return View(model);
                    }
                }
                
                var user = new User
                {
                    SalaryNumber = model.SalaryNumber,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Email = string.IsNullOrEmpty(model.Email) ? string.Empty : model.Email,
                    Username = model.Username,
                    IsDeleted = false,
                    UserTemplateId = (from ut in _repository.Get<UserTemplate>() select ut).OrderByDescending(x => x.TemplateLevel).First().Id,
                    CountryId = 1, //Temporary save of Denmark as default country
                    UserGroupId = model.UserGroupId
                };

                //Hash and salt the given password
                var hashPassSalt = _pass.CreateSaltedSHA512Hash(model.Password);
                user.Password = hashPassSalt.Item1;
                user.Salt = hashPassSalt.Item2;

                if (!CreateEntity(user, "Register User Error", model.Username, LogType.DbCreateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    ViewBag.IsVisible = false;
                    ViewBag.BlockFields = false;
                    return View(model);
                }

                Logger.Log("Register OK", model.Username, LogType.DbCreateOk, LogEntryType.Info);
                ViewBag.IsVisible = true;
                ViewBag.BlockFields = true;
                return View(model);
            }

            ViewBag.IsVisible = false;
            ViewBag.BlockFields = false;
            return View(model);
        }

        /// <summary>
        /// Partial view for successfull citizen self-registration
        /// </summary>
        /// <returns>CitizenCreateSuccess partial view</returns>
        public ActionResult CitizenCreateSuccess()
        {
            return PartialView();
        }

        #endregion

        #region //------------------- PersonalDetails - Password -------------------------//

        /// <summary>
        /// Personal details page
        /// </summary>
        /// <returns>PersonalDetails view</returns>
        [AuthorizeAccess("CustomPageAuthorize")]
        public ActionResult PersonalDetails()
        {
            var culture = Session["WDCulture"].ToString();
            //Get UserId from Session
            var id = (int)Session["UserID"];

            User viewedUser;

            //Get the viewed user from DB and put info into model
            try
            {
                viewedUser = (from use in _repository.Get<User>() where use.Id == id select use).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserNotFoundError", culture));
                Logger.Log("PersonalDetails GetUser Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return View();
            }

            var model = new UserPersonalViewModel();

            if (viewedUser.SalaryNumber != null)
            {
                model.SalaryNumber = (int)viewedUser.SalaryNumber;
            }
            else
            {
                model.SalaryNumber = 0;
            }

            model.Firstname = viewedUser.Firstname;
            model.Lastname = viewedUser.Lastname;
            model.Phone = viewedUser.Phone;

            //Show username if present
            if (!string.IsNullOrEmpty(viewedUser.Username))
            {
                model.Username = viewedUser.Username;
            }
            
            //Show email if present
            if (!string.IsNullOrEmpty(viewedUser.Email))
            {
                model.Email = viewedUser.Email;
            }
            
            //Resolve language
            if (viewedUser.CountryId != null)
            {
                var country = new Country();

                try
                {
                    country = (from ct in _repository.Get<Country>() where ct.Id == viewedUser.CountryId select ct).Single();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserNotFoundError", culture));
                    Logger.Log("PersonalDetails GetCountry Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                    return View();
                }

                model.Language = _handler.GetResource(country.Language, Session["WDCulture"].ToString());
            }

            return View(model);
        }

        /// <summary>
        /// Personal details page - HttpPost
        /// </summary>
        /// <param name="model">UserPersonalViewModel object from view</param>
        /// <returns>Redirects to ChangePassword action</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PersonalDetails(UserPersonalViewModel model)
        {
            return RedirectToAction("ChangePassword", "Account"); //Redirect to ChangePassword method
        }

        /// <summary>
        /// Change password page
        /// </summary>
        /// <returns>ChangePasswor view</returns>
        [AuthorizeAccess("CustomPageAuthorize")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Change password page
        /// </summary>
        /// <param name="model">ChangePasswordModel obkect from view</param>
        /// <returns>ChangePasswordSuccess view, or ChangePassword view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var culture = Session["WDCulture"].ToString();
                //Get user Id from Session and get user from DB
                User user;
                var id = (int)Session["UserID"];
                
                try
                {
                    user = (from use in _repository.Get<User>() where use.Id == id select use).Single();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithMailNotFoundError", culture));
                    Logger.Log("ChangePassword Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                    return View(model);
                }

                //Encode the given password and create salt
                var passSalt = _pass.CreateSaltedSHA512Hash(model.NewPassword);
                user.Password = passSalt.Item1;
                user.Salt = passSalt.Item2;

                //Update user in DB
                if (!UpdateEntity(user, "ChangePassword Error", string.Empty, LogType.DbUpdateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    return View(model);
                }

                Logger.Log("ChangePassword OK", LogType.PasswordChangeOk, LogEntryType.Info);
                Session["UserID"] = null;
                return RedirectToAction("ChangePasswordSuccess");
            }

            return View(model);
        }

        /// <summary>
        /// Successfull password change page
        /// </summary>
        /// <returns>ChangePasswordSuccess view</returns>
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        /// <summary>
        /// Forgotten password page
        /// </summary>
        /// <returns>ForgottenPassword view</returns>
        public ActionResult ForgottenPassword()
        {
            return View();
        }

        /// <summary>
        /// Forgotten password page
        /// </summary>
        /// <param name="model">ForgottenPasswordModel object from view</param>
        /// <returns>Different views depending on user role</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgottenPassword(ForgottenPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var culture = Session["WDCulture"].ToString();
                //Get user with the given email address
                User user;
                
                //Check if e-mail or username
                var match = Regex.Match(model.UserIdent, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);

                if(match.Success) //User identified via e-mail
                {
                    try
                    {
                        user = (from use in _repository.Get<User>() where use.Email == model.UserIdent && use.IsDeleted == false select use).Single();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithMailNotFoundError", culture));
                        Logger.Log("ForgottenPassword E-mail Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                        return View(model);
                    }
                }
                else //User identified via username
                {
                    try
                    {
                        user = (from use in _repository.Get<User>() where use.Username == model.UserIdent && use.IsDeleted == false select use).Single();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithUsernameNotFoundError", culture));
                        Logger.Log("ForgottenPassword Username Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                        return View(model);
                    }
                }

                var randomPass = _pass.CreateRandomPassword();
                var passSalt = _pass.CreateSaltedSHA512Hash(randomPass);
                user.Password = passSalt.Item1;
                user.Salt = passSalt.Item2;

                using(var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    if (!UpdateEntity(user, "ForgottenPassword Error", user.Id.ToString(), LogType.DbUpdateError))
                    {
                        ModelState.AddModelError(string.Empty, string.Empty);
                        return View(model);
                    }

                    if (match.Success) //User identified via e-mail, complete transaction and send e-mail
                    {
                        transaction.Complete();
                        
                        //Send e-mail with new pass to user
                        new EmailController().ForgottenPassEmail(model.UserIdent, randomPass).DeliverAsync();
                        return RedirectToAction("EmailPasswordRecreateSuccess");
                    }
                    else //User identified via username
                    {
                        //Find the responsible for user
                        var responsible = (from utr in _repository.Get<ResponsibleToUserGroup>()
                                           where utr.UserGroupId == user.UserGroupId
                                           join us in _repository.Get<User>() on utr.ResponsibleUserId equals us.Id
                                           where us.IsDeleted == false
                                           select us).FirstOrDefault();

                        var userGroup = (from ug in _repository.Get<UserGroup>()
                                         where ug.Id == user.UserGroupId
                                         select ug).FirstOrDefault();

                        if (responsible != null) //Responsible found, complete transaction and send e-mail
                        {
                            transaction.Complete();

                            //Send e-mail with new pass to user
                            new EmailController().ForgottenPassEmailResponsible(responsible.Email, user, userGroup.GroupName, randomPass).DeliverAsync();

                            TempData["Responsible"] = responsible.Firstname + " " + responsible.Lastname;
                            return RedirectToAction("UsernamePasswordRecreateSuccess");

                        }
                        else //Responsible not found, redirect to page where user can choose the responsible
                        {
                            return RedirectToAction("ChooseResponsibleUser", new { userId = user.Id, groupId = userGroup.Id });
                        }
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// Allow user who forgot password to choose person who will receive e-mail with their new password
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="groupId">UserGroup ID</param>
        /// <returns>ChooseResponsibleUser view</returns>
        public ActionResult ChooseResponsibleUser(int userId, int groupId)
        {
            var model = new ForgottenPasswordModel { ResponsibleUsers = new List<GroupsResponsible>() };

            //Try to find if group is a customer or group under customer
            var userGroup = (from ug in _repository.Get<UserGroup>() where ug.Id == groupId select ug).FirstOrDefault();

            var effectiveGroupId = 0;

            if(userGroup.CustomerId == null) //Group is a customer
            {
                effectiveGroupId = groupId;
            }
            else //Child group to customer
            {
                effectiveGroupId = (int)userGroup.CustomerId;
            }

            //Choose first administrative user from the group
            var firstUser = (from fu in _repository.Get<User>()
                             where fu.UserGroupId == effectiveGroupId && fu.IsDeleted == false && fu.SalaryNumber == null
                             select fu).First();

            //Generate master rights model for the user
            _masterGenerator.GenerateMasterRightsModel(firstUser.Id);

            //Get user group's responsible - return 0 if not present
            var responsibleId = (from rtg in _repository.Get<ResponsibleToUserGroup>() where rtg.UserGroupId == groupId select rtg.ResponsibleUserId).FirstOrDefault();
            var responsibleUsers = new List<GroupsResponsible>();
            
            //Get allowed user groups from MasterModel
            var gtgRights = ((MasterUserRightsModel)Session["Rights"]).GroupToGroupViewRights;

            if (gtgRights.Any(x => x.Id == groupId))
            {
                GetGroupsResponsibleUsers(responsibleUsers, groupId, responsibleId);
            }

            model.UserId = userId;
            model.UserGroupId = groupId;
            model.ResponsibleUsers = responsibleUsers;

            return View(model);
        }

        /// <summary>
        /// Choose user who reveives e-mail - HttpPost
        /// </summary>
        /// <param name="model">ForgottenPasswordModel object from view</param>
        /// <returns>ResponsiblePasswordRecreateSuccess view, or ChooseResponsibleUser on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseResponsibleUser(ForgottenPasswordModel model)
        {
            //Find teh user who needs password change
            User user;

            var culture = Session["WDCulture"].ToString();
            try
            {
                user = (from use in _repository.Get<User>() where use.Id == model.UserId && use.IsDeleted == false select use).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithMailNotFoundError", culture));
                Logger.Log("ChooseResponsibleUser User Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return View(model);
            }

            //Find the responsible
            User responsible;

            try
            {
                responsible = (from use in _repository.Get<User>() where use.Id == model.ChosenResponsibleId && use.IsDeleted == false select use).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserWithMailNotFoundError", culture));
                Logger.Log("ChooseResponsibleUser Responsible Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return View(model);
            }

            //Generate password
            var randomPass = _pass.CreateRandomPassword();
            var passSalt = _pass.CreateSaltedSHA512Hash(randomPass);
            user.Password = passSalt.Item1;
            user.Salt = passSalt.Item2;

            if (!UpdateEntity(user, "ChooseResponsibleUser Error", user.Id.ToString(), LogType.DbUpdateError))
            {
                ModelState.AddModelError(string.Empty, string.Empty);
                return View(model);
            }

            //Get user's group
            var userGroup = (from ug in _repository.Get<UserGroup>() where ug.Id == user.UserGroupId select ug).FirstOrDefault();

            //Send email to responsible
            new EmailController().ForgottenPassEmailResponsible(responsible.Email, user, userGroup.GroupName, randomPass).DeliverAsync();

            //Reset Session rights
            Session["Rights"] = null;
            
            TempData["Responsible"] = responsible.Firstname + " " + responsible.Lastname;
            return RedirectToAction("ResponsiblePasswordRecreateSuccess");
        }

        /// <summary>
        /// Successfull password recreation page for users with email
        /// </summary>
        /// <returns>EmailPasswordRecreateSuccess view</returns>
        public ActionResult EmailPasswordRecreateSuccess()
        {
            return View();
        }

        /// <summary>
        /// Successfull password recreation page for users without email
        /// </summary>
        /// <returns>UsernamePasswordRecreateSuccess view</returns>
        public ActionResult UsernamePasswordRecreateSuccess()
        {
            return View();
        }

        /// <summary>
        /// Successfull password recreation page for users who chose the responsible
        /// </summary>
        /// <returns>ResponsiblePasswordRecreateSuccess view</returns>
        public ActionResult ResponsiblePasswordRecreateSuccess()
        {
            return View();
        }

        #endregion

        #region Other

        /// <summary>
        /// Checking culture cookie and changing culture
        /// </summary>
        /// <param name="culture">Culture, if given</param>
        [HttpPost]
        public void CheckAndChangeCulture(string culture)
        {   
            //Check if correct culture string passed, change to default if error or empty
            if (!string.IsNullOrEmpty(culture))
            {
                if (!CultureHelper.IsValidCultureName(culture))
                {
                    culture = DefaultCulture;
                    Logger.Log("Invalid culture", culture, LogType.InvalidCulture, LogEntryType.Warning);
                }
            }

            if (HttpContext.Request.Cookies["WDCulture"] != null) //Check if cookie exists
            {
                if (string.IsNullOrEmpty(culture)) //No culture specified, so just check cookies/change session
                {
                    Session["WDCulture"] = HttpContext.Request.Cookies["WDCulture"].Value;
                }
                else //Culture specified, change session, change cookie if cookie value different
                {
                    if (HttpContext.Request.Cookies["WDCulture"].Value != culture)
                    {
                        Session["WDCulture"] = culture;
                        Response.Cookies["WDCulture"].Value = culture;
                        Response.Cookies["WDCulture"].Expires = DateTime.Now.AddDays(365);
                    }
                }
            }
            else //Cookie does not exist, create one and change session
            {
                var cultureCookie = new HttpCookie("WDCulture") { Expires = DateTime.Now.AddDays(365) };

                if (string.IsNullOrEmpty(culture)) //No culture specified, create cookie with default culture
                {
                    Session["WDCulture"] = DefaultCulture;
                    cultureCookie.Value = DefaultCulture;
                }
                else //Culture specified, create cookie with given culture
                {
                    Session["WDCulture"] = culture;
                    cultureCookie.Value = culture;
                }

                Response.Cookies.Add(cultureCookie);
            }
        }

        /// <summary>
        /// Performing log off functions
        /// </summary>
        private void LogOffFunctions()
        {
            //Prevent caching the page so users cannot use the back button when they log-out
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            //Clear Session variables and abandon
            Session.Clear();
            Session.Abandon();
            
            //Clean auth cookie 
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty) { Expires = DateTime.Now.AddDays(-1) };
            HttpContext.Response.Cookies.Add(authCookie);

            //Clean session cookie 
            var sessionCookie = Request.Cookies["ASP.NET_SessionId"];
            sessionCookie.Value = string.Empty;
            sessionCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Set(sessionCookie);

            CheckAndChangeCulture(string.Empty);

            FormsAuthentication.SignOut(); //Signout
        }

        #endregion
    }
}