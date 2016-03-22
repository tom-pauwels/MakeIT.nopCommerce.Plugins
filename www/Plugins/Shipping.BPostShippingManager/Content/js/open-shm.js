﻿function loadShm(accountId, orderRef, custCountry, checksum, customerFirstName, customerLastName, customerEmail,
    customerStreet, customerPostalCode, customerCity, confirmUrl, cancelUrl, errorUrl) {
    SHM.open({
        integrationType: 'POPUP',
        popupWidth: 1024,
        popupHeight: 600,
        parameters: {
            accountId: accountId,
            action: 'START',
            customerCountry: custCountry,
            confirmUrl: 'http://localhost:9090/Plugins/BpostShippingManager/ConfirmHandler',
            orderReference: orderRef,
            customerFirstName: customerFirstName,
            customerLastName: customerLastName,
            customerEmail: customerEmail,
            customerStreet: customerStreet, 
            customerPostalCode: customerPostalCode, 
            customerCity: customerCity, 
            confirmUrl: confirmUrl,
            cancelUrl: cancelUrl,
            errorUrl: errorUrl,
            checksum: checksum
        },
        closeCallback: function(data) {
            if (data === 'confirm') {
                ShippingMethod.save();
                // do something after client has clicked confirm in shippingmanager
                //window.location.href = 'http://localhost:9090/onepagecheckout';
            } else {
                //window.top.location.href = "http://www.Shop.test/checkout/producten.php";
            }
        }
    });
}
