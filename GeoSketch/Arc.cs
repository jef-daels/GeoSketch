using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoSketch
{
    /// <summary>
    /// An Arc is a piece of a Circle starting from a start angle and ending with an end angle.
    /// </summary>
    public class Arc : Circle
    {
        /// <summary>
        /// creates
        /// </summary>
        /// <param name="centerPoint">centerpoint of the circle this arc is part of</param>
        /// <param name="radius">radius of the circle this arc is part of</param>
        /// <param name="fillColor">color inside the border (can be transparant)</param>
        /// <param name="strokeColor">border color</param>
        /// <param name="strokeThickness">border thickness</param>
        /// <param name="startArcAngle">angle on the circle where the arc starts</param>
        /// <param name="endArcAngle">angle on the circle where the arc ends</param>
        /// <param name="spriteBatch">spriteBatch</param>
        public Arc(Vector2 centerPoint, float radius, Color fillColor, Color strokeColor, 
            int strokeThickness, float startArcAngle, float endArcAngle, SpriteBatch spriteBatch) :
            base(centerPoint, radius, fillColor, strokeColor, strokeThickness,startArcAngle,
                endArcAngle, spriteBatch)
        {
        }
    }
}
