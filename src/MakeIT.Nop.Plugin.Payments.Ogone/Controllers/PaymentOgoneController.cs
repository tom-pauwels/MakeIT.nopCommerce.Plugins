using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using MakeIT.Nop.Plugin.Payments.Ogone.Models;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace MakeIT.Nop.Plugin.Payments.Ogone.Controllers
{
	public class PaymentOgoneController : BasePaymentController
	{
		private readonly ISettingService _settingService;
		private readonly IPaymentService _paymentService;
		private readonly IOrderProcessingService _orderProcessingService;
		private readonly ILogger _logger;
		private readonly IOrderService _orderService;
		private readonly IEncryptionService _encryptionService;
		private readonly PaymentSettings _paymentSettings;
	    private readonly IWorkContext _workContext;
	    private readonly IStoreService _storeService;

	    public PaymentOgoneController(
            IWorkContext workContext,
            IStoreService storeService, 
			ISettingService settingService,
			IPaymentService paymentService,
			PaymentSettings paymentSettings,
			IOrderProcessingService orderProcessingService,
			ILogger logger,
			IOrderService orderService, 
			IEncryptionService encryptionService)
		{
            _workContext = workContext;
            _storeService = storeService;
			_settingService = settingService;
			_orderProcessingService = orderProcessingService;
			_logger = logger;
			_orderService = orderService;
			_paymentService = paymentService;
			_paymentSettings = paymentSettings;
			_encryptionService = encryptionService;
		}

		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure()
		{
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ogonePaymentSettings = _settingService.LoadSetting<OgonePaymentSettings>(storeScope);

			var model = new ConfigurationModel();
			model.PSPId = ogonePaymentSettings.PSPId;
			model.SHAInPassPhrase = _encryptionService.DecryptText(ogonePaymentSettings.SHAInPassPhrase);
			model.SHAOutPassPhrase = _encryptionService.DecryptText(ogonePaymentSettings.SHAOutPassPhrase);
			model.AdditionalFee = ogonePaymentSettings.AdditionalFee;
			model.HashAllParameters = ogonePaymentSettings.HashAllParameters;
			model.HashingAlgorithmId = Convert.ToInt32(ogonePaymentSettings.HashingAlgorithm);
			model.HashingAlgorithmValues = ogonePaymentSettings.HashingAlgorithm.ToSelectList();
			model.OgoneGatewayUrl = ogonePaymentSettings.OgoneGatewayUrl;
            model.TemplateUrl = ogonePaymentSettings.TemplateUrl;
            model.TemplateTitle = ogonePaymentSettings.TemplateTitle;
			model.BackgroundColor = ogonePaymentSettings.BackgroundColor;
			model.TextColor = ogonePaymentSettings.TextColor;
			model.TableBackgroundColor = ogonePaymentSettings.TableBackgroundColor;
			model.TableTextColor = ogonePaymentSettings.TableTextColor;
			model.ButtonBackgroundColor = ogonePaymentSettings.ButtonBackgroundColor;
			model.ButtonTextColor = ogonePaymentSettings.ButtonTextColor;
			model.FontFamily = ogonePaymentSettings.FontFamily;
			model.LogoUrl = ogonePaymentSettings.LogoUrl;
			model.ParamVar = ogonePaymentSettings.ParamVar;
		    model.OrderIdPrefix = ogonePaymentSettings.OrderIdPrefix;
            model.PmList = ogonePaymentSettings.PmList;
            model.ExclPmList = ogonePaymentSettings.ExclPmList;
            
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.PSPId_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.PSPId, storeScope);
                model.SHAInPassPhrase_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.SHAInPassPhrase, storeScope);
                model.SHAOutPassPhrase_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.SHAOutPassPhrase, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.AdditionalFee, storeScope);
                model.HashAllParameters_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.HashAllParameters, storeScope);
                model.HashingAlgorithmId_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.HashingAlgorithm, storeScope);
                model.OgoneGatewayUrl_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.OgoneGatewayUrl, storeScope);
                model.TemplateUrl_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.TemplateUrl, storeScope);
                model.TemplateTitle_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.TemplateTitle, storeScope);
                model.BackgroundColor_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.BackgroundColor, storeScope);
                model.TextColor_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.TextColor, storeScope);
                model.TableBackgroundColor_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.TableBackgroundColor, storeScope);
                model.TableTextColor_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.TableTextColor, storeScope);
                model.ButtonBackgroundColor_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.ButtonBackgroundColor, storeScope);
                model.ButtonTextColor_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.ButtonTextColor, storeScope);
                model.FontFamily_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.FontFamily, storeScope);
                model.LogoUrl_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.LogoUrl, storeScope);
                model.ParamVar_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.ParamVar, storeScope);
                model.OrderIdPrefix_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.OrderIdPrefix, storeScope);
                model.PmList_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.PmList, storeScope);
                model.ExclPmList_OverrideForStore = _settingService.SettingExists(ogonePaymentSettings, x => x.ExclPmList, storeScope);
            }

            return View("~/Plugins/Payments.Ogone/Views/PaymentOgone/Configure.cshtml", model);
		}

		[HttpPost]
		[AdminAuthorize]
		[ChildActionOnly]
		public ActionResult Configure(ConfigurationModel model)
		{
			if (!ModelState.IsValid)
				return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ogonePaymentSettings = _settingService.LoadSetting<OgonePaymentSettings>(storeScope);

			//save settings
			ogonePaymentSettings.PSPId = model.PSPId;
			ogonePaymentSettings.SHAInPassPhrase = _encryptionService.EncryptText(model.SHAInPassPhrase);
			ogonePaymentSettings.SHAOutPassPhrase = _encryptionService.EncryptText(model.SHAOutPassPhrase);
			ogonePaymentSettings.AdditionalFee = model.AdditionalFee;
			ogonePaymentSettings.HashAllParameters = model.HashAllParameters;
			ogonePaymentSettings.HashingAlgorithm = (HashingAlgorithm)model.HashingAlgorithmId;
			ogonePaymentSettings.OgoneGatewayUrl = model.OgoneGatewayUrl;
            ogonePaymentSettings.TemplateUrl = model.TemplateUrl;
            ogonePaymentSettings.TemplateTitle = model.TemplateTitle;
			ogonePaymentSettings.BackgroundColor = model.BackgroundColor;
			ogonePaymentSettings.TextColor = model.TextColor;
			ogonePaymentSettings.TableBackgroundColor = model.TableBackgroundColor;
			ogonePaymentSettings.TableTextColor = model.TableTextColor;
			ogonePaymentSettings.ButtonBackgroundColor = model.ButtonBackgroundColor;
			ogonePaymentSettings.ButtonTextColor = model.ButtonTextColor;
			ogonePaymentSettings.FontFamily = model.FontFamily;
			ogonePaymentSettings.LogoUrl = model.LogoUrl;
			ogonePaymentSettings.ParamVar = model.ParamVar;
		    ogonePaymentSettings.OrderIdPrefix = model.OrderIdPrefix;
            ogonePaymentSettings.PmList = model.PmList;
            ogonePaymentSettings.ExclPmList = model.ExclPmList;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.PSPId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.PSPId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.PSPId, storeScope);

            if (model.SHAInPassPhrase_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.SHAInPassPhrase, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.SHAInPassPhrase, storeScope);

            if (model.SHAOutPassPhrase_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.SHAOutPassPhrase, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.SHAOutPassPhrase, storeScope);

            if (model.AdditionalFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.AdditionalFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.AdditionalFee, storeScope);

            if (model.HashAllParameters_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.HashAllParameters, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.HashAllParameters, storeScope);

            if (model.HashingAlgorithmId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.HashingAlgorithm, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.HashingAlgorithm, storeScope);

            if (model.OgoneGatewayUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.OgoneGatewayUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.OgoneGatewayUrl, storeScope);

            if (model.TemplateUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.TemplateUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.TemplateUrl, storeScope);

            if (model.TemplateTitle_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.TemplateTitle, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.TemplateTitle, storeScope);

            if (model.BackgroundColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.BackgroundColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.BackgroundColor, storeScope);

            if (model.TextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.TextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.TextColor, storeScope);

            if (model.TableBackgroundColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.TableBackgroundColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.TableBackgroundColor, storeScope);

            if (model.TableTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.TableTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.TableTextColor, storeScope);

            if (model.ButtonBackgroundColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.ButtonBackgroundColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.ButtonBackgroundColor, storeScope);

            if (model.ButtonTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.ButtonTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.ButtonTextColor, storeScope);

            if (model.FontFamily_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.FontFamily, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.FontFamily, storeScope);

            if (model.LogoUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.LogoUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.LogoUrl, storeScope);

            if (model.ParamVar_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.ParamVar, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.LogoUrl, storeScope);

            if (model.OrderIdPrefix_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.OrderIdPrefix, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.OrderIdPrefix, storeScope);

            if (model.PmList_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.PmList, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.PmList, storeScope);

            if (model.ExclPmList_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(ogonePaymentSettings, x => x.ExclPmList, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(ogonePaymentSettings, x => x.ExclPmList, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

		    return Configure();
		}

		[ChildActionOnly]
		public ActionResult PaymentInfo()
		{
			var model = new PaymentInfoModel();
            return View("~/Plugins/Payments.Ogone/Views/PaymentOgone/PaymentInfo.cshtml", model);
		}

		[HttpGet]
		[ValidateInput(false)]
        public ActionResult AcceptPayment(PostBackModel model)
		{
            if (model == null) 
                return new EmptyResult();

            var orderId = GetOrderId(model.ORDERID);
            ProcessPostBackModel(model);

            return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
		}

		[ValidateInput(false)]
        public ActionResult CancelPayment(PostBackModel model)
		{
            ProcessPostBackModel(model);

            return View("~/Plugins/Payments.Ogone/Views/PaymentOgone/CancelPayment.cshtml");
		}

		[ValidateInput(false)]
        public ActionResult DeclinePayment(PostBackModel model)
		{
            ProcessPostBackModel(model);

            return View("~/Plugins/Payments.Ogone/Views/PaymentOgone/DeclinePayment.cshtml");
		}

		[ValidateInput(false)]
        public ActionResult ExceptionPayment(PostBackModel model)
		{
            ProcessPostBackModel(model);

            return View("~/Plugins/Payments.Ogone/Views/PaymentOgone/ExceptionPayment.cshtml");
		}

		[ValidateInput(false)]
		public ActionResult PostBackHandler(PostBackModel model)
		{
            ProcessPostBackModel(model);

			// nothing should be rendered to visitor
			return Content(string.Empty);
		}

        [NonAction]
        public void ProcessPostBackModel(PostBackModel model)
        {
            if (model == null) return;

            var orderId = GetOrderId(model.ORDERID);

            var requestFields = new SortedDictionary<string, string>();
            var keyValueCollection = (Request.RequestType == "POST") ? Request.Form : Request.QueryString;

            var sb = new StringBuilder("Ogone Postback Fields:\r\n");
            foreach (string key in keyValueCollection)
            {
                if (!key.ToUpper().Equals("SHASIGN"))
                    requestFields.Add(key, keyValueCollection[key]);

                sb.AppendFormat("{0}={1};\r\n", key, keyValueCollection[key]);
            }

            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                _logger.InsertLog(LogLevel.Error, "Invalid order id", string.Format("Ogone Postback Error. Order id {0} not found.", orderId));
                return;    
            }
            
            AddOrderNote(order, sb.ToString(), false);

            var processor =
                _paymentService.LoadPaymentMethodBySystemName("Payments.Ogone") as OgonePaymentProcessor;
            if (processor == null || !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("Ogone Payment module cannot be loaded");

            if (processor.VerifyHashDigest(requestFields, model.SHASIGN) == false)
            {
                AddOrderNote(order, "Ogone Postback Error. SHA-digest verification failed", false);
                return;
            }

            var paymentStatus = OgoneHelper.GetPaymentStatus(model.STATUS.ToString(), model.NCERROR);
            switch (paymentStatus)
            {
                case PaymentStatus.Authorized:
                    if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                    {
                        _orderProcessingService.MarkAsAuthorized(order);
                    }
                    break;
                case PaymentStatus.Paid:
                    if (_orderProcessingService.CanMarkOrderAsPaid(order))
                    {
                        _orderProcessingService.MarkOrderAsPaid(order);
                    }
                    break;
                case PaymentStatus.Pending:
                    break;
                case PaymentStatus.Voided:
                    if (_orderProcessingService.CanCancelOrder(order))
                    {
                        _orderProcessingService.CancelOrder(order, notifyCustomer: true);
                    }
                    break;
            }
        }

	    [NonAction]
		public override IList<string> ValidatePaymentForm(FormCollection form)
		{
			var warnings = new List<string>();
			return warnings;
		}

		[NonAction]
		public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
		{
			var paymentInfo = new ProcessPaymentRequest();
			return paymentInfo;
		}

		[NonAction]
		public void AddOrderNote(Order order, string noteText, bool displayToCustomer)
		{
			// Add ordernote
			order.OrderNotes.Add(
                new OrderNote
			    {
				    Note = noteText,
				    DisplayToCustomer = displayToCustomer,
				    CreatedOnUtc = DateTime.UtcNow
			    });
			
			_orderService.UpdateOrder(order);
		}

        private int GetOrderId(string orderId)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ogonePaymentSettings = _settingService.LoadSetting<OgonePaymentSettings>(storeScope);

            return
                string.IsNullOrEmpty(ogonePaymentSettings.OrderIdPrefix)
                    ? Convert.ToInt32(orderId)
                    : Convert.ToInt32(orderId.Substring(ogonePaymentSettings.OrderIdPrefix.Length));
        }
	}
}