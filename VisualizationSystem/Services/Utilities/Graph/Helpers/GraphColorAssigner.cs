using VisualizationSystem.Models.DTOs;

namespace VisualizationSystem.Services.Utilities.Graph.Helpers;

public class GraphColorAssigner
{
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

    private double Sigmoid(double x, double k)
    {
        return 1 / (1 + Math.Exp(-k * (x - 0.5)));
    }

    private Color CalculateColorFromSimilarity(double similarity)
    {
        // Interpolation between red (when the closest is 0) and green (when the closest is 100)
        var red = (byte)(255 * (1 - similarity)); // Red decreases from 255 to 0
        var green = (byte)(255 * similarity);     // Green increases from 0 to 255
        const byte blue = 0;                                // Blue stays at 0

        return Color.FromArgb(red, green, blue);
    }

    private Color CalculateColorFromDensity(double density, byte minBrightness = 128)
    {
        var brightness = (byte)(255 * (1 - density));

        brightness = brightness < minBrightness ? minBrightness : brightness;

        return Color.FromArgb(brightness, brightness, 255);
    }
}
