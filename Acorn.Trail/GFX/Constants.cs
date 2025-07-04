using Microsoft.Xna.Framework;

namespace Acorn.Trail.GFX;

public static class Constants
{
    public static string GFXFormat { get; } = "egf/gfx{0,3:D3}.egf";
    
    public static class Colors
    {
        public static readonly Color LightGrayText = Color.FromNonPremultiplied(0xc8, 0xc8, 0xc8, 0xff);
        public static readonly Color LightYellowText = Color.FromNonPremultiplied(0xf0, 0xf0, 0xc8, 0xff);
        public static readonly Color BeigeText = Color.FromNonPremultiplied(0xb4, 0xa0, 0x8c, 0xff);
        public static readonly Color LightBeigeText = Color.FromNonPremultiplied(0xdc, 0xc8, 0xb4, 0xff);
        public static readonly Color LightGrayDialogMessage = Color.FromNonPremultiplied(0xe6, 0xe6, 0xd6, 0xff);
        public static readonly Color MediumGrayText = Color.FromNonPremultiplied(0xb9, 0xb9, 0xb9, 0xff);
    }
}