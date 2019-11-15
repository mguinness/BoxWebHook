using Microsoft.AspNetCore.WebHooks.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.WebHooks
{
    public class BoxWebHookAttribute : WebHookAttribute, IWebHookBodyTypeMetadata, IWebHookEventSelectorMetadata
    {
        public BoxWebHookAttribute() : base(BoxConstants.ReceiverName)
        {
        }

        public WebHookBodyType? BodyType => WebHookBodyType.Json;

        public string EventName { get; set; }
    }
}
