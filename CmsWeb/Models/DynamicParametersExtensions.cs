using Dapper;

namespace CmsWeb.Models
{
    static class DynamicParametersExtensions
    {
        public static bool Contains(this DynamicParameters value, string name)
        {
            var result = true;
            var result2 = true;

            try
            {
                value.Get<object>(name);
            }
            catch { result = false; }

            if (!result)
            {
                try
                {
                    value.Get<object>(name.ToLower());
                }
                catch { result2 = false; }
            }
            return result || result2;
        }
    }
}