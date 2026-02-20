
// If you want to customize the runner, you can define your own Main.
// However, for this demo we let the SDK generate the entry point to show the "Zero Config" approach first.
// 
// MTP Pillar: Hostable & Compile-time registration
// To customize:
// 1. Set <TestingPlatformDotnetTestSupport>false</TestingPlatformDotnetTestSupport> in csproj if needed
// 2. Use TestingPlatformBuilder to register extensions explicitly.

/*
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = await TestingPlatformApplication.CreateBuilderAsync(args);
        
        // Register custom extensions here
        // builder.AddExtension(new MyCustomExtension());
        
        return await builder.BuildAsync().RunAsync();
    }
}
*/
