using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;

namespace SharpDXControl
{
    public static class ImageLoader
    {
        private static Dictionary<string, int[]> ImageCash = new Dictionary<string, int[]>();

        private static int FromColor4(Color4 color)
        {
            byte B = (byte)(color.Blue * 255);
            byte G = (byte)(color.Green * 255);
            byte R = (byte)(color.Red * 255);
            byte A = (byte)(color.Alpha * 255);
            return R | (G << 8) | (B << 16) | (A << 24);
        }

        private static Bitmap CreateBitmap(RenderTarget renderTarget, string key, Size2 size, Color4 color, Func<int, int, bool> pred)
        {
            if (!ImageCash.ContainsKey(key))
            {
                var imageInt = new int[size.Width * size.Height];

                // Convert all pixels 
                for (int y = 0; y < size.Height; y++)
                {
                    for (int x = 0; x < size.Width; x++)
                    {
                        var rgba = pred(y, x) ? FromColor4(color) : 0;
                        imageInt[size.Width * y + x] = rgba;
                    }
                }

                ImageCash.Add(key, imageInt);
            }


            int stride = size.Width * sizeof(int);

            using (var tempStream = new DataStream(stride * size.Height, true, true))
            {
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));

                var cash = ImageCash[key];

                foreach (int t in cash) { tempStream.Write(t); }

                tempStream.Position = 0;

                return new Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
            }
        }

        /// <summary>
        /// 左と上のみ線が入ったBitmapを作成する
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Bitmap Grid(RenderTarget renderTarget, string key, Size2 size, Color4 color)
        {
            return CreateBitmap(renderTarget, key, size, color, (y, x) => y == 0 || x == 0);
        }

        /// <summary>
        /// 塗りつぶされたBitmapを作成する
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Bitmap Cell(RenderTarget renderTarget, string key, Size2 size, Color4 color)
        {
            return CreateBitmap(renderTarget, key, size, color, (_, __) => true);
        }

        /// <summary>
        /// System.Drawing.ImageからSharpDX.Direct2D1.Bitmapを作成する
        /// </summary>
        /// <param name="renderTarget"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap LoadFromImage(RenderTarget renderTarget, string key, System.Drawing.Bitmap bitmap)
        {
            if (!ImageCash.ContainsKey(key))
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

                var imageint = new int[bitmap.Width * bitmap.Height];

                // Lock System.Drawing.Bitmap
                var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                // Convert all pixels 
                for (int y = 0; y < bitmap.Height; y++)
                {
                    int offset = bitmapData.Stride * y;
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        // Not optimized 
                        byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        int rgba = R | (G << 8) | (B << 16) | (A << 24);
                        imageint[bitmap.Width * y + x] = rgba;
                    }

                }
                bitmap.UnlockBits(bitmapData);
                ImageCash.Add(key, imageint);
            }

            // Transform pixels from BGRA to RGBA
            int stride = bitmap.Width * sizeof(int);
            using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
            {
                var bitmapProperties =
                    new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied));

                var size = new Size2(bitmap.Width, bitmap.Height);

                var cash = ImageCash[key];

                foreach (var rgba in cash) { tempStream.Write(rgba); }

                tempStream.Position = 0;

                return new Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
            }


        }

        /// <summary>
        /// Loads a Direct2D Bitmap from a file using System.Drawing.Image.FromFile(...)
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="file">The file.</param>
        /// <returns>A D2D1 Bitmap</returns>
        public static Bitmap LoadFromFile(RenderTarget renderTarget, string file)
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(file))
            {
                return LoadFromImage(renderTarget, file, bitmap);
            }
        }
    }
}
