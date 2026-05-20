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

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("platforms")]
        public List<string> Platforms { get; set; }

        [JsonProperty("allow_gpu")]
        public bool AllowGpu { get; set; }

        [JsonProperty("machine_bound")]
        public bool MachineBound { get; set; }

        [JsonProperty("lic_machine_code")]
        public string MachineCode { get; set; }

        [JsonProperty("current_machine_code")]
        public string CurrentMachineCode { get; set; }

        [JsonProperty("machine_match")]
        public bool MachineMatch { get; set; }

        [JsonProperty("bind_mode")]
        public string BindMode { get; set; }

        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [JsonProperty("expire_time")]
        public string ExpireTime { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }
    }
}
