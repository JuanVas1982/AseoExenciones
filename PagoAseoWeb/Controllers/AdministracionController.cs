using SocialAseoWeb.Comun;
using SocialAseoWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data;

namespace SocialAseoWeb.Controllers
{
    public class AdministracionController : Controller
    {
        //
        // GET: /Administracion/
        accesosEntities entidadesAccesos = new accesosEntities();
        public ActionResult Index()
        {   
           // return View();
            return RedirectToAction("Menu", "Administracion");
        }

        public ActionResult Solicitudes()
        {
             return View();
           // return RedirectToAction("Menu", "Administracion");
        }
        public ActionResult Menu()
        {
            if (SiUsuarioLogueado().Equals(true))
            {
                
                string usuario = Session["usuario"].ToString();
                ViewBag.Usuario = usuario;
                try
                {
                   
                    ViewBag.user = usuario;
                    ViewBag.portal = 0;
                   
                    return View();
                }
                catch (Exception ex)
                {
                    string ns = this.ControllerContext.Controller.GetType().Namespace;
                    string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                    string action = this.ControllerContext.RouteData.Values["action"].ToString();
                  
                    return Json("Error: " + ex.Message);
                }
                finally
                {
                  
                }
            }
            else
            {

                return RedirectToAction("IndexAdmin", "Inicio");
            }

        
        }
        public ActionResult ConfirmarRol(decimal rolNum, decimal rolDv, string periodo, string nOrden)
        {
            List<Social_OrdenIngresoAseo> dbSol = new List<Models.Social_OrdenIngresoAseo>();
            accesosEntities dbAcceso = new Models.accesosEntities();
            using (aseoEntities db = new Models.aseoEntities()){
                var data = db.Social_OrdenIngresoAseo.Where(x => x.rolNum == rolNum && x.rolDv == rolDv && x.periodo == periodo && x.nOrden == nOrden).ToList();
                if (data != null)
                {
                    ViewData["Usuarios"] = dbAcceso.INAC_USUARIO.Where(x => x.USUA_CODIGO_SIST == 39 && x.USUA_VIGENCIA == "S" && x.USUA_CARGO == "ESTRATIFICACION").ToList();
                    return View(data);
                }
            }
     
            return View();
        }
        [HttpPost]
        public ActionResult ConfirmarRol(FormCollection form, HttpPostedFileBase uploadArchivo)
        {
            LogReg log = new LogReg();
            Social_OrdenIngresoAseo oi = new Social_OrdenIngresoAseo();
            AS_CONVENIO oe = new AS_CONVENIO();
            List<DetalleOrdenIngresoAseo> deuda = new List<DetalleOrdenIngresoAseo>();
            AS_CONTRIBUYENTE contr = new AS_CONTRIBUYENTE();
            AS_RUT rutContr = new AS_RUT();

            if (form.Count != 0)
            {
                string estado = form["estado"].ToString();

               

                string rolLog = form["manzana"].ToString() + "-" + form["predio"].ToString();
                string email = form["email"].ToString();
                decimal rolNum = decimal.Parse(form["manzana"].ToString());
                decimal rolDv = decimal.Parse(form["predio"].ToString());
                string nOrden = form["nOrden"].ToString();
                string Obs = form["Obs"].ToString();
                string periodo = form["fecha"].ToString();
                decimal anio = Convert.ToDecimal(periodo.Substring(0, 4));
                string ano = anio.ToString();
                string respo = form["respo"].ToString();

                if (estado == "R")
                {
                    string Text = "";
                    string motivo = estado.Equals("R") ? "Rechazado" : "Derivado";
                    string nemisor = email.ToString();
                    Text = "Solicitud " + nOrden + ".<br>";
                    Text = Text + "El tramite de exencion fue " + motivo + " <br>";
                    Text = Text + "<br>";
                    Text = Text + "Rol :" + rolNum.ToString() + "-" + rolDv.ToString() + "<br>";
                    Text = Text + "Motivo :" + Obs + "<br>";

                    using (aseoEntities db = new aseoEntities())
                    {
                        oi = db.Social_OrdenIngresoAseo.Where(x => x.rolNum.Equals(rolNum) && x.rolDv.Equals(rolDv) && x.periodo.Equals(ano) && x.nOrden == nOrden).FirstOrDefault();
                        oi.respo = Session["usuario"].ToString();
                        oi.estado = estado;
                        oi.Observacion = Obs.ToString();
                        db.SaveChanges();
                    }

                    Correo enviar = new Correo();
                    enviar.enviarCorreo(nemisor, Text, "Rechazo de Solicitud", "");
                    return RedirectToAction("Menu", "Administracion");
                }

                string direccion = form["domicilio"].ToString();
                string numero = form["numero"].ToString();
                string block = form["block"].ToString();
                string depto = form["depto"].ToString();

                string direccionRsh = form["domicilioRsh"].ToString();              
                string numeroRsh = form["numeroRsh"].ToString();
                string blockRsh = form["blockRsh"].ToString();
                string deptoRsh = form["deptoRsh"].ToString();

                string conjunto = form["conjunto"].ToString();
                string uv = form["uv"].ToString();
               
                string telefono = form["fono"].ToString();
                decimal folio = Convert.ToDecimal(form["folio"].ToString());
                
               
                periodo = periodo.Substring(0, 4) + periodo.Substring(5, 2) + periodo.Substring(8, 2);
                string tramo = form["tramo"].ToString();
                
                
                
                string emisor = Utils.adminEMail;
                //string estado = form["estado"].ToString();
                
                try
                {
                    log.abrirArchivo(rolLog);
                    log.registrar("*********************************************");
                    log.registrar(" * PROCESO: ConfirmarExentos");
                    log.registrar("*********************************************");
                    log.registrar(" * ROL: " + rolLog.ToString());
                    log.registrar(" * MAIL: " + email.ToString());
                    log.registrar(" * FONO: " + telefono.ToString());

                    //se busca de todos los registros los que esten chequeados para deuda
                    string sesion = Session.SessionID;
                    log.registrar("--------------------------------");
                                      
                    if (estado == "A")
                    {
                        using (aseoEntities db = new aseoEntities())
                        {
                            oi = db.Social_OrdenIngresoAseo.Where(x => x.rolNum.Equals(rolNum) && x.rolDv.Equals(rolDv) && x.periodo.Equals(ano) && x.nOrden == nOrden).FirstOrDefault();

                            oi.direccion2 = direccionRsh;
                            oi.numero2 = Convert.ToDecimal(numeroRsh);
                            oi.block2 = blockRsh;
                            oi.depto2 = deptoRsh;
                            oi.ch = conjunto;
                            oi.uv = Convert.ToDecimal(uv);
                            oi.fecha_RSH = Convert.ToDecimal(periodo); ;
                            oi.Tramo = Convert.ToDecimal(tramo); 
                            oi.Origen = 1;
                            oi.respo = respo.ToString();
                            oi.estado = estado;
                            oi.folioRSH = folio;
                            oi.Observacion = Obs.ToString();
                            db.SaveChanges();
                        }
                        log.registrar(" * OI: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(oi));
                    }
                    else 
                    {  
                        using (aseoEntities db = new aseoEntities())
                        {
                            oi = db.Social_OrdenIngresoAseo.Where(x => x.rolNum.Equals(rolNum) && x.rolDv.Equals(rolDv) && x.periodo.Equals(ano) && x.nOrden == nOrden).FirstOrDefault();
                            oi.respo = respo.ToString();
                            oi.estado = estado;
                            oi.Observacion = Obs.ToString();
                            db.SaveChanges();
                        }
                        log.registrar(" * OI: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(oi));
                        
                        string Mensaje = "";
                        if(estado == "P"){                                                    
                            Mensaje = "Solicitud " + nOrden + ".<br>";
                            Mensaje = Mensaje + "El tramite de exencion fue asignado a:" + respo + "<br>";
                            Mensaje = Mensaje + "<br>";
                            Mensaje = Mensaje + "Rol :" + rolNum.ToString() + "-" + rolDv.ToString() + "<br>";
                            Mensaje = Mensaje + "Direccion Propiedad :" + direccion + " Numero :" + numero + "<br>";                        
                         }
                        else  {
                            string motivo = estado.Equals("R") ? "Rechazado" : "Derivado";
                            emisor = email.ToString();
                            Mensaje = "Solicitud " + nOrden + ".<br>";
                            Mensaje = Mensaje + "El tramite de exencion fue " + motivo  + " <br>";
                            Mensaje = Mensaje + "<br>";
                            Mensaje = Mensaje + "Rol :" + rolNum.ToString() + "-" + rolDv.ToString() + "<br>";
                            Mensaje = Mensaje + "Motivo :" + Obs + "<br>";  
                        }

                        Correo enviar = new Correo();
                        enviar.enviarCorreo(emisor, Mensaje, "Asignacion de Solicitud", "");
                        return RedirectToAction("Menu", "Administracion");

                        
                    }
                    using (aseoEntities db = new aseoEntities())
                    {
                        var detCuotas = db.Social_DetalleOrdenIngresoAseo.Where(x => x.rolNum == rolNum && x.rolDv == rolDv && x.anio == anio).ToList();
                        foreach (Social_DetalleOrdenIngresoAseo aca in detCuotas)
                        {
                            oe = db.AS_CONVENIO.Where(x => x.CON_ROL_NUM == rolNum && x.CON_ROL_DV == rolDv && x.CON_ANO_CON == anio && x.CON_CUOTA == aca.cuota).FirstOrDefault();
                            oe.CON_MARCA = estado.ToString();
                            db.SaveChanges();
                        }
                       
                    }

                }
                catch (Exception ex)
                {
                    string ns = this.ControllerContext.Controller.GetType().Namespace;
                    string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                    string action = this.ControllerContext.RouteData.Values["action"].ToString();
                    //ManejoErrores(ns, controller, action, ex, log);
                    Session["err-enc"] = "Error en la recepción";
                    Session["err-dat"] = "Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + "; numOrden: ";
                    Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                    Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-fal"] = ex.Message;
                    return RedirectToAction("Mensaje", "Administracion");
                }

                return RedirectToAction("Menu", "Administracion");
            }
            return View("");
        }
        public ActionResult NuevaSolicitud()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NuevoRol(FormCollection form, HttpPostedFileBase uploadRut)
        {
            LogReg log = new LogReg();
            Social_OrdenIngresoAseo oi = new Social_OrdenIngresoAseo();
            AS_CONVENIO oe = new AS_CONVENIO();
            List<Social_DetalleOrdenIngresoAseo> exencion = new List<Social_DetalleOrdenIngresoAseo>();
            AS_CONTRIBUYENTE contr = new AS_CONTRIBUYENTE();
            AS_RUT rutContr = new AS_RUT();
            AS_CONVENIO ap2 = null;

            if (form.Count != 0)
            {
                string rolLog = form["rolA"].ToString() + "-" + form["rolB"].ToString();
                decimal rolNum = decimal.Parse(form["rolA"].ToString());
                decimal rolDv = decimal.Parse(form["rolB"].ToString());
                string nombre = form["nombre"].ToString().ToUpper();
                string[] arrayRow2 = form["rut"].ToString().Split('-');
                string rut = arrayRow2[0].ToString();
                string dv = arrayRow2[1].ToString();
                string direccion = form["domicilio"].ToString().ToUpper();
                decimal numero = decimal.Parse(form["numero"].ToString());
                string depto = form["depto"].ToString();
                string block = form["block"].ToString();
                string direccionRsh = form["domicilioRsh"].ToString().ToUpper();
                decimal numeroRsh = decimal.Parse(form["numeroRsh"].ToString());
                string deptoRsh = form["deptoRsh"].ToString();
                string blockRsh = form["blockRsh"].ToString();
                string conjunto = form["conjunto"].ToString();
                string email = form["email"].ToString();
                string telefono = form["fono"].ToString();
                string uv = form["uv"].ToString();
                string tramo = form["tramo"].ToString();              
                string estado = form["estado"].ToString();                             
                int origen = int.Parse(form["origen"].ToString());
                int cuotas = int.Parse(form["cuotas"].ToString());
                string responsable = form["respo"].ToString(); 

                decimal anio =  DateTime.Today.Year; 
                decimal? Nsolicitud = 0;

                try
                {
                    log.abrirArchivo(rolLog);
                    log.registrar("*********************************************");
                    log.registrar(" * PROCESO: ConfirmarExentos");
                    log.registrar("*********************************************");
                    log.registrar(" * ROL: " + rolLog.ToString());
                    log.registrar(" * MAIL: " + email.ToString());
                    log.registrar(" * FONO: " + telefono.ToString());
                    //se busca folio solicitud dísponible


                    AS_SOLICITUD ia = null;
                    using (aseoEntities entidadAseo = new aseoEntities())
                    {
                        ia = entidadAseo.AS_SOLICITUD.Where(x => x.SOL_TIPO.Equals("S")).FirstOrDefault();
                        if (ia == null)
                        {
                            log.registrar("no existe el folio o no esta cargado");
                        }
                        else
                        {
                            ia.SOL_NUM_SOL = ia.SOL_NUM_SOL + 1;
                            entidadAseo.SaveChanges();
                            Nsolicitud = ia.SOL_NUM_SOL;
                        }


                        HttpFileCollectionBase file = Request.Files;
                        string fecha = DateTime.Now.ToString("yyyyMMdd");
                        string RutFuncionario = form["rut"].ToString();
                        uploadRut = file.Get("uploadRut");
                        string archNombre = (uploadRut.FileName).ToLower();
                        archNombre = rolNum + "-" + rolDv + "_" + rut + ".PDF";
                        string ruta = Server.MapPath("~/UploadedFiles") + "\\" + rut + "\\";
                        Utils.crearDirectorio(ruta);
                        uploadRut.SaveAs(ruta + archNombre);

                        Social_DetalleArchivo data = new Social_DetalleArchivo();
                        data.SD_anio = anio;
                        data.SD_RolA = rolNum;
                        data.SD_RolB = rolDv;
                        data.SD_Cuota = 0;
                        data.SD_Arch_1 = ruta + archNombre;
                        data.SD_Arch_2 = "";
                        data.SD_Estado = "C";
                        entidadAseo.Social_DetalleArchivo.Add(data);
                        entidadAseo.SaveChanges();


                    }

                    //se busca de todos los registros los que esten chequeados excentos
                    string sesion = Session.SessionID;
                    log.registrar("--------------------------------");
                    log.registrar("CUOTAS A EXENTAR");
                    //foreach (var key in form.AllKeys)
                    for (var cuota = 1; cuota <= cuotas; cuota++)
                    {
                 

                 
                        using (aseoEntities entidadAseo = new aseoEntities())
                        {
                            AS_PAGOS ap = entidadAseo.AS_PAGOS.Where(x => x.PAG_ROL_NUM == rolNum && x.PAG_ROL_DV == rolDv && x.PAG_ANO_CON == anio && x.PAG_CUOTA == cuota).FirstOrDefault();
                            log.registrar(" * Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + " Año: " + ap.PAG_ANO_CON.ToString() + " Cuota: " + ap.PAG_CUOTA.ToString() + " Total: " + ap.PAG_TOTAL.ToString());
                            ap2 = entidadAseo.AS_CONVENIO.Where(x => x.CON_ROL_NUM == rolNum && x.CON_ROL_DV == rolDv && x.CON_ANO_CON == anio && x.CON_CUOTA == cuota).FirstOrDefault();

                            Social_DetalleOrdenIngresoAseo doi = new Social_DetalleOrdenIngresoAseo();
                            doi.idSesion = sesion;
                            doi.rolNum = rolNum;
                            doi.rolDv = rolDv;
                            doi.nOrden = Nsolicitud.ToString();
                            doi.denominacion = "Exencion D. Aseo cuota " + ap.PAG_CUOTA.ToString() + " año " + ap.PAG_ANO_CON.ToString();
                            doi.anio = ap.PAG_ANO_CON;
                            doi.cuota = ap.PAG_CUOTA;
                            doi.fecMov = ((DateTime)ap.PAG_FEC_V).ToString("dd-MM-yyyy");
                            doi.monto = ap.PAG_VALOR;
                            doi.porcentaje = ap2.CON_PORCEN;
                            doi.folio = "";
                            entidadAseo.Social_DetalleOrdenIngresoAseo.Add(doi);
                            entidadAseo.SaveChanges();
                            anio = ap.PAG_ANO_CON;

                            ap2.CON_MARCA = estado;
                            entidadAseo.SaveChanges();
  

                        }
                    }
                    log.registrar("--------------------------------");
                    using (aseoEntities entidadesAseo = new aseoEntities())
                    {
                        oi.idSesion = sesion;
                        oi.rut = rut;
                        oi.dv = dv;
                        oi.nombre = nombre;
                        oi.direccion = direccion;
                        oi.numeroD = numero;
                        oi.depto = depto;
                        oi.block = block;
                        oi.direccion2 = direccionRsh;
                        oi.numero2 = numeroRsh;
                        oi.depto2 = deptoRsh;
                        oi.block2 = blockRsh;
                        oi.rolNum = rolNum;
                        oi.rolDv = rolDv;
                        oi.telefono = telefono;
                        oi.eMail = email;
                        oi.fecRecepcion = DateTime.Now.ToString("yyyyMMdd");
                        oi.nOrden = Nsolicitud.ToString();
                        oi.estado = estado;
                        oi.ch = conjunto;
                        oi.uv = 0;
                        oi.periodo = anio.ToString();
                        oi.cuotas = cuotas.ToString();
                        oi.respo = responsable;
                        oi.folioRSH = 0;
                        oi.fecha_RSH = 0;
                        oi.Tramo = 0;
                        oi.Origen = origen;
                        entidadesAseo.Social_OrdenIngresoAseo.Add(oi);
                        entidadesAseo.SaveChanges();

                    }
                    using (aseoEntities db = new aseoEntities())
                    {
                        oe = db.AS_CONVENIO.Where(x => x.CON_ROL_NUM == rolNum && x.CON_ROL_DV == rolDv && x.CON_ANO_CON == anio).FirstOrDefault();
                        oe.CON_MARCA = estado.ToString();
                        db.SaveChanges();
                    }

                    string emisor = email != null ? emisor = email : "jvasquez@munimacul.cl";
                    string Mensaje = "";
                    Mensaje = "Solicitud " + Nsolicitud.ToString() + ".<br>";
                    Mensaje = Mensaje + "El tramite de exencion ha sido recepcionado " + "<br>";
                    Mensaje = Mensaje + "<br>";
                    Mensaje = Mensaje + "Rol :" + rolNum.ToString() + "-" + rolDv.ToString() + "<br>";
                    Mensaje = Mensaje + "Direccion Propiedad :" + direccion + " Numero :" + numero + "<br>";
                    Correo enviar = new Correo();
                    enviar.enviarCorreo(emisor, Mensaje, "Ingreso de Solicitud", "");
                

                }
                catch (Exception ex)
                {
                    string ns = this.ControllerContext.Controller.GetType().Namespace;
                    string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                    string action = this.ControllerContext.RouteData.Values["action"].ToString();
                    //ManejoErrores(ns, controller, action, ex, log);
                    Session["err-enc"] = "Error en la recepción";
                    Session["err-dat"] = "Rol:" + rolNum.ToString() + "-" + rolDv.ToString() + "; numOrden: ";
                    Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                    Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-fal"] = ex.Message;
                    return RedirectToAction("Mensaje", "Administracion");
                }

                return RedirectToAction("Menu", "Administracion");
            }
            return View("");
        }
        public ActionResult mantSolicitud()
        {
            return View();
        }
        public PartialViewResult ListarFormularios(string rol, string dv)
        {
                    
            List<Social_OrdenIngresoAseo> detalle = new List<Social_OrdenIngresoAseo>();
            using (aseoEntities db = new aseoEntities())
            {
                if (rol != null && dv != "")
                {
                    decimal rolA = Convert.ToDecimal(rol);
                    decimal rolB = Convert.ToDecimal(dv);

                    detalle = db.Social_OrdenIngresoAseo.Where(x => x.rolNum == rolA && x.rolDv == rolB && x.estado == "I").ToList();
                }
                else
                {
                    detalle = db.Social_OrdenIngresoAseo.Where(x => x.estado == "I").ToList();
                }
                
            }
            if (detalle == null)
            {
                return PartialView(new List<Social_OrdenIngresoAseo>());
            }
            else
            {

                return PartialView(detalle);
            }
        }

        public PartialViewResult ListarSolicitudes()
        {

            List<Social_OrdenIngresoAseo> detalle = new List<Social_OrdenIngresoAseo>();
            using (aseoEntities db = new aseoEntities())
            {
      
            detalle = db.Social_OrdenIngresoAseo.Where(x => !x.estado.Equals("I")).ToList();
            

            }
            if (detalle == null)
            {
                return PartialView(new List<Social_OrdenIngresoAseo>());
            }
            else
            {

                return PartialView(detalle);
            }
        }

        public PartialViewResult ListarSolicitudesFiltro(decimal rolNum, decimal rolDv, string periodo)
        {

            List<Social_OrdenIngresoAseo> detalle = new List<Social_OrdenIngresoAseo>();
            using (aseoEntities db = new aseoEntities())
            {

                detalle = db.Social_OrdenIngresoAseo.ToList();


            }
            if (detalle == null)
            {
                return PartialView(new List<Social_OrdenIngresoAseo>());
            }
            else
            {

                return PartialView(detalle);
            }
        }
        private bool SiUsuarioLogueado()
        {

            if (Session["ultiMov"] == null)
            {
                if (Request.Cookies["InicioSesion"] != null)
                {
                    if (Request.Cookies["InicioSesion"]["user"] != null)
                    {
                        DateTime ahora = DateTime.Now;
                        HttpCookie myCookie = new HttpCookie("temp");
                        myCookie = Request.Cookies["InicioSesion"];
                        DateTime ultiMov = DateTime.Parse(myCookie["ultimoMovimiento"].ToString());
                        DateTime expira = DateTime.Parse(myCookie["expira"].ToString());
                        string usuario = myCookie["user"].ToString();
                        string contraseña = myCookie["pass"].ToString();
                        decimal ulMov = decimal.Parse(ultiMov.ToString("ddHHmmss"));
                        decimal expi = decimal.Parse(expira.ToString("ddHHmmss"));
                        if (ulMov > expi)
                        {
                            return false;
                        }

                        Session["ultiMov"] = ahora;
                        Session["expiraSes"] = ahora.AddMinutes(120);
                        Session["usuario"] = usuario;
                        Session["contraseña"] = contraseña;

                        Response.Cookies["InicioSesion"]["user"] = Session["usuario"].ToString();
                        Response.Cookies["InicioSesion"]["pass"] = Session["contraseña"].ToString();
                        Response.Cookies["InicioSesion"]["ultimoMovimiento"] = ahora.ToString();
                        Response.Cookies["InicioSesion"]["expira"] = ahora.ToString();
                        Response.Cookies["InicioSesion"].Expires = ahora.AddMinutes(120);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                DateTime ahora = DateTime.Now;
                DateTime expira = DateTime.Parse(Session["expiraSes"].ToString());
                decimal ulMov = decimal.Parse(ahora.ToString("ddHHmmss"));
                decimal expi = decimal.Parse(expira.ToString("ddHHmmss"));
                if (ulMov > expi)
                {
                    return false;
                }
                else
                {


                    Session["ultiMov"] = DateTime.Now;
                    Session["expiraSes"] = DateTime.Now.AddMinutes(120);
                    if (Request.Cookies["InicioSesion"] != null)
                    {
                        if (Request.Cookies["InicioSesion"]["user"] != null)
                        {
                            Response.Cookies["InicioSesion"]["user"] = Session["usuario"].ToString();
                            Response.Cookies["InicioSesion"]["pass"] = Session["contraseña"].ToString();
                            Response.Cookies["InicioSesion"]["ultimoMovimiento"] = ahora.ToString();
                            Response.Cookies["InicioSesion"]["expira"] = ahora.AddMinutes(120).ToString();
                            Response.Cookies["InicioSesion"].Expires = ahora.AddMinutes(120);
                        }
                    }
                    return true;
                }
            }


        }

        public ActionResult CargaArchivoPDF(decimal rolM, decimal rolP)
        {
            string path = Utils.RutaUploaded;
            int añoActual = DateTime.Today.Year;
            using (aseoEntities db = new Models.aseoEntities()){

               var archivo = db.Social_DetalleArchivo.Where(x => x.SD_anio == añoActual && x.SD_RolA == rolM && x.SD_RolB == rolP).FirstOrDefault();
               var filename = archivo.SD_Arch_1.ToString();           
           
            return File(filename, "application/pdf");
            }
        }
        public ActionResult ValidaUsuario(string usuario, string contraseña, int siCierre)
        {

            if (!siCierre.Equals(0))
            {
                if (siCierre.Equals(1))
                {
                    return Json(4);
                }
                else if (siCierre.Equals(2))
                {
                    return Json(5);
                }

            }

            if (usuario.Equals(""))
            {
                return Json(2);
            }

            if (contraseña.Equals(""))
            {
                return Json(3);
            }
            try
            {

                decimal codSis = 39;
                //log.registrar("se consulta en PA_VALIDA_USUARIO_WEB(" + codSis + "," + usuario + "," + contraseña + "," + 1 + ") usuario " + usuario);

                var validacion = entidadesAccesos.PA_VALIDA_USUARIO_WEB(codSis, usuario, contraseña, 1).FirstOrDefault();

                if (validacion == null)
                {
                   // log.registrar("PA_VALIDA_USUARIO_WEB no trajo registros");
                   // log.registrar("se consulta si el usuario existe");
                    var existe = (from usuario_bbdd in entidadesAccesos.INAC_USUARIO
                                  where
                                    usuario_bbdd.USUA_CODIGO_SIST == codSis &&
                                    usuario_bbdd.USUA_CODIGO == usuario
                                  select usuario_bbdd).FirstOrDefault();

                    if (existe == null)
                    {
                        //usuario no existe
                       // log.registrar("Usuario no existe");
                        return Json(0);
                    }
                    else
                    {
                        //contraseña erronea
                       // log.registrar("Contraseña incorrecta");
                        return Json(1);
                    }
                }
                else
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    string inac_user = HttpUtility.UrlDecode(jss.Serialize(validacion));
                    DateTime ahora = DateTime.Now;
                    Response.Cookies["InicioSesion"]["user"] = usuario;
                    Response.Cookies["InicioSesion"]["pass"] = contraseña;
                    Response.Cookies["InicioSesion"]["ultimoMovimiento"] = ahora.ToString();
                    Response.Cookies["InicioSesion"]["expira"] = ahora.AddMinutes(120).ToString();
                    Response.Cookies["InicioSesion"].Expires = ahora.AddMinutes(120);

                    Session["usuario"] = usuario;
                    Session["contraseña"] = contraseña;
                    Session["ultiMov"] = ahora;
                    Session["expiraSes"] = ahora.AddMinutes(120);

                    //log.registrar("logueo exitoso");
                    return Json("exito");
                }

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                //ManejoErrores(ns, controller, action, ex, log);
                return Json("Error: " + ex.Message);
            }
            finally
            {

                //log.cerrar();

            }

        }
        public ActionResult Mensaje()
        {
            string encabezado = (Session["err-enc"] == null) ? "" : Session["err-enc"].ToString();
            string mensaje = (Session["err-msg"] == null) ? "" : Session["err-msg"].ToString();
            string recomendacion = (Session["err-rec"] == null) ? "" : Session["err-rec"].ToString();
            string falla = (Session["err-fal"] == null) ? "" : Session["err-fal"].ToString();
            string data = (Session["err-dat"] == null) ? "" : Session["err-dat"].ToString();
            ViewData["encabezado"] = encabezado;
            ViewData["data"] = data;
            ViewData["mensaje"] = mensaje;
            ViewData["recomendacion"] = recomendacion;
            ViewData["falla"] = falla;
            return View();
        }
        public ActionResult BuscaPropietarioByRut(string rut)
        {
           
            try
            {
                string[] dato = rut.Split('-');
                decimal Rut = Convert.ToDecimal(dato[0].ToString());
                string Dv = dato[1].ToString();
                using (aseoEntities db = new aseoEntities()) { 
                    // log.registrar("PA_VALIDA_USUARIO_WEB no trajo registros");
                    // log.registrar("se consulta si el usuario existe");
                    var existe = db.Buscar_beneficiario(Rut,Dv).FirstOrDefault();
                              

                    if (existe == null)
                    {
                        //usuario no existe
                        // log.registrar("Usuario no existe");
                        return Json(0);
                    }
                    else
                    {
                        //contraseña erronea
                        // log.registrar("Contraseña incorrecta");
                        return Json(existe);
                    }

                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                //ManejoErrores(ns, controller, action, ex, log);
                return Json("Error: " + ex.Message);
            }
            finally
            {

                //log.cerrar();

            }

        }

        public ActionResult ExpSolicitudes()
        {
            aseoEntities db = new Models.aseoEntities();
            int conteo = 1;
            string Nombre = "";
            string APaterno = "";
            string AMaterno = "";
            string Origen = "";
            if (SiUsuarioLogueado().Equals(true))
            {
                try
                {
                    string nombre = "SOLICITUDES PARA EXENCIONES";
                    string nombreArchivo = nombre.Replace(' ', '_') + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
                    //datatable para cargar en el excel
                    DataTable dtDetalleDlC = new DataTable("dtDetalleDlC");
                    dtDetalleDlC.Columns.Add(new DataColumn("N°", typeof(System.Int32), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("AP.Paterno", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("AP.Materno", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Nombres", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Rut", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Digito", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Periodo", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Fo.RSH", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Fo.Hoja", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Tramo", typeof(System.Int32), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Direccion", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Numero", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Manzana", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Predio", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("UV.", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Telefono", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Email", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Cuotas", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("ValorC", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Exencion", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("VAlorExentar", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("Decreto", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("PeriodoDec", typeof(System.String), null, MappingType.Attribute));
                    dtDetalleDlC.Columns.Add(new DataColumn("ViaIngreso", typeof(System.String), null, MappingType.Attribute));

                    var soli = db.Social_OrdenIngresoAseo.Where(x => x.estado == "A").ToList();
                    if (soli != null)
                    {
                       

                        foreach (Social_OrdenIngresoAseo datos in soli)
                        {
                          
                            string[] data = datos.nombre.Split(' ');
                            for (int x = 0; x < data.Count(); x++)
                            {
                                if (data.Count() == 2)
                                {
                                    if (Nombre == "")
                                    {
                                        Nombre = data[x].ToString();
                                    }
                                    
                                    APaterno = data[x].ToString();
                                    AMaterno = "";
                                }
                                if (data.Count() == 3)
                                {
                                    if (Nombre == "")
                                    {
                                        Nombre = data[x].ToString();
                                    }
                                    if (APaterno == "" && x == 1)
                                    {
                                        APaterno = data[x].ToString();
                                    }
                                    
                                    AMaterno = data[x].ToString();
                                }
                                if (data.Count() == 4)
                                {
                                    int pas = data.Count();
                                    if (Nombre == "" || x < 2)
                                    {
                                        Nombre = Nombre + " " + data[x].ToString();
                                    }
                                    if (APaterno == "" && x == 2 )
                                    {
                                        APaterno = data[x].ToString();
                                    }                            
                                    AMaterno = data[x].ToString();
                                }
                            }
                                
                            
                            
                            DataRow row = dtDetalleDlC.NewRow();
                            row["N°"] = conteo;
                            row["AP.Paterno"] = APaterno;
                            row["AP.Materno"] = AMaterno;
                            row["Nombres"] = Nombre;
                            row["Rut"] = datos.rut;
                            row["Digito"] = datos.dv;
                            row["Periodo"] = datos.periodo;
                            row["Fo.RSH"] = datos.folioRSH;
                            row["Fo.Hoja"] = datos.fecha_RSH;
                            row["Tramo"] = datos.Tramo;
                            row["Direccion"] = datos.direccion2;
                            row["Numero"] = datos.numero2;
                            row["Manzana"] = datos.rolNum;
                            row["Predio"] = datos.rolDv;
                            row["UV."] = datos.uv;
                            row["Telefono"] = datos.telefono;
                            row["Email"] = datos.eMail;
                            row["Cuotas"] = datos.cuotas;
                            string Vcuota = Utils.valorneto(Convert.ToDecimal(datos.rolNum), Convert.ToDecimal(datos.rolDv), Convert.ToDecimal(datos.periodo));
                            row["ValorC"] = Vcuota;
                            row["Exencion"] = "SI";
                            row["VAlorExentar"] = Convert.ToDecimal(Vcuota) * 4;
                            row["Decreto"] = "";
                            row["PeriodoDec"] = "";
                            if (datos.Origen == 1)
                            {
                                Origen = "Online";
                            }
                            else if (datos.Origen == 2)
                            {
                                Origen = "Presencial";
                            }
                            else if (datos.Origen == 3)
                            {
                                Origen = "Telefonico";
                            }
                            else
                            {
                                Origen = "Email";
                            }
                            row["ViaIngreso"] = Origen;                         
                            conteo++;
                            dtDetalleDlC.Rows.Add(row);
                            APaterno = "";
                            AMaterno = "";
                            Nombre = "";
                        }
                        
                    }

                    ExportarExcel date = new ExportarExcel();
                    date.CrearHoja(nombre);
                    date.Encabezado(nombre);
                    date.CargaData(dtDetalleDlC);
                    string ruta = date.GetExcel();
                    Response.AddHeader("content-disposition", "attachment;filename=" + nombreArchivo);
                    return new FilePathResult(ruta, "application/ms-excel");
                }
                catch (Exception ex)
                {
                    throw new Exception("V-Se ha producido un error en el metodo ExpDetallePasajero", ex);
                }
            }
            else
            {
                return RedirectToAction("IndexAdmin", "Inicio");
            }

        }
        public ActionResult CierreSesion()
        {
            Session["usuario"] = null;
            Session["contraseña"] = null;
            Session["ultiMov"] = null;
            Session["expiraSes"] = null;

            if (Request.Cookies["InicioSesion"] != null)
            {
                Response.Cookies["InicioSesion"].Expires = DateTime.Now.AddMinutes(0);
            }

            return RedirectToAction("IndexAdmin", "Inicio");
        }
    }
}
