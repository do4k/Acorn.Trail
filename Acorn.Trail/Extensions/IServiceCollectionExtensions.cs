using System.Linq;
using Acorn.Trail.GFX;
using Microsoft.Extensions.DependencyInjection;

namespace Acorn.Trail.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAcornTrailServices(this IServiceCollection services)
    {
        services.AddSingleton<IGraphicsLoader, GraphicsLoader>();
        services.AddSingleton<IGraphicsManager, GraphicsManager>();
        services.AddSingleton<IPEFileCollection>(sp =>
        {
            var fileCollection = new PEFileCollection();
            fileCollection.PopulateCollectionWithStandardGFX();
            fileCollection.ToList().ForEach(p => p.Value.Initialize());
            return fileCollection;
        });
        services.AddSingleton<IGraphicsDeviceManagerProvider>(new GraphicsDeviceManagerProvider(null, null));
        return services;
    }
}