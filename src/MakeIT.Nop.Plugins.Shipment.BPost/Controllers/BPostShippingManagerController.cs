using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager.Models;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace MakeIT.Nop.Plugin.Shipping.Bpost.ShippingManager.Controllers
{
    public class BPostShippingManagerController : BasePluginController
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly BpostShippingManagerSettings _bpostShippingManagerSettings;
        private readonly BpostShippingManagerSettings _settings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IShippingService _shippingService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;

        public BPostShippingManagerController(
            IStoreContext storeContext,
            IWorkContext workContext,
            IStoreMappingService storeMappingService,
            IGenericAttributeService genericAttributeService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IShippingService shippingService,
            ICheckoutAttributeService checkoutAttributeService,
            ICustomerService customerService,
            ICountryService countryService,
            ICheckoutAttributeParser checkoutAttributeParser,
            BpostShippingManagerSettings bpostShippingManagerSettings,
            BpostShippingManagerSettings settings,
            ILogger logger)
        {
            _storeContext = storeContext;
            _workContext = workContext;
            _storeMappingService = storeMappingService;
            _genericAttributeService = genericAttributeService;
            _bpostShippingManagerSettings = bpostShippingManagerSettings;
            _settings = settings;
            _logger = logger;
            _settingService = settingService;
            _localizationService = localizationService;
            _shippingService = shippingService;
            _checkoutAttributeService = checkoutAttributeService;
            _customerService = customerService;
            _countryService = countryService;
            _checkoutAttributeParser = checkoutAttributeParser;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new BpostShippingManagerModel
            {
                AccountId = _bpostShippingManagerSettings.AccountId,
                PassPhrase = _bpostShippingManagerSettings.PassPhrase,
                ButtonCssClass = _bpostShippingManagerSettings.ButtonCssClass,
                Standardprice = _bpostShippingManagerSettings.Standardprice,
                DoRefresh = _bpostShippingManagerSettings.DoRefresh
            };

            return View("~/Plugins/Shipping.BPostShippingManager/Views/BPostShippingManager/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(BpostShippingManagerModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _bpostShippingManagerSettings.AccountId = model.AccountId;
            _bpostShippingManagerSettings.PassPhrase = model.PassPhrase;
            _bpostShippingManagerSettings.ButtonCssClass = model.ButtonCssClass;
            _bpostShippingManagerSettings.Standardprice = model.Standardprice;
            _bpostShippingManagerSettings.DoRefresh = model.DoRefresh;

            _settingService.SaveSetting(_bpostShippingManagerSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return View("~/Plugins/Shipping.BPostShippingManager/Views/BPostShippingManager/Configure.cshtml", model);
        }

        [ValidateInput(false)]
        public ActionResult ConfirmHandler(PostBackModel model)
        {
            try
            {
                _logger.InsertLog(LogLevel.Information, "Entered ConfirmHandler", customer: _workContext.CurrentCustomer);

                UpdateShippingOptionRate(model);
                UpdateCheckoutAttribute(model);

                var doRefresh = _settings.DoRefresh ? "true" : "false";
                ViewData["data"] = $"confirm|{doRefresh}";

                //if (model.DeliveryMethod.Equals(DeliveryMethod.Regular, StringComparison.InvariantCultureIgnoreCase))
                //{
                //    var newCountry = _countryService.GetCountryByTwoLetterIsoCode(model.CustomerCountry);

                //    var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                //        model.CustomerFirstName, model.CustomerLastName, model.CustomerPhoneNumber,
                //        model.CustomerEmail, _workContext.CurrentCustomer.ShippingAddress.FaxNumber,
                //        _workContext.CurrentCustomer.ShippingAddress.Company,
                //        $"{model.CustomerStreet} {model.CustomerStreetNumber}",
                //        $"{model.CustomerBox}", model.CustomerCity, _workContext.CurrentCustomer.ShippingAddress.StateProvinceId,
                //        model.CustomerPostalCode, newCountry.Id, string.Empty);
                //    if (address == null)
                //    {
                //        address = new Address
                //        {
                //            CreatedOnUtc = DateTime.UtcNow,
                //            Address1 = $"{model.CustomerStreet} {model.CustomerStreetNumber}",
                //            Address2 = $"{model.CustomerBox}",
                //            City = $"{model.CustomerCity}",
                //            ZipPostalCode = $"{model.CustomerPostalCode}",
                //            Email = $"{model.CustomerEmail}",
                //            FirstName = $"{model.CustomerFirstName}",
                //            LastName = $"{model.CustomerLastName}",
                //            PhoneNumber = $"{model.CustomerPhoneNumber}",
                //            Country = newCountry
                //        };

                //        _workContext.CurrentCustomer.Addresses.Add(address);
                //    }

                //    _workContext.CurrentCustomer.ShippingAddress = address;
                //    _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                //    // Clear possible checkout attribute
                //    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CheckoutAttributes, string.Empty, _storeContext.CurrentStore.Id);
                //}

            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, ex.Message, ex.StackTrace, _workContext.CurrentCustomer);
            }

            return View("~/Plugins/Shipping.BPostShippingManager/Views/BPostShippingManager/ConfirmHandler.cshtml");
        }

        private void UpdateCheckoutAttribute(PostBackModel model)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var collectPoint = string.Format(@"{0}, {1} {2}, {3} {4}, {5}", 
                        model.CustomerPostalLocation, model.CustomerStreet, model.CustomerStreetNumber,
                        model.CustomerPostalCode, model.CustomerCity, model.CustomerCountry);

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            //var attributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _storeContext.CurrentStore.Id);
            var attributesXml = string.Empty;
            foreach (var attribute in checkoutAttributes)
            {
                if (attribute.Name == "CollectPoint")
                {
                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, collectPoint);
                }

                if (attribute.Name == "OrderReference")
                {
                    attributesXml = _checkoutAttributeParser.AddCheckoutAttribute(attributesXml, attribute, model.OrderReference);
                }
            }

            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);

            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, CustomCustomerAttributeNames.DeliveryMethod, model.DeliveryMethod, _storeContext.CurrentStore.Id);
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, CustomCustomerAttributeNames.DeliveryMethodRate, model.DeliveryMethodPriceTotalEuro, _storeContext.CurrentStore.Id);
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, CustomCustomerAttributeNames.DeliveryMethodAddress, collectPoint, _storeContext.CurrentStore.Id);
        }

        private void UpdateShippingOptionRate(PostBackModel model)
        {
            var shippingOptions =
                _workContext.CurrentCustomer.GetAttribute<List<ShippingOption>>(
                    SystemCustomerAttributeNames.OfferedShippingOptions, _storeContext.CurrentStore.Id);
            
            //loaded cached results. let's filter result by a chosen shipping rate computation method
            shippingOptions =
                shippingOptions.Where(
                    so =>
                        so.ShippingRateComputationMethodSystemName.Equals("Shipping.Bpost.ShippingManager",
                            StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

            var shippingOption = shippingOptions.FirstOrDefault();
            if (shippingOption != null)
            {
                shippingOption.Rate = model.DeliveryMethodPriceTotalEuro;

                _genericAttributeService.SaveAttribute(
                    _workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.OfferedShippingOptions,
                    shippingOptions,
                    _storeContext.CurrentStore.Id);

                _genericAttributeService.SaveAttribute(
                    _workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedShippingOption,
                    shippingOption,
                    _storeContext.CurrentStore.Id);
            }
        }

        [ValidateInput(false)]
        public ActionResult CancelHandler(PostBackModel model)
        {
            return View("~/Plugins/Shipping.BPostShippingManager/Views/BPostShippingManager/CancelHandler.cshtml");
        }

        [ValidateInput(false)]
        public ActionResult ErrorHandler(PostBackModel model)
        {
            return View("~/Plugins/Shipping.BPostShippingManager/Views/BPostShippingManager/ErrorHandler.cshtml");
        }

    }
}
