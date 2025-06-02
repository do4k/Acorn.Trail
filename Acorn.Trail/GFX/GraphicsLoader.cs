using System;

namespace Acorn.Trail.GFX;

public interface IGraphicsLoader
{
    public ReadOnlyMemory<byte> LoadGFX(GFXTypes file, int resourceValue);
}

public class GraphicsLoader : IGraphicsLoader
{
    private readonly IPEFileCollection _modules;

    public GraphicsLoader(IPEFileCollection modules)
    {
        _modules = modules;
    }

    public ReadOnlyMemory<byte> LoadGFX(GFXTypes file, int resourceValue)
    {
        var fileBytes = ReadOnlyMemory<byte>.Empty;
        try
        {
            fileBytes = _modules[file].GetEmbeddedBitmapResourceByID(resourceValue + 100);
        }
        catch (ArgumentException)
        {
#if DEBUG
            throw;
#endif
        }

        if (fileBytes.Length == 0)
        {
#if DEBUG
            throw new GFXLoadException(resourceValue, file);
#else
                return Array.Empty<byte>();
#endif
        }

        return fileBytes;
    }
}