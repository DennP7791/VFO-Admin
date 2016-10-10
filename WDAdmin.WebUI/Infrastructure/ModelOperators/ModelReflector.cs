using System;
using System.Collections.Generic;
using System.Linq;
using WDAdmin.Domain.Entities;
using WDAdmin.WebUI.Models;

namespace WDAdmin.WebUI.Infrastructure.ModelOperators
{
    /// <summary>
    /// Reflection based methods for setting values in models
    /// </summary>
    public sealed class ModelReflector
    {
        /// <summary>
        /// The _page structure
        /// </summary>
        private readonly PageStructureGenerator _pageStructure = PageStructureGenerator.GetInstance;
        /// <summary>
        /// The _instance
        /// </summary>
        private static ModelReflector _instance;

        /// <summary>
        /// GetInstance method for singleton use
        /// </summary>
        /// <value>The get instance.</value>
        public static ModelReflector GetInstance
        {
            get { return _instance ?? (_instance = new ModelReflector()); }
        }

        /// <summary>
        /// Getting bool properties from within model class
        /// </summary>
        /// <param name="classToScan">Class to be checked for bool properties</param>
        /// <returns>List of Tuple objects (Property name, Property value)</returns>
        public List<Tuple<string, bool>> GetBoolPropertiesFromModel(dynamic classToScan)
        {
            var properties = new List<Tuple<string, bool>>();

            //Use Reflection to check bool fields in model and create rules about sites in DB
            var modelType = classToScan.GetType();
            var modelProps = modelType.GetProperties();

            foreach (var inf in modelProps)
            {
                if (inf.PropertyType.Name == "Boolean")
                {
                    var name = inf.Name;
                    var value = (bool)inf.GetValue(classToScan, null);
                    var propertyPair = new Tuple<string, bool>(name, value);
                    properties.Add(propertyPair);
                }
            }

            return properties;
        }

        /// <summary>
        /// Setting bool rights properties in model from DB input
        /// </summary>
        /// <param name="classToScan">Class to be updated - only bool properties</param>
        /// <param name="properties">List of Tuple objects (Property name, Property value)</param>
        /// <param name="rights">List of TemplateToPageRight objects - source rights values</param>
        /// <returns>Class with updated rights properties</returns>
        public dynamic SetRightsPropertiesInModelDb(dynamic classToScan, IEnumerable<Tuple<string, bool>> properties, IQueryable<TemplateToPageRight> rights)
        {
            foreach (var prop in properties)
            {
                var pageId = _pageStructure.GetPageId(prop.Item1);

                if (pageId != -1)
                {
                    var groupRight = (from gr in rights where gr.PageId == pageId select gr).FirstOrDefault();
                    if (groupRight != null)
                    {
                        var p = classToScan.GetType().GetProperty(prop.Item1);
                        p.SetValue(classToScan, groupRight.IsAllowed, null);
                    }
                }
            }

            return classToScan;
        }

        /// <summary>
        /// Setting bool rights properties in model from MasterRightsModel
        /// </summary>
        /// <param name="classToScan">Class to be updated - only bool properties</param>
        /// <param name="properties">List of Tuple objects (Property name, Property value)</param>
        /// <param name="rightsModel">MasterUserRightsModel object - source rights values</param>
        /// <returns>Class with updated rights properties</returns>
        public dynamic SetRightsPropertiesInModel(dynamic classToScan, IEnumerable<Tuple<string, bool>> properties, MasterUserRightsModel rightsModel)
        {
            foreach (var prop in properties)
            {
                var modelProperty = classToScan.GetType().GetProperty(prop.Item1);
                var rightsProperty = rightsModel.GetType().GetProperty(prop.Item1);

                if (rightsProperty != null)
                {
                    modelProperty.SetValue(classToScan, (bool)rightsProperty.GetValue(rightsModel, null), null);
                }
            }

            return classToScan;
        }

        /// <summary>
        /// Setting user values only to fields that are visible
        /// </summary>
        /// <param name="classToSet">Class with user fields to be filled</param>
        /// <param name="properties">List of Tuple objects (Property name, Property value)</param>
        /// <param name="userData">User object with source values</param>
        /// <returns>Class with filled user fields</returns>
        public dynamic SetValuesInModelDb(dynamic classToSet, IEnumerable<Tuple<string, bool>> properties, User userData)
        {
            foreach (var prop in properties)
            {
                var modelProperty = classToSet.GetType().GetProperty(prop.Item1);
                var modelValue = (bool)modelProperty.GetValue(classToSet, null);

                if (modelValue && (prop.Item1).Contains("_Visible"))
                {
                    var fieldName = (prop.Item1).Replace("_Visible", string.Empty);
                    var modelValueProperty = classToSet.GetType().GetProperty(fieldName);

                    if (modelValueProperty != null)
                    {
                        var userDataValue = userData.GetType().GetProperty(fieldName).GetValue(userData, null);
                        modelValueProperty.SetValue(classToSet, userDataValue, null);
                    }
                }
            }

            return classToSet;
        }
    }
}