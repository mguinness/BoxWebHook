using Microsoft.AspNetCore.WebHooks.Internal;
using Microsoft.AspNetCore.WebHooks.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BoxMvcBuilderExtensions
    {
        public static IMvcBuilder AddBoxWebHooks(this IMvcBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            WebHookMetadata.Register<BoxMetadata>(builder.Services);

            BoxServiceCollectionSetup.AddBoxServices(builder.Services);

            return builder.AddWebHooks();
        }
    }
}
