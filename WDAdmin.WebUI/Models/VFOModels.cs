using System;
using System.Collections.Generic;
using WDAdmin.Domain.Entities;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Model for CreateLink page
    /// </summary>
    public class LinkViewModel
    {
        /// <summary>
        /// The group hierachy
        /// </summary>
        public Infrastructure.Various.GroupNode GroupHierachy;
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        public int CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the group tree.
        /// </summary>
        /// <value>The group tree.</value>
        public string GroupTree { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [no groups].
        /// </summary>
        /// <value><c>true</c> if [no groups]; otherwise, <c>false</c>.</value>
        public bool NoGroups { get; set; }
        /// <summary>
        /// Gets or sets the user groups.
        /// </summary>
        /// <value>The user groups.</value>
        public List<UserGroup> UserGroups { get; set; } 
    }

    /// <summary>
    /// Class for UserGroup's responsible
    /// </summary>
    public class GroupsResponsible
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is responsible.
        /// </summary>
        /// <value><c>true</c> if this instance is responsible; otherwise, <c>false</c>.</value>
        public bool IsResponsible { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }
    }

    /// <summary>
    /// Model for UserResults page
    /// </summary>
    public class UsersViewModel
    {
        /// <summary>
        /// The group average
        /// </summary>
        public string GroupAverage;
        /// <summary>
        /// The group ticks
        /// </summary>
        public string GroupTicks;
        /// <summary>
        /// The user average
        /// </summary>
        public string UserAverage;
        /// <summary>
        /// The user ticks
        /// </summary>
        public string UserTicks;
        /// <summary>
        /// Gets or sets a value indicating whether [no results].
        /// </summary>
        /// <value><c>true</c> if [no results]; otherwise, <c>false</c>.</value>
        public bool NoResults { get; set; }
        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        public List<VFOUser> Users { get; set; }
    }

    /// <summary>
    /// Helper class for UserResults page
    /// </summary>
    public class VFOUser
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the salary number.
        /// </summary>
        /// <value>The salary number.</value>
        public int? SalaryNumber { get; set; }
        /// <summary>
        /// Gets or sets the firstname.
        /// </summary>
        /// <value>The firstname.</value>
        public string Firstname { get; set; }
        /// <summary>
        /// Gets or sets the lastname.
        /// </summary>
        /// <value>The lastname.</value>
        public string Lastname { get; set; }
        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName { get; set; }
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public string Score { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VFOUser"/> is passed.
        /// </summary>
        /// <value><c>true</c> if passed; otherwise, <c>false</c>.</value>
        public bool Passed { get; set; }
    }

    /// <summary>
    /// Model for CategoryResults page
    /// </summary>
    public class CategoryResultsViewModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the average.
        /// </summary>
        /// <value>The average.</value>
        public double Average { get; set; }
        /// <summary>
        /// Gets or sets the passed procent.
        /// </summary>
        /// <value>The passed procent.</value>
        public double PassedProcent { get; set; }
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public List<CategoryResult> Results { get; set; }
        /// <summary>
        /// Gets or sets the try statistic.
        /// </summary>
        /// <value>The try statistic.</value>
        public GraphModel TryStatistic { get; set; }
        /// <summary>
        /// Gets or sets the activity statistic.
        /// </summary>
        /// <value>The activity statistic.</value>
        public GraphModel ActivityStatistic { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryResultsViewModel"/> class.
        /// </summary>
        public CategoryResultsViewModel()
        {
            TryStatistic = new GraphModel();
            ActivityStatistic = new GraphModel();
        }
    }

    /// <summary>
    /// Helper class for CategoryResults page
    /// </summary>
    public class CategoryResult
    {
        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        /// <value>The category identifier.</value>
        public int CategoryId { get; set; }
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName { get; set; }
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public string Score { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CategoryResult"/> is passed.
        /// </summary>
        /// <value><c>true</c> if passed; otherwise, <c>false</c>.</value>
        public bool Passed { get; set; }
        /// <summary>
        /// Gets or sets the n tries.
        /// </summary>
        /// <value>The n tries.</value>
        public int NTries { get; set; }
    }

    /// <summary>
    /// Model for ExerciseResults page
    /// </summary>
    public class ExerciseResultsViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether [no results].
        /// </summary>
        /// <value><c>true</c> if [no results]; otherwise, <c>false</c>.</value>
        public bool NoResults { get; set; }
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public List<ExerciseResult> Results { get; set; }
        /// <summary>
        /// Gets or sets the statistic.
        /// </summary>
        /// <value>The statistic.</value>
        public GraphModel Statistic { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ExerciseResultsViewModel"/> class.
        /// </summary>
        public ExerciseResultsViewModel()
        {
            Statistic = new GraphModel();
        }

        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName { get; set; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
    }

    /// <summary>
    /// Class GraphModel.
    /// </summary>
    public class GraphModel
    {
        /// <summary>
        /// Gets or sets the plot data.
        /// </summary>
        /// <value>The plot data.</value>
        public string PlotData { get; set; }
        /// <summary>
        /// Gets or sets the ticks.
        /// </summary>
        /// <value>The ticks.</value>
        public string Ticks { get; set; }
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        /// <value>The labels.</value>
        public string Labels { get; set; }
        /// <summary>
        /// Gets or sets the ids.
        /// </summary>
        /// <value>The ids.</value>
        public string Ids { get; set; }
    }

    /// <summary>
    /// Helper class for ExerciseResults page
    /// </summary>
    public class ExerciseResult
    {
        /// <summary>
        /// Gets or sets the exercise identifier.
        /// </summary>
        /// <value>The exercise identifier.</value>
        public int ExerciseId { get; set; }
        /// <summary>
        /// Gets or sets the name of the exercise.
        /// </summary>
        /// <value>The name of the exercise.</value>
        public string ExerciseName { get; set; }
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public string Score { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExerciseResult"/> is passed.
        /// </summary>
        /// <value><c>true</c> if passed; otherwise, <c>false</c>.</value>
        public bool Passed { get; set; }
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime? Timestamp { get; set; }
        /// <summary>
        /// Gets or sets the n tries.
        /// </summary>
        /// <value>The n tries.</value>
        public int NTries { get; set; }
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }
    }

    /// <summary>
    /// Class FlotData.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FlotData<T>
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public List<List<T>> data { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string label { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="FlotData{T}"/> class.
        /// </summary>
        public FlotData()
        {
            data = new List<List<T>>();
        }
    }

    /// <summary>
    /// Class GroupResultViewModel.
    /// </summary>
    public class GroupResultViewModel
    {
        /// <summary>
        /// Gets or sets the name of the current group.
        /// </summary>
        /// <value>The name of the current group.</value>
        public string CurrentGroupName { get; set; }
        /// <summary>
        /// Gets or sets the combined passed procent.
        /// </summary>
        /// <value>The combined passed procent.</value>
        public double CombinedPassedProcent { get; set; }
        /// <summary>
        /// Gets or sets the combined average.
        /// </summary>
        /// <value>The combined average.</value>
        public double CombinedAvg { get; set; }
        /// <summary>
        /// Gets or sets the worst exercises.
        /// </summary>
        /// <value>The worst exercises.</value>
        public IEnumerable<ExerciseScore> WorstExercises { get; set; }
        /// <summary>
        /// Gets or sets the group stats.
        /// </summary>
        /// <value>The group stats.</value>
        public IEnumerable<StatViewModel> GroupStats { get; set; }
        /// <summary>
        /// Gets or sets the user stats.
        /// </summary>
        /// <value>The user stats.</value>
        public IEnumerable<StatViewModel> UserStats { get; set; }
        /// <summary>
        /// Gets or sets the group plot data.
        /// </summary>
        /// <value>The group plot data.</value>
        public string GroupPlotData { get; set; }
        /// <summary>
        /// Gets or sets the user plot data.
        /// </summary>
        /// <value>The user plot data.</value>
        public string UserPlotData { get; set; }
        //public GroupResultsGraphModel GroupPlotData { get; set; }
        //public GroupResultsGraphModel UserPlotData { get; set; }
    }

    /// <summary>
    /// Class StatViewModel.
    /// </summary>
    public class StatViewModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the passed percent.
        /// </summary>
        /// <value>The passed percent.</value>
        public double PassedPercent { get; set; }
        /// <summary>
        /// Gets or sets the average.
        /// </summary>
        /// <value>The average.</value>
        public double Average { get; set; }
    }

    /// <summary>
    /// Class GroupResultsGraphModel.
    /// </summary>
    public class GroupResultsGraphModel
    {
        /// <summary>
        /// Gets or sets the average.
        /// </summary>
        /// <value>The average.</value>
        public string Average { get; set; }
        /// <summary>
        /// Gets or sets the passed procent.
        /// </summary>
        /// <value>The passed procent.</value>
        public string PassedProcent { get; set; }
        /// <summary>
        /// Gets or sets the ticks.
        /// </summary>
        /// <value>The ticks.</value>
        public string Ticks { get; set; }
    }

    /// <summary>
    /// Class ExerciseScore.
    /// </summary>
    public class ExerciseScore
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public double Score { get; set; }
    }
}