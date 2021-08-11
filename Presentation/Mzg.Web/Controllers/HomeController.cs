using Microsoft.AspNetCore.Mvc;
using Mzg.Web.Framework.Context;
using Mzg.Web.Framework.Controller;
using Mzg.Web.Framework.Filters;

namespace Mzg.Web.Controllers
{
    [TypeFilter(typeof(InitializationFilterAttribute), Order = 0)]
    [TypeFilter(typeof(IdentityFilterAttribute), Order = 1)]
    [TypeFilter(typeof(OrganizationFilterAttribute), Order = 2)]
    public class HomeController : XmsControllerBase
    {
        public HomeController(IWebAppContext appContext)
            : base(appContext)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}