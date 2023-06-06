var Fun = {
    SiProceso: function (baseUrl) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ValidaPatente: function (baseUrl, placaTex, placaNum, dv) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum,
                dv: dv
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaLugarPlaca: function (baseUrl, placaTex, placaNum) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaDatosPlaca: function (baseUrl, placaTex, placaNum) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaSoap: function (baseUrl, placaTex, placaNum) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaRevisionTecnica: function (baseUrl, placaTex, placaNum) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaDatosVehicul: function (baseUrl, placaTex, placaNum) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaDatosContrib: function (baseUrl, placaTex, placaNum, rutCon, dvRutCon) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum,
                rut: rutCon,
                dv: dvRutCon
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaCodigos: function (baseUrl, placaTex, placaNum, cod1, cod2) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum,
                tipo: cod1,
                cod: cod2
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaComuna: function (baseUrl, placaTex, placaNum, codigo) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum,
                codigo: codigo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaEstadoOtraComuna: function (baseUrl, placaTex, placaNum) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaEstClasArchivo: function (baseUrl, placaTex, placaNum, clasifica) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum,
                clasifica: clasifica
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ValidaUsuario: function (baseUrl, usuario, contraseña) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                usuario: usuario,
                contraseña: contraseña,
                siCierre: 0
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    VerAñosParticular: function (baseUrl) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    VerArchivosParticular: function (baseUrl, año, estadCons, placa) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                año: año,
                estadCons: estadCons,
                placa: placa
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    BuscarArchivoParticular: function (baseUrl, archivo) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                archivo: archivo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ActualizaArchivoParticular: function (baseUrl, archivo, estado, observacion) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                archivo: archivo,
                estado: estado,
                observacion: observacion
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaCodigosChosen: function (baseUrl, cod1, cod2) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                tipo: cod1,
                cod: cod2
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaCodigosByCodigo: function (baseUrl, cod1, cod2) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                tipo: cod1,
                cod: cod2
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaMarcaChosen: function (baseUrl, codigo) {
        /* obtiene los datos */
        codigo = typeof codigo !== 'undefined' ? codigo : "";
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                cod: codigo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaModeloChosen: function (baseUrl, codigo) {
        /* obtiene los datos */
        codigo = typeof codigo !== 'undefined' ? codigo : "";
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                cod: codigo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaSII: function (baseUrl, codigo) {
        /* obtiene los datos */
        codigo = typeof codigo !== 'undefined' ? codigo : "";
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                codigo: codigo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaComunaChosen: function (baseUrl, codigo) {
        /* obtiene los datos */
        codigo = typeof codigo !== 'undefined' ? codigo : "";
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                codigo: codigo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaDatosContribByRut: function (baseUrl, rutCon, dvRutCon) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                rut: rutCon,
                dv: dvRutCon
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaComunaByCodigo: function (baseUrl, codigo) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                codigo: codigo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaComCodigoEmp: function (baseUrl, rut, codigo) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                rut: rut,
                codigo: codigo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    VerAñosEmpresa: function (baseUrl) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    VerArchivosEmpresa: function (baseUrl, año, estadCons, rut) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                año: año,
                estadCons: estadCons,
                rut: rut
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    BuscarArchivoEmpresa: function (baseUrl, archivo) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                archivo: archivo
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ActualizaArchivoEmpresa: function (baseUrl, archivo, estado, observacion) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                archivo: archivo,
                estado: estado,
                observacion: observacion
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaLugarRut: function (baseUrl, rut) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                rut: rut
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaMaestro: function (baseUrl, placaT, placaN) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaT: placaT,
                placaN: placaN
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ObtenerEncabezadoEmpresa: function (baseUrl, rut) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                rut: rut
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaValores: function (baseUrl, ordenIngreso, registro, cuota) {
        /* obtiene los datos */
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                ordenIngreso: encodeURIComponent(ordenIngreso),
                registro: encodeURIComponent(JSON.stringify(registro)),
                cuota: cuota
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    BuscaPropietario: function (baseUrl, placaT, placaN) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaT: placaT,
                placaN: placaN
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ObtenerEncabezadoParticular: function (baseUrl, placaT, placaN) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaT,
                placaNum: placaN
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ConsultaSiPagado: function (baseUrl, placaTex, placaNum) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                placaTex: placaTex,
                placaNum: placaNum
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    BuscaPropietarioByRut: function (baseUrl, Rut) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                Rut: Rut
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    validaRut: function (rutCompleto) {

        rutInt = rutCompleto.replace(/_/gi, "");

        if (!/^[0-9]+-[0-9kK]{1}$/.test(rutInt)) {
            return false;
        }
        var tmp = rutInt.split('-');
        var digv = tmp[1];
        var rut = tmp[0];
        if (digv == 'K') { digv = 'k' };
        return (Fun.dv(rut) == digv);
    },
    dv: function (T) {
        var M = 0, S = 1;
        for (; T; T = Math.floor(T / 10))
            S = (S + T % 10 * (9 - M++ % 6)) % 11;
        return S ? S - 1 : 'k';
    },
    validaFecha: function (fecha) {
        var dtCh = "/";
        var minYear = 1900;
        var maxYear = 2100;
        function isInteger(s) {
            var i;
            for (i = 0; i < s.length; i++) {
                var c = s.charAt(i);
                if (((c < "0") || (c > "9"))) return false;
            }
            return true;
        }
        function stripCharsInBag(s, bag) {
            var i;
            var returnString = "";
            for (i = 0; i < s.length; i++) {
                var c = s.charAt(i);
                if (bag.indexOf(c) == -1) returnString += c;
            }
            return returnString;
        }
        function daysInFebruary(year) {
            return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
        }
        function DaysArray(n) {
            for (var i = 1; i <= n; i++) {
                this[i] = 31
                if (i == 4 || i == 6 || i == 9 || i == 11) { this[i] = 30 }
                if (i == 2) { this[i] = 29 }
            }
            return this
        }
        function isDate(dtStr) {
            var daysInMonth = DaysArray(12)
            var pos1 = dtStr.indexOf(dtCh)
            var pos2 = dtStr.indexOf(dtCh, pos1 + 1)
            var strDay = dtStr.substring(0, pos1)
            var strMonth = dtStr.substring(pos1 + 1, pos2)
            var strYear = dtStr.substring(pos2 + 1)
            strYr = strYear
            if (strDay.charAt(0) == "0" && strDay.length > 1) strDay = strDay.substring(1)
            if (strMonth.charAt(0) == "0" && strMonth.length > 1) strMonth = strMonth.substring(1)
            for (var i = 1; i <= 3; i++) {
                if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1)
            }
            month = parseInt(strMonth)
            day = parseInt(strDay)
            year = parseInt(strYr)
            if (pos1 == -1 || pos2 == -1) {
                return false
            }
            if (strMonth.length < 1 || month < 1 || month > 12) {
                return false
            }
            if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > daysInMonth[month]) {
                return false
            }
            if (strYear.length != 4 || year == 0 || year < minYear || year > maxYear) {
                return false
            }
            if (dtStr.indexOf(dtCh, pos2 + 1) != -1 || isInteger(stripCharsInBag(dtStr, dtCh)) == false) {
                return false
            }
            return true
        }
        if (isDate(fecha)) {
            return true;
        } else {
            return false;
        }
    },
    ConsultaUsuario: function (baseUrl) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ValidaPagoParTGR: function (baseUrl, folio, placa) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                folio: folio,
                placa: placa
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    },
    ValidaPagoEmpTGR: function (baseUrl, folio, rutCont) {
        url = baseUrl;
        return $.ajax({
            url: url,
            method: 'POST',
            async: false,
            dataType: 'json',
            data: {
                folio: folio,
                rutCont: rutCont
            },
            success: function (data, textStatus, xhr) {

            },
            error: function (request, status, error) {
                console.log(request.responseText);
                alert("Error has occurred..." + request.responseText);
            }
        });
    }
}