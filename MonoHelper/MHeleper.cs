﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace MonoHelper
{
    public static class MHeleper
    {
        public static bool In<T>(this T obj, params T[] args)
        {
            return args.Contains(obj);
        }

        public static int GetSign(this int number)
        {
            if (number == 0) return 0;
            if (number < 0) return -1;
            else return 1;
        }
        public static int GetSign(this double number)
        {
            if (number == 0) return 0;
            if (number < 0) return -1;
            else return 1;
        }
        public static int GetSign(this float number)
        {
            if (number == 0) return 0;
            if (number < 0) return -1;
            else return 1;
        }

        public static string PutZeros(string StrAfterZeros, int SizeOfOutputStr)
        {
            while (StrAfterZeros.Length < SizeOfOutputStr) StrAfterZeros = "0" + StrAfterZeros;
            return StrAfterZeros;
        }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        /// <summary>
        /// Random double [0; 1]
        /// </summary>
        /// <returns></returns>
        public static double RandomDouble()
        {
            lock (syncLock)
            { // synchronize
                return random.NextDouble();
            }
        }

        public static Texture2D Clone(this Texture2D texture, GraphicsDevice graphics)
        {
            Texture2D res = new Texture2D(graphics, texture.Width, texture.Height);
            Color[] cd = new Color[texture.Width * texture.Height];
            texture.GetData(cd);
            res.SetData(cd);
            return res;
        }

        /// <summary>
        /// Creates filled circle with smooth borders
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
        /// Creates filled circle with smooth borders
        /// </summary>
        public static Color[] CreateCircle(this Color[] colorData, int radius, Color color)
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

        public static Texture2D DrawCircle(this Texture2D texture, Vector2 center, double radius, double width, Color color)
        {
            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData(colorData);
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    double dist = Math.Abs(Math.Sqrt(Math.Pow(center.X - x, 2) + Math.Pow(center.Y - y, 2)) - radius);
                    if (dist <= width) 
                    {
                        //                        colorData[x + y * texture.Width] = MixTwoColorsNA(colorData[x + y * texture.Width], new Color(color, (float)Math.Max(1d, width - dist)));
                        colorData[x + y * texture.Width] = new Color(color, (float)Math.Min(1d, width - dist));
                    }
                }
            }
            texture.SetData(colorData);
            return texture;
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

        /// <summary>
        /// Mixes two colors but output color didn't have A channel(only RGB)
        /// </summary>
        /// <param name="color"></param>
        /// <param name="blend"></param>
        /// <returns></returns>
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

        static public Texture2D CreateCurve(GraphicsDevice graphics, int width, int height, List<Vector2> points, Color color)
        {
            System.Drawing.Image image = new System.Drawing.Bitmap(width, height);
            Texture2D res = new Texture2D(graphics, width, height);
            Color[] data = new Color[width * height];
            res.GetData(data);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                // draw in bmp using g
                g.DrawCurve(new System.Drawing.Pen(color.ToColor()), points.ToPointFs().ToArray());
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

        public static List<int> CreateCurve(int width, int height, List<Vector2> points)
        {
            List<int> res = new List<int>();
            System.Drawing.Image image = new System.Drawing.Bitmap(width, height);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image))
            {
                // draw in bmp using g
                g.DrawCurve(new System.Drawing.Pen(System.Drawing.Color.Red), points.ToPointFs().ToArray());
                using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image))
                {
                    for (int x = 0; x < width; x++)
                    {
                        int y;
                        for (y = 0; y < height; y++)
                        {
                            if (y == 376)
                            {

                            }
                            if (bmp.GetPixel(x, y).R == 255) break;
                        }
                        res.Add(y);
                    }
                }
            }
            return res;
        }

        public static bool InRect(this Vector2 pos, int width, int height)
        {
            return ((pos.X >= 0) && (pos.Y >= 0) && (pos.X < width) && (pos.Y < height));
        }

        public static bool InRect(this Vector2 pos, Vector2 BiggerThan, Vector2 SmallerThan)
        {
            return ((pos.X >= BiggerThan.X) && (pos.Y >= BiggerThan.Y) && (pos.X < SmallerThan.X) && (pos.Y < SmallerThan.Y));
        }

        public static bool InRect(this PointD pos, int width, int height)
        {
            return ((pos.X >= 0) && (pos.Y >= 0) && (pos.X < width) && (pos.Y < height));
        }

        public static bool InRect(this PointD pos, Vector2 BiggerThan, Vector2 SmallerThan)
        {
            return ((pos.X >= BiggerThan.X) && (pos.Y >= BiggerThan.Y) && (pos.X < SmallerThan.X) && (pos.Y < SmallerThan.Y));
        }

        public static bool InRect(this int pos, int width, int height)
        {
            int x = pos % width, y = pos / width;
            return ((x >= 0) && (y >= 0) && (x < width) && (y < height));
        }

        public static void Fill(this Texture2D texture, Vector2 position, Color color_from, Color color_to)
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
        }

        public static void Fill(this Texture2D texture, Color color_to)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (int i = 0; i < texture.Width * texture.Height; i++)
            {
                data[i] = color_to;
            }
            texture.SetData(data);
        }

        public static void FillNot(this Texture2D texture, Vector2 position, Color color_notfill, Color color_to)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            List<int> target = new List<int>();
            target.Add((int)(position.Y * texture.Width + position.X));
            while (target.Count != 0)
            {
                List<int> next_target = new List<int>();
                foreach (int pos in target)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 vnpos = new Vector2((-1 + i) % 2 + (pos % texture.Width), (i % 2) + (pos / texture.Width));
                        if (i == 3) vnpos.Y -= 2;
                        int npos = (int)(vnpos.Y * texture.Width + vnpos.X);
                        if (vnpos.InRect(texture.Width, texture.Height) && (data[npos] != color_notfill) && (data[npos] != color_to))
                        {
                            next_target.Add(npos);
                            data[npos] = color_to;
                        }
                    }
                }
                target = next_target;
            }
            texture.SetData(data);
        }

        public static List<List<bool>> ToBoolMatrix(this Texture2D texture, List<Color> true_data)
        {
            Color[] cd = new Color[(uint)(texture.Width * texture.Height)];
            texture.GetData(cd);
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
