using System.Web.Mvc;
using StreamSearch.Common.Models.Contexts;
using StreamSearch.Common.Models.Entities;
using StreamSearch.Interface.Views.Application;

namespace StreamSearch.Interface.Controllers
{
    public class ApplicationController : AbstractController
    {
        [Route]
        public ActionResult Search()
        {
            return View(new SearchViewModel());
        }

        [Route("{videoID:int}")]
        public ActionResult Watch(int videoID)
        {
            using (var context = new DatabaseContext())
            {
                var video = context.Set<Video>().Find(videoID);

                if (video == null)
                {
                    return HttpNotFound();
                }

                return View(new WatchViewModel()
                {
                    Video = video,
                });
            }
        }
    }
}