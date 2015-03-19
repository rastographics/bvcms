using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData.Registration;
using UtilityExtensions;

namespace CmsWeb
{
    class SmartBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            string type = null;
            if (modelType == typeof(Ask))
            {
                var requestname = bindingContext.ModelName + ".Type";
                var value = controllerContext.Controller.ValueProvider.GetValue(requestname);
                if (value == null)
                    throw new Exception("Ask Type '{0}' not found".Fmt(requestname));

                type = value.AttemptedValue;

                switch (type)
                {
                    case "AnswersNotRequired":
                    case "AskSMS":
                    case "AskEmContact":
                    case "AskInsurance":
                    case "AskDoctor":
                    case "AskAllergies":
                    case "AskTylenolEtc":
                    case "AskParents":
                    case "AskCoaching":
                    case "AskChurch":
                        return new Ask(type);
                    case "AskCheckboxes": return new AskCheckboxes();
                    case "AskDropdown": return new AskDropdown();
                    case "AskMenu": return new AskMenu();
                    case "AskSuggestedFee": return new AskSuggestedFee();
                    case "AskSize": return new AskSize();
                    case "AskRequest": return new AskRequest();
                    case "AskHeader": return new AskHeader();
                    case "AskInstruction": return new AskInstruction();
                    case "AskTickets": return new AskTickets();
                    case "AskYesNoQuestions": return new AskYesNoQuestions();
                    case "AskExtraQuestions": return new AskExtraQuestions();
                    case "AskText": return new AskText();
                    case "AskGradeOptions": return new AskGradeOptions();
                    default:
                        return base.CreateModel(controllerContext, bindingContext, modelType);
                }
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        override protected ICustomTypeDescriptor GetTypeDescriptor(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(Ask) && bindingContext.Model != null)
            {
                var concreteType = bindingContext.Model.GetType();

                if (Nullable.GetUnderlyingType(concreteType) == null)
                {
                    return new AssociatedMetadataTypeTypeDescriptionProvider(concreteType).GetTypeDescriptor(concreteType);
                }
            }
            return base.GetTypeDescriptor(controllerContext, bindingContext);
        }
    }

    public class NullableIntModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };
            object actualValue = null;
            int i;
            if (int.TryParse(valueResult.AttemptedValue, out i))
                actualValue = i;
            else if(valueResult.AttemptedValue.HasValue())
                modelState.Errors.Add(new FormatException("not a valid integer"));
            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            return actualValue;
        }
    }
}
