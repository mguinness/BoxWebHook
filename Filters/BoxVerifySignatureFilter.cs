using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.WebHooks.Filters
{
    public class BoxVerifySignatureFilter : WebHookVerifySignatureFilter, IAsyncResourceFilter
    {
        private readonly IWebHookRequestReader _requestReader;

        public BoxVerifySignatureFilter(IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory, IWebHookRequestReader requestReader)
            : base(configuration, hostingEnvironment, loggerFactory)
        {
            _requestReader = requestReader;
        }

        public override string ReceiverName => BoxConstants.ReceiverName;

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (next == null)
                throw new ArgumentNullException(nameof(next));

            var routeData = context.RouteData;
            var request = context.HttpContext.Request;

            if (routeData.TryGetWebHookReceiverName(out var receiverName) && IsApplicable(receiverName) && HttpMethods.IsPost(request.Method))
            {
                var errorResult = EnsureSecureConnection(ReceiverName, context.HttpContext.Request);
                if (errorResult != null)
                {
                    context.Result = errorResult;
                    return;
                }
            }

            var secretKey = GetSecretKey(ReceiverName, routeData, BoxConstants.SecretKeyMinLength, BoxConstants.SecretKeyMaxLength);
            if (secretKey == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            var deliveryTimestamp = GetRequestHeader(request, BoxConstants.TimestampHeaderName, out IActionResult errorResultTimestamp);
            if (errorResultTimestamp != null)
            {
                context.Result = errorResultTimestamp;
                return;
            }

            //https://developer.box.com/en/guides/webhooks/handle/verify-signatures/
            var secret = Encoding.UTF8.GetBytes(secretKey);
            var suffix = Encoding.UTF8.GetBytes(deliveryTimestamp);
            var actualHash = await ComputeRequestBodySha256HashAsync(request, secret, new byte[0], suffix);

            var expectedHashPrimary = FromBase64(request.Headers[BoxConstants.PrimaryHashHeaderName], BoxConstants.PrimaryHashHeaderName);
            var expectedHashSecondary = new byte[0]; //header not always present

            if (request.Headers[BoxConstants.SecondaryHashHeaderName].Count > 0)
                expectedHashSecondary = FromBase64(request.Headers[BoxConstants.SecondaryHashHeaderName], BoxConstants.SecondaryHashHeaderName);

            if (!SecretEqual(actualHash, expectedHashPrimary) && !SecretEqual(actualHash, expectedHashSecondary))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var data = await _requestReader.ReadBodyAsync<JObject>(context);

            var eventName = data.Value<string>(BoxConstants.EventPropertyName);
            routeData.Values[WebHookConstants.EventKeyName] = eventName;

            var idName = data["source"].Value<string>(BoxConstants.IdPropertyName);
            routeData.Values[WebHookConstants.IdKeyName] = idName;

            await next();
        }
    }
}
