using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AiPathFinding.Common
{
    public static class Utils
    {
        #region Constants

        /// <summary>
        /// Minimum opacity to be used when drawing image.
        /// </summary>
        private const float OpacityMinimum = 0.3f;

        /// <summary>
        /// Offset all indices by that amount (to accommodate zero-based arrays).
        /// </summary>
        private const float OpacityIndexOffset = 1f;

        #endregion

        #region Methods

        /// <summary>
        /// Draws an image transparently.
        /// </summary>
        /// <param name="g">Graphics where the image should be drawn</param>
        /// <param name="image">Image to be drawn</param>
        /// <param name="location">Location, where the image should be drawn</param>
        /// <param name="opacity">Opacity of the image</param>
        /// <param name="red">True if the image should be drawn in red</param>
        public static void DrawTransparentImage(Graphics g, Bitmap image, Rectangle location, float opacity,
            bool red = false)
        {
            // create new bitmap for the modified image
            var bmp = new Bitmap(image.Width, image.Height);

            // create the colormatrix to modify the image
            var matrix = new ColorMatrix {Matrix33 = opacity};
            if (red)
                matrix.Matrix30 = 0.5f;

            // create image attributes and set the colormatrix
            var att = new ImageAttributes();
            att.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            // draw the image
            g.DrawImage(image, new Rectangle(location.X, location.Y, location.Width, location.Width), 0, 0,
                image.Width, image.Height, GraphicsUnit.Pixel, att);
        }

        /// <summary>
        /// Converts an array index and length to a fraction to be used for its opacity.
        /// </summary>
        /// <param name="pathIndex">Index in the array</param>
        /// <param name="pathLength">Length of the array</param>
        /// <returns></returns>
        public static float GetPathOpacity(int pathIndex, int pathLength)
        {
            if (pathIndex >= pathLength)
                throw new ArgumentException("Invalid array data");

            // calculate fraction
            var frac = OpacityMinimum + (pathIndex + OpacityIndexOffset)/pathLength*(1 - OpacityMinimum);

            if (frac < 0 || frac < OpacityMinimum || frac > 1)
                throw new Exception("Invalid opacity calculated");

            return frac;
        }

        /// <summary>
        /// Gets the subarray of an array.
        /// </summary>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <param name="data">Array itself</param>
        /// <param name="index">Index, from where the subarray should be retrieved from</param>
        /// <param name="length">Length of the subarray</param>
        /// <returns>Subarray</returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        #endregion
    }
}
