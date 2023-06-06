using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Image = System.Drawing.Image;
using ThoughtWorks.QRCode;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SocialAseoWeb.Comun
{
    public static class CodigoQr
    {
        public static void crearCodigoQr(string rutaNombre, string datos)
        {
            if (!File.Exists(rutaNombre))
            {
                QRCodeEncoder codigoQr = new QRCodeEncoder();
                codigoQr.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                codigoQr.QRCodeScale = 4;
                codigoQr.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                codigoQr.QRCodeVersion = 0;
                codigoQr.QRCodeBackgroundColor = Color.FromArgb(255, 255, 255, 255);
                codigoQr.QRCodeForegroundColor = Color.FromArgb(255, 50, 50, 150);
                PictureBox imgQr = new PictureBox();
                imgQr.Image = codigoQr.Encode(datos, System.Text.Encoding.UTF8);

                SaveFileDialog gurdarQr = new SaveFileDialog();
                gurdarQr.Filter = "JPEG|*.jpg|Mapa de Bits|*.bmp|Gif|*.gif|PNG|*.png";
                gurdarQr.Title = "Guardar código QR";
                gurdarQr.FileName = rutaNombre;
                System.IO.FileStream fs = (System.IO.FileStream)gurdarQr.OpenFile();
                imgQr.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                fs.Close();

            }

        }
    }
}
