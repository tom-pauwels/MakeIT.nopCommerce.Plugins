using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Events;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager
{
    public class OrderPlacedEventHandler : IConsumer<OrderPlacedEvent>
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;

        public OrderPlacedEventHandler(
            IWorkContext workContext, IStoreContext storeContext,
            IGenericAttributeService genericAttributeService)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            _genericAttributeService.SaveAttribute<decimal>(_workContext.CurrentCustomer, CustomCustomerAttributeNames.DeliveryMethodRate, 0, _storeContext.CurrentStore.Id);
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, CustomCustomerAttributeNames.DeliveryMethodAddress, "", _storeContext.CurrentStore.Id);
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, CustomCustomerAttributeNames.DeliveryMethod, "", _storeContext.CurrentStore.Id);
        }
    }
}