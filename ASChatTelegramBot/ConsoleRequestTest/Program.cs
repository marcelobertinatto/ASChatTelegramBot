using ConsoleRequestTest.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ConsoleRequestTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            var authentication = new Authentication();
            RestClient client = null;
            IRestResponse response = null;
            var signatures = new Signatures();
            var newPurchases = new NewPurchases();
            string content = null;
            int value = 1;
            string client_id = "4f112197-67e9-4ed0-9e40-077944d4c64e";
            string basic = "NGYxMTIxOTctNjdlOS00ZWQwLTllNDAtMDc3OTQ0ZDRjNjRlOjc3MGNhNmM5LTRmOGUtNDMyMi05NjYyLTQxYmRkYWJmNDE3Mg==";
            string client_secret = "770ca6c9-4f8e-4322-9662-41bddabf4172";
            string authURL = "https://api-sec-vlc.hotmart.com/security/oauth/token?grant_type=client_credentials&client_id="+
                client_id+"&client_secret="+client_secret;
            


            authentication = Authentication(authURL, basic, client_id, client_secret);
            var request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "Bearer " + authentication.access_token + "");
            request.AddHeader("Content-Type", "application/json");

            switch (value)
            {
                case 1: //obter assinaturas
                        string assinaturas = "https://sandbox.hotmart.com/payments/api/v1/subscriptions?status=CANCELLED_BY_SELLER&status=ACTIVE";
                        signatures = GetResponseFromHotmart(client, request, assinaturas, authentication.access_token, new Signatures());
                        break;
                case 2: //obter compras assinantes
                        string subscriber_code = signatures.items[0].subscriber_code;
                        string comprasAssinantes = "https://sandbox.hotmart.com/payments/api/v1/subscriptions/"+subscriber_code+"/purchases";
                        newPurchases = GetResponseFromHotmart(client, request, comprasAssinantes, authentication.access_token, new NewPurchases());
                    break;
                default:
                    break;
            }
                        
            Console.WriteLine(content);
            Console.ReadLine();
        }

        public static T GetResponseFromHotmart<T>(RestClient client, RestRequest method , string url, string token, T model)
        {
            try
            {
                IRestResponse response = null;
                client = new RestClient(url);
                response = client.Execute(method);
                string content = response.Content;
                model = JsonConvert.DeserializeObject<T>(response.Content);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return model;
        }

        public static Authentication Authentication(string url, string basic,string client_id, string client_secret)
        {
            var authentication = new Authentication();
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Basic " + basic);
                request.AddHeader("Content-Type", "application/json");

                var response = client.Execute(request);
                authentication = JsonConvert.DeserializeObject<Authentication>(response.Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return authentication;
        }
    }
}
