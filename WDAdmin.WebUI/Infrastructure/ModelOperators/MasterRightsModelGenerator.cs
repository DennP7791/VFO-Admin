using System.Collections.Generic;
using System.Linq;
using System.Web;
using WDAdmin.Domain.Abstract;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Infrastructure.ModelOperators;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Infrastructure.ModelOperators
{
    /// <summary>
    /// Singleton implementation of the user rights model generator
    /// Used when user logged in or when the active user group changed
    /// </summary>
    public sealed class MasterRightsModelGenerator
    {
        /// <summary>
        /// The _instance
        /// </summary>
        private static MasterRightsModelGenerator _instance;
        /// <summary>
        /// The _repository
        /// </summary>
        private IGenericRepository _repository;
        /// <summary>
        /// The _reflector
        /// </summary>
        private ModelReflector _reflector;

        /// <summary>
        /// GetInstance method for singleton use
        /// </summary>
        /// <value>The get instance.</value>
        public static MasterRightsModelGenerator GetInstance
        {
            get { return _instance ?? (_instance = new MasterRightsModelGenerator()); }
        }

        /// <summary>
        /// Initialize repository
        /// </summary>
        /// <param name="repository">GenericRepository object to be initialized</param>
        public void SetRepository(IGenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Initialize model reflector
        /// </summary>
        /// <param name="reflector">ModelReflector object to be initialized</param>
        public void SetReflector(ModelReflector reflector)
        {
            _reflector = reflector;
        }

        /// <summary>
        /// MasterRightsModel generator
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        public void GenerateMasterRightsModel(int? userId)
        {
            //Reset Rights object in Session
            HttpContext.Current.Session["Rights"] = null;

            if (userId == null)
            {
                userId = (int)HttpContext.Current.Session["UserID"];
            }

            //Get all info necessary to set rights for the current user & UserGroup
            var user = (from us in _repository.Get<User>() where us.Id == userId select us).Single();
            var userGroup = (from ug in _repository.Get<UserGroup>() where ug.Id == user.UserGroupId select ug).Single();
            var userTemplate = (from ugt in _repository.Get<UserTemplate>() where ugt.Id == user.UserTemplateId select ugt).Single();
            var userToPageRights = from right in _repository.Get<TemplateToPageRight>() where right.UserTemplateId == userTemplate.Id && right.IsAllowed select right;

            HttpContext.Current.Session["UserGroup"] = userGroup.GroupName;

            //Initialize MasterUserRightsModel
            var masterModel = new MasterUserRightsModel();

            //Get bool properties from model and set the to values from DB
            var properties = _reflector.GetBoolPropertiesFromModel(masterModel);
            masterModel = (MasterUserRightsModel)_reflector.SetRightsPropertiesInModelDb(masterModel, properties, userToPageRights);

            //Put group to group rights into model
            masterModel.GroupToGroupViewRights = new List<GroupToGroupRight>
                                                     {
                                                         new GroupToGroupRight //Add the current group to allowed groups
                                                             {
                                                                 Id = userGroup.Id,
                                                                 GroupName = userGroup.GroupName,
                                                                 CountryId = userGroup.CountryId
                                                             }
                                                     };

            //Search for all children of a current group and put it into MasterModel
            var immidiateChildGroups = from icg in _repository.Get<UserGroup>() where icg.UserGroupParentId == userGroup.Id orderby icg.GroupName select icg;

            foreach (var ichg in immidiateChildGroups)
            {
                var gtgr = new GroupToGroupRight { Id = ichg.Id, GroupName = ichg.GroupName, CountryId = ichg.CountryId };
                masterModel.GroupToGroupViewRights.Add(gtgr);
                GetChildrenGroups(masterModel.GroupToGroupViewRights, ichg.Id);
            }

            //Put template rights into model
            masterModel.UserToTemplateViewRights = new List<UserToTemplateRight>
                                                        {
                                                            new UserToTemplateRight //Add the current template to allowed templates
                                                                {
                                                                    Id = userTemplate.Id,
                                                                    TemplateName = userTemplate.TemplateName,
                                                                    TemplateLevel = userTemplate.TemplateLevel,
                                                                    ParentTemplateId = userTemplate.ParentTemplateId
                                                                }
                                                        };

            //Search for all children templates and put them into MasterModel
            var immidiateChildTemplates = from ict in _repository.Get<UserTemplate>() where ict.ParentTemplateId == userTemplate.Id orderby ict.TemplateName select ict;

            foreach (var ict in immidiateChildTemplates)
            {
                var ugroupt = new UserToTemplateRight
                {
                    Id = ict.Id,
                    TemplateName = ict.TemplateName,
                    TemplateLevel = ict.TemplateLevel,
                    ParentTemplateId = ict.ParentTemplateId
                };
                masterModel.UserToTemplateViewRights.Add(ugroupt);
                GetChildrenTemplates(masterModel.UserToTemplateViewRights, ict.Id);
            }

            //Establish if user is TopAdmin and give full access if so
            if(userGroup.UserGroupParentId == null && userGroup.CustomerId == null)
            {
                masterModel.FullAccess = true;
            }
            else
            {
                masterModel.FullAccess = false;
            }
            
            //Save the updated values in Master model
            HttpContext.Current.Session["Rights"] = masterModel;
        }

        /// <summary>
        /// MasterRightsModel children groups helper
        /// </summary>
        /// <param name="gtgRights">List of GroupToGroupRight objects</param>
        /// <param name="parentId">ID of the parent user group</param>
        private void GetChildrenGroups(ICollection<GroupToGroupRight> gtgRights, int parentId)
        {
            //Child groups
            var childGroups = from cg in _repository.Get<UserGroup>()
                              where cg.UserGroupParentId == parentId
                              orderby cg.GroupName
                              select cg;

            //Check if there are nay children
            if (childGroups.Any())
            {
                foreach (var cg in childGroups)
                {
                    var gtgr = new GroupToGroupRight
                    {
                        Id = cg.Id,
                        GroupName = cg.GroupName,
                    };

                    gtgRights.Add(gtgr);

                    //Call itself for children
                    this.GetChildrenGroups(gtgRights, cg.Id);
                }
            }
        }

        /// <summary>
        /// MasterRightsModel children templates helper
        /// </summary>
        /// <param name="gttRights">List of GroupToTemplateRight objects</param>
        /// <param name="parentId">ID of the parent user group</param>
        private void GetChildrenTemplates(ICollection<UserToTemplateRight> gttRights, int parentId)
        {
            //Child templates
            var childTemplates = from ct in _repository.Get<UserTemplate>()
                                 where ct.ParentTemplateId == parentId
                                 orderby ct.TemplateName
                                 select ct;

            //Check if there are nay children
            if (childTemplates.Any())
            {
                foreach (var ct in childTemplates)
                {
                    var ugroupt = new UserToTemplateRight
                    {
                        Id = ct.Id,
                        TemplateName = ct.TemplateName,
                        TemplateLevel = ct.TemplateLevel,
                        ParentTemplateId = ct.ParentTemplateId
                    };

                    gttRights.Add(ugroupt);

                    //Call itself for children
                    this.GetChildrenTemplates(gttRights, ct.Id);
                }
            }
        }
    }
}