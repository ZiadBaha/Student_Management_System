namespace SMS.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var path = context.Request.Path;
            var method = context.Request.Method;
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            _logger.LogInformation($"[{DateTime.UtcNow}] {method} {path} from {ip} - UA: {userAgent}");

            await _next(context);
        }
    }
}
