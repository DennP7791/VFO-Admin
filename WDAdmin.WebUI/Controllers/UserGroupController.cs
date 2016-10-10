using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.WebUI.Infrastructure.ModelOperators;
using WDAdmin.WebUI.Infrastructure.Various;
using WDAdmin.WebUI.Models;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;
using System.Text;
using WDAdmin.Domain;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class UserGroupController.
    /// </summary>
    [Authorize]
    public class UserGroupController : BaseController
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
        /// The _handler
        /// </summary>
        private readonly ResourceHandler _handler;
        /// <summary>
        /// The _master generator
        /// </summary>
        private readonly MasterRightsModelGenerator _masterGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public UserGroupController(IGenericRepository repository) : base(repository)
        {
            _repository = repository;
            _reflector = ModelReflector.GetInstance;
            _handler = ResourceHandler.GetInstance;
            _masterGenerator = MasterRightsModelGenerator.GetInstance;
            _masterGenerator.SetRepository(_repository);
            _masterGenerator.SetReflector(_reflector);
        }

        #region //------------------------ UserGroups / Customer ---------------------------/

        /// <summary>
        /// List of created UserGroups page
        /// </summary>
        /// <returns>UserGroupIndex view</returns>
        [AuthorizeAccess("Group2Page1")]
        public ActionResult UserGroupIndex()
        {
            var model = new UserGroupViewModel {UserGroups = new List<UserGroup>()};

            //Get allowed user groups from MasterModel and put them into current model
            var gtgRights = ((MasterUserRightsModel)Session["Rights"]).GroupToGroupViewRights;

            model.GroupHierachy = new GroupHierarchyBuilder(_repository, countUsers: true).Get(gtgRights);

            return View(model);
        }

        /// <summary>
        /// Customer creation page
        /// </summary>
        /// <returns>CustomerAdd view</returns>
        [AuthorizeAccess("Group2Page7")]
        public ActionResult CustomerAdd()
        {
            var model = new UserGroupFormModel { UserGroupParentId = (int)Session["UserGroupID"]};
            model = BuildUserGroupFormModel(model); //Build UserGroupFormModel for TopAdmin user

            //Pass information between models
            var cmodel = new CustomerFormModel
            {
                UserGroupParentId = model.UserGroupParentId,
                Countries = (from country in _repository.Get<Country>() select country).ToList(), //Add country list to model
                Categories = model.Categories,
                ExercisesChosen = new List<int>()
            };

            return View(cmodel);
        }

        /// <summary>
        /// Customer creation page
        /// </summary>
        /// <param name="model">CustomerFormModel object from view</param>
        /// <returns>UserGroupIndex view, or CustomerAdd view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerAdd(CustomerFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                //Warn that no exercises chosen
                if (model.ExercisesChosen == null)
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("NoExercisesChosen", culture));
                    model = CustomerAddErrorHandler(model);
                    return View(model);
                }

                //Create a new group in DB
                var customer = new UserGroup();
                customer.GroupName = model.GroupName;
                customer.CountryId = model.CountryId;

                if (model.UserGroupParentId != 0)
                {
                    customer.UserGroupParentId = model.UserGroupParentId;
                }

                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    //Create customer in DB
                    if (!CreateEntity(customer, "CustomerAdd Error", string.Empty, LogType.DbCreateError))
                    {
                        ModelState.AddModelError(string.Empty, string.Empty);
                        model = CustomerAddErrorHandler(model);
                        return View(model);
                    }
                    if (model.UserGroupParentId != 0)
                    {
                        var parentPath = _repository.Get<UserGroup>()
                            .Where(x => x.Id == model.UserGroupParentId)
                            .Select(x => x.Path)
                            .SingleOrDefault();
                        customer.Path = parentPath + "." + customer.Id.ToString();
                        if (!UpdateEntity(customer, "UserGroupAdd GroupAdd Error", string.Empty, LogType.DbCreateError))
                        {
                            ModelState.AddModelError(string.Empty, string.Empty);
                            model = CustomerAddErrorHandler(model);
                            return View(model);
                        }
                    }

                    //Create only if something chosen
                    if (model.ExercisesChosen != null)
                    {
                        foreach (var exe in model.ExercisesChosen)
                        {
                            var custExe = new GroupToExerciseRight {ExerciseId = exe, GroupId = customer.Id, IsChosen = true};

                            if (!CreateEntity(custExe, "CustomerAdd GroupExerciseAdd Error", string.Empty, LogType.DbCreateError))
                            {
                                ModelState.AddModelError(string.Empty, string.Empty);
                                model = CustomerAddErrorHandler(model);
                                return View(model);
                            }
                        }
                    }

                    transaction.Complete();
                }

                _masterGenerator.GenerateMasterRightsModel(null);
                return RedirectToAction("UserGroupIndex");
            }

            model = CustomerAddErrorHandler(model);
            return View(model);
        }

        /// <summary>
        /// Add user group page
        /// </summary>
        /// <param name="id">Parent UserGroup ID</param>
        /// <returns>UserGroupAdd view</returns>
        [AuthorizeAccess("Group2Page2")]
        public ActionResult UserGroupAdd(int id)
        {
            var model = new UserGroupFormModel { UserGroupParentId = id };
            model = BuildUserGroupFormModel(model); //Build UserGroupFormModel
            return View(model);
        }

        /// <summary>
        /// Add user group page
        /// </summary>
        /// <param name="model">UserGroupFormModel object from view</param>
        /// <returns>UserGroupIndex view, or UserGroupAdd view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserGroupAdd(UserGroupFormModel model)
        {
            if (ModelState.IsValid)
            {
                //Create a new group in DB
                var group = new UserGroup();
                group.GroupName = model.GroupName;
                group.CountryId = model.CountryId;
                group.CustomerId = model.CustomerId;

                if (model.UserGroupParentId != 0)
                {
                    group.UserGroupParentId = model.UserGroupParentId;
                }

                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    //Create group in DB
                    if (!CreateEntity(group, "UserGroupAdd GroupAdd Error", string.Empty, LogType.DbCreateError))
                    {
                        ModelState.AddModelError(string.Empty, string.Empty);
                        model = GroupAddErrorHandler(model);
                        return View(model);
                    }
                    else if (model.UserGroupParentId != 0)
                    {
                        var parentPath = _repository.Get<UserGroup>()
                            .Where(x => x.Id == model.UserGroupParentId)
                            .Select(x => x.Path)
                            .SingleOrDefault();
                        group.Path = parentPath + "." + group.Id.ToString();
                        if (!UpdateEntity(group, "UserGroupAdd GroupAdd Error", string.Empty, LogType.DbCreateError))
                        {
                            ModelState.AddModelError(string.Empty, string.Empty);
                            model = GroupAddErrorHandler(model);
                            return View(model);
                        }
                    }

                    //Create only if something chosen
                    if (model.ExercisesChosen != null)
                    {
                        foreach (var exe in model.ExercisesChosen)
                        {
                            var groupExe = new GroupToExerciseRight {ExerciseId = exe, GroupId = group.Id, IsChosen = true};

                            if (!CreateEntity(groupExe, "UserGroupAdd GroupExerciseAdd Error", string.Empty, LogType.DbCreateError))
                            {
                                ModelState.AddModelError(string.Empty, string.Empty);
                                model = GroupAddErrorHandler(model);
                                return View(model);
                            }
                        }
                    }

                    transaction.Complete();
                }

                _masterGenerator.GenerateMasterRightsModel(null);
                return RedirectToAction("UserGroupIndex");
            }

            model = GroupAddErrorHandler(model);
            return View(model);
        }

        /// <summary>
        /// Edit user group page
        /// </summary>
        /// <param name="id">USerGroup ID</param>
        /// <returns>UserGroupEdit view</returns>
        [AuthorizeAccess("Group2Page3")]
        public ActionResult UserGroupEdit(int id)
        {
            var culture = Session["WDCulture"].ToString();
            //Get UserGroup info from DB and set it in model
            UserGroup groupToEdit;
            try
            {
                groupToEdit = (from grp in _repository.Get<UserGroup>() where grp.Id == id select grp).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserGroupNotFound", culture));
                Logger.Log("UserGroupEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return View();
            }

            var model = new UserGroupFormModel {Id = id};

            //Resolve TopAdmin & Customer access
            if (groupToEdit.CustomerId == null && groupToEdit.UserGroupParentId == null || groupToEdit.CustomerId == null && groupToEdit.UserGroupParentId != null)
            {
                model.UserGroupParentId = groupToEdit.Id;
            }
            else
            {
                model.UserGroupParentId = (int)groupToEdit.UserGroupParentId;
            }

            model = BuildUserGroupFormModel(model); //Build UserGroupFormModel
            model.GroupName = groupToEdit.GroupName;

            //Don't allow user group edit by itself
            model.IsEditedBySelf = id == (int)Session["UserGroupID"];

            return View(model);
        }

        /// <summary>
        /// Edit user group page
        /// </summary>
        /// <param name="model">UserGroupFormModel object from view</param>
        /// <returns>UserGroupIndex view, or UserGroupEdit view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserGroupEdit(UserGroupFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                //Get UserGroup info from DB
                UserGroup updateGroup;
                try
                {
                    updateGroup = (from grp in _repository.Get<UserGroup>() where grp.Id == model.Id select grp).Single();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("UserGroupNotFound", culture));
                    Logger.Log("UserGroupEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                    return View(model);
                }

                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    updateGroup.GroupName = model.GroupName;

                    //Update the group in DB
                    if (!UpdateEntity(updateGroup, "UserGroupEdit Error", string.Empty, LogType.DbUpdateError))
                    {
                        ModelState.AddModelError(string.Empty, string.Empty);
                        model = GroupEditErrorHandler(model);
                        return View(model);
                    }

                    //If nothing was chosen, initialize list to allow update
                    if (model.ExercisesChosen == null)
                    {
                        model.ExercisesChosen = new List<int>();
                    }

                    //Get the list of all group Ids which will be affected in case an exercise is removed
                    var groupIds = new List<int>();
                    GetChildrenGroupIds(groupIds, model.Id);

                    //Get all exercises
                    var exercises = (from exe in _repository.Get<ExerciseDetails>() select exe).ToList();

                    //Get exercises associated with group
                    var assocExercises = (from ae in _repository.Get<GroupToExerciseRight>() where ae.GroupId == model.Id select ae).ToList();

                    foreach (var ex in exercises)
                    {
                        //Find exercise on associated list
                        var assocExercise = (from ae in assocExercises where ae.ExerciseId == ex.Id select ae).FirstOrDefault();

                        //If chosen exercise list contains the exercise
                        if (model.ExercisesChosen.Contains(ex.Id))
                        {
                            if (assocExercise != null) //Exercise found - see if update necessary
                            {
                                if (!assocExercise.IsChosen) //Exercise not chosen before - update
                                {
                                    assocExercise.IsChosen = true;

                                    if (!UpdateEntity(assocExercise, "UserGroupEdit GroupEx Error", string.Empty, LogType.DbUpdateError))
                                    {
                                        ModelState.AddModelError(string.Empty, string.Empty);
                                        model = GroupEditErrorHandler(model);
                                        return View(model);
                                    }
                                }
                            }
                            else //Exercise not found - create
                            {
                                var newGrExe = new GroupToExerciseRight { ExerciseId = ex.Id, GroupId = model.Id, IsChosen = true };

                                if (!CreateEntity(newGrExe, "UserGroupEdit GroupEx Error", string.Empty, LogType.DbCreateError))
                                {
                                    ModelState.AddModelError(string.Empty, string.Empty);
                                    model = GroupEditErrorHandler(model);
                                    return View(model);
                                }
                            }
                        }
                        else //Exercise not chosen
                        {
                            if (assocExercise != null) //Exercise found - see if update necessary
                            {
                                if (assocExercise.IsChosen) //Exercise chosen before - update
                                {
                                    assocExercise.IsChosen = false;

                                    if (!UpdateEntity(assocExercise, "UserGroupEdit GroupEx Error", string.Empty, LogType.DbUpdateError))
                                    {
                                        ModelState.AddModelError(string.Empty, string.Empty);
                                        model = GroupEditErrorHandler(model);
                                        return View(model);
                                    }
                                }

                                //Loop through children groups and deactivate the exercise if neccessary
                                foreach (var chg in groupIds)
                                {
                                    //Find the exercise in DB for the group
                                    var grExercise = (from ae in _repository.Get<GroupToExerciseRight>()
                                                      where ae.ExerciseId == ex.Id && ae.GroupId == chg
                                                      select ae).FirstOrDefault();
                                    
                                    if(grExercise != null)
                                    {
                                        //Exercise found - see if update necessary
                                        if (grExercise.IsChosen) //Exercise chosen before - update
                                        {
                                            grExercise.IsChosen = false;

                                            if (!UpdateEntity(grExercise, "UserGroupEdit ChGroupEx Error", string.Empty, LogType.DbUpdateError))
                                            {
                                                ModelState.AddModelError(string.Empty, string.Empty);
                                                model = GroupEditErrorHandler(model);
                                                return View(model);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    transaction.Complete();
                }

                _masterGenerator.GenerateMasterRightsModel(null);
                return RedirectToAction("UserGroupIndex");
            }

            model = GroupEditErrorHandler(model);
            return View(model);
        }
        
        #endregion

        #region /-------------------- Helpers -------------------/

        /// <summary>
        /// Helper method for building UserFormModel
        /// </summary>
        /// <param name="model">UserGroupFormModel to build/rebuild</param>
        /// <returns>Formed UserGroupFormModel object</returns>
        private UserGroupFormModel BuildUserGroupFormModel(UserGroupFormModel model)
        {
            if (model == null)
            {
                model = new UserGroupFormModel();
            }

            //Establish if group or parent group should be loaded from DB
            var groupId = model.Id != 0 ? model.Id : model.UserGroupParentId;

            //Get group
            var pgo = (from pg in _repository.Get<UserGroup>() where pg.Id == groupId select pg).Single();

            //Set CountryId
            model.CountryId = pgo.CountryId;

            //Find ClientId so the groups can inherit it - if parent does not have ClientId, it must be the Client group 
            model.CustomerId = pgo.CustomerId ?? pgo.Id;

            //If customer creation, get all available exercises, else only those group is/will be allowed to use
            var categories = (from exer in _repository.Get<CategoryDetails>() select exer).ToList();
            var exercises = new List<ExerciseDetails>();
            
            //Establish level of access for user
            if(((MasterUserRightsModel)Session["Rights"]).FullAccess)
            {
                //Get all available exercises
                exercises = (from exer in _repository.Get<ExerciseDetails>() select exer).ToList();
            }
            else
            {
                //Get only allowed exercises
                var parentAllowedExercises = (from ae in _repository.Get<GroupToExerciseRight>() where ae.GroupId == model.UserGroupParentId && ae.IsChosen select ae.ExerciseId).ToList();
                exercises.AddRange(parentAllowedExercises.Select(ae => (from exer in _repository.Get<ExerciseDetails>() where exer.Id == ae select exer).Single())); 
            }

            var groupAllowedExe = (from ae in _repository.Get<GroupToExerciseRight>() where ae.GroupId == groupId && ae.IsChosen select ae.ExerciseId).ToList();
            if (model.ExercisesChosen == null)
            {
                model.ExercisesChosen = new List<int>();
                model.ExercisesChosen.AddRange(groupAllowedExe);
            }

            //Generate dictionary with localized category/exercise names
            var categoryNamesResx = _handler.GetResources(categories.Select(cat => cat.Name).ToList(), Session["WDCulture"].ToString());
            var exerciseNamesResx = _handler.GetResources(exercises.Select(cat => cat.Name).ToList(), Session["WDCulture"].ToString());

            model.Categories = new List<CategoryDetails>();

            //Loop through categories and pick exercises which belong to them
            foreach (var cat in categories)
            {
                var catExercises = (from exer in exercises where exer.CategoryId == cat.Id select exer).ToList();
                var catDetail = new CategoryDetails { Id = cat.Id, Name = categoryNamesResx[cat.Name], Exercises = new List<ExerciseDetails>() };

                foreach (var exe in catExercises)
                {
                    var exeDetail = new ExerciseDetails
                    {
                        Id = exe.Id,
                        CategoryId = cat.Id,
                        SceneFunction = exe.SceneFunction,
                        Name = exerciseNamesResx[exe.Name]
                    };
                    catDetail.Exercises.Add(exeDetail);
                }

                //Add a category only if any exercises present
                if(catDetail.Exercises.Any())
                {
                    model.Categories.Add(catDetail);
                }
            }
            
            return model;
        }

        /// <summary>
        /// Helper for handling CustomerAdd errors
        /// </summary>
        /// <param name="model">CustomerFormModel object to rebuild</param>
        /// <returns>Builded CustomerFormModel object</returns>
        private CustomerFormModel CustomerAddErrorHandler(CustomerFormModel model)
        {
            if (model == null)
            {
                model = new CustomerFormModel();
            }

            var ufModel = new UserGroupFormModel { UserGroupParentId = (int)Session["UserGroupID"]};
            ufModel = BuildUserGroupFormModel(ufModel);

            if (model.ExercisesChosen == null)
            {
                model.ExercisesChosen = new List<int>();
            }

            model.Countries = (from country in _repository.Get<Country>() select country).ToList(); //Add country list to model
            model.Categories = ufModel.Categories;

            return model;
        }

        /// <summary>
        /// Helper for handling GroupAdd errors
        /// </summary>
        /// <param name="model">UserGroupFormModel object to rebuild</param>
        /// <returns>Builded UserGroupFormModel object</returns>
        private UserGroupFormModel GroupAddErrorHandler(UserGroupFormModel model)
        {
            if (model == null)
            {
                model = new UserGroupFormModel();
            }

            model = BuildUserGroupFormModel(model);

            if (model.ExercisesChosen == null)
            {
                model.ExercisesChosen = new List<int>();
            }

            return model;
        }

        /// <summary>
        /// Helper for getting children group Ids
        /// </summary>
        /// <param name="groupIds">List of UserGroup IDs</param>
        /// <param name="parentId">Parent USerGroup ID</param>
        private void GetChildrenGroupIds(ICollection<int> groupIds, int parentId)
        {
            //child groups
            var childGroups = from cg in _repository.Get<UserGroup>() where cg.UserGroupParentId == parentId select cg;

            //check if there are nay children
            if (childGroups.Any())
            {
                foreach (var cg in childGroups)
                {
                    groupIds.Add(cg.Id);

                    //call itself for children
                    this.GetChildrenGroupIds(groupIds, cg.Id);
                }
            }
        }

        /// <summary>
        /// Helper for handling GroupEdit errors
        /// </summary>
        /// <param name="model">UserGroupFormModel object to rebuild</param>
        /// <returns>Builded UserGroupFormModel object</returns>
        private UserGroupFormModel GroupEditErrorHandler(UserGroupFormModel model)
        {
            if (model == null)
            {
                model = new UserGroupFormModel();
            }

            model = BuildUserGroupFormModel(model);

            if (model.ExercisesChosen == null)
            {
                model.ExercisesChosen = new List<int>();
            }

            return model;
        }

        #endregion
    }
}