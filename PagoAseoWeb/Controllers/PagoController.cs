using CrystalDecisions.CrystalReports.Engine;
using SocialAseoWeb.Comun;
using SocialAseoWeb.Models;
using SocialAseoWeb.PagosMacul;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Transbank.NET.sample.certificates;
//using Webpay.Transbank.Library;
//using Webpay.Transbank.Library.Wsdl.Normal;
//using Webpay.Transbank.Library.Wsdl.Nullify;
using Transbank.Webpay;
using Transbank.Webpay.Wsdl.Normal; 

namespace SocialAseoWeb.Controllers
{
    public class PagoController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult VerificaDeuda(string rol, string dv)
        {
            LogReg log = new LogReg();
            
            decimal numRol = 0;
            decimal numDv = 0;
            string rolLog = rol + "-" + dv;
            List<AS_PAGOS> deudas = new List<AS_PAGOS>();

            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: VerificaDeuda");
                log.registrar("*********************************************");
                bool valRol = decimal.TryParse(rol, out numRol);
                bool valDv = decimal.TryParse(dv, out numDv);
                if (valRol == false || valDv == false)
                {
                    return Json("El rol se compone de números. Debe Ingresar datos validos.");
                }


                using (aseoEntities entidadesAseo = new aseoEntities())
                {

                    var d = entidadesAseo.AS_PAGOS.Where(x => x.PAG_FEC_P == null && x.PAG_ROL_NUM == numRol && x.PAG_ROL_DV == numDv && x.PAG_MARCA != "C").ToList();
                    if (DateTime.Now.Month < 9)
                    {
                        d = d.Where(x => x.PAG_ANO_CON != DateTime.Now.Year).ToList();
                    }
                    foreach (AS_PAGOS p in d)
                    {
                        /*verifica si exento*/
                        var convenio = entidadesAseo.AS_CONVENIO.Where(x => x.CON_ANO_CON == p.PAG_ANO_CON && x.CON_ROL_NUM == p.PAG_CUOTA && x.CON_ROL_NUM == p.PAG_ROL_NUM && x.CON_ROL_DV == p.PAG_ROL_DV).FirstOrDefault();

                        if (convenio == null)
                        {
                            return Json(new Object());
                        }
                        else if (convenio.CON_CODEX.Equals(5) || convenio.CON_CODEX.Equals(6) || convenio.CON_CODEX.Equals(7))
                        {

                        }
                        else
                        {
                            return Json(new Object());
                        }

                    }

                }
                
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
            }
            return Json("No hay una deuda asociada con el Rol ingresado.");
           
        }

        [HttpGet]
        public PartialViewResult ListarDeuda(string rol, string dv) {

            LogReg log = new LogReg();
            decimal numRol = 0;
            decimal numDv = 0;
            string rolLog = rol + "-" + dv;
            List<AS_PAGOS> deudas = new List<AS_PAGOS>();
            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: ListarDeuda");
                log.registrar("*********************************************");
                bool valRol = decimal.TryParse(rol, out numRol);
                bool valDv = decimal.TryParse(dv, out numDv);

                if (valRol == false || valDv == false)
                {
                    return PartialView(deudas);
                }

                using (aseoEntities entidadesAseo = new aseoEntities())
                {
                    var d = entidadesAseo.AS_PAGOS.Where(x => x.PAG_FEC_P == null && x.PAG_ROL_NUM == numRol && x.PAG_ROL_DV == numDv && x.PAG_MARCA != "C").OrderBy(x => x.PAG_CUOTA).OrderBy(x => x.PAG_ANO_CON).ToList();
                    if (DateTime.Now.Month < 9)
                    {
                        d = d.Where(x => x.PAG_ANO_CON != DateTime.Now.Year).ToList();
                    }
                    foreach (AS_PAGOS p in d)
                    {
                        bool siExento = false;
                        /*verifica si exento*/
                        var convenio = entidadesAseo.AS_CONVENIO.Where(x => x.CON_ANO_CON == p.PAG_ANO_CON && x.CON_ROL_NUM == p.PAG_CUOTA && x.CON_ROL_NUM == p.PAG_ROL_NUM && x.CON_ROL_DV == p.PAG_ROL_DV).FirstOrDefault();

                        if (convenio == null)
                        {

                        }
                        else if (convenio.CON_CODEX.Equals(5) || convenio.CON_CODEX.Equals(6) || convenio.CON_CODEX.Equals(7))
                        {
                            siExento = true;
                        }


                        if (siExento == false)
                        {
                            if (p.PAG_VALOR > 0)
                            {
                                decimal anioActual = DateTime.Now.Year;
                                decimal mesActual = DateTime.Now.Month;
                                decimal valor = decimal.Parse(p.PAG_VALOR.ToString());
                                decimal facIpc = FUNCIONES_DDBB.FN_OBT_IPC(mesActual, anioActual, p.PAG_FEC_V.Value.Month, p.PAG_FEC_V.Value.Year);
                                decimal facMul = FUNCIONES_DDBB.FN_OBT_MULTA(mesActual, anioActual, p.PAG_FEC_V.Value.Month, p.PAG_FEC_V.Value.Year);
                                decimal ipc = decimal.Round((valor * facIpc) / 100, 0);
                                decimal multa = decimal.Round(((valor + ipc) * facMul) / 100);

                                decimal ipcF = (decimal.Round(ipc % 2, 1) == 0.5m) ? 0.1m : 0;
                                decimal multaF = (decimal.Round(multa % 2, 1) == 0.5m) ? 0.1m : 0;

                                ipc = ipc + ipcF;
                                multa = multa + multaF;
                                p.INTERES = decimal.Round(ipc, 0);
                                p.MULTA = decimal.Round(multa, 0);
                                p.TOTAL = decimal.Round(decimal.Parse(p.PAG_VALOR.ToString()) + p.INTERES + p.MULTA, 0);

                                p.PAG_IPC = decimal.Round(ipc, 0);
                                p.PAG_MULTA = decimal.Round(multa, 0);
                                p.PAG_TOTAL = decimal.Round(decimal.Parse(p.PAG_VALOR.ToString()) + p.INTERES + p.MULTA, 0);
                                entidadesAseo.SaveChanges();
                                deudas.Add(p);

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);

            }
           
           
            return PartialView(deudas);
        }
        [HttpGet]
        public PartialViewResult ListarRol(string rol, string dv)
        {

            LogReg log = new LogReg();
            decimal numRol = 0;
            decimal numDv = 0;
            string rolLog = rol + "-" + dv;
            List<AS_CONVENIO> deudas = new List<AS_CONVENIO>();
            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: Listar");
                log.registrar("*********************************************");
                bool valRol = decimal.TryParse(rol, out numRol);
                bool valDv = decimal.TryParse(dv, out numDv);

                if (valRol == false || valDv == false)
                {
                    return PartialView(deudas);
                }

                using (aseoEntities entidadesAseo = new aseoEntities())
                {
                    var d = entidadesAseo.AS_CONVENIO.Where(x => x.CON_ROL_NUM == numRol && x.CON_ROL_DV == numDv && x.CON_MARCA != "S").OrderBy(x => x.CON_CUOTA).OrderBy(x => x.CON_ANO_CON).ToList();
                    if (DateTime.Now.Month < 9)
                    {
                        d = d.Where(x => x.CON_ANO_CON != DateTime.Now.Year).ToList();
                    }
                    foreach (AS_CONVENIO p in d)
                    {
                        bool siExento = false;
                        /*verifica si exento*/
                        //var convenio = entidadesAseo.AS_CONVENIO.Where(x => x.CON_ANO_CON == p.PAG_ANO_CON && x.CON_ROL_NUM == p.PAG_CUOTA && x.CON_ROL_NUM == p.PAG_ROL_NUM && x.CON_ROL_DV == p.PAG_ROL_DV).FirstOrDefault();

                        //if (convenio == null)
                        //{

                        //}
                        //else if (convenio.CON_CODEX.Equals(5) || convenio.CON_CODEX.Equals(6) || convenio.CON_CODEX.Equals(7))
                        //{
                        //    siExento = true;
                        //}


                        if (siExento == false)
                        {
                           
                                decimal anioActual = DateTime.Now.Year;
                                decimal mesActual = DateTime.Now.Month;
                               
                                entidadesAseo.SaveChanges();
                               deudas.Add(p);

                         
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);

            }


            return PartialView(deudas);
        }
        [HttpPost]
       
        public ActionResult ResultadoQR(string url)
        {
            Dictionary<string, string> lista = new Dictionary<string, string>();
            string[] miUrlArr = url.Split(':');
            string rol = miUrlArr[0];
            string folio = miUrlArr[2];
            string total = miUrlArr[1];

            lista.Add("rol", Utils.DesEncriptar(rol));
            lista.Add("folio", Utils.DesEncriptar(folio));
            lista.Add("total", Utils.DesEncriptar(total));

            return View(lista);
        }
        
        public ActionResult Exito()
        {
            LogReg log = new LogReg();
            OrdenIngresoAseo ordenIngreso = new OrdenIngresoAseo();
            List<DetalleOrdenIngresoAseo> detalle = new List<DetalleOrdenIngresoAseo>();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            HttpCookie myCookieCliente = new HttpCookie("DatosPagoAseoTBKTmp");
            WEBPAY wp = new WEBPAY();
            string idSesion = "";
            string nOrden = "0";
            decimal rolNum = 0;
            decimal rolDv = 0;
            string rolLog = rolNum.ToString() + "-" + rolNum.ToString();
            try
            {
                /*
                idSesion = "dlkgubbygiiao5cvod5ioyhp";
                nOrden = "216020004875";
                rolNum = 8029;
                rolDv = 115;*/
                
                if (Request.Cookies["DatosPagoAseoTBK"] == null)
                {
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden;
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo. Debe verificar si las cookies de su navegador estan habilidatas, ya que estas son requeridas para el procesamiento del pago";
                    Session["err-rec"] = "Antes de continuar debe verificar que estas esten activadas. Para esto puede derigirse a <br><ul><li>Chrome: <a href='https://support.google.com/accounts/answer/61416?co=GENIE.Platform%3DDesktop&hl=es' target='_blank'>Aquí</a></li><li>Firefox: <a href='https://support.mozilla.org/es/kb/los-sitios-web-dicen-que-las-cookies-estan-bloquea' target='_blank'>Aquí</a></li><li>Opera: <a href='https://help.opera.com/en/latest/web-preferences/#cookies' target='_blank'>Aquí</a></li><li>Safari: <a href='https://support.apple.com/es-cl/guide/safari/sfri11471/mac' target='_blank'>Aquí</a></li></ul>";
                    Session["err-fal"] = "No estan guardadas las cookies en el navegador.";
                    return RedirectToAction("Mensaje", "Pago");
                }
                else
                {
                    myCookieCliente = Request.Cookies["DatosPagoAseoTBK"];
                    nOrden = myCookieCliente["nOrden"].ToString();
                    rolNum = decimal.Parse(myCookieCliente["rolNum"].ToString());
                    rolDv = decimal.Parse(myCookieCliente["rolDv"].ToString());
                    idSesion = myCookieCliente["sesion"].ToString();
                    rolLog = rolNum.ToString() + "-" + rolNum.ToString();
                }
                
                log.abrirArchivo(rolNum.ToString() + "-" + rolDv.ToString());
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: Exito");
                log.registrar("*********************************************");
                log.registrar("Paso a Exito: " + rolNum.ToString() + "-" + rolDv.ToString() + " nOrden :" + nOrden);
                using (aseoEntities aseo = new aseoEntities())
                {
                    ordenIngreso = (from o in aseo.OrdenIngresoAseo
                                    where o.idSesion == idSesion && o.nOrden == nOrden && o.rolNum == rolNum && o.rolDv == rolDv
                                    select o).FirstOrDefault();
                    detalle = (from o in aseo.DetalleOrdenIngresoAseo
                               where o.idSesion == idSesion && o.nOrden == nOrden && o.rolNum == rolNum && o.rolDv == rolDv
                               select o).ToList();
                }
                if (ordenIngreso == null)
                {
                    log.registrar("No se registra orden de ingreso en la base de datos");
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo.";
                    Session["err-rec"] = "Antes de continuar debe verificar si el saldo fue descontado de su cuenta bancaria";
                    Session["err-fal"] = "No se encontro orden de ingreso relacionada con el pago";
                    return RedirectToAction("Mensaje", "Pago");
                }
                else if (detalle.Count == 0)
                {
                    log.registrar("No se encontro detalle de la orden de ingreso relacionada con el pago");
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo.";
                    Session["err-rec"] = "Antes de continuar debe verificar si el saldo fue descontado de su cuenta bancaria";
                    Session["err-fal"] = "No se encontro detalle de la orden de ingreso relacionada con el pago";
                    return RedirectToAction("Mensaje", "Pago");
                }
                else
                {
                    ordenIngreso.DOI = detalle;
                }
             
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {
                    wp = kcc.WEBPAY.Where(x => x.TBK_ORDEN_COMPRA == ordenIngreso.nOrden).FirstOrDefault();
                    
                }
                if (wp != null)
                {
                    log.registrar("se carga orden de ingreso para pago");
                    log.registrar(" sesion " + idSesion);
                    using (aseoEntities aseo = new aseoEntities())
                    {
                        foreach (DetalleOrdenIngresoAseo doi in detalle)
                        {
                            var pta = aseo.AS_PAGOS.Where(x => x.PAG_ROL_NUM == doi.rolNum && x.PAG_ROL_DV == doi.rolDv && x.PAG_ANO_CON == doi.anio && x.PAG_CUOTA == doi.cuota).FirstOrDefault();
                            if (pta.PAG_FEC_P == null)
                            {
                                decimal caja = 201;
                                string fecVence = doi.fecVence;
                                if (doi.fecVence.Contains("-"))
                                {
                                    string[] arrFec = doi.fecVence.Split('-');
                                    fecVence = arrFec[2] + arrFec[1] + arrFec[0];
                                }
                                RegistraAsPagoAseo(doi.rolNum, doi.rolDv, doi.anio, doi.cuota, Convert.ToDecimal(ordenIngreso.rut), doi.monto, doi.ipc, doi.multa,
                                    DateTime.Now.ToString("yyyyMMdd"), fecVence, Convert.ToDecimal(nOrden), Convert.ToDecimal(wp.TBK_FINAL_NUMERO_TARJETA), caja, log);
                            }                           
                        }
                    }                                    
                }              
                return View(ordenIngreso);
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + "; numOrden: " + nOrden.ToString();
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
            }
            finally
            {
                log.cerrar();
            }                    
        }
  
        public ActionResult ValidaPagoTBK(string folio)
        {
            using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
            {

                try
                {
                    var foliotrans = Convert.ToDecimal(folio.ToString());
                    var ptp1 = (from transaccion in kcc.PPKCC_TRANSAC_ASEO
                                where
                                    transaccion.ASE_FOLIO_TRANS == foliotrans &&
                                    transaccion.ASE_ESTADO_TRANS.Equals("R")
                                select transaccion).FirstOrDefault();
                    kcc.Database.Connection.Close();
                    if (ptp1 != null)
                    {
                        return Json("RECHAZADO", JsonRequestBehavior.AllowGet);
                    }

                    var ptp = (from transaccion in kcc.WEBPAY
                               where
                                   transaccion.TBK_ORDEN_COMPRA == folio
                               select transaccion).FirstOrDefault();
                    if (ptp == null)
                    {
                        return Json("ADEUDADO", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("PAGADO", JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                   
                    return Json("Error: " + ex.Message, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult ConfirmarDeuda(FormCollection form)
        {
            LogReg log = new LogReg();
            OrdenIngresoAseo oi = new OrdenIngresoAseo();
            List<DetalleOrdenIngresoAseo> deuda = new List<DetalleOrdenIngresoAseo>();
            AS_CONTRIBUYENTE contr = new AS_CONTRIBUYENTE();
            AS_RUT rutContr = new AS_RUT();

            string rolLog = form["rolNum"].ToString() + "-" + form["rolDv"].ToString();
            string email = form["email"].ToString();
            string telefono = form["telefono"].ToString();
            decimal rolNum = decimal.Parse(form["rolNum"].ToString());
            decimal rolDv = decimal.Parse(form["rolDv"].ToString());
            
            decimal subtotal = 0;
            decimal ipc = 0;
            decimal multas = 0;
            decimal total = 0;
            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: ConfirmarDeuda");
                log.registrar("*********************************************");
                log.registrar(" * ROL: " + rolLog.ToString());
                log.registrar(" * MAIL: " + email.ToString());
                log.registrar(" * FONO: " + telefono.ToString());

                //se busca de todos los registros los que esten chequeados para deuda
                string sesion = Session.SessionID;
                log.registrar("--------------------------------");
                log.registrar("CUOTAS A PAGAR");
                foreach (var key in form.AllKeys)
                {
                    //var value = form[key];
                    if (key.Contains("chk_"))
                    {
                        if (!key.Equals("chk_todos"))
                        {
                            string[] arrayRow = key.Split('_');
                            decimal anio = decimal.Parse(arrayRow[1].ToString());
                            decimal cuota = decimal.Parse(arrayRow[2].ToString());
                            using (aseoEntities entidadAseo = new aseoEntities())
                            {   
                                AS_PAGOS ap = entidadAseo.AS_PAGOS.Where(x => x.PAG_ROL_NUM == rolNum && x.PAG_ROL_DV == rolDv && x.PAG_ANO_CON == anio && x.PAG_CUOTA == cuota).FirstOrDefault();
                                log.registrar(" * Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + " Año: " + ap.PAG_ANO_CON.ToString() + " Cuota: " + ap.PAG_CUOTA.ToString() + " Total: " + ap.PAG_TOTAL.ToString());
                                DetalleOrdenIngresoAseo doi = new DetalleOrdenIngresoAseo();
                                doi.idSesion = sesion;
                                doi.rolNum = rolNum;
                                doi.rolDv = rolDv;
                                doi.nOrden = "";
                                doi.denominacion = "Pago D. Aseo cuota " + ap.PAG_CUOTA.ToString() + " año " + ap.PAG_ANO_CON.ToString();
                                doi.anio = ap.PAG_ANO_CON;
                                doi.cuota = ap.PAG_CUOTA;
                                doi.fecVence = ((DateTime)ap.PAG_FEC_V).ToString("dd-MM-yyyy");
                                doi.monto = ap.PAG_VALOR;
                                doi.multa = ap.PAG_MULTA;
                                doi.ipc = ap.PAG_IPC;
                                doi.total = ap.PAG_TOTAL;
                                doi.folio = "";
                                subtotal = subtotal + decimal.Parse(ap.PAG_VALOR.ToString());
                                ipc = ipc + decimal.Parse(ap.PAG_IPC.ToString());
                                multas = multas + decimal.Parse(ap.PAG_MULTA.ToString());
                                total = total + decimal.Parse(ap.PAG_TOTAL.ToString());
                                deuda.Add(doi);
                            }

                        }
                    }
                }
                log.registrar("--------------------------------");
                using (aseoEntities entidadesAseo = new aseoEntities())
                {
                    contr = entidadesAseo.AS_CONTRIBUYENTE.Where(x => x.CON_ROL_NUM.Equals(rolNum) && x.CON_ROL_DV.Equals(rolDv)).OrderByDescending(x => x.CON_ANO_CON).FirstOrDefault();
                    rutContr = entidadesAseo.AS_RUT.Where(x => x.RUT_RUT.Equals(contr.CON_RUT)).FirstOrDefault();
                }

                oi.idSesion = sesion;
                oi.rut = contr.CON_RUT.ToString();
                oi.dv = contr.CON_DV;
                oi.nombre = rutContr.RUT_NOMBRE;
                oi.direccion = contr.CON_CALLE + ' ' + contr.CON_NUMERO;
                oi.rolNum = rolNum;
                oi.rolDv = rolDv;
                oi.telefono = telefono;
                oi.eMail = email;
                oi.fecPago = "";
                oi.cuidad = Utils.cuidad;
                oi.nOrden = "";
                oi.tipoTributo = Utils.tipoTributo;
                oi.unidadGiradora = Utils.usuarioWS;
                oi.tipoPago = "";
                oi.tipoCuota = "";
                oi.nCuotas = "";
                oi.tipoTransaccion = "";
                oi.digTarjeta = "";
                oi.codAutoriza = "";
                oi.subtotal = subtotal;
                oi.ipc = ipc;
                oi.multa = multas;
                oi.aPagar = total;
                oi.DOI = deuda;
                log.registrar(" * OI: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(oi));
                
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + "; numOrden: ";
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
            }

            return View(oi);

        }


        /* TBK WEB SERVER */
        public ActionResult TransbankNew(FormCollection form)
        {
            // falta try catch para log
            OrdenIngresoAseo oi = new OrdenIngresoAseo();
            List<DetalleOrdenIngresoAseo> deuda = new List<DetalleOrdenIngresoAseo>();
            Dictionary<string, string> contribuyente = new Dictionary<string, string>();
            string email = form["email"].ToString();
            string telefono = form["telefono"].ToString();
            decimal rolNum = decimal.Parse(form["rolNum"].ToString());
            decimal rolDv = decimal.Parse(form["rolDv"].ToString());
            decimal subtotal = 0;
            decimal ipc = 0;
            decimal multas = 0;
            decimal total = 0;
            decimal nOrden = 0;
            string fecPago = "";
            string sesion = Session.SessionID;

            LogReg log = new LogReg();
            string rolLog = form["rolNum"].ToString() + "-" + form["rolDv"].ToString();
            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: TransbankNew");
                log.registrar("*********************************************");
                /*validamos si hay variables de sesion y cookies ya asignadas */
                nOrden = Convert.ToDecimal(ObtieneFolioTbk(log));
                if (nOrden == 0)
                {
                    log.registrar("VAL: No arrojo numero de folio");
                    Session["err-enc"] = "Error de validación";
                    Session["err-dat"] = "Rol:" + rolLog;
                    Session["err-msg"] = "Se produjo un error inesperado, comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                    Session["err-rec"] = "Envie un E-Mail a la casilla mencionada, adjuntado este fallo.";
                    Session["err-fal"] = "No arrojo numero de folio tbk";
                    return RedirectToAction("Mensaje", "Pago");
                }

                log.registrar("se gurda la orden de ingreso y su detalle en la base de datos");
                /*se genera la orden de ingreso y se guarda en la bbdd*/
                for (int row = 0; row < form.AllKeys.Length; row++)
                {
                    string key = form.AllKeys[row].ToString();
                    if (key.Equals("deuda"))
                    {
                        var value = form[key];
                        string[] miArray = value.Split(',');
                        foreach (string a in miArray)
                        {
                            string[] arrayRow = a.Split('_');
                            decimal anio = decimal.Parse(arrayRow[0].ToString());
                            decimal cuota = decimal.Parse(arrayRow[1].ToString());
                            
                            AS_PAGOS ap = ObtieneAsPagos(rolNum, rolDv,anio,cuota,log);
                            subtotal = subtotal + decimal.Parse(ap.PAG_VALOR.ToString());
                            ipc = ipc + decimal.Parse(ap.PAG_IPC.ToString());
                            multas = multas + decimal.Parse(ap.PAG_MULTA.ToString());
                            total = total + decimal.Parse(ap.PAG_TOTAL.ToString());
                            string denominacion = "Pago D. Aseo cuota " + ap.PAG_CUOTA.ToString() + " año " + ap.PAG_ANO_CON.ToString();
                            DetalleOrdenIngresoAseo doi = RegistraAsDetalleOrdenIngreso(sesion, rolNum, rolDv, nOrden.ToString(), denominacion, anio, cuota,
                                ((DateTime)ap.PAG_FEC_V).ToString("dd-MM-yyyy"), ap.PAG_VALOR, ap.PAG_IPC, ap.PAG_MULTA, ap.PAG_TOTAL, "0", log);
                            deuda.Add(doi);
                           
                        }

                        contribuyente = ObtieneContribuyente(rolNum, rolDv, log);
                        oi = RegistraAsOrdenIngreso(sesion, contribuyente["rut"], contribuyente["dv"], contribuyente["nombre"], contribuyente["direccion"],
                                rolNum, rolDv, telefono, email, fecPago, Utils.cuidad, nOrden.ToString(), Utils.tipoTributo, Utils.usuarioWS, "", "", "", "", "", "", subtotal,
                                ipc, multas, total, log);
                        oi.DOI = deuda;
                    }
                    
                }

                return RedirectToAction("EnviarPagoTransbankNew", "Pago", new { rolNum = oi.rolNum, rolDv = oi.rolDv, sesion = oi.idSesion, nOrden = oi.nOrden });
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
            }

             //return View(oi);
        }

        public ActionResult EnviarPagoTransbankNew(decimal rolNum, decimal rolDv, string sesion, string nOrden)
        {
            OrdenIngresoAseo orIn = new OrdenIngresoAseo();
            List<DetalleOrdenIngresoAseo> deOrIn = new List<DetalleOrdenIngresoAseo>();
            decimal suma = 0;
            string estadoPagoTBK = "I";
            string fecPago = "";
            LogReg log = new LogReg();
            string rolLog = rolNum.ToString() + "-" + rolDv.ToString();
            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: EnviarPagoTransbankNew");
                log.registrar("*********************************************");
                log.registrar(" * ROL NUM: " + rolNum.ToString());
                log.registrar(" * ROL DV: " + rolDv.ToString());
                log.registrar(" * ID SESION: " + sesion.ToString());
                log.registrar(" * FOLIO TBK: " + nOrden.ToString());
                orIn = ObtieneAsOrdenIngreso(rolNum, rolDv, sesion, nOrden, log);
                if (orIn == null)
                {
                    log.registrar("VAL: La orden de ingreso para sesion " + sesion + " no existe ");
                    Session["err-enc"] = "Error de validación";
                    Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Se produjo un error inesperado";
                    Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-fal"] = "Estimado contribuyente no se ha generado la orden de ingreso para ejecutar el pago de los derechos de aseo";
                    return RedirectToAction("Mensaje", "Pago");
                }

                deOrIn = ListaAsDetalleOrdenIngreso(rolNum, rolDv, sesion, nOrden, log);
                if (deOrIn.Count == 0)
                {
                    log.registrar("VAL: el detalle de la orden de ingreso para sesion " + sesion + " no existe ");
                    Session["err-enc"] = "Error de validación";
                    Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Se produjo un error inesperado";
                    Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-fal"] = "Estimado contribuyente no se ha generado el detalle de la orden de ingreso para ejecutar el pago de los derechos de aseo";
                    return RedirectToAction("Mensaje", "Pago");
                }
                orIn.DOI = deOrIn;

                foreach (DetalleOrdenIngresoAseo doi in deOrIn)
                {
                    suma = suma + decimal.Parse(doi.total.ToString());
                }
                decimal amount = Convert.ToDecimal(orIn.aPagar.ToString());
                if (suma != amount)
                {
                    log.registrar("VAL: La suma del detalle de la OI (" + suma + ") y el total de la OI (" + amount + ") no son iguales.");
                    Session["err-enc"] = "Error de validación";
                    Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Se produjo un error inesperado";
                    Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-fal"] = "Estimado contribuyente el total de la orden de ingreso no es igual a la suma del detalle de esta.";
                    return RedirectToAction("Mensaje", "Pago");
                }

                string buyOrder = nOrden.ToString();
                string sessionId = nOrden.ToString();
                wsInitTransactionOutput initResult = InformaVentaTBK(amount, buyOrder, sessionId, log);
                log.registrar(" initResult: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(initResult));
                if (initResult != null)
                {
                    if (!string.IsNullOrEmpty(initResult.token))
                    {
                        string token = initResult.token;
                        BorraAsOrdenIngreso(orIn.rolNum, orIn.rolDv, orIn.idSesion, nOrden.ToString(), log);
                        BorraAsDetalleOrdenIngreso(orIn.rolNum, orIn.rolDv, orIn.idSesion, nOrden.ToString(), log);

                        RegistraAsOrdenIngreso(token, orIn.rut, orIn.dv, orIn.nombre, orIn.direccion,orIn.rolNum,orIn.rolDv,orIn.telefono,orIn.eMail,orIn.fecPago,
                                                orIn.cuidad,orIn.nOrden,orIn.tipoTributo,orIn.unidadGiradora,orIn.tipoPago,orIn.tipoCuota,orIn.nCuotas,orIn.tipoTransaccion,
                                                orIn.digTarjeta,orIn.codAutoriza,orIn.subtotal,orIn.ipc,orIn.multa,orIn.aPagar,log);

                        foreach (DetalleOrdenIngresoAseo doi in deOrIn)
                        {
                            // se regsitra detalle orden de ingreso con token

                            RegistraAsDetalleOrdenIngreso(token, doi.rolNum, doi.rolDv, nOrden, doi.denominacion, doi.anio, doi.cuota,
                                doi.fecVence, doi.monto, doi.ipc, doi.multa, doi.total, doi.folio, log);

                            RegistraKccAseo(doi.rolNum, doi.rolDv, doi.anio, doi.cuota, Convert.ToDecimal(doi.nOrden), estadoPagoTBK, Convert.ToDecimal(doi.monto), 
                                Convert.ToDecimal(doi.ipc), Convert.ToDecimal(doi.multa), Convert.ToDecimal(doi.total), doi.fecVence,fecPago, orIn.telefono, orIn.eMail, 
                                Convert.ToDecimal(doi.folio), orIn.tipoCuota, orIn.nCuotas, orIn.tipoTransaccion, orIn.digTarjeta, Convert.ToDecimal(doi.nOrden), orIn.codAutoriza,"", log);
                        }
                        string data = "sesion=" + token + ";rol=" + rolNum + "-" + rolDv + ";numOrden=" + orIn.nOrden + ";rutCont=" + orIn.rut + "-" + orIn.dv;
                        RegistraKccFoliosAsignados(rolLog, data, "Aseo", Convert.ToDecimal(nOrden), orIn.telefono, orIn.eMail, token, log);

                        log.registrar("Ingresamos los datos a las cookies");
                        HttpCookie myCookie = new HttpCookie("DatosPagoAseoTBK");
                        myCookie["nOrden"] = nOrden.ToString();
                        myCookie["rolNum"] = rolNum.ToString();
                        myCookie["rolDv"] = rolDv.ToString();
                        myCookie["sesion"] = token;
                        myCookie["token_ws"] = token;
                        myCookie["motivo"] = "";
                        myCookie.Expires = DateTime.Now.AddDays(Int32.Parse(Utils.duracionCookie));
                        Response.Cookies.Add(myCookie);

                        log.registrar("Se envia info a TBK");
                        string remotePost = "";
                        remotePost = remotePost + "<html><body onload=\"document.envioPago.submit()\" >";
                        remotePost = remotePost + "<form name=\"envioPago\" action=" + initResult.url + " method='post'><input type='hidden' name='token_ws' value=" + initResult.token + ">";
                        remotePost = remotePost + "</form></body></html>";
                        remotePost = remotePost + "";
                        return Content(remotePost);
                    }
                    else
                    {
                        log.registrar("VAL: token vacio");
                        Session["err-enc"] = "Error en el envío de la transacción a transbank";
                        Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                        Session["err-msg"] = "Se produjo un error inesperado";
                        Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                        Session["err-fal"] = "Token TBK sin datos";
                        return RedirectToAction("Mensaje", "Pago");
                    }
                }
                else
                {
                    log.registrar("VAL: initResult vacio");
                    Session["err-enc"] = "Error en el envío de la transacción a transbank";
                    Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Se produjo un error inesperado";
                    Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-fal"] = "initResult TBK sin datos";
                    return RedirectToAction("Mensaje", "Pago");
                }
               
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + orIn.nOrden.ToString();
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
            }

           
        }

        public ActionResult ResultTBK() {
            HttpCookie myCookieCliente = new HttpCookie("DatosPagoAseoTBKTmp");
            LogReg log = new LogReg();
            string idSesion = "";
            string nOrden = "0";
            decimal rolNum = 0;
            decimal rolDv = 0;
            string motivo = "";
            string token = Request.Form["token_ws"];
            string procesado = "token";
            string rolLog = "0-0";
            if (!string.IsNullOrEmpty(token))
            {
                var fa = ObtieneKccFoliosAsignadosByTokenTbk(token);
                if (fa == null)
                {
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo. Debe verificar si las cookies de su navegador estan habilidatas, ya que estas son requeridas para el procesamiento del pago";
                    Session["err-rec"] = "Antes de continuar debe verificar que estas esten activadas. Para esto puede derigirse a <br><ul><li>Chrome: <a href='https://support.google.com/accounts/answer/61416?co=GENIE.Platform%3DDesktop&hl=es' target='_blank'>Aquí</a></li><li>Firefox: <a href='https://support.mozilla.org/es/kb/los-sitios-web-dicen-que-las-cookies-estan-bloquea' target='_blank'>Aquí</a></li><li>Opera: <a href='https://help.opera.com/en/latest/web-preferences/#cookies' target='_blank'>Aquí</a></li><li>Safari: <a href='https://support.apple.com/es-cl/guide/safari/sfri11471/mac' target='_blank'>Aquí</a></li></ul>";
                    Session["err-fal"] = "No esta el registro de folios asignados para el token.";
                    return RedirectToAction("Mensaje", "Pago");
                }
                rolLog = fa.FA_LLAVE1;
                rolNum = Convert.ToDecimal(rolLog.Split('-')[0]);
                rolDv = Convert.ToDecimal(rolLog.Split('-')[1]);
                nOrden = fa.FA_FOLIO_TRANS.ToString();
                idSesion = token;  
            }
            else
            {
                if (Request.Cookies["DatosPagoAseoTBK"] != null)
                {
                    /* cookies existen. se procesa de manera normal */
                    myCookieCliente = Request.Cookies["DatosPagoAseoTBK"];
                    nOrden = myCookieCliente["nOrden"].ToString(); 
                    rolNum = decimal.Parse(myCookieCliente["rolNum"].ToString());
                    rolDv = decimal.Parse(myCookieCliente["rolDv"].ToString());
                    idSesion = myCookieCliente["sesion"].ToString();
                    token = myCookieCliente["token_ws"].ToString().Trim();
                    procesado = "cookie";
                    rolLog = rolNum.ToString() + "-" + rolDv.ToString();
                }
                else
                {
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo. Debe verificar si las cookies de su navegador estan habilidatas, ya que estas son requeridas para el procesamiento del pago";
                    Session["err-rec"] = "Antes de continuar debe verificar que estas esten activadas. Para esto puede derigirse a <br><ul><li>Chrome: <a href='https://support.google.com/accounts/answer/61416?co=GENIE.Platform%3DDesktop&hl=es' target='_blank'>Aquí</a></li><li>Firefox: <a href='https://support.mozilla.org/es/kb/los-sitios-web-dicen-que-las-cookies-estan-bloquea' target='_blank'>Aquí</a></li><li>Opera: <a href='https://help.opera.com/en/latest/web-preferences/#cookies' target='_blank'>Aquí</a></li><li>Safari: <a href='https://support.apple.com/es-cl/guide/safari/sfri11471/mac' target='_blank'>Aquí</a></li></ul>";
                    Session["err-fal"] = "No estan guardadas las cookies en el navegador.";
                    return RedirectToAction("Mensaje", "Pago");
                }
            }
           
   
      
            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: ResultTBK");
                log.registrar("*********************************************");
                log.registrar(" * ROL NUM: " + rolNum.ToString());
                log.registrar(" * ROL DV: " + rolDv.ToString());
                log.registrar(" * TOKEN: " + token.ToString());
                log.registrar(" * FOLIO TBK: " + nOrden.ToString());
                log.registrar(" * PROCES: " + procesado.ToString());

                string[] keysPost = Request.Form.AllKeys;
                log.registrar("Obtiene Información POST");
                log.registrar(" keysPost: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(keysPost));
    

                transactionResultOutput result = ConfirmaVentaTBK(token, log);
                log.registrar(" result: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                if (result == null)
                {
                    log.registrar("VAL: WS TBK falla en recepcion de información");
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo.";
                    Session["err-rec"] = "Antes de continuar debe verificar si el saldo fue descontado de su cuenta bancaria";
                    Session["err-fal"] = "WS TBK falla en recepcion de información";
                    return RedirectToAction("Mensaje", "Pago");
                }

                if (result.detailOutput[0].responseCode == 0)
                {
                    log.registrar("Cobro existoso");
                    string estadoPago = "A";
                    string fecPago = DateTime.Now.ToString("yyyyMMdd");
                    string hora = DateTime.Now.ToString("HHmmss");
                    decimal amount = result.detailOutput[0].amount;
                    
                    /*EXITO*/
                    OrdenIngresoAseo oi = new OrdenIngresoAseo();
                    List<DetalleOrdenIngresoAseo> doi = new List<DetalleOrdenIngresoAseo>();

                    WEBPAY w= RegistraKccWebpay(result, log);
                    decimal suma = 0;

                    var pfa = ObtieneKccFoliosAsignadosByTokenTbk(token);
                    var pkp = ListaKccAseoByFolioTbk(Convert.ToDecimal(nOrden), log); 
                    if (pfa == null)
                    {
                        log.registrar("registro no esta en PPKCC_FOLIOS_ASIGNADOS");
                    }
                    else if (pkp.Count == 0)
                    {
                        log.registrar("registro no esta en PPKCC_TRANSAC_ASEO");
                    }
                    else
                    {
                        oi = ObtieneAsOrdenIngreso(rolNum, rolDv, token, nOrden, log);
                        if (result == null)
                        {
                            log.registrar("VAL: no existe orden de ingreso");
                            Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                            Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                            Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo.";
                            Session["err-rec"] = "Antes de continuar debe verificar si el saldo fue descontado de su cuenta bancaria";
                            Session["err-fal"] = "No se encontro orden de ingreso relacionada con el pago";
                            return RedirectToAction("Mensaje", "Pago");
                        }
                        doi = ListaAsDetalleOrdenIngreso(rolNum, rolDv, token, nOrden, log);
                        if (result == null)
                        {
                            log.registrar("No se encontro detalle de la orden de ingreso relacionada con el pago");
                            Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                            Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                            Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo.";
                            Session["err-rec"] = "Antes de continuar debe verificar si el saldo fue descontado de su cuenta bancaria";
                            Session["err-fal"] = "No se encontro detalle de la orden de ingreso relacionada con el pago";
                            return RedirectToAction("Mensaje", "Pago");
                        }
                        oi.DOI = doi;
                        //se informa pagada la orden de ingreso
                        BorraAsOrdenIngreso(rolNum, rolDv, token, nOrden, log);

                        RegistraAsOrdenIngreso(token, oi.rut, oi.dv, oi.nombre, oi.direccion, oi.rolNum, oi.rolDv, oi.telefono, oi.eMail, 
                            fecPago,oi.cuidad, nOrden.ToString(), oi.tipoTributo, oi.unidadGiradora, w.TBK_TIPO_PAGO, oi.tipoCuota,
                            w.TBK_NUMERO_CUOTAS, w.TBK_TIPO_TRANSACCION, w.TBK_FINAL_NUMERO_TARJETA, w.TBK_CODIGO_AUTORIZACION,
                            oi.subtotal, oi.ipc, oi.multa, oi.aPagar, log);

                        foreach (DetalleOrdenIngresoAseo reg in doi)
                        {
                            if (ObtieneAsSiAseoPagado(reg.rolNum, reg.rolDv, reg.anio, reg.cuota, log))
                            {
                                decimal caja = 201;
                                string fecVence = reg.fecVence;
                                if (reg.fecVence.Contains("-"))
                                {
                                    string[] arrFec = reg.fecVence.Split('-');
                                    fecVence = arrFec[2] + arrFec[1] + arrFec[0];
                                }
                                RegistraAsPagoAseo(reg.rolNum, reg.rolDv, reg.anio, reg.cuota, Convert.ToDecimal(oi.rut), reg.monto, reg.ipc, reg.multa,
                                    fecPago, fecVence, Convert.ToDecimal(nOrden), Convert.ToDecimal(w.TBK_FINAL_NUMERO_TARJETA), caja, log);

                            }
                               
                        }

                        foreach (PPKCC_TRANSAC_ASEO p in pkp)
                        {
                            //se borra el pendiente
                            BorraKccAseo(p.ASE_ROL_NUM, p.ASE_ROL_DV, p.ASE_ANIO, p.ASE_CUOTA, p.ASE_FOLIO_TRANS, "I", log);
                            //se ingresa el pagado
                            decimal folioTesmu = ObtieneFolioTbkPago(p.ASE_FOLIO_TRANS, p.ASE_TOTAL, log);
                            RegistraKccAseo(p.ASE_ROL_NUM, p.ASE_ROL_DV, p.ASE_ANIO, p.ASE_CUOTA, p.ASE_FOLIO_TRANS, estadoPago, p.ASE_VALOR, p.ASE_IPC, p.ASE_MULTA,
                                p.ASE_TOTAL, p.ASE_FEC_VEN, fecPago, p.ASE_TELEFONO, p.ASE_EMAIL, folioTesmu, w.TBK_TIPO_PAGO, w.TBK_NUMERO_CUOTAS, w.TBK_TIPO_TRANSACCION,
                                w.TBK_FINAL_NUMERO_TARJETA, p.ASE_FOLIO_TRANS, w.TBK_CODIGO_AUTORIZACION, "P", log);
                        }
                              
                    }
                   
                    HttpCookie myCookie = new HttpCookie("DatosPagoAseoTBK");
                    myCookie["nOrden"] = nOrden.ToString();
                    myCookie["rolNum"] = rolNum.ToString();
                    myCookie["rolDv"] = rolDv.ToString();
                    myCookie["sesion"] = token;
                    myCookie["token_ws"] = token;
                    myCookie["motivo"] = "";
                    myCookie.Expires = DateTime.Now.AddDays(Int32.Parse(Utils.duracionCookie));
                    Response.Cookies.Add(myCookie);

                }
                else
                {
                    /*FRACASO*/

                    log.registrar("Cobro rechazado");
                    /** Crea Dictionary con codigos de resultado */
                    Dictionary<string, string> codes = new Dictionary<string, string>();

                    codes.Add("0", "Transacción aprobada");
                    codes.Add("-1", "Rechazo de transacción");
                    codes.Add("-2", "Transacción debe reintentarse");
                    codes.Add("-3", "Error en transacción");
                    codes.Add("-4", "Rechazo de transacción");
                    codes.Add("-5", "Rechazo por error de tasa");
                    codes.Add("-6", "Excede cupo mmáximo mensual");
                    codes.Add("-7", "Excede límite diario por transacción");
                    codes.Add("-8", "Rubro no autorizado");
                    /*FRACASO*/
                    log.registrar("FRACASO Webpay Token");

                    motivo = "Motivo de rechazo:(" + result.detailOutput[0].responseCode.ToString() + ") " + codes[result.detailOutput[0].responseCode.ToString()];
                    log.registrar(motivo);
                    string estadoPago = "R";

                    var pkp = ListaKccAseoByFolioTbk(Convert.ToDecimal(nOrden), log); 
                    foreach (PPKCC_TRANSAC_ASEO p in pkp)
                    {
                        //se borra el pendiente
                        BorraKccAseo(p.ASE_ROL_NUM, p.ASE_ROL_DV, p.ASE_ANIO, p.ASE_CUOTA, p.ASE_FOLIO_TRANS, "I", log);
                        //se ingresa el rechazo
                        RegistraKccAseo(p.ASE_ROL_NUM, p.ASE_ROL_DV, p.ASE_ANIO, p.ASE_CUOTA, p.ASE_FOLIO_TRANS, estadoPago, p.ASE_VALOR, p.ASE_IPC, p.ASE_MULTA,
                            p.ASE_TOTAL, p.ASE_FEC_VEN, "", p.ASE_TELEFONO, p.ASE_EMAIL, Convert.ToDecimal(p.ASE_FOLIO_TESMU), "", "", "", "0", 0, "", "", log);
                    }
                   
                    HttpCookie myCookie = new HttpCookie("DatosPagoAseoTBK");
                    myCookie["nOrden"] = nOrden.ToString();
                    myCookie["rolNum"] = rolNum.ToString();
                    myCookie["rolDv"] = rolDv.ToString();
                    myCookie["sesion"] = token;
                    myCookie["token_ws"] = token;
                    myCookie["motivo"] = motivo;
                    myCookie.Expires = DateTime.Now.AddDays(Int32.Parse(Utils.duracionCookie));
                    Response.Cookies.Add(myCookie);

                }
                string remotePost = "";
                remotePost = remotePost + "<html><body onload=\"document.envioPago.submit()\" >";
                remotePost = remotePost + "<form name=\"envioPago\" action=" + result.urlRedirection + " method='post'><input type='hidden' name='token_ws' value=" + token + ">";
                remotePost = remotePost + "</form></body></html>";
                return Content(remotePost);
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);

                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + "; numOrden: " + nOrden.ToString();
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
           }
           finally
           {
               log.cerrar();
           }

            
        }
     
        public ActionResult EndTBK()
        {
            LogReg log = new LogReg();
            HttpCookie myCookieCliente = new HttpCookie("DatosPagoAseoTBKTmp");
            string nOrden = "0";
            string rolLog = "0-0";
            string motivo = "";
            string token = "";

            if (Request.Cookies["DatosPagoAseoTBK"] == null)
            {
                Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden;
                Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo. Debe verificar si las cookies de su navegador estan habilidatas, ya que estas son requeridas para el procesamiento del pago";
                Session["err-rec"] = "Antes de continuar debe verificar que estas esten activadas. Para esto puede derigirse a <br><ul><li>Chrome: <a href='https://support.google.com/accounts/answer/61416?co=GENIE.Platform%3DDesktop&hl=es' target='_blank'>Aquí</a></li><li>Firefox: <a href='https://support.mozilla.org/es/kb/los-sitios-web-dicen-que-las-cookies-estan-bloquea' target='_blank'>Aquí</a></li><li>Opera: <a href='https://help.opera.com/en/latest/web-preferences/#cookies' target='_blank'>Aquí</a></li><li>Safari: <a href='https://support.apple.com/es-cl/guide/safari/sfri11471/mac' target='_blank'>Aquí</a></li></ul>";
                Session["err-fal"] = "No estan guardadas las cookies en el navegador.";
                return RedirectToAction("Mensaje", "Pago");
            }
            else
            {
                myCookieCliente = Request.Cookies["DatosPagoAseoTBK"];
                nOrden = myCookieCliente["nOrden"];
                rolLog = myCookieCliente["rolNum"] + "-" + myCookieCliente["rolDv"];
                motivo = myCookieCliente["motivo"];
                token = myCookieCliente["token_ws"];
            }
              
            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: EndTBK");
                log.registrar("*********************************************");
                log.registrar(" * ROL: " + rolLog.ToString());
                log.registrar(" * FOLIO TBK: " + nOrden.ToString());
                log.registrar(" * TOKEN: " + token.ToString());
                log.registrar(" * MOTIVO: " + motivo.ToString());
                log.registrar("se consulta si los datos estan el WEBPAY.");
                WEBPAY w = ObtieneKccWebpay(nOrden, log);
                if (w == null)
                {
                    Session["err-enc"] = "Transaccion Rechazada";
                    Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente la transaccion fue rechazada por Transbank";
                    Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-fal"] = (motivo == "") ? "Transaccion cancelada" : motivo;
                    return RedirectToAction("Mensaje", "Pago");
                }
                else
                {
                    return RedirectToAction("Exito", "Pago");
                }    
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rolLog + "; numOrden: " + nOrden.ToString();
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
            }

        }

        public ActionResult PagoPendiente()
        {
            LogReg log = new LogReg();
            OrdenIngresoAseo ordenIngreso = new OrdenIngresoAseo();
            List<DetalleOrdenIngresoAseo> detalle = new List<DetalleOrdenIngresoAseo>();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            HttpCookie myCookieCliente = new HttpCookie("DatosPagoAseoTBKTmp");
            WEBPAY wp = new WEBPAY();
            string idSesion = "";
            string nOrden = "0";
            decimal rolNum = 0;
            decimal rolDv = 0;
            string rolLog = rolNum.ToString() + "-" + rolNum.ToString();
            try
            {

                idSesion = "ec900a2547f1a65ed222218cec993c72ae949a2ba2a231cfbe4c3e4750f07dac";
                nOrden = "216020005543";
                rolNum = 9078;
                rolDv = 470;

               

                log.abrirArchivo(rolNum.ToString() + "-" + rolDv.ToString());
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: Exito");
                log.registrar("*********************************************");
                log.registrar("Paso a Exito: " + rolNum.ToString() + "-" + rolDv.ToString() + " nOrden :" + nOrden);
                using (aseoEntities aseo = new aseoEntities())
                {
                    ordenIngreso = (from o in aseo.OrdenIngresoAseo
                                    where o.idSesion == idSesion && o.nOrden == nOrden && o.rolNum == rolNum && o.rolDv == rolDv
                                    select o).FirstOrDefault();
                    detalle = (from o in aseo.DetalleOrdenIngresoAseo
                               where o.idSesion == idSesion && o.nOrden == nOrden && o.rolNum == rolNum && o.rolDv == rolDv
                               select o).ToList();
                }
                if (ordenIngreso == null)
                {
                    log.registrar("No se registra orden de ingreso en la base de datos");
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo.";
                    Session["err-rec"] = "Antes de continuar debe verificar si el saldo fue descontado de su cuenta bancaria";
                    Session["err-fal"] = "No se encontro orden de ingreso relacionada con el pago";
                    return RedirectToAction("Mensaje", "Pago");
                }
                else if (detalle.Count == 0)
                {
                    log.registrar("No se encontro detalle de la orden de ingreso relacionada con el pago");
                    Session["err-enc"] = "Error en la recepción de la transacción de transbank";
                    Session["err-dat"] = "Rol:" + rolLog.ToString() + "; numOrden: " + nOrden.ToString();
                    Session["err-msg"] = "Estimado contribuyente ha ocurrido un error al procesar el pago del derecho de aseo.";
                    Session["err-rec"] = "Antes de continuar debe verificar si el saldo fue descontado de su cuenta bancaria";
                    Session["err-fal"] = "No se encontro detalle de la orden de ingreso relacionada con el pago";
                    return RedirectToAction("Mensaje", "Pago");
                }
                else
                {
                    ordenIngreso.DOI = detalle;
                }

                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {
                    wp = kcc.WEBPAY.Where(x => x.TBK_ORDEN_COMPRA == ordenIngreso.nOrden).FirstOrDefault();

                }
                if (wp != null)
                {
                    log.registrar("se carga orden de ingreso para pago");
                    log.registrar(" sesion " + idSesion);
                    using (aseoEntities aseo = new aseoEntities())
                    {
                        foreach (DetalleOrdenIngresoAseo doi in detalle)
                        {
                            var pta = aseo.AS_PAGOS.Where(x => x.PAG_ROL_NUM == doi.rolNum && x.PAG_ROL_DV == doi.rolDv && x.PAG_ANO_CON == doi.anio && x.PAG_CUOTA == doi.cuota).FirstOrDefault();
                            if (pta.PAG_FEC_P == null)
                            {
                                decimal caja = 201;
                                string fecVence = doi.fecVence;
                                if (doi.fecVence.Contains("-"))
                                {
                                    string[] arrFec = doi.fecVence.Split('-');
                                    fecVence = arrFec[2] + arrFec[1] + arrFec[0];
                                }
                                RegistraAsPagoAseo(doi.rolNum, doi.rolDv, doi.anio, doi.cuota, Convert.ToDecimal(ordenIngreso.rut), doi.monto, doi.ipc, doi.multa,
                                    DateTime.Now.ToString("yyyyMMdd"), fecVence, Convert.ToDecimal(nOrden), Convert.ToDecimal(wp.TBK_FINAL_NUMERO_TARJETA), caja, log);
                            }
                        }
                    }
                }
                return View(ordenIngreso);
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + "; numOrden: " + nOrden.ToString();
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
            }
            finally
            {
                log.cerrar();
            }
        }
  
    }
}
