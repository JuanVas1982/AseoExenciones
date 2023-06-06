using CrystalDecisions.CrystalReports.Engine;
using SocialAseoWeb.Comun;
using SocialAseoWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using System.Web.Mvc;
using Transbank.NET.sample.certificates;
using Transbank.Webpay;
using Transbank.Webpay.Wsdl.Normal;
using Transbank.Webpay.Wsdl.Nullify;

namespace SocialAseoWeb.Controllers
{
    public class BaseController : Controller
    {
        #region BBDD

        //REGISTRA
        public WEBPAY RegistraKccWebpay(transactionResultOutput result, LogReg log)
        {
            WEBPAY w = null;
            try
            {
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {

                    log.registrar("RegistraKccWebpay");
                    log.registrar(" Filtros: ");
                    log.registrar("     folio: " + result.buyOrder.ToString());
                    w = kcc.WEBPAY.Where(x => x.TBK_ORDEN_COMPRA == result.buyOrder).OrderByDescending(x => x.TBK_HORA_TRANSACCION).FirstOrDefault();
                    if (w == null)
                    {
                        WEBPAY pw = new WEBPAY();
                        pw.TBK_TIPO_TRANSACCION = Utils.tipoTransaccion;
                        pw.TBK_RESPUESTA = result.detailOutput[0].responseCode.ToString();
                        pw.TBK_ORDEN_COMPRA = result.buyOrder;
                        pw.TBK_ID_SESION = result.buyOrder;
                        pw.TBK_CODIGO_AUTORIZACION = result.detailOutput[0].authorizationCode;
                        pw.TBK_MONTO = result.detailOutput[0].amount;
                        pw.TBK_FINAL_NUMERO_TARJETA = result.cardDetail.cardNumber;
                        pw.TBK_FECHA_EXPIRACION = "";
                        pw.TBK_FECHA_CONTABLE = result.accountingDate;
                        pw.TBK_FECHA_TRANSACCION = result.transactionDate.ToString("MMdd");
                        pw.TBK_HORA_TRANSACCION = result.transactionDate.ToString("HHmmss");
                        pw.TBK_ID_TRANSACCION = result.sessionId;
                        pw.TBK_TIPO_PAGO = result.detailOutput[0].paymentTypeCode;
                        pw.TBK_NUMERO_CUOTAS = "";
                        pw.TBK_MAC = "";
                        pw.TBK_TASA_INTERES_MAX = "";
                        kcc.WEBPAY.Add(pw);
                        kcc.SaveChanges();
                        w = pw;
                    }
                    else
                    {
                        log.registrar("data existe en WEBPAY no se ingresa");
                    }
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(w));
                return w;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return w;
            }

        }

        public PPKCC_FOLIOS_ASIGNADOS RegistraKccFoliosAsignados(string rol, string data, string portal, decimal nOrden, string telefono, string mail, string token, LogReg log)
        {
            PPKCC_FOLIOS_ASIGNADOS fa = null;
            try
            {
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {
                    /*
                    if (placa == null) { log.registrar("ERROR VALIDACION (RegistraKccFoliosAsignados): placa no ingresada"); }
                    if (placa == null) { log.registrar("ERROR VALIDACION (RegistraKccFoliosAsignados): placa no ingresada"); }
                    if (placa == null) { log.registrar("ERROR VALIDACION (RegistraKccFoliosAsignados): placa no ingresada"); }
                    if (placa == null) { log.registrar("ERROR VALIDACION (RegistraKccFoliosAsignados): placa no ingresada"); }
                    if (placa == null) { log.registrar("ERROR VALIDACION (RegistraKccFoliosAsignados): placa no ingresada"); }
                    if (placa == null) { log.registrar("ERROR VALIDACION (RegistraKccFoliosAsignados): placa no ingresada"); }
                    if (placa == null) { log.registrar("ERROR VALIDACION (RegistraKccFoliosAsignados): placa no ingresada"); }
                    */
                    log.registrar("RegistraKccFoliosAsignados");
                    log.registrar(" Filtros: ");
                    log.registrar("     folio: " + nOrden.ToString());
                    fa = kcc.PPKCC_FOLIOS_ASIGNADOS.Where(x => x.FA_FOLIO_TRANS == nOrden).FirstOrDefault();
                    if (fa != null)
                    {
                        kcc.PPKCC_FOLIOS_ASIGNADOS.Remove(fa);
                        kcc.SaveChanges();
                    }
                    PPKCC_FOLIOS_ASIGNADOS fat = new PPKCC_FOLIOS_ASIGNADOS();
                    fat.FA_LLAVE1 = rol.ToString();
                    fat.FA_LLAVE2 = data;
                    fat.FA_LLAVE3 = portal;
                    fat.FA_LLAVE4 = "";
                    fat.FA_FOLIO_TRANS = nOrden;
                    fat.FA_TELEFONO = telefono;
                    fat.FA_EMAIL = mail;
                    fat.FA_TOKEN = token;
                    fat.FA_CREACION = DateTime.Now;
                    kcc.PPKCC_FOLIOS_ASIGNADOS.Add(fat);
                    kcc.SaveChanges();
                    fa = fat;
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(fa));
                return fa;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return fa;
            }
        }

        public PPKCC_FOLIOS_ASIGNADOS ObtieneKccFoliosAsignadosByTokenTbk(string token)
        {
            PPKCC_FOLIOS_ASIGNADOS fa = null;
            try
            {
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {
                    fa = kcc.PPKCC_FOLIOS_ASIGNADOS.Where(x => x.FA_TOKEN == token).FirstOrDefault();
                }
                return fa;
            }
            catch (Exception ex)
            {
                return fa;
            }
        }

        public PPKCC_TRANSAC_ASEO RegistraKccAseo(decimal rolNum, decimal rolDv, decimal anio, decimal cuota,  decimal folioTbk, string estadoTbk,
         decimal valor, decimal ipc, decimal multa, decimal total, string fecVenc, string fecPago, string fono, string mail, decimal folioTesmu,
         string tipoPago, string numCuota, string tipoTrans, string digTarj, decimal idSesion, string codAutoriza, string estadoMail, LogReg log)
        {
            PPKCC_TRANSAC_ASEO result = null;
            try
            {
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {

                    log.registrar("RegistraKccAseo");
                    log.registrar(" Filtros: ");
                    log.registrar("     rolNum: " + rolNum.ToString());
                    log.registrar("     rolDv: " + rolDv.ToString());
                    log.registrar("     anio: " + anio.ToString());
                    log.registrar("     cuota: " + cuota.ToString());
                    log.registrar("     estadoTbk: " + estadoTbk.ToString());
                    log.registrar("     folioTbk: " + folioTbk.ToString());
                    result = kcc.PPKCC_TRANSAC_ASEO.Where(x =>
                                        x.ASE_ROL_NUM == rolNum &&
                                        x.ASE_ROL_DV == rolNum &&
                                        x.ASE_ANIO == anio &&
                                        x.ASE_CUOTA == cuota &&
                                        x.ASE_ESTADO_TRANS == estadoTbk &&
                                        x.ASE_FOLIO_TRANS == folioTbk
                                    ).FirstOrDefault();
                    if (result != null)
                    {
                        kcc.PPKCC_TRANSAC_ASEO.Remove(result);
                        kcc.SaveChanges();
                    }
                    PPKCC_TRANSAC_ASEO tranPaten = new PPKCC_TRANSAC_ASEO();
                    tranPaten.ASE_ROL_NUM = rolNum;
                    tranPaten.ASE_ROL_DV = rolDv;
                    tranPaten.ASE_ANIO = anio;
                    tranPaten.ASE_CUOTA = cuota;
                    tranPaten.ASE_VALOR = valor;
                    tranPaten.ASE_IPC = ipc;
                    tranPaten.ASE_MULTA = multa;
                    tranPaten.ASE_TOTAL = total;
                    tranPaten.ASE_FOLIO_TRANS = folioTbk;
                    tranPaten.ASE_ESTADO_TRANS = estadoTbk;
                    tranPaten.ASE_FEC_PAG = fecPago;
                    tranPaten.ASE_FEC_VEN = fecVenc;
                    tranPaten.ASE_EMAIL = mail;
                    tranPaten.ASE_TELEFONO = fono;
                    tranPaten.ASE_FOLIO_TESMU = folioTesmu;
                    tranPaten.ASE_TIPO_PAGO = tipoPago;
                    tranPaten.ASE_NUM_CUOTA = numCuota;
                    tranPaten.ASE_TIPO_TRANS = tipoTrans;
                    tranPaten.ASE_DIG_TARJETA = digTarj;
                    tranPaten.ASE_ID_SESION = idSesion;
                    tranPaten.ASE_COD_AUTORIZA = codAutoriza;
                    tranPaten.ASE_FEC_CREA = DateTime.Now;
                    tranPaten.ASE_ENVIA_MAIL = estadoMail;
                    kcc.PPKCC_TRANSAC_ASEO.Add(tranPaten);
                    kcc.SaveChanges();
                    result = tranPaten;
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }

        }


        public OrdenIngresoAseo RegistraAsOrdenIngreso(string idSesion, string rut, string dv, string nombre, string direccion, decimal rolNum, decimal rolDv,
          string fono, string mail, string fecPago, string cuidad, string folioTbk, string tipoTributo, string unidadGiradora, 
          string tipoPago, string tipoCuota, string nCuotas, string tipoTransaccion, string digTarjeta, string codAutoriza, 
          decimal? subtotal, decimal? ipc, decimal? multa, decimal? aPagar, LogReg log)
        {
            OrdenIngresoAseo result = null;
            try
            {
                using (aseoEntities pat = new aseoEntities())
                {

                    log.registrar("RegistraAsOrdenIngreso");
                    log.registrar(" Filtros: ");
                    log.registrar("     rolNum: " + rolNum.ToString());
                    log.registrar("     rolDv: " + rolDv.ToString());
                    log.registrar("     idSesion: " + idSesion.ToString());
                    log.registrar("     folioTbk: " + folioTbk.ToString());
                    result = pat.OrdenIngresoAseo.Where(x =>
                                        x.rolNum == rolNum &&
                                        x.rolDv == rolDv &&
                                        x.idSesion == idSesion &&
                                        x.nOrden == folioTbk
                                    ).FirstOrDefault();
                    if (result != null)
                    {
                        pat.OrdenIngresoAseo.Remove(result);
                        pat.SaveChanges();
                    }
                    OrdenIngresoAseo oi = new OrdenIngresoAseo();
                    oi.idSesion = idSesion;
                    oi.rut = rut;
                    oi.dv = dv;
                    oi.nombre = nombre;
                    oi.direccion = direccion;
                    oi.rolNum = rolNum;
                    oi.rolDv = rolDv;
                    oi.telefono = fono;
                    oi.eMail = mail;
                    oi.fecPago = fecPago;
                    oi.cuidad = cuidad;
                    oi.nOrden = folioTbk;
                    oi.tipoTributo = tipoTributo;
                    oi.unidadGiradora = unidadGiradora;
                    oi.tipoPago = tipoPago;
                    oi.tipoCuota = tipoCuota;
                    oi.nCuotas = nCuotas;
                    oi.tipoTransaccion = tipoTransaccion;
                    oi.digTarjeta = digTarjeta;
                    oi.subtotal = subtotal;
                    oi.ipc = ipc;
                    oi.multa = multa;
                    oi.aPagar = aPagar;

                    pat.OrdenIngresoAseo.Add(oi);
                    pat.SaveChanges();
                    result = oi;
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }

        }

        public DetalleOrdenIngresoAseo RegistraAsDetalleOrdenIngreso(string idSesion, decimal rolNum, decimal rolDv, string folioTbk, string denominacion,
            decimal anio, decimal cuota, string fecVenc, decimal? subtotal, decimal? ipc,
            decimal? multa, decimal? aPagar, string folioTesmu, LogReg log)
        {
            DetalleOrdenIngresoAseo result = null;
            try
            {
                using (aseoEntities pat = new aseoEntities())
                {

                    log.registrar("RegistraAsDetalleOrdenIngreso");
                    log.registrar(" Filtros: ");
                    log.registrar("     idSesion: " + idSesion.ToString());
                    log.registrar("     rolNum: " + rolNum.ToString());
                    log.registrar("     rolDv: " + rolDv.ToString());
                    log.registrar("     folioTbk: " + folioTbk.ToString());
                    log.registrar("     anio: " + anio.ToString());
                    log.registrar("     cuota: " + cuota.ToString());
                    result = pat.DetalleOrdenIngresoAseo.Where(x =>
                                        x.rolNum == rolNum &&
                                        x.rolDv == rolDv &&
                                        x.idSesion == idSesion &&
                                        x.nOrden == folioTbk &&
                                        x.anio == anio &&
                                        x.cuota == cuota
                                    ).FirstOrDefault();
                    if (result != null)
                    {
                        pat.DetalleOrdenIngresoAseo.Remove(result);
                        pat.SaveChanges();
                    }
                    DetalleOrdenIngresoAseo doi = new DetalleOrdenIngresoAseo();
                    doi.idSesion = idSesion;
                    doi.rolNum = rolNum;
                    doi.rolDv = rolDv;
                    doi.nOrden = folioTbk;
                    doi.denominacion = denominacion;
                    doi.anio = anio;
                    doi.cuota = cuota;
                    doi.fecVence = fecVenc;
                    doi.monto = subtotal;
                    doi.multa = multa;
                    doi.ipc = ipc;
                    doi.total = aPagar;
                    doi.folio = folioTesmu;
                    pat.DetalleOrdenIngresoAseo.Add(doi);
                    pat.SaveChanges();
                    result = doi;
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }

        }

        public bool RegistraAsPagoAseo(decimal rolNum, decimal rolDv, decimal anio, decimal cuota, decimal rut,
       decimal? valor, decimal? ipc, decimal? multa, string fecPago, string fecVenc, decimal folioTbk,
          decimal digTarjTbk, decimal cajero, LogReg log)
        {
            try
            {
                using (aseoEntities ase = new aseoEntities())
                {
                    log.registrar("RegistraPerPagoPermiso");
                    cajero = (cajero == 0) ? 201 : cajero;
                    log.registrar(" * PA_PAGO_ASEO_WS " + rolNum + "," + rolDv + "," + rut + "," + anio + "," + cuota + ",'" + fecPago + "','" + fecVenc + "'," + valor + "," + ipc + "," + multa + "," + folioTbk + "," + cajero.ToString() + "," + digTarjTbk);

                    ase.PA_PAGO_ASEO_WS(rolNum, rolDv, rut, anio, cuota, fecPago, fecVenc, valor, ipc, multa, folioTbk, cajero, digTarjTbk);
                    return true;
                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return false;
            }
        }

       
        //OBTIENE

        public bool ObtieneAsSiAseoPagado(decimal rolNum, decimal rolDv, decimal anio, decimal cuota, LogReg log)
        {
            AS_PAGOS result = null;
            try
            {
                log.registrar("ObtieneAsSiAseoPagado");
                log.registrar(" Filtros: ");
                log.registrar("     rolNum: " + rolNum.ToString());
                log.registrar("     rolDv: " + rolDv.ToString());
                log.registrar("     anio: " + anio.ToString());
                log.registrar("     cuota: " + cuota.ToString());
                using (aseoEntities ase = new aseoEntities())
                {
                    result = ase.AS_PAGOS.Where(x => x.PAG_ROL_NUM == rolNum && x.PAG_ROL_DV == rolDv && x.PAG_ANO_CON == anio && x.PAG_CUOTA == cuota).FirstOrDefault();
                    log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                    if (result == null)
                    {
                        log.registrar("dato null, NO procede pago");
                        return false;
                    }
                    else
                    {
                        DateTime limit = new DateTime(1985, 1, 1);
                        if (result.PAG_FEC_P == null || result.PAG_FEC_P < limit)
                        {
                            log.registrar("fecha null o pago bajo el limite,  procede pago");
                            return true;
                        }
                        else
                        {
                            log.registrar("registro pagado, NO procede pago");
                            return false;
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
                //no se paga si hay un error
                return true;
            }
        }
        public string ObtieneFolioTbk(LogReg log)
        {
            string result = "0";
            try
            {
                log.registrar("ObtieneFolioTbk");
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {
                    string anio = Utils.Right(DateTime.Now.Year.ToString(), 2);
                    PPKCC_FOLIOS consulta = kcc.PPKCC_FOLIOS.Where(x =>
                                            x.FOL_CAJA == Utils.caja &&
                                            x.FOL_SISTEMA == Utils.sistema &&
                                            x.FOL_ANIO == anio
                                        ).FirstOrDefault();
                    if (consulta == null)
                    {
                        PPKCC_FOLIOS folio = new PPKCC_FOLIOS();
                        folio.FOL_CAJA = Utils.caja;
                        folio.FOL_SISTEMA = Utils.sistema;
                        folio.FOL_ANIO = anio;
                        folio.FOL_FOLIO = "000001";
                        kcc.PPKCC_FOLIOS.Add(folio);
                        kcc.SaveChanges();
                        result = Utils.caja.ToString() + Utils.sistema.ToString() + anio + "000001";
                    }
                    else
                    {
                        decimal folio_fol = decimal.Parse(consulta.FOL_FOLIO);
                        folio_fol = folio_fol + 1;
                        consulta.FOL_CAJA = Utils.caja;
                        consulta.FOL_SISTEMA = Utils.sistema;
                        consulta.FOL_ANIO = anio;
                        consulta.FOL_FOLIO = folio_fol.ToString("000000");
                        kcc.SaveChanges();
                        result = Utils.caja.ToString() + Utils.sistema.ToString() + anio + folio_fol.ToString("000000");
                    }
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return "0";
            }
        }

        public AS_PAGOS ObtieneAsPagos(decimal rolNum, decimal rolDv, decimal anio, decimal cuota, LogReg log)
        {
            AS_PAGOS result = null;
            try
            {
                log.registrar("ObtieneAsPagos");
                log.registrar(" Filtros: ");
                log.registrar("     rolNum: " + rolNum.ToString());
                log.registrar("     rolDv: " + rolDv.ToString());
                log.registrar("     anio: " + anio.ToString());
                log.registrar("     cuota: " + cuota.ToString());
                using (aseoEntities ase = new aseoEntities())
                {
                    result = ase.AS_PAGOS.Where(x => x.PAG_ROL_NUM == rolNum && x.PAG_ROL_DV == rolDv && x.PAG_ANO_CON == anio && x.PAG_CUOTA == cuota).FirstOrDefault();
                    log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                    return result;
                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }
        }

        public OrdenIngresoAseo ObtieneAsOrdenIngreso(decimal rolNum, decimal rolDv, string idSesion, string folioTbk, LogReg log)
        {
            OrdenIngresoAseo result = null;
            try 
            {
                log.registrar("ObtieneAsPagos");
                log.registrar(" Filtros: ");
                log.registrar("     rolNum: " + rolNum.ToString());
                log.registrar("     rolDv: " + rolDv.ToString());
                log.registrar("     idSesion: " + idSesion.ToString());
                log.registrar("     folioTbk: " + folioTbk.ToString());
                using (aseoEntities ase = new aseoEntities())
                {
                    result = ase.OrdenIngresoAseo.Where(x => x.rolNum == rolNum && x.rolDv == rolDv && x.idSesion == idSesion && x.nOrden == folioTbk).FirstOrDefault();
                    log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                    return result;
                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }
        }

        public List<DetalleOrdenIngresoAseo> ListaAsDetalleOrdenIngreso(decimal rolNum, decimal rolDv, string idSesion, string folioTbk, LogReg log)
        {
            List<DetalleOrdenIngresoAseo> result = null;
            try
            {
                log.registrar("ListaAsDetalleOrdenIngreso");
                log.registrar(" Filtros: ");
                log.registrar("     rolNum: " + rolNum.ToString());
                log.registrar("     rolDv: " + rolDv.ToString());
                log.registrar("     idSesion: " + idSesion.ToString());
                log.registrar("     folioTbk: " + folioTbk.ToString());
                using (aseoEntities ase = new aseoEntities())
                {
                    result = ase.DetalleOrdenIngresoAseo.Where(x => x.rolNum == rolNum && x.rolDv == rolDv && x.idSesion == idSesion && x.nOrden == folioTbk).ToList();
                    log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                    return result;
                }
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }
        }

        public Dictionary<string,string> ObtieneContribuyente(decimal rolNum, decimal rolDv, LogReg log)
        {
            Dictionary<string,string> result = null;
            try
            {
                log.registrar("ObtieneContribuyente");
                log.registrar(" Filtros: ");
                log.registrar("     rolNum: " + rolNum.ToString());
                log.registrar("     rolDv: " + rolDv.ToString());
                using (aseoEntities ase = new aseoEntities())
                {
                    var contr = ase.AS_CONTRIBUYENTE.Where(x => x.CON_ROL_NUM.Equals(rolNum) && x.CON_ROL_DV.Equals(rolDv)).OrderByDescending(x => x.CON_ANO_CON).FirstOrDefault();
                    var rutContr = ase.AS_RUT.Where(x => x.RUT_RUT.Equals(contr.CON_RUT)).FirstOrDefault();
                    result = new Dictionary<string, string>();
                    result.Add("rut",contr.CON_RUT.ToString());
                    result.Add("dv", contr.CON_DV);
                    result.Add("nombre",rutContr.RUT_NOMBRE);
                    result.Add("direccion",contr.CON_CALLE + ' ' + contr.CON_NUMERO);
                    log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                    return result;
                }

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }
           
        }

        public List<PPKCC_TRANSAC_ASEO> ListaKccAseoByFolioTbk(decimal folioTbk, LogReg log)
        {
            List<PPKCC_TRANSAC_ASEO> result = null;
            try
            {
                using (PagosPortaleKCCEntities per = new PagosPortaleKCCEntities())
                {
                    log.registrar("ListaKccAseoByFolioTbk");
                    log.registrar(" Filtros: ");
                    log.registrar("     folioTbk: " + folioTbk.ToString());
                    result = per.PPKCC_TRANSAC_ASEO.Where(x => x.ASE_FOLIO_TRANS == folioTbk).ToList();
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                return result;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }
        }

        public decimal ObtieneFolioTbkPago(decimal folioTbk, decimal monto, LogReg log)
        {
            try
            {
                log.registrar("ObtieneFolioTbkPago");
                log.registrar(" Filtros: ");
                log.registrar("     folioTbk: " + folioTbk.ToString());
                log.registrar("     monto: " + monto.ToString());
                using (tesoreria2008Entities tes = new tesoreria2008Entities())
                {
                    var result = tes.ATETIPOPAGO.Where(x =>
                                       x.numcuenta == folioTbk &&
                                       x.monto == monto
                        ).FirstOrDefault();
                    log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                    return (result == null) ? 0 : result.folio;
                }

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return 0;
            }
        }

        public WEBPAY ObtieneKccWebpay(string folioTbk, LogReg log)
        {
            WEBPAY w = null;
            try
            {
                log.registrar("ObtieneKccWebpay");
                log.registrar(" Filtros: ");
                log.registrar("     folioTbk: " + folioTbk.ToString());
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {
                    w = kcc.WEBPAY.Where(x =>
                                       x.TBK_ORDEN_COMPRA == folioTbk
                        ).FirstOrDefault();
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(w));
                return w;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return w;
            }
        }
    

        //BORRAR
        public bool BorraAsOrdenIngreso(decimal rolNum, decimal rolDv, string idSesion, string folioTbk, LogReg log)
        {
            OrdenIngresoAseo result = null;
            try
            {
                using (aseoEntities pat = new aseoEntities())
                {

                    log.registrar("BorraAsOrdenIngreso");
                    log.registrar(" Filtros: ");
                    log.registrar("     idSesion: " + idSesion.ToString());
                    log.registrar("     rolNum: " + rolNum.ToString());
                    log.registrar("     rolDv: " + rolDv.ToString());
                    log.registrar("     folioTbk: " + folioTbk.ToString());
                    result = pat.OrdenIngresoAseo.Where(x =>
                                         x.rolNum == rolNum &&
                                        x.rolDv == rolDv &&
                                        x.nOrden == folioTbk
                                    ).FirstOrDefault();
                    if (result != null)
                    {
                        pat.OrdenIngresoAseo.Remove(result);
                        pat.SaveChanges();
                    }
                }
                log.registrar(" * Data: " + new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(result));
                return true;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return false;
            }

        }

        public bool BorraAsDetalleOrdenIngreso(decimal rolNum, decimal rolDv, string idSesion, string folioTbk, LogReg log)
        {
            List<DetalleOrdenIngresoAseo> result = null;
            try
            {
                using (aseoEntities per = new aseoEntities())
                {
                    log.registrar("BorraAsDetalleOrdenIngreso");
                    log.registrar(" Filtros: ");
                    log.registrar("     idSesion: " + idSesion.ToString());
                    log.registrar("     rolNum: " + rolNum.ToString());
                    log.registrar("     rolDv: " + rolDv.ToString());
                    log.registrar("     folioTbk: " + folioTbk.ToString());

                    result = per.DetalleOrdenIngresoAseo.Where(x =>
                                        x.rolNum == rolNum &&
                                        x.rolDv == rolDv &&
                                        x.idSesion == idSesion &&
                                        x.nOrden == folioTbk
                                ).ToList();
                    if (result.Count > 0)
                    {
                        foreach (DetalleOrdenIngresoAseo reg in result)
                        {
                            per.DetalleOrdenIngresoAseo.Remove(reg);
                            per.SaveChanges();
                        }

                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return false;
            }

        }

        public bool BorraKccAseo(decimal rolNum, decimal rolDv, decimal anio, decimal cuota, decimal folioTbk, string estadoTbk, LogReg log)
        {
            List<PPKCC_TRANSAC_ASEO> result = null;
            try
            {
                using (PagosPortaleKCCEntities kcc = new PagosPortaleKCCEntities())
                {
                    log.registrar("BorraKccAseo");
                    log.registrar(" Filtros: ");
                    log.registrar("     rolNum: " + rolNum.ToString());
                    log.registrar("     rolDv: " + rolDv.ToString());
                    log.registrar("     anio: " + anio.ToString());
                    log.registrar("     cuota: " + cuota.ToString());
                    log.registrar("     folioTbk: " + folioTbk.ToString());
                    log.registrar("     estadoTbk: " + estadoTbk.ToString());

                    result = kcc.PPKCC_TRANSAC_ASEO.Where(x =>
                                        x.ASE_ROL_NUM == rolNum &&
                                        x.ASE_ROL_DV == rolDv &&
                                        x.ASE_ANIO == anio &&
                                        x.ASE_CUOTA == cuota &&
                                        x.ASE_ESTADO_TRANS == estadoTbk &&
                                        x.ASE_FOLIO_TRANS == folioTbk
                                ).ToList();
                    if (result.Count > 0)
                    {
                        foreach (PPKCC_TRANSAC_ASEO reg in result)
                        {
                            kcc.PPKCC_TRANSAC_ASEO.Remove(reg);
                            kcc.SaveChanges();
                        }

                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return false;
            }

        }



        #endregion

        #region SERVICIO WEB TBK
        // CONFIGURACION
        Configuration configuration = new Configuration();
        private WebpayNormal ConfiguraTBK(LogReg log)
        {
            WebpayNormal transaction = null;
            try
            {
                /** Crea Dictionary con datos Integración Pruebas */
                Dictionary<string, string> certificate = CertNormal.certificate();

                log.registrar("preparacion de datos para TBK");


                /** Creacion Objeto Webpay */
                log.registrar("creacion de objeto transbank");

                if (certificate["environment"] == "PRODUCCION")
                {
                    configuration.Environment = certificate["environment"];
                    configuration.CommerceCode = certificate["commerce_code"];
                    configuration.PrivateCertPfxPath = certificate["webpay_cert"];
                    configuration.WebpayCertPath = certificate["public_cert"];
                    configuration.Password = certificate["password"];
                    log.registrar(" * environment:" + certificate["environment"]);
                    log.registrar(" * commerce_code:" + certificate["commerce_code"]);
                    log.registrar(" * public_cert:" + certificate["public_cert"]);
                    log.registrar(" * webpay_cert:" + certificate["webpay_cert"]);
                    log.registrar(" * password: ver archivo de configuracion");
                    log.registrar("configurar webpay");

                }
                else
                {
                    configuration = Configuration.ForTestingWebpayPlusNormal();

                }
                transaction = new Webpay(configuration).NormalTransaction;
                log.registrar("envio datos a TBK");

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
            }

            return transaction;
        }

        private WebpayNullify ConfiguraAnulaTBK(LogReg log)
        {
            WebpayNullify transaction = null;
            try
            {
                /** Crea Dictionary con datos Integración Pruebas */
                Dictionary<string, string> certificate = CertNormal.certificate();

                log.registrar("preparacion de datos para TBK");
                if (certificate["environment"] == "PRODUCCION")
                {
                    configuration.Environment = certificate["environment"];
                    configuration.CommerceCode = certificate["commerce_code"];
                    configuration.PrivateCertPfxPath = certificate["webpay_cert"];
                    configuration.WebpayCertPath = certificate["public_cert"];
                    configuration.Password = certificate["password"];
                    log.registrar(" * environment:" + certificate["environment"]);
                    log.registrar(" * commerce_code:" + certificate["commerce_code"]);
                    log.registrar(" * public_cert:" + certificate["public_cert"]);
                    log.registrar(" * webpay_cert:" + certificate["webpay_cert"]);
                    log.registrar(" * password: ver archivo de configuracion");
                    log.registrar("configurar webpay");
                    /** Creacion Objeto Webpay */
                    log.registrar("creacion de objeto transbank");

                }
                else
                {
                    configuration = Configuration.ForTestingWebpayPlusNormal();
                }
                transaction = new Webpay(configuration).NullifyTransaction;
                log.registrar("envio datos a TBK");

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
            }



            return transaction;
        }

        //PROCESOS
        public wsInitTransactionOutput InformaVentaTBK(decimal amount, string buyOrder, string sessionId, LogReg log)
        {
            try
            {
                WebpayNormal transaction = ConfiguraTBK(log);
                /** Información de Host para crear URL */
                log.registrar("Información de Host para crear URL");
                String httpHost = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"].ToString();

                /** Crea URL de Aplicación */
                log.registrar("Crea URL de Aplicación");
                string sample_baseurl = "";
                if (Utils.ambiente != "INTEGRACION")
                {
                    sample_baseurl = "https://" + httpHost;
                }
                else
                {
                    sample_baseurl = "http://" + httpHost;
                }

                string urlReturn = sample_baseurl + Utils.resultTBK;
                string urlFinal = sample_baseurl + Utils.endTBK;

                log.registrar(" * " + sample_baseurl);
                log.registrar("Data a enviar a TBK");
                log.registrar("**********************");
                /** Monto de la transacción */
                log.registrar(" * amount:" + amount.ToString());

                /** Orden de compra de la tienda */
                log.registrar(" * buyOrder:" + buyOrder);

                /** (Opcional) Identificador de sesión, uso interno de comercio */
                log.registrar(" * sessionId:" + sessionId);

                log.registrar("envio datos a TBK");

                log.registrar("ejecucion initTransaction( wsInitTransactionOutput result = webpay.getNormalTransaction().initTransaction( " + amount + "," + buyOrder + "," + sessionId + "," + urlReturn + "," + urlFinal + ")");
                return transaction.initTransaction(amount, buyOrder, sessionId, urlReturn, urlFinal);
            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
            }

            return null;
        }

        public transactionResultOutput ConfirmaVentaTBK(string token, LogReg log)
        {
            try
            {
                WebpayNormal transaction = ConfiguraTBK(log);

                log.registrar("Se envia token a transbank para cobro");
                //result = webpay.getNormalTransaction().getTransactionResult(token.ToString());
                log.registrar("transactionResultOutput result = webpay.getNormalTransaction().getTransactionResult(" + token + ")");
                return transaction.getTransactionResult(token);

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }

        }

        public nullificationOutput AnulaVentaTBK(string authorizationCode, decimal amount, string buyOrder, string sessionId, LogReg log)
        {
            try
            {
                string commercecode = configuration.CommerceCode;
                WebpayNullify transaction = ConfiguraAnulaTBK(log);

                log.registrar("Se envia anulacion de la venta");
                return transaction.nullify(authorizationCode, amount, buyOrder, amount, commercecode);

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                return null;
            }
        }

        #endregion  

        #region MESAJES DE ERROR Y RECHAZO

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

        protected void ManejoErrores(string ns, string controller, string action, Exception ex, LogReg log)
        {
            log.registrar("***************************************");
            string tipo = ex.GetType().Name;
            log.registrar("ERROR TYPE: " + tipo);
            log.registrar("ERROR MESSAGE D: " + ex.Message);

            if (tipo == "EntityException")
            {
                EntityException tmp = (EntityException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "EntitySqlException")
            {
                EntitySqlException tmp = (EntitySqlException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "EntityCommandCompilationException")
            {
                EntityCommandCompilationException tmp = (EntityCommandCompilationException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "EntityCommandExecutionException")
            {
                EntityCommandExecutionException tmp = (EntityCommandExecutionException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "NoReportPartsException")
            {
                NoReportPartsException tmp = (NoReportPartsException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "LoadSaveReportException")
            {
                LoadSaveReportException tmp = (LoadSaveReportException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "ReportClosedException")
            {
                ReportClosedException tmp = (ReportClosedException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "EngineException")
            {
                EngineException tmp = (EngineException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "DbUpdateException")
            {
                DbUpdateException tmp = (DbUpdateException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }

            }
            else if (tipo == "DbUpdateConcurrencyException")
            {
                DbUpdateConcurrencyException tmp = (DbUpdateConcurrencyException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }
            else if (tipo == "DBConcurrencyException")
            {
                DBConcurrencyException tmp = (DBConcurrencyException)ex;
                log.registrar("ERROR MESSAGE 1: " + tmp.Message);
                if (tmp.InnerException != null)
                {
                    log.registrar("ERROR MESSAGE 2: " + tmp.InnerException.Message);
                    if (tmp.InnerException.InnerException != null)
                    {
                        log.registrar("ERROR MESSAGE 3: " + tmp.InnerException.InnerException.Message);
                        if (tmp.InnerException.InnerException.InnerException != null)
                        {
                            log.registrar("ERROR MESSAGE 4: " + tmp.InnerException.InnerException.InnerException.Message);
                        }
                    }
                }
            }

            log.registrar("ERROR NAMESPACE: " + ns);
            log.registrar("ERROR CONTROLLER: " + controller);
            log.registrar("ERROR ACTION: " + action);
            log.registrar("ERROR SOURCE: " + ex.Source);
            log.registrar("***************************************");
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            serializer.MaxJsonLength = 500000000;
            log.registrar(" * ERR SOU: " + serializer.Serialize(ex));
            log.registrar("***************************************");
        }

        #endregion

        #region VISTAS

        public ActionResult MuestraReporte(string rol, string dv, string folio)
        {
            LogReg log = new LogReg();
            decimal numRol = 0;
            decimal numDv = 0;
            decimal numFolio = 0;
            string ordenEncrip = "";
            long valor = 0;
            long ipc = 0;
            long multa = 0;
            long total = 0;
            string tipoPago2 = "";
            string tipoTransaccion = "";
            string nCuotas = "";
            string digTarjeta = "";
            string codAutoriza = "";
            string fono = "n/a";
            string fechPago = "n/a";
            string fecVenci = "n/a";
            string mail = "n/a";
            decimal mesPago;
            decimal añoPago;
            string fileName = "";
            string contentType = "";
            log.abrirArchivo(rol.ToString() + "-" + dv);
            log.registrar("*********************************************");
            log.registrar(" * PROCESO: MuestraReporte");
            log.registrar("*********************************************");
            log.registrar("Muestra reporte Orden ingreso: " + folio.ToString());
            bool valRol = decimal.TryParse(rol, out numRol);
            bool valDv = decimal.TryParse(dv, out numDv);
            bool valFolio = decimal.TryParse(folio, out numFolio);
            if (valRol == false || valDv == false || valFolio == false)
            {
                log.registrar("parametros de entrada no validos");
                return Json("parametros de entrada no validos", JsonRequestBehavior.AllowGet);
            }

            ReportDocument reportDocument = new ReportDocument();
            DataTable dtDetallePago = new DataTable("detallePagoT");
            DataTable dtCodigoRr = new DataTable("codigoQrT");
            DataSet dataSet = new DataSet("dsPermiso");
            List<PPKCC_TRANSAC_ASEO> detReporte = new List<PPKCC_TRANSAC_ASEO>();
            PPKCC_TRANSAC_ASEO encReporte = new PPKCC_TRANSAC_ASEO();
            AS_CONTRIBUYENTE contr = new AS_CONTRIBUYENTE();
            AS_RUT rutContr = new AS_RUT();
            string datos = "";
            try
            {
                using (PagosPortaleKCCEntities entidadesKCC = new PagosPortaleKCCEntities())
                {
                    detReporte = (from p in entidadesKCC.PPKCC_TRANSAC_ASEO
                                  where
                                     p.ASE_ROL_NUM == numRol &&
                                     p.ASE_ROL_DV == numDv &&
                                     p.ASE_FOLIO_TRANS == numFolio &&
                                     p.ASE_ESTADO_TRANS.Equals("A")
                                  select p
                            ).ToList();

                    encReporte = (from p in entidadesKCC.PPKCC_TRANSAC_ASEO
                                  where
                                     p.ASE_ROL_NUM == numRol &&
                                     p.ASE_ROL_DV == numDv &&
                                     p.ASE_FOLIO_TRANS == numFolio &&
                                     p.ASE_ESTADO_TRANS.Equals("A")
                                  select p
                            ).FirstOrDefault();
                }


                using (accesosEntities entidadesAccesos = new accesosEntities())
                {
                    ordenEncrip = entidadesAccesos.PA_ENCIP_DESENCRIP_DATOS_MVC(folio, "E").FirstOrDefault();
                }

                using (aseoEntities entidadesAseo = new aseoEntities())
                {
                    contr = entidadesAseo.AS_CONTRIBUYENTE.Where(x => x.CON_ROL_NUM.Equals(numRol) && x.CON_ROL_DV.Equals(numDv)).OrderByDescending(x => x.CON_ANO_CON).FirstOrDefault();
                    rutContr = entidadesAseo.AS_RUT.Where(x => x.RUT_RUT.Equals(contr.CON_RUT)).FirstOrDefault();
                }


                Utils.crearDirectorio(Utils.RutaCodQr);
                Utils.crearDirectorio(Utils.RutaReporte);
                Utils.crearDirectorio(Utils.RutaPDF);
                string rutaCodiQr = Utils.RutaCodQr + folio.ToString() + "_" + rol + "_" + dv + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".png";
                string rutaReporte = Utils.RutaReporte + "CRCompPago.rpt";

                dtCodigoRr.Columns.Add(new DataColumn("codigoQrArrayBytes", typeof(System.Byte[]), null, MappingType.Attribute));
                dtDetallePago.Columns.Add(new DataColumn("denominacion", typeof(System.String), null, MappingType.Attribute));
                dtDetallePago.Columns.Add(new DataColumn("folio", typeof(System.String), null, MappingType.Attribute));
                dtDetallePago.Columns.Add(new DataColumn("folioEncripado", typeof(System.String), null, MappingType.Attribute));
                dtDetallePago.Columns.Add(new DataColumn("valor", typeof(System.String), null, MappingType.Attribute));
                dataSet.Tables.Add(dtDetallePago);
                dataSet.Tables.Add(dtCodigoRr);
                //dataSet.WriteXmlSchema(rutaReporte = Utils.RutaReporte + "xmlMultas.xml");

                if (encReporte == null)
                {
                    log.registrar("No existe registro relacionada con la búsqueda de la orden de ingreso.");
                    return Json("No existe registro relacionada con la búsqueda de la orden de ingreso.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    tipoPago2 = encReporte.ASE_TIPO_PAGO.ToString();
                    tipoTransaccion = encReporte.ASE_TIPO_TRANS;
                    nCuotas = encReporte.ASE_NUM_CUOTA;
                    digTarjeta = encReporte.ASE_DIG_TARJETA;
                    codAutoriza = encReporte.ASE_COD_AUTORIZA.ToString();
                    fono = encReporte.ASE_TELEFONO.ToString();
                    fechPago = Utils.formatFechaVista(encReporte.ASE_FEC_PAG);
                    mesPago = decimal.Parse(Utils.Mid(encReporte.ASE_FEC_PAG, 4, 2));
                    añoPago = decimal.Parse(Utils.Left(encReporte.ASE_FEC_PAG, 4));
                    fecVenci = Utils.formatFechaVista(encReporte.ASE_FEC_VEN);
                    mail = encReporte.ASE_EMAIL;
                    decimal folioTGR = decimal.Parse(numFolio.ToString());
                    if (Utils.ambiente != "INTEGRACION")
                    {
                        //informa el pago a la subdere por medio el 13
                        //Service1SoapClient subdere = new Service1SoapClient();
                        //subdere.InformaPagoSubdere(folioTGR);
                    }
                    /*Fin Swict Integracion*/

                }

                reportDocument.Load(rutaReporte);
                foreach (PPKCC_TRANSAC_ASEO pago in detReporte)
                {
                    DataRow row = dtDetallePago.NewRow();
                    row["denominacion"] = "Pago D. Aseo cuota " + pago.ASE_CUOTA.ToString() + " año " + pago.ASE_ANIO.ToString();
                    row["folio"] = pago.ASE_FOLIO_TESMU.ToString();
                    using (accesosEntities entidadesAccesos = new accesosEntities())
                    {
                        row["folioEncripado"] = entidadesAccesos.PA_ENCIP_DESENCRIP_DATOS_MVC(pago.ASE_FOLIO_TESMU.ToString(), "E").FirstOrDefault();
                    }
                    row["valor"] = "$" + pago.ASE_VALOR.ToString("0,0", CultureInfo.InvariantCulture);
                    dtDetallePago.Rows.Add(row);

                    valor = valor + long.Parse(pago.ASE_VALOR.ToString());
                    ipc = ipc + long.Parse(pago.ASE_IPC.ToString());
                    multa = multa + long.Parse(pago.ASE_MULTA.ToString());
                    total = total + long.Parse(pago.ASE_TOTAL.ToString());
                }

                datos = Utils.SitioCodQr + "url=" + Utils.Encriptar(folio.ToString()) + ":" + Utils.Encriptar(rol.ToString() + "-" + dv.ToString()) + ":" + Utils.Encriptar(total.ToString());

                CodigoQr.crearCodigoQr(rutaCodiQr, datos);
                DataRow rowQr = dtCodigoRr.NewRow();
                rowQr["codigoQrArrayBytes"] = ImageHelper.ImageToByteArray(ImageHelper.ObtenerCodigoQr(rutaCodiQr));
                dtCodigoRr.Rows.Add(rowQr);

                string tipoPago = "";
                string tipoCuota = "";
                string numCuota = "";

                if (tipoPago2.ToString().Trim().Equals("VD"))
                {
                    tipoPago = "REDCOMPRA";
                }
                else
                {
                    tipoPago = "CRÉDITO";
                }

                if (nCuotas.ToString().Trim().Equals("0") && !tipoPago2.ToString().Trim().Equals("VD"))
                {
                    tipoCuota = "SIN CUOTAS";
                }
                else if (nCuotas.ToString().Trim().Equals("3") && !tipoPago2.ToString().Trim().Equals("VD"))
                {
                    tipoCuota = "SIN INTERÉS";
                }
                else if (tipoPago2.ToString().Trim().Equals("CI"))
                {
                    tipoCuota = "CUOTAS COMERCIO";
                }
                else if (tipoPago2.ToString().Trim().Equals("VD"))
                {
                    tipoCuota = "DÉBITO";
                }
                else
                {
                    tipoCuota = "CUOTAS NORMALES";
                }

                if (nCuotas.Trim().Equals("0"))
                {
                    numCuota = "00";
                }
                else
                {
                    numCuota = nCuotas.ToString();
                }

                reportDocument.SetDataSource(dataSet);
                reportDocument.SetParameterValue("txtSubTotal", "$" + valor.ToString("0,0", CultureInfo.InvariantCulture));
                reportDocument.SetParameterValue("txtIpc", "$" + ipc.ToString("0,0", CultureInfo.InvariantCulture));
                reportDocument.SetParameterValue("txtMulta", "$" + multa.ToString("0,0", CultureInfo.InvariantCulture));
                reportDocument.SetParameterValue("txtTotal", "$" + total.ToString("0,0", CultureInfo.InvariantCulture));
                reportDocument.SetParameterValue("txtNombre", rutContr.RUT_NOMBRE);
                reportDocument.SetParameterValue("txtFecPago", fechPago);
                reportDocument.SetParameterValue("txtNOrden", folio);
                reportDocument.SetParameterValue("txtDomicilio", contr.CON_CALLE + " " + contr.CON_NUMERO);
                reportDocument.SetParameterValue("txtCuidad", Utils.cuidad.ToString());
                reportDocument.SetParameterValue("txtRut", rutContr.RUT_RUT + "-" + rutContr.RUT_DV);
                reportDocument.SetParameterValue("txtTipTributo", Utils.tipoTributo);
                reportDocument.SetParameterValue("txtTelefono", fono.ToString());
                reportDocument.SetParameterValue("txtRol", rol + "-" + dv);
                reportDocument.SetParameterValue("txtEMail", mail);
                reportDocument.SetParameterValue("txtUniGira", Utils.usuarioWS);
                reportDocument.SetParameterValue("txtCodigo", ordenEncrip);
                reportDocument.SetParameterValue("txtTipoPago", tipoPago);
                reportDocument.SetParameterValue("txtTipoCuota", tipoCuota);
                reportDocument.SetParameterValue("txtNumCuota", numCuota);
                reportDocument.SetParameterValue("txtTipoTrans", tipoTransaccion);
                reportDocument.SetParameterValue("txtDigTarj", digTarjeta);
                reportDocument.SetParameterValue("txtCodAutoriza", codAutoriza);


                fileName = Utils.RutaPDF + folio + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".pdf";
                Pdf exportPdf = new Pdf();
                exportPdf.crearPdf(fileName, reportDocument);
                
                contentType = "application/pdf";
                return new FilePathResult(fileName, contentType);

            }
            catch (Exception ex)
            {
                string ns = this.ControllerContext.Controller.GetType().Namespace;
                string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
                string action = this.ControllerContext.RouteData.Values["action"].ToString();
                ManejoErrores(ns, controller, action, ex, log);
                Session["err-enc"] = "Error en la recepción de la transacción a transbank";
                Session["err-dat"] = "Rol:" + rol + "; numOrden: " + numFolio.ToString();
                Session["err-msg"] = "Se produjo un error inesperado. Comuniquese con " + Utils.adminContacto + " al E-Mail " + Utils.adminEMail;
                Session["err-rec"] = "Generar nuevamente la solicitud utilizando otro navegador o limpie las cookies";
                Session["err-fal"] = ex.Message;
                return RedirectToAction("Mensaje", "Pago");
            }
            finally
            {
                log.registrar("**************************************");
                log.cerrar();
            }
        }

       
        #endregion
    }
}
