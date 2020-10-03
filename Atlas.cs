using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Tga;
using System.ComponentModel;
using Point = System.Drawing.Point;
using Image = SixLabors.ImageSharp.Image;
using MyInterface;

namespace TextureAsset
{
    /// <summary>
    /// A Static Class to deal with Atlas texture
    /// </summary>
    public static class Atlas 
    {
        /// <summary>
        /// Read from an existing atlas and decode it as multiple squaresprite;        
        /// should contain one image and one meta file
        /// </summary>
        /// <param name="AtlasPath">
        /// the path of atlas
        /// </param>
        /// <returns></returns>
        public static List<TexImage> ReadFromAtlas(string AtlasPath)
        {
            string AtlasMeta = AtlasPath + ".meta";
            if (!File.Exists(AtlasPath))
            {
                Console.WriteLine("atlas image lost!");
                return null;
            }
            if (!File.Exists(AtlasMeta))
            {
                Console.WriteLine("atlas metafile lost!");
                return null;
            }

            List<TexImage> texImageset = new List<TexImage>();
            Texture texture = TextureSet.Generate_texture(AtlasPath);

            BinaryReader metafile = new BinaryReader(File.Open(AtlasMeta,FileMode.Open));
            TexImageInfo texImageInfo = new TexImageInfo();

            while (metafile.BaseStream.Position!=metafile.BaseStream.Length)
            {
                try
                {
                    texImageInfo.BinaryRead(metafile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("metafile corrupt!!" + ex.ToString());
                    return null;
                }
                TexImage texImage = new TexImage(texture, texImageInfo.filename, texImageInfo.LeftBottomLocation, texImageInfo.TopRightLocation);
                texImageset.Add(texImage);
            }

            metafile.Close();
            return texImageset;
        }
        /// <summary>
        /// Read from an existing atlas and decode it as multiple squaresprite;        
        /// should contain one image and one meta file
        /// </summary>
        /// <param name="AtlasPath">
        /// the path of atlas
        /// </param>
        /// <returns></returns>
        public static TexImage SingleReadFromAtlas(string AtlasPath,string SpriteName)
        {
            string AtlasMeta = AtlasPath + ".meta";
            if (!File.Exists(AtlasPath))
            {
                Console.WriteLine("atlas image lost!");
                return null;
            }
            if (!File.Exists(AtlasMeta))
            {
                Console.WriteLine("atlas metafile lost!");
                return null;
            }

            Texture texture = TextureSet.Generate_texture(AtlasPath);
            TexImage texImage = null;

            BinaryReader metafile = new BinaryReader(File.Open(AtlasMeta, FileMode.Open));
            TexImageInfo texImageInfo = new TexImageInfo();

            while (metafile.BaseStream.Position != metafile.BaseStream.Length)
            {
                try
                {
                    texImageInfo.BinaryRead(metafile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("metafile corrupt!!" + ex.ToString());
                    return null;
                }
                if (texImageInfo.filename == SpriteName)
                {
                    texImage = new TexImage(texture, texImageInfo.filename, texImageInfo.LeftBottomLocation, texImageInfo.TopRightLocation);
                    break;
                }
            }

            metafile.Close();
            return texImage;
        }

        /// <summary>
        /// Read from an existing atlas and decode it as multiple bitmap
        /// should contain one image and one meta file
        /// </summary>
        /// <param name="AtlasPath">
        /// the path of atlas
        /// </param>
        /// <returns></returns>
        public static Dictionary<string,Bitmap> ReadFromAtlasToBitmap(string AtlasPath)
        {
            string AtlasMeta = AtlasPath + ".meta";
            if (!File.Exists(AtlasPath))
            {
                Console.WriteLine("atlas image lost!");
                return null;
            }
            if (!File.Exists(AtlasMeta))
            {
                Console.WriteLine("atlas metafile lost!");
                return null;
            }
            Dictionary<string, Bitmap> keyValuePairs = new Dictionary<string, Bitmap>();
            Image<Rgba32> Origin_image = Image.Load<Rgba32>(AtlasPath);

            BinaryReader metafile = new BinaryReader(File.Open(AtlasMeta, FileMode.Open));

            TexImageInfo texImageInfo = new TexImageInfo();
            while (metafile.BaseStream.Position != metafile.BaseStream.Length)
            {
                try
                {
                    texImageInfo.BinaryRead(metafile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("metafile corrupt!!" + ex.ToString());
                    return null;
                }
                int SubImageWidth = texImageInfo.TopRightLocation.X - texImageInfo.LeftBottomLocation.X + 1;
                int SubImageHeight = texImageInfo.TopRightLocation.Y - texImageInfo.LeftBottomLocation.Y + 1;

                Image<Rgba32> image = new Image<Rgba32>(SubImageWidth, SubImageHeight);
                for (int i = 0; i < SubImageWidth; i++)
                {
                    for (int j = 0; j < SubImageHeight; j++)
                    {
                        try
                        {
                            image[i, j] = Origin_image[texImageInfo.LeftBottomLocation.X + i, Origin_image.Height - texImageInfo.TopRightLocation.Y + j];
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("error convert atlas to bitmap" + ex.ToString());
                            return null;
                        }
                    }
                }
                //image.Mutate(i => i.Crop(new SixLabors.ImageSharp.Rectangle(LeftBottomLocation.X, Origin_image.Height - TopRightLocation.Y , SubImageWidth, SubImageHeight)));
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, new PngEncoder());

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    keyValuePairs.Add(texImageInfo.filename, new Bitmap(memoryStream));
                }
                image.Dispose();
            }

            Origin_image.Dispose();
            metafile.Close();
            return keyValuePairs;
        }

        /// <summary>
        /// Make an atlas from img files in one existing directory
        /// the image file types supported is *.jpeg*.jpg*.png*.bmp*.tga
        /// </summary>
        /// <param name="AtlasPath">
        /// the path to atlas
        /// </param>
        /// <param name="folderpath">
        /// target path which contains images
        /// </param>
        /// <param name="imageWidth">
        /// the gap between images on atlas,default is 1
        /// </param>
        /// <param name="imageHeight">
        /// the width of output,defualt is 8192 
        /// </param>
        /// <param name="gap">
        /// the height of output,defualt is 4096
        /// </param>
        /// <param name="imagetype">
        /// the filetype encoder of atlas,default is null (which leads to png format)
        /// </param>
        public static void MakeAtlas(string AtlasPath, string folderpath, uint imageWidth = 8192, uint imageHeight = 4096, uint gap = 1, IImageEncoder imagetype = null)
        {
            //check for valid input
            if (!Directory.Exists(AtlasPath))
            {
                Console.WriteLine("Error accessing atlas path");
                return;
            }
            if (!Directory.Exists(folderpath))
            {
                Console.WriteLine("Error accessing target folder path");
                return;
            }
            Point canvassize = new Point((int)imageWidth, (int)imageHeight);
            string foldername = new DirectoryInfo(folderpath).Name;

            //setting output image encoder;
            string atlasformat = null;
            if (imagetype == null)
            {
                imagetype = new PngEncoder();
            }
            switch (TypeDescriptor.GetClassName(imagetype))
            {
                case "SixLabors.ImageSharp.Formats.Png.PngEncoder":
                    atlasformat = ".png";
                    break;
                case "SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder":
                    atlasformat = ".jpeg";
                    break;
                case "SixLabors.ImageSharp.Formats.Bmp.BmpEncoder":
                    atlasformat = ".bmp";
                    break;
                case "SixLabors.ImageSharp.Formats.Gif.GifEncoder":
                    atlasformat = ".gif";
                    break;
                case "SixLabors.ImageSharp.Formats.Tga.TgaEncoder":
                    atlasformat = ".tga";
                    break;
                default:
                    atlasformat = ".png";
                    break;
            }

            FileStream imagefile = new FileStream(AtlasPath + @"\" + foldername + atlasformat, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter metafile = new BinaryWriter(File.Open(AtlasPath + @"\" + foldername + atlasformat + ".meta",FileMode.Truncate));


            //check for image's extension first 
            string[] imageExtensions = { ".png", ".bmp", ".tga", ".jpeg", ".jpg" };
            Dictionary<string, Image<Rgba32>> ImageSet = new Dictionary<string, Image<Rgba32>>();

            foreach (string str in Directory.EnumerateFiles(folderpath, "*" ).Where(s => imageExtensions.Any(ext => ext == Path.GetExtension(s))))
            {
                Image<Rgba32> image = Image.Load<Rgba32>(str);
                if (image != null)
                {
                    ImageSet.Add(str, image);
                }
            }
            Dictionary<string, Image<Rgba32>> SortedImageSet = ImageSet.OrderByDescending(o => o.Value.Width * canvassize.Y + o.Value.Height).ToDictionary(o => o.Key, o => o.Value);
            

            Image<Rgba32> A = new Image<Rgba32>(canvassize.X, canvassize.Y);
            //int horizonoffset = 0;
            int horizontalOffset_precached = 0;
            Point Offset = new Point(0, 0);
            //Point PreviousOffset = Offset;

            foreach (KeyValuePair<string, Image<Rgba32>> keyValuePair in SortedImageSet)
            {
                if (horizontalOffset_precached < keyValuePair.Value.Width + (int)gap)
                {
                    horizontalOffset_precached = keyValuePair.Value.Width + (int)gap;
                }

                if (Offset.Y + keyValuePair.Value.Height >= canvassize.Y)
                {
                    Offset.X += horizontalOffset_precached;
                    Offset.Y = 0;
                }
                for (int i = 0; i < keyValuePair.Value.Width; i++)
                {
                    for (int j = 0; j < keyValuePair.Value.Height; j++)
                    {
                        try
                        {
                            A[Offset.X + i, Offset.Y + j] = keyValuePair.Value[i, j];
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("image size is too small,cannot make the atlas \n" + ex.ToString());
                            return;
                        }
                    }
                }

                //List<string> strset = keyValuePair.Key.Split(@"\").ToList<string>();
                //strset.RemoveAt(0);
                //string _string = string.Join(@"\", strset);
                //metafile.Write("{0}:({1},{2}),({3},{4})\n", _string, Offset.X, Offset.Y, Offset.X + keyValuePair.Value.Width - 1, Offset.Y + keyValuePair.Value.Height - 1);
                string str = Path.GetFileNameWithoutExtension(keyValuePair.Key);


                //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
                //This will correct that, making the texture display properly.

                new TexImageInfo(str, new Point(Offset.X, (int)imageHeight - (Offset.Y + keyValuePair.Value.Height - 1)), new Point(Offset.X + keyValuePair.Value.Width - 1, (int)imageHeight - Offset.Y)).BinaryWrite(metafile);
               
                Offset.Y += keyValuePair.Value.Height + (int)gap;
            }

            //save meta file
            A.Save(imagefile, imagetype);
            metafile.Flush();

            imagefile.Close();
            A.Dispose();
            metafile.Close();
        }
    }

    //[Serializable]
    //class AtlasInfo:ISerializable
    //{
    //    internal readonly string filename = "";
    //    internal readonly Point LeftBottomLocation = new Point();
    //    internal readonly Point TopRightLocation = new Point();

    //    public void GetObjectData(SerializationInfo info, StreamingContext context)
    //    {
    //        info.AddValue("filename", filename, typeof(string));
    //        info.AddValue("LeftBottomLocation", LeftBottomLocation, typeof(Point));
    //        info.AddValue("TopRightLocation", TopRightLocation, typeof(Point));
    //    }

    //    public AtlasInfo(SerializationInfo info, StreamingContext context)
    //    {
    //        filename = (string)info.GetValue("filename", typeof(string));
    //        LeftBottomLocation = (Point)info.GetValue("LeftBottomLocation", typeof(Point));
    //        TopRightLocation = (Point)info.GetValue("TopRightLocation", typeof(Point));
    //    }

    //    public AtlasInfo()
    //    {
    //    }
    //} 
    class TexImageInfo: IBinarySavable
    {
        public string filename { get; protected set; }
        public Point LeftBottomLocation { get; protected set; }
        public Point TopRightLocation { get; protected set; }

        public void BinaryWrite(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(filename);
            binaryWriter.Write(LeftBottomLocation.X);
            binaryWriter.Write(LeftBottomLocation.Y);
            binaryWriter.Write(TopRightLocation.X);
            binaryWriter.Write(TopRightLocation.Y);
        }

        public void BinaryRead(BinaryReader binaryReader)
        {
            filename = binaryReader.ReadString();
            Point temp = new Point();
            temp.X = binaryReader.ReadInt32();
            temp.Y = binaryReader.ReadInt32();
            LeftBottomLocation = temp;
            temp.X = binaryReader.ReadInt32();
            temp.Y = binaryReader.ReadInt32();
            TopRightLocation = temp;
        }

        public TexImageInfo()
        {

        }
        public TexImageInfo(string filename, Point LeftBottomLocation, Point TopRightLocation)
        {
            this.filename = filename;
            this.LeftBottomLocation = LeftBottomLocation;
            this.TopRightLocation = TopRightLocation;
        }
    }


}
