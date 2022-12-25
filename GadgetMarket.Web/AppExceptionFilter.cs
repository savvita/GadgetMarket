using GadgetMarket.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace GadgetMarket.Web
{
    public class AppExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                if (context.Exception is UserEmailConflictException)
                {
                    context.Result = new ObjectResult(new
                    {
                        code = "email-exists"
                    })
                    {
                        StatusCode = (int)HttpStatusCode.Conflict
                    };
                }

                else if (context.Exception is ForbiddenException)
                {
                    context.Result = new ObjectResult(new
                    {
                        code = "forbidden"
                    })
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                }

                else if (context.Exception is UserNotFoundException unfe)
                {
                    context.Result = new ObjectResult(new
                    {
                        code = "user-not-found",
                        userId = unfe.Id
                    })
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                else if (context.Exception is BadRequestException)
                {
                    context.Result = new ObjectResult(new
                    {
                        code = "bad-request"
                    })
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                else if (context.Exception is InvalidCredentialsException)
                {
                    context.Result = new ObjectResult(new
                    {
                        code = "invalid-credentials"
                    })
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                else if (context.Exception is CategoryNotFoundException cnfe)
                {
                    context.Result = new ObjectResult(new
                    {
                        code = "category-not-found",
                        categoryId = cnfe.Id
                    })
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }

                else if (context.Exception is GadgetNotFoundException gnfe)
                {
                    context.Result = new ObjectResult(new
                    {
                        code = "gadget-not-found",
                        gadgetId = gnfe.Id
                    })
                    {
                        StatusCode = (int)HttpStatusCode.NotFound
                    };
                }


                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
