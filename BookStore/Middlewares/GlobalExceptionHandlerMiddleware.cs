using BookStore.BLL.IServices;
using BookStore.Common.Exceptions;

namespace BookStore.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            try
            {
                return _next(context);
            }
            catch (ShoppingCartsAppException ex)
            {
                switch (ex)
                {
                    case EntityNotFoundException entityNotFoundException:
                        context.Response.StatusCode = 404;
                        return context.Response.WriteAsync(entityNotFoundException.Message);
                    case FailedToRetrieveShoppingCartsException failedToRetrieveShoppingCartsException:
                        context.Response.StatusCode = 400;
                        return context.Response.WriteAsync(failedToRetrieveShoppingCartsException.Message);
                    default:
                        context.Response.StatusCode = 500;
                        return context.Response.WriteAsync(ex.Message);
                }
            }
            catch (Exception ex)
            {
                var logger = context.RequestServices.GetRequiredService<ILoggerService>();
                logger.LogError(ex);
                context.Response.StatusCode = 500;
                return context.Response.WriteAsync("Internal server error");
            }
        }
    }
}
