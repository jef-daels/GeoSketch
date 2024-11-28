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
    ///   draws triangles, identified by it's 3 vertices (no order implied on the vertices)
    /// </summary>
    public class Triangle : GeometricVisual
    {
        /// <summary>
        /// the first known point
        /// </summary>
        public Vector2 Point1 { get; private set; }
        /// <summary>
        /// the second known point
        /// </summary>
        public Vector2 Point2 { get; private set; }
        /// <summary>
        /// the third known point
        /// </summary>
        public Vector2 Point3 { get; private set; }

        #region constructors
        /// <summary>
        /// define the triangle (the 3 vertices must be different points)
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="spriteBatch"></param>
        public Triangle(Vector2 point1, Vector2 point2, Vector2 point3, Color fillColor, Color strokeColor, int strokeThickness, SpriteBatch spriteBatch) :
            this(new Vector2[] { point1, point2, point3 }, fillColor, strokeColor, strokeThickness, spriteBatch)
        { }

        /// <summary>
        /// define the triangle (3 different points must be provided)
        /// </summary>
        /// <param name="points"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="spriteBatch"></param>
        public Triangle(Vector2[] points, Color fillColor, Color strokeColor, int strokeThickness, SpriteBatch spriteBatch)
        {
            Point1 = points[0];
            Point2 = points[1];
            Point3 = points[2];
            FillColor = fillColor;
            StrokeColor = strokeColor;
            StrokeThickness = strokeThickness;

            Sprite = GeometricContent.GetTexture(spriteBatch, this);
        }

        /// <summary>
        /// define the triangle (3 different points must be provided)
        /// </summary>
        /// <param name="points"></param>
        /// <param name="fillColor"></param>
        /// <param name="spriteBatch"></param>
        public Triangle(Vector2[] points, Color fillColor, SpriteBatch spriteBatch)
            : this(points, fillColor, Color.Transparent, 0, spriteBatch)
        {

        }
        #endregion

        /// <summary>
        /// returns the vertix (x,y) where x is the smallest X for the points and y is the smallest Y for the points
        /// (the texture is the smallest (unrotated) rectangle enclosing the triangle)
        /// </summary>
        public override Vector2 TextureTopLeftPosition
        {
            get
            {
                float x = new float[] { Point1.X, Point2.X, Point3.X }.Min();
                float y = new float[] { Point1.Y, Point2.Y, Point3.Y }.Min();
                //float y = Vector2sHighToLow[0].Y;
                return new Vector2(x, y);
            }
        }

        private Vector2[] Vector2sHighToLow
        {
            get
            {
                Vector2[] vector2s = new Vector2[] { Point1, Point2, Point3 };
                GeometricHelpers.SortVector2HighToLow(vector2s);
                return vector2s;
            }
        }

        /// <summary>
        /// the difference between the max and min X values of the defining vertices
        /// (the texture is the smallest (unrotated) rectangle enclosing the triangle)
        /// </summary>
        public override int TextureWidth
        {
            get
            {
                List<float> xs = new List<float>(3)
                {
                    Point1.X,
                    Point2.X,
                    Point3.X
                };
                return (int)(xs.Max() - xs.Min()) + 1;
            }
        }

        /// <summary>
        /// the difference between the max and min Y values of the defining vertices
        /// (the texture is the smallest (unrotated) rectangle enclosing the triangle)
        /// </summary>
        public override int TextureHeight
        {
            get
            {
                return (int)(Vector2sHighToLow[2].Y - Vector2sHighToLow[0].Y) + 1;
            }
        }

        /// <summary>
        /// uniqely identifies the current object (this will be the used key in the Texture2D cache)
        /// </summary>
        /// <returns></returns>
        public override string UniqueString()
        {
            return "triangle" + Point1 + Point2 + Point3 + FillColor;
        }

        /// <summary>
        /// make a 3-points copy, so that the topleft corner of the boxing rectangle is at the origin (move topleftposition of the boxing rectangle to the origin)
        /// </summary>
        /// <returns></returns>
        public (Vector2 v1, Vector2 v2, Vector2 v3) TextureVectors()
        {
            Vector2 v1 = Vector2sHighToLow[0] - TextureTopLeftPosition;
            Vector2 v2 = Vector2sHighToLow[1] - TextureTopLeftPosition;
            Vector2 v3 = Vector2sHighToLow[2] - TextureTopLeftPosition;
            return (v1, v2, v3);
        }

        /// <summary>
        /// creates a texture for this triangle
        /// </summary>
        /// <param name="textureBuffer"></param>
        internal override void DrawInTextureBuffer(Color[] textureBuffer)
        {
            //draw a horizontal line through the middle (height) point and fill the top and bottom triangles
            //there are 2 types of triangles possible: bottomflat and top flat

            //calculate the endpoint of the horizontal line, starting in v2
            Vector2 v1, v2, v3;
            (v1, v2, v3) = TextureVectors();  //v1 is the top of the triangle

            if (IsBottomFlat)
            {
                FillBottomFlatTriangle(v1, v2, v3, textureBuffer);
            }
            else if (IsTopFlat)
            {
                FillTopFlatTriangle(v1, v2, v3, textureBuffer);
            }
            else
            {
                /* general case - split the triangle in a topflat and bottom-flat one */
                Vector2 v4 = HorizontalEndpointOppositev2(v1, v2, v3);
                FillBottomFlatTriangle(v1, v2, v4, textureBuffer);
                //the lower triangle must be drawn at a vertical offset, because it should not start at the top
                FillTopFlatTriangle(v2, v4, v3, textureBuffer);
            }
        }

        private Vector2 HorizontalEndpointOppositev2(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            //calculate x4 which, at the same height as v2 yields a horizontal line splitting the triangle in a bottomflat and topflat triangle
            //http://www.sunshine2k.de/coding/java/TriangleRasterization/TriangleRasterization.html
            //for the interested: the formula for x4 is explained in the link above
            float x4 = v1.X + ((v2.Y - v1.Y) / (v3.Y - v1.Y)) * (v3.X - v1.X);
            Vector2 v4 = new Vector2(x4, v2.Y);    //v4 is the endpoint for the horizontal line starting in v2, ending on the line between v1 and v3
            return v4;
        }

        private Boolean IsTopFlat
        {
            get { return Vector2sHighToLow[0].Y == Vector2sHighToLow[1].Y; }
        }
        private Boolean IsBottomFlat
        {
            get { return Vector2sHighToLow[1].Y == Vector2sHighToLow[2].Y; }
        }


        /// <summary>
        /// fill a bottomflat triangle by drawing the horizontal lines 1 by 1
        /// </summary>
        /// <param name="v1">top point</param>
        /// <param name="v2">point of the flat bottom</param>
        /// <param name="v3">other point of the flat bottom</param>
        /// <param name="data"></param>
        private void FillBottomFlatTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color[] data)
        {
            //v1 is the top, v2 and v3 are at the same height, lower then v1
            //the coordinates are pixels, so can be interpreted as rows and columns
            //v2 should be more to the left then v3, if not we switch them
            (v2, v3) = SortLeftToRight(v2, v3);
            (float slopev1v2, float slopev1v3) = GetSlopes(v1, v2, v3);

            //we start at the top of the triangle, and move down to the bottom
            float horizontalLineLeftX = v1.X;
            float horizontalLineRightX = v1.X;

            for (int horizontalLineNumber = 0; horizontalLineNumber < v2.Y; horizontalLineNumber++)
            {
                DrawHorizontalLineInBottomFlatTriangle((int)v2.Y, data, horizontalLineLeftX, horizontalLineRightX, horizontalLineNumber);
                horizontalLineLeftX += slopev1v2;
                horizontalLineRightX += slopev1v3;
            }
        }

        private void DrawHorizontalLineInBottomFlatTriangle(int triangleBottom, Color[] textureBuffer, float horizontalLineLeftX
            , float horizontalLineRightX, int horizontalLineNumber)
        {

            for (int x = (int)horizontalLineLeftX; x <= horizontalLineRightX; x++)
            {
                Point pixel = new Point(x, horizontalLineNumber);
                Color pixelColor = PixelColorInBottomFlatTriangle(triangleBottom, horizontalLineLeftX, horizontalLineRightX, pixel);
                DrawPixelInHorizontalLine(pixel, textureBuffer, pixelColor);
            }
        }

        private Color PixelColorInBottomFlatTriangle(int triangleBottom, float horizontalLineLeftX, float horizontalLineRightX, Point pixel)
        {
            Color pixelColor = FillColor;
            if (IsPixelStrokedInBottomFlatTriangle(pixel, horizontalLineLeftX, horizontalLineRightX, triangleBottom))
                pixelColor = StrokeColor;
            return pixelColor;
        }

        private void DrawPixelInHorizontalLine(Point pixel, Color[] textureBuffer, Color pixelColor)
        {
            textureBuffer[pixel.Y * TextureWidth + pixel.X] = pixelColor;
        }

        private Boolean IsPixelStrokedInBottomFlatTriangle(Point pixel, float horizontalLineLeftX, float horizontalLineRightX, float triangleBottom)
        {
            if (pixel.X < (int)horizontalLineLeftX + StrokeThickness) return true;    //left side
            if (pixel.X > (int)horizontalLineRightX - StrokeThickness) return true; //right side
            //only when the original triangle is bottomflat should the bottom be stroked
            if (IsBottomFlat && pixel.Y > triangleBottom - StrokeThickness) return true; //don't draw the horizontal line between the two triangles
            return false;
        }

        private Boolean IsPixelStrokedInTopFlatTriangle(Point pixel, float horizontalLineLeftX, float horizontalLineRightX, int triangleTop)
        {
            if (pixel.X < (int)horizontalLineLeftX + StrokeThickness) return true;    //left side
            if (pixel.X > (int)horizontalLineRightX - StrokeThickness) return true; //right side
            //only when the original triangle is topflat should the top be stroked
            if (IsTopFlat && pixel.Y < triangleTop + StrokeThickness) return true; //don't draw the horizontal line between the two triangles
            return false;
        }


        /// <summary>
        /// fill the bottom triangle (below the top triangle).  this one has an offset in respect to the top of the original triangle
        /// </summary>
        /// <param name="v1">point of the flat top</param>
        /// <param name="v2">other point of the flat top</param>
        /// <param name="v3">bottom point</param>
        /// <param name="textureBuffer"></param>
        /// <param name="isTopStroked"></param>
        private void FillTopFlatTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color[] textureBuffer, Boolean isTopStroked = false)
        {
            //we fill from the flat top down to the point: the y-co goes from v1.Y to v3.y
            (v1, v2) = SortLeftToRight(v1, v2);
            (float slopev3v1, float slopev3v2) = GetSlopes(v3, v1, v2);

            float horizontalLineLeftX = v1.X;       //x co of the leftmost pixel of the horizontal line
            float horizontalLineRightX = v2.X;      //x co of the rightmost pixel of the horizontal line

            for (int horizontalLineNumber = (int)(v1.Y); horizontalLineNumber <= v3.Y; horizontalLineNumber++)
            {
                DrawHorizontalLineInTopFlatTriangle((int)v2.Y, textureBuffer, isTopStroked, horizontalLineLeftX, horizontalLineRightX, horizontalLineNumber);
                horizontalLineLeftX += slopev3v1;
                horizontalLineRightX += slopev3v2;
            }
        }
        private void DrawHorizontalLineInTopFlatTriangle(int triangleTop, Color[] textureBuffer, bool isTopStroked
            , float horizontalLineLeftX, float horizontalLineRightX, int horizontalLineNumber)
        {
            //draw the different pixels in the horizontal line: the x-value goes from leftX to rightX of the horizontal line
            for (int x = (int)horizontalLineLeftX; x <= horizontalLineRightX; x++)
            {
                Point pixel = new Point(x, horizontalLineNumber);
                Color pixelColor = PixelColorInTopFlatTriangle(triangleTop, horizontalLineLeftX, horizontalLineRightX, pixel);
                DrawPixelInHorizontalLine(pixel, textureBuffer, pixelColor);
            }
        }

        private Color PixelColorInTopFlatTriangle(int triangleTop, float horizontalLineLeftX, float horizontalLineRightX, Point pixel)
        {
            Color pixelColor = FillColor;
            if (IsPixelStrokedInTopFlatTriangle(pixel, horizontalLineLeftX, horizontalLineRightX, triangleTop))
                pixelColor = StrokeColor;
            return pixelColor;
        }
    }
}
