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
        string sessionID = "";
        string authToken = "";
        bool loggedIn = false;

        string[] soldItems = new string[] { "Item Name", "Date Sold", "Price", "Buyer Name", "Time of Sale" };
        string[] purchasedItems = new string[] { "Item Name", "Date Purchased", "Price", "Seller", "Item ID" };

        string pathToItemTitle = "TransactionArray;Transaction;Item;Title";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var client = new RestClient("https://api.ebay.com/ws/api.dll");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-ebay-api-siteid", "0");
            request.AddHeader("x-ebay-api-call-name", "GetSessionID");
            request.AddHeader("x-ebay-api-compatibility-level", "967");
            request.AddHeader("content-type", "text/xml");
            request.AddHeader("x-ebay-api-cert-name", "PRD-45ed603527c9-2461-4859-9906-7f37");
            request.AddHeader("x-ebay-api-dev-name", "8105fd0e-a76c-4e10-80e8-43e86ab59f7c");
            request.AddHeader("x-ebay-api-app-name", "GregoryM-mailer-PRD-a45ed6035-97c14545");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<GetSessionIDRequest xmlns=\"urn:ebay:apis:eBLBaseComponents\">\n\t<RuName>Gregory_Morris_-GregoryM-mailer-viaojj</RuName>\n\t<ErrorLanguage>en_US</ErrorLanguage>\n\t<MessageID></MessageID>\n\t<Version>967</Version>\n\t<WarningLevel>High</WarningLevel>\n</GetSessionIDRequest>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            //MessageBox.Show(((int)response.StatusCode).ToString());
            //MessageBox.Show(response.Content);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content);
            XmlNodeList nodes = ((XmlElement)doc.GetElementsByTagName("GetSessionIDResponse")[0]).GetElementsByTagName("SessionID");

            sessionID = nodes[0].InnerText;

            Process.Start("https://signin.ebay.com/ws/eBayISAPI.dll?SignIn&runame=Gregory_Morris_-GregoryM-mailer-viaojj&SessID=" + sessionID);
            MessageBox.Show("Click \"OK\" when finished logging in");
            FetchToken(sessionID);
        }

        string FetchToken(string sessionID)
        {
            var client = new RestClient("https://api.ebay.com/ws/api.dll");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "aa94b5fa-1e64-1402-a25b-6505dde5c093");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-ebay-api-siteid", "0");
            request.AddHeader("x-ebay-api-call-name", "FetchToken");
            request.AddHeader("x-ebay-api-compatibility-level", "967");
            request.AddHeader("content-type", "text/xml");
            request.AddHeader("x-ebay-api-cert-name", "PRD-45ed603527c9-2461-4859-9906-7f37");
            request.AddHeader("x-ebay-api-dev-name", "8105fd0e-a76c-4e10-80e8-43e86ab59f7c");
            request.AddHeader("x-ebay-api-app-name", "GregoryM-mailer-PRD-a45ed6035-97c14545");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<FetchTokenRequest xmlns=\"urn:ebay:apis:eBLBaseComponents\">\n\t<SessionID>" + sessionID + "</SessionID>\n\t<ErrorLanguage>en_US</ErrorLanguage>\n\t<Version>967</Version>\n\t<WarningLevel>High</WarningLevel>\n</FetchTokenRequest>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content);
            XmlNodeList nodes = ((XmlElement)doc.GetElementsByTagName("FetchTokenResponse")[0]).GetElementsByTagName("eBayAuthToken");

            try
            {
                authToken = nodes[0].InnerText;
                MessageBox.Show("Login successful.");
                label1.Text = "Logged in";
                loggedIn = true;
                button1.Enabled = false;
                button2.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Error logging in.");
                label1.Text = "Not logged in";
                loggedIn = false;
                button1.Enabled = true;
                button2.Enabled = false;
            }

            return nodes[0].InnerText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var client = new RestClient("https://api.ebay.com/ws/api.dll");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-ebay-api-siteid", "0");
            request.AddHeader("x-ebay-api-call-name", "RevokeToken");
            request.AddHeader("x-ebay-api-compatibility-level", "967");
            request.AddHeader("content-type", "text/xml");
            request.AddHeader("x-ebay-api-cert-name", "PRD-45ed603527c9-2461-4859-9906-7f37");
            request.AddHeader("x-ebay-api-dev-name", "8105fd0e-a76c-4e10-80e8-43e86ab59f7c");
            request.AddHeader("x-ebay-api-app-name", "GregoryM-mailer-PRD-a45ed6035-97c14545");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?><RevokeTokenRequest xmlns=\"urn: ebay:apis: eBLBaseComponents\"><UnsubscribeNotification> boolean </UnsubscribeNotification><ErrorLanguage> string </ErrorLanguage><MessageID> string </MessageID><Version> string </Version><WarningLevel> WarningLevelCodeType </WarningLevel></RevokeTokenRequest>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            XmlDocument responseXml = new XmlDocument();
            responseXml.LoadXml(response.Content);
            MessageBox.Show(responseXml.InnerText);

            loggedIn = false;
            button1.Enabled = true;
            button2.Enabled = false;
            label1.Text = "Not logged in";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            if (radioButton1.Checked)
            {
                foreach (string s in soldItems)
                {
                    checkedListBox1.Items.Add(s);
                }
            }
            else
            {
                foreach (string s in purchasedItems)
                {
                    checkedListBox1.Items.Add(s);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var client = new RestClient("https://api.ebay.com/ws/api.dll");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "7f88d4bd-7f50-42b1-b6e6-1efab465043e");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-ebay-api-siteid", "0");
            request.AddHeader("x-ebay-api-call-name", "GetSellerTransactions");
            request.AddHeader("x-ebay-api-compatibility-level", "967");
            request.AddHeader("content-type", "text/xml");
            request.AddHeader("x-ebay-api-cert-name", "PRD-45ed603527c9-2461-4859-9906-7f37");
            request.AddHeader("x-ebay-api-dev-name", "8105fd0e-a76c-4e10-80e8-43e86ab59f7c");
            request.AddHeader("x-ebay-api-app-name", "GregoryM-mailer-PRD-a45ed6035-97c14545");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<GetSellerTransactionsRequest xmlns=\"urn:ebay:apis:eBLBaseComponents\">\r\n  <RequesterCredentials>\r\n    <eBayAuthToken>" + authToken + "</eBayAuthToken>\r\n  </RequesterCredentials>\r\n  <IncludeCodiceFiscale>true</IncludeCodiceFiscale>\r\n  <IncludeContainingOrder>true</IncludeContainingOrder>\r\n  <IncludeFinalValueFee>true</IncludeFinalValueFee>\r\n  <NumberOfDays>1</NumberOfDays>\r\n  <Pagination> PaginationType\r\n    <EntriesPerPage>15</EntriesPerPage>\r\n    <PageNumber>1</PageNumber>\r\n  </Pagination>\r\n  <DetailLevel>ReturnAll</DetailLevel>\r\n  <ErrorLanguage>en_US</ErrorLanguage>\r\n  <Version>967</Version>\r\n  <WarningLevel>High</WarningLevel>\r\n</GetSellerTransactionsRequest>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content);
            XmlNodeList nodes = ((XmlElement)doc.GetElementsByTagName("TransactionArray")[0]).GetElementsByTagName("Transaction");
            string info = "Item Name,Date Sold,Price,Buyer,Time Sold\n";
            foreach (XmlElement ele in nodes)
            {
                info += ele.GetElementsByTagName("Title")[0].InnerText;

                string first = ele.GetElementsByTagName("EndTime")[0].InnerText;
                string[] components = first.Split('T');
                string[] date = components[0].Split('-');
                string[] time = components[1].Split(':');
                time[2] = time[2].Substring(0, time[2].IndexOf('.'));

                string finalDate = date[1] + "/" + date[2] + "/" + date[0];
                info += "," + finalDate;

                string price;
                try
                {
                    price = ((XmlElement)ele.GetElementsByTagName("ExternalTransaction")[0]).GetElementsByTagName("PaymentOrRefundAmount")[0].InnerText; //amount paid, which includes extra fees
                }
                catch
                {
                    price = ",Unpaid";
                }

                if (price != ",Unpaid") {
                    decimal num = decimal.Parse(price);
                    price = String.Format("C", num);
                    info += "," + price;
                }

                info += price;

                info += "," + ((XmlElement)ele.GetElementsByTagName("Buyer")[0]).GetElementsByTagName("Name")[0].InnerText;

                string finalTime = time[0] + ":" + time[1] + ":" + time[2];
                info += "," + finalTime + "\n";
            }

            MessageBox.Show(info);
        }
    }
}
