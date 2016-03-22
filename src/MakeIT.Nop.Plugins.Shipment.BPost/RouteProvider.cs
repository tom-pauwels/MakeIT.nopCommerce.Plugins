using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            // Confirm Postback
            routes.MapRoute(
                "Plugin.Shipping.Bpost.ShippingManager.ConfirmHandler",
                "Plugins/BpostShippingManager/ConfirmHandler",
                new { controller = "BPostShippingManager", action = "ConfirmHandler" },
                new[] { "MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager.Controllers" });

            // Cancel Postback
            routes.MapRoute(
                "Plugin.Shipping.Bpost.ShippingManager.CancelHandler",
                "Plugins/BpostShippingManager/CancelHandler",
                new { controller = "BPostShippingManager", action = "CancelHandler" },
                new[] { "MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager.Controllers" });

            // Error Postback
            routes.MapRoute(
                "Plugin.Shipping.Bpost.ShippingManager.ErrorHandler",
                "Plugins/BpostShippingManager/ErrorHandler",
                new { controller = "BPostShippingManager", action = "ErrorHandler" },
                new[] { "MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager.Controllers" });
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
