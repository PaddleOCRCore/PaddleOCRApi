// Copyright (c) 2025 PaddleOCRCore All Rights Reserved.
// https://github.com/PaddleOCRCore/PaddleOCRApi.git
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Newtonsoft.Json;
namespace PaddleOCRSDK
{
    /// <summary>
    /// Json序列化工具
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return default(T);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
            return (T)JsonConvert.DeserializeObject(json, typeof(T), settings);
        }
        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SeObject(object obj)
        {
            if (obj == null) return null;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
