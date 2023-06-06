using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Drawing;
using System.Data;

namespace SocialAseoWeb.Comun
{
    public class ExportarExcel
    {
        private ExcelPackage libro = null;
        private Dictionary<string, ExcelWorksheet> hojas = new Dictionary<string, ExcelWorksheet>();
        private ExcelWorksheet hojaActual = null;
        private string ruta = "";
        public int Ult = 0;

        public ExportarExcel()
        {
            ruta = Utils.RutaExcel + "reporte_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
            libro = new ExcelPackage(new FileInfo(ruta));
        }
        public ExportarExcel(string path)
        {
            ruta = path;
            libro = new ExcelPackage(new FileInfo(ruta));
        }
        public void CrearHoja(string nombre)
        {
            hojas.Add(nombre, libro.Workbook.Worksheets.Add(nombre));
        }
        public void Encabezado(string nombre)
        {
            hojaActual = hojas[nombre];
            hojaActual.Cells["B1"].Value = "I. MUNICIPALIDAD DE MACUL";
            hojaActual.Cells["B2"].Value = "DIRECCION DE FINANZAS";
            hojaActual.Cells["B3"].Value = "SECCION DE SOCIAL";
            hojaActual.Cells["E3"].Value = "SOLICITUDES DE EXENCIONES DERECHOS DE ASEO";
            hojaActual.Cells["B5"].Style.Font.Size = 10;
            hojaActual.Cells["B5"].Style.Font.Bold = true;
            hojaActual.Cells["B5"].Style.Font.Color.SetColor(System.Drawing.Color.Gray);
        }
        public void CargaData(DataTable dt)
        {
            int i = dt.Columns.Count;
            int J = dt.Rows.Count;


            //data
            hojaActual.Cells["A7"].LoadFromDataTable(dt, true);
            hojaActual.Cells[hojaActual.Dimension.Address].AutoFitColumns();

            J = J + 7;
            string rango = "A7:X" + J.ToString();
            hojaActual.Cells[rango].Style.Border.Left.Style = ExcelBorderStyle.Double;
            hojaActual.Cells[rango].Style.Border.Top.Style = ExcelBorderStyle.Double;
            hojaActual.Cells[rango].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            hojaActual.Cells[rango].Style.Border.Right.Style = ExcelBorderStyle.Double;

            Ult = J;
        }

     
      
        public string GetExcel()
        {
            libro.Save();
            return ruta;
        }
    }
}