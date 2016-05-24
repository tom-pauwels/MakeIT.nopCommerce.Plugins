function loadShm(accountId, orderRef, custCountry, checksum, customerFirstName, customerLastName, customerEmail,
    customerStreet, customerPostalCode, customerCity, confirmUrl, cancelUrl, errorUrl, lang, customerStreetNumber) {

    expandShm();

    SHM.open({
        integrationType: 'INLINE',
        forceIntegrationType: true,
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
                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
                        // tasks to do if it is a Mobile Device
                        window.location.href = parts[2];
                    } else {
                        $.event.trigger({ type: "RefreshOnePageCheckoutEvent" });
                        $("#shm-inline-container").height(0);
                    }
                }
            } else {
                $("#shm-inline-container").height(0);
            }
        }
    });
}

function collapseShm() {
    $("#shm-inline-container").height(0);
}

function expandShm() {
    $("#shm-inline-container").height(550);
}
