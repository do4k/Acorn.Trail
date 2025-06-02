using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.HighPerformance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acorn.Trail.GFX;

public interface IGraphicsManager : IDisposable
{
    /// <summary>
    /// Returns a byte array of image data from a single image within an endless online *.egf file
    /// Image is specified by the library file (GFXTypes) and the resourceName (number)
    /// </summary>
    /// <param name="resourceVal">Name (number) of the image resource</param>
    /// <param name="file">File type to load from</param>
    /// <param name="transparent">Whether or not to make the background black color transparent</param>
    /// <param name="reloadFromFile">True to force reload the gfx from the gfx file, false to use the in-memory cache</param>
    /// <param name="fullTransparent">True to make the gfx "fully" transparent; that is, to hide all transparent colors (0x08 blacks and full black)</param>
    /// <returns>Texture2D containing the image from the *.egf file</returns>
    Texture2D TextureFromResource(GFXTypes file, int resourceVal, bool transparent = false, bool reloadFromFile = false, bool fullTransparent = false);
}

public sealed class GraphicsManager : IGraphicsManager
{
    private readonly ConcurrentDictionary<GFXTypes, ConcurrentDictionary<int, Texture2D>> _cache;

    private readonly IGraphicsLoader _gfxLoader;
    private readonly IGraphicsDeviceManagerProvider _graphicsDeviceManagerProvider;

    public GraphicsManager(IGraphicsLoader gfxLoader, IGraphicsDeviceManagerProvider graphicsDeviceManagerProvider)
    {
        _cache = new ConcurrentDictionary<GFXTypes, ConcurrentDictionary<int, Texture2D>>();
        _gfxLoader = gfxLoader;
        _graphicsDeviceManagerProvider = graphicsDeviceManagerProvider;
    }

    // todo: instead of having a bunch of bool params, maybe an enum param with [Flags] for the different options would be better
    public Texture2D TextureFromResource(GFXTypes file, int resourceVal, bool transparent = false, bool reloadFromFile = false, bool fullTransparent = false)
    {
        if (_cache.ContainsKey(file) && _cache[file].ContainsKey(resourceVal))
        {
            if (reloadFromFile)
            {
                _cache[file][resourceVal]?.Dispose();
                _cache[file].Remove(resourceVal, out _);
            }
            else
            {
                return _cache[file][resourceVal];
            }
        }

        var ret = LoadTexture(file, resourceVal, transparent, fullTransparent);
        if (_cache.ContainsKey(file) ||
            _cache.TryAdd(file, new ConcurrentDictionary<int, Texture2D>()))
        {
            _cache[file].TryAdd(resourceVal, ret);
        }

        return ret;
    }

    private Texture2D LoadTexture(GFXTypes file, int resourceVal, bool transparent, bool fullTransparent)
    {
        var rawData = _gfxLoader.LoadGFX(file, resourceVal);

        if (rawData.IsEmpty)
            return new Texture2D(_graphicsDeviceManagerProvider.GraphicsDeviceManager?.GraphicsDevice, 1, 1);

        Action<byte[]> processAction = null;

        if (transparent)
        {
            processAction = data => CrossPlatformMakeTransparent(data);

            if (fullTransparent)
            {
                processAction = data => CrossPlatformMakeTransparent(data, isHat: true);
            }
            else if (file == GFXTypes.FemaleHat || file == GFXTypes.MaleHat)
            {
                processAction = data => CrossPlatformMakeTransparent(data, checkClip: true, isHat: true);
            }
        }

        using var ms = rawData.AsStream();
        var ret = Texture2D.FromStream(_graphicsDeviceManagerProvider.GraphicsDeviceManager?.GraphicsDevice, ms, processAction);

        return ret;
    }

    private static unsafe void CrossPlatformMakeTransparent(byte[] data, bool isHat = false, bool checkClip = false)
    {
        var shouldClip = false;
        if (checkClip)
        {
            fixed (byte* ptr = data)
            {
                for (var i = 0; i < data.Length; i += 4)
                {
                    var addr = (uint*)(ptr + i);
                    if (*addr != 0xff000008)
                        continue;
                    
                    shouldClip = true;
                    break;
                }
            }
        }

        // for all gfx: 0,0,0 is transparent

        // for some hats: 8,0,0 and 0,0,0 are both transparent

        // for hats: R=8 G=0 B=0 is transparent
        // some default gfx use R=0 G=8 B=0 as black
        // 0,0,0 clips pixels below it if 8,0,0 is present on the frame

        var transparentColors = isHat
            ? shouldClip
                ? new Color[] { new Color(0xff000008) } // check clip: make ff000008 transparent only, use black for clipping if present
                : new Color[] { Color.Black, new Color(0xff000008) } // isHat: make both colors transparent
            : new Color[] { Color.Black }; // default: make only black transparent

        fixed (byte* ptr = data)
        {
            for (int i = 0; i < data.Length; i += 4)
            {
                uint* addr = (uint*)(ptr + i);
                if (transparentColors.Contains(new Color(*addr)))
                    *addr = 0;
            }
        }
    }

    public void Dispose()
    {
        foreach (var text in _cache.SelectMany(x => x.Value.Values))
            text.Dispose();

        _cache.Clear();
    }
}