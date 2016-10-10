using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.WebUI.Models;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;
using WDAdmin.WebUI.Infrastructure;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class ContentController.
    /// </summary>
    [Authorize]
    public class ContentController : BaseController
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private readonly IGenericRepository _repository;
        /// <summary>
        /// The _handler
        /// </summary>
        private readonly ResourceHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public ContentController(IGenericRepository repository) : base(repository)
        {
            _repository = repository;
            _handler = ResourceHandler.GetInstance;
        }

        #region /-------------------- Category -------------------------/

        /// <summary>
        /// CategoryIndex page
        /// </summary>
        /// <returns>CategoryIndex view</returns>
        [AuthorizeAccessAttribute("Group4Page1")]
        public ActionResult CategoryIndex()
        {
            var model = new CategoryViewModel {Categories = new List<CategoryDetails>()};

            //Get list of categories
            var categories = (from cat in _repository.Get<CategoryDetails>() select cat).ToList();

            //Localize DB names
            var localizedNames = _handler.GetResources(categories.Select(x => x.Name).ToList(),Session["WDCulture"].ToString());
            foreach (var category in categories.Select(cat => new CategoryDetails {Id = cat.Id, Name = localizedNames[cat.Name]}))
            {
                model.Categories.Add(category);
            }

            if (!model.Categories.Any())
            {
                model.NoCategories = true;
            }

            return View(model);
        }

        /// <summary>
        /// CategoryAdd page
        /// </summary>
        /// <returns>CategoryAdd view</returns>
        [AuthorizeAccessAttribute("Group4Page2")]
        public ActionResult CategoryAdd()
        {
            return View();
        }

        /// <summary>
        /// CategoryAdd page
        /// </summary>
        /// <param name="model">CategoryFormModel object from view</param>
        /// <returns>CategoryIndex view, or CategoryAdd view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryAdd(CategoryFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                //Check if category with same name exists
                var categories = from cat in _repository.Get<CategoryDetails>() where cat.Name == model.CategoryName select cat;

                if (categories.Any())
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("CategoryWithSameNameExists", culture));
                    return View(model);
                }

                var category = new CategoryDetails { Name = model.CategoryName };

                //Create new category in DB
                if (!CreateEntity(category, "CategoryAdd Error", string.Empty, LogType.DbCreateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    return View(model);
                }

                return RedirectToAction("CategoryIndex");
            }

            return View(model);
        }

        /// <summary>
        /// CategoryEdit page
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>CategoryEdit view</returns>
        [AuthorizeAccessAttribute("Group4Page3")]
        public ActionResult CategoryEdit(int id)
        {
            //Get the correct category  from DB
            var culture = Session["WDCulture"].ToString();
            CategoryDetails category;
            try
            {
                category = (from cat in _repository.Get<CategoryDetails>() where cat.Id == id select cat).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("CategoryNotFoundError", culture));
                Logger.Log("CategoryEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return View();
            }

            var model = new CategoryFormModel {Id = category.Id, CategoryName = category.Name};
            return View(model);   
        }

        /// <summary>
        /// CategoryEdit page
        /// </summary>
        /// <param name="model">CategoryFormModel object from view</param>
        /// <returns>CategoryIndex view, or CategoryEdit view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(CategoryFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                //Check if category with same name exists
                var categories = from cat in _repository.Get<CategoryDetails>() where cat.Name == model.CategoryName select cat;

                if (categories.Any())
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("CategoryWithSameNameExists", culture));
                    return View(model);
                }

                //Get the correct from DB
                CategoryDetails category;
                try
                {
                    category = (from cat in _repository.Get<CategoryDetails>() where cat.Id == model.Id select cat).Single();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("CategoryNotFoundError", culture));
                    Logger.Log("CategoryEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                    return View(model);
                }

                //Update name
                category.Name = model.CategoryName;

                //Update category in DB
                if (!UpdateEntity(category, "CategoryEdit Error", string.Empty, LogType.DbUpdateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    return View(model);
                }

                return RedirectToAction("CategoryIndex");
            }

            return View(model);
        }

        #endregion

        #region /------------------------- Exercise ---------------------------/

        /// <summary>
        /// ExerciseIndex page
        /// </summary>
        /// <returns>ExerciseIndex view</returns>
        [AuthorizeAccessAttribute("Group4Page4")]
        public ActionResult ExerciseIndex()
        {
            var model = new ExerciseViewModel {Exercises = new List<ExerciseDetails>()};

            //Get list of exercises
            var exercises = (from exe in _repository.Get<ExerciseDetails>() select exe).ToList();

            //Localize DB names
            var localizedNames = _handler.GetResources(exercises.Select(x => x.Name).ToList(), Session["WDCulture"].ToString());
            foreach (var exercise in exercises.Select(exe => new ExerciseDetails { Id = exe.Id, Name = localizedNames[exe.Name] }))
            {
                model.Exercises.Add(exercise);
            }

            if (!model.Exercises.Any())
            {
                model.NoExercises = true;
            }

            return View(model);
        }

        /// <summary>
        /// ExerciseAdd
        /// </summary>
        /// <returns>ExerciseAdd view</returns>
        [AuthorizeAccessAttribute("Group4Page5")]
        public ActionResult ExerciseAdd()
        {
            var model = new ExerciseFormModel {Categories = new List<CategoryDetails>()};

            //Get available categories
            var categories = (from cat in _repository.Get<CategoryDetails>() select cat).ToList();

            //Localize DB names
            var localizedNames = _handler.GetResources(categories.Select(x => x.Name).ToList(), Session["WDCulture"].ToString());
            foreach (var category in categories.Select(cat => new CategoryDetails { Id = cat.Id, Name = localizedNames[cat.Name] }))
            {
                model.Categories.Add(category);
            }

            return View(model);
        }

        /// <summary>
        /// ExerciseAdd page
        /// </summary>
        /// <param name="model">ExerciseFormModel from view</param>
        /// <returns>ExerciseIndex view, or ExerciseAdd view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExerciseAdd(ExerciseFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                //Check if exercise with same name exists
                var exe = from ex in _repository.Get<ExerciseDetails>() where ex.Name == model.ExerciseName select ex;

                if (exe.Any())
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("ExerciseWithSameNameExists", culture));
                    model = ExerciseErrorHandler(model);
                    return View(model);
                }

                var exercise = new ExerciseDetails { Name = model.ExerciseName, CategoryId = model.CategoryId, SceneFunction = model.SceneFunction};

                //Create new Exercise in DB
                if (!CreateEntity(exercise, "ExerciseAdd Error", string.Empty, LogType.DbCreateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    model = ExerciseErrorHandler(model);
                    return View(model);
                }

                return RedirectToAction("ExerciseIndex");
            }

            model = ExerciseErrorHandler(model);
            return View(model);
        }

        /// <summary>
        /// ExerciseEdit page
        /// </summary>
        /// <param name="id">Exercise ID</param>
        /// <returns>ExerciseEdit view</returns>
        [AuthorizeAccessAttribute("Group4Page6")]
        public ActionResult ExerciseEdit(int id)
        {
            var culture = Session["WDCulture"].ToString();
            //Get the correct exercise  from DB
            ExerciseDetails exercise;
            
            try
            {
                exercise = (from ex in _repository.Get<ExerciseDetails>() where ex.Id == id select ex).Single();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("ExerciseNotFoundError", culture));
                Logger.Log("ExerciseEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                return View();
            }

            var model = new ExerciseFormModel
                                          {
                                              Id = exercise.Id,
                                              ExerciseName = exercise.Name,
                                              SceneFunction = exercise.SceneFunction,
                                              Categories = new List<CategoryDetails>(),
                                              CategoryId = exercise.CategoryId
                                          };

            var categories = (from cat in _repository.Get<CategoryDetails>() select cat).ToList();

            //Localize DB names
            var localizedNames = _handler.GetResources(categories.Select(x => x.Name).ToList(), Session["WDCulture"].ToString());
            foreach (var category in categories.Select(cat => new CategoryDetails { Id = cat.Id, Name = localizedNames[cat.Name] }))
            {
                model.Categories.Add(category);
            } 

            return View(model);   
        }

        /// <summary>
        /// ExerciseEdit page
        /// </summary>
        /// <param name="model">ExerciseFormModel object from view</param>
        /// <returns>ExerciseIndex view, or ExerciseEdit view on error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExerciseEdit(ExerciseFormModel model)
        {
            var culture = Session["WDCulture"].ToString();
            if (ModelState.IsValid)
            {
                //Check if exercise with same name exists
                var exe = from ex in _repository.Get<ExerciseDetails>() where ex.Name == model.ExerciseName && ex.Id != model.Id select ex;

                if (exe.Any())
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("ExerciseWithSameNameExists", culture));
                    model = ExerciseErrorHandler(model);
                    return View(model);
                }

                //Get the correct exercise from DB
                ExerciseDetails exercise;
                try
                {
                    exercise = (from exp in _repository.Get<ExerciseDetails>() where exp.Id == model.Id select exp).Single();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ResourceHandler.GetInstance.GetResource("ExerciseNotFoundError", culture));
                    Logger.Log("ExerciseEdit Error", ex.Message, LogType.DbQueryError, LogEntryType.Error);
                    model = ExerciseErrorHandler(model);
                    return View(model);
                }

                //Update name & category Id
                exercise.Name = model.ExerciseName;
                exercise.CategoryId = model.CategoryId;

                //Update exercise in DB
                if(!UpdateEntity(exercise, "ExerciseEdit Error", string.Empty, LogType.DbUpdateError))
                {
                    ModelState.AddModelError(string.Empty, string.Empty);
                    model = ExerciseErrorHandler(model);
                    return View(model);
                }

                return RedirectToAction("ExerciseIndex");
            }

            model = ExerciseErrorHandler(model);
            return View(model);
        }

        #endregion

        #region /----------------------- Error handlers ---------------------/

        /// <summary>
        /// Helper for Exercise Add/Edit error handling
        /// </summary>
        /// <param name="model">ExerciseFormModel object to rebuild</param>
        /// <returns>ExerciseFormModel object with rebuilded data</returns>
        private ExerciseFormModel ExerciseErrorHandler(ExerciseFormModel model)
        {
            if(model == null)
            {
                model = new ExerciseFormModel();
            }

            //Get available categories
            model.Categories = new List<CategoryDetails>();
            var categories = (from cat in _repository.Get<CategoryDetails>() select cat).ToList();

            //Localize DB names
            var localizedNames = _handler.GetResources(categories.Select(x => x.Name).ToList(), Session["WDCulture"].ToString());
            foreach (var category in categories.Select(cat => new CategoryDetails { Id = cat.Id, Name = localizedNames[cat.Name] }))
            {
                model.Categories.Add(category);
            }

            return model;
        }

        #endregion
    }
}
