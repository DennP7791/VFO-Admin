using System.Collections.Generic;
using WDAdmin.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using WDAdmin.Resources;
using System;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Model for overview of existing UserGroups
    /// </summary>
    public class UserGroupViewModel
    {
        public List<UserGroup> UserGroups { get; set; }

        public string GroupTree { get; set; }

        public Infrastructure.Various.GroupNode GroupHierachy { get; set; }
    }

    /// <summary>
    /// Model for UserGroup rights for the existing groups/pages - based on UserGroupTemplate
    /// </summary>
    public class UserGroupFormModel
    {
        public int Id { get; set; }
        public int UserGroupParentId { get; set; }
        public int CountryId { get; set; }
        public int? CustomerId { get; set; }
        public bool FullAccess { get; set; }
        public bool IsEditedBySelf { get; set; }

        //Group name
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "Groupname")]
        public string GroupName { get; set; }

        //List with available UserGroups checkboxes
        public List<GroupToGroupRight> GroupToGroupViewRights { get; set; }

        //List with UserGroups checkbox values
        public List<GroupToGroupRight> GroupToGroupRights { get; set; }

        //List of Category details
        public List<CategoryDetails> Categories { get; set; }

        //List of chosen categories/exercises groups will have access to
        public List<int> ExercisesChosen { get; set; }
    }

    /// <summary>
    /// Model for Customer add/edit
    /// </summary>
    public class CustomerFormModel : UserGroupFormModel
    {
        //Customer name
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "CustomerName")]
        public new string GroupName { get; set; }
        
        //List with available countries
        [Display(ResourceType = typeof(LangResources), Name = "Countries")]
        public List<Country> Countries { get; set; }

        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "CountryNotChosenError")]
        public new int CountryId { get; set; }
    }

    /// <summary>
    /// Model for overview of existing UserGroups templates
    /// </summary>
    public class UserTemplateViewModel
    {
        public List<UserTemplate> UserTemplates { get; set; }
        public string TemplateTree { get; set; }
    }
    
    /// <summary>
    /// Model for UserGroupTemplate rights for the existing groups/pages
    /// Remember to follow naming of bool fields according to tree structure in view :o)
    /// </summary>
    [Serializable]
    public class UserTemplateFormModel
    {
        public int Id { get; set; }
        public int TemplateLevel { get; set; }
        public int ParentTemplateId { get; set; }
        public bool IsActive { get; set; }

        //Group name
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredTemplateNameFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "TemplateName")]
        public string TemplateName { get; set; }

        //Tree structure of pages------------------------------------------------------------------------------
        
        //Homepage
        [Display(ResourceType = typeof(LangResources), Name = "Home")]
        public bool Home { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "HomeModule1")]
        public bool HomeModule1 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "HomeModule2")]
        public bool HomeModule2 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "HomeModule3")]
        public bool HomeModule3 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "HomeModule4")]
        public bool HomeModule4 { get; set; }

        
        //User management        
        [Display(ResourceType = typeof(LangResources), Name = "Group1")]
        public bool Group1 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group1Page1")]
        public bool Group1Page1 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group1Page2")]
        public bool Group1Page2 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group1Page3")]
        public bool Group1Page3 { get; set; }


        //User group management
        [Display(ResourceType = typeof(LangResources), Name = "Group2")]
        public bool Group2 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group2Page1")]
        public bool Group2Page1 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group2Page2")]
        public bool Group2Page2 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group2Page3")]
        public bool Group2Page3 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group2Page4")]
        public bool Group2Page4 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group2Page5")]
        public bool Group2Page5 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group2Page6")]
        public bool Group2Page6 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group2Page7")]
        public bool Group2Page7 { get; set; }


        //Result, exercise package & hyperlink-generation management
        [Display(ResourceType = typeof(LangResources), Name = "Group3")]
        public bool Group3 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group3Page1")]
        public bool Group3Page1 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group3Page2")]
        public bool Group3Page2 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group3Side3")]
        public bool Group3Side3 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group3Side4")]
        public bool Group3Side4 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group3Side5")]
        public bool Group3Side5 { get; set; }

        
        //Content management
        [Display(ResourceType = typeof(LangResources), Name = "Group4")]
        public bool Group4 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page1")]
        public bool Group4Page1 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page2")]
        public bool Group4Page2 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page3")]
        public bool Group4Page3 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page4")]
        public bool Group4Page4 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page5")]
        public bool Group4Page5 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page6")]
        public bool Group4Page6 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page7")]
        public bool Group4Page7 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page8")]
        public bool Group4Page8 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group4Page9")]
        public bool Group4Page9 { get; set; }


        //Log management
        [Display(ResourceType = typeof(LangResources), Name = "Group5")]
        public bool Group5 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group5Page1")]
        public bool Group5Page1 { get; set; }

        [Display(ResourceType = typeof(LangResources), Name = "Group5Page2")]
        public bool Group5Page2 { get; set; }
    }

    /// <summary>
    /// Helper class for UserGroups with Rights
    /// </summary>
    [Serializable]
    public class GroupToGroupRight
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public int CountryId { get; set; }
    }

    /// <summary>
    /// Helper class for UserGroupTemplates with Rights
    /// </summary>
    [Serializable]
    public class UserToTemplateRight
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public int TemplateLevel { get; set; }
        public int? ParentTemplateId { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Model for MasterPage UserGroup rights - inheriting from UserGroupTemplateFormModel
    /// </summary>
    [Serializable]
    public class MasterUserRightsModel : UserTemplateFormModel
    {       
        //List with allowed UserGroups
        public List<GroupToGroupRight> GroupToGroupViewRights { get; set; }

        //List with allowed UserGroupTemplates
        public List<UserToTemplateRight> UserToTemplateViewRights { get; set; }

        //TopAdmin full access value
        public bool FullAccess { get; set; }
    }
}