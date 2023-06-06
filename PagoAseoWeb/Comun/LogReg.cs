using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Permissions;
using System.Configuration;
namespace SocialAseoWeb.Comun
{
    public class LogReg
    {
        string path = ConfigurationManager.AppSettings["RutaLogTrans"];
        string pathReg = "";
        StreamWriter writer;

        public LogReg()
        {
            DateTime Hoy = DateTime.Today;
            string fecha_actual = Hoy.ToString("yyyyMMdd");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileIOPermission f = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
            else
            {
                FileIOPermission f = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }

            path = path + fecha_actual + "\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
            else
            {
                FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
        }

        public void abrirArchivo(string archivo)
        {
            try
            {
                pathReg = archivo + ".log";
                if (!File.Exists(path + pathReg))
                {
                    FileStream newPath = File.Create(Path.Combine(path, pathReg), 1, FileOptions.Asynchronous);
                    newPath.Close();
                    newPath.Dispose();
                }
                else
                {
                    FileStream newPath = File.Open(Path.Combine(path, pathReg), FileMode.Append);
                    newPath.Close();
                    newPath.Dispose();
                }
            }
            catch
            {
                Random r = new Random();
                string id = r.Next(1000).ToString();
                pathReg = archivo + "-" + id + ".log";
                if (!File.Exists(path + pathReg))
                {
                    FileStream newPath = File.Create(Path.Combine(path, pathReg), 1, FileOptions.Asynchronous);
                    newPath.Close();
                    newPath.Dispose();
                }
            }
        }

        public void encabezado(string logMessage)
        {
            FileStream newPath = File.Open(Path.Combine(path, pathReg), FileMode.Append);
            writer = new StreamWriter(newPath);
            writer.WriteLine("Log Entry: " + logMessage);
            writer.Close();
            writer.Dispose();
            newPath.Close();
            newPath.Dispose();
        }
        public void registrar(string logMessage)
        {
            FileStream newPath = File.Open(Path.Combine(path, pathReg), FileMode.Append);
            writer = new StreamWriter(newPath);
            writer.Write("{0}", DateTime.Now.ToString());
            writer.WriteLine(" :{0}", logMessage);
            writer.Close();
            writer.Dispose();
            newPath.Close();
            newPath.Dispose();
        }

        public void cerrar()
        {
            
        }
    }
}