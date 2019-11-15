using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.WebHooks.Metadata
{
    public class BoxMetadata : WebHookMetadata, IWebHookBodyTypeMetadataService, IWebHookGetHeadRequestMetadata
    {
        public BoxMetadata() : base(BoxConstants.ReceiverName)
        {
        }

        public override WebHookBodyType BodyType => WebHookBodyType.Json;

        public bool AllowHeadRequests => false;

        public string ChallengeQueryParameterName => null;

        public int SecretKeyMinLength => BoxConstants.SecretKeyMinLength;

        public int SecretKeyMaxLength => BoxConstants.SecretKeyMaxLength;
    }
}
