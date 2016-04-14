using Nop.Core.Configuration;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager
{
    public class BpostShippingManagerSettings : ISettings
    {
        public string AccountId { get; set; }
        public string PassPhrase { get; set; }
        public string ButtonCssClass { get; set; }
        public decimal Standardprice { get; set; }
        public bool DoRefresh { get; set; }
    }
}