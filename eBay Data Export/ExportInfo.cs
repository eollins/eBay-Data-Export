using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace eBay_Data_Export
{
    public partial class ExportInfo : Form
    {
        int pageNumber = 5000;
        public ExportInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ExportParams.apiCall == "soldItems")
            {
                soldItems();
            }
            else if (ExportParams.apiCall == "purchasedItems")
            {

            }
            else if (ExportParams.apiCall == "completedListings")
            {

            }

            
        }

        public void soldItems()
        {
            string info = "Item Name,Date Sold,Price,Buyer,Time Sold\n";
            DateTime start = DateTime.Now;
            for (int i = 1; i < pageNumber; i++)
            {
                if (i == 1)
                {
                    progressBar1.Value = progressBar1.Maximum;
                }

                progressBar2.Maximum = pageNumber;
                progressBar2.Value++;
                label2.Text = "Downloading Page " + i + "/" + pageNumber;
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
                request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<GetSellerTransactionsRequest xmlns=\"urn:ebay:apis:eBLBaseComponents\">\r\n  <RequesterCredentials>\r\n    <eBayAuthToken>" + ExportParams.authToken + "</eBayAuthToken>\r\n  </RequesterCredentials>\r\n  <IncludeCodiceFiscale>true</IncludeCodiceFiscale>\r\n  <IncludeContainingOrder>true</IncludeContainingOrder>\r\n <NumberOfDays>" + ExportParams.numberOfDays + "</NumberOfDays> <IncludeFinalValueFee>true</IncludeFinalValueFee>\r\n  <Pagination> PaginationType\r\n    <EntriesPerPage>200</EntriesPerPage>\r\n    <PageNumber>" + i + "</PageNumber>\r\n  </Pagination>\r\n  <DetailLevel>ReturnAll</DetailLevel>\r\n  <ErrorLanguage>en_US</ErrorLanguage>\r\n  <Version>967</Version>\r\n  <WarningLevel>High</WarningLevel>\r\n</GetSellerTransactionsRequest>", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response.Content);
                XmlNodeList nodes = ((XmlElement)doc.GetElementsByTagName("TransactionArray")[0]).GetElementsByTagName("Transaction");

                if (i == 1)
                {
                    pageNumber = int.Parse(((XmlElement)doc.GetElementsByTagName("PaginationResult")[0]).GetElementsByTagName("TotalNumberOfPages")[0].InnerText);
                }

                foreach (XmlElement ele in nodes)
                {

                    string first = ele.GetElementsByTagName("EndTime")[0].InnerText;
                    string[] components = first.Split('T');
                    string[] date = components[0].Split('-');
                    string[] time = components[1].Split(':');
                    time[2] = time[2].Substring(0, time[2].IndexOf('.'));

                    string finalDate = date[1] + "/" + date[2] + "/" + date[0];

                    DateTime dt = new DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]), int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]));
                    int subtraction = Convert.ToInt32(-1 * ExportParams.numberOfDays);
                    DateTime minimumDay = DateTime.Now.AddDays(subtraction);
                    if (DateTime.Compare(dt, minimumDay) < 0)
                        continue;

                    info += ele.GetElementsByTagName("Title")[0].InnerText;
                    info += "," + finalDate;

                    string price;
                    try
                    {
                        price = ((XmlElement)((XmlElement)ele.GetElementsByTagName("Item")[0]).GetElementsByTagName("SellingStatus")[0]).GetElementsByTagName("CurrentPrice")[0].InnerText; //amount paid, which includes extra fees
                    }
                    catch
                    {
                        price = ",Unpaid";
                    }

                    if (price != ",Unpaid")
                    {
                        decimal num = decimal.Parse(price);
                        price = String.Format("{0:C}", num);
                        //MessageBox.Show(price);
                        info += "," + price;
                    }

                    info += "," + ((XmlElement)ele.GetElementsByTagName("Buyer")[0]).GetElementsByTagName("Name")[0].InnerText;

                    string finalTime = time[0] + ":" + time[1] + ":" + time[2];
                    info += "," + finalTime + "\n";
                }
            }


            DateTime end = DateTime.Now;

            TimeSpan span = end - start;

            MessageBox.Show("Download completed in " + span.Seconds + " seconds");
            progressBar3.Value = 100;
            progressBar3.Maximum = 100;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog1.FileName);
                writer.Write(info);
                writer.Close();
                MessageBox.Show("File saved successfully");
            }
        }
    }
}
