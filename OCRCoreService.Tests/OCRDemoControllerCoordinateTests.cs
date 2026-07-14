using System.Reflection;
using OCRCoreService.Controllers;
using PaddleOCRSDK;
using SkiaSharp;
using Xunit;

namespace OCRCoreService.Tests;

public class OCRDemoControllerCoordinateTests
{
    [Fact]
    public void BuildOcrBoxes_PreservesPolygonAndBoundingRectangle()
    {
        OCRResult result = new()
        {
            WordsResult = new List<JsonResult>
            {
                new()
                {
                    Words = "rotated text",
                    Score = 0.96f,
                    Location = new List<OCRLocation>
                    {
                        new() { x = 10, y = 20 },
                        new() { x = 110, y = 30 },
                        new() { x = 106, y = 60 },
                        new() { x = 6, y = 50 }
                    }
                }
            }
        };

        List<OCRDemoBox> boxes = Invoke<List<OCRDemoBox>>("BuildOcrBoxes", result);

        OCRDemoBox box = Assert.Single(boxes);
        Assert.Equal("rotated text", box.Text);
        Assert.True(box.IsTextLine);
        Assert.Equal(6, box.X);
        Assert.Equal(20, box.Y);
        Assert.Equal(104, box.Width);
        Assert.Equal(40, box.Height);
        Assert.Collection(
            box.Points,
            point => AssertPoint(point, 10, 20),
            point => AssertPoint(point, 110, 30),
            point => AssertPoint(point, 106, 60),
            point => AssertPoint(point, 6, 50));
    }

    [Fact]
    public void BuildLayoutBoxes_PrefersOverallOcrLinesAndHandlesMismatchedArrays()
    {
        LayoutDetectResult result = new()
        {
            OverallOcrRes = new LayoutOverallOcrResult
            {
                RecTexts = new List<string> { "first line", "invalid line", "third line" },
                RecScores = new List<double> { 0.91 },
                DtPolys = new List<List<List<double>>>
                {
                    new()
                    {
                        new() { 12, 18 },
                        new() { 112, 22 },
                        new() { 110, 46 },
                        new() { 10, 42 }
                    }
                },
                RecBoxes = new List<List<double>>
                {
                    new() { 10, 18, 112, 46 },
                    new() { 20, 50, 20, 70 },
                    new() { 30, 80, 160, 108 }
                }
            },
            ParsingResList = new List<LayoutBlockResult>
            {
                new() { BlockContent = "fallback block", BlockBbox = new List<double> { 0, 0, 300, 200 } }
            }
        };

        List<OCRDemoBox> boxes = Invoke<List<OCRDemoBox>>("BuildLayoutBoxes", result);

        Assert.Equal(2, boxes.Count);
        Assert.Equal("first line", boxes[0].Text);
        Assert.True(boxes[0].IsTextLine);
        Assert.Equal(0.91, boxes[0].Score);
        Assert.Equal(4, boxes[0].Points.Count);
        Assert.Equal("third line", boxes[1].Text);
        Assert.Null(boxes[1].Score);
        Assert.Equal(4, boxes[1].Points.Count);
        Assert.DoesNotContain(boxes, box => box.Text == "fallback block");
    }

    [Fact]
    public void BuildLayoutBoxes_WhenLinesHaveNoCoordinates_UsesBlockPolygon()
    {
        LayoutDetectResult result = new()
        {
            OverallOcrRes = new LayoutOverallOcrResult
            {
                RecTexts = new List<string> { "line without coordinates" }
            },
            ParsingResList = new List<LayoutBlockResult>
            {
                new()
                {
                    BlockId = 7,
                    BlockOrder = 2,
                    BlockLabel = "text",
                    BlockContent = "fallback block",
                    PolygonPoints = new List<LayoutPoint>
                    {
                        new() { X = 5, Y = 10 },
                        new() { X = 205, Y = 15 },
                        new() { X = 200, Y = 95 },
                        new() { X = 0, Y = 90 }
                    }
                }
            }
        };

        OCRDemoBox box = Assert.Single(Invoke<List<OCRDemoBox>>("BuildLayoutBoxes", result));

        Assert.Equal("fallback block", box.Text);
        Assert.Equal("text", box.Label);
        Assert.False(box.IsTextLine);
        Assert.Equal(7, box.BlockId);
        Assert.Equal(4, box.Points.Count);
    }

    [Fact]
    public void GetImageSize_ReadsDimensionsFromOcrInputBytes()
    {
        using SKBitmap bitmap = new(37, 19);
        using SKCanvas canvas = new(bitmap);
        canvas.Clear(SKColors.White);
        using SKImage image = SKImage.FromBitmap(bitmap);
        using SKData encoded = image.Encode(SKEncodedImageFormat.Png, 100);

        (int width, int height) = Invoke<(int Width, int Height)>("GetImageSize", encoded.ToArray());

        Assert.Equal(37, width);
        Assert.Equal(19, height);
    }

    private static T Invoke<T>(string methodName, params object[] arguments)
    {
        MethodInfo method = typeof(OCRDemoController).GetMethod(
            methodName,
            BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Method '{methodName}' was not found.");
        return Assert.IsType<T>(method.Invoke(null, arguments));
    }

    private static void AssertPoint(OCRDemoPoint point, double x, double y)
    {
        Assert.Equal(x, point.X);
        Assert.Equal(y, point.Y);
    }
}