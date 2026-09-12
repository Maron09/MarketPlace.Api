using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.SharedKernel
{
    public static class ControllerResultExtensions
    {
        public static IActionResult ToActionResult(this ControllerBase controller, Result result)
        {
            var error = new { error = result.Error };
            return result.ErrorType switch
            {
                ErrorType.NotFound => controller.NotFound(error),
                ErrorType.Conflict => controller.Conflict(error),
                ErrorType.Unauthorized => controller.Unauthorized(error),
                _ => controller.BadRequest(error)
            };
        }
    }
}