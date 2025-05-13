using BookStore.DTO;
using System.Text.Json;

namespace BookStore.Exceptions
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (NotFoundException ex)
            {
                await HandleException(httpContext, 404, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await HandleException(httpContext, 403, ex.Message);
            }
            catch (ValidationException ex)
            {
                await HandleException(httpContext, 400, ex.Message);
            }
            catch (ConflictException ex)
            {
                await HandleException(httpContext, 409, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, 500, "Internal Server Error");
            }

        }

        private async Task HandleException(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new BaseResponse<string>(
                statusCode,
                false,
                message,
                null
            );
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}