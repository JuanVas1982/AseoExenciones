using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Windows.Forms;
using CrystalDecisions.Shared;
using System.Security.Cryptography;
using SocialAseoWeb.Models;
using System.Data;
using System.Drawing;
namespace SocialAseoWeb.Comun
{
    public class Pdf
    {
        public void crearPdf(string rutaNombre, ReportDocument reportDocument)
        {
            if (File.Exists(rutaNombre))
            {
                File.Delete(rutaNombre);
            }
            reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, rutaNombre);

        }
        public static void GeneraCertificado(decimal rolM, decimal rolP, decimal periodo)
        {
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            string hora = DateTime.Now.ToString("hhMMss");
            int anoCargo = DateTime.Now.Year;
            var Meses = new string[12] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            string rutaReporte = Utils.RutaReporte + "Certificado.rpt";
            string Aux_Nombre = "";
            string Aux_Rol = rolM.ToString() + "-" + rolP.ToString();
            string Aux_nDecreto = "";
            string Aux_fDecreto = "";
            string Aux_Periodo = periodo.ToString();
            DataTable dtCodigoRr = new DataTable("codigoQrT");
            DataSet dataSet = new DataSet("dsSocial");

            dtCodigoRr.Columns.Add(new DataColumn("NUMDECRE", typeof(System.String), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("codigoQrArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma1ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma2ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma3ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma4ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma5ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma6ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma7ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
            dtCodigoRr.Columns.Add(new DataColumn("Firma8ArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));


            using (aseoEntities d = new aseoEntities())
            {

                ReportDocument reportDocument = new ReportDocument();
                
                var beneficiario = d.Buscar_beneficiario2(rolM, rolP, periodo).FirstOrDefault();
                if (beneficiario != null)
                {
                    Aux_Nombre = beneficiario.Nombres + " " + beneficiario.APELLIDO_PATERNO + " " + beneficiario.APELLIDO_MATERNO;
                }

                string nombreArchQr = Utils.RutaCodQr + fecha + "_" + hora + "_" + rolM + "-" + rolP + ".png";
                string datos = Utils.SitioCodQr + "x=" + rolM.ToString() + "&Y=" + rolP.ToString() + "&Z=" + periodo.ToString();
                CodigoQr.crearCodigoQr(nombreArchQr, datos);

                //dtCodigoRr.Columns.Add(new DataColumn("codigoQrArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
                dataSet.Reset();
                dataSet.Tables.Add(dtCodigoRr);

                DataRow rowQr = dtCodigoRr.NewRow();
                //log.registrar("se carga qr a dataset");
                rowQr["NUMDECRE"] = rolM.ToString() + "-" + rolP.ToString();
                rowQr["codigoQrArrayBytes"] = ImageHelper.ImageToByteArray(ImageHelper.ObtenerCodigoQr(nombreArchQr));

                dtCodigoRr.Rows.Add(rowQr);
                reportDocument.Load(rutaReporte);
                reportDocument.SetDataSource(dataSet);

                //dataSet.WriteXmlSchema(rutaReporte = Utils.RutaReporte + "xmlImage.xml");  
                var DatosEncabezado = d.AS_CONVENIO.Where(sn => sn.CON_ANO_CON == periodo && sn.CON_ROL_NUM == rolM && sn.CON_ROL_DV == rolP).FirstOrDefault();
                Aux_nDecreto = DatosEncabezado.CON_NUM_DECRE.ToString();
                Aux_fDecreto = DatosEncabezado.CON_FEC_DECRE.ToString();
                reportDocument.SetParameterValue("txtNombre", Aux_Nombre);
                reportDocument.SetParameterValue("txtRol", Aux_Rol);
                reportDocument.SetParameterValue("txtnDecreto", Aux_nDecreto);
                reportDocument.SetParameterValue("txtfDecreto", Aux_fDecreto.Substring(0,10));
                reportDocument.SetParameterValue("txtPeriodo", Aux_Periodo);
                
                // Nombre
                string rol = rolM.ToString() + "-" + rolP.ToString(); 
                string archNombreFinal = "Certificado_" + rol + "" + fecha + ".pdf";
                
                Pdf exportPdf = new Pdf();
                string fileName = Utils.RutaPDF + archNombreFinal;
                //   System.IO.File.Delete(fileName);

                exportPdf.crearPdf(fileName, reportDocument);
               
            }
        }

    }

}
