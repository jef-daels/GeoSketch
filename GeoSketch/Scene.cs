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
    /// a scene can contain multiple objects to be drawn, and draw them all with 1 command
    /// </summary>
    public class Scene
    {
        private readonly List<GeometricVisual> _geometricVisuals = new List<GeometricVisual>();

        /// <summary>
        /// add an object to the scene
        /// </summary>
        /// <param name="bv"></param>
        public void AddGeometricVisual(GeometricVisual bv)
        {
            _geometricVisuals.Add(bv);
        }

        /// <summary>
        /// draw all objects in the scene
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (GeometricVisual bv in _geometricVisuals)
            {
                bv.Draw(spriteBatch);
            }
        }


        //public Scene Clone()
        //{
        //    Scene clone = new Scene();
        //    foreach (GeometricVisual bv in _geometricVisuals)
        //        clone.AddGeometricVisual(bv.Clone());

        //    return clone;
        //}

        //public void Translate(Vector2 displacement)
        //{
        //    foreach (GeometricVisual bv in _geometricVisuals)
        //        bv.Translate(displacement);
        //}
    }
}
