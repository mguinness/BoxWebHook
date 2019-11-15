using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.WebHooks
{
    public class BoxConstants
    {
        public static string ReceiverName => "box";
        public static int SecretKeyMinLength => 1;
        public static int SecretKeyMaxLength => 100;
        public static string TimestampHeaderName => "box-delivery-timestamp";
        public static string PrimaryHashHeaderName => "box-signature-primary";
        public static string SecondaryHashHeaderName => "box-signature-secondary";
        public static string IdPropertyName = "id";
        public static string EventPropertyName = "trigger";
    }
}
