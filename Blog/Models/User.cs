using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Blog.Models
{
    public class User
    {
        public  virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }

        public virtual string Email { get; set; }

        public virtual string Url { get; set; }

        public  virtual DateTime Registered { get; set; }
        
        public virtual string ActivationKey { get; set; }
        
        public virtual Int16 Status { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual bool IsStatus => Status == 1;

        public virtual IList<Role> Roles { get; set; }

        public User()
        {
            Roles = new List<Role>();
            this.Registered = DateTime.UtcNow;
        }

        public virtual void SetPassword(string password)
        {
            Password = BCrypt.Net.BCrypt.HashPassword(password, 13);
        }

        public virtual bool CheckPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, Password);
        }

    }

    public class UserMap : ClassMapping<User>
    {
        public UserMap()
        {
            Table("users");

            Id(x=>x.Id, x=>x.Generator(Generators.Identity));

            Property(x=>x.UserName, x=>x.NotNullable(true));

            Property(x=>x.Password, x=>x.NotNullable(true));

            Property(x=>x.Email, x=>x.NotNullable(true));

            Property(x=>x.Url, x=>x.NotNullable(false));

            Property(x=>x.Registered, x=>x.NotNullable(true));

            Property(x=>x.ActivationKey, x=>x.NotNullable(false));

            Property(x => x.ActivationKey, x =>
            {
                x.Column("activation_key");
                x.NotNullable(true);
            });

            Property(x=>x.Status, x=>x.NotNullable(true));

            Property(x => x.DisplayName, x =>
            {
                x.Column("display_name");
                x.NotNullable(true);
            });

            Bag(x => x.Roles, x =>
            {
                x.Table("role_users");
                x.Key(k => k.Column("user_id"));
            }, x => x.ManyToMany(k => k.Column("role_id")));
            

        }
    }
}