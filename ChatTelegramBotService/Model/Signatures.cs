using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChatTelegramBotService.Model
{    
    public class Plan
    {
        [JsonProperty("name")]
        public string name { get; set; }
    }

    public class Product
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("ucode")]
        public string ucode { get; set; }
    }

    public class Price
    {
        [JsonProperty("value")]
        public double value { get; set; }

        [JsonProperty("currency_code")]
        public string currency_code { get; set; }
    }

    public class Subscriber
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("ucode")]
        public string ucode { get; set; }
    }

    public class Item
    {
        [JsonProperty("subscriber_code")]
        public string subscriber_code { get; set; }

        [JsonProperty("subscription_id")]
        public int subscription_id { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("accession_date")]
        public long accession_date { get; set; }

        [JsonProperty("request_date")]
        public long request_date { get; set; }

        [JsonProperty("trial")]
        public bool trial { get; set; }

        [JsonProperty("plan")]
        public Plan plan { get; set; }

        [JsonProperty("product")]
        public Product product { get; set; }

        [JsonProperty("price")]
        public Price price { get; set; }

        [JsonProperty("subscriber")]
        public Subscriber subscriber { get; set; }
    }

    public class PageInfo
    {
        [JsonProperty("total_results")]
        public int total_results { get; set; }

        [JsonProperty("next_page_token")]
        public string next_page_token { get; set; }

        [JsonProperty("prev_page_token")]
        public string prev_page_token { get; set; }

        [JsonProperty("results_per_page")]
        public int results_per_page { get; set; }
    }

    public class Signatures
    {
        [JsonProperty("items")]
        public IList<Item> items { get; set; }

        [JsonProperty("page_info")]
        public PageInfo page_info { get; set; }
    }    
}
