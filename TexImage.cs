using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace TextureAsset
{
    /// <summary>
    /// 2d image raw data with no vertex setting
    /// </summary>
    public class TexImage
    {
        private readonly Texture texture;
        /// <summary>
        /// left bottom coordinate of teximage in texture
        /// </summary>
        public readonly Point LeftBottomLocation;
        /// <summary>
        /// top right coordinate of teximage in texture
        /// </summary>
        public readonly Point TopRightLocaion;
        /// <summary>
        /// the sprite name
        /// </summary>
        public readonly string SpriteName;
        /// <summary>
        /// the handle of texture
        /// </summary>
        public int TextureHandle { get { return texture.handle; } }
        /// <summary>
        /// the original texture size in pixel
        /// </summary>
        public Point TextureSize { get { return new Point(texture.Width,texture.Height); } }
        /// <summary>
        /// represent if the original texture is transparent or not
        /// </summary>
        public bool IsTransparent { get { return texture.IsTransparent; } }

        ////public readonly int transform_handle;
        //protected readonly int VertexBufferObject;
        //protected readonly int VertexArrayObject;
        //protected readonly int ElementBufferObject;
        ////protected readonly int rotation_handle;
        ////protected readonly int dilation_handle;
        //// public Vector3 World_Transform { get; protected set; }

        //protected readonly float[] vertices ={
        //    //Position          Texture coordinates
        //     0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
        //     0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        //    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        //    -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        //};
        //protected readonly uint[] indices = {
        //    0, 2, 1,   // first triangle
        //    0, 3, 2    // second triangle
        //};
        /// <summary>
        /// make a TexImage with a specific filepath
        /// </summary>
        /// <param name="filepath">the name of the 2d sprite</param>
        public TexImage(string filepath)
        {
            texture = TextureSet.Generate_texture(filepath);
            this.LeftBottomLocation = new Point();
            this.TopRightLocaion = new Point(texture.Width - 1, texture.Height - 1);
            SpriteName = texture.OriginFileName;
            //rotation_handle = GL.GetUniformLocation(maingame.shader.Handle, "rotation");
            //dilation_handle = GL.GetUniformLocation(maingame.shader.Handle, "dilation");
            //transform_handle = GL.GetUniformLocation(maingame.shader.Handle, "world_transform");
            //VertexBufferObject = GL.GenBuffer();
            //VertexArrayObject = GL.GenVertexArray();
            //ElementBufferObject = GL.GenBuffer();

            //GL.BindVertexArray(VertexArrayObject);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            //GL.VertexAttribPointer(maingame.vertCoordLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            //GL.VertexAttribPointer(maingame.texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            //GL.EnableVertexAttribArray(maingame.vertCoordLocation);
            //GL.EnableVertexAttribArray(maingame.texCoordLocation);

            //vertices[3] = right;
            //vertices[4] = top;
            //vertices[8] = right;
            //vertices[9] = bottom;
            //vertices[13] = left;
            //vertices[14] = bottom;
            //vertices[18] = left;
            //vertices[19] = top;
        }

        internal TexImage(Texture texture,string SpriteName,Point LeftBottomLocation,Point RightTopLocation)
        {
            this.texture = texture;
            this.LeftBottomLocation = LeftBottomLocation;
            this.TopRightLocaion = RightTopLocation;
            this.SpriteName = SpriteName;
        }
            //public virtual void Plot_Me()
            //{
            //    GL.BindVertexArray(VertexArrayObject);

            //    GL.ActiveTexture(TextureUnit.Texture0);
            //    GL.BindTexture(TextureTarget.Texture2D, texture.handle);

            //    GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //    GL.BindVertexArray(0);

            //}
    }
}
