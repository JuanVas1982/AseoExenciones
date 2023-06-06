using SocialAseoWeb.Comun;
using SocialAseoWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialAseoWeb.Controllers;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using Aspose.Words;
using Aspose.Words.Saving;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace SocialAseoWeb.Controllers
{
    public class InicioController : Controller
    {
        //
        // GET: /Inicio/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Inicio/Details/5
    
        [HttpGet]
        public PartialViewResult ListarRol(string rol, string dv)
        {
            List<AS_CONVENIO> deudas = new List<AS_CONVENIO>();

            using (aseoEntities entidadesAseo = new aseoEntities())
            {
                decimal numRol = Convert.ToDecimal(rol);
                decimal numDv = Convert.ToDecimal(dv);
                var Resp = (from Tconv in entidadesAseo.AS_CONVENIO
                            from TPag in entidadesAseo.AS_PAGOS
                            where Tconv.CON_ANO_CON == (DateTime.Now.Year - 2) &&
                                    Tconv.CON_ROL_NUM == numRol &&
                                    Tconv.CON_ROL_DV == numDv &&
                                    Tconv.CON_ANO_CON == TPag.PAG_ANO_CON &&
                                    Tconv.CON_ROL_NUM == TPag.PAG_ROL_NUM &&
                                    Tconv.CON_ROL_DV == TPag.PAG_ROL_DV &&
                                    Tconv.CON_CUOTA == TPag.PAG_CUOTA &&
                                    Tconv.CON_CODEX == 8
                            select Tconv).ToList();
                deudas.AddRange(Resp);
                var Resp1 = (from Tconv in entidadesAseo.AS_CONVENIO
                             from TPag in entidadesAseo.AS_PAGOS
                             where Tconv.CON_ANO_CON == (DateTime.Now.Year - 1) &&
                                     Tconv.CON_ROL_NUM == numRol &&
                                     Tconv.CON_ROL_DV == numDv &&
                                     Tconv.CON_ANO_CON == TPag.PAG_ANO_CON &&
                                     Tconv.CON_ROL_NUM == TPag.PAG_ROL_NUM &&
                                     Tconv.CON_ROL_DV == TPag.PAG_ROL_DV &&
                                     Tconv.CON_CUOTA == TPag.PAG_CUOTA &&
                                     Tconv.CON_CODEX == 8
                             select Tconv).ToList();
                deudas.AddRange(Resp1);
                var Resp2 = (from Tconv in entidadesAseo.AS_CONVENIO
                             from TPag in entidadesAseo.AS_PAGOS
                             where Tconv.CON_ANO_CON == (DateTime.Now.Year) &&
                                     Tconv.CON_ROL_NUM == numRol &&
                                     Tconv.CON_ROL_DV == numDv &&
                                     Tconv.CON_ANO_CON == TPag.PAG_ANO_CON &&
                                     Tconv.CON_ROL_NUM == TPag.PAG_ROL_NUM &&
                                     Tconv.CON_ROL_DV == TPag.PAG_ROL_DV &&
                                     Tconv.CON_CUOTA == TPag.PAG_CUOTA &&
                                     TPag.PAG_FEC_P == null &&
                                     (Tconv.CON_CODEX <= 4 || Tconv.CON_CODEX == 8)

                             select Tconv).ToList();

                deudas.AddRange(Resp2);
                           


                    var data = entidadesAseo.AS_CONTRIBUYENTE.Where(x => x.CON_ROL_NUM == numRol && x.CON_ROL_DV == numDv).FirstOrDefault();

                    if (data != null)
                    {
                        var nombre = entidadesAseo.AS_RUT.Where(x => x.RUT_RUT == data.CON_RUT).Select(x => x.RUT_NOMBRE).FirstOrDefault();
                        ViewBag.nombre = nombre;
                        ViewBag.direccion = data.CON_CALLE;
                        ViewBag.rutCont = data.CON_RUT + "-" + data.CON_DV;
                    }

                
            }
            return PartialView(deudas);
        }
        public JsonResult VerificaRol(string rol, string dv)
        {
            LogReg log = new LogReg();

            decimal numRol = 0;
            decimal numDv = 0;
            string rolLog = rol + "-" + dv;
            //List<AS_PAGOS> deudas = new List<AS_PAGOS>();

            try
            {
                log.abrirArchivo(rolLog);
                log.registrar("*********************************************");
                log.registrar(" * PROCESO: VerificaExiste");
                log.registrar("*********************************************");
                bool valRol = decimal.TryParse(rol, out numRol);
                bool valDv = decimal.TryParse(dv, out numDv);
                if (valRol == false || valDv == false)
                {
                    return Json("El rol se compone de números. Debe Ingresar datos validos.");
                }


                using (aseoEntities entidadesAseo = new aseoEntities())
                {

                    //var d = entidadesAseo.AS_PAGOS.Where(x => x.PAG_FEC_P == null && x.PAG_ROL_NUM == numRol && x.PAG_ROL_DV == numDv && x.PAG_MARCA != "C").ToList();
                    var data = entidadesAseo.AS_CONTRIBUYENTE.Where(x => x.CON_ROL_NUM == numRol && x.CON_ROL_DV == numDv).FirstOrDefault();

                    if (data != null)
                    {
                        return Json(new Object());
                    }
                    else
                    {
                        return Json("El rol no existe en registro del municipio. Debe Ingresar datos validos.");
                    }
             
                    
                }

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                //ManejoErrores(ns, controller, action, ex, log);
            }
            return Json("No hay una deuda asociada con el Rol ingresado.");

        }
       
        public ActionResult ConfirmarRol()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ConfirmarRol(FormCollection form, HttpPostedFileBase uploadRut)
        {
            LogReg log = new LogReg();
            Social_OrdenIngresoAseo oi = new Social_OrdenIngresoAseo();
            List<Social_DetalleOrdenIngresoAseo> exencion = new List<Social_DetalleOrdenIngresoAseo>();
            AS_CONTRIBUYENTE contr = new AS_CONTRIBUYENTE();
            AS_RUT rutContr = new AS_RUT();
            AS_CONVENIO ap2 = null;
               
            if (form.Count != 0)
            {      
                string rolLog = form["rolA"].ToString() + "-" + form["rolB"].ToString();
                string email = form["email"].ToString();
                string telefono = form["telefono"].ToString();
                string[] arrayRow2 = form["rut"].ToString().Split('-');
                string rut = arrayRow2[0].ToString();
                string dv = arrayRow2[1].ToString();
                string nombre = form["nombre"].ToString().ToUpper();
                string direccion = form["direccion"].ToString().ToUpper();
                string block = form["block"].ToString();
                decimal numero =  decimal.Parse(form["numero"].ToString());
                decimal rolNum = decimal.Parse(form["rolA"].ToString());
                decimal rolDv = decimal.Parse(form["rolB"].ToString());
                decimal anio = DateTime.Today.Year;
                decimal? Nsolicitud = 0;
                int conteo = 0;

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
                     archNombre = rolNum +"-"+ rolDv +"_"+ rut + ".PDF";
                     string ruta = Server.MapPath("~/UploadedFiles") + "\\" + rut + "\\";
                     Utils.crearDirectorio(ruta);
                     uploadRut.SaveAs(ruta + archNombre);

                     var existencia = entidadAseo.Social_DetalleArchivo.Where(x => x.SD_anio == anio && x.SD_RolA == rolNum && x.SD_RolB == rolDv).FirstOrDefault();
                     if (existencia == null)
                     {
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
                     }else
                     {
                         existencia.SD_Arch_1 = ruta + archNombre;
                         existencia.SD_Arch_2 = "";
                         existencia.SD_Estado = "C";
                         entidadAseo.SaveChanges();
                     }
                    
     

                }

                    //se busca de todos los registros los que esten chequeados excentos
                    string sesion = Session.SessionID;
                    log.registrar("--------------------------------");
                    log.registrar("CUOTAS A EXENTAR");
                    foreach (var key in form.AllKeys)
                    {
                
                        if (key.Contains("chk_"))
                        {
                            
                            if (!key.Equals("chk_todos"))
                            {
                               
                                string[] arrayRow = key.Split('_');
                                 anio = decimal.Parse(arrayRow[1].ToString());
                                decimal cuota = decimal.Parse(arrayRow[2].ToString());
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
                                    anio= ap.PAG_ANO_CON;

                                    ap2.CON_MARCA = "I";
                                    entidadAseo.SaveChanges();
                                    conteo = conteo + 1;


                                }

                            }
                        }
                    }
                    log.registrar("--------------------------------");
                    using (aseoEntities entidadesAseo = new aseoEntities())
                    {
                        contr = entidadesAseo.AS_CONTRIBUYENTE.Where(x => x.CON_ROL_NUM.Equals(rolNum) && x.CON_ROL_DV.Equals(rolDv)).OrderByDescending(x => x.CON_ANO_CON).FirstOrDefault();
                        rutContr = entidadesAseo.AS_RUT.Where(x => x.RUT_RUT.Equals(contr.CON_RUT)).FirstOrDefault();
                        
                    oi.idSesion = sesion;
                    oi.rut = rut;
                    oi.dv = dv;
                    oi.nombre = nombre;
                    oi.direccion = direccion;
                    oi.numeroD = numero;
                    oi.depto = "";
                    oi.block = block;
                    oi.direccion2 = "";
                    oi.depto2 = "";
                    oi.block2 = "";
                    oi.rolNum = rolNum;
                    oi.rolDv = rolDv;
                    oi.telefono = telefono;
                    oi.eMail = email;
                    oi.fecRecepcion = DateTime.Now.ToString("yyyyMMdd");
                    oi.nOrden = Nsolicitud.ToString();
                    oi.estado = "I";
                    oi.ch = "";
                    oi.uv = 0;
                    oi.periodo = anio.ToString();
                    oi.cuotas = conteo.ToString();
                    oi.respo = "";
                    oi.folioRSH = 0;
                    oi.fecha_RSH = 0;
                    oi.Tramo = 0;
                    oi.Origen = 1;
                    entidadesAseo.Social_OrdenIngresoAseo.Add(oi);
                    entidadesAseo.SaveChanges();
                    //entidadesAseo.Database.Connection.Close();
                   
                    }

                   /// log.registrar(" * OI: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(oi));



                    string emisor = email != null ? emisor = email : "jvasquez@munimacul.cl";
                    string Mensaje = "";
                    Mensaje = "Solicitud " + Nsolicitud.ToString() + ".<br>";
                    Mensaje = Mensaje + "El tramite de exencion ha sido recepcionado " + "<br>";
                    Mensaje = Mensaje + "<br>";
                    Mensaje = Mensaje + "Rol :" + rolNum.ToString() + "-" + rolDv.ToString() + "<br>";
                    Mensaje = Mensaje + "Direccion Propiedad :" + direccion + " Numero :" + numero + "<br>";
                    Correo enviar = new Correo();
                    enviar.enviarCorreo(emisor, Mensaje, "Ingreso de Solicitud","");

                    Session["err-enc"] = "El tramite de exencion ha sido recepcionado " + "<br>";
                    Session["err-dat"] = "Folio = " + Nsolicitud.ToString() + "<br>";
                    Session["err-msg"] = Mensaje;
                    Session["err-fal"] = "Recepción solicitud exención.";
                    //Session["err-fal"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                    Session["err-rec"] = "Si la solicitud tiene algún cambio, esta será notificada por correo electrónico.";

                    return RedirectToAction("Mensaje", "inicio");

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
                    return RedirectToAction("Mensaje", "inicio");
                }

                //return View(oi);
            }
            return View("");
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

        public PartialViewResult listaDeArchivos(string placaTex, decimal placaNum, string proceso)
        {
            LogReg log = new LogReg();
            string placa = placaTex + placaNum.ToString();
            int añoActual = DateTime.Today.Year;
            try
            {
                log.abrirArchivo(placa.Replace("-", ""));
                log.registrar("Se consulta si existe el registro en listado de TABLE_PARTICULAR_ARCHIVOS_DETALLE");
                log.registrar("se llama a la tabla TABLE_PARTICULAR_ARCHIVOS_DETALLE para el rut " + placa + ")");
                proceso = (proceso == "") ? "0" : proceso;
                ViewBag.proceso = proceso;
                using (aseoEntities db = new aseoEntities())
                {


                    TABLE_PARTICULAR_ARCHIVOS_ENCABEZADO datosTpae = db.TABLE_PARTICULAR_ARCHIVOS_ENCABEZADO.Where(x =>
                                    x.TPAE_MANZANA == placaTex &&
                                    x.TPAE_PREDIO == placaNum &&
                                    x.TPAE_ANO == añoActual).FirstOrDefault();
                    if (datosTpae == null)
                    {
                        ViewBag.estado = "Aun no se han subido todos los archivos. Debe finalizar este proceso.";
                    }

                    var datosConsulta = db.TABLE_PARTICULAR_ARCHIVOS_DETALLE.Where(x =>
                                                           x.TPAD_MANZANA == placaTex &&
                                                           x.TPAD_PREDIO == placaNum &&
                                                           x.TPAD_ESTADO != 3 &&
                                                           x.TPAD_ANO == añoActual).ToList();


                    if (datosConsulta.Count == 0)
                    {
                        log.registrar("no existe registro en TABLE_PARTICULAR_ARCHIVOS_DETALLE");
                        return PartialView(new List<TABLE_PARTICULAR_ARCHIVOS_DETALLE>());
                    }
                    else
                    {
                        log.registrar("existe registro en TABLE_PARTICULAR_ARCHIVOS_DETALLE");
                        log.registrar("se consulta si registro esta listo para proceso de pago");
                        return PartialView(datosConsulta);
                    }
                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                //ManejoErrores(ns, controller, action, ex, log);
            }
            finally
            {
                log.cerrar();
            }

            return PartialView(new TABLE_PARTICULAR_ARCHIVOS_DETALLE());

        }

        public ActionResult IndexAdmin()
        {
            if (SiUsuarioLogueado().Equals(true))
            {
                return RedirectToAction("Index", "Administracion");
            }
            return View();
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
                DateTime ultiMov = DateTime.Parse(Session["ultiMov"].ToString());
                DateTime expira = DateTime.Parse(Session["expiraSes"].ToString());
                decimal ulMov = decimal.Parse(ultiMov.ToString("ddHHmmss"));
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

        public FileStreamResult CargaCertPDF(decimal rolM, decimal rolP, decimal periodo)
        {
          
                using (aseoEntities db = new aseoEntities())
                {
                    string fecha = DateTime.Now.ToString("yyyyMMdd");
                    List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                   
                        Pdf.GeneraCertificado(rolM, rolP, periodo);
                        string rol = rolM.ToString() + "-" + rolP.ToString();
                        string archNombreFinal = "Certificado_" + rol + "" + fecha + ".pdf";
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("tipo", "Certificado");
                        dic.Add("archivo", Utils.RutaPDF + archNombreFinal);
                        data.Add(dic);

                        byte[] fileBytes = System.IO.File.ReadAllBytes(Utils.RutaPDF + archNombreFinal);
                        MemoryStream stream = new MemoryStream(fileBytes, 0, fileBytes.Length, true, true);
                        stream.Position = 0;
                        Response.AddHeader("content-disposition", "attachment;filename=Certificado.PDF");
                        return new FileStreamResult(stream, "application/pdf");
                                 
                }

        }

        public ActionResult ValidaSolicitud(decimal rol, decimal dv, string rut)
        {
            string añoActual = (DateTime.Today.Year).ToString();
            string[] rutArray = rut.Split('-');
            string rutA = rutArray[0];
            using(aseoEntities db = new aseoEntities()){
                var registros = db.Social_OrdenIngresoAseo.Where(x => x.periodo == añoActual && x.rut == rutA).ToList();
                foreach (var datos in registros)
                {
                    var data = db.Social_OrdenIngresoAseo.Where(x => x.rut == rutA && x.rolNum == rol && x.rolDv == dv).FirstOrDefault();
                    if (data != null)
                    {
                        if (data.estado == "I")
                        {
                            return Json("INFORMADO");
                        }
                        else if (data.estado == "D")
                        {
                            return Json("DERIVADO");
                        }
                        else if (data.estado == "A")
                        {
                            return Json("ACEPTADO");
                        }
                        else if (data.estado == "P")
                        {
                            return Json("ASIGNADO");
                        }                     
                    }
                    else
                    {
                        return Json("EXISTE RUT");
                    }
                }
       
            }

            
           return Json(0);
        }

        private ActionResult GeneraWord(string placaTex, decimal placaNum, string proceso)
        {
            object ObjMiss = System.Reflection.Missing.Value;
            string rutaS = Server.MapPath("~/Reporte/");
            Word.Application ObjWord = new Word.Application();
            string ruta = Utils.RutaWord + "Plantilla.docx";
            object parametro = ruta;
            object nombre1 = "nombre";
            object direccion1 = "direccion";
            
            Word.Document ObjDoc = ObjWord.Documents.Open(parametro, ObjMiss);
            Word.Range nom = ObjDoc.Bookmarks.get_Item(ref nombre1).Range;
            nom.Text = "Jose Antonio Roman Valdes";
            Word.Range dir = ObjDoc.Bookmarks.get_Item(ref direccion1).Range;
            dir.Text = "La Porfia 607, La Florida";
            object rango1 = nom;
            object rango2 = dir;
            ObjDoc.Bookmarks.Add("nombre", ref rango1);
            ObjDoc.Bookmarks.Add("direccion", ref rango2);
            
            object format = Word.WdSaveFormat.wdFormatPDF;

            object fileName = @"D:\Publicacion\Archivos\SocialAseoWeb\Pdf\Certificado_"  + DateTime.Now.ToString("yyyyMMdd") + ".pdf";
   
            ObjWord.ActiveDocument.SaveAs2(ref fileName, format, ObjMiss);
            ObjWord.Visible = false;
            ObjWord.Documents.Close(Word.WdSaveOptions.wdDoNotSaveChanges, ObjMiss);
            ObjWord.Quit(Word.WdSaveOptions.wdDoNotSaveChanges, ObjMiss);

            Response.ContentType = "application/pdf";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.TransmitFile(fileName.ToString());
            Response.End();

            return File(fileName.ToString(), System.Net.Mime.MediaTypeNames.Application.Octet, "Certificado.pdf");

                    /*
         byte[] fileBytes = System.IO.File.ReadAllBytes(fileName.ToString());
         MemoryStream stream = new MemoryStream(fileBytes, 0, fileBytes.Length, true, true);
         stream.Position = 0;
         Response.AddHeader("content-disposition", "attachment;filename=Certificado.PDF");
         return new FileStreamResult(stream, "application/pdf");
        */
     
        }


    }
}
