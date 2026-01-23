namespace ApartmentManagementSystem.API.Middlewares
{
   // namespace ApartmentManagementSystem.API.Middlewares;

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate Next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] {context.Request.Method} {context.Request.Path}");
            await Next(context);
        }
    }
}