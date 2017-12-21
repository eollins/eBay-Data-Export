using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
                Application.Exit();
            }
            else if (ExportParams.apiCall == "purchasedItems")
            {
                purchasedItems();
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

        public async void purchasedItems()
        {
            progressBar1.Value = 0;
            progressBar1.Maximum = 1;
            progressBar2.Value = 0;
            progressBar2.Maximum = pageNumber;
            progressBar3.Value = 0;
            progressBar3.Maximum = 1;

            DateTime start = DateTime.Now;
            string info = "Item Title,Price Paid,Date,Seller ID,Item Number,Paid,Date Paid,Shipped,Shipping Price,Shipping Carrier,Tracking Number,Shipping Status\n";

            for (int i = 1; i < pageNumber; i++)
            {
                WebRequest request = WebRequest.Create("https://api.ebay.com/ws/api.dll");
                request.Method = "POST";
                request.ContentType = "text/xml";
                request.Headers.Add("x-ebay-api-siteid", "0");
                request.Headers.Add("x-ebay-api-call-name", "GetOrders");
                request.Headers.Add("x-ebay-api-compatibility-level", "967");
                request.Headers.Add("x-ebay-api-cert-name", "PRD-45ed603527c9-2461-4859-9906-7f37");
                request.Headers.Add("x-ebay-api-dev-name", "8105fd0e-a76c-4e10-80e8-43e86ab59f7c");
                request.Headers.Add("x-ebay-api-app-name", "GregoryM-mailer-PRD-a45ed6035-97c14545");

                XmlDocument docu = new XmlDocument();
                docu.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<GetOrdersRequest xmlns=\"urn:ebay:apis:eBLBaseComponents\">\n  <RequesterCredentials>\n    <eBayAuthToken>" + ExportParams.authToken + "</eBayAuthToken>\n  </RequesterCredentials>\n  <OrderRole>Buyer</OrderRole>\n  <OrderStatus>All</OrderStatus>\n  <Pagination>\n  <EntriesPerPage>100</EntriesPerPage>\n  <PageNumber>" + i + "</PageNumber>\n  </Pagination>\n  <NumberOfDays>" + ExportParams.numberOfDays + "</NumberOfDays>\n</GetOrdersRequest>");

                request.ContentLength = docu.InnerXml.Length;
                byte[] data = new ASCIIEncoding().GetBytes(docu.InnerXml);
                request.GetRequestStream().Write(data, 0, docu.InnerXml.Length);
                 
                WebResponse response = await request.GetResponseAsync();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(new StreamReader(response.GetResponseStream()).ReadToEnd());
                XmlNodeList nodes = ((XmlElement)doc.GetElementsByTagName("GetOrdersResponse")[0]).GetElementsByTagName("PaginationResult");
                pageNumber = int.Parse(nodes[0].InnerText);
                progressBar2.Maximum = pageNumber;

                progressBar2.Value += 1;

                XmlNodeList nodes2 = ((XmlElement)((XmlElement)doc.GetElementsByTagName("GetOrdersResponse")[0]).GetElementsByTagName("OrderArray")[0]).GetElementsByTagName("Order");
                foreach (XmlElement ele in nodes2)
                {
                    string first = ele.GetElementsByTagName("CreatedDate")[0].InnerText;
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

                    string name = ele.GetElementsByTagName("Item")[0].InnerText;

                    if (name.Contains(',')) {
                        while (name.Contains(','))
                        {
                            StringBuilder sb = new StringBuilder(name);
                            sb[name.IndexOf(',')] = ' ';
                            name = sb.ToString();
                        }
                    }

                    string itemNumber = ((XmlElement)((XmlElement)ele.GetElementsByTagName("TransactionArray")[0]).GetElementsByTagName("Item")[0]).GetElementsByTagName("ItemID")[0].InnerText;
                    StringBuilder sb2 = new StringBuilder(name);
                    sb2.Replace(itemNumber, "");
                    try
                    {
                        string textToErase = name.Substring(name.IndexOf("2750"));
                        sb2.Replace(textToErase, "");
                    }
                    catch { }
                    try
                    {
                        string textToErase2 = name.Substring(name.IndexOf("5000"));
                        sb2.Replace(textToErase2, "");
                    }
                    catch { }
                    try
                    {
                        string textToErase3 = name.Substring(name.IndexOf("4000"));
                        sb2.Replace(textToErase3, "");
                    }
                    catch { }
                    try
                    {
                        string textToErase4 = name.Substring(name.IndexOf("82750"));
                        sb2.Replace(textToErase4, "");
                    }
                    catch { }
                    name = sb2.ToString();

                    string paid = ((XmlElement)doc.GetElementsByTagName("CheckoutStatus")[0]).GetElementsByTagName("Status")[0].InnerText;
                    if (paid == "Incomplete")
                    {
                        paid = "No";
                    }
                    else if (paid == "Complete")
                    {
                        paid = "Yes";
                    }

                    string shipped;
                    string shippingPrice;
                    string shippingService;
                    string trackingNumber;
                    try
                    {
                        shippingPrice = ((XmlElement)doc.GetElementsByTagName("ShippingServiceSelected")[0]).GetElementsByTagName("ShippingServiceCost")[0].InnerText;

                        try
                        {
                            shippingService = ((XmlElement)doc.GetElementsByTagName("ShippingServiceSelected")[0]).GetElementsByTagName("ShippingService")[0].InnerText;
                        }
                        catch
                        {
                            shippingService = ((XmlElement)doc.GetElementsByTagName("ShippingServiceSelected")[0]).GetElementsByTagName("ShippingCarrierUsed")[0].InnerText;
                        }

                        try
                        {
                            trackingNumber = ((XmlElement)((XmlElement)ele.GetElementsByTagName("ShippingDetails")[0]).GetElementsByTagName("ShipmentTrackingDetails")[0]).GetElementsByTagName("ShipmentTrackingNumber")[0].InnerText;
                        }
                        catch
                        {
                            trackingNumber = "Unavailable";
                        }

                        try
                        {
                            string shippedTime = ele.GetElementsByTagName("ShippedTime")[0].InnerText;
                            shipped = "Yes";
                        }
                        catch
                        {
                            shipped = "No";
                        }
                    }
                    catch
                    {
                        shippingPrice = "N/A";
                        shippingService = "N/A";
                        trackingNumber = "N/A";
                        shipped = "No";
                    }
                    string datePaid;
                    string finalDate2;
                    try
                    {
                        datePaid = ele.GetElementsByTagName("PaidTime")[0].InnerText;
                        string[] components2 = datePaid.Split('T');
                        string[] date2 = components2[0].Split('-');
                        string[] time2 = components2[1].Split(':');
                        time2[2] = time2[2].Substring(0, time2[2].IndexOf('.'));
                        finalDate2 = date2[1] + "/" + date2[2] + "/" + date2[0];
                    }
                    catch
                    {
                        finalDate2 = "Unavailable";
                    }

                    info += name;
                    info += "," + ele.GetElementsByTagName("Subtotal")[0].InnerText;
                    info += "," + finalDate;
                    info += "," + ele.GetElementsByTagName("SellerUserID")[0].InnerText;
                    info += "," + itemNumber;
                    info += "," + paid;
                    info += "," + finalDate2;
                    info += "," + shipped;
                    info += "," + shippingPrice;
                    info += "," + shippingService;
                    info += "," + trackingNumber + "\n";
                }
            }

            DateTime end = DateTime.Now;

            TimeSpan span = end - start;

            MessageBox.Show("Download completed in " + span.Seconds + " seconds");
            progressBar3.Maximum = 100;
            progressBar3.Value = 100;
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
