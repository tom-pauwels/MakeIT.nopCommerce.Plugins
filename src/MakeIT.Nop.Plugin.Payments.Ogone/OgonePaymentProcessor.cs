using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Routing;
using MakeIT.Nop.Plugin.Payments.Ogone.Components;
using MakeIT.Nop.Plugin.Payments.Ogone.Controllers;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System.Security.Cryptography;
using Nop.Services.Common;
using Nop.Services.Security;

namespace MakeIT.Nop.Plugin.Payments.Ogone
{
	/// <summary>
	/// Ogone payment processor
	/// </summary>
	public class OgonePaymentProcessor : BasePlugin, IPaymentMethod
	{
		#region Fields

		private readonly OgonePaymentSettings _ogonePaymentSettings;
		private readonly ISettingService _settingService;
		private readonly ICurrencyService _currencyService;
		private readonly CurrencySettings _currencySettings;
		private readonly IWebHelper _webHelper;
		private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
		private readonly IOrderService _orderService;
		private readonly IEncryptionService _encryptionService;
	    private readonly IGenericAttributeService _genericAttributeService;
	    private readonly IWorkContext _workContext;
        
		#endregion

		public OgonePaymentProcessor(
			OgonePaymentSettings ogonePaymentSettings,
			ISettingService settingService, 
			ICurrencyService currencyService,
			CurrencySettings currencySettings, 
			IWebHelper webHelper,
            IStoreContext storeContext, 
			ILocalizationService localizationService,
 			IOrderService orderService,
			IEncryptionService encryptionService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
		{
			_ogonePaymentSettings = ogonePaymentSettings;
			_settingService = settingService;
			_currencyService = currencyService;
			_currencySettings = currencySettings;
			_webHelper = webHelper;
            _storeContext = storeContext;
			_localizationService = localizationService;
			_orderService = orderService;
			_encryptionService = encryptionService;
		    _genericAttributeService = genericAttributeService;
		    _workContext = workContext;
			_encryptionService = encryptionService;

#if DEBUG
			InstallLocaleResources();
#endif
		}

		#region Methods
        
	    /// <summary>
		/// Process a payment.
		/// </summary>
		/// <param name="processPaymentRequest">Payment info required for an order processing.</param>
		/// <returns>Process payment result.</returns>
		public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
		{
			return new ProcessPaymentResult
			{
				NewPaymentStatus = PaymentStatus.Pending
			};
		}

		/// <summary>
		/// Post process payment (used by payment gateways that require redirecting to a third-party URL).
		/// </summary>
		/// <param name="postProcessPaymentRequest">Payment info required for an order processing.</param>
		public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
		{
			// Store formatted order number in PurchaseOrderNumber field
			var order = postProcessPaymentRequest.Order;

            var remotePost = new RemotePost("OgoneForm", _ogonePaymentSettings.OgoneGatewayUrl);
            var orderID = string.IsNullOrEmpty(_ogonePaymentSettings.OrderIdPrefix) 
                                ? order.Id.ToString() : string.Format("{0}{1}", _ogonePaymentSettings.OrderIdPrefix, order.Id);

            remotePost.Add("PSPID", _ogonePaymentSettings.PSPId);
            remotePost.Add("orderID", orderID);
			remotePost.Add("amount", Convert.ToInt32(postProcessPaymentRequest.Order.OrderTotal * 100).ToString(CultureInfo.InvariantCulture));

			string currencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
			remotePost.Add("currency", currencyCode);

			string customerName = string.Format(
				"{0} {1}",
				postProcessPaymentRequest.Order.BillingAddress.FirstName,
				postProcessPaymentRequest.Order.BillingAddress.LastName);
			remotePost.Add("CN", customerName);

			string customerAddress = string.Format(
				"{0} {1}",
				postProcessPaymentRequest.Order.BillingAddress.Address1,
				postProcessPaymentRequest.Order.BillingAddress.Address2);
			remotePost.Add("owneraddress", customerAddress);
			remotePost.Add("EMAIL", postProcessPaymentRequest.Order.Customer.Email);
			remotePost.Add("ownerZIP", postProcessPaymentRequest.Order.BillingAddress.ZipPostalCode);
			remotePost.Add("language", _workContext.WorkingLanguage.LanguageCulture);
            remotePost.Add("TP", _ogonePaymentSettings.TemplateUrl);
            remotePost.Add("TITLE", _ogonePaymentSettings.TemplateTitle);
			remotePost.Add("BGCOLOR", _ogonePaymentSettings.BackgroundColor);
			remotePost.Add("TXTCOLOR", _ogonePaymentSettings.TextColor);
			remotePost.Add("TBLBGCOLOR", _ogonePaymentSettings.TableBackgroundColor);
			remotePost.Add("TBLTXTCOLOR", _ogonePaymentSettings.TableTextColor);
			remotePost.Add("BUTTONBGCOLOR", _ogonePaymentSettings.ButtonBackgroundColor);
			remotePost.Add("BUTTONTXTCOLOR", _ogonePaymentSettings.ButtonTextColor);
			remotePost.Add("FONTTYPE", _ogonePaymentSettings.FontFamily);
			remotePost.Add("LOGO", _ogonePaymentSettings.LogoUrl);

			//string storeUrl = _webHelper.GetStoreLocation(false);
            var storeUrl = _storeContext.CurrentStore.Url;
            if (!storeUrl.EndsWith("/"))
                storeUrl += "/";

            remotePost.Add("accepturl", storeUrl + "Plugins/PaymentOgone/AcceptPayment");
			remotePost.Add("cancelurl", storeUrl + "Plugins/PaymentOgone/CancelPayment");
			remotePost.Add("declineurl", storeUrl + "Plugins/PaymentOgone/DeclinePayment");
			remotePost.Add("exceptionurl", storeUrl + "Plugins/PaymentOgone/ExceptionPayment");

            if (!string.IsNullOrEmpty(_ogonePaymentSettings.ParamVar))
                remotePost.Add("PARAMVAR",  _ogonePaymentSettings.ParamVar);
            
            if (!string.IsNullOrEmpty(_ogonePaymentSettings.PmList))
                remotePost.Add("PMLIST", _ogonePaymentSettings.PmList); 
            
            if (!string.IsNullOrEmpty(_ogonePaymentSettings.ExclPmList))
                remotePost.Add("EXCLPMLIST", _ogonePaymentSettings.ExclPmList);

            var comParameter = order.GetAttribute<string>("COM", _storeContext.CurrentStore.Id);
            if (!string.IsNullOrEmpty(comParameter))
                remotePost.Add("COM", comParameter);

            var shaInPassPhrase = _encryptionService.DecryptText(_ogonePaymentSettings.SHAInPassPhrase);
		    
            var text2Hash = GetHashText(remotePost.SortedPostFields, shaInPassPhrase);
            var orderNoteText = GetHashText(remotePost.SortedPostFields, "+++HASHKEY+++");

            var shaSign = CalculateSHA(text2Hash);
            remotePost.Add("SHASIGN", shaSign);

            AddOrderNote(order, string.Format("Hashed Text:{0}\r\nSHA Digest:{1}", orderNoteText, shaSign), false);

			remotePost.Post();
		}

	    public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
	    {
	        return false;
	    }

	    /// <summary>
		/// Gets additional handling fee.
		/// </summary>
		/// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _ogonePaymentSettings.AdditionalFee;
        }

		/// <summary>
		/// Captures payment
		/// </summary>
		/// <param name="capturePaymentRequest">Capture payment request</param>
		/// <returns>Capture payment result</returns>
		public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
		{
			var result = new CapturePaymentResult();
			result.AddError("capture not supported.");
			return result;

		}

		/// <summary>
		/// Refunds a payment
		/// </summary>
		/// <param name="refundPaymentRequest">Request</param>
		/// <returns>Result</returns>
		public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
		{
			var result = new RefundPaymentResult();
			result.AddError("Refund method not supported");
			return result;
		}

		/// <summary>
		/// Voids a payment
		/// </summary>
		/// <param name="voidPaymentRequest">Request</param>
		/// <returns>Result</returns>
		public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
		{
			var result = new VoidPaymentResult();
			result.AddError("Void method not supported");
			return result;
		}

		/// <summary>
		/// Process recurring payment
		/// </summary>
		/// <param name="processPaymentRequest">Payment info required for an order processing</param>
		/// <returns>Process payment result</returns>
		public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
		{
			var result = new ProcessPaymentResult();
			result.AddError("recurring payment not supported.");

			return result;
		}

		/// <summary>
		/// Cancels a recurring payment
		/// </summary>
		/// <param name="cancelPaymentRequest">Request</param>
		/// <returns>Result</returns>
		public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
		{
			var result = new CancelRecurringPaymentResult();
			result.AddError("cancel recurring payment not supported.");
			return result;
		}

	    public bool CanRePostProcessPayment(Order order)
	    {
	        return false;
	    }

	    /// <summary>
		/// Gets a route for provider configuration
		/// </summary>
		/// <param name="actionName">Action name</param>
		/// <param name="controllerName">Controller name</param>
		/// <param name="routeValues">Route values</param>
		public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "Configure";
			controllerName = "PaymentOgone";
			routeValues = new RouteValueDictionary() { { "Namespaces", "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" }, { "area", null } };
		}

		/// <summary>
		/// Gets a route for payment info
		/// </summary>
		/// <param name="actionName">Action name</param>
		/// <param name="controllerName">Controller name</param>
		/// <param name="routeValues">Route values</param>
		public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "PaymentInfo";
			controllerName = "PaymentOgone";
			routeValues = new RouteValueDictionary() { { "Namespaces", "MakeIT.Nop.Plugin.Payments.Ogone.Controllers" }, { "area", null } };
		}

		/// <summary>
		/// Verify the hash digest in the request string.
		/// </summary>
		/// <param name="values">Request string key/value pairs.</param>
		/// <returns></returns>
		public bool VerifyHashDigest(SortedDictionary<string, string> values,string digest)
		{
		    var hashText = GetHashText(values, _encryptionService.DecryptText(_ogonePaymentSettings.SHAOutPassPhrase));
			var calcDigest = CalculateSHA(hashText);
            return calcDigest.Equals(digest);
		}

		public Type GetControllerType()
		{
			return typeof(PaymentOgoneController);
		}

		public override void Install()
		{
			var settings = new OgonePaymentSettings()
			{
				OgoneGatewayUrl = "https://secure.ogone.com/ncol/test/orderstandard.asp",
                HashingAlgorithm = HashingAlgorithm.Sha1,
                HashAllParameters = true
			};
			_settingService.SaveSetting(settings);

			InstallLocaleResources();

			base.Install();
		}

		#endregion

		#region Utilities

		private void InstallLocaleResources()
		{
			//InsertLocaleresource(1, "MakeIT.Nop.Plugin.Payments.Ogone.CancelPaymentTitle", "Payment Cancelled (GB)");
			//InsertLocaleresource(1, "MakeIT.Nop.Plugin.Payments.Ogone.CancelPaymentText", "Payment Cancelled Text (GB)");
			//InsertLocaleresource(2, "MakeIT.Nop.Plugin.Payments.Ogone.CancelPaymentTitle", "Payment Cancelled (NL)");
			//InsertLocaleresource(2, "MakeIT.Nop.Plugin.Payments.Ogone.CancelPaymentText", "Payment Cancelled Text (NL)");
			//InsertLocaleresource(3, "MakeIT.Nop.Plugin.Payments.Ogone.CancelPaymentTitle", "Payment Cancelled (FR)");
			//InsertLocaleresource(3, "MakeIT.Nop.Plugin.Payments.Ogone.CancelPaymentText", "Payment Cancelled Text (FR)");
		}

		private void InsertLocaleresource(int languageId, string resourceName, string resourceValue)
		{
			if (_localizationService.GetLocaleStringResourceByName(resourceName, languageId, false) == null)
			{
				_localizationService.InsertLocaleStringResource(
					new LocaleStringResource()
					{
						LanguageId = languageId,
						ResourceName = resourceName,
						ResourceValue = resourceValue
					});
			}
		}

		private string CalculateSHA(string hashText)
		{
		    var sha = new SHA1CryptoServiceProvider();
            var utf8Bytes = sha.ComputeHash(new UTF8Encoding().GetBytes(hashText));

            return BitConverter.ToString(utf8Bytes).Replace("-", string.Empty);
		}

        private string GetHashText(SortedDictionary<string, string> formFields, string hashPassPhrase)
        {
            var sb = new StringBuilder();

            foreach (var key in formFields.Keys)
            {
                if (!string.IsNullOrEmpty(formFields[key]))
                    sb.AppendFormat("{0}={1}{2}", key.ToUpper(), formFields[key], hashPassPhrase);
            }

            return sb.ToString();
        }

        private void AddOrderNote(Order order, string noteText, bool displayToCustomer)
        {
            // Add ordernote
            order.OrderNotes.Add(new OrderNote()
            {
                Note = noteText,
                DisplayToCustomer = displayToCustomer,
                CreatedOnUtc = DateTime.UtcNow
            });

            _orderService.UpdateOrder(order);
        }

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether capture is supported.
		/// </summary>
		public bool SupportCapture
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether partial refund is supported.
		/// </summary>
		public bool SupportPartiallyRefund
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether refund is supported.
		/// </summary>
		public bool SupportRefund
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether void is supported.
		/// </summary>
		public bool SupportVoid
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a recurring payment type of payment method
		/// </summary>
		public RecurringPaymentType RecurringPaymentType
		{
			get
			{
				return RecurringPaymentType.NotSupported;
			}
		}

		/// <summary>
		/// Gets a payment method type
		/// </summary>
		public PaymentMethodType PaymentMethodType
		{
			get
			{
				return PaymentMethodType.Redirection;
			}
		}

	    public bool SkipPaymentInfo {
	        get { return true; }
	    }

	    #endregion

	}
}
