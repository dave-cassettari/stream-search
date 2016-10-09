using System.Web.Mvc;
using StreamSearch.Common.Models.Contexts;
using StreamSearch.Common.Models.Entities;
using StreamSearch.Web.Views.Application;

namespace StreamSearch.Web.Controllers
{
    public class ApplicationController : AbstractController
    {
        private readonly DatabaseContext _context;

        public ApplicationController()
        {
            _context = new DatabaseContext();
        }

        [Route]
        public ActionResult Search()
        {
            return View(new SearchViewModel());
        }

        [Route("watch/{videoID:int}")]
        public ActionResult Watch(int videoID)
        {
            var video = _context.Set<Video>().Find(videoID);

            if (video == null)
            {
                return HttpNotFound();
            }

            return View(new WatchViewModel()
            {
                Video = video,
            });
        }

        [Route("episodes/{showID:int}")]
        public ActionResult Episodes(int showID)
        {
            var show = _context.Set<Show>().Find(showID);

            if (show == null)
            {
                return HttpNotFound();
            }

            return View(new EpisodesViewModel()
            {
                Show = show,
            });
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}