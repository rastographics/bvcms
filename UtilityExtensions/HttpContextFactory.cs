using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace UtilityExtensions
{
    public class HttpContextFactory
    {
        private static HttpContextBase _currentContext;
        public static HttpContextBase Current
        {
            get
            {
                if (_currentContext != null)
                {
                    return _currentContext;
                }
                if(HttpContext.Current == null)
                {
                    return null;
                }
                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public static void SetCurrentContext(HttpContextBase context)
        {
            _currentContext = context;
        }
    }
}
