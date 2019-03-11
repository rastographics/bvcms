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
        private static HttpContextBase _mockCurrent;
        public static HttpContextBase Current
        {
            get
            {
                if(_mockCurrent != null)
                    return _mockCurrent;
                if(HttpContext.Current == null)
                    throw new InvalidOperationException("HttpContext is not available");
                return new HttpContextWrapper(HttpContext.Current);
            }
        }
        public static void SetMockCurrent(HttpContextBase context)
        {
            _mockCurrent = context;
        }
    }
}
