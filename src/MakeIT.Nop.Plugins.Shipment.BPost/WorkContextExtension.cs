using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager
{
    public static class WorkContextExtension
    {
        public static ShippingOption GetBpostShippingOption(this IWorkContext workContext, IStoreContext storeContext)
        {
            var shippingOptions =
                workContext.CurrentCustomer.GetAttribute<List<ShippingOption>>(
                    SystemCustomerAttributeNames.OfferedShippingOptions, storeContext.CurrentStore.Id);

            return
                shippingOptions?.FirstOrDefault(
                    so => so.ShippingRateComputationMethodSystemName.Equals("Shipping.Bpost.ShippingManager", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}