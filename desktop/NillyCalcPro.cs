// ============================================================================
// NillyCalc Pro — Ultra-Light Native Windows 11 & Graphic Designer Calculator
// created by Nilly graphics (Sotiris Christos) — https://nillygraphics.com/
// Strictly C# 5 compatible (.NET Framework 4.x csc.exe — 0 external DLLs)
// Standard + Print/DPI (px/mm/cm/in/pt/pc) + CMYK/HEX Color Picker + VAT/m2
// ============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

[assembly: AssemblyTitle("NillyCalc Pro")]
[assembly: AssemblyDescription("Ultra-Light Windows 11 & Graphic Designer Calculator (DPI, mm/cm/in/pt/px, CMYK, Color Picker, VAT & m2)")]
[assembly: AssemblyCompany("Nilly graphics (Sotiris Christos) — https://nillygraphics.com/")]
[assembly: AssemblyProduct("NillyCalc Pro")]
[assembly: AssemblyCopyright("Copyright (c) 2026 Nilly graphics (Sotiris Christos)")]
[assembly: AssemblyVersion("1.2.3.0")]
[assembly: AssemblyFileVersion("1.2.3.0")]

namespace NillyCalcPro
{
    // ========================================================================
    // 1. WIN32 INTEROP (DPI Awareness, Windows 11 Dark Titlebar, Single-Instance)
    // ========================================================================
    internal static class Native
    {
        public const int SW_RESTORE = 9;

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr minSize, IntPtr maxSize);

        public static void TrimMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false);
                SetProcessWorkingSetSize(GetCurrentProcess(), (IntPtr)(-1), (IntPtr)(-1));
            }
            catch { }
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static void SetDarkTitleBar(IntPtr hwnd, bool dark)
        {
            try
            {
                int val = dark ? 1 : 0;
                if (DwmSetWindowAttribute(hwnd, 20, ref val, 4) != 0)
                    DwmSetWindowAttribute(hwnd, 19, ref val, 4);
            }
            catch { }
        }
    }

    // ========================================================================
    // 2. WINDOWS 11 AUTHENTIC THEMES (Dark Mica & Light)
    // ========================================================================
    internal class CalcTheme
    {
        public bool IsDark;
        public Color WindowBg;
        public Color CardBg;
        public Color CardBorder;
        public Color TextPrimary;
        public Color TextMuted;
        public Color NumBtnBg;
        public Color NumBtnHover;
        public Color NumBtnPress;
        public Color OpBtnBg;
        public Color OpBtnHover;
        public Color OpBtnPress;
        public Color AccentBg;
        public Color AccentHover;
        public Color AccentText;
        public Color StudioAccent; // Nilly warm amber
        public Color InputBg;
        public Color InputBorder;
        public Color TabActiveBg;
        public Color DangerColor;
        public Color OkColor;

        public static CalcTheme Win11Dark()
        {
            CalcTheme t = new CalcTheme();
            t.IsDark = true;
            t.WindowBg = Color.FromArgb(29, 31, 32);       // #1D1F20 Ariakit Charcoal
            t.CardBg = Color.FromArgb(34, 36, 38);         // #222426 Ariakit Panel
            t.CardBorder = Color.FromArgb(54, 57, 61);     // #36393D Hairline
            t.TextPrimary = Color.FromArgb(242, 244, 247);
            t.TextMuted = Color.FromArgb(154, 160, 168);
            t.NumBtnBg = Color.FromArgb(39, 41, 43);       // #27292B Surface
            t.NumBtnHover = Color.FromArgb(48, 51, 54);    // #303336 Hover
            t.NumBtnPress = Color.FromArgb(34, 36, 38);
            t.OpBtnBg = Color.FromArgb(34, 36, 38);
            t.OpBtnHover = Color.FromArgb(44, 47, 50);
            t.OpBtnPress = Color.FromArgb(27, 29, 30);
            t.AccentBg = Color.FromArgb(252, 150, 31);     // #FC961F Nilly Orange
            t.AccentHover = Color.FromArgb(255, 169, 64);
            t.AccentText = Color.FromArgb(20, 21, 23);
            t.StudioAccent = Color.FromArgb(252, 150, 31); // #FC961F Nilly Orange
            t.InputBg = Color.FromArgb(24, 26, 27);        // #181A1B Recessed
            t.InputBorder = Color.FromArgb(54, 57, 61);
            t.TabActiveBg = Color.FromArgb(48, 51, 54);
            t.DangerColor = Color.FromArgb(240, 104, 118);
            t.OkColor = Color.FromArgb(166, 226, 46);
            return t;
        }

        public static CalcTheme Win11Light()
        {
            CalcTheme t = new CalcTheme();
            t.IsDark = false;
            t.WindowBg = Color.FromArgb(248, 249, 251);    // #F8F9FB unified Nilly Light
            t.CardBg = Color.FromArgb(238, 241, 246);
            t.CardBorder = Color.FromArgb(210, 216, 226);
            t.TextPrimary = Color.FromArgb(28, 32, 40);
            t.TextMuted = Color.FromArgb(98, 108, 125);
            t.NumBtnBg = Color.FromArgb(255, 255, 255);
            t.NumBtnHover = Color.FromArgb(236, 240, 246);
            t.NumBtnPress = Color.FromArgb(224, 230, 238);
            t.OpBtnBg = Color.FromArgb(242, 245, 249);
            t.OpBtnHover = Color.FromArgb(230, 235, 243);
            t.OpBtnPress = Color.FromArgb(218, 225, 235);
            t.AccentBg = Color.FromArgb(224, 118, 12);     // #E0760C Nilly Orange Light
            t.AccentHover = Color.FromArgb(204, 104, 8);
            t.AccentText = Color.White;
            t.StudioAccent = Color.FromArgb(224, 118, 12);
            t.InputBg = Color.White;
            t.InputBorder = Color.FromArgb(210, 216, 226);
            t.TabActiveBg = Color.FromArgb(226, 232, 242);
            t.DangerColor = Color.FromArgb(204, 28, 92);
            t.OkColor = Color.FromArgb(62, 145, 32);
            return t;
        }
    }

    // ========================================================================
    // 3. WINDOWS 11 CUSTOM ROUNDED BUTTON (Crisp GDI+, 0 Flicker)
    // ========================================================================
    internal enum Win11BtnStyle { Digit, Operator, Accent, Subtle, StudioPill }

    internal class Win11Button : Control
    {
        public Win11BtnStyle StyleKind = Win11BtnStyle.Digit;
        public CalcTheme Theme = CalcTheme.Win11Dark();
        public int CornerRadius = 5;
        public bool IsToggled = false;
        private bool _hover = false;
        private bool _down = false;

        public Win11Button()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.Cursor = Cursors.Hand;
            this.Font = new Font("Segoe UI", 12f, FontStyle.Regular);
            this.TabStop = false;
        }

        private static GraphicsPath RoundedRect(Rectangle r, int rad)
        {
            GraphicsPath p = new GraphicsPath();
            int d = Math.Max(2, rad * 2);
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Color bg, fg, border;
            if (StyleKind == Win11BtnStyle.Accent || IsToggled)
            {
                bg = _down ? Theme.AccentHover : (_hover ? Theme.AccentHover : Theme.AccentBg);
                fg = Theme.AccentText;
                border = bg;
            }
            else if (StyleKind == Win11BtnStyle.StudioPill)
            {
                bg = _down ? Theme.OpBtnPress : (_hover ? Theme.NumBtnBg : Theme.OpBtnBg);
                fg = Theme.StudioAccent;
                border = Theme.CardBorder;
            }
            else if (StyleKind == Win11BtnStyle.Subtle)
            {
                bg = _down ? Theme.OpBtnPress : (_hover ? Theme.OpBtnBg : Theme.WindowBg);
                fg = _hover ? Theme.TextPrimary : Theme.TextMuted;
                border = _hover ? Theme.CardBorder : Theme.WindowBg;
            }
            else if (StyleKind == Win11BtnStyle.Operator)
            {
                bg = _down ? Theme.OpBtnPress : (_hover ? Theme.OpBtnHover : Theme.OpBtnBg);
                fg = Theme.TextPrimary;
                border = Theme.CardBorder;
            }
            else
            {
                bg = _down ? Theme.NumBtnPress : (_hover ? Theme.NumBtnHover : Theme.NumBtnBg);
                fg = Theme.TextPrimary;
                border = Theme.CardBorder;
            }

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using (GraphicsPath path = RoundedRect(rect, CornerRadius))
            {
                using (SolidBrush b = new SolidBrush(bg)) g.FillPath(b, path);
                if (StyleKind != Win11BtnStyle.Accent && !IsToggled && border != Theme.WindowBg)
                {
                    using (Pen pen = new Pen(border, 1f)) g.DrawPath(pen, path);
                }
            }

            TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true; this.Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; _down = false; this.Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) { _down = true; this.Invalidate(); } }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); _down = false; this.Invalidate(); }
    }

    // ========================================================================
    // 4. PURE MATH, PRINT/DPI, COLOR & VAT ENGINES (Testable & Exact)
    // ========================================================================
    internal static class StudioMath
    {
        public static bool TryParseNum(string s, out double val)
        {
            val = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            string clean = s.Trim().Replace(" ", "").Replace("€", "").Replace("%", "").Replace(",", ".");
            return double.TryParse(clean, NumberStyles.Float, CultureInfo.InvariantCulture, out val);
        }

        public static string Fmt(double v, int maxDecimals)
        {
            if (double.IsNaN(v) || double.IsInfinity(v)) return "Error";
            if (Math.Abs(v) < 1e-12) v = 0;
            string pattern = maxDecimals <= 0 ? "0" : ("0." + new string('#', maxDecimals));
            return v.ToString(pattern, CultureInfo.InvariantCulture);
        }

        // Convert any unit (px, mm, cm, in, pt, pc) -> inches
        public static double ToInches(double value, string unit, double dpi)
        {
            if (dpi <= 0) dpi = 300;
            switch (unit)
            {
                case "px": return value / dpi;
                case "mm": return value / 25.4;
                case "cm": return value / 2.54;
                case "in": return value;
                case "pt": return value / 72.0;
                case "pc": return value / 6.0;
                default: return value;
            }
        }

        public static double FromInches(double inches, string unit, double dpi)
        {
            if (dpi <= 0) dpi = 300;
            switch (unit)
            {
                case "px": return inches * dpi;
                case "mm": return inches * 25.4;
                case "cm": return inches * 2.54;
                case "in": return inches;
                case "pt": return inches * 72.0;
                case "pc": return inches * 6.0;
                default: return inches;
            }
        }

        public static string FormatAspectRatio(double w, double h)
        {
            if (w <= 0 || h <= 0) return "-";
            double r = w / h;
            double[,] known = new double[,] {
                { 1, 1 }, { 4, 3 }, { 3, 2 }, { 16, 9 }, { 16, 10 }, { 21, 9 }, { 4, 5 }, { 9, 16 }, { 2, 3 }
            };
            for (int i = 0; i < known.GetLength(0); i++)
            {
                if (Math.Abs(r - (known[i, 0] / known[i, 1])) < 0.015)
                    return known[i, 0].ToString("0") + ":" + known[i, 1].ToString("0");
            }
            if (Math.Abs(r - 0.7071) < 0.015 || Math.Abs(r - 1.4142) < 0.015)
                return r < 1 ? "1:1.414 (ISO A)" : "1.414:1 (ISO A)";
            return Fmt(r, 2) + ":1";
        }

        public static void RgbToCmyk(int r, int g, int b, out int c, out int m, out int y, out int k)
        {
            double rf = r / 255.0, gf = g / 255.0, bf = b / 255.0;
            double kf = 1.0 - Math.Max(rf, Math.Max(gf, bf));
            if (kf >= 0.9999)
            {
                c = 0; m = 0; y = 0; k = 100;
                return;
            }
            c = (int)Math.Round(((1.0 - rf - kf) / (1.0 - kf)) * 100.0);
            m = (int)Math.Round(((1.0 - gf - kf) / (1.0 - kf)) * 100.0);
            y = (int)Math.Round(((1.0 - bf - kf) / (1.0 - kf)) * 100.0);
            k = (int)Math.Round(kf * 100.0);
        }

        public static Color CmykToRgb(int c, int m, int y, int k)
        {
            double cf = Math.Max(0, Math.Min(100, c)) / 100.0;
            double mf = Math.Max(0, Math.Min(100, m)) / 100.0;
            double yf = Math.Max(0, Math.Min(100, y)) / 100.0;
            double kf = Math.Max(0, Math.Min(100, k)) / 100.0;
            int r = (int)Math.Round(255.0 * (1.0 - cf) * (1.0 - kf));
            int g = (int)Math.Round(255.0 * (1.0 - mf) * (1.0 - kf));
            int b = (int)Math.Round(255.0 * (1.0 - yf) * (1.0 - kf));
            return Color.FromArgb(ClampByte(r), ClampByte(g), ClampByte(b));
        }

        public static void RgbToHsl(int r, int g, int b, out int h, out int s, out int l)
        {
            double rf = r / 255.0, gf = g / 255.0, bf = b / 255.0;
            double max = Math.Max(rf, Math.Max(gf, bf));
            double min = Math.Min(rf, Math.Min(gf, bf));
            double lf = (max + min) / 2.0;
            double hf = 0, sf = 0;
            if (Math.Abs(max - min) > 1e-6)
            {
                double d = max - min;
                sf = lf > 0.5 ? d / (2.0 - max - min) : d / (max + min);
                if (max == rf) hf = (gf - bf) / d + (gf < bf ? 6.0 : 0.0);
                else if (max == gf) hf = (bf - rf) / d + 2.0;
                else hf = (rf - gf) / d + 4.0;
                hf /= 6.0;
            }
            h = (int)Math.Round(hf * 360.0) % 360;
            s = (int)Math.Round(sf * 100.0);
            l = (int)Math.Round(lf * 100.0);
        }

        public static Color HslToRgb(int h, int s, int l)
        {
            double hf = ((h % 360) + 360) % 360 / 360.0;
            double sf = Math.Max(0, Math.Min(100, s)) / 100.0;
            double lf = Math.Max(0, Math.Min(100, l)) / 100.0;
            if (sf < 1e-6)
            {
                int v = ClampByte((int)Math.Round(lf * 255.0));
                return Color.FromArgb(v, v, v);
            }
            double q = lf < 0.5 ? lf * (1.0 + sf) : lf + sf - lf * sf;
            double p = 2.0 * lf - q;
            int r = ClampByte((int)Math.Round(Hue2Rgb(p, q, hf + 1.0 / 3.0) * 255.0));
            int g = ClampByte((int)Math.Round(Hue2Rgb(p, q, hf) * 255.0));
            int b = ClampByte((int)Math.Round(Hue2Rgb(p, q, hf - 1.0 / 3.0) * 255.0));
            return Color.FromArgb(r, g, b);
        }

        private static double Hue2Rgb(double p, double q, double t)
        {
            if (t < 0) t += 1.0;
            if (t > 1) t -= 1.0;
            if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;
            if (t < 0.5) return q;
            if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;
            return p;
        }

        public static bool TryParseHex(string hex, out Color color)
        {
            color = Color.Empty;
            if (string.IsNullOrWhiteSpace(hex)) return false;
            string s = hex.Trim().TrimStart('#');
            if (s.Length == 3)
                s = "" + s[0] + s[0] + s[1] + s[1] + s[2] + s[2];
            if (s.Length != 6) return false;
            int rgb;
            if (!int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out rgb)) return false;
            color = Color.FromArgb((rgb >> 16) & 255, (rgb >> 8) & 255, rgb & 255);
            return true;
        }

        public static string ToHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.R, c.G, c.B);
        }

        public static double RelativeLuminance(Color c)
        {
            double r = Lin(c.R / 255.0), g = Lin(c.G / 255.0), b = Lin(c.B / 255.0);
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private static double Lin(double v)
        {
            return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4);
        }

        public static int ClampByte(int v) { return v < 0 ? 0 : (v > 255 ? 255 : v); }
    }

    // ========================================================================
    // 5. LIVE SCREEN COLOR PICKER OVERLAY (11x11 Pixel-Grid Loupe + HEX/CMYK)
    // ========================================================================
    internal class ScreenEyedropperForm : Form
    {
        private Bitmap _screenBmp;
        private Rectangle _vBounds;
        private Point _mousePos;
        public Color PickedColor = Color.Empty;
        public bool DidPick = false;

        public ScreenEyedropperForm()
        {
            _vBounds = SystemInformation.VirtualScreen;
            _screenBmp = new Bitmap(_vBounds.Width, _vBounds.Height);
            using (Graphics g = Graphics.FromImage(_screenBmp))
                g.CopyFromScreen(_vBounds.Left, _vBounds.Top, 0, 0, _vBounds.Size);

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = _vBounds;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Cross;
            this.KeyPreview = true;
            _mousePos = this.PointToClient(Cursor.Position);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _mousePos = e.Location;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                int x = Math.Max(0, Math.Min(_screenBmp.Width - 1, e.X));
                int y = Math.Max(0, Math.Min(_screenBmp.Height - 1, e.Y));
                PickedColor = _screenBmp.GetPixel(x, y);
                DidPick = true;
            }
            this.Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape) this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImageUnscaled(_screenBmp, 0, 0);

            int px = Math.Max(0, Math.Min(_screenBmp.Width - 1, _mousePos.X));
            int py = Math.Max(0, Math.Min(_screenBmp.Height - 1, _mousePos.Y));
            Color col = _screenBmp.GetPixel(px, py);
            int c, m, y, k;
            StudioMath.RgbToCmyk(col.R, col.G, col.B, out c, out m, out y, out k);

            // Floating Loupe Card (170x195) positioned cleanly away from screen edges
            int cardW = 176, cardH = 192;
            int lx = px + 22;
            int ly = py + 22;
            if (lx + cardW > this.Width - 10) lx = px - cardW - 22;
            if (ly + cardH > this.Height - 10) ly = py - cardH - 22;

            using (SolidBrush cardBg = new SolidBrush(Color.FromArgb(242, 26, 26, 26)))
            using (Pen cardBorder = new Pen(Color.FromArgb(255, 253, 151, 31), 2f))
            {
                g.FillRectangle(cardBg, lx, ly, cardW, cardH);
                g.DrawRectangle(cardBorder, lx, ly, cardW, cardH);
            }

            // 11x11 magnified pixel grid (11 * 11 = 121 px)
            int cells = 11, cellSz = 11, half = 5;
            int gx = lx + (cardW - cells * cellSz) / 2;
            int gy = ly + 8;
            for (int dy = -half; dy <= half; dy++)
            {
                for (int dx = -half; dx <= half; dx++)
                {
                    int sx = Math.Max(0, Math.Min(_screenBmp.Width - 1, px + dx));
                    int sy = Math.Max(0, Math.Min(_screenBmp.Height - 1, py + dy));
                    using (SolidBrush sb = new SolidBrush(_screenBmp.GetPixel(sx, sy)))
                        g.FillRectangle(sb, gx + (dx + half) * cellSz, gy + (dy + half) * cellSz, cellSz, cellSz);
                }
            }
            // Center target box
            using (Pen targetPen = new Pen(Color.White, 1.5f))
                g.DrawRectangle(targetPen, gx + half * cellSz, gy + half * cellSz, cellSz - 1, cellSz - 1);

            // Swatch + Readout
            int infoY = gy + cells * cellSz + 8;
            using (SolidBrush sw = new SolidBrush(col))
                g.FillRectangle(sw, lx + 10, infoY, 26, 42);
            using (Pen swp = new Pen(Color.FromArgb(120, 255, 255, 255)))
                g.DrawRectangle(swp, lx + 10, infoY, 26, 42);

            using (Font fBold = new Font("Consolas", 9.5f, FontStyle.Bold))
            using (Font fSm = new Font("Segoe UI", 8f))
            {
                TextRenderer.DrawText(g, StudioMath.ToHex(col) + "  (" + col.R + "," + col.G + "," + col.B + ")",
                    fBold, new Point(lx + 42, infoY - 1), Color.White);
                TextRenderer.DrawText(g, string.Format("CMYK {0}% {1}% {2}% {3}%", c, m, y, k),
                    fSm, new Point(lx + 42, infoY + 17), Color.FromArgb(200, 200, 200));
                TextRenderer.DrawText(g, "Click: Pick  |  Esc: Cancel",
                    fSm, new Point(lx + 42, infoY + 32), Color.FromArgb(253, 151, 31));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _screenBmp != null) { _screenBmp.Dispose(); _screenBmp = null; }
            base.Dispose(disposing);
        }
    }

    // ========================================================================
    // 6. MAIN WINDOWS 11 STUDIO CALCULATOR WINDOW (MainForm)
    // ========================================================================
    internal class MainForm : Form
    {
        private CalcTheme _theme = CalcTheme.Win11Dark();
        private bool _pinned = false;
        private int _activeTab = 0;

        // Top Navigation & Tools
        private Panel _topBar;
        private AkButton[] _tabBtns = new AkButton[4];
        private AkButton _btnPipetteTop;
        private AkButton _btnPin;
        private AkButton _btnTheme;
        private Panel _contentHost;
        private Panel[] _tabPanels = new Panel[4];
        private Label _lblToast;
        private System.Windows.Forms.Timer _toastTimer;

        // Tab 0: Standard Windows 11 Calculator
        private Label _lblExpr;
        private Label _lblDisp;
        private double _acc = 0;
        private double _lastOperand = 0;
        private string _pendingOp = "";
        private string _lastOp = "";
        private bool _freshEntry = true;
        private double _memory = 0;
        private bool _hasMemory = false;
        private Label _lblMemBadge;
        private List<string> _history = new List<string>();
        private Panel _historyOverlay;
        private ListBox _historyList;

        // Tab 1: Print & DPI Studio (in, mm, cm, px, pt, pc)
        private bool _updatingUnits = false;
        private double _dpi = 300.0;
        private TextBox _txtDpi;
        private Win11Button[] _dpiPills;
        private TextBox _txtPx, _txtMm, _txtCm, _txtIn, _txtPt, _txtPc;
        private ComboBox _cmbPreset;
        private ComboBox _cmbCanvasUnit;
        private TextBox _txtCanvasW, _txtCanvasH, _txtBleed, _txtScaleNewW;
        private Label _lblCanvasSpecs;

        // Tab 2: CMYK & Color Studio
        private bool _updatingColor = false;
        private Color _curColor = Color.FromArgb(253, 151, 31); // #FD971F Nilly Amber
        private Panel _swatchBanner;
        private Label _lblSwatchHex, _lblSwatchWcag, _lblSwatchTac;
        private TextBox _txtHex, _txtR, _txtG, _txtB, _txtC, _txtM, _txtY, _txtK, _txtH, _txtS, _txtL;
        private Panel[] _shadeBoxes = new Panel[5];
        private Panel[] _recentBoxes = new Panel[8];
        private List<Color> _recentColors = new List<Color>();

        // Tab 3: VAT & m2 Studio Pricing
        private bool _updatingVat = false;
        private double _vatRate = 24.0;
        private TextBox _txtVatCustom, _txtVatNet, _txtVatGross, _txtVatDisc;
        private Label _lblVatResult;
        private TextBox _txtM2W, _txtM2H, _txtM2Qty, _txtM2Price;
        private ComboBox _cmbM2Unit;
        private CheckBox _chkM2Vat;
        private Label _lblM2Result;

        private static string SettingsPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NillyCalcPro\\settings.ini"); }
        }

        public MainForm() : this(null) { }

        public MainForm(string initialHex)
        {
            this.Text = "NillyCalc Pro";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.ClientSize = new Size(444, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;

            try
            {
                string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NillyCalcPro.ico");
                if (File.Exists(icoPath)) this.Icon = new Icon(icoPath);
                else this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            _recentColors.Add(Color.FromArgb(253, 151, 31));
            _recentColors.Add(Color.FromArgb(76, 194, 255));
            _recentColors.Add(Color.FromArgb(249, 38, 114));
            _recentColors.Add(Color.FromArgb(166, 226, 46));
            _recentColors.Add(Color.FromArgb(39, 40, 34));
            _recentColors.Add(Color.FromArgb(255, 255, 255));

            BuildUI();
            LoadSettings();
            ApplyTheme(_theme);
            SwitchTab(0);
            UpdateAllUnitsFromInches(210.0 / 25.4, null);
            RecalcCanvasSpecs();
            SetStudioColor(_curColor, false);
            RecalcVatFromNet();
            RecalcM2();

            if (!string.IsNullOrEmpty(initialHex))
            {
                Color parsedHex;
                if (StudioMath.TryParseHex(initialHex, out parsedHex))
                {
                    SwitchTab(2);
                    SetStudioColor(parsedHex, true);
                }
            }

            _toastTimer = new System.Windows.Forms.Timer();
            _toastTimer.Interval = 1600;
            _toastTimer.Tick += delegate { _toastTimer.Stop(); _lblToast.Text = "NillyCalc Pro · Nilly graphics (Ctrl+1..4 tabs · Ctrl+Shift+C Pipette)"; };

            this.HandleCreated += delegate { Native.SetDarkTitleBar(this.Handle, _theme.IsDark); };
            this.FormClosing += delegate { SaveSettings(); };
            this.KeyDown += OnGlobalKeyDown;
        }

        private void ShowToast(string msg)
        {
            _lblToast.Text = msg;
            if (_toastTimer != null) { _toastTimer.Stop(); _toastTimer.Start(); }
        }

        private void CopyText(string text, string label)
        {
            try
            {
                Clipboard.SetText(text);
                ShowToast("Copied: " + label + " (" + text + ")");
            }
            catch { }
        }

        private static string FindSiblingExe(string folder, string exeName)
        {
            try
            {
                string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"NillySuite\" + folder + @"\" + exeName);
                if (File.Exists(appData)) return appData;
                string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                DirectoryInfo par = Directory.GetParent(baseDir);
                if (par != null)
                {
                    string sib = Path.Combine(Path.Combine(par.FullName, folder), exeName);
                    if (File.Exists(sib)) return sib;
                }
            }
            catch { }
            return null;
        }

        private void SendToNillyNote()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== NillyCalc Pro — Σύνοψη Υπολογισμών ===");
            sb.AppendLine("Ημερομηνία: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            sb.AppendLine("created by Nilly graphics (Sotiris Christos) — nillygraphics.com");
            sb.AppendLine("");
            if (_lblDisp != null) sb.AppendLine("Τρέχον Αποτέλεσμα:  " + _lblDisp.Text);
            if (_lblExpr != null && !string.IsNullOrEmpty(_lblExpr.Text)) sb.AppendLine("Πράξη:  " + _lblExpr.Text);
            if (_history.Count > 0)
            {
                sb.AppendLine("");
                sb.AppendLine("--- Ιστορικό Πράξεων ---");
                for (int i = 0; i < _history.Count; i++) sb.AppendLine("  • " + _history[i]);
            }
            if (_lblCanvasSpecs != null && !string.IsNullOrEmpty(_lblCanvasSpecs.Text))
            {
                sb.AppendLine("");
                sb.AppendLine("--- Print & DPI ---");
                sb.AppendLine(_lblCanvasSpecs.Text);
            }
            if (_txtVatNet != null && _txtVatGross != null)
            {
                sb.AppendLine("");
                sb.AppendLine("--- ΦΠΑ & m² ---");
                sb.AppendLine("Καθαρή: " + _txtVatNet.Text + " €  |  Τελική με ΦΠΑ: " + _txtVatGross.Text + " €");
                if (_lblM2Result != null && !string.IsNullOrEmpty(_lblM2Result.Text))
                    sb.AppendLine(_lblM2Result.Text);
            }

            string content = sb.ToString();
            string noteExe = FindSiblingExe("NillyNotePro", "NillyNotePro.exe");
            if (!string.IsNullOrEmpty(noteExe))
            {
                try
                {
                    string tmp = Path.Combine(Path.GetTempPath(), "NillyCalc_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
                    File.WriteAllText(tmp, content, new UTF8Encoding(false));
                    Process.Start(new ProcessStartInfo(noteExe, "\"" + tmp + "\"") { WorkingDirectory = Path.GetDirectoryName(noteExe), UseShellExecute = true });
                    ShowToast("↗ Εστάλη στο NillyNote Pro!");
                    return;
                }
                catch { }
            }
            try { Clipboard.SetText(content); ShowToast("✓ Αντιγράφηκε η σύνοψη στο πρόχειρο (βάλε NillyNote Pro για άμεσο άνοιγμα)"); } catch { }
        }

        private void BuildUI()
        {
            // 1. Top Ariakit Navigation & Vector Utility Bar
            _topBar = new Panel();
            _topBar.Dock = DockStyle.Top;
            _topBar.Height = 44;
            _topBar.Paint += delegate(object s, PaintEventArgs pe)
            {
                using (Pen p = new Pen(_theme.CardBorder))
                    pe.Graphics.DrawLine(p, 0, _topBar.Height - 1, _topBar.Width, _topBar.Height - 1);
            };

            string[] names = new string[] { "Standard", "Print·DPI", "CMYK", "ΦΠΑ·m²" };
            int[] widths = new int[] { 76, 76, 62, 68 };
            int x = 8;
            for (int i = 0; i < 4; i++)
            {
                int idx = i;
                AkButton tb = new AkButton();
                tb.Kind = AkKind.Tab;
                tb.Text = names[i];
                tb.Font = new Font("Segoe UI", 8.6f, FontStyle.Bold);
                tb.Bounds = new Rectangle(x, 6, widths[i], 34);
                tb.Click += delegate { SwitchTab(idx); };
                _tabBtns[i] = tb;
                _topBar.Controls.Add(tb);
                x += widths[i] + 3;
            }

            // Right crisp vector icon utilities (Pipette, Pin, Theme)
            _btnPipetteTop = new AkButton();
            _btnPipetteTop.Kind = AkKind.AccentOutline;
            _btnPipetteTop.Icon = AkIcon.Pipette;
            _btnPipetteTop.Text = "";
            _btnPipetteTop.Bounds = new Rectangle(322, 8, 34, 28);
            _btnPipetteTop.Click += delegate { LaunchScreenPipette(); };
            _topBar.Controls.Add(_btnPipetteTop);

            _btnPin = new AkButton();
            _btnPin.Kind = AkKind.Secondary;
            _btnPin.Icon = AkIcon.Pin;
            _btnPin.Text = "";
            _btnPin.Bounds = new Rectangle(362, 8, 34, 28);
            _btnPin.Click += delegate { TogglePin(); };
            _topBar.Controls.Add(_btnPin);

            _btnTheme = new AkButton();
            _btnTheme.Kind = AkKind.Secondary;
            _btnTheme.Icon = AkIcon.Sun;
            _btnTheme.Text = "";
            _btnTheme.Bounds = new Rectangle(402, 8, 34, 28);
            _btnTheme.Click += delegate { ApplyTheme(_theme.IsDark ? CalcTheme.Win11Light() : CalcTheme.Win11Dark()); };
            _topBar.Controls.Add(_btnTheme);

            ToolTip tip = new ToolTip();
            tip.SetToolTip(_btnPipetteTop, "Screen Color Picker / Πιπέτα Οθόνης (Ctrl+Shift+C)");
            tip.SetToolTip(_btnPin, "Keep on Top / Καρφίτσωμα (Ctrl+T)");
            tip.SetToolTip(_btnTheme, "Switch Dark / Light Theme");

            // 2. Subtle Bottom Footer / Toast + 1-Click Send to NillyNote Pro
            Panel footerBar = new Panel();
            footerBar.Dock = DockStyle.Bottom;
            footerBar.Height = 30;
            footerBar.Padding = new Padding(0, 2, 8, 2);
            footerBar.Paint += delegate(object s, PaintEventArgs pe)
            {
                using (Pen p = new Pen(_theme.CardBorder))
                    pe.Graphics.DrawLine(p, 0, 0, footerBar.Width, 0);
            };

            _lblToast = new Label();
            _lblToast.Dock = DockStyle.Fill;
            _lblToast.TextAlign = ContentAlignment.MiddleLeft;
            _lblToast.Padding = new Padding(8, 0, 4, 0);
            _lblToast.Font = new Font("Segoe UI", 7.8f);
            _lblToast.AutoEllipsis = true;
            _lblToast.Text = "NillyCalc Pro · Nilly graphics (Ctrl+1..4 tabs)";

            AkButton btnSendNote = new AkButton();
            btnSendNote.Kind = AkKind.AccentOutline;
            btnSendNote.Icon = AkIcon.External;
            btnSendNote.Text = "Note";
            btnSendNote.Dock = DockStyle.Right;
            btnSendNote.Width = 72;
            btnSendNote.Font = new Font("Segoe UI", 8.0f, FontStyle.Bold);
            btnSendNote.Click += delegate { SendToNillyNote(); };
            tip.SetToolTip(btnSendNote, "Αποστολή αποτελέσματος, ιστορικού & ΦΠΑ/m² απευθείας σε NillyNote Pro");

            footerBar.Controls.Add(_lblToast);
            footerBar.Controls.Add(btnSendNote);

            // 3. Content Host
            _contentHost = new Panel();
            _contentHost.Dock = DockStyle.Fill;
            _contentHost.Padding = new Padding(8, 2, 8, 4);

            _tabPanels[0] = BuildStandardTab();
            _tabPanels[1] = BuildPrintDpiTab();
            _tabPanels[2] = BuildColorTab();
            _tabPanels[3] = BuildVatM2Tab();

            for (int i = 0; i < 4; i++)
            {
                _tabPanels[i].Dock = DockStyle.Fill;
                _tabPanels[i].Visible = (i == 0);
                _contentHost.Controls.Add(_tabPanels[i]);
            }

            this.Controls.Add(_contentHost);
            this.Controls.Add(footerBar);
            this.Controls.Add(_topBar);
        }

        // ====================================================================
        // TAB 0: WINDOWS 11 STANDARD CALCULATOR
        // ====================================================================
        private Panel BuildStandardTab()
        {
            Panel p = new Panel();

            // Display block (non-overlapping memory badge + expression)
            _lblMemBadge = new Label();
            _lblMemBadge.Bounds = new Rectangle(4, 4, 34, 24);
            _lblMemBadge.TextAlign = ContentAlignment.MiddleLeft;
            _lblMemBadge.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            _lblMemBadge.Text = "";
            p.Controls.Add(_lblMemBadge);

            _lblExpr = new Label();
            _lblExpr.Bounds = new Rectangle(42, 4, 382, 24);
            _lblExpr.TextAlign = ContentAlignment.MiddleRight;
            _lblExpr.Font = new Font("Segoe UI", 10.5f);
            _lblExpr.Text = "";
            p.Controls.Add(_lblExpr);

            _lblDisp = new Label();
            _lblDisp.Bounds = new Rectangle(4, 28, 420, 68);
            _lblDisp.TextAlign = ContentAlignment.MiddleRight;
            _lblDisp.Font = new Font("Segoe UI Semibold", 34f, FontStyle.Bold);
            _lblDisp.Text = "0";
            _lblDisp.Cursor = Cursors.Hand;
            _lblDisp.Click += delegate { CopyText(_lblDisp.Text, "Result"); };
            p.Controls.Add(_lblDisp);

            // Quick Memory + Studio VAT + History Row
            string[] memLabels = new string[] { "MC", "MR", "M+", "M-", "+24%", "÷1.24", "Copy", "Hist" };
            int mx = 4;
            int mw = 51;
            for (int i = 0; i < memLabels.Length; i++)
            {
                string lbl = memLabels[i];
                Win11Button b = new Win11Button();
                b.Text = lbl;
                b.Font = new Font("Segoe UI", 8.2f, FontStyle.Bold);
                b.StyleKind = (lbl == "+24%" || lbl == "÷1.24") ? Win11BtnStyle.StudioPill : Win11BtnStyle.Subtle;
                b.CornerRadius = 4;
                b.Bounds = new Rectangle(mx, 102, mw, 28);
                b.Click += delegate { HandleQuickAction(lbl); };
                p.Controls.Add(b);
                mx += mw + 2;
            }

            // Exact Windows 11 Calculator 4x6 Button Grid
            string[,] grid = new string[6, 4] {
                { "%",   "CE",  "C",   "⌫" },
                { "1/x", "x²",  "√x",  "÷" },
                { "7",   "8",   "9",   "×" },
                { "4",   "5",   "6",   "−" },
                { "1",   "2",   "3",   "+" },
                { "±",   "0",   ".",   "=" }
            };

            int startY = 136;
            int cols = 4, rows = 6, gap = 5;
            int btnW = (420 - gap * (cols - 1)) / cols;
            int btnH = 62;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    string key = grid[r, c];
                    Win11Button btn = new Win11Button();
                    btn.Text = key;
                    btn.CornerRadius = 6;
                    btn.Bounds = new Rectangle(4 + c * (btnW + gap), startY + r * (btnH + gap), btnW, btnH);

                    bool isDigit = (key.Length == 1 && char.IsDigit(key[0])) || key == "." || key == "±";
                    if (key == "=")
                    {
                        btn.StyleKind = Win11BtnStyle.Accent;
                        btn.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
                    }
                    else if (isDigit)
                    {
                        btn.StyleKind = Win11BtnStyle.Digit;
                        btn.Font = new Font("Segoe UI Semibold", 15f);
                    }
                    else
                    {
                        btn.StyleKind = Win11BtnStyle.Operator;
                        btn.Font = new Font("Segoe UI", (key == "÷" || key == "×" || key == "−" || key == "+") ? 15f : 11.5f);
                    }

                    btn.Click += delegate { HandleCalcKey(key); };
                    p.Controls.Add(btn);
                }
            }

            // Collapsible History Overlay inside Standard tab
            _historyOverlay = new Panel();
            _historyOverlay.Bounds = new Rectangle(4, 138, 420, 434);
            _historyOverlay.Visible = false;

            Label hTitle = new Label();
            hTitle.Text = "Calculation History (click to reuse)";
            hTitle.Dock = DockStyle.Top;
            hTitle.Height = 28;
            hTitle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            hTitle.Padding = new Padding(6, 4, 0, 0);

            Win11Button btnClearHist = new Win11Button();
            btnClearHist.Text = "Clear History";
            btnClearHist.Dock = DockStyle.Bottom;
            btnClearHist.Height = 32;
            btnClearHist.Font = new Font("Segoe UI", 9f);
            btnClearHist.StyleKind = Win11BtnStyle.Operator;
            btnClearHist.Click += delegate { _history.Clear(); _historyList.Items.Clear(); };

            _historyList = new ListBox();
            _historyList.Dock = DockStyle.Fill;
            _historyList.BorderStyle = BorderStyle.None;
            _historyList.Font = new Font("Consolas", 10.5f);
            _historyList.ItemHeight = 24;
            _historyList.SelectedIndexChanged += delegate
            {
                if (_historyList.SelectedItem != null)
                {
                    string item = _historyList.SelectedItem.ToString();
                    int eq = item.LastIndexOf('=');
                    if (eq >= 0)
                    {
                        SetDisplayValue(item.Substring(eq + 1).Trim());
                        _freshEntry = true;
                        _historyOverlay.Visible = false;
                    }
                }
            };

            _historyOverlay.Controls.Add(_historyList);
            _historyOverlay.Controls.Add(hTitle);
            _historyOverlay.Controls.Add(btnClearHist);
            p.Controls.Add(_historyOverlay);
            _historyOverlay.BringToFront();

            return p;
        }

        private void SetDisplayValue(string txt)
        {
            _lblDisp.Text = txt;
            float sz = txt.Length <= 11 ? 34f : (txt.Length <= 15 ? 26f : 20f);
            if (Math.Abs(_lblDisp.Font.Size - sz) > 0.5f)
                _lblDisp.Font = new Font("Segoe UI Semibold", sz, FontStyle.Bold);
        }

        private double CurrentVal()
        {
            double v;
            return StudioMath.TryParseNum(_lblDisp.Text, out v) ? v : 0;
        }

        private void HandleQuickAction(string act)
        {
            double cur = CurrentVal();
            if (act == "MC") { _memory = 0; _hasMemory = false; _lblMemBadge.Text = ""; ShowToast("Memory cleared"); }
            else if (act == "MR") { if (_hasMemory) { SetDisplayValue(StudioMath.Fmt(_memory, 8)); _freshEntry = true; } }
            else if (act == "M+") { _memory += cur; _hasMemory = true; _lblMemBadge.Text = "M"; ShowToast("M = " + StudioMath.Fmt(_memory, 4)); }
            else if (act == "M-") { _memory -= cur; _hasMemory = true; _lblMemBadge.Text = "M"; ShowToast("M = " + StudioMath.Fmt(_memory, 4)); }
            else if (act == "+24%")
            {
                double res = cur * 1.24;
                AddHistory(StudioMath.Fmt(cur, 4) + " + 24% ΦΠΑ = " + StudioMath.Fmt(res, 2));
                _lblExpr.Text = StudioMath.Fmt(cur, 4) + " + 24% =";
                SetDisplayValue(StudioMath.Fmt(res, 2));
                _freshEntry = true;
            }
            else if (act == "÷1.24")
            {
                double res = cur / 1.24;
                AddHistory(StudioMath.Fmt(cur, 4) + " ÷ 1.24 = " + StudioMath.Fmt(res, 2));
                _lblExpr.Text = StudioMath.Fmt(cur, 4) + " ÷ 1.24 =";
                SetDisplayValue(StudioMath.Fmt(res, 2));
                _freshEntry = true;
            }
            else if (act == "Copy") { CopyText(_lblDisp.Text, "Result"); }
            else if (act == "Hist") { _historyOverlay.Visible = !_historyOverlay.Visible; _historyOverlay.BringToFront(); }
        }

        private void AddHistory(string entry)
        {
            _history.Insert(0, entry);
            if (_history.Count > 35) _history.RemoveAt(_history.Count - 1);
            _historyList.Items.Clear();
            for (int i = 0; i < _history.Count; i++) _historyList.Items.Add(_history[i]);
        }

        public void HandleCalcKey(string key)
        {
            if (key.Length == 1 && char.IsDigit(key[0]))
            {
                if (_freshEntry || _lblDisp.Text == "0" || _lblDisp.Text == "Error")
                {
                    SetDisplayValue(key);
                    _freshEntry = false;
                }
                else if (_lblDisp.Text.Length < 18)
                {
                    SetDisplayValue(_lblDisp.Text + key);
                }
                return;
            }

            if (key == "." || key == ",")
            {
                if (_freshEntry || _lblDisp.Text == "Error") { SetDisplayValue("0."); _freshEntry = false; }
                else if (_lblDisp.Text.IndexOf('.') < 0) SetDisplayValue(_lblDisp.Text + ".");
                return;
            }

            if (key == "C")
            {
                _acc = 0; _pendingOp = ""; _lastOp = ""; _lastOperand = 0; _freshEntry = true;
                _lblExpr.Text = ""; SetDisplayValue("0");
                return;
            }
            if (key == "CE") { SetDisplayValue("0"); _freshEntry = true; return; }
            if (key == "⌫")
            {
                if (_freshEntry) return;
                string s = _lblDisp.Text;
                SetDisplayValue(s.Length <= 1 || (s.Length == 2 && s.StartsWith("-")) ? "0" : s.Substring(0, s.Length - 1));
                return;
            }

            double cur = CurrentVal();
            if (key == "±") { SetDisplayValue(StudioMath.Fmt(-cur, 10)); return; }
            if (key == "1/x")
            {
                if (Math.Abs(cur) < 1e-15) { SetDisplayValue("Error"); _freshEntry = true; return; }
                double r = 1.0 / cur;
                _lblExpr.Text = "1/(" + StudioMath.Fmt(cur, 8) + ")";
                SetDisplayValue(StudioMath.Fmt(r, 10)); _freshEntry = true; return;
            }
            if (key == "x²")
            {
                double r = cur * cur;
                _lblExpr.Text = "sqr(" + StudioMath.Fmt(cur, 8) + ")";
                SetDisplayValue(StudioMath.Fmt(r, 10)); _freshEntry = true; return;
            }
            if (key == "√x")
            {
                if (cur < 0) { SetDisplayValue("Error"); _freshEntry = true; return; }
                double r = Math.Sqrt(cur);
                _lblExpr.Text = "√(" + StudioMath.Fmt(cur, 8) + ")";
                SetDisplayValue(StudioMath.Fmt(r, 10)); _freshEntry = true; return;
            }
            if (key == "%")
            {
                double pct = string.IsNullOrEmpty(_pendingOp) ? (cur / 100.0) : (_acc * cur / 100.0);
                SetDisplayValue(StudioMath.Fmt(pct, 10)); _freshEntry = true; return;
            }

            if (key == "+" || key == "−" || key == "-" || key == "×" || key == "*" || key == "÷" || key == "/")
            {
                string normOp = (key == "-") ? "−" : ((key == "*") ? "×" : ((key == "/") ? "÷" : key));
                if (!string.IsNullOrEmpty(_pendingOp) && !_freshEntry)
                {
                    _acc = ApplyBinary(_acc, cur, _pendingOp);
                    SetDisplayValue(StudioMath.Fmt(_acc, 10));
                }
                else
                {
                    _acc = cur;
                }
                _pendingOp = normOp;
                _lblExpr.Text = StudioMath.Fmt(_acc, 10) + " " + _pendingOp;
                _freshEntry = true;
                return;
            }

            if (key == "=")
            {
                if (!string.IsNullOrEmpty(_pendingOp))
                {
                    double right = cur;
                    double res = ApplyBinary(_acc, right, _pendingOp);
                    string line = StudioMath.Fmt(_acc, 8) + " " + _pendingOp + " " + StudioMath.Fmt(right, 8) + " = " + StudioMath.Fmt(res, 10);
                    _lblExpr.Text = StudioMath.Fmt(_acc, 8) + " " + _pendingOp + " " + StudioMath.Fmt(right, 8) + " =";
                    SetDisplayValue(StudioMath.Fmt(res, 10));
                    AddHistory(line);
                    _lastOp = _pendingOp;
                    _lastOperand = right;
                    _acc = res;
                    _pendingOp = "";
                    _freshEntry = true;
                }
                else if (!string.IsNullOrEmpty(_lastOp))
                {
                    double res = ApplyBinary(cur, _lastOperand, _lastOp);
                    _lblExpr.Text = StudioMath.Fmt(cur, 8) + " " + _lastOp + " " + StudioMath.Fmt(_lastOperand, 8) + " =";
                    SetDisplayValue(StudioMath.Fmt(res, 10));
                    _acc = res;
                    _freshEntry = true;
                }
            }
        }

        private static double ApplyBinary(double a, double b, string op)
        {
            if (op == "+") return a + b;
            if (op == "−") return a - b;
            if (op == "×") return a * b;
            if (op == "÷") return Math.Abs(b) < 1e-15 ? double.NaN : (a / b);
            return b;
        }

        // ====================================================================
        // TAB 1: GRAPHIC DESIGNER PRINT & DPI CONVERTER (px/mm/cm/in/pt/pc)
        // ====================================================================
        private Panel BuildPrintDpiTab()
        {
            Panel p = new Panel();

            // Section 1: DPI Selector
            Label lDpi = MakeSectionTitle("RESOLUTION (DPI / PPI)", 4, 6);
            p.Controls.Add(lDpi);

            int[] dpis = new int[] { 72, 96, 150, 300, 600 };
            _dpiPills = new Win11Button[dpis.Length];
            int dx = 4;
            for (int i = 0; i < dpis.Length; i++)
            {
                int val = dpis[i];
                Win11Button b = new Win11Button();
                b.Text = val.ToString();
                b.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                b.StyleKind = Win11BtnStyle.Operator;
                b.CornerRadius = 5;
                b.Bounds = new Rectangle(dx, 28, 58, 28);
                b.IsToggled = (val == 300);
                b.Click += delegate { SetDpi(val); };
                _dpiPills[i] = b;
                p.Controls.Add(b);
                dx += 62;
            }

            _txtDpi = MakeInputBox("300", 318, 28, 66);
            _txtDpi.TextChanged += delegate
            {
                double d;
                if (StudioMath.TryParseNum(_txtDpi.Text, out d) && d > 0 && d <= 9600)
                {
                    _dpi = d;
                    UpdateDpiPills();
                    double mm;
                    if (StudioMath.TryParseNum(_txtMm.Text, out mm))
                        UpdateAllUnitsFromInches(mm / 25.4, _txtMm);
                    RecalcCanvasSpecs();
                }
            };
            p.Controls.Add(_txtDpi);

            Label lDpiTag = MakeMutedLabel("DPI", 388, 32, 34);
            p.Controls.Add(lDpiTag);

            // Section 2: 6 Live Simultaneous Designer Units (2 columns x 3 rows)
            p.Controls.Add(MakeSectionTitle("INSTANT UNIT CONVERTER (type in any box)", 4, 66));

            _txtPx = AddUnitRow(p, "Pixels (px)", 4, 88, "px");
            _txtMm = AddUnitRow(p, "Millimeters (mm)", 218, 88, "mm");
            _txtCm = AddUnitRow(p, "Centimeters (cm)", 4, 138, "cm");
            _txtIn = AddUnitRow(p, "Inches (in)", 218, 138, "in");
            _txtPt = AddUnitRow(p, "Points (pt)", 4, 188, "pt");
            _txtPc = AddUnitRow(p, "Picas (pc)", 218, 188, "pc");

            // Section 3: 2D Print Canvas, Paper Presets, Bleed & Aspect Ratio Lock
            p.Controls.Add(MakeSectionTitle("PRINT CANVAS · PRESETS · BLEED · ASPECT SCALE", 4, 244));

            _cmbPreset = new ComboBox();
            _cmbPreset.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbPreset.Font = new Font("Segoe UI", 9.5f);
            _cmbPreset.Bounds = new Rectangle(4, 266, 290, 28);
            _cmbPreset.Items.AddRange(new object[] {
                "A4 — 210 × 297 mm",
                "A3 — 297 × 420 mm",
                "A5 — 148 × 210 mm",
                "A2 — 420 × 594 mm",
                "A1 — 594 × 841 mm",
                "A0 — 841 × 1189 mm",
                "Business Card — 85 × 55 mm",
                "Roll-Up Banner — 85 × 200 cm",
                "Instagram Post — 1080 × 1080 px",
                "Instagram Story — 1080 × 1920 px",
                "Full HD — 1920 × 1080 px",
                "4K UHD — 3840 × 2160 px"
            });
            _cmbPreset.SelectedIndex = 0;
            _cmbPreset.SelectedIndexChanged += delegate { ApplyCanvasPreset(); };
            p.Controls.Add(_cmbPreset);

            _cmbCanvasUnit = new ComboBox();
            _cmbCanvasUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbCanvasUnit.Font = new Font("Segoe UI", 9.5f);
            _cmbCanvasUnit.Bounds = new Rectangle(302, 266, 122, 28);
            _cmbCanvasUnit.Items.AddRange(new object[] { "mm", "cm", "in", "px" });
            _cmbCanvasUnit.SelectedIndex = 0;
            _cmbCanvasUnit.SelectedIndexChanged += delegate { RecalcCanvasSpecs(); };
            p.Controls.Add(_cmbCanvasUnit);

            p.Controls.Add(MakeMutedLabel("Width", 4, 302, 95));
            _txtCanvasW = MakeInputBox("210", 4, 320, 95);
            _txtCanvasW.TextChanged += delegate { RecalcCanvasSpecs(); };
            p.Controls.Add(_txtCanvasW);

            p.Controls.Add(MakeMutedLabel("Height", 107, 302, 95));
            _txtCanvasH = MakeInputBox("297", 107, 320, 95);
            _txtCanvasH.TextChanged += delegate { RecalcCanvasSpecs(); };
            p.Controls.Add(_txtCanvasH);

            p.Controls.Add(MakeMutedLabel("Bleed (mm)", 210, 302, 84));
            _txtBleed = MakeInputBox("3", 210, 320, 84);
            _txtBleed.TextChanged += delegate { RecalcCanvasSpecs(); };
            p.Controls.Add(_txtBleed);

            p.Controls.Add(MakeMutedLabel("Scale New W", 302, 302, 122));
            _txtScaleNewW = MakeInputBox("", 302, 320, 122);
            _txtScaleNewW.TextChanged += delegate { RecalcCanvasSpecs(); };
            p.Controls.Add(_txtScaleNewW);

            // Live Specs Output Card
            _lblCanvasSpecs = new Label();
            _lblCanvasSpecs.Bounds = new Rectangle(4, 362, 420, 160);
            _lblCanvasSpecs.Font = new Font("Consolas", 9.5f);
            _lblCanvasSpecs.Padding = new Padding(10);
            _lblCanvasSpecs.Tag = "CARD";
            p.Controls.Add(_lblCanvasSpecs);

            Win11Button btnCopySpecs = new Win11Button();
            btnCopySpecs.Text = "Copy Print Specs";
            btnCopySpecs.StyleKind = Win11BtnStyle.Accent;
            btnCopySpecs.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnCopySpecs.Bounds = new Rectangle(4, 530, 420, 36);
            btnCopySpecs.Click += delegate { CopyText(_lblCanvasSpecs.Text.Trim(), "Print Specs"); };
            p.Controls.Add(btnCopySpecs);

            return p;
        }

        private TextBox AddUnitRow(Panel parent, string label, int x, int y, string unit)
        {
            parent.Controls.Add(MakeMutedLabel(label, x, y, 200));
            TextBox tb = MakeInputBox("0", x, y + 18, 162);
            tb.TextChanged += delegate
            {
                if (_updatingUnits) return;
                double v;
                if (StudioMath.TryParseNum(tb.Text, out v))
                    UpdateAllUnitsFromInches(StudioMath.ToInches(v, unit, _dpi), tb);
            };
            parent.Controls.Add(tb);

            Win11Button cp = new Win11Button();
            cp.Text = "Cp";
            cp.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            cp.StyleKind = Win11BtnStyle.Operator;
            cp.CornerRadius = 4;
            cp.Bounds = new Rectangle(x + 166, y + 18, 36, 26);
            cp.Click += delegate { CopyText(tb.Text + " " + unit, unit); };
            parent.Controls.Add(cp);
            return tb;
        }

        private void SetDpi(int val)
        {
            _dpi = val;
            _txtDpi.Text = val.ToString();
            UpdateDpiPills();
        }

        private void UpdateDpiPills()
        {
            if (_dpiPills == null) return;
            for (int i = 0; i < _dpiPills.Length; i++)
            {
                _dpiPills[i].IsToggled = (_dpiPills[i].Text == ((int)Math.Round(_dpi)).ToString());
                _dpiPills[i].Invalidate();
            }
        }

        private void UpdateAllUnitsFromInches(double inches, TextBox source)
        {
            _updatingUnits = true;
            try
            {
                if (source != _txtPx) _txtPx.Text = StudioMath.Fmt(StudioMath.FromInches(inches, "px", _dpi), 1);
                if (source != _txtMm) _txtMm.Text = StudioMath.Fmt(StudioMath.FromInches(inches, "mm", _dpi), 2);
                if (source != _txtCm) _txtCm.Text = StudioMath.Fmt(StudioMath.FromInches(inches, "cm", _dpi), 3);
                if (source != _txtIn) _txtIn.Text = StudioMath.Fmt(inches, 4);
                if (source != _txtPt) _txtPt.Text = StudioMath.Fmt(StudioMath.FromInches(inches, "pt", _dpi), 2);
                if (source != _txtPc) _txtPc.Text = StudioMath.Fmt(StudioMath.FromInches(inches, "pc", _dpi), 3);
            }
            finally { _updatingUnits = false; }
        }

        private void ApplyCanvasPreset()
        {
            switch (_cmbPreset.SelectedIndex)
            {
                case 0: SetCanvas(210, 297, "mm", 3); break;
                case 1: SetCanvas(297, 420, "mm", 3); break;
                case 2: SetCanvas(148, 210, "mm", 3); break;
                case 3: SetCanvas(420, 594, "mm", 3); break;
                case 4: SetCanvas(594, 841, "mm", 5); break;
                case 5: SetCanvas(841, 1189, "mm", 5); break;
                case 6: SetCanvas(85, 55, "mm", 3); break;
                case 7: SetCanvas(85, 200, "cm", 5); break;
                case 8: SetCanvas(1080, 1080, "px", 0); break;
                case 9: SetCanvas(1080, 1920, "px", 0); break;
                case 10: SetCanvas(1920, 1080, "px", 0); break;
                case 11: SetCanvas(3840, 2160, "px", 0); break;
            }
        }

        private void SetCanvas(double w, double h, string unit, double bleed)
        {
            _cmbCanvasUnit.SelectedItem = unit;
            _txtCanvasW.Text = StudioMath.Fmt(w, 1);
            _txtCanvasH.Text = StudioMath.Fmt(h, 1);
            _txtBleed.Text = StudioMath.Fmt(bleed, 1);
            RecalcCanvasSpecs();
        }

        private void RecalcCanvasSpecs()
        {
            double w, h, bleed;
            if (!StudioMath.TryParseNum(_txtCanvasW.Text, out w) || !StudioMath.TryParseNum(_txtCanvasH.Text, out h) || w <= 0 || h <= 0)
            {
                _lblCanvasSpecs.Text = "Enter valid Width && Height...";
                return;
            }
            if (!StudioMath.TryParseNum(_txtBleed.Text, out bleed) || bleed < 0) bleed = 0;

            string u = _cmbCanvasUnit.SelectedItem != null ? _cmbCanvasUnit.SelectedItem.ToString() : "mm";
            double wIn = StudioMath.ToInches(w, u, _dpi);
            double hIn = StudioMath.ToInches(h, u, _dpi);
            double wMm = wIn * 25.4, hMm = hIn * 25.4;
            int wPx = (int)Math.Round(wIn * _dpi);
            int hPx = (int)Math.Round(hIn * _dpi);

            double bwMm = wMm + bleed * 2.0, bhMm = hMm + bleed * 2.0;
            int bwPx = (int)Math.Round((bwMm / 25.4) * _dpi);
            int bhPx = (int)Math.Round((bhMm / 25.4) * _dpi);
            double mp = ((double)wPx * hPx) / 1000000.0;
            double cmykMb = ((double)wPx * hPx * 4.0) / (1024.0 * 1024.0);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Pixels @ {0:0} DPI :  {1} × {2} px  ({3:0.00} MP)", _dpi, wPx, hPx, mp));
            sb.AppendLine(string.Format("Trim Size       :  {0} × {1} mm  ({2} × {3} in)", StudioMath.Fmt(wMm, 1), StudioMath.Fmt(hMm, 1), StudioMath.Fmt(wIn, 2), StudioMath.Fmt(hIn, 2)));
            if (bleed > 0)
                sb.AppendLine(string.Format("+ {0:0.#}mm Bleed   :  {1} × {2} mm  ({3} × {4} px)", bleed, StudioMath.Fmt(bwMm, 1), StudioMath.Fmt(bhMm, 1), bwPx, bhPx));
            sb.AppendLine(string.Format("Aspect Ratio    :  {0}   |   CMYK Raw ~{1:0.0} MB", StudioMath.FormatAspectRatio(wIn, hIn), cmykMb));

            double newW;
            if (StudioMath.TryParseNum(_txtScaleNewW.Text, out newW) && newW > 0)
            {
                double newH = newW * (h / w);
                sb.AppendLine(string.Format("Scaled ({0})      :  {1} × {2} {0}  (locked ratio)", u, StudioMath.Fmt(newW, 2), StudioMath.Fmt(newH, 2)));
            }
            _lblCanvasSpecs.Text = sb.ToString();
        }

        // ====================================================================
        // TAB 2: CMYK & COLOR PICKER STUDIO (Screen Pipette + TAC% + Swatches)
        // ====================================================================
        private Panel BuildColorTab()
        {
            Panel p = new Panel();

            // Big live Swatch Banner
            _swatchBanner = new Panel();
            _swatchBanner.Bounds = new Rectangle(4, 6, 420, 92);
            _swatchBanner.Cursor = Cursors.Hand;
            _swatchBanner.Click += delegate { CopyText(StudioMath.ToHex(_curColor), "HEX"); };

            _lblSwatchHex = new Label();
            _lblSwatchHex.Bounds = new Rectangle(14, 10, 210, 38);
            _lblSwatchHex.Font = new Font("Consolas", 22f, FontStyle.Bold);
            _lblSwatchHex.BackColor = Color.Transparent;
            _lblSwatchHex.Click += delegate { CopyText(StudioMath.ToHex(_curColor), "HEX"); };

            _lblSwatchWcag = new Label();
            _lblSwatchWcag.Bounds = new Rectangle(16, 54, 240, 24);
            _lblSwatchWcag.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            _lblSwatchWcag.BackColor = Color.Transparent;

            _lblSwatchTac = new Label();
            _lblSwatchTac.Bounds = new Rectangle(228, 16, 180, 60);
            _lblSwatchTac.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            _lblSwatchTac.TextAlign = ContentAlignment.MiddleRight;
            _lblSwatchTac.BackColor = Color.Transparent;

            _swatchBanner.Controls.Add(_lblSwatchHex);
            _swatchBanner.Controls.Add(_lblSwatchWcag);
            _swatchBanner.Controls.Add(_lblSwatchTac);
            p.Controls.Add(_swatchBanner);

            // Screen Pipette + Color Wheel Buttons
            Win11Button btnPick = new Win11Button();
            btnPick.Text = "Pick Color from Screen (Ctrl+Shift+C)";
            btnPick.StyleKind = Win11BtnStyle.Accent;
            btnPick.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnPick.Bounds = new Rectangle(4, 106, 278, 36);
            btnPick.Click += delegate { LaunchScreenPipette(); };
            p.Controls.Add(btnPick);

            Win11Button btnWheel = new Win11Button();
            btnWheel.Text = "Color Wheel...";
            btnWheel.StyleKind = Win11BtnStyle.Operator;
            btnWheel.Font = new Font("Segoe UI", 9.5f);
            btnWheel.Bounds = new Rectangle(288, 106, 136, 36);
            btnWheel.Click += delegate
            {
                using (ColorDialog dlg = new ColorDialog())
                {
                    dlg.FullOpen = true;
                    dlg.Color = _curColor;
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                        SetStudioColor(dlg.Color, true);
                }
            };
            p.Controls.Add(btnWheel);

            // HEX row
            p.Controls.Add(MakeSectionTitle("HEX", 4, 152));
            _txtHex = MakeInputBox("#FD971F", 4, 172, 290);
            _txtHex.TextChanged += delegate
            {
                if (_updatingColor) return;
                Color c;
                if (StudioMath.TryParseHex(_txtHex.Text, out c)) SetStudioColor(c, false, _txtHex);
            };
            p.Controls.Add(_txtHex);
            p.Controls.Add(MakeCopyBtn("Copy HEX", 300, 172, 124, delegate { CopyText(StudioMath.ToHex(_curColor), "HEX"); }));

            // RGB row
            p.Controls.Add(MakeSectionTitle("RGB (0 – 255)", 4, 210));
            _txtR = MakeInputBox("253", 4, 230, 92);
            _txtG = MakeInputBox("151", 102, 230, 92);
            _txtB = MakeInputBox("31", 200, 230, 94);
            EventHandler onRgb = delegate
            {
                if (_updatingColor) return;
                int r, g, b;
                if (int.TryParse(_txtR.Text, out r) && int.TryParse(_txtG.Text, out g) && int.TryParse(_txtB.Text, out b))
                    SetStudioColor(Color.FromArgb(StudioMath.ClampByte(r), StudioMath.ClampByte(g), StudioMath.ClampByte(b)), false, _txtR);
            };
            _txtR.TextChanged += onRgb; _txtG.TextChanged += onRgb; _txtB.TextChanged += onRgb;
            p.Controls.Add(_txtR); p.Controls.Add(_txtG); p.Controls.Add(_txtB);
            p.Controls.Add(MakeCopyBtn("Copy rgb()", 300, 230, 124, delegate { CopyText("rgb(" + _curColor.R + ", " + _curColor.G + ", " + _curColor.B + ")", "RGB"); }));

            // CMYK row (Prepress Print!)
            p.Controls.Add(MakeSectionTitle("CMYK % (PRINT / PREPRESS)", 4, 268));
            _txtC = MakeInputBox("0", 4, 288, 68);
            _txtM = MakeInputBox("40", 78, 288, 68);
            _txtY = MakeInputBox("88", 152, 288, 68);
            _txtK = MakeInputBox("1", 226, 288, 68);
            EventHandler onCmyk = delegate
            {
                if (_updatingColor) return;
                int c, m, y, k;
                if (int.TryParse(_txtC.Text, out c) && int.TryParse(_txtM.Text, out m) && int.TryParse(_txtY.Text, out y) && int.TryParse(_txtK.Text, out k))
                    SetStudioColor(StudioMath.CmykToRgb(c, m, y, k), false, _txtC);
            };
            _txtC.TextChanged += onCmyk; _txtM.TextChanged += onCmyk; _txtY.TextChanged += onCmyk; _txtK.TextChanged += onCmyk;
            p.Controls.Add(_txtC); p.Controls.Add(_txtM); p.Controls.Add(_txtY); p.Controls.Add(_txtK);
            p.Controls.Add(MakeCopyBtn("Copy CMYK", 300, 288, 124, delegate { CopyText(string.Format("cmyk({0}%, {1}%, {2}%, {3}%)", _txtC.Text, _txtM.Text, _txtY.Text, _txtK.Text), "CMYK"); }));

            // HSL row
            p.Controls.Add(MakeSectionTitle("HSL (H° · S% · L%)", 4, 326));
            _txtH = MakeInputBox("33", 4, 346, 92);
            _txtS = MakeInputBox("98", 102, 346, 92);
            _txtL = MakeInputBox("56", 200, 346, 94);
            EventHandler onHsl = delegate
            {
                if (_updatingColor) return;
                int h, s, l;
                if (int.TryParse(_txtH.Text, out h) && int.TryParse(_txtS.Text, out s) && int.TryParse(_txtL.Text, out l))
                    SetStudioColor(StudioMath.HslToRgb(h, s, l), false, _txtH);
            };
            _txtH.TextChanged += onHsl; _txtS.TextChanged += onHsl; _txtL.TextChanged += onHsl;
            p.Controls.Add(_txtH); p.Controls.Add(_txtS); p.Controls.Add(_txtL);
            p.Controls.Add(MakeCopyBtn("Copy hsl()", 300, 346, 124, delegate { CopyText(string.Format("hsl({0}, {1}%, {2}%)", _txtH.Text, _txtS.Text, _txtL.Text), "HSL"); }));

            // 5 Tints & Shades
            p.Controls.Add(MakeSectionTitle("TINTS & SHADES (click to select)", 4, 392));
            for (int i = 0; i < 5; i++)
            {
                Panel sb = new Panel();
                sb.Bounds = new Rectangle(4 + i * 85, 414, 80, 44);
                sb.Cursor = Cursors.Hand;
                sb.Click += delegate(object s, EventArgs ev) { SetStudioColor(((Panel)s).BackColor, true); CopyText(StudioMath.ToHex(((Panel)s).BackColor), "Shade"); };
                _shadeBoxes[i] = sb;
                p.Controls.Add(sb);
            }

            // 8 Recent Swatches
            p.Controls.Add(MakeSectionTitle("RECENT COLORS", 4, 474));
            for (int i = 0; i < 8; i++)
            {
                Panel rb = new Panel();
                rb.Bounds = new Rectangle(4 + i * 53, 496, 48, 38);
                rb.Cursor = Cursors.Hand;
                rb.Click += delegate(object s, EventArgs ev)
                {
                    Panel clicked = (Panel)s;
                    if (clicked.Tag is Color) SetStudioColor((Color)clicked.Tag, false);
                };
                _recentBoxes[i] = rb;
                p.Controls.Add(rb);
            }

            return p;
        }

        private void LaunchScreenPipette()
        {
            bool wasTop = this.TopMost;
            this.Hide();
            Thread.Sleep(120);
            Color picked = Color.Empty;
            bool ok = false;
            try
            {
                using (ScreenEyedropperForm eye = new ScreenEyedropperForm())
                {
                    eye.ShowDialog();
                    if (eye.DidPick) { picked = eye.PickedColor; ok = true; }
                }
            }
            catch (Exception ex) { ShowToast("Pipette error: " + ex.Message); }
            finally
            {
                this.Show();
                this.TopMost = wasTop;
                this.Activate();
            }

            if (ok)
            {
                SwitchTab(2);
                SetStudioColor(picked, true);
                CopyText(StudioMath.ToHex(picked), "Picked HEX");
            }
        }

        private void SetStudioColor(Color col, bool addRecent) { SetStudioColor(col, addRecent, null); }

        private void SetStudioColor(Color col, bool addRecent, TextBox source)
        {
            _curColor = col;
            _updatingColor = true;
            try
            {
                int c, m, y, k, h, s, l;
                StudioMath.RgbToCmyk(col.R, col.G, col.B, out c, out m, out y, out k);
                StudioMath.RgbToHsl(col.R, col.G, col.B, out h, out s, out l);
                int tac = c + m + y + k;

                _swatchBanner.BackColor = col;
                double lum = StudioMath.RelativeLuminance(col);
                Color fg = lum > 0.38 ? Color.FromArgb(20, 20, 20) : Color.White;
                _lblSwatchHex.ForeColor = fg;
                _lblSwatchWcag.ForeColor = fg;
                _lblSwatchTac.ForeColor = fg;

                _lblSwatchHex.Text = StudioMath.ToHex(col);
                _lblSwatchWcag.Text = (lum > 0.38 ? "Best text: BLACK" : "Best text: WHITE") + "  (click to copy)";
                _lblSwatchTac.Text = "CMYK " + c + "/" + m + "/" + y + "/" + k + "%\nTotal Ink (TAC): " + tac + "%" + (tac > 300 ? " !" : " OK");

                if (source != _txtHex) _txtHex.Text = StudioMath.ToHex(col);
                if (source != _txtR) { _txtR.Text = col.R.ToString(); _txtG.Text = col.G.ToString(); _txtB.Text = col.B.ToString(); }
                if (source != _txtC) { _txtC.Text = c.ToString(); _txtM.Text = m.ToString(); _txtY.Text = y.ToString(); _txtK.Text = k.ToString(); }
                if (source != _txtH) { _txtH.Text = h.ToString(); _txtS.Text = s.ToString(); _txtL.Text = l.ToString(); }

                // 5 shades from lighter to darker
                int[] lOffsets = new int[] { 26, 13, 0, -13, -26 };
                for (int i = 0; i < 5; i++)
                {
                    int nl = Math.Max(4, Math.Min(96, l + lOffsets[i]));
                    _shadeBoxes[i].BackColor = StudioMath.HslToRgb(h, s, nl);
                }

                if (addRecent)
                {
                    _recentColors.RemoveAll(delegate(Color x) { return x.ToArgb() == col.ToArgb(); });
                    _recentColors.Insert(0, col);
                    if (_recentColors.Count > 8) _recentColors.RemoveAt(_recentColors.Count - 1);
                }
                for (int i = 0; i < 8; i++)
                {
                    if (i < _recentColors.Count)
                    {
                        _recentBoxes[i].BackColor = _recentColors[i];
                        _recentBoxes[i].Tag = _recentColors[i];
                        _recentBoxes[i].Visible = true;
                    }
                    else _recentBoxes[i].Visible = false;
                }
            }
            finally { _updatingColor = false; }
        }

        // ====================================================================
        // TAB 3: GREEK VAT (ΦΠΑ 24%/13%/6%) & m2 MATERIAL PRICING
        // ====================================================================
        private Panel BuildVatM2Tab()
        {
            Panel p = new Panel();

            // Section A: Greek VAT
            p.Controls.Add(MakeSectionTitle("ΥΠΟΛΟΓΙΣΤΗΣ ΦΠΑ ΕΛΛΑΔΑΣ (Καθαρή ↔ Τελική)", 4, 6));

            double[] rates = new double[] { 24, 13, 6, 17 };
            int rx = 4;
            for (int i = 0; i < rates.Length; i++)
            {
                double rv = rates[i];
                Win11Button rb = new Win11Button();
                rb.Text = rv.ToString("0") + "%";
                rb.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
                rb.StyleKind = Win11BtnStyle.Operator;
                rb.CornerRadius = 5;
                rb.Bounds = new Rectangle(rx, 28, 66, 28);
                rb.Click += delegate { _vatRate = rv; _txtVatCustom.Text = rv.ToString("0"); RecalcVatFromNet(); };
                p.Controls.Add(rb);
                rx += 70;
            }
            _txtVatCustom = MakeInputBox("24", 290, 28, 64);
            _txtVatCustom.TextChanged += delegate
            {
                double r;
                if (StudioMath.TryParseNum(_txtVatCustom.Text, out r) && r >= 0 && r <= 100)
                { _vatRate = r; RecalcVatFromNet(); }
            };
            p.Controls.Add(_txtVatCustom);
            p.Controls.Add(MakeMutedLabel("% ΦΠΑ", 358, 32, 64));

            p.Controls.Add(MakeMutedLabel("Καθαρή Αξία (€)", 4, 64, 134));
            _txtVatNet = MakeInputBox("100", 4, 82, 134);
            _txtVatNet.TextChanged += delegate { if (!_updatingVat) RecalcVatFromNet(); };
            p.Controls.Add(_txtVatNet);

            p.Controls.Add(MakeMutedLabel("Τελική με ΦΠΑ (€)", 146, 64, 134));
            _txtVatGross = MakeInputBox("124", 146, 82, 134);
            _txtVatGross.TextChanged += delegate { if (!_updatingVat) RecalcVatFromGross(); };
            p.Controls.Add(_txtVatGross);

            p.Controls.Add(MakeMutedLabel("Έκπτωση (%)", 288, 64, 136));
            _txtVatDisc = MakeInputBox("0", 288, 82, 136);
            _txtVatDisc.TextChanged += delegate { if (!_updatingVat) RecalcVatFromNet(); };
            p.Controls.Add(_txtVatDisc);

            _lblVatResult = new Label();
            _lblVatResult.Bounds = new Rectangle(4, 118, 420, 74);
            _lblVatResult.Font = new Font("Consolas", 9.8f);
            _lblVatResult.Padding = new Padding(10);
            _lblVatResult.Tag = "CARD";
            p.Controls.Add(_lblVatResult);

            p.Controls.Add(MakeCopyBtn("Αντιγραφή Σύνοψης ΦΠΑ", 4, 198, 420, delegate { CopyText(_lblVatResult.Text.Replace("\r\n", " | ").Replace("\n", " | ").Trim(), "ΦΠΑ"); }));

            // Section B: m2 & Material Costing
            p.Controls.Add(MakeSectionTitle("ΥΠΟΛΟΓΙΣΤΗΣ m² & ΚΟΣΤΟΥΣ ΥΛΙΚΟΥ (Εκτυπώσεις / Επιγραφές)", 4, 248));

            p.Controls.Add(MakeMutedLabel("Πλάτος", 4, 270, 92));
            _txtM2W = MakeInputBox("200", 4, 288, 92);
            _txtM2W.TextChanged += delegate { RecalcM2(); };
            p.Controls.Add(_txtM2W);

            p.Controls.Add(MakeMutedLabel("Ύψος", 104, 270, 92));
            _txtM2H = MakeInputBox("100", 104, 288, 92);
            _txtM2H.TextChanged += delegate { RecalcM2(); };
            p.Controls.Add(_txtM2H);

            p.Controls.Add(MakeMutedLabel("Μονάδα", 204, 270, 68));
            _cmbM2Unit = new ComboBox();
            _cmbM2Unit.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbM2Unit.Font = new Font("Segoe UI", 9.5f);
            _cmbM2Unit.Bounds = new Rectangle(204, 288, 68, 26);
            _cmbM2Unit.Items.AddRange(new object[] { "cm", "mm", "m" });
            _cmbM2Unit.SelectedIndex = 0;
            _cmbM2Unit.SelectedIndexChanged += delegate { RecalcM2(); };
            p.Controls.Add(_cmbM2Unit);

            p.Controls.Add(MakeMutedLabel("Τεμ.", 280, 270, 56));
            _txtM2Qty = MakeInputBox("1", 280, 288, 56);
            _txtM2Qty.TextChanged += delegate { RecalcM2(); };
            p.Controls.Add(_txtM2Qty);

            p.Controls.Add(MakeMutedLabel("€/m²", 344, 270, 80));
            _txtM2Price = MakeInputBox("25", 344, 288, 80);
            _txtM2Price.TextChanged += delegate { RecalcM2(); };
            p.Controls.Add(_txtM2Price);

            _chkM2Vat = new CheckBox();
            _chkM2Vat.Text = "Προσθήκη ΦΠΑ 24% στο τελικό σύνολο";
            _chkM2Vat.Checked = true;
            _chkM2Vat.Font = new Font("Segoe UI", 9f);
            _chkM2Vat.Bounds = new Rectangle(4, 322, 340, 24);
            _chkM2Vat.CheckedChanged += delegate { RecalcM2(); };
            p.Controls.Add(_chkM2Vat);

            _lblM2Result = new Label();
            _lblM2Result.Bounds = new Rectangle(4, 352, 420, 146);
            _lblM2Result.Font = new Font("Consolas", 9.8f);
            _lblM2Result.Padding = new Padding(10);
            _lblM2Result.Tag = "CARD";
            p.Controls.Add(_lblM2Result);

            Win11Button btnCopyM2 = new Win11Button();
            btnCopyM2.Text = "Αντιγραφή Προσφοράς m² (για Viber / Email)";
            btnCopyM2.StyleKind = Win11BtnStyle.Accent;
            btnCopyM2.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnCopyM2.Bounds = new Rectangle(4, 508, 420, 38);
            btnCopyM2.Click += delegate { CopyText(_lblM2Result.Text.Trim(), "Προσφορά m²"); };
            p.Controls.Add(btnCopyM2);

            return p;
        }

        private void RecalcVatFromNet()
        {
            double net, disc;
            if (!StudioMath.TryParseNum(_txtVatNet.Text, out net)) return;
            if (!StudioMath.TryParseNum(_txtVatDisc.Text, out disc) || disc < 0) disc = 0;
            double discounted = net * (1.0 - disc / 100.0);
            double vat = discounted * (_vatRate / 100.0);
            double gross = discounted + vat;

            _updatingVat = true;
            try { _txtVatGross.Text = StudioMath.Fmt(gross, 2); }
            finally { _updatingVat = false; }

            _lblVatResult.Text = string.Format(
                "Καθαρή Αξία : {0:0.00} €{1}\nΦΠΑ ({2:0.#}%)   : {3:0.00} €\nΤΕΛΙΚΗ ΤΙΜΗ : {4:0.00} €",
                discounted, disc > 0 ? string.Format(" (-{0:0.#}%)", disc) : "", _vatRate, vat, gross);
        }

        private void RecalcVatFromGross()
        {
            double gross;
            if (!StudioMath.TryParseNum(_txtVatGross.Text, out gross)) return;
            double net = gross / (1.0 + _vatRate / 100.0);
            double vat = gross - net;

            _updatingVat = true;
            try { _txtVatNet.Text = StudioMath.Fmt(net, 2); _txtVatDisc.Text = "0"; }
            finally { _updatingVat = false; }

            _lblVatResult.Text = string.Format(
                "Καθαρή Αξία : {0:0.00} €  (Αποφορολόγηση)\nΦΠΑ ({1:0.#}%)   : {2:0.00} €\nΤΕΛΙΚΗ ΤΙΜΗ : {3:0.00} €",
                net, _vatRate, vat, gross);
        }

        private void RecalcM2()
        {
            double w, h, qty, price;
            if (!StudioMath.TryParseNum(_txtM2W.Text, out w) || !StudioMath.TryParseNum(_txtM2H.Text, out h) || w <= 0 || h <= 0)
            {
                _lblM2Result.Text = "Εισάγετε διαστάσεις Πλάτος × Ύψος...";
                return;
            }
            if (!StudioMath.TryParseNum(_txtM2Qty.Text, out qty) || qty <= 0) qty = 1;
            if (!StudioMath.TryParseNum(_txtM2Price.Text, out price) || price < 0) price = 0;

            string u = _cmbM2Unit.SelectedItem != null ? _cmbM2Unit.SelectedItem.ToString() : "cm";
            double mult = (u == "mm") ? 0.001 : ((u == "cm") ? 0.01 : 1.0);
            double wm = w * mult, hm = h * mult;
            double areaEach = wm * hm;
            double totalArea = areaEach * qty;
            double perim = 2.0 * (wm + hm) * qty;
            double net = totalArea * price;
            double vat = _chkM2Vat.Checked ? (net * 0.24) : 0;
            double gross = net + vat;

            _lblM2Result.Text = string.Format(
                "Διάσταση      : {0} × {1} {2}  (× {3:0} τεμ.)\n" +
                "Εμβαδόν       : {4:0.000} m²  (Περίμετρος: {5:0.00} m)\n" +
                "Καθαρό Κόστος : {6:0.00} €  (@ {7:0.00} €/m²)\n" +
                "ΦΠΑ (24%)     : {8:0.00} €\n" +
                "ΤΕΛΙΚΟ ΣΥΝΟΛΟ : {9:0.00} €",
                StudioMath.Fmt(w, 1), StudioMath.Fmt(h, 1), u, qty, totalArea, perim, net, price, vat, gross);
        }

        // ====================================================================
        // HELPERS, KEYBOARD, THEME & SETTINGS
        // ====================================================================
        private Label MakeSectionTitle(string text, int x, int y)
        {
            Label l = new Label();
            l.Text = text;
            l.Bounds = new Rectangle(x, y, 420, 18);
            l.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
            l.Tag = "MUTED";
            return l;
        }

        private Label MakeMutedLabel(string text, int x, int y, int w)
        {
            Label l = new Label();
            l.Text = text;
            l.Bounds = new Rectangle(x, y, w, 16);
            l.Font = new Font("Segoe UI", 8.2f);
            l.Tag = "MUTED";
            return l;
        }

        private TextBox MakeInputBox(string def, int x, int y, int w)
        {
            TextBox tb = new TextBox();
            tb.Text = def;
            tb.Bounds = new Rectangle(x, y, w, 26);
            tb.BorderStyle = BorderStyle.FixedSingle;
            tb.Font = new Font("Consolas", 10.5f);
            return tb;
        }

        private Win11Button MakeCopyBtn(string text, int x, int y, int w, Action onClick)
        {
            Win11Button b = new Win11Button();
            b.Text = text;
            b.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            b.StyleKind = Win11BtnStyle.Operator;
            b.CornerRadius = 4;
            b.Bounds = new Rectangle(x, y, w, 26);
            b.Click += delegate { if (onClick != null) onClick(); };
            return b;
        }

        public void SwitchTab(int idx)
        {
            if (idx < 0 || idx > 3) return;
            _activeTab = idx;
            for (int i = 0; i < 4; i++)
            {
                _tabPanels[i].Visible = (i == idx);
                _tabBtns[i].Active = (i == idx);
                _tabBtns[i].Invalidate();
            }
        }

        private void TogglePin()
        {
            _pinned = !_pinned;
            this.TopMost = _pinned;
            _btnPin.Active = _pinned;
            _btnPin.Invalidate();
            ShowToast(_pinned ? "Pinned: Always on Top" : "Unpinned");
        }

        public void ApplyTheme(CalcTheme t)
        {
            _theme = t;
            this.BackColor = t.WindowBg;
            this.ForeColor = t.TextPrimary;
            if (this.IsHandleCreated) Native.SetDarkTitleBar(this.Handle, t.IsDark);
            _btnTheme.Icon = t.IsDark ? AkIcon.Sun : AkIcon.Moon;
            _btnPin.Active = _pinned;
            _lblToast.BackColor = t.CardBg;
            _lblToast.ForeColor = t.TextMuted;
            _topBar.BackColor = t.CardBg;
            _contentHost.BackColor = t.WindowBg;
            _historyOverlay.BackColor = t.CardBg;
            _historyList.BackColor = t.CardBg;
            _historyList.ForeColor = t.TextPrimary;
            _lblMemBadge.ForeColor = t.StudioAccent;
            _lblExpr.ForeColor = t.TextMuted;
            _lblDisp.ForeColor = t.TextPrimary;

            ApplyThemeRecursive(this, t);
            _topBar.BackColor = t.CardBg;
            _topBar.Invalidate();
            SetStudioColor(_curColor, false);
        }

        private void ApplyThemeRecursive(Control parent, CalcTheme t)
        {
            foreach (Control c in parent.Controls)
            {
                if (c == _swatchBanner) continue;
                if (c is AkButton)
                {
                    ((AkButton)c).Dark = t.IsDark;
                    c.Invalidate();
                }
                else if (c is Win11Button)
                {
                    ((Win11Button)c).Theme = t;
                    c.Invalidate();
                }
                else if (c is TextBox || c is ComboBox)
                {
                    c.BackColor = t.InputBg;
                    c.ForeColor = t.TextPrimary;
                }
                else if (c is Label)
                {
                    string tag = c.Tag as string;
                    if (tag == "CARD")
                    {
                        c.BackColor = t.CardBg;
                        c.ForeColor = t.TextPrimary;
                    }
                    else if (tag == "MUTED")
                    {
                        c.ForeColor = t.TextMuted;
                    }
                    else if (c != _lblMemBadge && c != _lblExpr && c != _lblToast)
                    {
                        c.ForeColor = t.TextPrimary;
                    }
                }
                else if (c is CheckBox)
                {
                    c.ForeColor = t.TextPrimary;
                }
                else if (c is Panel && c != _historyOverlay)
                {
                    if (Array.IndexOf(_shadeBoxes, c) < 0 && Array.IndexOf(_recentBoxes, c) < 0)
                    {
                        c.BackColor = t.WindowBg;
                        ApplyThemeRecursive(c, t);
                    }
                }
            }
        }

        private void OnGlobalKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.C) { LaunchScreenPipette(); e.Handled = true; return; }
            if (e.Control && e.KeyCode == Keys.T) { TogglePin(); e.Handled = true; return; }
            if (e.Control && e.KeyCode == Keys.D1) { SwitchTab(0); e.Handled = true; return; }
            if (e.Control && e.KeyCode == Keys.D2) { SwitchTab(1); e.Handled = true; return; }
            if (e.Control && e.KeyCode == Keys.D3) { SwitchTab(2); e.Handled = true; return; }
            if (e.Control && e.KeyCode == Keys.D4) { SwitchTab(3); e.Handled = true; return; }

            // Standard calculator keyboard input ONLY when Tab 0 is active
            if (_activeTab != 0) return;

            if (e.Control && e.KeyCode == Keys.C) { CopyText(_lblDisp.Text, "Result"); e.Handled = true; return; }
            if (e.Control && e.KeyCode == Keys.V)
            {
                try
                {
                    double v;
                    if (Clipboard.ContainsText() && StudioMath.TryParseNum(Clipboard.GetText(), out v))
                    { SetDisplayValue(StudioMath.Fmt(v, 10)); _freshEntry = false; }
                }
                catch { }
                e.Handled = true; return;
            }

            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 && !e.Shift) { HandleCalcKey(((int)(e.KeyCode - Keys.D0)).ToString()); e.Handled = true; }
            else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) { HandleCalcKey(((int)(e.KeyCode - Keys.NumPad0)).ToString()); e.Handled = true; }
            else if (e.KeyCode == Keys.Add || (e.Shift && e.KeyCode == Keys.Oemplus)) { HandleCalcKey("+"); e.Handled = true; }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus) { HandleCalcKey("−"); e.Handled = true; }
            else if (e.KeyCode == Keys.Multiply || (e.Shift && e.KeyCode == Keys.D8)) { HandleCalcKey("×"); e.Handled = true; }
            else if (e.KeyCode == Keys.Divide || e.KeyCode == Keys.OemQuestion) { HandleCalcKey("÷"); e.Handled = true; }
            else if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Oemcomma) { HandleCalcKey("."); e.Handled = true; }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Oemplus) { HandleCalcKey("="); e.Handled = true; }
            else if (e.KeyCode == Keys.Back) { HandleCalcKey("⌫"); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape) { HandleCalcKey("C"); e.Handled = true; }
            else if (e.KeyCode == Keys.Delete) { HandleCalcKey("CE"); e.Handled = true; }
            else if (e.Shift && e.KeyCode == Keys.D5) { HandleCalcKey("%"); e.Handled = true; }
        }

        private void LoadSettings()
        {
            try
            {
                string suiteTheme = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"NillySuite\suite.theme");
                bool hasLocal = File.Exists(SettingsPath);
                if (hasLocal)
                {
                    string[] lines = File.ReadAllLines(SettingsPath, Encoding.UTF8);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        int eq = lines[i].IndexOf('=');
                        if (eq <= 0) continue;
                        string k = lines[i].Substring(0, eq).Trim();
                        string v = lines[i].Substring(eq + 1).Trim();
                        if (k == "Theme" && v == "Light") _theme = CalcTheme.Win11Light();
                        else if (k == "Pin" && v == "1") { _pinned = true; this.TopMost = true; _btnPin.Active = true; }
                    }
                }
                if (File.Exists(suiteTheme) && (!hasLocal || File.GetLastWriteTimeUtc(suiteTheme) > File.GetLastWriteTimeUtc(SettingsPath)))
                {
                    string s = File.ReadAllText(suiteTheme, Encoding.ASCII).Trim();
                    if (s.Equals("light", StringComparison.OrdinalIgnoreCase)) _theme = CalcTheme.Win11Light();
                    else if (s.Equals("dark", StringComparison.OrdinalIgnoreCase)) _theme = CalcTheme.Win11Dark();
                }
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                File.WriteAllLines(SettingsPath, new string[] {
                    "Theme=" + (_theme.IsDark ? "Dark" : "Light"),
                    "Pin=" + (_pinned ? "1" : "0")
                }, Encoding.UTF8);
            }
            catch { }
        }
    }

    // ========================================================================
    // 7. ENTRY POINT & AUTOMATED QA SMOKE TEST
    // ========================================================================
    internal static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            try { Native.SetProcessDPIAware(); } catch { }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args != null && args.Length > 0 && args[0] == "--test-smoke")
                return RunSmokeTest(args.Length > 1 ? args[1] : null);

            string initHex = null;
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].Equals("--hex", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) { initHex = args[i + 1]; break; }
                    if (args[i].StartsWith("#") && (args[i].Length == 4 || args[i].Length == 7)) { initHex = args[i]; break; }
                }
            }

            bool createdNew;
            using (Mutex m = new Mutex(true, "NillyCalcPro_SingleInstance_" + Environment.UserName.Replace(" ", "_"), out createdNew))
            {
                if (!createdNew && string.IsNullOrEmpty(initHex))
                {
                    IntPtr h = Native.FindWindow(null, "NillyCalc Pro");
                    if (h != IntPtr.Zero) { Native.ShowWindow(h, Native.SW_RESTORE); Native.SetForegroundWindow(h); }
                    return 0;
                }
                MainForm mf = new MainForm(initHex);
                mf.Shown += delegate { Native.TrimMemory(); };
                Application.Run(SuiteUpdater.Attach(mf, "nillycalc-pro", "NillyCalc-Pro", "Install_NillyCalcPro.ps1"));
            }
            return 0;
        }

        private static int RunSmokeTest(string logPath)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                // 1. Print & DPI math checks
                double a4wPx = Math.Round(StudioMath.FromInches(StudioMath.ToInches(210.0, "mm", 300), "px", 300));
                double a4hPx = Math.Round(StudioMath.FromInches(StudioMath.ToInches(297.0, "mm", 300), "px", 300));
                double inToPt = Math.Round(StudioMath.FromInches(1.0, "pt", 300));
                double inToPc = Math.Round(StudioMath.FromInches(1.0, "pc", 300));
                sb.AppendLine("A4_300DPI_OK=" + (a4wPx == 2480 && a4hPx == 3508) + " (" + a4wPx + "x" + a4hPx + ")");
                sb.AppendLine("IN_TO_PT_PC_OK=" + (inToPt == 72 && inToPc == 6));

                // 2. Color HEX <-> RGB <-> CMYK <-> HSL checks
                Color c;
                bool hexOk = StudioMath.TryParseHex("#FD971F", out c);
                int cc, cm, cy, ck, ch, cs, cl;
                StudioMath.RgbToCmyk(c.R, c.G, c.B, out cc, out cm, out cy, out ck);
                StudioMath.RgbToHsl(c.R, c.G, c.B, out ch, out cs, out cl);
                Color back = StudioMath.CmykToRgb(cc, cm, cy, ck);
                sb.AppendLine("COLOR_CMYK_OK=" + (hexOk && cc == 0 && cm == 40 && cy == 88 && ck == 1 && Math.Abs(back.R - c.R) <= 3));
                sb.AppendLine("COLOR_HSL=" + ch + "," + cs + "%," + cl + "%");

                // 3. UI Form + All 4 Tabs + Themes + RAM check
                using (MainForm form = new MainForm())
                {
                    form.Show();
                    for (int t = 0; t < 4; t++) { form.SwitchTab(t); Application.DoEvents(); }
                    form.SwitchTab(0);
                    form.HandleCalcKey("1"); form.HandleCalcKey("0"); form.HandleCalcKey("0");
                    form.HandleCalcKey("+"); form.HandleCalcKey("2"); form.HandleCalcKey("4"); form.HandleCalcKey("=");
                    form.ApplyTheme(CalcTheme.Win11Light());
                    form.ApplyTheme(CalcTheme.Win11Dark());
                    Application.DoEvents();
                    long ramMb = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
                    sb.AppendLine("WINDOW_OPENED_OK=" + form.IsHandleCreated);
                    sb.AppendLine("WORKING_SET_MB=" + ramMb);
                    form.Close();
                }
                sb.AppendLine("SMOKE_RESULT=PASS");
            }
            catch (Exception ex)
            {
                sb.AppendLine("SMOKE_RESULT=FAIL");
                sb.AppendLine(ex.ToString());
                if (!string.IsNullOrEmpty(logPath)) File.WriteAllText(logPath, sb.ToString(), Encoding.UTF8);
                return 1;
            }
            if (!string.IsNullOrEmpty(logPath)) File.WriteAllText(logPath, sb.ToString(), Encoding.UTF8);
            return 0;
        }
    }

    // ========================================================================
    // BUILT-IN ULTRA-LIGHT AUTO-UPDATER (1x/24h shared CDN check · SHA-256)
    // ========================================================================
    internal static class SuiteUpdater
    {
        private const string CATALOG_URL = "https://raw.githubusercontent.com/nillynp/nillyhub/main/catalog.json";

        public static T Attach<T>(T form, string repoName, string pkgPrefix, string installerScript) where T : System.Windows.Forms.Form
        {
            if (form == null) return null;
            form.Shown += delegate
            {
                System.Threading.Thread th = new System.Threading.Thread(delegate()
                {
                    try
                    {
                        System.Threading.Thread.Sleep(2200);
                        if (form.IsDisposed) return;
                        string curTag, newTag;
                        if (!CheckForUpdate(repoName, out curTag, out newTag)) return;
                        if (form.IsDisposed || !form.IsHandleCreated) return;
                        form.BeginInvoke((System.Action)delegate
                        {
                            if (form.IsDisposed || form.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None) return;
                            ShowUpdateBanner(form, repoName, pkgPrefix, installerScript, curTag, newTag);
                        });
                    }
                    catch { }
                });
                th.IsBackground = true;
                th.Start();
            };
            return form;
        }

        private static void ShowUpdateBanner(System.Windows.Forms.Form form, string repoName, string pkgPrefix, string installerScript, string curTag, string newTag)
        {
            System.Drawing.Font fTxt = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Regular);
            System.Drawing.Font fBtn = new System.Drawing.Font("Segoe UI", 8.2f, System.Drawing.FontStyle.Bold);

            System.Windows.Forms.Panel bar = new System.Windows.Forms.Panel();
            bar.Dock = System.Windows.Forms.DockStyle.Top;
            bar.Height = 28;
            bar.BackColor = System.Drawing.Color.FromArgb(40, 29, 18);
            bar.Padding = new System.Windows.Forms.Padding(10, 0, 8, 1);
            bar.Paint += delegate(object s, System.Windows.Forms.PaintEventArgs e)
            {
                using (System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.FromArgb(252, 150, 31)))
                    e.Graphics.DrawLine(p, 0, bar.Height - 1, bar.Width, bar.Height - 1);
            };

            System.Windows.Forms.Button btnClose = new System.Windows.Forms.Button();
            btnClose.Text = "×";
            btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            btnClose.Width = 26;
            btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.BackColor = System.Drawing.Color.Transparent;
            btnClose.ForeColor = System.Drawing.Color.FromArgb(200, 170, 130);
            btnClose.Font = fBtn;
            btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            btnClose.TabStop = false;
            btnClose.Click += delegate { bar.Dispose(); };

            System.Windows.Forms.Button btnUpd = new System.Windows.Forms.Button();
            btnUpd.Text = "Ενημέρωση";
            btnUpd.Dock = System.Windows.Forms.DockStyle.Right;
            btnUpd.Width = 88;
            btnUpd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnUpd.FlatAppearance.BorderSize = 0;
            btnUpd.BackColor = System.Drawing.Color.FromArgb(252, 150, 31);
            btnUpd.ForeColor = System.Drawing.Color.FromArgb(20, 22, 26);
            btnUpd.Font = fBtn;
            btnUpd.Cursor = System.Windows.Forms.Cursors.Hand;
            btnUpd.TabStop = false;

            System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();
            lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lbl.Font = fTxt;
            lbl.ForeColor = System.Drawing.Color.FromArgb(255, 185, 95);
            lbl.UseMnemonic = false;
            lbl.Text = "Νέα έκδοση " + newTag + " διαθέσιμη (έχεις " + curTag + ")";

            btnUpd.Click += delegate
            {
                btnUpd.Enabled = false;
                btnUpd.Text = "Λήψη...";
                System.Threading.Thread uth = new System.Threading.Thread(delegate()
                {
                    try
                    {
                        ApplySelfUpdate(repoName, pkgPrefix, installerScript,
                            delegate(string st)
                            {
                                try { if (!form.IsDisposed) form.BeginInvoke((System.Action)delegate { lbl.Text = st; }); } catch { }
                            },
                            delegate
                            {
                                try { if (!form.IsDisposed) form.BeginInvoke((System.Action)delegate { System.Windows.Forms.Application.Exit(); }); } catch { }
                            });
                    }
                    catch (System.Exception ex)
                    {
                        try
                        {
                            if (!form.IsDisposed) form.BeginInvoke((System.Action)delegate
                            {
                                lbl.Text = "Σφάλμα ενημέρωσης: " + ex.Message;
                                btnUpd.Text = "Ενημέρωση";
                                btnUpd.Enabled = true;
                            });
                        }
                        catch { }
                    }
                });
                uth.IsBackground = true;
                uth.Start();
            };

            bar.Disposed += delegate { fTxt.Dispose(); fBtn.Dispose(); };
            bar.Controls.Add(lbl);
            bar.Controls.Add(btnUpd);
            bar.Controls.Add(btnClose);
            form.Controls.Add(bar);
        }

        private static bool TryParseVer(string raw, out System.Version ver)
        {
            ver = null;
            if (string.IsNullOrEmpty(raw)) return false;
            string s = raw.Trim();
            if (s.StartsWith("v", System.StringComparison.OrdinalIgnoreCase)) s = s.Substring(1);
            string[] p = s.Split('.');
            if (p.Length < 2 || p.Length > 4) return false;
            int maj, min, pat = 0;
            if (!int.TryParse(p[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out maj)) return false;
            if (!int.TryParse(p[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out min)) return false;
            if (p.Length >= 3 && !int.TryParse(p[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out pat)) return false;
            ver = new System.Version(maj, min, pat, 0);
            return true;
        }

        private static string FmtVer(System.Version v)
        {
            return v.Build > 0
                ? string.Format(System.Globalization.CultureInfo.InvariantCulture, "v{0}.{1}.{2}", v.Major, v.Minor, v.Build)
                : string.Format(System.Globalization.CultureInfo.InvariantCulture, "v{0}.{1}", v.Major, v.Minor);
        }

        public static bool CheckForUpdate(string repoName, out string curTag, out string newTag)
        {
            curTag = null; newTag = null;
            System.Version rawCur = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            System.Version curVer = new System.Version(rawCur.Major, rawCur.Minor, rawCur.Build >= 0 ? rawCur.Build : 0, 0);
            curTag = FmtVer(curVer);

            string suiteDir = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "NillySuite");
            string cachePath = System.IO.Path.Combine(suiteDir, ".nillyhub_catalog.json");
            string json = null;
            try
            {
                if (System.IO.File.Exists(cachePath))
                {
                    System.DateTime wt = System.IO.File.GetLastWriteTimeUtc(cachePath);
                    if ((System.DateTime.UtcNow - wt).TotalHours < 24.0)
                    {
                        string cached = System.IO.File.ReadAllText(cachePath, System.Text.Encoding.UTF8);
                        if (cached.IndexOf("\"latestVersion\"", System.StringComparison.Ordinal) >= 0) json = cached;
                    }
                }
            }
            catch { }

            if (string.IsNullOrEmpty(json))
            {
                json = HttpGetText(CATALOG_URL, 5000);
                if (string.IsNullOrEmpty(json) || json.Length > 256 * 1024) return false;
                try
                {
                    System.IO.Directory.CreateDirectory(suiteDir);
                    System.IO.File.WriteAllText(cachePath, json, new System.Text.UTF8Encoding(false));
                }
                catch { }
            }

            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(
                json,
                "\"repo\"\\s*:\\s*\"" + System.Text.RegularExpressions.Regex.Escape(repoName) + "\"[^\\}]{0,500}?\"latestVersion\"\\s*:\\s*\"(v[0-9]+\\.[0-9]+(?:\\.[0-9]+)?)\"",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!m.Success) return false;
            System.Version remVer;
            if (!TryParseVer(m.Groups[1].Value, out remVer) || remVer <= curVer) return false;
            newTag = FmtVer(remVer);
            return true;
        }

        private static string HttpGetText(string url, int timeoutMs)
        {
            try { System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Tls12; } catch { }
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            req.UserAgent = "NillySuite-Updater/1.0 (https://nillygraphics.com/)";
            req.Accept = "application/vnd.github+json, application/json";
            req.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            req.Timeout = timeoutMs;
            req.ReadWriteTimeout = timeoutMs;
            using (System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.GetResponse())
            using (System.IO.Stream st = resp.GetResponseStream())
            using (System.IO.StreamReader sr = new System.IO.StreamReader(st, System.Text.Encoding.UTF8))
                return sr.ReadToEnd();
        }

        private static string ExtractAssetBlock(string json, string assetName)
        {
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(
                json, "\"name\"\\s*:\\s*\"" + System.Text.RegularExpressions.Regex.Escape(assetName) + "\"");
            if (!m.Success) return null;
            int start = m.Index;
            int depth = 0;
            while (start > 0)
            {
                char c = json[start];
                if (c == '}') depth++;
                else if (c == '{') { if (depth == 0) break; depth--; }
                start--;
            }
            int end = start;
            depth = 0;
            bool inStr = false;
            for (int i = start; i < json.Length; i++)
            {
                char c = json[i];
                if (c == '"' && (i == 0 || json[i - 1] != '\\')) inStr = !inStr;
                if (inStr) continue;
                if (c == '{') depth++;
                else if (c == '}') { depth--; if (depth == 0) { end = i; break; } }
            }
            return end > start ? json.Substring(start, end - start + 1) : null;
        }

        public static void ApplySelfUpdate(string repoName, string pkgPrefix, string installerScript, System.Action<string> onStatus, System.Action onExit)
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appDir = System.IO.Path.GetDirectoryName(exePath);
            if (System.IO.Directory.Exists(System.IO.Path.Combine(appDir, ".git")))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/nillynp/" + repoName + "/releases/latest") { UseShellExecute = true });
                if (onStatus != null) onStatus("Άνοιξε το GitHub Release (φάκελος Git).");
                return;
            }

            if (onStatus != null) onStatus("Έλεγχος SHA-256 στο GitHub...");
            string relJson = HttpGetText("https://api.github.com/repos/nillynp/" + repoName + "/releases/latest", 12000);
            if (string.IsNullOrEmpty(relJson)) throw new System.IO.InvalidDataException("Κενή απόκριση GitHub.");
            if (System.Text.RegularExpressions.Regex.IsMatch(relJson, "\"draft\"\\s*:\\s*true") ||
                System.Text.RegularExpressions.Regex.IsMatch(relJson, "\"prerelease\"\\s*:\\s*true"))
                throw new System.IO.InvalidDataException("Μη σταθερή έκδοση.");

            System.Text.RegularExpressions.Match tm = System.Text.RegularExpressions.Regex.Match(relJson, "\"tag_name\"\\s*:\\s*\"(v[0-9]+\\.[0-9]+(?:\\.[0-9]+)?)\"");
            if (!tm.Success) throw new System.IO.InvalidDataException("Μη έγκυρο tag έκδοσης.");
            string tag = tm.Groups[1].Value;

            System.Version curRaw = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            System.Version curVer = new System.Version(curRaw.Major, curRaw.Minor, curRaw.Build >= 0 ? curRaw.Build : 0, 0);
            System.Version remVer;
            if (!TryParseVer(tag, out remVer) || remVer <= curVer)
                throw new System.InvalidOperationException("Έχεις ήδη την τελευταία έκδοση.");

            string zipName = pkgPrefix + "-" + tag + "-Portable.zip";
            string expectedUrl = "https://github.com/nillynp/" + repoName + "/releases/download/" + tag + "/" + zipName;
            string blk = ExtractAssetBlock(relJson, zipName);
            if (string.IsNullOrEmpty(blk)) throw new System.IO.FileNotFoundException("Δεν βρέθηκε το Portable ZIP.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(blk, "\"state\"\\s*:\\s*\"uploaded\""))
                throw new System.IO.InvalidDataException("Το αρχείο δεν είναι έτοιμο.");
            System.Text.RegularExpressions.Match um = System.Text.RegularExpressions.Regex.Match(blk, "\"browser_download_url\"\\s*:\\s*\"([^\"]+)\"");
            if (!um.Success || !string.Equals(um.Groups[1].Value, expectedUrl, System.StringComparison.Ordinal))
                throw new System.IO.InvalidDataException("Μη έγκυρο URL λήψης.");
            System.Text.RegularExpressions.Match dm = System.Text.RegularExpressions.Regex.Match(blk, "\"digest\"\\s*:\\s*\"sha256:([0-9a-f]{64})\"");
            if (!dm.Success) throw new System.IO.InvalidDataException("Απουσιάζει SHA-256.");
            string expectedSha = dm.Groups[1].Value;

            string tmpDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "NillyUpd-" + System.Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(tmpDir);
            string zipFile = System.IO.Path.Combine(tmpDir, "pkg.zip");

            if (onStatus != null) onStatus("Λήψη " + tag + "...");
            try { System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072 | System.Net.SecurityProtocolType.Tls12; } catch { }
            System.Net.HttpWebRequest dreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(expectedUrl);
            dreq.UserAgent = "NillySuite-Updater/1.0 (https://nillygraphics.com/)";
            dreq.Timeout = 20000;
            dreq.ReadWriteTimeout = 30000;
            using (System.Net.HttpWebResponse dresp = (System.Net.HttpWebResponse)dreq.GetResponse())
            using (System.IO.Stream src = dresp.GetResponseStream())
            using (System.IO.FileStream dst = new System.IO.FileStream(zipFile, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                byte[] buf = new byte[16384];
                int n; long total = 0;
                while ((n = src.Read(buf, 0, buf.Length)) > 0)
                {
                    total += n;
                    if (total > 32L * 1024L * 1024L) throw new System.IO.InvalidDataException("Υπέρβαση μεγέθους ZIP.");
                    dst.Write(buf, 0, n);
                }
            }

            if (onStatus != null) onStatus("Επαλήθευση SHA-256...");
            string actualSha;
            using (System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create())
            using (System.IO.FileStream fs = System.IO.File.OpenRead(zipFile))
            {
                byte[] hash = sha.ComputeHash(fs);
                System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
                for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("x2", System.Globalization.CultureInfo.InvariantCulture));
                actualSha = sb.ToString();
            }
            if (!string.Equals(actualSha, expectedSha, System.StringComparison.OrdinalIgnoreCase))
            {
                try { System.IO.Directory.Delete(tmpDir, true); } catch { }
                throw new System.IO.InvalidDataException("Αποτυχία ελέγχου SHA-256.");
            }

            if (onStatus != null) onStatus("Εγκατάσταση " + tag + " & επανεκκίνηση...");
            string ps1 = System.IO.Path.Combine(tmpDir, "apply.ps1");
            string script =
                "param($myPid,$zip,$appDir,$instName,$exePath,$tag)\r\n" +
                "for($i=0;$i -lt 60;$i++){ if(-not (Get-Process -Id $myPid -ErrorAction SilentlyContinue)){break}; Start-Sleep -Milliseconds 150 }\r\n" +
                "Add-Type -AssemblyName System.IO.Compression.FileSystem\r\n" +
                "$za = [System.IO.Compression.ZipFile]::OpenRead($zip)\r\n" +
                "try {\r\n" +
                "  $pref = $null; $shared = $true\r\n" +
                "  foreach($e in $za.Entries){ $fn=$e.FullName -replace '\\\\','/'; $sl=$fn.IndexOf('/'); if($sl -le 0){$shared=$false;break}; $p=$fn.Substring(0,$sl+1); if(-not $pref){$pref=$p}elseif(-not $pref.Equals($p,[StringComparison]::OrdinalIgnoreCase)){$shared=$false;break} }\r\n" +
                "  $rootFull = [System.IO.Path]::GetFullPath($appDir).TrimEnd('\\') + '\\'\r\n" +
                "  foreach($e in $za.Entries){\r\n" +
                "    $rel = $e.FullName -replace '\\\\','/'\r\n" +
                "    if($shared -and $pref -and $rel.StartsWith($pref,[StringComparison]::OrdinalIgnoreCase)){ $rel=$rel.Substring($pref.Length) }\r\n" +
                "    if([string]::IsNullOrEmpty($rel) -or $rel.EndsWith('/') -or $rel.Contains(':')){ continue }\r\n" +
                "    $dst = [System.IO.Path]::GetFullPath((Join-Path $appDir ($rel -replace '/','\\')))\r\n" +
                "    if(-not $dst.StartsWith($rootFull,[StringComparison]::OrdinalIgnoreCase)){ continue }\r\n" +
                "    $par = Split-Path -Parent $dst; if($par){ New-Item -ItemType Directory -Force -Path $par | Out-Null }\r\n" +
                "    for($k=0;$k -lt 5;$k++){ try { [System.IO.Compression.ZipFileExtensions]::ExtractToFile($e,$dst,$true); break } catch { Start-Sleep -Milliseconds 250 } }\r\n" +
                "  }\r\n" +
                "} finally { $za.Dispose() }\r\n" +
                "$bak = $exePath + '.bak'\r\n" +
                "if(Test-Path $exePath){ Copy-Item -LiteralPath $exePath -Destination $bak -Force -ErrorAction SilentlyContinue; for($k=0;$k -lt 5;$k++){ try { Remove-Item -LiteralPath $exePath -Force -ErrorAction Stop; break } catch { Start-Sleep -Milliseconds 250 } } }\r\n" +
                "Get-ChildItem (Join-Path $appDir 'desktop') -Filter '*.cs' -ErrorAction SilentlyContinue | ForEach-Object { $_.LastWriteTimeUtc = [DateTime]::UtcNow }\r\n" +
                "if($instName -and (Test-Path (Join-Path $appDir $instName))){ & (Join-Path $appDir $instName) -Silent 2>$null | Out-Null }\r\n" +
                "if(-not (Test-Path $exePath) -and (Test-Path $bak)){ Move-Item -LiteralPath $bak -Destination $exePath -Force -ErrorAction SilentlyContinue } else { Remove-Item -LiteralPath $bak -Force -ErrorAction SilentlyContinue }\r\n" +
                "if($tag){ Set-Content -LiteralPath (Join-Path $appDir '.nilly_version') -Value $tag -Encoding UTF8 -ErrorAction SilentlyContinue }\r\n" +
                "if(Test-Path $exePath){ Start-Process -FilePath $exePath -WorkingDirectory $appDir }\r\n" +
                "Start-Sleep -Milliseconds 400\r\n" +
                "Remove-Item -LiteralPath (Split-Path $zip) -Recurse -Force -ErrorAction SilentlyContinue\r\n";
            System.IO.File.WriteAllText(ps1, script, new System.Text.UTF8Encoding(false));

            int myPid = System.Diagnostics.Process.GetCurrentProcess().Id;
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(
                "powershell.exe",
                "-NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"" + ps1 + "\" " +
                myPid + " \"" + zipFile + "\" \"" + appDir + "\" \"" + installerScript + "\" \"" + exePath + "\" \"" + tag + "\"");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            System.Diagnostics.Process.Start(psi);
            if (onExit != null) onExit();
        }
    }

    // ========================================================================
    // ARIAKIT-INSPIRED MINIMAL VECTOR UI KIT (Zero DLLs · 1.5px Stroke Icons)
    // ========================================================================
    internal enum AkKind { Primary, Secondary, Ghost, Tab, AccentOutline, Danger }

    internal enum AkIcon
    {
        None, Refresh, Sun, Moon, Search, Pin, Info, Play, Pause, Stop,
        Plus, Minus, Close, Up, Down, Folder, Copy, Trash, Check, Swap,
        Download, Link, Split, Pipette, Bolt, List, Filter, Shield, Lock, Unlock,
        Edit, Save, External, Bold, Italic, Undo, ChevLeft, ChevRight, Star
    }

    internal static class AkDraw
    {
        public static System.Drawing.Drawing2D.GraphicsPath RoundRect(float x, float y, float w, float h, float r)
        {
            System.Drawing.Drawing2D.GraphicsPath p = new System.Drawing.Drawing2D.GraphicsPath();
            float d = System.Math.Min(r * 2f, System.Math.Min(w, h));
            if (d <= 0.5f) { p.AddRectangle(new System.Drawing.RectangleF(x, y, w, h)); return p; }
            p.AddArc(x, y, d, d, 180, 90);
            p.AddArc(x + w - d, y, d, d, 270, 90);
            p.AddArc(x + w - d, y + h - d, d, d, 0, 90);
            p.AddArc(x, y + h - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void DrawIcon(System.Drawing.Graphics g, AkIcon icon, float cx, float cy, float sz, System.Drawing.Color color)
        {
            if (icon == AkIcon.None) return;
            System.Drawing.Drawing2D.SmoothingMode oldSm = g.SmoothingMode;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            float r = sz * 0.5f;
            using (System.Drawing.Pen pen = new System.Drawing.Pen(color, 1.5f))
            using (System.Drawing.SolidBrush br = new System.Drawing.SolidBrush(color))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                switch (icon)
                {
                    case AkIcon.Refresh:
                        g.DrawArc(pen, cx - r * 0.82f, cy - r * 0.82f, r * 1.64f, r * 1.64f, -55, 295);
                        g.DrawLine(pen, cx + r * 0.28f, cy - r * 0.85f, cx + r * 0.62f, cy - r * 0.52f);
                        g.DrawLine(pen, cx + r * 0.62f, cy - r * 0.52f, cx + r * 0.20f, cy - r * 0.42f);
                        break;
                    case AkIcon.Sun:
                        g.DrawEllipse(pen, cx - r * 0.42f, cy - r * 0.42f, r * 0.84f, r * 0.84f);
                        for (int i = 0; i < 8; i++)
                        {
                            double a = i * System.Math.PI / 4.0;
                            float x1 = cx + (float)(System.Math.Cos(a) * r * 0.68f);
                            float y1 = cy + (float)(System.Math.Sin(a) * r * 0.68f);
                            float x2 = cx + (float)(System.Math.Cos(a) * r * 0.94f);
                            float y2 = cy + (float)(System.Math.Sin(a) * r * 0.94f);
                            g.DrawLine(pen, x1, y1, x2, y2);
                        }
                        break;
                    case AkIcon.Moon:
                        g.DrawArc(pen, cx - r * 0.78f, cy - r * 0.78f, r * 1.56f, r * 1.56f, 40, 235);
                        g.DrawArc(pen, cx - r * 0.30f, cy - r * 0.92f, r * 1.32f, r * 1.32f, 115, 130);
                        break;
                    case AkIcon.Search:
                        g.DrawEllipse(pen, cx - r * 0.82f, cy - r * 0.82f, r * 1.24f, r * 1.24f);
                        g.DrawLine(pen, cx + r * 0.25f, cy + r * 0.25f, cx + r * 0.82f, cy + r * 0.82f);
                        break;
                    case AkIcon.Pin:
                        g.DrawLine(pen, cx - r * 0.48f, cy - r * 0.72f, cx + r * 0.48f, cy - r * 0.72f);
                        g.DrawLine(pen, cx - r * 0.34f, cy - r * 0.72f, cx - r * 0.56f, cy + r * 0.14f);
                        g.DrawLine(pen, cx + r * 0.34f, cy - r * 0.72f, cx + r * 0.56f, cy + r * 0.14f);
                        g.DrawLine(pen, cx - r * 0.66f, cy + r * 0.14f, cx + r * 0.66f, cy + r * 0.14f);
                        g.DrawLine(pen, cx, cy + r * 0.14f, cx, cy + r * 0.86f);
                        break;
                    case AkIcon.Info:
                        g.DrawEllipse(pen, cx - r * 0.85f, cy - r * 0.85f, r * 1.70f, r * 1.70f);
                        g.DrawLine(pen, cx, cy - r * 0.08f, cx, cy + r * 0.48f);
                        g.FillEllipse(br, cx - 1.1f, cy - r * 0.52f, 2.2f, 2.2f);
                        break;
                    case AkIcon.Play:
                        g.FillPolygon(br, new System.Drawing.PointF[] {
                            new System.Drawing.PointF(cx - r * 0.45f, cy - r * 0.68f),
                            new System.Drawing.PointF(cx + r * 0.68f, cy),
                            new System.Drawing.PointF(cx - r * 0.45f, cy + r * 0.68f)
                        });
                        break;
                    case AkIcon.Pause:
                        g.DrawLine(pen, cx - r * 0.36f, cy - r * 0.62f, cx - r * 0.36f, cy + r * 0.62f);
                        g.DrawLine(pen, cx + r * 0.36f, cy - r * 0.62f, cx + r * 0.36f, cy + r * 0.62f);
                        break;
                    case AkIcon.Stop:
                        g.FillRectangle(br, cx - r * 0.56f, cy - r * 0.56f, r * 1.12f, r * 1.12f);
                        break;
                    case AkIcon.Plus:
                        g.DrawLine(pen, cx - r * 0.68f, cy, cx + r * 0.68f, cy);
                        g.DrawLine(pen, cx, cy - r * 0.68f, cx, cy + r * 0.68f);
                        break;
                    case AkIcon.Minus:
                        g.DrawLine(pen, cx - r * 0.68f, cy, cx + r * 0.68f, cy);
                        break;
                    case AkIcon.Close:
                        g.DrawLine(pen, cx - r * 0.58f, cy - r * 0.58f, cx + r * 0.58f, cy + r * 0.58f);
                        g.DrawLine(pen, cx + r * 0.58f, cy - r * 0.58f, cx - r * 0.58f, cy + r * 0.58f);
                        break;
                    case AkIcon.Up:
                        g.DrawLine(pen, cx, cy + r * 0.68f, cx, cy - r * 0.64f);
                        g.DrawLine(pen, cx - r * 0.52f, cy - r * 0.14f, cx, cy - r * 0.66f);
                        g.DrawLine(pen, cx + r * 0.52f, cy - r * 0.14f, cx, cy - r * 0.66f);
                        break;
                    case AkIcon.Down:
                        g.DrawLine(pen, cx, cy - r * 0.68f, cx, cy + r * 0.64f);
                        g.DrawLine(pen, cx - r * 0.52f, cy + r * 0.14f, cx, cy + r * 0.66f);
                        g.DrawLine(pen, cx + r * 0.52f, cy + r * 0.14f, cx, cy + r * 0.66f);
                        break;
                    case AkIcon.Folder:
                        g.DrawPolygon(pen, new System.Drawing.PointF[] {
                            new System.Drawing.PointF(cx - r * 0.85f, cy - r * 0.56f),
                            new System.Drawing.PointF(cx - r * 0.22f, cy - r * 0.56f),
                            new System.Drawing.PointF(cx - r * 0.04f, cy - r * 0.34f),
                            new System.Drawing.PointF(cx + r * 0.85f, cy - r * 0.34f),
                            new System.Drawing.PointF(cx + r * 0.85f, cy + r * 0.64f),
                            new System.Drawing.PointF(cx - r * 0.85f, cy + r * 0.64f)
                        });
                        break;
                    case AkIcon.Copy:
                        g.DrawRectangle(pen, cx - r * 0.26f, cy - r * 0.26f, r * 1.02f, r * 1.02f);
                        g.DrawLine(pen, cx - r * 0.68f, cy + r * 0.30f, cx - r * 0.68f, cy - r * 0.68f);
                        g.DrawLine(pen, cx - r * 0.68f, cy - r * 0.68f, cx + r * 0.30f, cy - r * 0.68f);
                        break;
                    case AkIcon.Trash:
                        g.DrawLine(pen, cx - r * 0.76f, cy - r * 0.44f, cx + r * 0.76f, cy - r * 0.44f);
                        g.DrawLine(pen, cx - r * 0.28f, cy - r * 0.74f, cx + r * 0.28f, cy - r * 0.74f);
                        g.DrawRectangle(pen, cx - r * 0.54f, cy - r * 0.42f, r * 1.08f, r * 1.16f);
                        break;
                    case AkIcon.Check:
                        g.DrawLine(pen, cx - r * 0.68f, cy + r * 0.04f, cx - r * 0.14f, cy + r * 0.58f);
                        g.DrawLine(pen, cx - r * 0.14f, cy + r * 0.58f, cx + r * 0.74f, cy - r * 0.52f);
                        break;
                    case AkIcon.Swap:
                        g.DrawLine(pen, cx - r * 0.70f, cy - r * 0.32f, cx + r * 0.70f, cy - r * 0.32f);
                        g.DrawLine(pen, cx + r * 0.32f, cy - r * 0.68f, cx + r * 0.72f, cy - r * 0.32f);
                        g.DrawLine(pen, cx + r * 0.70f, cy + r * 0.34f, cx - r * 0.70f, cy + r * 0.34f);
                        g.DrawLine(pen, cx - r * 0.32f, cy + r * 0.70f, cx - r * 0.72f, cy + r * 0.34f);
                        break;
                    case AkIcon.Download:
                        g.DrawLine(pen, cx, cy - r * 0.72f, cx, cy + r * 0.28f);
                        g.DrawLine(pen, cx - r * 0.44f, cy - r * 0.12f, cx, cy + r * 0.32f);
                        g.DrawLine(pen, cx + r * 0.44f, cy - r * 0.12f, cx, cy + r * 0.32f);
                        g.DrawLine(pen, cx - r * 0.74f, cy + r * 0.72f, cx + r * 0.74f, cy + r * 0.72f);
                        break;
                    case AkIcon.Link:
                    case AkIcon.External:
                        g.DrawLine(pen, cx - r * 0.58f, cy + r * 0.58f, cx + r * 0.62f, cy - r * 0.62f);
                        g.DrawLine(pen, cx - r * 0.06f, cy - r * 0.64f, cx + r * 0.64f, cy - r * 0.64f);
                        g.DrawLine(pen, cx + r * 0.64f, cy - r * 0.64f, cx + r * 0.64f, cy + r * 0.06f);
                        break;
                    case AkIcon.Split:
                        g.DrawRectangle(pen, cx - r * 0.82f, cy - r * 0.64f, r * 1.64f, r * 1.28f);
                        g.DrawLine(pen, cx, cy - r * 0.64f, cx, cy + r * 0.64f);
                        break;
                    case AkIcon.Pipette:
                        g.DrawLine(pen, cx - r * 0.66f, cy + r * 0.66f, cx + r * 0.40f, cy - r * 0.40f);
                        g.DrawLine(pen, cx + r * 0.12f, cy - r * 0.68f, cx + r * 0.68f, cy - r * 0.12f);
                        break;
                    case AkIcon.Bolt:
                        g.DrawPolygon(pen, new System.Drawing.PointF[] {
                            new System.Drawing.PointF(cx + r * 0.14f, cy - r * 0.82f),
                            new System.Drawing.PointF(cx - r * 0.62f, cy + r * 0.08f),
                            new System.Drawing.PointF(cx - r * 0.04f, cy + r * 0.08f),
                            new System.Drawing.PointF(cx - r * 0.18f, cy + r * 0.82f),
                            new System.Drawing.PointF(cx + r * 0.62f, cy - r * 0.10f),
                            new System.Drawing.PointF(cx + r * 0.04f, cy - r * 0.10f)
                        });
                        break;
                    case AkIcon.List:
                        g.DrawLine(pen, cx - r * 0.72f, cy - r * 0.46f, cx + r * 0.72f, cy - r * 0.46f);
                        g.DrawLine(pen, cx - r * 0.72f, cy, cx + r * 0.72f, cy);
                        g.DrawLine(pen, cx - r * 0.72f, cy + r * 0.46f, cx + r * 0.72f, cy + r * 0.46f);
                        break;
                    case AkIcon.Filter:
                        g.DrawPolygon(pen, new System.Drawing.PointF[] {
                            new System.Drawing.PointF(cx - r * 0.78f, cy - r * 0.60f),
                            new System.Drawing.PointF(cx + r * 0.78f, cy - r * 0.60f),
                            new System.Drawing.PointF(cx + r * 0.16f, cy + r * 0.08f),
                            new System.Drawing.PointF(cx + r * 0.16f, cy + r * 0.68f),
                            new System.Drawing.PointF(cx - r * 0.16f, cy + r * 0.52f),
                            new System.Drawing.PointF(cx - r * 0.16f, cy + r * 0.08f)
                        });
                        break;
                    case AkIcon.Shield:
                        g.DrawPolygon(pen, new System.Drawing.PointF[] {
                            new System.Drawing.PointF(cx, cy - r * 0.82f),
                            new System.Drawing.PointF(cx + r * 0.72f, cy - r * 0.44f),
                            new System.Drawing.PointF(cx + r * 0.58f, cy + r * 0.34f),
                            new System.Drawing.PointF(cx, cy + r * 0.82f),
                            new System.Drawing.PointF(cx - r * 0.58f, cy + r * 0.34f),
                            new System.Drawing.PointF(cx - r * 0.72f, cy - r * 0.44f)
                        });
                        break;
                    case AkIcon.Lock:
                        g.DrawRectangle(pen, cx - r * 0.58f, cy - r * 0.08f, r * 1.16f, r * 0.84f);
                        g.DrawArc(pen, cx - r * 0.38f, cy - r * 0.76f, r * 0.76f, r * 0.84f, 180, 180);
                        break;
                    case AkIcon.Unlock:
                        g.DrawRectangle(pen, cx - r * 0.58f, cy - r * 0.08f, r * 1.16f, r * 0.84f);
                        g.DrawArc(pen, cx - r * 0.38f, cy - r * 0.76f, r * 0.76f, r * 0.84f, 180, 130);
                        break;
                    case AkIcon.Save:
                        g.DrawRectangle(pen, cx - r * 0.72f, cy - r * 0.72f, r * 1.44f, r * 1.44f);
                        g.DrawRectangle(pen, cx - r * 0.42f, cy - r * 0.72f, r * 0.72f, r * 0.48f);
                        g.DrawRectangle(pen, cx - r * 0.44f, cy + r * 0.14f, r * 0.88f, r * 0.58f);
                        break;
                    case AkIcon.Edit:
                        g.DrawLine(pen, cx - r * 0.62f, cy + r * 0.62f, cx + r * 0.56f, cy - r * 0.56f);
                        g.DrawLine(pen, cx + r * 0.24f, cy - r * 0.74f, cx + r * 0.74f, cy - r * 0.24f);
                        g.DrawLine(pen, cx - r * 0.72f, cy + r * 0.72f, cx - r * 0.34f, cy + r * 0.62f);
                        break;
                    case AkIcon.Undo:
                        g.DrawArc(pen, cx - r * 0.52f, cy - r * 0.34f, r * 1.24f, r * 1.04f, -135, 225);
                        g.DrawLine(pen, cx - r * 0.72f, cy - r * 0.22f, cx - r * 0.28f, cy - r * 0.62f);
                        g.DrawLine(pen, cx - r * 0.72f, cy - r * 0.22f, cx - r * 0.24f, cy + r * 0.10f);
                        break;
                    case AkIcon.ChevLeft:
                        g.DrawLine(pen, cx + r * 0.28f, cy - r * 0.58f, cx - r * 0.34f, cy);
                        g.DrawLine(pen, cx - r * 0.34f, cy, cx + r * 0.28f, cy + r * 0.58f);
                        break;
                    case AkIcon.ChevRight:
                        g.DrawLine(pen, cx - r * 0.28f, cy - r * 0.58f, cx + r * 0.34f, cy);
                        g.DrawLine(pen, cx + r * 0.34f, cy, cx - r * 0.28f, cy + r * 0.58f);
                        break;
                    case AkIcon.Star:
                        System.Drawing.PointF[] sp = new System.Drawing.PointF[10];
                        for (int k = 0; k < 10; k++)
                        {
                            double ang = -System.Math.PI / 2.0 + k * System.Math.PI / 5.0;
                            float rad = (k % 2 == 0) ? r * 0.85f : r * 0.38f;
                            sp[k] = new System.Drawing.PointF(cx + (float)(System.Math.Cos(ang) * rad), cy + (float)(System.Math.Sin(ang) * rad));
                        }
                        g.DrawPolygon(pen, sp);
                        break;
                }
            }
            g.SmoothingMode = oldSm;
        }
    }

    internal sealed class AkButton : System.Windows.Forms.Button
    {
        public AkKind Kind = AkKind.Secondary;
        public AkIcon Icon = AkIcon.None;
        public bool Active = false;
        public bool Dark = true;
        public int Radius = 6;
        private bool _hov = false;
        private bool _down = false;

        public AkButton()
        {
            SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                     System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer |
                     System.Windows.Forms.ControlStyles.UserPaint |
                     System.Windows.Forms.ControlStyles.ResizeRedraw, true);
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Cursor = System.Windows.Forms.Cursors.Hand;
            TabStop = false;
            UseMnemonic = false;
        }

        protected override void OnMouseEnter(System.EventArgs e) { _hov = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(System.EventArgs e) { _hov = false; _down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) { if (e.Button == System.Windows.Forms.MouseButtons.Left) { _down = true; Invalidate(); } base.OnMouseDown(e); }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) { _down = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            System.Drawing.Color parentBg = Parent != null ? Parent.BackColor : (Dark ? System.Drawing.Color.FromArgb(29, 31, 32) : System.Drawing.Color.FromArgb(248, 249, 250));
            g.Clear(parentBg);

            System.Drawing.Color accent = Dark ? System.Drawing.Color.FromArgb(252, 150, 31) : System.Drawing.Color.FromArgb(224, 118, 12);
            System.Drawing.Color accentHov = Dark ? System.Drawing.Color.FromArgb(255, 169, 64) : System.Drawing.Color.FromArgb(204, 102, 8);
            System.Drawing.Color surf = Dark ? System.Drawing.Color.FromArgb(39, 41, 43) : System.Drawing.Color.White;
            System.Drawing.Color surfHov = Dark ? System.Drawing.Color.FromArgb(50, 53, 56) : System.Drawing.Color.FromArgb(236, 239, 243);
            System.Drawing.Color border = Dark ? System.Drawing.Color.FromArgb(58, 62, 66) : System.Drawing.Color.FromArgb(210, 216, 224);
            System.Drawing.Color borderHov = Dark ? System.Drawing.Color.FromArgb(84, 89, 95) : System.Drawing.Color.FromArgb(176, 184, 196);
            System.Drawing.Color fgMain = Dark ? System.Drawing.Color.FromArgb(242, 244, 247) : System.Drawing.Color.FromArgb(26, 29, 33);
            System.Drawing.Color fgMuted = Dark ? System.Drawing.Color.FromArgb(154, 160, 168) : System.Drawing.Color.FromArgb(95, 104, 116);
            System.Drawing.Color fgDis = Dark ? System.Drawing.Color.FromArgb(92, 97, 104) : System.Drawing.Color.FromArgb(170, 178, 188);

            bool en = Enabled;
            System.Drawing.Color bg = parentBg;
            System.Drawing.Color bd = System.Drawing.Color.Transparent;
            System.Drawing.Color fg = en ? fgMain : fgDis;
            bool drawFill = false;

            if (Kind == AkKind.Primary)
            {
                drawFill = true;
                bg = en ? (_hov ? accentHov : accent) : (Dark ? System.Drawing.Color.FromArgb(56, 48, 38) : System.Drawing.Color.FromArgb(228, 224, 218));
                fg = en ? (Dark ? System.Drawing.Color.FromArgb(20, 21, 23) : System.Drawing.Color.White) : fgDis;
            }
            else if (Kind == AkKind.AccentOutline)
            {
                drawFill = true;
                bg = en ? (_hov ? (Dark ? System.Drawing.Color.FromArgb(52, 42, 30) : System.Drawing.Color.FromArgb(255, 243, 228)) : surf) : (Dark ? System.Drawing.Color.FromArgb(32, 34, 36) : System.Drawing.Color.FromArgb(244, 246, 248));
                bd = en ? accent : border;
                fg = en ? accent : fgDis;
            }
            else if (Kind == AkKind.Danger)
            {
                drawFill = true;
                System.Drawing.Color red = Dark ? System.Drawing.Color.FromArgb(240, 104, 118) : System.Drawing.Color.FromArgb(196, 40, 56);
                bg = en ? (_hov ? (Dark ? System.Drawing.Color.FromArgb(62, 30, 36) : System.Drawing.Color.FromArgb(254, 228, 231)) : (Dark ? System.Drawing.Color.FromArgb(46, 28, 32) : System.Drawing.Color.FromArgb(255, 240, 242))) : surf;
                bd = en ? (Dark ? System.Drawing.Color.FromArgb(116, 48, 58) : System.Drawing.Color.FromArgb(236, 154, 164)) : border;
                fg = en ? red : fgDis;
            }
            else if (Kind == AkKind.Tab)
            {
                drawFill = _hov && en;
                bg = surfHov;
                fg = en ? (Active ? fgMain : (_hov ? fgMain : fgMuted)) : fgDis;
            }
            else if (Kind == AkKind.Ghost)
            {
                drawFill = (Active || _hov) && en;
                bg = Active ? (Dark ? System.Drawing.Color.FromArgb(52, 42, 30) : System.Drawing.Color.FromArgb(255, 243, 228)) : surfHov;
                bd = Active ? accent : System.Drawing.Color.Transparent;
                fg = en ? (Active ? accent : (_hov ? fgMain : fgMuted)) : fgDis;
            }
            else // Secondary (Ariakit tactile elevated pill)
            {
                drawFill = true;
                bg = en ? (Active ? (Dark ? System.Drawing.Color.FromArgb(52, 42, 30) : System.Drawing.Color.FromArgb(255, 243, 228)) : (_hov ? surfHov : surf)) : (Dark ? System.Drawing.Color.FromArgb(32, 34, 36) : System.Drawing.Color.FromArgb(245, 247, 249));
                bd = en ? (Active ? accent : (_hov ? borderHov : border)) : (Dark ? System.Drawing.Color.FromArgb(44, 47, 50) : System.Drawing.Color.FromArgb(224, 228, 234));
                fg = en ? (Active ? accent : fgMain) : fgDis;
            }

            float ox = 0.5f, oy = (_down && en) ? 1.5f : 0.5f;
            float rw = Width - 1.5f, rh = Height - 1.5f - ((_down && en) ? 1f : 0f);
            if (drawFill || bd.A > 0)
            {
                using (System.Drawing.Drawing2D.GraphicsPath gp = AkDraw.RoundRect(ox, oy, rw, rh, Radius))
                {
                    if (drawFill) using (System.Drawing.SolidBrush br = new System.Drawing.SolidBrush(bg)) g.FillPath(br, gp);
                    if (bd.A > 0) using (System.Drawing.Pen p = new System.Drawing.Pen(bd, 1f)) g.DrawPath(p, gp);
                }
            }
            if (Kind == AkKind.Tab && Active)
            {
                using (System.Drawing.SolidBrush ab = new System.Drawing.SolidBrush(accent))
                    g.FillRectangle(ab, 6, Height - 3, System.Math.Max(8, Width - 12), 2.5f);
            }

            string txt = Text ?? "";
            float yShift = (_down && en) ? 1f : 0f;
            if (Icon != AkIcon.None && txt.Length == 0)
            {
                AkDraw.DrawIcon(g, Icon, Width * 0.5f, Height * 0.5f + yShift, 13.5f, fg);
            }
            else if (Icon != AkIcon.None)
            {
                System.Drawing.Size ts = System.Windows.Forms.TextRenderer.MeasureText(txt, Font, new System.Drawing.Size(Width, Height), System.Windows.Forms.TextFormatFlags.NoPrefix | System.Windows.Forms.TextFormatFlags.NoPadding);
                float iconW = 13f, gap = 6f;
                float totalW = iconW + gap + ts.Width;
                float startX = System.Math.Max(8f, (Width - totalW) * 0.5f);
                AkDraw.DrawIcon(g, Icon, startX + iconW * 0.5f, Height * 0.5f + yShift, 12.5f, fg);
                System.Drawing.Rectangle tr = new System.Drawing.Rectangle((int)(startX + iconW + gap), (int)yShift, System.Math.Max(10, Width - (int)(startX + iconW + gap) - 4), Height);
                System.Windows.Forms.TextRenderer.DrawText(g, txt, Font, tr, fg, System.Windows.Forms.TextFormatFlags.Left | System.Windows.Forms.TextFormatFlags.VerticalCenter | System.Windows.Forms.TextFormatFlags.NoPrefix | System.Windows.Forms.TextFormatFlags.EndEllipsis);
            }
            else
            {
                System.Drawing.Rectangle tr = new System.Drawing.Rectangle(4, (int)yShift, System.Math.Max(8, Width - 8), Height);
                System.Windows.Forms.TextRenderer.DrawText(g, txt, Font, tr, fg, System.Windows.Forms.TextFormatFlags.HorizontalCenter | System.Windows.Forms.TextFormatFlags.VerticalCenter | System.Windows.Forms.TextFormatFlags.NoPrefix | System.Windows.Forms.TextFormatFlags.EndEllipsis);
            }
        }
    }

}
