using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialAseoWeb.Models;
namespace SocialAseoWeb.Controllers
{
    public class ValidarController : Controller
    {
        //
        // GET: /Validar/
          
        public ActionResult Index(decimal X, decimal Y, decimal Z)
        {
            using (aseoEntities db = new aseoEntities())
            {           
                //int xID = int.Parse(ID);
                var data = db.AS_CONVENIO.Where(x => x.CON_ANO_CON == Z && x.CON_ROL_NUM == X && x.CON_ROL_DV == Y).FirstOrDefault();

                if (data != null)
                {
                    ViewBag.EncabezadoID = "OK";
                    ViewBag.Solicitud = "S/N";
                    ViewBag.Beneficiario = "";
                    ViewBag.Rol = X + "-" + Y;
                    ViewBag.Decreto = data.CON_NUM_DECRE;
                    ViewBag.FechaDecreto = data.CON_FEC_DECRE; 
                }
                else
                {
                    ViewBag.EncabezadoID = "NOK";
                }
               
            }
            return View();
        }

    }
}
