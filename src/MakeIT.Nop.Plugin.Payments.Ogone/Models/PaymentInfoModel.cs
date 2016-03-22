using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Mvc;

namespace MakeIT.Nop.Plugin.Payments.Ogone.Models
{
    public class PaymentInfoModel : BaseNopModel
	{
		public Customer Customer { get; set; }

		public string PSPId { get; set; }

		public string SHASign { get; set; }

		public string AcceptUrl { get; set; }

		public string ExceptionUrl { get; set; }
	
		public string DeclineUrl { get; set; }
		
		public string CancelUrl { get; set; }

		public string PaymentUrl { get; set; }
		
		public decimal AdditionalFee { get; set; }

		public string PageTitle { get; set; }
		
		public string PageBackgroundColor { get; set; }
		
		public string PageTextColor { get; set; }
		
		public string PageTableBackgroundColor { get; set; }
		
		public string PageTableTextColor { get; set; }
		
		public string PageButtonBackgroundColor { get; set; }
		
		public string PageButtonTextColor { get; set; }
		
		public string PageFont { get; set; }
		
		public string PageUrlLogo { get; set; }

		public string OrderId { get; set; }
		
		public int Amount { get; set; }
		
		public string Language { get; set; }
		
		public string Currency { get; set; }
	}
}