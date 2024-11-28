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
    /// actually a rectangle, but to avoid a naming conflict with xna.framework.rectangle
    /// we choose the name Quadrangle
    /// </summary>
    public class Quadrangle : GeometricVisual
    {
        /// <summary>
        /// rectangle defining the Quadrangle
        /// </summary>
        public Rectangle Rectangle { get; set; }

        /// <summary>
        /// width of the Texture2D
        /// </summary>
        public override int TextureWidth => Rectangle.Width;

        /// <summary>
        /// height of the Texture2D
        /// </summary>
        public override int TextureHeight => Rectangle.Height;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="spriteBatch"></param>
        public Quadrangle(Rectangle rectangle, Color fillColor, Color strokeColor, int strokeThickness, SpriteBatch spriteBatch)
        {
            Rectangle = rectangle;
            FillColor = fillColor;
            StrokeColor = strokeColor;
            StrokeThickness = strokeThickness;

            Sprite = GeometricContent.GetTexture(spriteBatch, this);
        }

        /// <summary>
        /// top left position of the rectangle
        /// </summary>
        public override Vector2 TextureTopLeftPosition
        {
            get { return new Vector2(Rectangle.X, Rectangle.Y); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="fillColor"></param>
        /// <param name="spriteBatch"></param>
        public Quadrangle(Rectangle rectangle, Color fillColor, SpriteBatch spriteBatch)
            : this(rectangle, fillColor, Color.Transparent, 0, spriteBatch)
        {

        }

        /// <summary>
        /// uniqely identifies the current object (this will be the used key in the Texture2D cache)
        /// </summary>
        /// <returns></returns>
        public override string UniqueString()
        {
            Rectangle rectangle00= new Rectangle(0,0,Rectangle.Width,Rectangle.Height);
            return this.GetType().ToString() + rectangle00.ToString() + FillColor + StrokeThickness + StrokeColor;
        }

        internal override void DrawInTextureBuffer(Color[] textureBuffer)
        {
            int width = Rectangle.Width;
            int height = Rectangle.Height;

            int strokeThickness = Math.Max(0, StrokeThickness);
            strokeThickness = Math.Min(strokeThickness, width / 2);

            //inner rectangle: fill
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if (y < strokeThickness                     //top border
                        || y > height - strokeThickness - 1        //bottom border
                        || x < strokeThickness                  //left border
                        || x > width - strokeThickness - 1         //right border
                        )
                    {
                        textureBuffer[y * width + x] = StrokeColor;
                    }
                    else
                    {
                        textureBuffer[y * width + x] = FillColor;
                    }
                }
        }
    }
}
