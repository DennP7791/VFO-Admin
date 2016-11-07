using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace WDAdmin.WebUI.Models
{
    /// <summary>
    /// Models/DataContracts from DataService
    /// </summary>
    
    [DataContract]
    public class Collection
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        [DataMember]
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        [DataMember]
        public List<CategoryData> Categories { get; set; }
    }
    /// <summary>
    /// Class QrVideoCollection.
    /// </summary>
    [DataContract]
    public class QrVideoCollection
    {
        /// <summary>
        /// Gets or sets the QR-videos.
        /// </summary>
        /// <value>The QR-videos.</value>
        [DataMember]
        public List<QrVideoData> QrVideos { get; set; }
    }
    /// <summary>
    /// Class VideoCategoryCollection.
    /// </summary>
    [DataContract]
    public class VideoCategoryCollection
    {
        /// <summary>
        /// Gets or sets the QR-videos.
        /// </summary>
        /// <value>The QR-videos.</value>
        [DataMember]
        public List<VideoCategoryData> VideoCategories { get; set; }
    }

    /// <summary>
    /// Class CategoryData.
    /// </summary>
    [DataContract]
    public class CategoryData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        [DataMember]
        public double Score { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the exercises.
        /// </summary>
        /// <value>The exercises.</value>
        [DataMember]
        public List<ExerciseData> Exercises { get; set; }
    }
    /// <summary>
    /// Class CategoryData.
    /// </summary>
    [DataContract]
    public class QrVideoData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        /// <value>The url.</value>
        [DataMember]
        public string Path { get; set; }
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        [DataMember]
        public int Count { get; set; }
        /// <summary>
        /// Gets or sets the usergroup identifier.
        /// </summary>
        /// <value>The userGroupId.</value>
        [DataMember]
        public int UserGroupId { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The userid.</value>
        [DataMember]
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the releasedate.
        /// </summary>
        /// <value>The releasedate.</value>
        [DataMember]    
        public DateTime? ReleaseDate { get; set; }
        /// <summary>
        /// Gets or sets the VideoCategoryId.
        /// </summary>
        /// <value>The password</value>
        [DataMember]
        public int VideoCategoryId { get; set; }
    }    
    /// <summary>
    /// Class VideoCategoryData.
    /// </summary>
    [DataContract]
    public class VideoCategoryData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        /// <value>The Name.</value>
        [DataMember]
        public string Name { get; set; }
    }
    /// <summary>
    /// Class UserGroupData.
    /// </summary>
    [DataContract]
    public class UserGroupData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the GroupName.
        /// </summary>
        /// <value>The GroupName.</value>
        [DataMember]
        public string GroupName { get; set; }
        /// <summary>
        /// Gets or sets the Customer identifier.
        /// </summary>
        /// <value>The Customer identifier.</value>
        [DataMember]
        public int? CustomerId { get; set; }
    }
    /// <summary>
    /// Class VideoUserViewData.
    /// </summary>
    [DataContract]
    public class VideoUserViewData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the video identifier.
        /// </summary>
        /// <value>The videoId.</value>
        [DataMember]
        public int VideoId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The userid.</value>
        [DataMember]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the ViewDate.
        /// </summary>
        /// <value>The viewdate.</value>
        [DataMember]
        public DateTime ViewDate { get; set; }
    }    
    /// <summary>
        /// Class ExerciseData.
        /// </summary>
    [DataContract]
    public class ExerciseData
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        [DataMember]
        public double Score { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the scene function.
        /// </summary>
        /// <value>The scene function.</value>
        [DataMember]
        public int SceneFunction { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExerciseData"/> is attempted.
        /// </summary>
        /// <value><c>true</c> if attempted; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool Attempted { get; set; }
    }
    /// <summary>
    /// Class LoginData.
    /// </summary>
    [DataContract]
    public class LoginData
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [DataMember]
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [DataMember]
        public string Password { get; set; }
    }
}