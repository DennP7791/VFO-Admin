using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDAdmin.Domain.Entities;

namespace Migration
{
    class Program
    {
        public const string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=VFO;Integrated Security=True";

        static void Main(string[] args)
        {
            try
            {
                var db = new DataContext(connectionString);

                ////// For TESTING
                ////var groupHierarchi = GroupRelationShipUpdater.GenerateTestData();
                ////var updater = new GroupRelationShipUpdater(db);

                //// To run the real thing
                //try
                //{
                //    var res = db.ExecuteCommand(@"ALTER TABLE UserGroup ADD Path nvarchar(50) NULL;", new object[] { });
                //    //Debug.Assert(res > 0);
                //    db.SubmitChanges();
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
                var groups = db.GetTable<UserGroup>();
                var groupHierarchi = groups.Where(x => x.UserGroupParentId == null).JoinChildGroups(groups).ToList().AsQueryable();
                var updater = new GroupRelationShipUpdater(db, pretend: false);

                updater.UpdateGroupHierachi(groupHierarchi);
                updater.UpdateAll();

                ////Update to ExerciseDetails + new entries
                //try
                //{
                //    var res = db.ExecuteCommand(@"ALTER TABLE ExerciseDetails ADD OrderNr int NULL;", new object[] { });
                //    //Debug.Assert(res > 0);
                //    db.SubmitChanges();
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}

                //var exDetailsUpdater = new ExerciseDetailsUpdater(db);
                //exDetailsUpdater.AddNewExerciseDetails();
                //Console.WriteLine("New ExerciseDetails added!");
                //exDetailsUpdater.UpdateOrderNumbers();
                //Console.WriteLine("Old ExerciseDetails updated!");
                //exDetailsUpdater.AddExerciseRightsToGroups();
                //Console.WriteLine("Group rights created!");

                db.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press RETURN to exit");
            Console.ReadLine();
        }
    }

    public class GroupRelationShipUpdater
    {
        DataContext _db;
        bool _pretend;
        List<UserGroup> _groups = new List<UserGroup>();
        public GroupRelationShipUpdater(DataContext db, bool pretend = true)
        {
            _db = db;
            _pretend = pretend;
        }

        public void UpdateGroupHierachi(IQueryable<GroupRelationship> hierarchi, string pathPrefix = null)
        {
            foreach (var item in hierarchi)
            {
                var parent = item.Parent;
                var path = pathPrefix == null ? parent.Id.ToString() : pathPrefix + "." + parent.Id.ToString();
                Console.WriteLine(string.Format("{0}\tUpdating path to {1}", parent.Id, path));
                parent.Path = path;
                _groups.Add(parent);
                if (item.Children != null)
                {
                    UpdateGroupHierachi(item.Children, path);
                }
            }
        }

        public void UpdateAll()
        {
            var visited = new List<int>();
            if (!_pretend)
            {
                foreach (var item in _groups)
                {
                    if (!visited.Contains(item.Id))
                    {
                        visited.Add(item.Id);
                        Update(item);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Warning {0} have already been updated", item.Id));
                    }
                }
                _db.SubmitChanges();
            }
        }

        private void Update(UserGroup group)
        {
            var table = _db.GetTable<UserGroup>();

            //Check if entity already attached - returns null if not attached
            var origstate = table.GetOriginalEntityState(group);
            if (origstate == null)
            {
                table.Attach(group);
            }

            _db.Refresh(RefreshMode.KeepCurrentValues, group);
        }

        public static IQueryable<GroupRelationship> GenerateTestData()
        {
            var groupHierarchi = new List<GroupRelationship> {
                new GroupRelationship {
                    Parent = new UserGroup { Id = 1 },
                    Children = new List<GroupRelationship> {
                        new GroupRelationship {
                            Parent = new UserGroup { Id = 11 },
                            Children = new List<GroupRelationship> {
                                new GroupRelationship {
                                    Parent = new UserGroup { Id = 111 },
                                },
                            }.AsQueryable(),
                        },
                    }.AsQueryable(),
                },
                new GroupRelationship {
                    Parent = new UserGroup { Id = 2 },
                    Children = new List<GroupRelationship> {
                        new GroupRelationship {
                            Parent = new UserGroup { Id = 22 },
                        }
                    }.AsQueryable(),
                },
            }.AsQueryable();
            return groupHierarchi;
        }
    }

    public static class Extensions
    {
        public static IQueryable<GroupRelationship> JoinChildGroups(
            this IQueryable<WDAdmin.Domain.Entities.UserGroup> selection,
            IQueryable<WDAdmin.Domain.Entities.UserGroup> collection)
        {
            return selection.Select(x => new GroupRelationship
            {
                Parent = x,
                Children = collection
                    .Where(y => y.UserGroupParentId == x.Id)
                    .JoinChildGroups(collection),
            });
        }
    }

    public class GroupRelationship
    {
        public WDAdmin.Domain.Entities.UserGroup Parent { get; set; }
        public IQueryable<GroupRelationship> Children { get; set; }
    }

    public class ExerciseDetailsUpdater
    {
        DataContext _db;

        public ExerciseDetailsUpdater(DataContext db)
        {
            _db = db;
        }

        public void AddNewExerciseDetails()
        {
            var listOfDetails = new List<ExerciseDetails>();
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise103", SceneFunction = 106, CategoryId = 1, OrderNr = 3 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise104", SceneFunction = 107, CategoryId = 1, OrderNr = 4 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise211", SceneFunction = 108, CategoryId = 6, OrderNr = 6 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise507", SceneFunction = 109, CategoryId = 10, OrderNr = 2 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise508", SceneFunction = 110, CategoryId = 10, OrderNr = 4 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise509", SceneFunction = 111, CategoryId = 10, OrderNr = 7 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise510", SceneFunction = 112, CategoryId = 10, OrderNr = 10 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise511", SceneFunction = 113, CategoryId = 10, OrderNr = 11 });
            listOfDetails.Add(new ExerciseDetails { Name = "Exercise512", SceneFunction = 114, CategoryId = 10, OrderNr = 12 });

            foreach(var ld in listOfDetails)
            {
                AddExDetails(ld);
            }

            _db.SubmitChanges();
        }

        public void AddExDetails(ExerciseDetails eDetails)
        {
            var table = _db.GetTable<ExerciseDetails>();
            table.InsertOnSubmit(eDetails);
        }

        public void UpdateOrderNumbers()
        {
            //Get category Ids
            var exdtable = _db.GetTable<ExerciseDetails>();
            var categoryIds = exdtable.Select(x => x.CategoryId).Distinct().ToList();

            //Foreach category get exercises, update their order numbers and update objects
            foreach(var cid in categoryIds)
            {
                var exercises = exdtable.Where(x => x.CategoryId == cid).OrderBy(x => x.Id).ToList();

                var orderNr = 1;
                
                foreach(var exe in exercises)
                {
                    if (exercises.Any(x => x.OrderNr == orderNr))
                    {
                        orderNr = orderNr + 1;
                    }

                    if(exe.OrderNr == null)
                    {
                        exe.OrderNr = orderNr;
                    }

                    orderNr = orderNr + 1;
                }

                //Update objects in db
                foreach (var ld in exercises)
                {
                    UpdateExDetails(ld);
                }
            }

            _db.SubmitChanges();
        }

        public void UpdateExDetails(ExerciseDetails eDetails)
        {
            var table = _db.GetTable<ExerciseDetails>();

            //Check if entity already attached - returns null if not attached
            var origstate = table.GetOriginalEntityState(eDetails);
            if (origstate == null)
            {
                table.Attach(eDetails);
            }

            _db.Refresh(RefreshMode.KeepCurrentValues, eDetails);
        }

        public void AddExerciseRightsToGroups()
        {
            var exDetailsTable = _db.GetTable<ExerciseDetails>();
            var newExIds = exDetailsTable.Where(x => x.Id > 44).Select(x => x.Id).ToList();

            var groupsTable = _db.GetTable<UserGroup>();
            var groupIds = groupsTable.Select(x => x.Id);
            
            foreach(var gId in groupIds)
            {
                foreach(var exId in newExIds)
                {
                    var right = new GroupToExerciseRight { ExerciseId = exId, GroupId = gId, IsChosen = true };

                    AddExRight(right);
                }
            }

            _db.SubmitChanges();
        }

        public void AddExRight(GroupToExerciseRight eRight)
        {
            var table = _db.GetTable<GroupToExerciseRight>();
            table.InsertOnSubmit(eRight);
        }
    }
}
