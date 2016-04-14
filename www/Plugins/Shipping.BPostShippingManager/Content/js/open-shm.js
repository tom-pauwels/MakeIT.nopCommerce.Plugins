function loadShm(accountId, orderRef, custCountry, checksum, customerFirstName, customerLastName, customerEmail,
    customerStreet, customerPostalCode, customerCity, confirmUrl, cancelUrl, errorUrl, lang, customerStreetNumber,
    reloadUrl, doRefresh) {
    SHM.open({
        integrationType: 'POPUP',
        popupWidth: 1024,
        popupHeight: 600,
        parameters: {
            accountId: accountId,
            lang: lang,
            action: 'START',
            customerCountry: custCountry,
            orderReference: orderRef,
            customerFirstName: customerFirstName,
            customerLastName: customerLastName,
            customerEmail: customerEmail,
            customerStreet: customerStreet,
            customerStreetNumber: customerStreetNumber,
            customerPostalCode: customerPostalCode, 
            customerCity: customerCity, 
            confirmUrl: confirmUrl,
            cancelUrl: cancelUrl,
            errorUrl: errorUrl,
            checksum: checksum
        },
        closeCallback: function(data) {
            if (data === 'confirm') {
                if (doRefresh === 'false') {
                    ShippingMethod.save();
                } else {
                    window.location.href = reloadUrl;

                    //var rootscopetemp = angular.injector(['ng']).get('$rootScope');
                    //rootscopetemp.$apply(function () {
                    //    rootscopetemp.$broadcast('shippingMethodsContextChanged');
                    //});
                }
            }
        }
    });
}
