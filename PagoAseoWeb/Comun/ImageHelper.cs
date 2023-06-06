using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace SocialAseoWeb.Comun
{
    public class ImageHelper
    {
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            return Image.FromStream(ms);
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Png);
            // return Convert.ToBase64String(ms.ToArray());
            return ms.ToArray();

        }

        public static Image ObtenerImagenNoDisponible()
        {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Stream file = assembly.GetManifestResourceStream("Helpers.Imagenes.NoDisponible.jpg");
            return Image.FromStream(file);
        }

        public static Image ObtenerCodigoQr(string ruta)
        {
            Image imagen;
            FileStream file = File.Open(ruta, FileMode.Open);
            imagen = Image.FromStream(file);
            file.Close();
            return imagen;

        }
    }

}