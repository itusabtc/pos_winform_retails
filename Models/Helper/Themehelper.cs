using System;
using System.Drawing;

namespace NailsChekin.Models.Helper
{
    public struct ThemePalette
    {
        public Color Base, Hover, Press, Border, SelBorder;
        public Color Title, Subtitle, Description;
    }

    // Các override tùy chọn (nếu để null -> dùng mặc định/tự tính)
    public struct ThemeOverrides
    {
        public Color? Base, Hover, Press, Border, SelBorder;
        public Color? Title, Subtitle, Description;
        public static readonly ThemeOverrides Empty = default(ThemeOverrides);
    }

    public static class ThemeHelper
    {
        // ====== Defaults (có thể cấu hình lại app-wide) ======
        public static bool DefaultAutoTextContrast { get; set; } = true;
        public static ThemePalette DefaultPalette { get; private set; }
            = CreatePaletteFromBaseInternal(Color.White, true);

        /// <summary>Đặt default nhanh: truyền mỗi BaseColor (hoặc thêm overrides nếu muốn)</summary>
        public static void SetDefaultPalette(Color baseColor,
                                             bool? autoTextContrast = null,
                                             ThemeOverrides overrides = default(ThemeOverrides))
        {
            var p = CreatePaletteFromBaseInternal(baseColor, autoTextContrast ?? DefaultAutoTextContrast);
            ApplyOverrides(ref p, overrides);
            DefaultPalette = p;
        }

        // ====== Mapping theo status (tuỳ biến nhẹ ở đây) ======
        public static ThemePalette MapStatus(string status,
                                             bool? autoTextContrast = null,
                                             ThemeOverrides overrides = default(ThemeOverrides))
        {
            string key = (status ?? "").Trim().ToUpperInvariant();
            Color? baseColor = null;

            if (key.Contains("CLOCK IN") || key == "IN")
                baseColor = Color.FromArgb(0x2E, 0x7D, 0x32);     // xanh lá
            else if (key.Contains("CLOCK OUT") || key == "OUT")
                baseColor = Color.FromArgb(0x90, 0xA4, 0xAE);     // xám
            else if (key.Contains("BREAK"))
                baseColor = Color.FromArgb(0xF57C00);             // cam
            else if (key.Contains("PROCESS") || key.Contains("ĐANG"))
                baseColor = Color.FromArgb(0x19, 0x76, 0xD2);     // xanh dương

            if (baseColor == null)
            {
                // Không match → dùng DefaultPalette + overrides (nếu có)
                var p = DefaultPalette;
                ApplyOverrides(ref p, overrides);
                return p;
            }

            var palette = CreatePaletteFromBaseInternal(baseColor.Value, autoTextContrast ?? DefaultAutoTextContrast);
            ApplyOverrides(ref palette, overrides);
            return palette;
        }

        /// <summary>
        /// Tạo palette từ BaseColor. Nếu baseColor = null → dùng DefaultPalette.
        /// Bạn có thể truyền overrides từng màu (chỉ mục nào != null sẽ được áp).
        /// </summary>
        public static ThemePalette CreatePaletteFromBase(Color? baseColor = null,
                                                         bool? autoTextContrast = null,
                                                         ThemeOverrides overrides = default(ThemeOverrides))
        {
            if (baseColor == null)
            {
                var p = DefaultPalette;
                ApplyOverrides(ref p, overrides);
                return p;
            }

            var pp = CreatePaletteFromBaseInternal(baseColor.Value, autoTextContrast ?? DefaultAutoTextContrast);
            ApplyOverrides(ref pp, overrides);
            return pp;
        }

        // ====== Internals ======
        private static ThemePalette CreatePaletteFromBaseInternal(Color baseColor, bool autoTextContrast)
        {
            var p = new ThemePalette
            {
                Base = baseColor,
                Hover = Blend(baseColor, Color.White, 0.10),
                Press = Blend(baseColor, Color.Black, 0.06),
                Border = Blend(baseColor, Color.Black, 0.25),
                SelBorder = Color.White
            };

            if (autoTextContrast)
            {
                bool dark = IsDark(baseColor);
                p.Title = dark ? Color.White : Color.Black;
                p.Subtitle = dark ? Color.FromArgb(230, 255, 255, 255) : Color.FromArgb(130, 0, 0, 0);
                p.Description = dark ? Color.FromArgb(230, 255, 255, 255) : Color.FromArgb(160, 0, 0, 0);
            }
            else
            {
                // Nếu không auto, để trống – caller có thể override sau
                p.Title = Color.White; p.Subtitle = Color.White; p.Description = Color.White;
            }
            return p;
        }

        private static void ApplyOverrides(ref ThemePalette p, ThemeOverrides o)
        {
            if (o.Base.HasValue) p.Base = o.Base.Value;
            if (o.Hover.HasValue) p.Hover = o.Hover.Value;
            if (o.Press.HasValue) p.Press = o.Press.Value;
            if (o.Border.HasValue) p.Border = o.Border.Value;
            if (o.SelBorder.HasValue) p.SelBorder = o.SelBorder.Value;
            if (o.Title.HasValue) p.Title = o.Title.Value;
            if (o.Subtitle.HasValue) p.Subtitle = o.Subtitle.Value;
            if (o.Description.HasValue) p.Description = o.Description.Value;
        }

        // Utils
        public static bool IsDark(Color c)
        {
            double l = (0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B) / 255.0;
            return l < 0.55;
        }
        public static Color Blend(Color a, Color b, double t)
        {
            t = Math.Max(0, Math.Min(1, t));
            int r = (int)Math.Round(a.R + (b.R - a.R) * t);
            int g = (int)Math.Round(a.G + (b.G - a.G) * t);
            int b2 = (int)Math.Round(a.B + (b.B - a.B) * t);
            return Color.FromArgb(255, r, g, b2);
        }
    }
}

