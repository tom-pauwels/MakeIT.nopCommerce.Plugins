using System;
using Nop.Core.Configuration;

namespace MakeIT.Nop.Plugin.Payments.Ogone
{
	public class OgonePaymentSettings : ISettings
	{
		public string PSPId { get; set; }

		public string SHAInPassPhrase { get; set; }

		public string SHAOutPassPhrase { get; set; }

		public bool HashAllParameters { get; set; }

        public string ParamVar { get; set; }

        public string PmList { get; set; }

        public string ExclPmList { get; set; }

	    public string OrderIdPrefix { get; set; }

	    public HashingAlgorithm HashingAlgorithm { get; set; }

        public string TemplateUrl { get; set; }
        
        public string TemplateTitle { get; set; }

		public string OgoneGatewayUrl { get; set; }

		public string BackgroundColor { get; set; }

		public string TextColor { get; set; }

		public string TableBackgroundColor { get; set; }

		public string TableTextColor { get; set; }

		public string ButtonBackgroundColor { get; set; }

		public string ButtonTextColor { get; set; }

		public string FontFamily { get; set; }

		public string LogoUrl { get; set; }

		public string AcceptUrl { get; set; }

		public decimal AdditionalFee { get; set; }
	}
}
