using Cbw;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace Cbw.Sever
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute("cbwRoute", "cbw", GetModel());
            config.EnsureInitialized();
        }

        private static Microsoft.OData.Edm.IEdmModel GetModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder
            {
                Namespace = "Cbw",
                ContainerName = "CbwContainer"
            };
            //builder.EnumType<DisplayMode>();
            //builder.ComplexType<CaptionConfig>();
            //builder.EntityType<Channel>();

            builder.EntitySet<Channel>("Channels");
            return builder.GetEdmModel();
        }
    }
}
