function loadShm(accountId, orderRef, custCountry, checksum, customerFirstName, customerLastName, customerEmail,
    customerStreet, customerPostalCode, customerCity, confirmUrl, cancelUrl, errorUrl, lang, customerStreetNumber) {

    SHM.open({
        integrationType: 'INLINE',
        inlineContainerId: 'shm-inline-container',
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
        closeCallback: function (data) {
            var parts = data.split("|");
            if (parts[0] === 'confirm') {
                if (parts[1] === 'false') {
                    ShippingMethod.save();
                } else {
                    $.event.trigger({ type: "RefreshOnePageCheckoutEvent" });
                    $('#shm-inline-container').hide();
                }
            }
        }
    });
}
