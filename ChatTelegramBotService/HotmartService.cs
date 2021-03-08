using System;
using System.Collections.Generic;
using ChatTelegramBotService.Model;
using Newtonsoft.Json;
using RestSharp;

namespace ChatTelegramBotService
{
    public class HotmartService
    {
        public HotmartService()
        {

        }

        public Authentication Authentication(Credentials cred)
        {
            var authentication = new Authentication();
            RestClient client = null;
            
            try
            {
                client = new RestClient(cred.authURL);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Basic " + cred.basic);
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

        public Signatures GetSignatures(Credentials cred,Authentication auth)
        {
            RestClient client = null;
            var signatures = new Signatures();            

            try
            {
                client = new RestClient(cred.signaturesUrl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("authorization", "Bearer " + auth.access_token + "");
                request.AddHeader("Content-Type", "application/json");

                signatures = GetResponseFromHotmart(client, request, cred.signaturesUrl, auth.access_token, new Signatures());

                return signatures;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Signatures GetSignaturesByEmail(Credentials cred, Authentication auth, string email)
        {
            RestClient client = null;
            var signatures = new Signatures();
            var returnedSignature = new Signatures();
            returnedSignature.items = new List<Item>();

            try
            {
                client = new RestClient(cred.signaturesUrl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("authorization", "Bearer " + auth.access_token + "");
                request.AddHeader("Content-Type", "application/json");

                signatures = GetResponseFromHotmart(client, request, cred.signaturesUrl, auth.access_token, new Signatures());

                if (signatures.items != null)
                {
                    for (int i = 0; i < signatures.items.Count; i++)
                    {
                        if (signatures.items[i].subscriber.email.ToUpper().Equals(email.ToUpper()))
                        {
                            returnedSignature.items.Add(signatures.items[i]);
                        }
                    }
                }

                return returnedSignature;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public NewPurchases GetNewPurchases(Credentials cred, Authentication auth, string subscriber_code)
        {
            RestClient client = null;
            var newPurchases = new NewPurchases();

            try
            {
                client = new RestClient(cred.newPurchasesUrl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("authorization", "Bearer " + auth.access_token + "");
                request.AddHeader("Content-Type", "application/json");

                newPurchases = GetResponseFromHotmart(client, request, cred.newPurchasesUrl.Replace("{subscriber_code}", subscriber_code)
                    , auth.access_token, new NewPurchases());

                return newPurchases;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Credentials GetCredentials(bool isTest)
        {
            var cred = new Credentials();
            if (isTest)
            {
                cred.client_id = "4f112197-67e9-4ed0-9e40-077944d4c64e";
                cred.basic = "NGYxMTIxOTctNjdlOS00ZWQwLTllNDAtMDc3OTQ0ZDRjNjRlOjc3MGNhNmM5LTRmOGUtNDMyMi05NjYyLTQxYmRkYWJmNDE3Mg==";
                cred.client_secret = "770ca6c9-4f8e-4322-9662-41bddabf4172";
                cred.authURL = "https://api-sec-vlc.hotmart.com/security/oauth/token?grant_type=client_credentials&client_id=" +
                    cred.client_id + "&client_secret=" + cred.client_secret;
                cred.signaturesUrl = "https://sandbox.hotmart.com/payments/api/v1/subscriptions?status=CANCELLED_BY_SELLER&status=ACTIVE";
                cred.newPurchasesUrl = "https://sandbox.hotmart.com/payments/api/v1/subscriptions/{subscriber_code}/purchases";
            }
            else
            {
                cred.client_id = "411d55ef-e113-4437-94c0-d0a281a669fc";
                cred.client_secret = "14bf222b-9b58-453f-b494-3aac6dc1ce27";
                cred.basic = "NDExZDU1ZWYtZTExMy00NDM3LTk0YzAtZDBhMjgxYTY2OWZjOjE0YmYyMjJiLTliNTgtNDUzZi1iNDk0LTNhYWM2ZGMxY2UyNw==";
                cred.authURL = "https://api-sec-vlc.hotmart.com/security/oauth/token?grant_type=client_credentials&client_id=" +
                    cred.client_id + "&client_secret=" + cred.client_secret;
                cred.signaturesUrl = "https://developers.hotmart.com/payments/api/v1/subscriptions?status=ACTIVE";
                cred.newPurchasesUrl = "https://developers.hotmart.com/payments/api/v1/subscriptions/{subscriber_code}/purchases";
            }

            return cred;
        }

        public T GetResponseFromHotmart<T>(RestClient client, RestRequest method, string url, string token, T model)
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
    }
}
