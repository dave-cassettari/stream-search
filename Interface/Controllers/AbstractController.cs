using System.Web.Mvc;
using StreamSearch.Web.Controllers.Results;

namespace StreamSearch.Web.Controllers
{
    public abstract class AbstractController : Controller
    {
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
    }
}