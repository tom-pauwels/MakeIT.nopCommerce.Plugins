using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager
{
    public class OrderPaidEventHandler : IConsumer<OrderPaidEvent>
    {
        public void HandleEvent(OrderPaidEvent eventMessage)
        {

        }
    }
}