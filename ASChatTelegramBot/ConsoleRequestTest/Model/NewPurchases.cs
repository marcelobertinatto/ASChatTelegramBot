using System;
using Newtonsoft.Json;

namespace ConsoleRequestTest.Model
{
    public class Price
    {

        [JsonProperty("value")]
        public double value { get; set; }

        [JsonProperty("currency_code")]
        public string currency_code { get; set; }
    }

    public class NewPurchases
    {

        [JsonProperty("transaction")]
        public string transaction { get; set; }

        [JsonProperty("approved_date")]
        public long approved_date { get; set; }

        [JsonProperty("payment_engine")]
        public string payment_engine { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("price")]
        public Price price { get; set; }

        [JsonProperty("payment_type")]
        public string payment_type { get; set; }

        [JsonProperty("payment_method")]
        public string payment_method { get; set; }

        [JsonProperty("recurrency_number")]
        public int recurrency_number { get; set; }

        [JsonProperty("under_warranty")]
        public bool under_warranty { get; set; }

        [JsonProperty("purchase_subscription")]
        public bool purchase_subscription { get; set; }
    }


}
