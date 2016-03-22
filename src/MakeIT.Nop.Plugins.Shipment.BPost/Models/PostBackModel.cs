using System;
using Nop.Core.Domain.Catalog;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager.Models
{
    public class PostBackModel
    {
        public string OrderReference { get; set; } 
        public int OrderTotalPrice { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerMemberId { get; set; }
        public string CustomerStreet { get; set; }
        public string CustomerStreetNumber { get; set; }
        public string CustomerBox { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerPostalCode { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerPostalLocation { get; set; }
        public string CustomerRcCode { get; set; }
        public string DeliveryDate { get; set; }
        public string OrderLine { get; set; }
        public int OrderWeight { get; set; }
        public string Extra { get; set; }
        public string ExtraSecure { get; set; }
        public string DeliveryMethod { get; set; }
        public int DeliveryMethodPriceDefault { get; set; }
        public int DeliveryMethodPriceOverride { get; set; }
        public int DeliveryMethodPriceTotal { get; set; }

        public decimal DeliveryMethodPriceTotalEuro
        {
            get { return decimal.Divide(DeliveryMethodPriceTotal,100); }
        }
    }
}