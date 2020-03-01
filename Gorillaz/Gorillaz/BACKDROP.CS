using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gorillaz
{
    class BackDrop
    {
        //Stores the information regarding backdrop 
        private Texture2D image;
        private Rectangle bounds;
        private float transparancy;

        /// <summary>
        /// Initializes a new instance of a backdrop
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Bounds"></param>
        public BackDrop(Texture2D Image, Rectangle Bounds, float Transparency)
        {

            image = Image;
            bounds = Bounds;
            transparancy = Transparency;

        }

        /// <summary>
        /// Draws the backdrop
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            //Draws the backdrop with the provided information
            spriteBatch.Draw(image, bounds, Color.White*transparancy);

        }
    }
}
