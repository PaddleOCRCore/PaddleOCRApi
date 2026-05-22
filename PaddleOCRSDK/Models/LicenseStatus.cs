using System.Collections.Generic;
using Newtonsoft.Json;

namespace PaddleOCRSDK.Models
{
    /// <summary>
    /// 授权状态信息
    /// </summary>
    public class LicenseStatus
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        /// <summary>
        /// 是否激活授权
        /// </summary>
        [JsonProperty("activated")]
        public bool Activated { get; set; }

        /// <summary>
        /// 授权状态
        /// </summary>
        [JsonProperty("license_state")]
        public string LicenseState { get; set; }

        /// <summary>
        /// 客户信息
        /// </summary>
        [JsonProperty("customer")]
        public string Customer { get; set; }

        /// <summary>
        /// 授权编号
        /// </summary>
        [JsonProperty("license_id")]
        public string LicenseId { get; set; }

        /// <summary>
        /// 授权版本
        /// </summary>
        [JsonProperty("product_version")]
        public string ProductVersion { get; set; }

        /// <summary>
        /// 授权文件包含的产品列表，例如 PaddleOCR、PaddleOCR-VL。
        /// </summary>
        [JsonProperty("products")]
        public List<string> Products { get; set; }

        /// <summary>
        /// 授权平台
        /// </summary>
        [JsonProperty("platforms")]
        public List<string> Platforms { get; set; }

        /// <summary>
        /// 是否允许使用GPU
        /// </summary>
        [JsonProperty("allow_gpu")]
        public bool AllowGpu { get; set; }

        /// <summary>
        /// 是否绑定设备
        /// </summary>
        [JsonProperty("machine_bound")]
        public bool MachineBound { get; set; }

        /// <summary>
        /// 授权机器码
        /// </summary>
        [JsonProperty("lic_machine_code")]
        public string MachineCode { get; set; }

        /// <summary>
        /// 当前机器码
        /// </summary>
        [JsonProperty("current_machine_code")]
        public string CurrentMachineCode { get; set; }

        /// <summary>
        /// 是否匹配
        /// </summary>
        [JsonProperty("machine_match")]
        public bool MachineMatch { get; set; }

        /// <summary>
        /// 绑定模式，例如 machine 表示绑定设备，none 表示不绑定设备。
        /// </summary>
        [JsonProperty("bind_mode")]
        public string BindMode { get; set; }

        /// <summary>
        /// 授权开始时间，UTC ISO 8601 格式。
        /// </summary>
        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 授权到期时间，UTC ISO 8601 格式。
        /// </summary>
        [JsonProperty("expire_time")]
        public string ExpireTime { get; set; }
    }
}
