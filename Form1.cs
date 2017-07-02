using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Parset
{
    public partial class Form1 : Form
    {
        string path;
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Parse_Click(object sender, EventArgs er)
        {
            var fbd = new FolderBrowserDialog();

            if(path!=null)
                fbd.SelectedPath = path;

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                path = fbd.SelectedPath;

                GetWebpage(tb_URL.Text);
            }
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = (WebBrowser)sender;
            var client = new WebClient();
            int ind = 0;

            string htmlCode = browser.DocumentText;

            string pattern = "< img.+? src =[\"'](.+?)[\"'].+?>";

            foreach (Match m in Regex.Matches(htmlCode, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                if (ind >= numericUpDown1.Value)
                {
                    break;
                }

                string src = m.Groups[1].Value;

                if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
                {
                    src = string.Concat(browser.Document.Url.AbsoluteUri, "/", src);
                }

                try
                {
                    var filename = Path.Combine(path, ind + ".png");

                    var req = WebRequest.Create(src);
                    var imagestream = req.GetResponse().GetResponseStream();
                    var bmp = Image.FromStream(imagestream);
                    bmp.Save(filename);

                    imagestream.Dispose();
                    bmp.Dispose();

                    ind++;
                }
                catch { }
            }
        }

        private void GetWebpage(string url)
        {
            WebBrowser browser = new WebBrowser();
            browser.Navigate(url);
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }
        /*
        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = (WebBrowser)sender;
            var client = new WebClient();
            int ind = 0;

            HtmlElementCollection ecol = browser.Document.GetElementsByTagName("img");

            foreach (HtmlElement img in ecol)
            {
                if(ind >= numericUpDown1.Value)
                {
                    break;
                }

                var src = img.GetAttribute("src");

                if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
                {
                    src = string.Concat(browser.Document.Url.AbsoluteUri, "/", src);
                }

                var req = WebRequest.Create(src);

                //Append any path to filename as needed
                var filename = Path.Combine(path, ind + ".png");

                var imagestream = req.GetResponse().GetResponseStream();
                var bmp = Image.FromStream(imagestream);
                bmp.Save(filename);

                imagestream.Dispose();
                bmp.Dispose();

                //var image = img as HtmlElement;
                //var src = image.GetAttribute("src");

                //if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
                //{
                //    src = string.Concat(browser.Document.Url.AbsoluteUri, "/", src);
                //}

                ////Append any path to filename as needed
                //var filename = Path.Combine(path, ind + ".png");

                //try
                //{
                //    File.WriteAllBytes(filename, client.DownloadData(src));

                //    ind++;
                //}
                //catch {}
            }
        }*/

    }
}
