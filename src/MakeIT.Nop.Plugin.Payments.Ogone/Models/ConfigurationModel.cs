using System.ComponentModel;
using System.Web.Mvc;
using Nop.Web.Framework.Mvc;

namespace MakeIT.Nop.Plugin.Payments.Ogone.Models
{
	public class ConfigurationModel : BaseNopModel
	{
        public int ActiveStoreScopeConfiguration { get; set; }

		[DisplayName("Ogone PSPID")]
		public string PSPId { get; set; }
        public bool PSPId_OverrideForStore { get; set; }

		[DisplayName("SHA-In Pass Phrase")]
		public string SHAInPassPhrase { get; set; }
        public bool SHAInPassPhrase_OverrideForStore { get; set; }

		[DisplayName("SHA-Out Pass Phrase")]
		public string SHAOutPassPhrase { get; set; }
        public bool SHAOutPassPhrase_OverrideForStore { get; set; }

		[DisplayName("Hash All Parameters")]
        public bool HashAllParameters { get; set; }
        public bool HashAllParameters_OverrideForStore { get; set; }

		public int HashingAlgorithmId { get; set; }
        public bool HashingAlgorithmId_OverrideForStore { get; set; }

		[DisplayNameAttribute("Hashing Algorithm")]
		public SelectList HashingAlgorithmValues { get; set; }

		[DisplayName("Ogone Gateway Url")]
		public string OgoneGatewayUrl { get; set; }
        public bool OgoneGatewayUrl_OverrideForStore { get; set; }

        [DisplayName("Ogone ID Prefix")]
        public string OrderIdPrefix { get; set; }
        public bool OrderIdPrefix_OverrideForStore { get; set; }

        [DisplayName("Template Url (TP)")]
        public string TemplateUrl { get; set; }
        public bool TemplateUrl_OverrideForStore { get; set; }

		[DisplayName("Template Title")]
		public string TemplateTitle { get; set; }
        public bool TemplateTitle_OverrideForStore { get; set; }

		[DisplayName("Background Color")]
		public string BackgroundColor { get; set; }
        public bool BackgroundColor_OverrideForStore { get; set; }

		[DisplayName("Text Color")]
		public string TextColor { get; set; }
        public bool TextColor_OverrideForStore { get; set; }

		[DisplayName("Table Background Color")]
		public string TableBackgroundColor { get; set; }
        public bool TableBackgroundColor_OverrideForStore { get; set; }

		[DisplayName("Table Text Color")]
		public string TableTextColor { get; set; }
        public bool TableTextColor_OverrideForStore { get; set; }

		[DisplayName("Button Background Color")]
		public string ButtonBackgroundColor { get; set; }
        public bool ButtonBackgroundColor_OverrideForStore { get; set; }

		[DisplayName("Button Text Color")]
		public string ButtonTextColor { get; set; }
        public bool ButtonTextColor_OverrideForStore { get; set; }

		[DisplayName("Font Family")]
		public string FontFamily { get; set; }
        public bool FontFamily_OverrideForStore { get; set; }

		[DisplayName("Logo Url")]
		public string LogoUrl { get; set; }
        public bool LogoUrl_OverrideForStore { get; set; }

		[DisplayName("Additional fee")]
		public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [DisplayName("PARAMVAR parameter")]
        public string ParamVar { get; set; }
        public bool ParamVar_OverrideForStore { get; set; }

        [DisplayName("PMLIST parameter")]
        public string PmList { get; set; }
        public bool PmList_OverrideForStore { get; set; }

        [DisplayName("EXCLPMLIST parameter")]
        public string ExclPmList { get; set; }
        public bool ExclPmList_OverrideForStore { get; set; }
    }
}