using System;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneLib.Helpers
{
    public static class StringExtensions
    {
        // Hack I've taken from http://matthiasshapiro.com/2010/10/25/international-utf-8-characters-in-windows-phone-7-webbrowser-control/
        public static string ConvertExtendedAscii(this string htmlString)
        {
            /*string retVal = "";
            char[] s = htmlString.ToCharArray();

            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }

            return retVal;*/
            StringBuilder sb = new StringBuilder();
            char[] s = htmlString.ToCharArray();
            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    sb.Append("&#" + Convert.ToInt32(c) + ";");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
