using System;
using System.Collections.Generic;
using System.Text;

namespace TextureAsset
{
    static class TextureSet
    {
        readonly static List<Texture> textureset = new List<Texture>();
        internal static Texture Generate_texture(string str)
        {
            bool f(Texture texture)
            {
                if (texture.OriginFileName == str)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            Texture temp = textureset.Find(f);
            if (temp != null)
            {
                return temp;
            }
            else
            {
                temp = new Texture(str);
                if (temp == null)
                {
                    return null;
                }
                textureset.Add(temp);
                return temp;
            }
        }

        public static void Dispose()
        {
            textureset.Clear();
        }
    }
}
