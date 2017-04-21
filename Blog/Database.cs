using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

namespace Blog
{
    public class Database
    {
        private const string SessionKey = "Blog.Database.SessionKey";

        private static ISessionFactory _sessionFactory;

        public static ISession Session
        {
            get { return (ISession)HttpContext.Current.Items[SessionKey]; }
        }
        public static void Configure()
        {
            var config = new Configuration();
            //configure the connection string
            config.Configure();

            //add our mappings
            var mapper = new ModelMapper();
            mapper.AddMapping<UserMap>();
            mapper.AddMapping<RoleMap>();
            //mapper.AddMapping<TagMap>();
            //mapper.AddMapping<PostMap>();

            config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            //create session factory
            _sessionFactory = config.BuildSessionFactory();
        }

        public static void OpenSession()
        {
            HttpContext.Current.Items[SessionKey] = _sessionFactory.OpenSession();
        }

        public static void CloseSession()
        {
            var session = HttpContext.Current.Items[SessionKey] as ISession;
            if (session != null)
            {
                session.Close();
            }

            HttpContext.Current.Items.Remove(SessionKey);
        }
    }
}