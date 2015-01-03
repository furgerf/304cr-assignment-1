using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AiPathFinding.Common
{
    public static class Utils
    {
        private const float OpacityMinimum = 0.3f;
        private const float OpacityIndexOffset = 1f;

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

        public static float GetPathOpacity(int pathIndex, int pathLength)
        {
            return OpacityMinimum + (pathIndex + OpacityIndexOffset) / pathLength * (1 - OpacityMinimum);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
