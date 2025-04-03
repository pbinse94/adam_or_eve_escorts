using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.Common
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = context.ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage
                });
            }
            base.OnActionExecuting(context);
        }
    }
}
