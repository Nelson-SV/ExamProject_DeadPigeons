namespace api.Middleware;

public class Middleware(RequestDelegate next, ILogger<Middleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        try
        {
            await LogRequest(context.Request);

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await next(context);

                await LogResponse(context.Response, responseBody);

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        catch (ApplicationException ex)
        {
            await HandleApplicationExceptionAsync(context, ex, originalBodyStream);
        }
        catch (Exception ex)
        {
            await HandleServerErrorAsync(context, ex, originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream; // Restore in case of exceptions
        }
    }
    private async Task HandleServerErrorAsync(HttpContext context, Exception exception, Stream originalBodyStream)
    {
        context.Response.Body = originalBodyStream; // Restore the original stream
        context.Response.Clear(); 
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        logger.LogError(exception, "An unexpected error occurred.");
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later. " + exception.Message);
    }


    private async Task HandleApplicationExceptionAsync(HttpContext context, ApplicationException exception, Stream originalBodyStream)
    {
        context.Response.Body = originalBodyStream; // Restore the original stream
        context.Response.Clear(); 
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync(exception.Message);
    }

    private async Task LogRequest(HttpRequest request)
    {
        request.EnableBuffering();

        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;

        var message = $"\n\nHTTP Request Information:{Environment.NewLine}" +
                      $"Schema: {request.Scheme}{Environment.NewLine}" +
                      $"Host: {request.Host}{Environment.NewLine}" +
                      $"Path: {request.Path}{Environment.NewLine}" +
                      $"QueryString: {request.QueryString}{Environment.NewLine}" +
                      $"Request Body: {requestBody}";

        logger.LogInformation(message);
    }

    private async Task LogResponse(HttpResponse response, MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();

        var message = $"HTTP Response Information:{Environment.NewLine}" +
                      $"StatusCode: {response.StatusCode}{Environment.NewLine}" +
                      $"Response Body: {responseBodyText}\n\n";

        logger.LogInformation(message);
    }
}
