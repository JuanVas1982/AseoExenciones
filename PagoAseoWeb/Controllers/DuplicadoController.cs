using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialAseoWeb.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using SocialAseoWeb.Comun;
using System.Globalization;

namespace SocialAseoWeb.Controllers
{
    public class DuplicadoController : BaseController
    {
        //
        // GET: /Duplicado/

        public ActionResult Index()
        {
            OrdenIngresoAseo OI = new OrdenIngresoAseo();
            List<PPKCC_TRANSAC_ASEO> sn = new List<PPKCC_TRANSAC_ASEO>();
            ViewBag.resultado = sn;
            ViewBag.RolNum = "";
            ViewBag.RolDv = "";
            return View(OI); 
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            OrdenIngresoAseo OI = new OrdenIngresoAseo();
            List<PPKCC_TRANSAC_ASEO> sn = new List<PPKCC_TRANSAC_ASEO>();
            ViewBag.resultado = sn;
            ViewBag.RolNum = "";
            ViewBag.RolDv = "";
            try
            {
                if (form["rolNum"].ToString() != "" && form["rolDv"].ToString() != "")
                {
                    var rolNum = Int32.Parse(form["rolNum"].ToString());
                    var rolDv = Int32.Parse(form["rolDv"].ToString());
                    using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                    {
                        ViewBag.resultado = kcc.PPKCC_TRANSAC_ASEO.Where(d => d.ASE_ROL_NUM == rolNum).Where(d => d.ASE_ROL_DV == rolDv).Where(d => d.ASE_ESTADO_TRANS.Equals("A")).ToList();
                    }
                    using (aseoEntities ase = new aseoEntities())
                    {
                        OI = ase.OrdenIngresoAseo.Where(d => d.rolDv == rolDv).Where(d => d.rolNum == rolNum).First();
                    }
                    ViewBag.RolNum = rolNum.ToString();
                    ViewBag.RolDv = rolDv.ToString();
                    return View(OI);
                }
                else
                {
                    return View(OI);
                }
            }
            catch
            {
                return View(OI);
            }

        }

        public ActionResult BuscarRol(string rol, string dv)
        {
            Dictionary<string, string> datos = new Dictionary<string, string>();
            decimal numRol = 0;
            decimal numDv = 0;
            bool valRol = decimal.TryParse(rol, out numRol);
            bool valDv = decimal.TryParse(dv, out numDv);
            if (valRol == false || valDv == false)
            {
                return Json("El rol se compone de números. Debe Ingresar datos validos.");
            }

            using (PagosPortaleKCCEntities entidadesKCC = new PagosPortaleKCCEntities())
            {
                decimal rol_num = decimal.Parse(rol);
                decimal rol_dv = decimal.Parse(dv);
                var siPagado = (from pago in entidadesKCC.PPKCC_TRANSAC_ASEO
                                where
                                    pago.ASE_ROL_NUM == rol_num &&
                                    pago.ASE_ROL_DV == rol_dv &&
                                    pago.ASE_ESTADO_TRANS.Equals("A")
                                select pago).OrderByDescending(c => c.ASE_FEC_PAG).FirstOrDefault();

                if (siPagado == null)
                {
                    datos = null;
                    return Json("Error: No Existe Registro Con El Rol Ingresado.",JsonRequestBehavior.AllowGet);
                }
                else
                {
                    datos.Add("nOrdIng", siPagado.ASE_FOLIO_TRANS.ToString());
                    datos.Add("rolNum", siPagado.ASE_ROL_NUM.ToString());
                    datos.Add("rolDv", siPagado.ASE_ROL_DV.ToString());
                    return Json(datos);
                }
            }

        }

    
    }
}
