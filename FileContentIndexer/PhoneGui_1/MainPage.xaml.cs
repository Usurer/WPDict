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

        private bool IsTranslatedFlag { get; set; }
        private bool IsInitialized { get; set; }

        private bool IsBusy { get; set; }

        public event EventHandler IndexStart;
        public event EventHandler IndexEnd;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            brsResult.Background = new SolidColorBrush(Colors.Black);
            txtMessages.Text = string.Empty;

            IndexStart += new EventHandler(OnIndexStart);
            IndexEnd += new EventHandler(OnIndexEnd);
        }

        /* TODO: Now you'll get only one definition for words like Man and man. So instead of using First in index search, you'd better get all matches. */
        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            var query = txtInputBox.Text;
            txtMessages.Text = txtInputBox.Text;
            if (string.IsNullOrEmpty(query) || !IsInitialized)
            {
                return;
            }
            var reply = string.Empty;
            /*var searchResult = DictionaryData.GetCardAsTags(query, DictStream).ConvertToNoTagsString();*/
            var searchResult = DictionaryData.GetCardAsTags(query, DictStream);
            reply = searchResult == null ? "Nothing was found" : searchResult.ConvertToStyledStrings();

            var css = "body {background-color: #000; color: #fff; } div {}";
            css = css + ".italics { font-style: italic;}";
            css = css + ".bold { font-weight: bold;}";
            css = css + ".underline { text-decoration: underline;}";
            css = css + ".coloured { color: #afeeee;}";
            css = css + ".translation { padding: 0 0 0 5%;}";
            var encoding = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-1251\">";
            var head = string.Format("<html><head>{0}<style>{1}</style></head><body>", encoding, css);
            var bottom = "</body></html>";
            if (reply.StartsWith("\n" + query + "\n"))
            {
                reply = reply.Substring(string.Format("{0}{1}{0}", "\n", query).Length);
            }
            var mainContent = string.Format("<div>{0}</div>", reply.TrimStart(query.ToCharArray()));
            brsResult.NavigateToString(string.Format("{0}{1}{2}", head, mainContent, bottom));

            IsTranslatedFlag = true;
        }

        private void QueryBoxFocused(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtInputBox.Text))
            {
                if (IsTranslatedFlag)
                {
                    txtInputBox.Text = string.Empty;
                    IsTranslatedFlag = false;
                }
            }
        }

        private void OnAppLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnIndexClicked(object sender, RoutedEventArgs e)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            /*IndexStart.Invoke(this, new EventArgs());*/

            txtInputBox.IsEnabled = false;
            btnSearch.IsEnabled = false;

            txtMessages.Text = "Getting file...";
            var dictFilePath = "data/dict.dsl";
            var resource = Application.GetResourceStream(new Uri(dictFilePath, UriKind.Relative));
            DictStream = resource.Stream;

            txtMessages.Text = "Indexing in proccess...";
            DictionaryData = new DictionaryDataProvider(DictStream);

            IsInitialized = true;
            txtMessages.Text = string.Empty;

            txtInputBox.IsEnabled = true;
            btnSearch.IsEnabled = true;

            IsBusy = false;

            /*IndexStart.Invoke(this, new EventArgs());*/
        }

        void OnIndexStart(object sender, EventArgs e)
        {
            IsBusy = true;
            IndexStart.Invoke(this, new EventArgs());

            txtInputBox.IsEnabled = false;
            btnSearch.IsEnabled = false;

            txtMessages.Text = "Getting file...";
            var dictFilePath = "data/dict.dsl";
            var resource = Application.GetResourceStream(new Uri(dictFilePath, UriKind.Relative));
            DictStream = resource.Stream;

            txtMessages.Text = "Indexing in proccess...";
            DictionaryData = new DictionaryDataProvider(DictStream);

            IsInitialized = true;
            txtMessages.Text = string.Empty;

            txtInputBox.IsEnabled = true;
            btnSearch.IsEnabled = true;
            
            IndexEnd.Invoke(this, new EventArgs());
        }

        void OnIndexEnd(object sender, EventArgs e)
        {
            IsBusy = false;
            txtMessages.Text = string.Empty;
        }
    }
}