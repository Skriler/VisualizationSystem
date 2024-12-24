using DotNetGraph.Compilation;
using DotNetGraph.Core;

namespace VisualizationSystem.Services.Utilities.FileSystem;

public class FileWriter
{
    public async Task WriteGraphAsync(DotGraph graph, string fileName)
    {
        await using var writer = new StringWriter();
        var context = new CompilationContext(writer, new CompilationOptions());

        await graph.CompileAsync(context);
        var result = writer.GetStringBuilder().ToString();

        await File.WriteAllTextAsync(fileName, result);
    }
}
