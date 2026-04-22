using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

        [JsonProperty("seal_res_list")]
        public List<LayoutSealContent> SealResList { get; set; } = new List<LayoutSealContent>();

        [JsonProperty("formula_res_list")]
        public List<LayoutFormulaContent> FormulaResList { get; set; } = new List<LayoutFormulaContent>();

        [JsonProperty("chart_res_list")]
        public List<LayoutChartContent> ChartResList { get; set; } = new List<LayoutChartContent>();

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

        [JsonProperty("use_chart_recognition")]
        public bool? UseChartRecognition { get; set; }

        [JsonProperty("text_rec_score_thresh")]
        public double? TextRecScoreThresh { get; set; }

        [JsonProperty("layout_threshold")]
        public double? LayoutThreshold { get; set; }

        [JsonProperty("layout_nms")]
        public bool? LayoutNms { get; set; }

        [JsonProperty("layout_unclip_ratio_w")]
        public double? LayoutUnclipRatioW { get; set; }

        [JsonProperty("layout_unclip_ratio_h")]
        public double? LayoutUnclipRatioH { get; set; }

        [JsonProperty("layout_shape_mode")]
        public string LayoutShapeMode { get; set; }

        [JsonProperty("format_block_content")]
        public bool? FormatBlockContent { get; set; }
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
        public JToken BlockContent { get; set; }

        [JsonProperty("is_sub_block")]
        public bool? IsSubBlock { get; set; }

        [JsonProperty("parent_block_id")]
        public int? ParentBlockId { get; set; }

        [JsonIgnore]
        public string TextContent { get; set; }

        [JsonIgnore]
        public LayoutTableContent TableContent { get; set; }

        [JsonIgnore]
        public LayoutFormulaContent FormulaContent { get; set; }

        [JsonIgnore]
        public LayoutSealContent SealContent { get; set; }

        [JsonIgnore]
        public LayoutChartContent ChartContent { get; set; }
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
        public JToken DtPolys { get; set; }

        [JsonProperty("text_det_params")]
        public LayoutTextDetParams TextDetParams { get; set; }

        [JsonProperty("text_type")]
        public string TextType { get; set; }

        [JsonProperty("textline_orientation_angles")]
        public List<double> TextlineOrientationAngles { get; set; } = new List<double>();

        [JsonProperty("text_rec_score_thresh")]
        public double? TextRecScoreThresh { get; set; }

        [JsonProperty("rec_texts")]
        public List<string> RecTexts { get; set; } = new List<string>();

        [JsonProperty("rec_scores")]
        public List<double> RecScores { get; set; } = new List<double>();

        [JsonProperty("scores")]
        private List<double> LegacyScores
        {
            set
            {
                if ((RecScores == null || RecScores.Count == 0) && value != null)
                {
                    RecScores = value;
                }
            }
        }

        [JsonProperty("rec_polys")]
        public JToken RecPolys { get; set; }

        [JsonProperty("rec_boxes")]
        public JToken RecBoxes { get; set; }
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
        [JsonProperty("cell_box_list")]
        public JToken CellBoxList { get; set; }

        [JsonProperty("pred_html")]
        public string PredHtml { get; set; }

        [JsonProperty("table_content")]
        public JToken TableContent { get; set; }

        [JsonProperty("row_count")]
        public int? RowCount { get; set; }

        [JsonProperty("col_count")]
        public int? ColCount { get; set; }

        [JsonProperty("table_ocr_pred")]
        public LayoutTableOcrPrediction TableOcrPred { get; set; }
    }

    public class LayoutTableOcrPrediction
    {
        [JsonProperty("rec_polys")]
        public JToken RecPolys { get; set; }

        [JsonProperty("rec_texts")]
        public List<string> RecTexts { get; set; } = new List<string>();

        [JsonProperty("rec_scores")]
        public List<double> RecScores { get; set; } = new List<double>();

        [JsonProperty("scores")]
        private List<double> LegacyScores
        {
            set
            {
                if ((RecScores == null || RecScores.Count == 0) && value != null)
                {
                    RecScores = value;
                }
            }
        }

        [JsonProperty("rec_boxes")]
        public JToken RecBoxes { get; set; }
    }

    public class LayoutFormulaContent
    {
        [JsonProperty("input_path")]
        public string InputPath { get; set; }

        [JsonProperty("page_index")]
        public int? PageIndex { get; set; }

        [JsonProperty("rec_formula")]
        public string RecFormula { get; set; }

        [JsonProperty("confidence")]
        public double? Confidence { get; set; }

        [JsonProperty("formula_type")]
        public string FormulaType { get; set; }

        [JsonProperty("formula_region_id")]
        public int? FormulaRegionId { get; set; }

        [JsonProperty("dt_polys")]
        public JToken DtPolys { get; set; }
    }

    public class LayoutSealContent
    {
        [JsonProperty("rec_texts")]
        public List<string> RecTexts { get; set; } = new List<string>();

        [JsonProperty("scores")]
        public List<double> Scores { get; set; } = new List<double>();

        [JsonProperty("seal_type")]
        public string SealType { get; set; }
    }

    public class LayoutChartContent
    {
        [JsonProperty("table_data")]
        public JToken TableData { get; set; }

        [JsonProperty("chart_type")]
        public string ChartType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
