using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PELoaderLib;

namespace Acorn.Trail.GFX;

public interface IPEFileCollection : IDictionary<GFXTypes, IPEFile>, IDisposable
{
    void PopulateCollectionWithStandardGFX();
}

public sealed class PEFileCollection : Dictionary<GFXTypes, IPEFile>, IPEFileCollection
{
    public void PopulateCollectionWithStandardGFX()
    {
        var gfxTypes = (GFXTypes[])Enum.GetValues(typeof(GFXTypes));
        foreach (var type in gfxTypes)
            Add(type, CreateGFXFile(type));
    }

    private IPEFile CreateGFXFile(GFXTypes file)
    {
        var fName = string.Format(Constants.GFXFormat, (int)file);
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new PEFile(fName);

        return new PEFile(fName, shared: true);
    }

    public void Dispose()
    {
        foreach (var pair in this)
            pair.Value.Dispose();
    }
}