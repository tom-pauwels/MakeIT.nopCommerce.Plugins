using Nop.Core.Domain.Payments;

namespace MakeIT.Nop.Plugin.Payments.Ogone
{
	public class OgoneHelper
	{
        private const string STATUS_AUTHORIZED = "5";
        private const string STATUS_PAYMENT_REQUESTED = "9";
        private const string STATUS_AUTHORIZATION_WAITING = "51";
        private const string STATUS_PAYMENT_PROCESSING = "91";

        private const string STATUS_CANCELLEDBYCLIENT = "1";
        private const string STATUS_CANCELLEDBYMERCHANT = "6";

        private const string STATUS_AUTHORIZATION_REFUSED = "2";
        private const string STATUS_PAYMENT_REFUSED = "93";

		private const string STATUS_INVALID_OR_INCOMPLETE = "0";

        private const string STATUS_AUTHORIZATION_NOTKNOWN = "52";
		private const string STATUS_PAYMENT_UNCERTAIN = "92";

		public static PaymentStatus GetPaymentStatus(string status, string error)
		 {
			 switch (status)
			 {
				 case STATUS_AUTHORIZED:
			 		return PaymentStatus.Authorized;

				 case STATUS_PAYMENT_REQUESTED:
					return PaymentStatus.Paid;

				 case STATUS_PAYMENT_REFUSED:
				 case STATUS_INVALID_OR_INCOMPLETE:
				 case STATUS_AUTHORIZATION_WAITING:
				 case STATUS_PAYMENT_PROCESSING:
				 case STATUS_AUTHORIZATION_NOTKNOWN:
				 case STATUS_PAYMENT_UNCERTAIN:
				 case STATUS_AUTHORIZATION_REFUSED:
					return PaymentStatus.Pending;

                 case STATUS_CANCELLEDBYCLIENT:
                 case STATUS_CANCELLEDBYMERCHANT:
                    return PaymentStatus.Voided;
			 }

		 	return PaymentStatus.Pending;
		 }
	}
}