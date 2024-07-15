using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace CoreWebApi.Authorized
{
    public class EOAuthorizeAttribute : TypeFilterAttribute
    {
        public EOAuthorizeAttribute() : base(typeof(AddRandomNumberHeaderFilter))
        {
        }
    }

    public class AddRandomNumberHeaderFilter : IEOAuthorizeService
    {
        private readonly IEOAuthorizeService _authService;

        public AddRandomNumberHeaderFilter(IEOAuthorizeService authService)
        {
            _authService = authService;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }
            if (!IsApiAuthorized(actionContext, out var errorMessage))
            {
                actionContext.Result = new UnauthorizedObjectResult(errorMessage);
            }
        }

        public bool IsApiAuthorized(ResultExecutingContext actionContext, out string errorMessage)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            //var hasApiKey = actionContext.HttpContext.Headers.TryGetValues("apikey", out var apiKeys);

            var hasApiKey = actionContext.HttpContext.Request.Headers.TryGetValue("apiKey", out var apiKeys);

            if (!hasApiKey || StringValues.IsNullOrEmpty(apiKeys) || !apiKeys.Any())
            {
                errorMessage = GetErrorMessage();
                UnAuthorizeLogging(actionContext, errorMessage);
                return false;
            }

            var apiKey = apiKeys.FirstOrDefault();
            try
            {
                var isAuthorized = _authService.IsApiAuthorized(apiKey);
                errorMessage = GetErrorMessage(apiKey);
                if (!isAuthorized)
                {
                    UnAuthorizeLogging(actionContext, errorMessage);
                }

                return isAuthorized;

            }
            catch (Exception ex)
            {
                errorMessage = GetErrorMessage(apiKey);
                UnAuthorizeLogging(actionContext, errorMessage);
                return false;
            }
        }

        private void UnAuthorizeLogging(ResultExecutingContext actionContext, string message)
        {
            var request = actionContext.HttpContext.Request;
            var uri = GetUri(request);
            var requestUri = request.Path;
            var requestUriBase = request.PathBase;

            //var request = actionContext.HttpContext.Request.
            //var requestUri = actionContext.Request.RequestUri;
            //var absolutePath = requestUri.AbsolutePath; // /WebSite1test/Default2.aspx
            //var absoluteUri = actionContext.Request.RequestUri.AbsoluteUri;

            //_ieoLogging.Error(HttpStatusCode.Unauthorized, GetMethodName(absolutePath), absoluteUri, message);
        }

        public static Uri GetUri(HttpRequest request)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port.GetValueOrDefault(80),
                Path = request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            return uriBuilder.Uri;
        }

        private string GetErrorMessage(string apiKey = null)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return "The API Key must be provided";
            }

            return $"The API Key {apiKey} is invalid";
        }

        private string GetMethodName(string absolutePath)
        {
            //var methodName = absolutePath.Split('/').LastOrDefault();
            //if (methodName == null) return absolutePath;

            //var matchKey = _methodNamesDictionary.Keys.FirstOrDefault(x => x.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));
            //return matchKey != null ? _methodNamesDictionary[matchKey] : absolutePath;

            return string.Empty;
        }

        public async Task OnActionExecutionAsync(ResultExecutingContext actionContext, ActionExecutionDelegate next)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (!IsApiAuthorized(actionContext, out var errorMessage))
            {
                actionContext.Result = new UnauthorizedObjectResult(errorMessage);
            }
        }

        public bool IsApiAuthorized(string apiKey)
        {
            return apiKey == "ABC";
        }

    }

}