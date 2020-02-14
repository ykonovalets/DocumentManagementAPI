using Common.System;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.API.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static ActionResult Result(this ControllerBase controller, Result result)
        {
            if (result.Successful)
            {
                return new OkResult();
            }
            return new BadRequestObjectResult(result.Error.ConvertToApiError());
        }
    }
}
