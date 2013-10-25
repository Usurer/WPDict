using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using PhoneLib;
using PhoneLib.Helpers;

namespace PhoneGui_1
{
    public partial class MainPage : PhoneApplicationPage
    {
        private DictionaryDataProvider DictionaryData { get; set; }
        private Stream DictStream { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var dictFilePath = "data/dict.dsl";
            var resource = Application.GetResourceStream(new Uri(dictFilePath, UriKind.Relative));
            DictStream = resource.Stream;
            DictionaryData = new DictionaryDataProvider(DictStream);
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            var query = txtInputBox.Text;
            var searchResult = DictionaryData.GetCardAsTags(query, DictStream).ConvertToNoTagsString();

            var css = "body {background-color: #000; color: #fff; /*height: 200px; overflow-y: scroll;*/} div {/*height: 1000px;*/}";
            var encoding = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1251\">";
            var head = string.Format("<html><head>{0}<style>{1}</style></head><body>", encoding, css);
            var bottom = "</body></html>";
            var mainContent = string.Format("<div>{0}</div>", ConvertExtendedASCII(HttpUtility.HtmlEncode(searchResult)));
            brsResult.NavigateToString(string.Format("{0}{1}{2}", head, mainContent, bottom));
        }

        // Hack I've taken from http://matthiasshapiro.com/2010/10/25/international-utf-8-characters-in-windows-phone-7-webbrowser-control/
        private static string ConvertExtendedASCII(string htmlString)
        {
            string retVal = "";
            char[] s = htmlString.ToCharArray();

            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }

            return retVal;
        }
    }
}