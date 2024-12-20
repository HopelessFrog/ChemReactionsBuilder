using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace ChemReactionsBuilder.Helpers;

public static class StrokeHelper
{
    private static readonly SKColor[] Colors =
    [
        SKColors.CornflowerBlue,
        SKColors.Aqua,
        SKColors.Chartreuse,
        SKColors.Crimson,
        SKColors.Indigo,
        SKColors.DarkSlateGray,
        SKColors.Thistle, 
        SKColors.SaddleBrown,
        SKColors.LightCoral,
        SKColors.OrangeRed,
        SKColors.MediumSpringGreen,
        SKColors.Orange,
        SKColors.SeaGreen, 
        SKColors.Brown,
        SKColors.MediumVioletRed,
        SKColors.Khaki,
    ];
    public static SolidColorPaint GetRandomDashStroke(int thickness)
    {
        var random = new Random();
        var dashes = random.Next(0, 20);
        if (dashes % 2 == 1) dashes++;
        var dashDots = new float[dashes];
        for (var i = 0; i < dashDots.Length; i++)
        {
            dashDots[i] = (float)random.NextDouble() * thickness * 10;
        }
        var effect = new DashEffect(dashDots);
        var colorIndex = random.Next(0, Colors.Length);
        return new SolidColorPaint()
        {
            PathEffect = effect,
            StrokeThickness = thickness,
            Color = Colors[colorIndex],
            StrokeCap = SKStrokeCap.Round,
        };
    }
}