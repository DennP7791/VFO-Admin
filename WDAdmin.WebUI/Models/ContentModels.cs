using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WDAdmin.Domain.Entities;
using WDAdmin.Resources;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// View model for Category
    /// </summary>
    public class CategoryViewModel
    {
        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public List<CategoryDetails> Categories { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [no categories].
        /// </summary>
        /// <value><c>true</c> if [no categories]; otherwise, <c>false</c>.</value>
        public bool NoCategories { get; set; }
    }

    /// <summary>
    /// Form model for Category
    /// </summary>
    public class CategoryFormModel
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(LangResources), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        public string CategoryName { get; set; }     
    }

    /// <summary>
    /// View model for Exercise
    /// </summary>
    public class ExerciseViewModel
    {
        public List<ExerciseDetails> Exercises { get; set; }
        public bool NoExercises { get; set; }
    }

    /// <summary>
    /// Form model for Exercise
    /// </summary>
    public class ExerciseFormModel
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(LangResources), Name = "Category")]
        public List<CategoryDetails> Categories { get; set; }
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        public int CategoryId { get; set; }
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "ExerciseName")]
        public string ExerciseName { get; set; }
        [Required(ErrorMessageResourceType = typeof(LangResources), ErrorMessageResourceName = "RequiredFieldError")]
        [Display(ResourceType = typeof(LangResources), Name = "SceneFunction")]
        public int SceneFunction { get; set; }
    }
}