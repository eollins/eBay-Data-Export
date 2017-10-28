using eBay.Service.Core.Sdk;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace eBay_Data_Export
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var client = new RestClient("https://api.sandbox.ebay.com/ws/api.dll");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-ebay-api-siteid", "0");
            request.AddHeader("x-ebay-api-call-name", "GetSessionID");
            request.AddHeader("x-ebay-api-compatibility-level", "967");
            request.AddHeader("content-type", "text/xml");
            request.AddHeader("x-ebay-api-cert-name", "SBX-45f0c76378c2-8dd7-40e6-b49d-0c12");
            request.AddHeader("x-ebay-api-dev-name", "8105fd0e-a76c-4e10-80e8-43e86ab59f7c");
            request.AddHeader("x-ebay-api-app-name", "GregoryM-mailer-SBX-b45f0c763-f90c92ad");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<GetSessionIDRequest xmlns=\"urn:ebay:apis:eBLBaseComponents\">\n\t<RuName>Gregory_Morris_-GregoryM-mailer-fkfdcrz</RuName>\n\t<ErrorLanguage>en_US</ErrorLanguage>\n\t<MessageID></MessageID>\n\t<Version>967</Version>\n\t<WarningLevel>High</WarningLevel>\n</GetSessionIDRequest>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            //MessageBox.Show(((int)response.StatusCode).ToString());
            //MessageBox.Show(response.Content);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content);
            XmlNodeList nodes = ((XmlElement)doc.GetElementsByTagName("GetSessionIDResponse")[0]).GetElementsByTagName("SessionID");

            string sessionID = nodes[0].InnerText;
        }
    }
}
