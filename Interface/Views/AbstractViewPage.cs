using System.Web.Mvc;

namespace StreamSearch.Interface.Views
{
    public abstract class AbstractViewPage<TModel> : WebViewPage<TModel>
    {
        public string Title { get; set; }
    }
}