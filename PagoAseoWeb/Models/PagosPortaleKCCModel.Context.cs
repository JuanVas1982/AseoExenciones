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
    
    public partial class PagosPortaleKCCEntities : DbContext
    {
        public PagosPortaleKCCEntities()
            : base("name=PagosPortaleKCCEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<PPKCC_FOLIOS> PPKCC_FOLIOS { get; set; }
        public DbSet<TGR_ENVIA_GIRO> TGR_ENVIA_GIRO { get; set; }
        public DbSet<TGR_RECIBE_PAGO> TGR_RECIBE_PAGO { get; set; }
        public DbSet<WEBPAY> WEBPAY { get; set; }
        public DbSet<PPKCC_FOLIOS_ASIGNADOS> PPKCC_FOLIOS_ASIGNADOS { get; set; }
        public DbSet<PPKCC_TRANSAC_ASEO> PPKCC_TRANSAC_ASEO { get; set; }
    }
}
