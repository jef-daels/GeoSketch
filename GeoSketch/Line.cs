using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace GeoSketch
{
    /// <summary>
    /// a class which defines a draweable line object
    /// </summary>
    public class Line : GeometricVisual
    {
        //Point1 and Point2 within the texture
        private Vector2 _texturePoint1;
        private Vector2 _texturePoint2;

        #region constructors
        /// <summary>
        /// draw a line from point1 to point2
        /// </summary>
        /// <param name="point1">first point of the line</param>
        /// <param name="point2">second point of the line</param>
        /// <param name="lineThickness">thickness of the line (we use the absolute value)</param>
        /// <param name="fillColor"></param>
        /// <param name="strokeThickness">thickness of the stroke(we use the absolute value)</param>
        /// <param name="strokeColor"></param>
        /// <param name="spriteBatch"></param>
        public Line(Vector2 point1, Vector2 point2, int lineThickness, Color fillColor, int strokeThickness, Color strokeColor, SpriteBatch spriteBatch) : base()
        {
            this.Point1 = point1;
            this.Point2 = point2;
            FillColor = fillColor;
            StrokeColor = strokeColor;
            StrokeThickness = Math.Abs(strokeThickness);
            LineThickness = Math.Abs(lineThickness);

            _texturePoint1 = Point1 - TextureTopLeftPosition;
            _texturePoint2 = Point2 - TextureTopLeftPosition;

            Sprite = GeometricContent.GetTexture(spriteBatch, this);
        }
        #endregion

        /// <summary>
        /// the thickness in pixels of the line to be drawn
        /// </summary>
        public int LineThickness { get; private set; }
        /// <summary>
        /// the topleft position of the texture (the texture is the boxing rectangle of the line.  When the line goes right up, TextureTopLeftPosition is different from both points
        /// </summary>
        public override Vector2 TextureTopLeftPosition
        {
            get
            {
                float x = Math.Min(Point1.X, Point2.X);
                float y = Math.Min(Point1.Y, Point2.Y);
                return new Vector2(x, y);
            }
        }

        /// <summary>
        /// first point to define the line
        /// </summary>
        public Vector2 Point1 { get; private set; }
        /// <summary>
        /// second point to define the line
        /// </summary>
        public Vector2 Point2 { get; private set; }

        /// <summary>
        /// TextureWidth adds LineThickness to the horizontal distance between the points for nicer drawingresults
        /// </summary>
        public override int TextureWidth
        {
            get
            {
                return (int)Math.Abs(Point1.X - Point2.X) + 1 + LineThickness;
            }
        }

        /// <summary>
        /// TextureHeight adds LineThickness to the horizontal distance between the points for nicer drawingresults
        /// </summary>
        public override int TextureHeight
        {
            get
            {
                return (int)Math.Abs(Point1.Y - Point2.Y) + 1 + LineThickness;  // pixel 0 to pixel 100 => height is 101 pixels
            }
        }

        /// <summary>
        /// the line is drawn as a set of circles,  the centerpoints are on the line to draw.  The radius of the circles is half the LineThickness
        /// </summary>
        /// <param name="textureBuffer"></param>
        internal override void DrawInTextureBuffer(Color[] textureBuffer)
        {
            Vector2[] centerPoints = CenterPointsForCirclesOnLine();
            DrawStrokeAndFillCirclesOnLine(textureBuffer, centerPoints);
        }

        private void DrawStrokeAndFillCirclesOnLine(Color[] textureBuffer, Vector2[] centerPoints)
        {
            //if linethickness == 0 => don't draw
            //we draw circles if the linethickness > 1 (with a radius == linethickness/2)
            //if linethickness == 1 => draw the pixels on the centerpoints
            if (LineThickness == 0) return;
            //draw a line by drawing first a wide line of circles with stroke color, then a thinner one with fill color
            DrawCirclesOnLine(textureBuffer, centerPoints, StrokeColor, LineThickness / 2);
            DrawCirclesOnLine(textureBuffer, centerPoints, FillColor, (LineThickness - StrokeThickness) / 2);
        }

        private void DrawCirclesOnLine(Color[] textureBuffer, Vector2[] centerPoints, Color color, int radius)
        {
            //if (radius == 0) return;
            //foreach (Vector2 center in centerPoints)
            for (int i = 0; i < centerPoints.Length; i++)
            {
                Vector2 centerPoint = centerPoints[i];
                DrawCircleRoundCenter(textureBuffer, centerPoint, radius, color);

                if (i < centerPoints.Length - 1)
                {
                    Vector2 nextCenterPoint = centerPoints[i + 1];
                    FillGapsIfAnyInLine(centerPoint, nextCenterPoint, textureBuffer, color, radius);
                }
            }

        }

        private void FillGapsIfAnyInLine(Vector2 centerPoint, Vector2 nextCenterPoint, Color[] textureBuffer, Color color, int radius)
        {
            Vector2 topCenterPoint, bottomCenterPoint;
            if (centerPoint.Y < nextCenterPoint.Y)
            {
                topCenterPoint = centerPoint;
                bottomCenterPoint = nextCenterPoint;
            }
            else
            {
                topCenterPoint = nextCenterPoint;
                bottomCenterPoint = centerPoint;
            }
            float verticalGap = bottomCenterPoint.Y - topCenterPoint.Y;
            if (verticalGap > radius)
            {
                float stepY = 1;
                float gapY = topCenterPoint.Y + stepY;
                while (gapY < bottomCenterPoint.Y)
                {
                    Vector2 gapPoint = new Vector2(topCenterPoint.X, gapY);
                    DrawCircleRoundCenter(textureBuffer, gapPoint, radius, color);
                    gapY += stepY;
                }
            }
        }

        private Vector2[] CenterPointsForCirclesOnLine()
        {
            if (IsVerticalLine)
                return CenterPointsForCirclesOnVerticalLine();
            else
                return CenterPointsForCirclesOnNonVerticalLine();
        }

        private Vector2[] CenterPointsForCirclesOnNonVerticalLine()
        {
            (Vector2 leftCenterPointOnLineInTexture, Vector2 rightCenterPointOnLineInTexture) = NonVerticalLineLeftAndRightCenterPointsInTexture();

            int minX = (int)leftCenterPointOnLineInTexture.X;
            int maxX = (int)rightCenterPointOnLineInTexture.X;

            //iterate each x-pixel, calculate y for this x
            // a.x + b = y: calculate a and b
            float a = (rightCenterPointOnLineInTexture.Y - leftCenterPointOnLineInTexture.Y) / (maxX - minX);
            float b = leftCenterPointOnLineInTexture.Y - a * leftCenterPointOnLineInTexture.X;
            float a2 = (Point2.Y - Point1.Y) / (Point2.X - Point1.X);
            float b2 = Point1.Y - a2 * Point1.X;

            Vector2[] centerpoints = new Vector2[(int)(rightCenterPointOnLineInTexture.X - leftCenterPointOnLineInTexture.X + 1)];
            for (int x = minX; x <= maxX; x++)
            {
                float y = a * x + b;
                centerpoints[x - minX] = new Vector2(x, y);
            }

            return centerpoints;
        }

        private Vector2[] CenterPointsForCirclesOnVerticalLine()
        {

            (Vector2 topPoint, Vector2 bottomPoint) = VerticalLineTopAndBottomCenterPointsInTexture();
            int x = (int)topPoint.X;
            int minY = (int)topPoint.Y;
            int maxY = (int)bottomPoint.Y;

            Vector2[] centerpoints = new Vector2[maxY - minY];
            for (int y = minY; y < maxY; y++)
            {
                centerpoints[y - minY] = new Vector2(x, y);
            }

            return centerpoints;
        }


        /// <summary>
        /// calculate the top and bottom of a VERTICAL line point in the texture, taking the stroke into account (for a vertical line)
        /// </summary>
        /// <returns></returns>
        private (Vector2 topPoint, Vector2 BottomPoint) VerticalLineTopAndBottomCenterPointsInTexture()
        {
            //Vector2 p1 = Point1 - TextureTopLeftPosition;
            //Vector2 p2 = Point2 - TextureTopLeftPosition;

            //(Vector2 topPoint, Vector2 bottomPoint) = GeometricHelpers.SortVector2HighToLow(p1, p2);
            (Vector2 topPoint, Vector2 bottomPoint) = GeometricHelpers.SortVector2HighToLow(_texturePoint1, _texturePoint2);
            //move the top position down a bit so the stroke nicely fits in the texture
            topPoint += new Vector2(LineThickness / 2, LineThickness / 2);
            if (topPoint.Y > bottomPoint.Y)
            {
                Vector2 p = topPoint;
                topPoint = bottomPoint;
                bottomPoint = p;
            }

            return (topPoint, bottomPoint);
        }

        /// <summary>
        /// a non vertical line has a left- and right point (with some offset in the textbuffer for nice stroke/fill effect)
        /// </summary>
        /// <returns></returns>
        private (Vector2 leftCenterPointOnLineInTexture, Vector2 rightCenterPointOnLineInTexture)
            NonVerticalLineLeftAndRightCenterPointsInTexture()
        {
            ////p1 and p2 are the coordinates in the texture.
            //Vector2 p1 = Point1 - TextureTopLeftPosition;
            //Vector2 p2 = Point2 - TextureTopLeftPosition;
            //(Vector2 leftPoint, Vector2 rightPoint) = SortLeftToRight(p1, p2);
            (Vector2 leftPoint, Vector2 rightPoint) = SortLeftToRight(_texturePoint1, _texturePoint2);

            //to have a nice stroke at both ends, we move them a bit inwards (half LineThickness in both directions)
            Vector2 leftCenterPointOnLineInTexture = leftPoint + new Vector2((float)(LineThickness / 2), (float)(LineThickness / 2));
            Vector2 rightCenterPointOnLineInTexture = rightPoint + new Vector2((float)(LineThickness / 2), (float)(LineThickness / 2));
            return (leftCenterPointOnLineInTexture, rightCenterPointOnLineInTexture);
        }


        /// <summary>
        /// draw a (small) circle around center, as if we draw with a pen with the point size equal to that circle
        /// </summary>
        /// <param name="textureBuffer"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        private void DrawCircleRoundCenter(Color[] textureBuffer, Vector2 center, int radius, Color color)
        {
            //sweep throught the angles to a full circle, with a small enough step to avoid white spots (at the cost of duplicates)
            double angleStep = 1f / (LineThickness * 2);
            for (double angle = 0; angle < System.Math.PI * 2; angle += angleStep)
            {
                DrawPixelLineAtAngle(textureBuffer, center, angle, radius, color);
            }
        }

        private void DrawPixelLineAtAngle(Color[] textureBuffer, Vector2 center, double angle, int radius, Color color)
        {
            //avoiding double computation of cos and sin (I use them radius times)
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);//we draw different pixels, moving them out from the center 1 by 1
            for (int pixelDistanceFromCenter = 0; pixelDistanceFromCenter <= radius; pixelDistanceFromCenter++)
                DrawPixel(textureBuffer, center, pixelDistanceFromCenter, cos, sin, color);
        }

        private void DrawPixel(Color[] textureBuffer, Vector2 center, float pixelDistanceFromCenter, double cos, double sin, Color color)
        {
            int circlePixelX = (int)System.Math.Round((center.X) + pixelDistanceFromCenter * cos);
            int circlePixelY = (int)System.Math.Round((center.Y) + pixelDistanceFromCenter * sin);

            //the test below didn't hit during my tests, but rather safe then sorry
            if (circlePixelX < 0 || circlePixelX >= TextureWidth) return;
            if (circlePixelY < 0 || circlePixelY >= TextureHeight) return;

            //Point pixel = new Point(circlePixelX, circlePixelY);
            textureBuffer[circlePixelY * TextureWidth + circlePixelX] = color;
        }


        private Boolean IsVerticalLine
        {
            get { return Math.Abs(Point1.X - Point2.X) < Math.Max(1, LineThickness / 2); }
        }

        /// <summary>
        /// uniqely identifies the current object (this will be the used key in the Texture2D cache)
        /// </summary>
        /// <returns></returns>
        public override string UniqueString()
        {
            //return this.GetType().ToString() + Point1.ToString() + Point2.ToString() + FillColor + StrokeThickness + StrokeColor;
            //_texturePoints support caching of moving same line
            return this.GetType().ToString() + _texturePoint1.ToString() + _texturePoint2.ToString() + FillColor + LineThickness + "." + StrokeThickness + StrokeColor;
        }
    }
}
