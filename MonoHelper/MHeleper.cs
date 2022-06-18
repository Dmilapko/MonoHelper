﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;

namespace MonoHelper
{
    public static class MHeleper
    {
        public static string PutZeros(string StrAfterZeros, int SizeOfOutputStr)
        {
            while (StrAfterZeros.Length < SizeOfOutputStr) StrAfterZeros = "0" + StrAfterZeros;
            return StrAfterZeros;
        }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static double RandomDouble()
        {
            lock (syncLock)
            { // synchronize
                return random.NextDouble();
            }
        }

        public static Vector2 FlipY(this Vector2 vector)
        {
            vector.Y = 1080 - vector.Y;
            return vector;
        }

        public static Vector2 FlipX(this Vector2 vector)
        {
            vector.Y = 1920 - vector.Y;
            return vector;
        }

        /// <summary>
        /// Converts from degrees to radians
        /// </summary>
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }

        /// <summary>
        /// Converts from degrees to radians
        /// </summary>
        public static float ToRadians(this float val)
        {
            return (float)(Math.PI / 180) * val;
        }

        /// Converts from degrees to radians
        /// </summary>
        public static float ToRadians(this int val)
        {
            return (float)(Math.PI / 180) * val;
        }


        /// <summary>
        /// Converts from radians to degrees
        /// </summary>
        public static float ToDegrees(this float val)
        {
            return val/(float)Math.PI*180f;
        }

        /// <summary>
        /// Converts from radians to degrees
        /// </summary>
        public static float ToDegrees(this int val)
        {
            return val / (float)Math.PI * 180f;
        }

        /// <summary>
        /// Creates circle with smooth borders
        /// </summary>
        public static Texture2D CreateCircle(GraphicsDevice graphicsDevice, int radius, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius * 2, radius * 2);
            Color[] colorData = new Color[radius * radius * 4];

            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
                    int index = x * radius * 2 + y;
                    double ltc = Math.Sqrt(Math.Abs(x - radius + 0.5f) * Math.Abs(x - radius + 0.5f) + Math.Abs(y - radius + 0.5f) * Math.Abs(y - radius + 0.5f)) + 0.5f;
                    if (ltc <= radius)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        if (ltc <= radius + 1)
                        {
                            colorData[index] = new Color(color, radius + 1 - (float)ltc);
                        }
                        else colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        /// <summary>
        /// Creates circle with smooth borders
        /// </summary>
        private static Color[] CreateCircle(this Color[] colorData, int radius, Color color)
        {
            int initsize = (int)Math.Sqrt(colorData.Length) / 2;
            for (int x = 0; x < radius * 2; x++)
            {
                for (int y = 0; y < radius * 2; y++)
                {
                    int index = (x + (initsize - radius)) * initsize * 2 + (y + (initsize - radius));
                    double ltc = Math.Sqrt(Math.Abs(x - radius + 0.5f) * Math.Abs(x - radius + 0.5f) + Math.Abs(y - radius + 0.5f) * Math.Abs(y - radius + 0.5f)) + 0.5f;
                    if (ltc <= radius)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        if (ltc <= radius + 1)
                        {
                            if (colorData[index] == Color.Transparent) colorData[index] = new Color(color, radius + 1 - (float)ltc);
                            else colorData[index] = MixTwoColorsNA(new Color(color, MathHelper.Clamp((radius + 1 - (float)ltc) / Brightness(colorData[index], 1), 0, 1)), colorData[index]);
                        }
                    }
                }
            }

            return colorData;
        }
        /// <summary>
        /// Creates circle with outline
        /// </summary>
        public static Texture2D CreateCircle(GraphicsDevice graphicsDevice, int radius, Color color, int outlinewidth, Color outlinecolor)
        {
            Texture2D texture = new Texture2D(graphicsDevice, radius * 2, radius * 2);
            Color[] colorData = new Color[radius * radius * 4];

            colorData.CreateCircle(radius, outlinecolor);
            colorData.CreateCircle(radius - outlinewidth, color);

            texture.SetData(colorData);
            return texture;
        }

        /// <summary>
        /// Converts TIME in MS to string min:sec:ms
        /// </summary>
        public static string GetTimeMinSecMs(this int time)
        {
            int min = 0, sec = 0, ms = 0;
            min = time / 60000;
            time = time % 60000;
            sec = time / 1000;
            ms = time % 1000;
            if (min != 0) return min.ToString() + ":" + PutZeros(sec.ToString(), 2) + "." + PutZeros(ms.ToString(), 3);
            else return sec.ToString() + "." + PutZeros(ms.ToString(), 3);
        }

        /// <summary>
        /// Returns height in pixels of FONT
        /// </summary>
        public static int GetFontHeight(SpriteFont font)
        {
            return (int)Math.Round(font.MeasureString("Gg").Y * 0.8);
        }

        /// <summary>
        /// Returns width in pixels of TEXT with FONT;
        /// </summary>
        /// <param name="font"></param>
        /// <param name="_fontinitsize">initial size of FONT</param>
        /// <param name="_fontsize">scaled size of FONT</param>
        /// <returns></returns>
        public static int GetFontHeight(SpriteFont font, int _fontinitsize, int _fontsize)
        {
            return (int)Math.Round(font.MeasureString("Wg").Y / (float)_fontinitsize * (float)_fontsize);
        }

        /// <summary>
        /// Returns width in pixels of TEXT with FONT;
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="_fontinitsize">initial size of FONT</param>
        /// <param name="_fontsize">scaled size of FONT</param>
        /// <returns></returns>
        public static int GetTextWidth(string text, SpriteFont font, int _fontinitsize, int _fontsize)
        {
            return (int)Math.Round(font.MeasureString(text).X / (float)_fontinitsize * (float)_fontsize);
        }

        /// <summary>
        /// Changes COLOR to Color(0, 0, 0, 0)
        /// </summary>
        public static Texture2D MakeTransparent(this Texture2D texture, Color color)
        {
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            for (int i = 0; i < cd.Length; i++)
                if (cd[i] == color)
                    cd[i] = new Color(0, 0, 0, 0);
            texture.SetData(cd);
            return texture;
        }

        /// <summary>
        /// Changes COLOR to TRANSPARENTCOLOR
        /// </summary>
        public static Texture2D MakeTransparent(this Texture2D texture, Color color, Color transparentcolor)
        {
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            for (int i = 0; i < cd.Length; i++)
                if (cd[i] == color)
                    cd[i] = transparentcolor;
            texture.SetData(cd);
            return texture;
        }

        /// <summary>
        /// Returns brightness [0; MAXBRIGHTNESS]
        /// </summary>
        public static float Brightness(this Color color, float maxbrightness)
        {
            return (color.R + color.G + color.B) / 255f / 3f * maxbrightness;
        }

        /// <summary>
        /// Flips texture
        /// </summary>
        public static Texture2D SaveAsFlippedTexture2D(this Texture2D input, bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(input.GraphicsDevice, input.Width, input.Height);
            Color[] data = new Color[input.Width * input.Height];
            Color[] flipped_data = new Color[data.Length];

            input.GetData<Color>(data);

            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    int index = 0;
                    if (horizontal && vertical)
                        index = input.Width - 1 - x + (input.Height - 1 - y) * input.Width;
                    else if (horizontal && !vertical)
                        index = input.Width - 1 - x + y * input.Width;
                    else if (!horizontal && vertical)
                        index = x + (input.Height - 1 - y) * input.Width;
                    else if (!horizontal && !vertical)
                        index = x + y * input.Width;

                    flipped_data[x + y * input.Width] = data[index];
                }
            }

            flipped.SetData<Color>(flipped_data);

            return flipped;
        }

        /// <summary>
        /// Make borders smooth
        /// Requires MakeTransparent!!!
        /// </summary>
        public static Texture2D SmoothBorders(this Texture2D texture, Color BackgroundColor)
        {
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            texture.SmoothRight(cd, 1, BackgroundColor);
            texture.SmoothRight(cd, -1, BackgroundColor);
            texture.SmoothDown(cd, 1, BackgroundColor);
            texture.SmoothDown(cd, -1, BackgroundColor);
            return texture;
        }

        public static Color MixTwoColors(Color color, Color blend)
        {
            if (blend.A != 0) return new Color(
                 (byte)MathHelper.Clamp((color.R * color.A + blend.R * blend.A) / 2f / 255f, 0, 255),
                 (byte)MathHelper.Clamp((color.G * color.A + blend.G * blend.A) / 2f / 255f, 0, 255),
                 (byte)MathHelper.Clamp((color.B * color.A + blend.B * blend.A) / 2f / 255f, 0, 255),
                 (byte)MathHelper.Clamp((color.A + blend.A) / 2, 0, 255));
            else return color;
        }

        public static Color MixTwoColorsNA(Color color, Color blend)
        {
            return new Color(
                 (byte)MathHelper.Clamp((color.R * color.A + blend.R * blend.A) / (blend.A + color.A), 0, 255),
                 (byte)MathHelper.Clamp((color.G * color.A + blend.G * blend.A) / (blend.A + color.A), 0, 255),
                 (byte)MathHelper.Clamp((color.B * color.A + blend.B * blend.A) / (blend.A + color.A), 0, 255));
        }

        public static Texture2D SmoothDown(this Texture2D texture, Color[] cd, int stepdir, Color TransColor)
        {
            Color[] cdnow = new Color[texture.Width * texture.Height];
            texture.GetData(cdnow);
            for (int y = 1; y < texture.Height - 1 + stepdir; y++)
            {
                for (int x = 1; x < texture.Width; x++)
                {
                    if (cd[y * texture.Width + x] == TransColor)
                    {
                        if (cd[(y - stepdir) * texture.Width + x] != TransColor)
                        {
                            int bytop = 0, bybottom = 0;
                            if (x != 0)
                            {
                                int ny = y;
                                while ((cd[ny * texture.Width + x - 1] != TransColor) && (cd[ny * texture.Width + x] == TransColor))
                                {
                                    ny += stepdir;
                                    bytop++;
                                }
                            }
                            if (x != texture.Width)
                            {
                                int ny = y;
                                while ((cd[ny * texture.Width + x + 1] != TransColor) && (cd[ny * texture.Width + x] == TransColor))
                                {
                                    ny += stepdir;
                                    bybottom++;
                                }
                            }
                            //int max = (int)(Math.Max(bytop, bybottom) * cd[y * texture.Width + x - 1].Brightness(5))+1;
                            int max = Math.Max(bytop, bybottom);
                            //if (max < 2) max = 2;
                            for (int ny = 0; Math.Abs(ny) < max - 1; ny += stepdir)
                            {
                                Color insertcolor = new Color(cd[(y - stepdir) * texture.Width + x], (max - Math.Abs(ny) - 1) / (float)max);
                                Color backcolor = cdnow[(ny + y) * texture.Width + x];
                                if (backcolor.A != 0) cdnow[(ny + y) * texture.Width + x] = MixTwoColors(insertcolor, backcolor);
                                else cdnow[(ny + y) * texture.Width + x] = insertcolor;
                            }
                        }

                    }
                }
            }
            texture.SetData(cdnow);
            return texture;
        }

        public static Texture2D SmoothRight(this Texture2D texture, Color[] cd, int stepdir, Color TransColor)
        {
            Color[] cdnow = new Color[texture.Width * texture.Height];
            texture.GetData(cdnow);
            for (int y = 1; y < texture.Height; y++)
            {
                for (int x = 1; x < texture.Width - 1 + stepdir - 1; x++)
                {
                    if (cd[y * texture.Width + x] == TransColor)
                    {
                        if (cd[y * texture.Width + x - stepdir] != TransColor)
                        {
                            int bytop = 0, bybottom = 0;
                            if (y != 0)
                            {
                                int nx = x;
                                while ((cd[(y - 1) * texture.Width + nx] != TransColor) && (cd[(y) * texture.Width + nx] == TransColor))
                                {
                                    nx += stepdir;
                                    bytop++;
                                }
                            }
                            if (y != texture.Height)
                            {
                                int nx = x;
                                while ((cd[(y + 1) * texture.Width + nx] != TransColor) && (cd[(y) * texture.Width + nx] == TransColor))
                                {
                                    nx += stepdir;
                                    bybottom++;
                                }
                            }
                            //int max = (int)(Math.Max(bytop, bybottom) * cd[y * texture.Width + x - 1].Brightness(5))+1;
                            int max = Math.Max(bytop, bybottom);
                            //if (max < 2) max = 2;
                            for (int nx = 0; Math.Abs(nx) < max - 1; nx += stepdir)
                            {
                                Color insertcolor = new Color(cd[y * texture.Width + x - stepdir], (max - Math.Abs(nx) - 1) / (float)max);
                                Color backcolor = cdnow[y * texture.Width + x + nx];
                                if (backcolor.A != 0) cdnow[y * texture.Width + x + nx] = MixTwoColors(insertcolor, backcolor);
                                else cdnow[y * texture.Width + x + nx] = new Color(cd[y * texture.Width + x - stepdir], (max - Math.Abs(nx) - 1) / (float)max);
                            }

                        }
                    }
                }
            }
            texture.SetData(cdnow);
            return texture;
        }


        /// <summary>
        /// Converts List of Vector2 to List of System.Drawing.PointF
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<System.Drawing.PointF> ToPoints(this List<Vector2> points)
        {
            List<System.Drawing.PointF> res = new List<System.Drawing.PointF>();
            foreach (var point in points)
            {
                res.Add(new System.Drawing.PointF(point.X, point.Y));
            }
            return res;
        }

        public static Vector2 ToVector(this System.Drawing.PointF point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static System.Drawing.PointF ToPointF(this Vector2 vector)
        {
            return new System.Drawing.PointF(vector.X, vector.Y);
        }

        /// <summary>
        /// Converts Color to System.Drawing.Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Drawing.Color ToColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        static public Texture2D CreateCurve(GraphicsDevice graphics, int width, int height, List<Vector2> points, Color color)
        {
            System.Drawing.Image image = new System.Drawing.Bitmap(width, height);
            Texture2D res = new Texture2D(graphics, width, height);
            Color[] data = new Color[width * height];
            res.GetData(data);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                // draw in bmp using g
                g.DrawCurve(new System.Drawing.Pen(color.ToColor()), points.ToPoints().ToArray());
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image))
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            System.Drawing.Color clr = bmp.GetPixel(x, y); // Get the color of pixel at position 5,5
                            int red = clr.R;
                            int green = clr.G;
                            int blue = clr.B;
                            int alpha = clr.A;
                            data[y * width + x] = new Color(red, green, blue, alpha);
                        }
                    }
                }
            }
            res.SetData(data);
            return res;
        }

        public static bool InRect(this Vector2 pos, int width, int height)
        {
            return ((pos.X >= 0) && (pos.Y >= 0) && (pos.X < width) && (pos.Y < height));
        }

        public static bool InRect(this int pos, int width, int height)
        {
            int x = pos % width, y = pos / width;
            return ((x >= 0) && (y >= 0) && (x < width) && (y < height));
        }

        public static Texture2D Fill(this Texture2D texture, Vector2 position, Color color_from, Color color_to)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            List<int> target = new List<int>();
            target.Add((int)(position.Y * texture.Width + position.X));
            while (target.Count!=0)
            {
                List<int> next_target = new List<int>();
                foreach (int pos in target)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 vnpos = new Vector2((-1 + i) % 2 + (pos % texture.Width), (i % 2) + (pos / texture.Width));
                        if (i == 3) vnpos.Y-=2;
                        int npos = (int)(vnpos.Y * texture.Width + vnpos.X);
                        if (vnpos.InRect(texture.Width, texture.Height) && (data[npos] == color_from))
                        {
                            next_target.Add(npos);
                            data[npos] = color_to;
                        }
                    }
                }
                target = next_target;
            }
            texture.SetData(data);
            return texture;
        }

        public static Texture2D FillNot(this Texture2D texture, Vector2 position, Color color_notfill, Color color_to)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            HashSet<Vector2> target = new HashSet<Vector2>();
            target.Add(position);
            while (target.Count != 0)
            {
                HashSet<Vector2> next_target = new HashSet<Vector2>();
                foreach (var pos in target)
                {
                    data[(int)(pos.Y * texture.Width + pos.X)] = color_to;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 npos = new Vector2((-1 + i) % 2 + pos.X, (i % 2) + pos.Y);
                        if (i == 3) npos.Y -= 2;
                        if (npos.InRect(texture.Width, texture.Height) && (data[(int)(npos.Y * texture.Width + npos.X)] != color_notfill))
                        {
                            next_target.Add(npos);
                        }
                    }
                }
                target = next_target;
            }
            texture.SetData(data);
            return texture;
        }

        public static List<List<bool>> ToBoolMatrix(this Texture2D texture, List<Color> true_data)
        {
            Color[] cd = new Color[(uint)(texture.Width * texture.Height)];
            List<List<bool>> res = new List<List<bool>>();
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (y == 0) res.Add(new List<bool>());
                    if (true_data.Contains(cd[y*texture.Width + x]))
                    {
                        res[x].Add(true);
                    }
                    else res[x].Add(false);
                }
            }
            return res;
        }
    }
}