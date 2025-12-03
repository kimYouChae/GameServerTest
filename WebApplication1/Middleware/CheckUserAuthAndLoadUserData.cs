using WebApplication1.Repository;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Middleware
{
    public class CheckUserAuthAndLoadUserData
    {
        readonly IMemoryDB _memoryDB;
        readonly RequestDelegate _next;

        public CheckUserAuthAndLoadUserData(RequestDelegate next, IMemoryDB memoryDB)
        {
            _memoryDB = memoryDB;
            _next = next;
        }

        public async Task Invoke(HttpContext context) 
        {
            var formString = context.Request.Path.Value;

            if (string.Compare(formString, "/Login", StringComparison.OrdinalIgnoreCase) == 0 ||
            string.Compare(formString, "/CreateAccount", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await _next(context);
                return;
            }
        }
    }
}
