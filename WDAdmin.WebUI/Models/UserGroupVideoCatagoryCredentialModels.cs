using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WDAdmin.WebUI.Models
{
    public class UserGroupVideoCatagoryCredentialModels
    {
        public int Id { get; set; }

        public int VideoCatagoryId { get; set; }

        public int UserGroupId { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public bool IsValid { get; set; }

    }
}