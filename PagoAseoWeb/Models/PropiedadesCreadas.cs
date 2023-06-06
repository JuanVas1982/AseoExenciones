using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialAseoWeb.Models;
using System.Data.SqlClient;
using SocialAseoWeb.Comun;

namespace SocialAseoWeb.Models
{
    public partial class AS_PAGOS
    {
        public decimal INTERES { get; set; }
        public decimal MULTA { get; set; }
        public decimal TOTAL { get; set; }

        public string valorneto
        {
            get
            {
                return Utils.valorneto(PAG_ROL_NUM,PAG_ROL_DV,PAG_ANO_CON);
            }
        }
    }
    public partial class AS_CONVENIO
    {
        public string CLASIFICACION
        {
            get
            {
                return Utils.clasificacion(CON_CODEX);
            }
        }
    }
    public partial class OrdenIngresoAseo
    {
        public List<DetalleOrdenIngresoAseo> DOI { get; set; }


    }
    public static class FUNCIONES_DDBB
    {
        public static decimal FN_OBT_IPC(decimal MesActual, decimal AnoActual, decimal MesV, decimal AnoV)
        {
            using (aseoEntities entidadesPermiso = new aseoEntities())
            {
                decimal ipc = entidadesPermiso.Database.SqlQuery<decimal>(
                            "SELECT DBO.SacarIPC(@MesActual,@AnoActual,@MesV,@AnoV)",
                            new SqlParameter("MesActual", MesActual),
                            new SqlParameter("AnoActual", AnoActual),
                            new SqlParameter("MesV", MesV),
                            new SqlParameter("AnoV", AnoV)
                            ).FirstOrDefault();
                return ipc;
            }
        }
        public static decimal FN_OBT_MULTA(decimal MesActual, decimal AnoActual, decimal MesV, decimal AnoV)
        {
            using (aseoEntities entidadesPermiso = new aseoEntities())
            {
                decimal multa = entidadesPermiso.Database.SqlQuery<decimal>(
                            "SELECT DBO.SacarMulta(@MesActual,@AnoActual,@MesV,@AnoV)",
                            new SqlParameter("MesActual", MesActual),
                            new SqlParameter("AnoActual", AnoActual),
                            new SqlParameter("MesV", MesV),
                            new SqlParameter("AnoV", AnoV)
                            ).FirstOrDefault();
                return multa;
            }
        }
    }
}