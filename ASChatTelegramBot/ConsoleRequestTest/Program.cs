using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ConsoleRequestTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            string token = "H4sIAAAAAAAAAB2OyZKCMABEv8ipABLxiKCYCGHfcrEgoCxRURCErx9nDn3p5VWXM65yg9V2jf1wQQKpUY%2Funsw0BFHbJZGGtz%2FljJdU3E7URzCe8UzjQ4vqqS4k3BVGWCf%2BVNOkmlDz%2BJBm%2F9VVshr3Y2q4KhLvkf%2F3OGfz%2Fx4FgkfcCIeJQHZB62mBcP1nZHev%2BzJq65YuNLBksrDBCnBNfACo4cpm0C5ETAdi7Ndf%2F9tiotWQ6u9LKn46GssgEXn7xyBG%2BrEC9LEDNpCm4PYfI3YnM%2BCc6Opg6UyyAxdQ3WtI43F0Bz%2BR5kJP36NBSLO3DzvJj%2B%2BoviaV71ADHsNx6%2FTKwViy2yqFGNHTW1j3Yns4jv0tYmsdOS%2FOABhcYyW46vKS7VmhTy5Fql5eAM1H7flQUj2LHlny1EjfdYdai%2FB0Ia9xzYu88l97GbLPe50HQIW8bcgmrxPcRDvVN7SVeeKGxMTTORKdnXgC4GKzVR6%2BHfSNuy6WpbOjJnE5wlK5p7tjObGg6YdNqRSUnYdNeEMbmB2KwTPteVb2Qg%2FJHV4MzfSm7UjUpbyk9mdyGg%2Fx9niedaOrGc%2BCG5QLdH4uviZrvRgaEmmpuqOm4Asi1Z1K30KCb0oIkocDHrp0fYo4nAAXE%2Bs6qr%2BCWNTyYwIAAA%3D%3D";
            var client = new RestClient("https://sandbox.hotmart.com/payments/api/v1/subscriptions?status=CANCELLED_BY_SELLER&status=ACTIVE");
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "Bearer " + token + "");
            request.AddHeader("Content-Type", "application/json");            

            var response = client.Execute(request);
            var content = response.Content;
            var signatures = JsonConvert.DeserializeObject<Signatures>(response.Content);
            Console.WriteLine(content);
            Console.ReadLine();
        }
    }
}
