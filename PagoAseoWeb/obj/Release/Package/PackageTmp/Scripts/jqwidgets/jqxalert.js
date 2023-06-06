jqxAlert = {
    // top offset.
    top: 0,
    // left offset.
    left: 0,
    // opacity of the overlay element.
    overlayOpacity: 0.2,
    // background of the overlay element.
    overlayColor: '#ddd',
    // display alert.

    alert: function (message, title, tip, imagen, urlBase) {
        if (title == null) title = '';
        if (tip == null) tip = '';
        if (imagen == null) imagen = 'Informacion';
        if (urlBase == null) urlBase = "";
        jqxAlert._show(title, message, tip, imagen, urlBase);
    },
    // initializes a new alert and displays it.
    _show: function (title, msg, tipo, img, url) {
        var image = '';
        jqxAlert._hide();
        jqxAlert._handleOverlay('show');
        $("BODY").append(
                  '<div class="jqx-alert" style="width: auto; height: auto; overflow: hidden; white-space: nowrap;" id="alert_container">' +
                      '<div id="alert_title"></div>' +
                      '<div id="alert_content">' +
                           '<div id="message"></div>' +
                      '</div>' +
                  '</div>');


        switch (img) {
            case 'Info':
                image = '/Imagen/info.gif';
                break;
            case 'Help':
                image = '/Imagen/help.gif';
                break;
            case 'Important':
                image = '/Imagen/important.gif"';
                break;
        }
        $("#alert_title").text(title);
        $("#alert_title").addClass('jqx-alert-header');
        $("#alert_content").addClass('jqx-alert-content');
        switch (tipo) {
            case 'Pago':

                var html = '<table class="table table-bordered"> <tr><td class="alert alert-info" >En caso que el comprobante en el tiempo no este disponible , revise su estado de cuenta</td></tr> '
                html += '<tr><td class="alert alert-info">Si se ha efectuado el descuento correspondiente al pago, lo mas problable <br> que la Tesoría General de la Republica ha tenido un problema, se recomienda esperar.</td></tr> '
                html += '<tr><td class="alert alert-info">Contactar a Marco Landskron Toro  Correo Electronico mlandskron@munimacul.cl <br> para tener una respuesta mas concreta al problema </td></tr>'                
                html += '<tr><td class="alert alert-info"> Se insiste en caso de efectuarse el descuento en su estado de cuenta, <br>no intente realizar el pago, ya que con esta accion solo llevara a duplicarlo</td></tr>'
                html += '</table> '
                html += ' &nbsp;&nbsp;<br />'
                html += '<table class="table table-bordered"> '
                html += '<tr><td>Formulario de Contacto WEB</td><td ><a href="https://www.tesoreria.cl/IntClientePregosWSWeb/?RUT=0&DV=0"  style="text-decoration:none;" target="_blank"; title="Permite realizar consultas, reclamos, sugerencias y felicitaciones">  Consultas en Tesoreria<br></a></td></tr>'
                html += ' <tr><td> Mesa de Ayuda Tesorería </td><td> 2 2768 9800</td></tr>'
                html += '<tr><td> Mesa Central Tesorería </td><td> 2 2693 0500 </td></tr> </table>'
                html += '<input align="center" class="btn btn-success" style="margin-top: 10px;" type="button" value="' + msg + '" id="alert_button"/>';
                $("#message").html(html);
                $("#alert_button").width(190);
                $("#alert_button").click(function () {
                    jqxAlert._hide();
                });
                break;
            case 'Confirm':
                //si lo ocupo lo programo
                break;
            case 'Prompt':
                //si lo ocupo lo programo
                break;
        }
        jqxAlert._setPosition();



    },
    // hide alert.
    _hide: function () {
        $("#alert_container").remove();
        jqxAlert._handleOverlay('hide');
    },
    // initialize the overlay element.
    _handleOverlay: function (status) {
        switch (status) {
            case 'show':
                jqxAlert._handleOverlay('hide');
                $("BODY").append('<div id="alert_overlay"></div>');
                $("#alert_overlay").css({
                    position: 'absolute',
                    zIndex: 99998,
                    top: '0px',
                    left: '0px',
                    width: '100%',
                    height: $(document).height(),
                    background: jqxAlert.overlayColor,
                    opacity: jqxAlert.overlayOpacity
                });
                break;
            case 'hide':
                $("#alert_overlay").remove();
                break;
        }
    },
    // sets the alert's position.
    _setPosition: function () {
        // center screen with offset.
        var top = (($(window).height() / 2) - ($("#alert_container").outerHeight() / 2)) + jqxAlert.top;
        var left = (($(window).width() / 2) - ($("#alert_container").outerWidth() / 2)) + jqxAlert.left;
        if (top < 0) {
            top = 0;
        }
        if (left < 0) {
            left = 0;
        }
        // set position.
        $("#alert_container").css({
            top: top + 'px',
            left: left + 'px'
        });
        // update overlay.
        $("#alert_overlay").height($(document).height());
    }
}
