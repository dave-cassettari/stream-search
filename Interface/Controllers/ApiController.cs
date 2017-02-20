using System.Linq;
using System.Net;
using System.Web.Mvc;
using StreamSearch.Common.Models.Contexts;
using StreamSearch.Common.Models.Entities;
using StreamSearch.Web.Controllers.Api;
using StreamSearch.Web.Views.Api;

namespace StreamSearch.Web.Controllers
{
    [RoutePrefix("api")]
    public class ApiController : AbstractController
    {
        public const int DefaultPageSize = 20;

        [HttpPost]
        [Route("search")]
        public ActionResult Search(SearchFormModel form)
        {
            if (string.IsNullOrWhiteSpace(form.Term))
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            using (var context = new DatabaseContext())
            {
                var term = form.Term.Trim().ToUpper();
                var results = context.Set<Video>()
                    .Where(x => x.Title.ToUpper().Contains(term))
                    .OrderBy(x => x.Title)
                    .Skip(form.Page * DefaultPageSize)
                    .Take(DefaultPageSize)
                    .ToArray()
                    .Select(x => new VideoApiModel(x))
                    .ToArray();

                return Json(results);
            }
        }
    }
}