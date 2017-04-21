using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Blog.Models;

namespace Blog.Areas.admin.ViewModels
{
    public class UsersIndex
    {
        public IEnumerable<User> Users { get; set; }

        public UsersIndex()
        {
            Users = new List<User>();
        }
    }
    
    public class RoleCheckbox
    {
        public int Id { get; set; }
        public bool IsChecked { get; set; }
        public string Name { get; set; }
    }


    public class UserNew
    {
        public IList<RoleCheckbox> Roles { get; set; }

        [Required, MaxLength(128)]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MaxLength(256), DataType(DataType.Url)]
        public string Url { get; set; }

        [DisplayName("Display name")]
        public string DisplayName { get; set; }
    }

    public class UserEdit
    {
        public IList<RoleCheckbox> Roles { get; set; }

        [Required, MaxLength(128)]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MaxLength(256), DataType(DataType.Url)]
        public string Url { get; set; }

        [DisplayName("Display name")]
        public string DisplayName { get; set; }

    }


}