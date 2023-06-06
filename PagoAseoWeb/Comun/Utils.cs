using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;
using System.Web.Configuration;
using SocialAseoWeb.Models;

namespace SocialAseoWeb.Comun
{
    public static class Utils
    {

        public static string RutaExcel = WebConfigurationManager.AppSettings["RutaExcel"];
        public static string RutaUploaded = WebConfigurationManager.AppSettings["RutaUploaded"];
        public static string RutaCodQr = WebConfigurationManager.AppSettings["RutaCodQr"];
        public static string RutaReporte = WebConfigurationManager.AppSettings["RutaReporte"];
        public static string RutaLogTrans = WebConfigurationManager.AppSettings["RutaLogTrans"];
        public static string RutaPDF = WebConfigurationManager.AppSettings["RutaPDF"];
        public static string RutaWord = WebConfigurationManager.AppSettings["RutaWord"];
        public static string caja = WebConfigurationManager.AppSettings["caja"];
        public static string sistema = WebConfigurationManager.AppSettings["sistema"];
        public static string SitioCodQr = WebConfigurationManager.AppSettings["SitioCodQr"];
        public static string tipoTributo = WebConfigurationManager.AppSettings["tipoTributo"];
        public static string cuidad = WebConfigurationManager.AppSettings["cuidad"];
        public static string usuarioWS = WebConfigurationManager.AppSettings["usuarioWS"];
        public static string maxRegiPdf = WebConfigurationManager.AppSettings["maxRegiPdf"];
        public static string duracionCookie = WebConfigurationManager.AppSettings["duracionCookie"];
        public static string telefono = WebConfigurationManager.AppSettings["telefono"];
        public static string adminContacto = WebConfigurationManager.AppSettings["adminContacto"];
        public static string adminEMail = WebConfigurationManager.AppSettings["adminEMail"];
        public static string direccionPagoKCC = WebConfigurationManager.AppSettings["direccionPagoKCC"];
        public static string tipoTransaccion = WebConfigurationManager.AppSettings["tipoTransaccion"];
        public static string paginaExitoKCC = WebConfigurationManager.AppSettings["paginaExitoKCC"];
        public static string paginaFracasoKCC = WebConfigurationManager.AppSettings["paginaFracasoKCC"];
        public static string endTBK = WebConfigurationManager.AppSettings["endTBK"];
        public static string resultTBK = WebConfigurationManager.AppSettings["resultTBK"];
        public static string rutaCertTBK = WebConfigurationManager.AppSettings["RutaCertTBK"];
        public static string certPublico = WebConfigurationManager.AppSettings["certPublico"];
        public static string certSalida = WebConfigurationManager.AppSettings["certSalida"];
        public static string claveCertSalida = WebConfigurationManager.AppSettings["claveCertSalida"];
        public static string codigoComercio = WebConfigurationManager.AppSettings["codigoComercio"];
        public static string ambiente = WebConfigurationManager.AppSettings["ambiente"];
        //mail
        public static string mensaje = WebConfigurationManager.AppSettings["mensaje"];
        public static string asunto = WebConfigurationManager.AppSettings["asunto"];
        public static string emisor = WebConfigurationManager.AppSettings["emisor"];
        public static string password = WebConfigurationManager.AppSettings["password"];
        public static string Host = WebConfigurationManager.AppSettings["Host"];
        public static string Port = WebConfigurationManager.AppSettings["Port"];
        public static string EnableSsl = WebConfigurationManager.AppSettings["EnableSsl"];
        public static decimal? siDecimalNull(decimal? numero)
        {
            if (numero.HasValue)
            {
                /*si tiene valor*/
                return numero; 
            }
            else
            {
                /*si es nullo*/
                return 0;
            }
        }

        public static string Right(string param, int length)
        {
            int value = param.Length - length;
            string result = param.Substring(value, length);
            return result;
        }

        public static string Left(string param, int length)
        {

            string result = param.Substring(0, length);
            return result;
        }

        public static string Mid(string param, int startIndex, int length)
        {
            string result = param.Substring(startIndex, length);
            return result;
        }

        public static string Encriptar(this string _cadenaAencriptar)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        public static string DesEncriptar(this string _cadenaAdesencriptar)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            //result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
            result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }

        public static void crearDirectorio(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileIOPermission f = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
            else
            {
                FileIOPermission f = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
        }

        public static bool verificaSiExisteArchivo(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string formatFechaVista(string fecha)
        {
            return Right(fecha, 2) + "/" + Mid(fecha, 4, 2) + "/" + Left(fecha, 4);
        }

        public static string formatHoraVista(string fecha)
        {
            return Left(fecha, 2) + ":" + Mid(fecha, 2, 2) + ":" + Right(fecha, 2);
        }

        public static string formatFechaVista(DateTime? fecha)
        {
            return fecha.Value.ToString("yyyyMMdd");
        }

        public static string formatFechaBBDD(DateTime? fecha)
        {
            return fecha.Value.ToString("yyyyMMdd");
        }

        public static string clasificacion(decimal? param)
        {
            aseoEntities db = new aseoEntities();
            string result = db.AS_EXCLUSION.Where(x => x.EXC_CODIGO == param).FirstOrDefault().EXC_DESCRIPCION;
            return result;
        }

        public static string valorneto(decimal manzana, decimal predio, decimal periodo)
        {
            aseoEntities db = new aseoEntities();
            string result = db.AS_PAGOS.Where(x => x.PAG_ROL_NUM == manzana && x.PAG_ROL_DV == predio && x.PAG_ANO_CON == periodo).FirstOrDefault().PAG_VALOR.ToString();
            return result;
        }
    }
}