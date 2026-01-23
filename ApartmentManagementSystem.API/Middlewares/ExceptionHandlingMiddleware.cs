namespace ApartmentManagementSystem.API.Middlewares
{
  //  namespace ApartmentManagementSystem.API.Middlewares;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate Next;

        public ExceptionHandlingMiddleware(RequestDelegate next) => Next = next;

        public async Task Invoke(HttpContext context)
        {
            try { await Next(context); }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
        }
    }
}