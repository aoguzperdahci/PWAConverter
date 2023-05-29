using System.Security.Claims;

namespace PWAConverter.Helpers
{
    public static class HttpContextExtension
    {
        public static Guid GetUserId(this HttpContext context)
        {
            var claim = context.User.Claims.Single(c => c.Type == ClaimTypes.Actor);
            return Guid.Parse(claim.Value);
        }
    }
}
