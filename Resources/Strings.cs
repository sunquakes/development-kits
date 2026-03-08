using System.Globalization;
using System.Resources;
using System.Windows;

namespace DevTools.Resources
{
    public static class Strings
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("DevTools.Resources.Strings", typeof(Strings).Assembly);

        public static string Get(string key)
        {
            try
            {
                return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
            }
            catch
            {
                return key;
            }
        }

        public static string Back => Get("Back");
        public static string Convert => Get("Convert");
        public static string Copy => Get("Copy");
        public static string Generate => Get("Generate");
        public static string ClearAll => Get("ClearAll");
        public static string ShowOnly => Get("ShowOnly");
        public static string Save => Get("Save");
        public static string Delete => Get("Delete");
        public static string Format => Get("Format");
        public static string ExpandAll => Get("ExpandAll");
        public static string CollapseAll => Get("CollapseAll");
        public static string CopyFormatted => Get("CopyFormatted");
        public static string Decode => Get("Decode");
        public static string SelectImage => Get("SelectImage");
        public static string Toolbox => Get("Toolbox");
        public static string MD5 => Get("MD5");
        public static string Barcode => Get("Barcode");
        public static string QRCode => Get("QRCode");
        public static string Base64ToImage => Get("Base64ToImage");
        public static string ImageToBase64 => Get("ImageToBase64");
        public static string JSONFormat => Get("JSONFormat");
        public static string Result => Get("Result");
        public static string Lower32 => Get("Lower32");
        public static string Upper32 => Get("Upper32");
        public static string Lower16 => Get("Lower16");
        public static string Upper16 => Get("Upper16");
        public static string ImageSize => Get("ImageSize");
        public static string Size => Get("Size");
        public static string Base64 => Get("Base64");
        public static string IncludeDataUri => Get("IncludeDataUri");
        public static string PageMD5 => Get("PageMD5");
        public static string PageBarcode => Get("PageBarcode");
        public static string PageQRCode => Get("PageQRCode");
        public static string PageBase64ToImage => Get("PageBase64ToImage");
        public static string PageImageToBase64 => Get("PageImageToBase64");
        public static string PageJSONFormat => Get("PageJSONFormat");
        public static string CopySuccess => Get("CopySuccess");
        public static string CopyEmpty => Get("CopyEmpty");
        public static string Info => Get("Info");
        public static string Error => Get("Error");
        public static string Warning => Get("Warning");
        public static string EnterBase64 => Get("EnterBase64");
        public static string Base64DecodeFailed => Get("Base64DecodeFailed");
        public static string DecodeFailed => Get("DecodeFailed");
        public static string EnterTextToEncode => Get("EnterTextToEncode");
        public static string QRCodeGenerateFailed => Get("QRCodeGenerateFailed");
        public static string NoQRCodeToSave => Get("NoQRCodeToSave");
        public static string NoBarcodeToSave => Get("NoBarcodeToSave");
        public static string SaveSuccess => Get("SaveSuccess");
        public static string SaveFailed => Get("SaveFailed");
        public static string ComputeFirst => Get("ComputeFirst");
        public static string EnterJSON => Get("EnterJSON");
        public static string JSONFormatFailed => Get("JSONFormatFailed");
        public static string LoadImageFailed => Get("LoadImageFailed");
        public static string BarcodeGenerateFailed => Get("BarcodeGenerateFailed");
    }
}
