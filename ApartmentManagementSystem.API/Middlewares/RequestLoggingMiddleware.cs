namespace ApartmentManagementSystem.API.Middlewares
{
   // namespace ApartmentManagementSystem.API.Middlewares;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] {context.Request.Method} {context.Request.Path}");
            await _next(context);
        }
    }
}