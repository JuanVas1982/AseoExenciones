using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialAseoWeb.Comun;

/**
 * @author     Allware Ltda. (http://www.allware.cl)
 * @copyright  2015 Transbank S.A. (http://www.tranbank.cl)
 * @date       May 2016
 * @license    GNU LGPL
 * @version    1.0
 */

namespace Transbank.NET.sample.certificates
{
    public class CertNormal
    {

        internal static Dictionary<string, string> certificate()
        {

            string certPublico = Utils.certPublico;
            string certSalida = Utils.certSalida;
            string claveCertSalida = Utils.claveCertSalida;
            string codigoComercio = Utils.codigoComercio;
            string ambiente = Utils.ambiente;
            string certFolder = Utils.rutaCertTBK;

            /** Crea un Dictionary para almacenar los datos de integración pruebas */
            Dictionary<string, string> certificate = new Dictionary<string, string>();

            /** Agregar datos de integración a Dictionary */

            /** Modo de Utilización */
            certificate.Add("environment", ambiente);

            /** Certificado Publico (Dirección fisica de certificado o contenido) */
            certificate.Add("public_cert", certFolder + certPublico);

            /** Ejemplo de Ruta de Certificado de Salida */
            certificate.Add("webpay_cert", certFolder + certSalida);

            /** Ejemplo de Password de Certificado de Salida */
            certificate.Add("password", claveCertSalida);

            /** Codigo Comercio */
            certificate.Add("commerce_code", codigoComercio);

            return certificate;

        }

    }
}