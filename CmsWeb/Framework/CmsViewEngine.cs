using System;
using System.Web.Mvc;

namespace CmsWeb.Framework
{
    public class CmsViewEngine : IViewEngine
    {
        public bool UseTouchPointViews { get; private set; }
         
        public CmsViewEngine(bool useTouchPointViews)
        {
            UseTouchPointViews = useTouchPointViews;  
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }
    }
}