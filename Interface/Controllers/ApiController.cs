using System.Linq;
using System.Web.Mvc;
using StreamSearch.Common.Models.Contexts;
using StreamSearch.Common.Models.Entities;
using StreamSearch.Interface.Views.Api;

namespace StreamSearch.Interface.Controllers
{
    [RoutePrefix("api")]
    public class ApiController : Controller
    {
        public const int DefaultPageSize = 10;

        [HttpPost]
        [Route("search")]
        public ActionResult Search(SearchFormModel form)
        {
            Video[] results;

            if (string.IsNullOrWhiteSpace(form.Term))
            {
                results = new Video[0];
            }
            else
            {
                using (var context = new DatabaseContext())
                {
                    var term = form.Term.Trim().ToUpper();

                    results = context.Set<Video>()
                        .Where(x => x.Title.ToUpper().Contains(term))
                        .OrderBy(x => x.Title)
                        .Skip(form.Page * DefaultPageSize)
                        .Take(DefaultPageSize)
                        .ToArray();
                }
            }

            return Json(results);
        }
    }
}