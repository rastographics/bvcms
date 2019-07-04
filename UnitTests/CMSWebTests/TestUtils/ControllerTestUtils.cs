using Moq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMSWebTests
{
    public class ControllerTestUtils : Controller
    {
        public static ControllerContext FakeContextController(Controller controller, Dictionary<string, string> routeDataValues)
        {
            var mock = new Mock<ControllerContext>();
            var routeData = new RouteData();

            foreach (var keyValuePair in routeDataValues)
            {
                routeData.Values.Add(keyValuePair.Key, keyValuePair.Value);
            }

            mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            mock.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            mock.SetupGet(m => m.RouteData).Returns(routeData);

            var view = new Mock<IView>();
            var engine = new Mock<IViewEngine>();
            var viewEngineResult = new ViewEngineResult(view.Object, engine.Object);
            engine.Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(viewEngineResult);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(engine.Object);

            controller.ControllerContext = mock.Object;

            return controller.ControllerContext;
        }
    }
}
