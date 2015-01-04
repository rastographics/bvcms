using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public static class ModelViewModel
    {
        public static void CopyProperties2(this object viewmodel, object model)
        {
            var modelProps = model.GetType().GetProperties().Where(pp => pp.CanWrite).ToArray();
            var viewmodelProps = viewmodel.GetType().GetProperties().Where(pp => pp.CanWrite).ToArray();

            foreach (var vm in viewmodelProps)
            {
                var viewmodelvalue = vm.GetValue(viewmodel, null);

                // find a target property of the same name as source
                var m = modelProps.FirstOrDefault(mm => mm.Name == vm.Name);

                if (m == null)
                    continue;

                // if they are the same type, then straight copy
                if (m.PropertyType == vm.PropertyType)
                    m.SetValue(model, viewmodelvalue, null);

                else if (viewmodelvalue is string)
                    m.SetPropertyFromText(model, (string)viewmodelvalue);

                else // Handle any other type mismatches like int = Nullable<int> or vice-versa
                    m.SetPropertyFromText(model, (viewmodelvalue ?? "").ToString());
            }
        }
    }
}