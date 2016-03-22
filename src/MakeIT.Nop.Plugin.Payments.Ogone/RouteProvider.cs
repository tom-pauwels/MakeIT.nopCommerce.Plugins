using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace MakeIT.Nop.Plugin.Payments.Ogone
{
	public partial class RouteProvider : IRouteProvider
	{
		public void RegisterRoutes(RouteCollection routes)
		{
            //routes.MapRoute(
            //    "Plugin.Payments.Ogone.Configure",
            //    "Plugins/PaymentOgone/Configure",
            //    new { controller = "PaymentOgone", action = "Configure" },
            //    new[] { "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" });

			routes.MapRoute(
				"Plugin.Payments.Ogone.PaymentInfo",
				"Plugins/PaymentOgone/PaymentInfo",
				new { controller = "PaymentOgone", action = "PaymentInfo" },
				new[] { "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" });

			// Accept
			routes.MapRoute(
				"Plugin.Payments.Ogone.AcceptPayment",
				"Plugins/PaymentOgone/AcceptPayment",
				new { controller = "PaymentOgone", action = "AcceptPayment" },
				new[] { "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" });

			// Cancel
			routes.MapRoute(
				"Plugin.Payments.Ogone.CancelPayment",
				"Plugins/PaymentOgone/CancelPayment",
				new { controller = "PaymentOgone", action = "CancelPayment" },
				new[] { "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" });

			// Decline
			routes.MapRoute(
				"Plugin.Payments.Ogone.DeclinePayment",
				"Plugins/PaymentOgone/DeclinePayment",
				new { controller = "PaymentOgone", action = "DeclinePayment" },
				new[] { "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" });

			// Exception
			routes.MapRoute(
				"Plugin.Payments.Ogone.ExceptionPayment",
				"Plugins/PaymentOgone/ExceptionPayment",
				new { controller = "PaymentOgone", action = "ExceptionPayment" },
				new[] { "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" });

			// Postback
			routes.MapRoute(
				"Plugin.Payments.Ogone.PostBackHandler",
				"Plugins/PaymentOgone/PostBackHandler",
				new { controller = "PaymentOgone", action = "PostBackHandler" },
				new[] { "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" });
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
