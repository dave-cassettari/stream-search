using System.Web.Mvc;
using StreamSearch.Interface.Views.Api;

namespace StreamSearch.Interface.Controllers
{
    [RoutePrefix("api")]
    public class ApiController : Controller
    {
        [HttpPost]
        [Route("search")]
        public ActionResult Search(SearchFormModel form)
        {
            return Json(form);
        }
    }
}