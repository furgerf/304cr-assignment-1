using System.Drawing;
using System.Drawing.Imaging;

namespace AiPathFinding.Common
{
    public static class Utils
    {
        public static void DrawTransparentImage(Graphics g, Bitmap image, int x, int y, float opacity, bool red = false)
        {
            DrawTransparentImage(g, image, new Point(x, y), opacity, red);
        }

        public static void DrawTransparentImage(Graphics g, Bitmap image, Point location, float opacity, bool red = false)
        {
            var bmp = new Bitmap(image.Width, image.Height);
            var matrix = new ColorMatrix { Matrix33 = opacity };
            if (red)
                matrix.Matrix30 = 0.5f;
            var att = new ImageAttributes();
            att.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            g.DrawImage(image, new Rectangle(location.X, location.Y, bmp.Width, bmp.Width), 0, 0,
                image.Width, image.Height, GraphicsUnit.Pixel, att);
        }
    }
}
