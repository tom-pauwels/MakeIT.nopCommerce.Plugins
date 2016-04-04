using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Shipping;
using Nop.Services.Localization;
using Nop.Services.Shipping.Tracking;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager
{
    public class BpostShippingManagerComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        private readonly BpostShippingManagerSettings _settings;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public BpostShippingManagerComputationMethod(
            BpostShippingManagerSettings settings,
            ISettingService settingService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _settings = settings;
            _settingService = settingService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || getShippingOptionRequest.Items.Count == 0)
            {
                response.AddError("No shipment items");
                return response;
            }
            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }
            if (getShippingOptionRequest.ShippingAddress.Country == null)
            {
                response.AddError("Shipping country is not set");
                return response;
            }

            // string orderRef = DateTime.Now.ToString("yyyyMMddHHmmss");
            string orderRef = getShippingOptionRequest.Items.FirstOrDefault().ShoppingCartItem.Id.ToString();

            var checkSum = CheckSumCalculator.CalculateChecksum(
                new Dictionary<string, string>
                {
                    { "accountId", _settings.AccountId },
                    { "action", "START" },
                    { "customerCountry", getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode },
                    { "orderReference", orderRef }
                }, _settings.PassPhrase);

            var storeUrl = _storeContext.CurrentStore.Url;
            if (!storeUrl.EndsWith("/")) storeUrl += "/";

            var confirmUrl = storeUrl + "Plugins/BpostShippingManager/ConfirmHandler";
            var cancelUrl =  storeUrl + "Plugins/BpostShippingManager/CancelHandler";
            var errorUrl = storeUrl + "Plugins/PaymentOgone/ErrorHandler";

            var streetNumberStart =
                getShippingOptionRequest.ShippingAddress.Address1.IndexOfAny("0123456789".ToCharArray());

            var bpostShippingOption = new ShippingOption
            {
                Name = _localizationService.GetResource("MakeIT.Nop.Shipping.Bpost.ShippingManager.ShippingOptionTitle"),
                Rate = _settings.Standardprice,
                ShippingRateComputationMethodSystemName = "SHM",
                Description = string.Format(
                    @"<div><input class='{13}' type='button' onclick=""loadShm('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{15}', '{16}');"" value='{14}'></div>",
                    _settings.AccountId, 
                    orderRef, 
                    getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode, 
                    checkSum,
                    getShippingOptionRequest.ShippingAddress.FirstName, 
                    getShippingOptionRequest.ShippingAddress.LastName,
                    getShippingOptionRequest.Customer.Email,
                    getShippingOptionRequest.ShippingAddress.Address1.Substring(0, streetNumberStart - 1),
                    getShippingOptionRequest.ShippingAddress.ZipPostalCode,
                    getShippingOptionRequest.ShippingAddress.City,
                    confirmUrl,
                    cancelUrl,
                    errorUrl,
                    _settings.ButtonCssClass,
                    _localizationService.GetResource("MakeIT.Nop.Shipping.Bpost.ShippingManager.ButtonCaption"),
                    _workContext.WorkingLanguage.UniqueSeoCode,
                    getShippingOptionRequest.ShippingAddress.Address1.Substring(streetNumberStart)
                    )
            };
            response.ShippingOptions.Add(bpostShippingOption);

            return response;
        }

        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType.Realtime;

        public IShipmentTracker ShipmentTracker => null;

        public void GetConfigurationRoute(out string actionName, out string controllerName,
            out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "BPostShippingManager";
            routeValues = new RouteValueDictionary { { "Namespaces", "MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager.Controllers" }, { "area", null } };
        }

        public override void Install()
        {
            var pluginSettings = new BpostShippingManagerSettings
            {
                AccountId = "111111",
                PassPhrase = "MySecretPassPhrase",
                ButtonCssClass = "button-1",
                Standardprice = 4
            };
            _settingService.SaveSetting(pluginSettings);

            this.AddOrUpdatePluginLocaleResource("MakeIT.Nop.Shipping.Bpost.ShippingManager.ShippingOptionTitle", "Verzenden met Bpost");
            this.AddOrUpdatePluginLocaleResource("MakeIT.Nop.Shipping.Bpost.ShippingManager.ButtonCaption", "Kies uw verzendmethode...");

            base.Install();
        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("MakeIT.Nop.Shipping.Bpost.ShippingManager.ShippingOptionTitle");
            this.DeletePluginLocaleResource("MakeIT.Nop.Shipping.Bpost.ShippingManager.ButtonCaption");

            base.Uninstall();
        }
    }
}