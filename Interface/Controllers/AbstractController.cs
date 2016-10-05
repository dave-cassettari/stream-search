using System.Web.Mvc;
using StreamSearch.Interface.Controllers.Results;

namespace StreamSearch.Interface.Controllers
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