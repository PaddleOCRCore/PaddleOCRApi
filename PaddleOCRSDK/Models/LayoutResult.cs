using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace PaddleOCRSDK
{
    /// <summary>
    /// 版面识别结果（对应新版 DLL 返回结构）
    /// </summary>
    public class LayoutDetectResult
    {
        [JsonProperty("input_path")]
        public string InputPath { get; set; }

        [JsonProperty("page_index")]
        public int? PageIndex { get; set; }

        [JsonProperty("model_settings")]
        public LayoutModelSettings ModelSettings { get; set; }

        [JsonProperty("parsing_res_list")]
        public List<LayoutBlockResult> ParsingResList { get; set; } = new List<LayoutBlockResult>();

        [JsonProperty("table_res_list")]
        public List<LayoutTableContent> TableResList { get; set; } = new List<LayoutTableContent>();

        [JsonProperty("doc_preprocessor_res")]
        public LayoutDocPreprocessorResult DocPreprocessorRes { get; set; }

        [JsonProperty("layout_det_res")]
        public LayoutDetectionResult LayoutDetRes { get; set; }

        [JsonProperty("overall_ocr_res")]
        public LayoutOverallOcrResult OverallOcrRes { get; set; }

        [JsonProperty("markdown")]
        public string Markdown { get; set; }

        [JsonProperty("vis_path")]
        public string VisPath { get; set; }
    }

    public class LayoutModelSettings
    {
        [JsonProperty("use_doc_preprocessor")]
        public bool? UseDocPreprocessor { get; set; }

        [JsonProperty("use_doc_orientation_classify")]
        public bool? UseDocOrientationClassify { get; set; }

        [JsonProperty("use_doc_unwarping")]
        public bool? UseDocUnwarping { get; set; }

        [JsonProperty("run_ocr_after_layout")]
        public bool? RunOcrAfterLayout { get; set; }

        [JsonProperty("use_textline_orientation")]
        public bool? UseTextlineOrientation { get; set; }

        [JsonProperty("output_markdown")]
        public bool? OutputMarkdown { get; set; }

        [JsonProperty("use_seal_recognition")]
        public bool? UseSealRecognition { get; set; }

        [JsonProperty("use_table_recognition")]
        public bool? UseTableRecognition { get; set; }

        [JsonProperty("use_formula_recognition")]
        public bool? UseFormulaRecognition { get; set; }
    }

    public class LayoutBlockResult
    {
        [JsonProperty("block_id")]
        public int? BlockId { get; set; }

        [JsonProperty("block_order")]
        public int? BlockOrder { get; set; }

        [JsonProperty("block_label")]
        public string BlockLabel { get; set; }

        [JsonProperty("block_bbox")]
        public List<double> BlockBbox { get; set; } = new List<double>();

        [JsonProperty("score")]
        public double? Score { get; set; }

        [JsonProperty("polygon_points")]
        public List<LayoutPoint> PolygonPoints { get; set; } = new List<LayoutPoint>();

        [JsonProperty("block_content")]
        public string BlockContent { get; set; }

        [JsonProperty("is_sub_block")]
        public bool? IsSubBlock { get; set; }

        [JsonProperty("parent_block_id")]
        public int? ParentBlockId { get; set; }
    }

    public class LayoutPoint
    {
        [JsonProperty("x")]
        public double? X { get; set; }

        [JsonProperty("y")]
        public double? Y { get; set; }
    }

    public class LayoutDocPreprocessorResult
    {
        [JsonProperty("input_path")]
        public string InputPath { get; set; }

        [JsonProperty("page_index")]
        public int? PageIndex { get; set; }

        [JsonProperty("model_settings")]
        public LayoutModelSettings ModelSettings { get; set; }

        [JsonProperty("angle")]
        public double? Angle { get; set; }
    }

    public class LayoutDetectionResult
    {
        [JsonProperty("input_path")]
        public string InputPath { get; set; }

        [JsonProperty("page_index")]
        public int? PageIndex { get; set; }

        [JsonProperty("boxes")]
        public List<LayoutDetectionBox> Boxes { get; set; } = new List<LayoutDetectionBox>();
    }

    public class LayoutDetectionBox
    {
        [JsonProperty("cls_id")]
        public int? ClsId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("score")]
        public double? Score { get; set; }

        [JsonProperty("coordinate")]
        public List<double> Coordinate { get; set; } = new List<double>();
    }

    public class LayoutOverallOcrResult
    {
        [JsonProperty("input_path")]
        public string InputPath { get; set; }

        [JsonProperty("page_index")]
        public int? PageIndex { get; set; }

        [JsonProperty("model_settings")]
        public LayoutModelSettings ModelSettings { get; set; }

        [JsonProperty("dt_polys")]
        public List<List<List<double>>> DtPolys { get; set; }

        [JsonProperty("text_det_params")]
        public LayoutTextDetParams TextDetParams { get; set; }

        [JsonProperty("text_type")]
        public string TextType { get; set; }

        [JsonProperty("text_rec_score_thresh")]
        public double? TextRecScoreThresh { get; set; }

        [JsonProperty("rec_texts")]
        public List<string> RecTexts { get; set; } = new List<string>();

        [JsonProperty("rec_scores")]
        public List<double> RecScores { get; set; } = new List<double>();

        [JsonProperty("rec_boxes")]
        public List<List<double>> RecBoxes { get; set; }
    }

    public class LayoutTextDetParams
    {
        [JsonProperty("limit_side_len")]
        public int? LimitSideLen { get; set; }

        [JsonProperty("limit_type")]
        public string LimitType { get; set; }

        [JsonProperty("thresh")]
        public double? Thresh { get; set; }

        [JsonProperty("max_side_limit")]
        public int? MaxSideLimit { get; set; }

        [JsonProperty("box_thresh")]
        public double? BoxThresh { get; set; }

        [JsonProperty("unclip_ratio")]
        public double? UnclipRatio { get; set; }
    }

    public class LayoutTableContent
    {

        [JsonProperty("pred_html")]
        public string PredHtml { get; set; }

        [JsonProperty("table_content")]
        public List<List<string>> TableContent { get; set; } = new List<List<string>>();

        [JsonProperty("row_count")]
        public int? RowCount { get; set; }

        [JsonProperty("col_count")]
        public int? ColCount { get; set; }
    }

    internal static class LayoutJsonHelper
    {
        public static LayoutDetectResult DeserializeLayoutResult(string json, JsonSerializerSettings settings)
        {
            string normalizedJson = RemoveTrailingCommas(json);
            return JsonConvert.DeserializeObject<LayoutDetectResult>(normalizedJson, settings);
        }

        private static string RemoveTrailingCommas(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return json;
            }

            StringBuilder builder = null;
            bool inString = false;
            bool escaped = false;

            for (int i = 0; i < json.Length; i++)
            {
                char current = json[i];

                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (current == '\\')
                    {
                        escaped = true;
                    }
                    else if (current == '"')
                    {
                        inString = false;
                    }

                    builder?.Append(current);
                    continue;
                }

                if (current == '"')
                {
                    inString = true;
                    builder?.Append(current);
                    continue;
                }

                if (current == ',' && IsTrailingComma(json, i + 1))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(json.Length);
                        builder.Append(json, 0, i);
                    }

                    continue;
                }

                builder?.Append(current);
            }

            return builder?.ToString() ?? json;
        }

        private static bool IsTrailingComma(string json, int startIndex)
        {
            for (int i = startIndex; i < json.Length; i++)
            {
                char current = json[i];
                if (char.IsWhiteSpace(current))
                {
                    continue;
                }

                return current == ']' || current == '}';
            }

            return false;
        }
    }
}
