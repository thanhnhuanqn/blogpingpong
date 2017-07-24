using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Blog.Controllers;
using Blog.Models;
using NHibernate.Linq;
using NUnit.Framework.Internal;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace Blog.ControllerTests
{
    [TestFixture]
    public class VouxThemeControllerTests
    {
        [Test]
        public void ShouldRenderDefaultView()
        {
           var ts = new VouxThemeController();

            Database.Configure();
            Database.OpenSession();            
            
            ts.WithCallTo(x => x.Index(1)).ShouldRenderDefaultView()
                .WithModel(Database.Session.Query<Post>());


            Database.CloseSession();

        }
    }
}
