using System.Web.Mvc;

namespace StreamSearch.Web.Views
{
    public abstract class AbstractViewPage<TModel> : WebViewPage<TModel>
    {
        public string Title { get; set; }
    }
}