using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GeoSketch
{
    /// <summary>
    /// aroot of the drawing classes
    /// </summary>
    public abstract class GeometricVisual
    {
        #region properties and fields
        /// <summary>
        /// the inner color of the object
        /// </summary>
        public Color FillColor { get; set; }
        /// <summary>
        /// the thickness of the stroke of the object
        /// </summary>
        public int StrokeThickness { get; set; } = 0;
        /// <summary>
        /// the stroke color of the object
        /// </summary>
        public Color StrokeColor { get; set; } = Color.Transparent;
        /// <summary>
        /// the topleft position of the unrotated boxing rectangle of the object
        /// </summary>
        public abstract Vector2 TextureTopLeftPosition { get; }

        /// <summary>
        /// the unrotated boxing rectangle of the object
        /// </summary>
        protected virtual Rectangle DrawingRectangle
        {
            get { return new Rectangle(TextureTopLeftPosition.ToPoint(), new Point(Sprite.Width, Sprite.Height)); }
        }
        /// <summary>
        /// the Texture2D visualization of the object
        /// </summary>
        public Texture2D Sprite { get; set; }
        //public virtual Texture2D Sprite { get; set; }
        /// <summary>
        /// the width of the unrotated boxing rectangle
        /// </summary>
        public abstract int TextureWidth { get; }
        /// <summary>
        /// the height of the unrotated boxing rectangle
        /// </summary>
        public abstract int TextureHeight { get; }
        #endregion

        #region public/protected methods

        /// <summary>
        /// compute the boxing texture for this GeometricVisual
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <returns></returns>
        public Texture2D ComputeTexture(SpriteBatch spriteBatch)
        {
            Color[] textureBuffer = GetTransparantTextureBuffer();
            DrawInTextureBuffer(textureBuffer);
            return TextureBufferToTexture2D(spriteBatch, textureBuffer);
        }

        internal abstract void DrawInTextureBuffer(Color[] textureBuffer);


        /// <summary>
        /// copy the texturebuffer array to a Texture2D object
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="textureBuffer"></param>
        /// <returns></returns>
        private Texture2D TextureBufferToTexture2D(SpriteBatch spriteBatch, Color[] textureBuffer)
        {
            Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, TextureWidth, TextureHeight);
            texture.SetData(textureBuffer);
            return texture;
        }

        /// <summary>
        /// get a transparant texture buffer for this visual (a rectangle based on TextureWidth and TextureHeight)
        /// </summary>
        /// <returns></returns>
        public virtual Color[] GetTransparantTextureBuffer()
        {
            //the  texture is a square
            Color[] textureBuffer = new Color[TextureWidth * TextureHeight];
            for (int i = 0; i < textureBuffer.Length; i++)
                textureBuffer[i] = Color.Transparent;

            return textureBuffer;
        }
        /// <summary>
        /// Draw the object
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, DrawingRectangle, Color.White);
        }
        /// <summary>
        /// uniqely identifies the current object (this will be the used key in the Texture2D cache)
        /// </summary>
        /// <returns></returns>
        public abstract string UniqueString();
        /// <summary>
        /// string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return UniqueString();
        }
        #endregion

        internal (Vector2 leftPoint,Vector2 rightPoint) SortLeftToRight(Vector2 x, Vector2 y)
        {
            if (x.X > y.X)
                return  (y, x);
            else
                return (x, y);
        }
        internal (float xSlope, float ySlope) GetSlopes(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return  ((v2.X - v1.X) / (v2.Y - v1.Y), (v3.X - v1.X) / (v3.Y - v1.Y));
        }
    }
}
