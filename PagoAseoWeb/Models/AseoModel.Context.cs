﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SocialAseoWeb.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class aseoEntities : DbContext
    {
        public aseoEntities()
            : base("name=aseoEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AS_CONTRIBUYENTE> AS_CONTRIBUYENTE { get; set; }
        public DbSet<AS_PAGOS> AS_PAGOS { get; set; }
        public DbSet<AS_RUT> AS_RUT { get; set; }
        public DbSet<AS_CONVENIO> AS_CONVENIO { get; set; }
        public DbSet<DetalleOrdenIngresoAseo> DetalleOrdenIngresoAseo { get; set; }
        public DbSet<OrdenIngresoAseo> OrdenIngresoAseo { get; set; }
        public DbSet<TABLE_PARTICULAR_ARCHIVOS_DETALLE> TABLE_PARTICULAR_ARCHIVOS_DETALLE { get; set; }
        public DbSet<TABLE_PARTICULAR_ARCHIVOS_ENCABEZADO> TABLE_PARTICULAR_ARCHIVOS_ENCABEZADO { get; set; }
        public DbSet<AS_EXCLUSION> AS_EXCLUSION { get; set; }
        public DbSet<Social_OrdenIngresoAseo> Social_OrdenIngresoAseo { get; set; }
        public DbSet<Social_DetalleOrdenIngresoAseo> Social_DetalleOrdenIngresoAseo { get; set; }
        public DbSet<AS_SOLICITUD> AS_SOLICITUD { get; set; }
        public DbSet<Social_DetalleArchivo> Social_DetalleArchivo { get; set; }
    
        public virtual ObjectResult<Nullable<decimal>> PA_TRASPASA_CONTRIBUYENTE(Nullable<decimal> rOL_PRO, Nullable<decimal> rOL_DV, Nullable<decimal> aÑO_CUOTA)
        {
            var rOL_PROParameter = rOL_PRO.HasValue ?
                new ObjectParameter("ROL_PRO", rOL_PRO) :
                new ObjectParameter("ROL_PRO", typeof(decimal));
    
            var rOL_DVParameter = rOL_DV.HasValue ?
                new ObjectParameter("ROL_DV", rOL_DV) :
                new ObjectParameter("ROL_DV", typeof(decimal));
    
            var aÑO_CUOTAParameter = aÑO_CUOTA.HasValue ?
                new ObjectParameter("AÑO_CUOTA", aÑO_CUOTA) :
                new ObjectParameter("AÑO_CUOTA", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("PA_TRASPASA_CONTRIBUYENTE", rOL_PROParameter, rOL_DVParameter, aÑO_CUOTAParameter);
        }
    
        public virtual ObjectResult<string> PA_VER_EXENTO(Nullable<int> rOL_PRO, Nullable<int> rOL_DV, Nullable<int> aÑO_CUOTA, Nullable<int> nUM_CUOTA)
        {
            var rOL_PROParameter = rOL_PRO.HasValue ?
                new ObjectParameter("ROL_PRO", rOL_PRO) :
                new ObjectParameter("ROL_PRO", typeof(int));
    
            var rOL_DVParameter = rOL_DV.HasValue ?
                new ObjectParameter("ROL_DV", rOL_DV) :
                new ObjectParameter("ROL_DV", typeof(int));
    
            var aÑO_CUOTAParameter = aÑO_CUOTA.HasValue ?
                new ObjectParameter("AÑO_CUOTA", aÑO_CUOTA) :
                new ObjectParameter("AÑO_CUOTA", typeof(int));
    
            var nUM_CUOTAParameter = nUM_CUOTA.HasValue ?
                new ObjectParameter("NUM_CUOTA", nUM_CUOTA) :
                new ObjectParameter("NUM_CUOTA", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("PA_VER_EXENTO", rOL_PROParameter, rOL_DVParameter, aÑO_CUOTAParameter, nUM_CUOTAParameter);
        }
    
        public virtual int PA_PAGO_ASEO_WS(Nullable<decimal> rOL_PRO, Nullable<decimal> rOL_DV, Nullable<decimal> rUT, Nullable<decimal> aNIO_CUOTA, Nullable<decimal> nUM_CUOTA, string fEC_PAGO, string fEC_VENC, Nullable<decimal> vALOR_CUOTA, Nullable<decimal> vALOR_IPC, Nullable<decimal> vALOR_INTE, Nullable<decimal> cOD_TRANS, Nullable<decimal> cOD_CA_CA, Nullable<decimal> cOD_TARJETA)
        {
            var rOL_PROParameter = rOL_PRO.HasValue ?
                new ObjectParameter("ROL_PRO", rOL_PRO) :
                new ObjectParameter("ROL_PRO", typeof(decimal));
    
            var rOL_DVParameter = rOL_DV.HasValue ?
                new ObjectParameter("ROL_DV", rOL_DV) :
                new ObjectParameter("ROL_DV", typeof(decimal));
    
            var rUTParameter = rUT.HasValue ?
                new ObjectParameter("RUT", rUT) :
                new ObjectParameter("RUT", typeof(decimal));
    
            var aNIO_CUOTAParameter = aNIO_CUOTA.HasValue ?
                new ObjectParameter("ANIO_CUOTA", aNIO_CUOTA) :
                new ObjectParameter("ANIO_CUOTA", typeof(decimal));
    
            var nUM_CUOTAParameter = nUM_CUOTA.HasValue ?
                new ObjectParameter("NUM_CUOTA", nUM_CUOTA) :
                new ObjectParameter("NUM_CUOTA", typeof(decimal));
    
            var fEC_PAGOParameter = fEC_PAGO != null ?
                new ObjectParameter("FEC_PAGO", fEC_PAGO) :
                new ObjectParameter("FEC_PAGO", typeof(string));
    
            var fEC_VENCParameter = fEC_VENC != null ?
                new ObjectParameter("FEC_VENC", fEC_VENC) :
                new ObjectParameter("FEC_VENC", typeof(string));
    
            var vALOR_CUOTAParameter = vALOR_CUOTA.HasValue ?
                new ObjectParameter("VALOR_CUOTA", vALOR_CUOTA) :
                new ObjectParameter("VALOR_CUOTA", typeof(decimal));
    
            var vALOR_IPCParameter = vALOR_IPC.HasValue ?
                new ObjectParameter("VALOR_IPC", vALOR_IPC) :
                new ObjectParameter("VALOR_IPC", typeof(decimal));
    
            var vALOR_INTEParameter = vALOR_INTE.HasValue ?
                new ObjectParameter("VALOR_INTE", vALOR_INTE) :
                new ObjectParameter("VALOR_INTE", typeof(decimal));
    
            var cOD_TRANSParameter = cOD_TRANS.HasValue ?
                new ObjectParameter("COD_TRANS", cOD_TRANS) :
                new ObjectParameter("COD_TRANS", typeof(decimal));
    
            var cOD_CA_CAParameter = cOD_CA_CA.HasValue ?
                new ObjectParameter("COD_CA_CA", cOD_CA_CA) :
                new ObjectParameter("COD_CA_CA", typeof(decimal));
    
            var cOD_TARJETAParameter = cOD_TARJETA.HasValue ?
                new ObjectParameter("COD_TARJETA", cOD_TARJETA) :
                new ObjectParameter("COD_TARJETA", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("PA_PAGO_ASEO_WS", rOL_PROParameter, rOL_DVParameter, rUTParameter, aNIO_CUOTAParameter, nUM_CUOTAParameter, fEC_PAGOParameter, fEC_VENCParameter, vALOR_CUOTAParameter, vALOR_IPCParameter, vALOR_INTEParameter, cOD_TRANSParameter, cOD_CA_CAParameter, cOD_TARJETAParameter);
        }
    
        public virtual ObjectResult<Buscar_beneficiario_Result> Buscar_beneficiario(Nullable<decimal> rut, string dv)
        {
            var rutParameter = rut.HasValue ?
                new ObjectParameter("Rut", rut) :
                new ObjectParameter("Rut", typeof(decimal));
    
            var dvParameter = dv != null ?
                new ObjectParameter("Dv", dv) :
                new ObjectParameter("Dv", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Buscar_beneficiario_Result>("Buscar_beneficiario", rutParameter, dvParameter);
        }
    
        public virtual ObjectResult<Buscar_beneficiario2_Result> Buscar_beneficiario2(Nullable<decimal> mANZANA, Nullable<decimal> pREDIO, Nullable<decimal> periodo)
        {
            var mANZANAParameter = mANZANA.HasValue ?
                new ObjectParameter("MANZANA", mANZANA) :
                new ObjectParameter("MANZANA", typeof(decimal));
    
            var pREDIOParameter = pREDIO.HasValue ?
                new ObjectParameter("PREDIO", pREDIO) :
                new ObjectParameter("PREDIO", typeof(decimal));
    
            var periodoParameter = periodo.HasValue ?
                new ObjectParameter("Periodo", periodo) :
                new ObjectParameter("Periodo", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Buscar_beneficiario2_Result>("Buscar_beneficiario2", mANZANAParameter, pREDIOParameter, periodoParameter);
        }
    }
}
