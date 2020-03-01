using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gorillaz
{
    class Building
    {
        //Stores information on building's dimensions
        private Texture2D image;
        public Rectangle bounds;

        /// <summary>
        /// Initializes a new instance of a building
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Bounds"></param>
        public Building(Texture2D Image, Rectangle Bounds)
        {

            image = Image;
            bounds = Bounds;

        }

        /// <summary>
        /// Draws the building
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            //Draws the building using information provided
            spriteBatch.Draw(image, bounds, Color.White);

        }
    }
}
