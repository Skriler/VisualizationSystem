using VisualizationSystem.Models.DTOs;

namespace VisualizationSystem.Services.Utilities.Helpers.Colors;

public class ColorHelper
{
    private const int MinLightColorValue = 128;
    private const int MaxColorValue = 255;
    private const double DefaultSaturation = 1.0;

    public Color CalculateEdgeColor(float similarityPercentage, float minSimilarityThreshold)
    {
        var normalizedSimilarity = (similarityPercentage - minSimilarityThreshold) / (100 - minSimilarityThreshold);

        return CalculateColorFromSimilarity(normalizedSimilarity);
    }

    public Color GetNodeColorBasedOnSimilarityNormalized(NodeSimilarityResult similarityResult, float minSimilarity, float maxSimilarity)
    {
        var avgSimilarity = similarityResult.SimilarNodes.Average(s => s.SimilarityPercentage);

        var normalizedSimilarity = (avgSimilarity - minSimilarity) / (maxSimilarity - minSimilarity);
        var adjustedSimilarity = Math.Pow(normalizedSimilarity, 2);

        return CalculateColorFromSimilarity(adjustedSimilarity);
    }

    public Color GetNodeColorBasedOnSimilarityWithSigmoid(NodeSimilarityResult similarityResult, float minSimilarity, float maxSimilarity)
    {
        var avgSimilarity = similarityResult.SimilarNodes.Average(s => s.SimilarityPercentage);

        var normalizedSimilarity = (avgSimilarity - minSimilarity) / (maxSimilarity - minSimilarity);
        var adjustedSimilarity = Sigmoid(normalizedSimilarity, 10);

        return CalculateColorFromSimilarity(adjustedSimilarity);
    }

    public Color GetNodeColorFromDensity(NodeSimilarityResult similarityResult, int totalResults, float minSimilarityPercentage)
    {
        var similarNodes = similarityResult.SimilarNodes
            .Where(s => s.SimilarityPercentage >= minSimilarityPercentage)
            .ToList();

        var density = similarNodes.Count / (double)totalResults;
        var adjustedDensity = Math.Pow(density, 2);

        return CalculateColorFromDensity(adjustedDensity);
    }

    public Color GetNodeColorFromDensityWithSigmoid(NodeSimilarityResult similarityResult, int totalResults, float minSimilarityPercentage)
    {
        var similarNodes = similarityResult.SimilarNodes
            .Where(s => s.SimilarityPercentage >= minSimilarityPercentage)
            .ToList();

        var density = similarNodes.Count / (double)totalResults;
        var adjustedDensity = Sigmoid(density, 10);

        return CalculateColorFromDensity(adjustedDensity);
    }

    public Dictionary<int, Color> GetDistinctColors(List<int> ids, ColorBrightness brightness = ColorBrightness.Light)
    {
        var colors = new Dictionary<int, Color>();

        if (ids.Count <= 0)
            return colors;

        var (min, max) = brightness switch
        {
            ColorBrightness.Dark => (0, MinLightColorValue),
            ColorBrightness.Light => (MinLightColorValue, MaxColorValue),
            ColorBrightness.All => (0, MaxColorValue),
            _ => throw new ArgumentException("Unsupported brightness value")
        };

        for (int i = 0; i < ids.Count; i++)
        {
            var hue = (double)i / ids.Count;
            var value = (double)(max - min) / MaxColorValue;

            var color = ColorFromHSV(hue, DefaultSaturation, value);
            color = AdjustColorToBrightnessRange(color, min, max);

            colors.Add(ids[i], color);
        }

        return colors;
    }

    private static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue * 6)) % 6;
        double f = hue * 6 - Math.Floor(hue * 6);
        value *= 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        return hi switch
        {
            0 => Color.FromArgb(v, t, p),
            1 => Color.FromArgb(q, v, p),
            2 => Color.FromArgb(p, v, t),
            3 => Color.FromArgb(p, q, v),
            4 => Color.FromArgb(t, p, v),
            _ => Color.FromArgb(v, p, q)
        };
    }

    private static Color AdjustColorToBrightnessRange(Color color, int minValue, int maxValue)
    {
        float scale = (maxValue - minValue) / 255f;
        return Color.FromArgb(
            (int)(minValue + color.R * scale),
            (int)(minValue + color.G * scale),
            (int)(minValue + color.B * scale)
        );
    }

    private static double Sigmoid(double x, double k)
    {
        return 1 / (1 + Math.Exp(-k * (x - 0.5)));
    }

    private static Color CalculateColorFromSimilarity(double similarity)
    {
        // Interpolation between red (when the closest is 0) and green (when the closest is 100)
        var red = (byte)(255 * (1 - similarity)); // Red decreases from 255 to 0
        var green = (byte)(255 * similarity);     // Green increases from 0 to 255
        const byte blue = 0;                                // Blue stays at 0

        return Color.FromArgb(red, green, blue);
    }

    private static Color CalculateColorFromDensity(double density, byte minBrightness = 128)
    {
        var brightness = (byte)(255 * (1 - density));

        brightness = brightness < minBrightness ? minBrightness : brightness;

        return Color.FromArgb(brightness, brightness, 255);
    }
}
