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
    /// class which defines the extensions used in a monogame project
    /// </summary>
    public static class GeoSketchExtensions
    {
        /// <summary>
        /// draw a line according to the provided specifications
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="c"></param>
        /// <param name="lineWidth"></param>
        public static void DrawLine(this SpriteBatch spriteBatch, int x1, int y1, int x2, int y2, Color c, int lineWidth)
        {
            new Line(new Vector2(x1, y1), new Vector2(x2, y2), lineWidth, c, 0, Color.Transparent, spriteBatch).Draw(spriteBatch);
        }
        /// <summary>
        /// draw a rectangle according to the specifications
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="topleftX"></param>
        /// <param name="topleftY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, int topleftX, int topleftY, int width, int height, Color fillColor, Color strokeColor, int strokeThickness)
        {
            new Quadrangle(new Rectangle(topleftX, topleftY, width, height), fillColor, strokeColor, strokeThickness, spriteBatch).Draw(spriteBatch);
        }
        /// <summary>
        /// draw a circle according to the provided specifications
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        public static void DrawCircle(this SpriteBatch spriteBatch, int centerX, int centerY, int radius, Color fillColor, Color strokeColor, int strokeThickness)
        {
            new Circle(new Vector2(centerX, centerY), radius, fillColor, strokeColor, 
                strokeThickness, spriteBatch).Draw(spriteBatch);
        }

        /// <summary>
        /// draws part of a circle (the arc) according to the provided specifications
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="arcStart"></param>
        /// <param name="arcEnd"></param>
        public static void DrawArc(this SpriteBatch spriteBatch, int centerX, int centerY,
            int radius, Color fillColor, Color strokeColor, int strokeThickness,
            float arcStart, float arcEnd
            )
        {
            //internally, angles go clockwise, but we think counterclockwise (math)
            new Arc(new Vector2(centerX, centerY), radius, fillColor, strokeColor,
                strokeThickness, arcStart, arcEnd, spriteBatch).Draw(spriteBatch);
        }

        /// <summary>
        /// draws a triangle according to the provided specifications
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        public static void DrawTriangle(this SpriteBatch spriteBatch, 
            Vector2 point1, Vector2 point2, Vector2 point3, 
            Color fillColor, Color strokeColor, int strokeThickness)
        {
            new Triangle(point1,point2,point3, fillColor, strokeColor, 
                strokeThickness, spriteBatch).Draw(spriteBatch);
        }   
    }
}
