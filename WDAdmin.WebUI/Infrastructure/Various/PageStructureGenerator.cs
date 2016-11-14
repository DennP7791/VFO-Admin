using System.Collections.Generic;

namespace WDAdmin.WebUI.Infrastructure
{
    /// <summary>
    /// Page structure resolver for WebUI
    /// </summary>
    public sealed class PageStructureGenerator
    {
        /// <summary>
        /// The _instance
        /// </summary>
        private static PageStructureGenerator _instance;
        /// <summary>
        /// The _page structure
        /// </summary>
        private SortedDictionary<string, int> _pageStructure;

        /// <summary>
        /// GetInstance method for singleton use
        /// </summary>
        /// <value>The get instance.</value>
        public static PageStructureGenerator GetInstance
        {
            get { return _instance ?? (_instance = new PageStructureGenerator()); }
        }

        /// <summary>
        /// Getting page ID - if page not found -1 is returned
        /// </summary>
        /// <param name="pageName">Page name ie. Group1Page1</param>
        /// <returns>Page ID, or -1</returns>
        public int GetPageId(string pageName)
        {
            var pageId = -1;
            return _pageStructure.TryGetValue(pageName, out pageId) ? pageId : pageId;
        }

        /// <summary>
        /// Generate dictionary with static page structure
        /// </summary>
        public void GeneratePageStructure()
        {
            _pageStructure = new SortedDictionary<string, int>();

            

            //Homepage modules
            _pageStructure.Add("Home", 1); //Homepage
            _pageStructure.Add("HomeModule1", 11); //TopAdmin module
            _pageStructure.Add("HomeModule2", 12); //Customer module
            _pageStructure.Add("HomeModule3", 13); //Admin module
            _pageStructure.Add("HomeModule4", 14); //User module


            //User management modules and pages
            _pageStructure.Add("Group1", 2); //User
            _pageStructure.Add("Group1Page1", 21); //UserIndex
            _pageStructure.Add("Group1Page2", 22); //Add new user
            _pageStructure.Add("Group1Page3", 23); //Edit user

            //User group management modules and pages
            _pageStructure.Add("Group2", 3); //UserGroup
            _pageStructure.Add("Group2Page1", 31); //UserGroupIndex
            _pageStructure.Add("Group2Page2", 32); //Add new UserGroup
            _pageStructure.Add("Group2Page3", 33); //Edit UserGroup
            _pageStructure.Add("Group2Page4", 34); //UserGroupTemplateIndex
            _pageStructure.Add("Group2Page5", 35); //Add new UserGroupTemplate
            _pageStructure.Add("Group2Page6", 36); //Edit UserGroupTemplate
            _pageStructure.Add("Group2Page7", 37); //Create new customer

            //Result & hyperlink-generation management pages
            _pageStructure.Add("Group3", 4); //ManagementGroup
            _pageStructure.Add("Group3Page1", 41); //Link generation page
            _pageStructure.Add("Group3Page2", 42); //Group results page

            //Content management modules and pages
            _pageStructure.Add("Group4", 5); //ContentGroup
            _pageStructure.Add("Group4Page1", 51); //CategoryIndex
            _pageStructure.Add("Group4Page2", 52); //Add new category
            _pageStructure.Add("Group4Page3", 53); //Edit category
            _pageStructure.Add("Group4Page4", 54); //ExerciseIndex
            _pageStructure.Add("Group4Page5", 55); //Add new exercise
            _pageStructure.Add("Group4Page6", 56); //Edit exercise
            _pageStructure.Add("Group4Page7", 57); //ExercisePartIndex
            _pageStructure.Add("Group4Page8", 58); //Add new exercise part
            _pageStructure.Add("Group4Page9", 59); //Edit exercise part

            //Log management modules and pages
            _pageStructure.Add("Group5", 6); //LogGroup
            _pageStructure.Add("Group5Page1", 61); //LogIndex
            _pageStructure.Add("Group5Page2", 62); //Log details

            _pageStructure.Add("VideoPassword", 69); //Admin Video Password
        }
    }
}