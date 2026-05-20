using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaddleOCRSDK.Models
{
    public class LicenseStatus
    {
        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("activated")]
        public bool Activated { get; set; }

        [JsonProperty("license_state")]
        public string LicenseState { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("license_id")]
        public string LicenseId { get; set; }

        [JsonProperty("library_version")]
        public string LibraryVersion { get; set; }

        [JsonProperty("platforms")]
        public List<string> Platforms { get; set; }

        [JsonProperty("allow_gpu")]
        public bool AllowGpu { get; set; }

        [JsonProperty("machine_bound")]
        public bool MachineBound { get; set; }

        [JsonProperty("bind_mode")]
        public string BindMode { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("expire_time")]
        public string ExpireTime { get; set; }

        [JsonProperty("mac_address")]
        public string MacAddress { get; set; }

        [JsonProperty("mac_addresses")]
        public List<string> MacAddresses { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
