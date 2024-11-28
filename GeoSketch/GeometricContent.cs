using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSketch
{
    /// <summary>
    /// the cache of Texture2D objects
    /// </summary>
    public static class GeometricContent
    {
        private static readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        /// <summary>
        /// get's the (cached after first retrieval) Texture2D object of this object
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gv"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(SpriteBatch spriteBatch, GeometricVisual gv)
        {
            if (null == gv) return null;
            string key = gv.UniqueString();
            if (!_textures.Keys.Contains(key))
            {
                _textures.Add(key, gv.ComputeTexture(spriteBatch));
            }

            return _textures[key];
        }
    }
}
