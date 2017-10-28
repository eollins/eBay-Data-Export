using eBay.Service.Core.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eBay_Data_Export
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.sandbox.ebay.com/ws/api.dll/");
            request.Method = "POST";
            request.Headers.Add("X-EBAY-API-DEV-NAME", "GregoryM-mailer-SBX-b45f0c763-f90c92ad");
            request.Headers.Add("X-EBAY-API-APP-NAME", "8105fd0e-a76c-4e10-80e8-43e86ab59f7c");
            request.Headers.Add("X-EBAY-API-CERT-NAME", "SBX-45f0c76378c2-8dd7-40e6-b49d-0c12");
            request.Headers.Add("X-EBAY-API-COMPATIBILITY-LEVEL", "967");
            request.Headers.Add("X-EBAY-API-CALL-NAME", "GetSessionID");
            request.Headers.Add("X-EBAY-API-SITEID", "0");
            //request.Headers.Add("X-EBAY-API-IAF-TOKEN", "v^1.1#i^1#f^0#I^3#r^0#p^3#t^H4sIAAAAAAAAAOVXW2wUVRhmezMVWh5shIjGdQqGQmb3zM7s7MyE3bptQZb0yhaECilzOVMGZmfWOWfaLhfTNNI0CEJJTDRRqIRIAhjjiyJgQoiIGh94MF5CxASIMQ2iAqYPinpme2FbI/RCYhPnZXLO+W/f/3//uYCuouIlPSt7Bkt8D+X1d4GuPJ+PmQ2KiwqXlubnPVY4C+QI+Pq7FnYVdOf/uAzJKTMtrYYobVsI+jtTpoWk7GSUch1LsmVkIMmSUxBJWJWS8bpaKRQAUtqxsa3aJuVP1EQpOaxDTYOawPAsz4MQmbVGbDbbUUpjQgoI8wJktbAI1TBZR8iFCQth2cJRKgSYCM0AOiQ0g7DECRInBsJspIXyr4UOMmyLiAQAFcuGK2V1nZxY7x2qjBB0MDFCxRLxFcmGeKJmeX3zsmCOrdhwHpJYxi4aO6q2NehfK5suvLcblJWWkq6qQoSoYGzIw1ijUnwkmCmEn001qwiaIoZ1QRD4CKPwDySVK2wnJeN7x+HNGBqtZ0UlaGEDZ+6XUZINZQtU8fConphI1Pi9X5Mrm4ZuQCdKLa+Kr1+TXL6a8icbGx273SBE8pAyLMuHOI4LUTEMEUkhdFqR7KpQl1VsO5lhf0NGh7M9zmG1bWmGlzvkr7dxFSTBw/EpYnNSRIQarAYnrmMvsFy5yGgqmRavtkPFdPFmyysvTJF8+LPD+xdihBl3ufCguKGIOuBZgbRhiAHhXG54vT51fsS8EsUbG4NeLFCRM3RKdrZCnDZlFdIqSa+bgo6hSWxYD7GCDmmNF3WaE3WdVsIaTzM6hABCRVFF4X9IE4wdQ3ExHKXK+IUs1iiVVO00bLRNQ81Q40WyO9AwMTpRlNqMcVoKBjs6OgIdbMB22oIhAJjgurrapLoZpmRqVNa4vzBtZFmrQqKFDAln0iSaTsJA4txqo2KsozXKDs5UuRkyTkLTJL8RFo+JMDZ+9l+gIg/qzALp6SNiQE4bAY/kAdVOBW2ZNLU31ZqN2D8RoaDiZoh/DToBB8qabZmZieu1uYTEQ9oTU0KkGoGhfiQwiEev1yfjdayBSegYVjvhMmmxScIcqzwJHVlVbdfCU3E3rDoJDd01dcM0vXadisMc9cmEaclmBhsqGnU5rS6Lp9MJbWZ12bMObCNVryNHiGFCh05WraMVLqwDNcKztC4CVQzJ2rRQa7DdUGGrMcOQW65pTgtXDWwfX03S60f/a1wCuWvoGoC0HOFVmoPkKBQAFGiOhQIvK2FRj6jTwl3XNtNKWR+MTwtRtWmQfaE5M9OOwJU2wnB6zVdNLqQzC5S3w4xsMBFBDdGCpkVoDkCebDyiRgOVCU0U8riJnAvdP670wbFP69is7Md0+z4C3b4PyescRADNLAUVRflrCvLnUMjAMIBkS1PszoAh6wFktFnk5ejAwFaYScuGk1fke37BQOUfOY/6/o1g/uizvjifmZ3zxgeP310pZObOK2EiDAgJIMwJnNgCyu+uFjCPFpTt3XC89Ok+tvXIzwLuqvhl8ZyFvjAoGRXy+QpnFXT7Zp05+P7HgyVfPncovu+zM1LRoj+bt5X/2iEOtMz3XS3df+XavhN3bmQWzKG2N5Vf3H2nqvn3ge+eeaftyskVl8/e6t15qaK074Puha/1Xbt5bOcXB07eXsKVHE7c3lH4w9HjPa9s/PQT7s23XvjKv+2b3t1PbShrOnt+zxKudc+FHfuv/vXG+VP8pejgb6veTpw7KO4Vzn0vLdhlnN50g08c8TWdOPDI58Krx9kXj/b3GvPjqQsdFwcHXjp1M3B91eH1p/e+i/qaXr9c7lyfu2ht5ROHv9205eAVn3KmZ9fDePGTUt+q/O2VDT/1i9GXv9ZDvcdurauodHfUNpYNlEnce7Ud57bwh/w3T8wrtk6eHirj3wO7mn9uEQAA");
            request.ContentType = "text/xml";

            string postData = "<?xml version=\"1.0\" encoding=\"utf-8\"?><GetSessionIDRequest xmlns=\"urn:ebay:apis:eBLBaseComponents\"><ErrorLanguage>en_US</ErrorLanguage><WarningLevel>High</WarningLevel><RuName>Gregory_Morris-GregoryM-mailer-fkfdcrz</RuName></GetSessionIDRequest>";
        }
    }
}
