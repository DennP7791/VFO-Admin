using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;
using WDAdmin.WebUI.Infrastructure;
using WDAdmin.WebUI.Infrastructure.Various;
using WDAdmin.WebUI.Models;
using Newtonsoft.Json;
using WDAdmin.Domain;
using System.Text;
using System.IO;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Controllers
{
    /// <summary>
    /// Class ServiceController.
    /// </summary>
    public class ServiceController : BaseController
    {
        /// <summary>
        /// The _repository
        /// </summary>
        private readonly IGenericRepository _repository;
        /// <summary>
        /// The _pass
        /// </summary>
        private readonly PassGenHash _pass;
        /// <summary>
        /// The _handler
        /// </summary>
        private readonly ResourceHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceController"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public ServiceController(IGenericRepository repository)
            : base(repository)
        {
            _repository = repository;
            _pass = PassGenHash.GetInstance;
            _handler = ResourceHandler.GetInstance;
        }


        /// <summary>
        /// Service user authorization
        /// </summary>
        /// <param name="jobject">LoginData</param>
        /// <returns>User ID, -1 for invalid credentials, -9000 for error</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [HttpPost]
        [JsonFilter(Param = "jobject", RootType = typeof(LoginData))]
        public int Authorize(LoginData jobject)
        {
            var loginData = jobject;

            User user;
            try
            {
                //Get user from DB
                user = (from use in _repository.Get<User>()
                        where use.Email == loginData.Username && use.IsDeleted == false || use.Username == loginData.Username && use.IsDeleted == false
                        select use).Single();

                //Generate salted pass if user has pass and salt
                var saltedPass = user.Salt;

                if (!string.IsNullOrEmpty(saltedPass))
                {
                    saltedPass = _pass.CheckSaltedPass(loginData.Password, user.Salt);
                }

                if (user.Id == 0 || string.IsNullOrEmpty(saltedPass) && !user.Password.Equals(_pass.CreateSHA512Hash(loginData.Password)) || !string.IsNullOrEmpty(saltedPass) && saltedPass != user.Password)
                {
                    throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(InvalidOperationException)) //Invalid credentials
                {
                    Logger.Log("Login InvalidCredentials", ex.Message, loginData.Username, LogType.LoginError, LogEntryType.Warning);
                    return -1;
                }
                else //Login error
                {
                    Logger.Log("Login Error", ex.Message, loginData.Username, LogType.LoginError, LogEntryType.Error);
                    return -9000;
                }
            }

            Logger.Log("Login AuthorizeOK", "UserId: " + user.Id, loginData.Username, LogType.LoginOk, LogEntryType.Info);
            return user.Id;
        }


        /// <summary>
        /// "Fake" service for getting message to old clients
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Serialized data object</returns>
        public object GetData(int id)
        {
            var userId = id;
            var unityData = new Collection { UserId = userId, Categories = new List<CategoryData>() };
            unityData.Categories.Add(new CategoryData { Id = 1, Name = LangResources.UpdateMessage1, Exercises = new List<ExerciseData>() });
            unityData.Categories.Add(new CategoryData { Id = 2, Name = LangResources.UpdateMessage2, Exercises = new List<ExerciseData>() });
            unityData.Categories.Add(new CategoryData { Id = 3, Name = LangResources.UpdateMessage3, Exercises = new List<ExerciseData>() });
            unityData.Categories.Add(new CategoryData { Id = 4, Name = LangResources.UpdateMessage4, Exercises = new List<ExerciseData>() });
            Logger.Log("GetData FakeServiceOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);
            return JsonConvert.SerializeObject(unityData);
        }


        /// <summary>
        /// Get exercise data for VFO client
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="culture">Culture string</param>
        /// <returns>Serialized data object with categories, exercises and results</returns>
        [HttpGet]
        public object GetExercises(int id, string culture)
        {
            //Test situation (-1) - Normal user
            var userId = id == -1 ? (from use in _repository.Get<User>() select use.Id).First() : id;
            Logger.Log("GetExercises InitOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);

            //Get exercises allowed for the user group
            var allExer = (from use in _repository.Get<User>()
                           where use.Id == id
                           join ugr in _repository.Get<UserGroup>() on use.UserGroupId equals ugr.Id
                           join exe in _repository.Get<GroupToExerciseRight>() on ugr.Id equals exe.GroupId
                           where exe.IsChosen
                           select exe).ToList();

            //Get ExerciseDetails for allowed exercises
            var exerDetails = allExer.Select(aE => (from ed in _repository.Get<ExerciseDetails>() where ed.Id == aE.ExerciseId select ed).Single()).ToList();

            //Get CategoryDetails for exercises
            var exerCatId = (from eci in exerDetails select eci.CategoryId).Distinct().ToList();
            var catDetails = exerCatId.Select(edt => (from cd in _repository.Get<CategoryDetails>() where cd.Id == edt select cd).Single()).ToList();

            var exerLoc = new Dictionary<string, string>();
            var catLoc = new Dictionary<string, string>();

            //Create lists of exercise/category names according to culture specified
            if (string.IsNullOrEmpty(culture) || culture.Equals("da-DK")) //No culture of default culture specified
            {
                exerLoc = _handler.GetResources(exerDetails.Select(x => x.Name).ToList(), "da-DK");
                catLoc = _handler.GetResources(catDetails.Select(x => x.Name).ToList(), "da-DK");
            }
            else
            {
                exerLoc = _handler.GetResources(exerDetails.Select(x => x.Name).ToList(), culture);
                catLoc = _handler.GetResources(catDetails.Select(x => x.Name).ToList(), culture);
            }

            var unityData = new Collection { UserId = userId, Categories = new List<CategoryData>() };

            //For each category pick latest category data, list of attached exercises & exercise data
            foreach (var cat in catDetails)
            {
                //Get existing exercises for the current category
                var exercises = (from exer in exerDetails where exer.CategoryId == cat.Id select exer).OrderBy(x => x.OrderNr).ToList();

                //Pick latest category data score
                var latestCatScore = from catd in _repository.Get<Category>()
                                     where catd.UserId == userId && catd.DetailsId == cat.Id
                                     group catd by catd.DetailsId into ct
                                     select ct.OrderByDescending(t => t.Timestamp).First();

                if (latestCatScore.Any())
                {
                    //Create category data object
                    var cdata = new CategoryData
                                        {
                                            Id = cat.Id,
                                            Name = catLoc[cat.Name],
                                            Score = latestCatScore.Single().Score,
                                            Exercises = new List<ExerciseData>()
                                        };

                    foreach (var exer in exercises)
                    {
                        //Pick latest exercise data score
                        var latestEx = from exe in _repository.Get<Exercise>()
                                       where exe.CategoryId == latestCatScore.Single().Id && exe.DetailsId == exer.Id
                                       group exe by exe.DetailsId
                                           into ex
                                           select ex.OrderByDescending(t => t.Timestamp).First();

                        if (latestEx.Any())
                        {
                            //Create ExerciseData object + PartData object
                            var exdata = new ExerciseData
                                                {
                                                    Id = exer.Id,
                                                    Name = exerLoc[exer.Name],
                                                    SceneFunction = exer.SceneFunction,
                                                    Score = latestEx.Single().Score,
                                                    Attempted = false //latestEx.Single().Attempted
                                                };

                            //Add exercise data to Category Object
                            cdata.Exercises.Add(exdata);

                        }
                        else //No data found
                        {
                            //Create new ExerciseData object
                            var exdata = new ExerciseData
                                                {
                                                    Id = exer.Id,
                                                    Name = exerLoc[exer.Name],
                                                    SceneFunction = exer.SceneFunction,
                                                    Score = 0,
                                                    Attempted = false
                                                };

                            //Add exercise data to Category Object
                            cdata.Exercises.Add(exdata);
                        }
                    }

                    //Add CategoryData object to collection
                    unityData.Categories.Add(cdata);
                }
                else //No latest element found for category
                {
                    //Create CategoryData object

                    var cdata = new CategoryData { Id = cat.Id, Name = catLoc[cat.Name], Score = 0, Exercises = new List<ExerciseData>() };

                    //Loop through exercises
                    foreach (var exe in exercises)
                    {
                        //Create ExerciseData object
                        var exdata = new ExerciseData
                                         {
                                             Id = exe.Id,
                                             Name = exerLoc[exe.Name],
                                             SceneFunction = exe.SceneFunction,
                                             Score = 0,
                                             Attempted = false
                                         };

                        //Add ExerciseData object to category
                        cdata.Exercises.Add(exdata);
                    }

                    //Add CategoryData object to collection
                    unityData.Categories.Add(cdata);
                }
            }

            Logger.Log("GetExercises FinalOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);
            return JsonConvert.SerializeObject(unityData);
        }
        /// <summary>
        /// Get VideoCategory data for VFO client
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Serialized data object with videos</returns>
        [HttpGet]
        public object GetVideoCategories()
        {
            var allVideoCategories = _repository.Get<VideoCategory>();

            var unityData = new VideoCategoryCollection { VideoCategories = new List<VideoCategoryData>() };
            foreach (var cat in allVideoCategories)
            {
                var video = new VideoCategoryData
                {
                    Id = cat.Id,
                    Name = cat.Name
                };
                unityData.VideoCategories.Add(video);
            }
            return JsonConvert.SerializeObject(unityData);
        }
        /// <summary>
        /// Get VideoCategory data for VFO client
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Serialized data object with videos</returns>
        [HttpGet]
        public object GetUserGroup(int id)
        {
            var userId = id == -1 ? (from use in _repository.Get<User>() select use.Id).First() : id;
            var userGroup = (from use in _repository.Get<User>()
                             where use.Id == id
                             join ugr in _repository.Get<UserGroup>() on use.UserGroupId equals ugr.Id
                             select ugr).Single();

            var unityData = new UserGroupData();
            unityData.Id = userGroup.Id;
            unityData.GroupName = userGroup.GroupName;
            unityData.CustomerId = userGroup.CustomerId;

            return JsonConvert.SerializeObject(unityData);
        }
        /// <summary>
        /// Get video data for VFO client
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Serialized data object with videos</returns>
        [HttpGet]
        public object GetQrVideos(int id)
        {
            //Test situation (-1) - Normal user
            var userId = id == -1 ? (from use in _repository.Get<User>() select use.Id).First() : id;
            Logger.Log("GetVideos InitOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);

            var userGroup = (from use in _repository.Get<User>()
                             where use.Id == id
                             join ugr in _repository.Get<UserGroup>() on use.UserGroupId equals ugr.Id
                             select ugr).Single();

            List<Video> allVideos = null;
            if (userGroup.CustomerId != null)
            {
                allVideos = (from vid in _repository.Get<Video>()
                             where userGroup.CustomerId.Equals(userGroup.CustomerId) && vid.VideoCategoryId != (int)WDAdmin.Domain.Entities.VideoCategory.CategoryType.IndividuelForflytning
                             select vid).ToList();
            }
            else
            {
                allVideos = (from vid in _repository.Get<Video>()
                             where userGroup.Id.Equals(vid.UserGroupId)
                             select vid).ToList();
            }

            var unityData = new QrVideoCollection { QrVideos = new List<QrVideoData>() };
            foreach (var vid in allVideos)
            {
                var video = new QrVideoData
                {
                    Id = vid.Id,
                    Name = vid.Name,
                    Description = vid.Description,
                    Path = vid.Path,
                    Count = vid.Count,
                    UserGroupId = vid.UserGroupId,
                    UserId = vid.UserId,
                    ReleaseDate = vid.ReleaseDate,
                    VideoCategoryId = vid.VideoCategoryId
                };
                unityData.QrVideos.Add(video);
            }

            Logger.Log("GetVideos FinalOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);
            return JsonConvert.SerializeObject(unityData);
        }
        /// <summary>
        /// Get video Path data for VFO client
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Serialized data object with videos</returns>
        [HttpGet]
        public object GetVideoPaths(int id)
        {
            //Test situation (-1) - Normal user
            var userId = id == -1 ? (from use in _repository.Get<User>() select use.Id).First() : id;
            Logger.Log("GetVideos InitOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);

            var allVideos = (from video in _repository.Get<Video>() where video.UserId.Equals(id) & video.ReleaseDate.Equals(null) select video).ToList();

            var unityData = new QrVideoCollection { QrVideos = new List<QrVideoData>() };
            foreach (var vid in allVideos)
            {
                var video = new QrVideoData
                {
                    Id = vid.Id,
                    Name = vid.Name,
                    Description = vid.Description,
                    Path = vid.Path,
                    Count = vid.Count,
                    UserGroupId = vid.UserGroupId,
                    UserId = vid.UserId,
                    ReleaseDate = vid.ReleaseDate,
                    VideoCategoryId = vid.VideoCategoryId
                };
                unityData.QrVideos.Add(video);
            }

            Logger.Log("GetVideos FinalOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);
            return JsonConvert.SerializeObject(unityData);
        }
        /// <summary>
        /// Data save for VFO client
        /// </summary>
        /// <param name="jobject">Collection of data from VFO client</param>
        /// <returns>Result of the data save</returns>
        [HttpPost]
        [JsonFilter(Param = "jobject", RootType = typeof(Collection))]
        public bool SaveData(Collection jobject)
        {
            var unityData = jobject;
            var stamp = DateTime.Now; //Get the current timestamp
            int userId;

            //Resolve user id
            if (unityData.UserId != -1) //Not test case
            {
                userId = unityData.UserId; //Get Id from JSON string
            }
            else //Test case, take the last user available
            {
                userId = (from use in _repository.Get<User>() select use.Id).First();
            }

            Logger.Log("SaveData InitOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);

            using (var transaction = TransactionScopeUtils.CreateTransactionScope())
            {
                foreach (var cat in unityData.Categories)
                {
                    var category = new Category { DetailsId = cat.Id, Score = cat.Score, UserId = userId, Timestamp = stamp };

                    if (!CreateEntity(category, "SaveData Category Error", "UserId: " + userId, LogType.DbCreateError))
                    {
                        return false;
                    }

                    foreach (var exer in cat.Exercises)
                    {
                        var exercise = new Exercise
                        {
                            DetailsId = exer.Id,
                            Score = exer.Score,
                            CategoryId = category.Id,
                            Timestamp = stamp,
                            Attempted = exer.Attempted
                        };

                        if (!CreateEntity(exercise, "SaveData Exercise Error", "UserId: " + userId, LogType.DbCreateError))
                        {
                            return false;
                        }
                    }
                }

                transaction.Complete();
            }

            Logger.Log("SaveData FinalOK", "UserId: " + userId, LogType.DbCreateOk, LogEntryType.Info);
            return true;
        }
        /// <summary>
        /// VideoData save for VFO client
        /// </summary>
        /// <param name="jobject">Collection of videodata from VFO client</param>
        /// <returns>Result of the videodata save</returns>
        [HttpPost]
        [JsonFilter(Param = "jobject", RootType = typeof(QrVideoData))]
        public bool SaveQrVideo(QrVideoData jobject)
        {
            var stamp = DateTime.Now; //Get the current timestamp
            int userId;

            //Resolve user id
            if (jobject.UserId != -1) //Not test case
            {
                userId = jobject.UserId; //Get Id from JSON string
            }
            else //Test case, take the last user available
            {
                userId = (from use in _repository.Get<User>() select use.Id).First();
            }

            Logger.Log("SaveVideo InitOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);

            using (var transaction = TransactionScopeUtils.CreateTransactionScope())
            {

                var video = new Video
                {
                    Id = jobject.Id,
                    Name = jobject.Name,
                    Description = jobject.Description,
                    Path = jobject.Path,
                    Count = jobject.Count,
                    UserGroupId = jobject.UserGroupId,
                    UserId = userId,
                    ReleaseDate = jobject.ReleaseDate,
                    VideoCategoryId = jobject.VideoCategoryId
                };

                if (!CreateEntity(video, "SaveVideo Video Error", "UserId: " + userId, LogType.DbCreateError))
                {
                    return false;
                }

                transaction.Complete();
            }

            Logger.Log("SaveVideo FinalOK", "UserId: " + userId, LogType.DbCreateOk, LogEntryType.Info);
            return true;
        }
        /// <summary>
        /// SaveQrVideoUserView save for VFO client
        /// </summary>
        /// <param name="jobject">Collection of QrVideoUserViews from VFO client</param>
        /// <returns>Result of the QrVideoUserView save</returns>
        [HttpPost]
        [JsonFilter(Param = "jobject", RootType = typeof(VideoUserViewData))]
        public bool SaveQrVideoUserViewData(VideoUserViewData jobject)
        {
            var stamp = DateTime.Now; //Get the current timestamp
            int userId;

            //Resolve user id
            if (jobject.UserId != -1) //Not test case
            {
                userId = jobject.UserId; //Get Id from JSON string
            }
            else //Test case, take the last user available
            {
                userId = (from use in _repository.Get<User>() select use.Id).First();
            }

            Logger.Log("SaveVideoUserView InitOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);

            using (var transaction = TransactionScopeUtils.CreateTransactionScope())
            {

                var videoUserView = new VideoUserView
                {
                    VideoId = jobject.VideoId,
                    UserId = userId,
                    ViewDate = stamp
                };

                if (!CreateEntity(videoUserView, "SaveVideoUserView VideoUserView Error", "UserId: " + userId, LogType.DbCreateError))
                {
                    return false;
                }

                transaction.Complete();
            }

            Logger.Log("SaveVideoUserView FinalOK", "UserId: " + userId, LogType.DbCreateOk, LogEntryType.Info);
            return true;
        }
        /// <summary>
        /// QrVideo Update for VFO client
        /// </summary>
        /// <param name="jobject">Collection of QrVideo from VFO client</param>
        /// <returns>Result of the QrVideo Update</returns>
        [HttpPut]
        [JsonFilter(Param = "jobject", RootType = typeof(QrVideoData))]
        public bool UpdateQrVideo(QrVideoData jobject)
        {
            int userId;
            int? count = jobject.Count + 1;

            //Resolve user id
            if (jobject.UserId != -1) //Not test case
            {
                userId = jobject.UserId; //Get Id from JSON string
            }
            else //Test case, take the last user available
            {
                userId = (from use in _repository.Get<User>() select use.Id).First();
            }

            Logger.Log("UpdateVideo InitOK", "UserId: " + userId, LogType.Ok, LogEntryType.Info);

            using (var transaction = TransactionScopeUtils.CreateTransactionScope())
            {
                var video = new Video
                {
                    Id = jobject.Id,
                    Name = jobject.Name,
                    Description = jobject.Description,
                    Path = jobject.Path,
                    Count = jobject.Count,
                    UserGroupId = jobject.UserGroupId,
                    UserId = userId,
                    ReleaseDate = jobject.ReleaseDate,
                    VideoCategoryId = jobject.VideoCategoryId
                };

                if (!UpdateEntity(video, "SaveVideo Video Error", "UserId: " + userId, LogType.DbCreateError))
                {
                    return false;
                }

                transaction.Complete();
            }

            Logger.Log("UpdateVideo FinalOK", "UserId: " + userId, LogType.DbCreateOk, LogEntryType.Info);
            return true;
        }

        /// <summary>
        /// QrVideo Update for VFO client
        /// </summary>
        /// <param name="jobject">Collection of QrVideo from VFO client</param>
        /// <returns>Result of the QrVideo Update</returns>
        [HttpPut]
        public bool UpdateVideoCompression(Guid id)
        {
            bool success = false;
            Video video = _repository.Get<Video>().SingleOrDefault(x => x.Id == id);

            if (video != null)
            {
                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    video.IsCompressed = true;

                    if (!UpdateEntity(video, "SaveVideo Video Error", "Video Id: " + id, LogType.DbCreateError))
                    {
                        success = false;
                    }
                    else
                    {
                        success = true;
                    }
                    transaction.Complete();
                }

                Logger.Log("Update IsCompressed on: " + id, LogType.DbUpdateOk, LogEntryType.Info);
            }
            return success;
        }

        /// <summary>
        /// Physical delete of a video
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public object DeleteVideo(Guid id)
        {
            Video video = _repository.Get<Video>().SingleOrDefault(x => x.Id == id);
            bool success = false;

            if (video != null)
            {
                using (var transaction = TransactionScopeUtils.CreateTransactionScope())
                {
                    success = DeleteEntity<Video>(video, "Failed to delete video", "", LogType.DbDeleteError);
                    transaction.Complete();
                }
            }

            return success;
        }


        
    }
}