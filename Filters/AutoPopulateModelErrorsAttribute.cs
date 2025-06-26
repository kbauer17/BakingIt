// this filter gathers all ModelState Errors to a list
// it is registered as a service in the Program.cs
// Any controller action that returns a View(...) with invalid ModelState will have ViewBag.ModelErrors set automatically
// Pull in the Views\Shared\_ModelErrorsPartial.cshtml to display them in view

namespace BakingIt.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class AutoPopulateModelErrorsAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext ctx)
        {
            if (ctx.Controller is Controller c
                && !c.ModelState.IsValid
                && ctx.Result is ViewResult)
            {
                c.ViewBag.ModelErrors = c.ModelState
                     .SelectMany(kvp => kvp.Value!.Errors)
                     .Select(e => e.ErrorMessage)
                     .Where(msg => !string.IsNullOrWhiteSpace(msg))
                     .ToList();
            }
            base.OnResultExecuting(ctx);
        }
    }
}
