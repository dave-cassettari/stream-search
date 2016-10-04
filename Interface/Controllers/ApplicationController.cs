using System.Web.Mvc;
using StreamSearch.Interface.Views.Application;

namespace StreamSearch.Interface.Controllers
{
    public class ApplicationController : Controller
    {
        [Route]
        public ActionResult Search()
        {
            return View(new SearchViewModel());
        }
    }
}