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
    /// class which supports creating a texture for a circle
    /// </summary>
    public class Circle : GeometricVisual
    {
        private double _arcStartAngle;

        /// <summary>
        /// ArcStartAngle indicates the starting angle of the arc wrt the x-axis
        /// </summary>
        protected double ArcStartAngle
        {
            get { return _arcStartAngle; }
            set
            {
                _arcStartAngle = value;
            }
        }

        private double _arcEndAngle;
        /// <summary>
        /// ArcEndAngle indicates the ending angle of the arc wrt the x-axis
        /// </summary>
        protected double ArcEndAngle
        {
            get { return _arcEndAngle; }
            set { _arcEndAngle = value; }
        }

        /// <summary>
        /// radius of the circle
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// centerpoint of the circle
        /// </summary>
        public Vector2 CenterPoint { get; set; }


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="radius"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="spriteBatch"></param>
        public Circle(Vector2 centerPoint, float radius, Color fillColor,
            Color strokeColor, int strokeThickness, SpriteBatch spriteBatch)
            : this(centerPoint, radius, fillColor, strokeColor, strokeThickness, 0,
                  Math.PI*2, spriteBatch)
        {
            //CenterPoint = centerPoint;
            //Radius = Math.Abs(radius);

            //FillColor = fillColor;
            //StrokeColor = strokeColor;
            //StrokeThickness = strokeThickness;

            //Sprite = GeometricContent.GetTexture(spriteBatch, this);
        }
        /// <summary>
        /// used for arcs
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="radius"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="arcStartAngle"></param>
        /// <param name="arcEndAngle"></param>
        /// <param name="spriteBatch"></param>
        protected Circle(Vector2 centerPoint, float radius, Color fillColor,
            Color strokeColor, int strokeThickness,
            double arcStartAngle, double arcEndAngle,
            SpriteBatch spriteBatch)
        {
            ArcStartAngle = arcStartAngle;
            ArcEndAngle = arcEndAngle;
            CenterPoint = centerPoint;
            Radius = Math.Abs(radius);

            FillColor = fillColor;
            StrokeColor = strokeColor;
            StrokeThickness = strokeThickness;

            Sprite = GeometricContent.GetTexture(spriteBatch, this);
        }

        /// <summary>
        /// top left position of the boxing rectangle
        /// </summary>
        public override Vector2 TextureTopLeftPosition
        {
            get { return CenterPoint - new Vector2(Radius, Radius); }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="radius"></param>
        /// <param name="fillColor"></param>
        /// <param name="spriteBatch"></param>
        public Circle(Vector2 centerPoint, float radius, Color fillColor, SpriteBatch spriteBatch) : this(centerPoint, radius, fillColor, Color.Transparent, 0, spriteBatch)
        {
        }

        /// <summary>
        /// uniqely identifies the current object (this will be the used key in the Texture2D cache)
        /// </summary>
        /// <returns></returns>
        public override string UniqueString()
        {
            //return "Circle" + Radius + FillColor + StrokeThickness + StrokeColor;
            return $"Circle.{Radius}.{FillColor}{StrokeThickness}.{StrokeColor}.{ArcStartAngle}{ArcEndAngle}";
        }

        /// <summary>
        /// draw the circle in the textureBuffer
        /// </summary>
        /// <param name="textureBuffer"></param>
        internal override void DrawInTextureBuffer(Color[] textureBuffer)
        {
            // we swipe an angle from 0 to 2PI to get a full circle
            //the angle advances with a small step, so we advance so slow that each pixel in the circle is part of at least 1 line
            // multiply the radius by 2 to avoid white dots (pixels) far from the center point
            double angleStep = 1f / (Radius * 2);

            for (double angle = ArcStartAngle; angle <= _arcEndAngle; angle += angleStep)
            {
                DrawCircleLineAtThisAngle(textureBuffer, angle);
            }
        }

        /// <summary>
        /// Draw a line in the circle, starting at the center, given the angle, to the border (take fill and stroke into account)
        /// </summary>
        /// <param name="textureBuffer"></param>
        /// <param name="angle"></param>
        private void DrawCircleLineAtThisAngle(Color[] textureBuffer, double angle)
        {
            //avoiding double computation of cos and sin (I use them twice)
            double cos = Math.Cos(-angle);
            double sin = Math.Sin(-angle);

            DrawStrokeForThisAngle(textureBuffer, cos, sin);
            DrawFillForThisAngle(textureBuffer, cos, sin);
        }

        /// <summary>
        /// draw the fill- colored part of a line in the circle, starting at the center, given the angle, to the border
        /// </summary>
        /// <param name="textureBuffer"></param>
        /// <param name="cos"></param>
        /// <param name="sin"></param>
        private void DrawFillForThisAngle(Color[] textureBuffer, double cos, double sin)
        {
            //we draw different pixels, moving them out from the center 1 by 1
            for (int pixelDistanceFromCenter = 0; pixelDistanceFromCenter <= Radius - StrokeThickness; pixelDistanceFromCenter++)
            {
                Point pixel = GetPixel(pixelDistanceFromCenter, cos, sin);
                textureBuffer[pixel.Y * TextureWidth + pixel.X] = FillColor;
            }
        }

        /// <summary>
        /// draw the stroke- colored part of a line in the circle, starting at the center, given the angle, to the border
        /// </summary>
        /// <param name="textureBuffer"></param>
        /// <param name="cos"></param>
        /// <param name="sin"></param>
        private void DrawStrokeForThisAngle(Color[] textureBuffer, double cos, double sin)
        {
            for (int pixelDistanceFromCenter = (int)Radius - StrokeThickness; pixelDistanceFromCenter <= Radius; pixelDistanceFromCenter++)
            {
                Point pixel = GetPixel(pixelDistanceFromCenter, cos, sin);
                textureBuffer[pixel.Y * TextureWidth + pixel.X] = StrokeColor;   // c.StrokeColor;
            }
        }

        private Point GetPixel(int distanceFromCenter, double cos, double sin)
        {
            int x = (int)System.Math.Round((Radius) + distanceFromCenter * cos);
            int y = (int)System.Math.Round((Radius) + distanceFromCenter * sin);

            return new Point(x, y);
        }

        //radius = 3 => textureSide = 3+1+3 pixels (1 because of the centerpoint)
        /// <summary>
        /// the width of the boxing rectangle (2*radius +1 (for centerpoint))
        /// </summary>
        public override int TextureWidth => (int)Radius * 2 + 1;
        /// <summary>
        /// the height of the boxing rectangle (2*radius + 1 (for centerpoint))
        /// </summary>
        public override int TextureHeight => (int)Radius * 2 + 1;
    }
}
