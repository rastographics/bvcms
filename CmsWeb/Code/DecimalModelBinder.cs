using System;
using System.Globalization;
using System.Web.Mvc;

namespace CmsWeb
{
    public class DecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            ModelState modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            try
            {
                //Check if this is a nullable decimal and a null or empty string has been passed
                var isNullableAndNull = (bindingContext.ModelMetadata.IsNullableValueType && (string.IsNullOrEmpty(valueResult?.AttemptedValue)));

                //If not nullable and null then we should try and parse the decimal
                if (!isNullableAndNull)
                {
                    decimal d;
                    if(decimal.TryParse(valueResult.AttemptedValue, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                        actualValue = d;
                    else
                        throw new Exception($"cannot parse decimal value {valueResult.AttemptedValue}");
                }
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}