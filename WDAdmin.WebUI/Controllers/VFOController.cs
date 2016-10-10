using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.WebUI.Models;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;
using WDAdmin.WebUI.Infrastructure;
using System.Runtime.Serialization.Json;
using System;
using System.Web.Script.Serialization;
using System.Data.Linq.SqlClient;
using WDAdmin.WebUI.Infrastructure.Various;

namespace WDAdmin.WebUI.Controllers
{

    /// <summary>
    /// Class VFOController.
    /// </summary>
    public class VFOController : BaseController
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
        /// Initializes a new instance of the <see cref="VFOController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public VFOController(IGenericRepository repository)
            : base(repository)
        {
            _repository = repository;
            _handler = ResourceHandler.GetInstance;
        }

        /// <summary>
        /// Create auto-register link page
        /// </summary>
        /// <returns>CreateLink view</returns>
        [AuthorizeAccess("Group3Page1")]
        public ActionResult CreateLink()
        {
            //Establish CustomerId
            var userGroup = (from gr in _repository.Get<UserGroup>() where gr.Id == (int)Session["UserGroupID"] select gr).Single();
            int customerId;

            //Resolve TopAdmin & Customer issue
            if (userGroup.CustomerId == null)
            {
                customerId = userGroup.Id;
            }
            else
            {
                customerId = (int)userGroup.CustomerId;
            }

            var model = new LinkViewModel { UserGroups = new List<UserGroup>(), CustomerId = customerId }; //Set customer ID into model

            //Get allowed user groups from MasterModel and put them into current model
            var gtgRights = ((MasterUserRightsModel)Session["Rights"]).GroupToGroupViewRights;

            model.GroupHierachy = new GroupHierarchyBuilder(_repository, countUsers: false).Get(gtgRights);

            model.NoGroups = !model.UserGroups.Any();
            ViewBag.LinkBase = Url.Action("Register", "Account", routeValues: null, protocol: Request.Url.Scheme);

            return View(model);
        }

        /// <summary>
        /// Get list of responsible users from the chosen group
        /// </summary>
        /// <param name="id">UserGroup ID</param>
        /// <returns>Json string with a list of users</returns>
        [HttpPost]
        [AuthorizeAccess("Group3Page1")]
        public ActionResult GetResponsibleUsers(int id)
        {
            //Get user group's responsible - return 0 if not present
            var responsibleId = (from rtg in _repository.Get<ResponsibleToUserGroup>() where rtg.UserGroupId == id select rtg.ResponsibleUserId).FirstOrDefault();

            var responsibleUsers = new List<GroupsResponsible>();

            //Get allowed user groups from MasterModel
            var gtgRights = ((MasterUserRightsModel)Session["Rights"]).GroupToGroupViewRights;

            if (gtgRights.Any(x => x.Id == id))
            {
                GetGroupsResponsibleUsers(responsibleUsers, id, responsibleId);
            }

            return Json(responsibleUsers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Set a responsible user for the given group
        /// </summary>
        /// <param name="groupId">UserGroup ID</param>
        /// <param name="responsibleId">User ID - chosen to be the responsible</param>
        /// <returns>Result of the operation (true/false)</returns>
        [HttpPost]
        [AuthorizeAccess("Group3Page1")]
        public ActionResult SetResponsibleForGroup(int groupId, int responsibleId)
        {
            //Get user group's responsible - return 0 if not present
            var responsibleToUserGroup = (from rtg in _repository.Get<ResponsibleToUserGroup>() where rtg.UserGroupId == groupId select rtg).FirstOrDefault();

            if (responsibleToUserGroup == null) //No responsible before - create
            {
                responsibleToUserGroup = new ResponsibleToUserGroup { UserGroupId = groupId, ResponsibleUserId = responsibleId };

                if (!CreateEntity(responsibleToUserGroup, "Register RUG Create Error", "GroupId: " + groupId, LogType.DbCreateError))
                {
                    return Json(false);
                }
            }
            else //Update responsible
            {
                responsibleToUserGroup.ResponsibleUserId = responsibleId;

                if (!UpdateEntity(responsibleToUserGroup, "Register RUG Create Error", "GroupId: " + groupId, LogType.DbUpdateError))
                {
                    return Json(false);
                }
            }

            return Json(true);
        }

        /// <summary>
        /// Combined percent of exercises passed and average score for all users associated with the current group.
        /// As well as for all groups and users directly under the current group.
        /// </summary>
        /// <param name="id">Id for the current group. Will default to the group the logged in user belongs to.</param>
        /// <returns>Group results view</returns>
        [AuthorizeAccess("Group3Page2")]
        public ActionResult Groups(int? id = null)
        {
            int groupId = id ?? (int)Session["UserGroupID"];
            var culture = Session["WDCulture"].ToString();

            // Fetch name and path for the selected group
            var current = _repository.Get<UserGroup>()
                .Where(x => x.Id == groupId)
                .GroupJoin(_repository.Get<UserGroup>(),
                p => p.Id, c => c.UserGroupParentId,
                (p, c) => new { Parent = p, Children = c })
                .Select(x => new { Name = x.Parent.GroupName, Path = x.Parent.Path, Chilren = x.Children })
                .SingleOrDefault();

            var details = (from u in _repository.Get<User>()
                           join ug in _repository.Get<UserGroup>() on u.UserGroupId equals ug.Id
                           join r in _repository.Get<GroupToExerciseRight>() on ug.Id equals r.GroupId
                           join ed in _repository.Get<ExerciseDetails>() on r.ExerciseId equals ed.Id
                           where r.IsChosen
                               && u.IsDeleted == false
                               && ug.Path.StartsWith(current.Path)
                           select new { ug = ug, u = u, ed = ed });

            var exercises = (from ug in _repository.Get<UserGroup>()
                             join u in _repository.Get<User>() on ug.Id equals u.UserGroupId
                             join c in _repository.Get<Category>() on u.Id equals c.UserId
                             join e in _repository.Get<Exercise>() on c.Id equals e.CategoryId
                             where e.Attempted
                                && u.IsDeleted == false
                                && ug.Path.StartsWith(current.Path)
                             select new { ug = ug, u = u, e = e });

            var results = details.GroupJoin(exercises,
                d => d.u.Id, e => e.u.Id,
                (d, e) => new { ug = d.ug, u = d.u, ed = d.ed, e = e.Where(y => y.e.DetailsId == d.ed.Id) })
                .ToList();

            if (!results.Any())
            {
                return View("NoResults");
            }

            var scores = results.GroupBy(x => x.u.Id).GroupBy(x => x.First().ug.Id).Select(x => new
            {
                UserGroup = x.First().First().ug,
                Value = x.Select(y => new
                {
                    User = y.First().u,
                    Value = y.Select(z => new
                    {
                        ExerciseDetails = z.ed,
                        Value = z.e.OrderByDescending(t => t.e.Timestamp).Select(s => s.e.Score).FirstOrDefault(),
                    })
                })
            }).ToList();
            var groupStats = current.Chilren.Select(x => new
            {
                UserGroup = x,
                Scores = scores.Where(y => y.UserGroup.Path.StartsWith(x.Path))
                    .SelectMany(y => y.Value.SelectMany(z => z.Value.Select(s => s.Value)))
            })
            .Where(x => x.Scores.Any())
            .Select(x => new StatViewModel
            {
                Id = x.UserGroup.Id,
                Name = x.UserGroup.GroupName,
                PassedPercent = StatisticsHelper.CalculatePassedPercent(x.Scores),
                Average = StatisticsHelper.CalculateAverageScore(x.Scores),
            }).OrderBy(x => x.Name);

            var scoresForCurrentGroup = scores.SingleOrDefault(x => x.UserGroup.Id == groupId);
            IOrderedEnumerable<StatViewModel> userStats;
            if (scoresForCurrentGroup != null)
            {
                userStats = scoresForCurrentGroup.Value
                    .Select(x => new StatViewModel
                    {
                        Id = x.User.Id,
                        Name = x.User.Firstname + " " + x.User.Lastname,
                        PassedPercent = StatisticsHelper.CalculatePassedPercent(x.Value.Select(y => y.Value)),
                        Average = StatisticsHelper.CalculateAverageScore(x.Value.Select(y => y.Value)),
                    }).OrderBy(x => x.Name);
            }
            else
            {
                userStats = (new List<StatViewModel>()).OrderBy(x => x.Name);
            }
            var combined = scores.SelectMany(x => x.Value.SelectMany(y => y.Value.Select(z => z.Value)));

            var scoresByExercise = new Dictionary<ExerciseDetails, List<double>>();
            foreach (var ug in scores)
            {
                foreach (var u in ug.Value)
                {
                    foreach (var ed in u.Value)
                    {
                        if (!scoresByExercise.ContainsKey(ed.ExerciseDetails))
                        {
                            scoresByExercise.Add(ed.ExerciseDetails, new List<double>());
                        }
                        scoresByExercise[ed.ExerciseDetails].Add(ed.Value);
                    }
                }
            }

            var worstExercises = new List<ExerciseScore>();
            foreach (var ed in scoresByExercise)
            {
                worstExercises.Add(new ExerciseScore
                {
                    Id = ed.Key.Id,
                    Name = _handler.GetResource(ed.Key.Name, culture),
                    Score = ed.Value.Sum() / (double)ed.Value.Count,
                });
            }
            worstExercises = worstExercises.OrderBy(x => x.Score).Take(Constants.N_WORST_EXERCISES).ToList();

            var serializer = new JavaScriptSerializer();
            var model = new GroupResultViewModel
            {
                CurrentGroupName = current.Name,
                CombinedAvg = StatisticsHelper.CalculateAverageScore(combined),
                CombinedPassedProcent = StatisticsHelper.CalculatePassedPercent(combined),
                WorstExercises = worstExercises,
                GroupStats = groupStats,
                UserStats = userStats,
                GroupPlotData = serializer.Serialize(groupStats),
                UserPlotData = serializer.Serialize(userStats),
            };

            return View(model);
        }

        /// <summary>
        /// User results page
        /// </summary>
        /// <returns>User results view</returns>
        [AuthorizeAccess("Group3Page2")]
        public ActionResult Users()
        {
            var model = new UsersViewModel { Users = new List<VFOUser>() };

            var groupId = (int)Session["UserGroupID"];
            //Pick all child groups / users / category results for the customer
            var userAndGroups = (from grp in _repository.Get<UserGroup>()
                                 where grp.Id == groupId || grp.UserGroupParentId == groupId
                                 join user in _repository.Get<User>() on grp.Id equals user.UserGroupId
                                 join cat in _repository.Get<Category>() on user.Id equals cat.UserId into gcat
                                 where user.IsDeleted == false
                                 orderby user.Firstname + " " + user.Lastname
                                 select new
                                 {
                                     GroupId = grp.Id,
                                     UserId = user.Id,
                                     SalaryNumber = user.SalaryNumber,
                                     Firstname = user.Firstname,
                                     Lastname = user.Lastname,
                                     GroupName = grp.GroupName,
                                     Scores = gcat.Select(x => x.Score),
                                 }).ToList();

            model.Users = (from ug in userAndGroups
                           select new VFOUser
                           {
                               UserId = ug.UserId,
                               SalaryNumber = ug.SalaryNumber,
                               Firstname = ug.Firstname,
                               Lastname = ug.Lastname,
                               GroupName = ug.GroupName,
                           }).ToList();

            if (!model.Users.Any())
            {
                model.NoResults = true;
            }

            return View(model);
        }

        /// <summary>
        /// Category results page
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category results view</returns>
        [AuthorizeAccess("Group3Page2")]
        public ActionResult CategoryResults(int id)
        {
            var model = new CategoryResultsViewModel { Results = new List<CategoryResult>() };

            var user = _repository.Get<User>().Single(x => x.Id == id);
            model.UserId = user.Id;
            model.UserName = user.Firstname + " " + user.Lastname;

            var exerciseResults = _repository.Get<Category>()
                .Where(x => x.UserId == id)
                .Join(_repository.Get<Exercise>(),
                    c => c.Id, e => e.CategoryId,
                    (c, e) => e)
                .Where(x => x.Attempted);

            var results = _repository.Get<UserGroup>()
                .Where(x => x.Id == user.UserGroupId)
                .Join(_repository.Get<GroupToExerciseRight>(),
                    ug => ug.Id, r => r.GroupId,
                    (ug, r) => new { ug = ug, r = r })
                .Where(x => x.r.IsChosen)
                .Join(_repository.Get<ExerciseDetails>(),
                    ugr => ugr.r.ExerciseId, ed => ed.Id,
                    (ugr, ed) => ed)
                .GroupJoin(exerciseResults.DefaultIfEmpty(),
                    ed => ed.Id, e => e.DetailsId,
                    (ed, e) => new
                    {
                        ed = ed,
                        lastResult = e.OrderByDescending(t => t.Timestamp).FirstOrDefault(),
                        results = e,
                    })
                .GroupBy(x => x.ed.CategoryId)
                .Join(_repository.Get<CategoryDetails>(),
                    g => g.Key, cd => cd.Id,
                    (g, cd) => new { group = g, cd = cd }).ToList();
            model.Results = results
                .Select(x => new CategoryResult
                {
                    CategoryId = x.cd.Id,
                    CategoryName = _handler.GetResource(x.cd.Name, Session["WDCulture"].ToString()),
                    Passed = x.group.Where(y => y.lastResult != null && y.lastResult.Score >= Constants.MIN_SCORE_FOR_PASSING).Count() == x.group.Count(),
                    NTries = x.group.SelectMany(y => y.results).Count(),
                }).ToList();

            var exPassed = results.Select(
                x => x.group.Where(y => y.lastResult != null && y.lastResult.Score >= Constants.MIN_SCORE_FOR_PASSING)
                    .Count()).ToList();
            var exFailed = results.Select(
                x => x.group.Where(y => y.lastResult != null && y.lastResult.Score < Constants.MIN_SCORE_FOR_PASSING)
                    .Count()).ToList();
            var exTotal = results.Select(x => x.group.Count()).ToList();

            //model.PassedProcent = (Convert.ToDouble(exPassed.Sum()) / Convert.ToDouble(exPassed.Sum() + exFailed.Sum())) * 100;
            var scores = results.SelectMany(x => x.group.Select(y => y.lastResult != null ? y.lastResult.Score : 0d));
            model.PassedProcent = ((double)scores.Where(x => x >= Constants.MIN_SCORE_FOR_PASSING).Count() / (double)scores.Count()) * 100;
            model.Average = (double)scores.Sum() / (double)scores.Count();

            var usCi = new System.Globalization.CultureInfo("en-US");
            var activity = results
                .SelectMany(x => x.group.SelectMany(y => y.results))
                .GroupBy(x => x.Timestamp.Date)
                .Select(x => new { Date = x.Key.ToString(usCi), Count = x.Count() });

            var catIds = model.Results.Select(x => x.CategoryId).ToList();

            var plotData = new List<List<int>>() {
                exPassed,
                exFailed,
                exTotal.Select((x, i) => x - exPassed[i] - exFailed[i] ).ToList(),
            };
            var ticks = model.Results.Select(x => x.CategoryName);
            var serializer = new JavaScriptSerializer();
            model.TryStatistic.PlotData = serializer.Serialize(plotData);
            model.TryStatistic.Ticks = serializer.Serialize(ticks);
            model.TryStatistic.Ids = serializer.Serialize(catIds);
            model.ActivityStatistic.PlotData = serializer.Serialize(activity);
            return View(model);
        }

        /// <summary>
        /// Exercise results page for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="categoryId">Exercise category ID</param>
        /// <returns>Exercise results view</returns>
        [AuthorizeAccess("Group3Page2")]
        public ActionResult ExerciseResults(int userId, int categoryId)
        {
            var culture = Session["WDCulture"].ToString();

            var username = _repository.Get<User>()
                .Where(x => x.Id == userId)
                .Select(x => x.Firstname + " " + x.Lastname)
                .Single();

            var model = new ExerciseResultsViewModel
            {
                UserName = username,
                Results = new List<ExerciseResult>()
            };

            var categoryDetails = _repository.Get<CategoryDetails>()
                .Single(x => x.Id == categoryId);

            var exerciseDetails = _repository.Get<Exercise>()
                .Join(_repository.Get<ExerciseDetails>(),
                e => e.DetailsId, ed => ed.Id,
                (e, ed) => new { e = e, ed = ed });

            model.Results = _repository.Get<Category>()
                .Where(c => c.DetailsId == categoryDetails.Id && c.UserId == userId)
                .Join(exerciseDetails,
                    c => c.Id, e => e.e.CategoryId,
                    (c, e) => new { c = c, e = e.e, order = e.ed.OrderNr })
                .GroupBy(x => x.e.DetailsId)
                .Select(x => new ExerciseResult
                {
                    ExerciseId = x.Key,
                    ExerciseName = _repository.Get<ExerciseDetails>().Single(y => y.Id == x.First().e.DetailsId).Name,
                    //Passed = x.Where(y => y.e.Score >= Constants.MIN_SCORE_FOR_PASSING).Any(),
                    Passed = x.Where(y => y.e.Attempted).OrderByDescending(y => y.e.Timestamp).FirstOrDefault() == null ? false : x.Where(y => y.e.Attempted).OrderByDescending(y => y.e.Timestamp).FirstOrDefault().e.Score >= Constants.MIN_SCORE_FOR_PASSING,
                    Timestamp = x.Where(y => y.e.Attempted).OrderByDescending(y => y.e.Timestamp).FirstOrDefault().e.Timestamp,
                    NTries = x.Where(y => y.e.Attempted).Count(),
                    Order = x.First().order ?? 0,
                })
                .OrderBy(x => x.Order)
                .ToList();

            // Localize
            foreach (var res in model.Results)
            {
                res.ExerciseName = _handler.GetResource(res.ExerciseName, culture);
            }
            model.CategoryName = _handler.GetResource(categoryDetails.Name, culture);
            model.NoResults = model.Results.Select(x => x.NTries).Sum() == 0 ? true : false;
            var plotData = model.Results.Select(x => new { Name = x.ExerciseName, NTries = x.NTries });
            var serializer = new JavaScriptSerializer();
            model.Statistic.PlotData = serializer.Serialize(plotData);

            return View(model);
        }
    }
}