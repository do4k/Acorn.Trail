using System;

namespace Acorn.Trail.GFX;

public class GFXLoadException : Exception
{
    public GFXLoadException(int resource, GFXTypes gfx)
        : base($"Unable to load graphic {resource + 100} from file gfx{(int)gfx:000}.egf") { }
}