using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;

namespace TextureAsset
{
    internal class Texture
    {
        public readonly int handle;
        public readonly bool IsTransparent = false;
        public readonly string OriginFileName;
        public readonly int Width;
        public readonly int Height;

        internal Texture(string str)
        {
            //had to initialize a gamewindow before using GL function; 
            //GameWindow g = new GameWindow();

            handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            //Load the image

            OriginFileName = str;
            Image<Rgba32> image = Image.Load<Rgba32>(str);
            this.Width = image.Width;
            this.Height = image.Height;
            //Console.WriteLine(Dilation);

            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            //This will correct that, making the texture display properly.
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            //Get an array of the pixels, in ImageSharp's internal format.
            List<Rgba32> tempPixels = new List<Rgba32>();
            for (int i = 0; i < image.Height; i++)
            {
                tempPixels.AddRange(image.GetPixelRowSpan(i).ToArray());
            }
            //Convert ImageSharp's format into a byte array, so we can use it with OpenGL.
            List<byte> pixels = new List<byte>();

            foreach (Rgba32 p in tempPixels)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
                if (p.A < (byte)255)
                {
                    IsTransparent = true;
                }
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
               PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            image.Dispose();
            tempPixels.Clear();
            pixels.Clear();
            GL.BindTexture(TextureTarget.Texture2D, 0);
            //g.Close();
            //Console.WriteLine(tempPixels.Count);
            //Console.WriteLine(pixels[3].ToString());
        }
    }
}
