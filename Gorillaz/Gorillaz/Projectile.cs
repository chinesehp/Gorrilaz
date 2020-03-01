using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gorillaz
{
    class Projectile
    {
        //Stores the information regarding projectile motion
        public float angle;
        private Vector2 objectVelocity;
        public float objectTotalVelocity;
        public Player player;

        //Stores the dimensions and information on the projectile
        private Texture2D image;
        public Rectangle bounds;

        /// <summary>
        /// Initializes a new instance of projectile 
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Bounds"></param>
        /// <param name="Player"></param>
        public Projectile(Texture2D Image, Rectangle Bounds, Player Player)
        {

            image = Image;
            bounds = Bounds;
            player = Player;

        }

        /// <summary>
        /// Calculates the velocity that the projectile possesses initially
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector2 CalcVelocity(MouseState mouse)
        {

            //Calculates the velocity based on distance between projectile and mouse
            objectTotalVelocity = (float)(Math.Sqrt(Math.Pow(bounds.X - mouse.X, 2) + Math.Pow(bounds.Y - mouse.Y, 2)))/13;

            //Converts overall velocity into it's x and y components
            objectVelocity.X = (float)Math.Cos(angle) * objectTotalVelocity;
            objectVelocity.Y = (float)-Math.Sin(angle) * objectTotalVelocity;

            //Returns the object velocity
            return objectVelocity;

        }
        /// <summary>
        /// Calculates the angle that the projectile will be fired at
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns></returns>
        public float CalcAngle(MouseState mouse, bool isP1Turn)
        {

            //Creates and stores the values needed to calculate angle
            float adjacent;
            float opposite;

            //Calcautes the values of the triangle's legs based on mouse and player location
            if (isP1Turn)
            {
                adjacent = mouse.X - (player.position.X + player.frameSize.X);
            }
            else
            {
                adjacent = player.position.X - mouse.X;
            }
            opposite = player.position.Y - mouse.Y;

            //Calculates angle using inverse of tan and returns value
            angle = (float)Math.Atan2(opposite, adjacent);

            return angle;

        }
        /// <summary>
        /// Draws the projectile
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="angle"></param>
        public void Draw(SpriteBatch spriteBatch, float angle, bool isP1Turn)
        {

            //Draws the missile orientation based on player's turn
            if (isP1Turn)
            {
                spriteBatch.Draw(image, bounds, null, Color.White, -angle, Vector2.Zero, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(image, bounds, null, Color.White, angle, new Vector2(image.Width, 0), SpriteEffects.FlipHorizontally, 0f);
            }

        }
        /// <summary>
        /// Changes the position of the projectile based on player's turn
        /// </summary>
        public void ChangePosition(bool isP1Turn)
        {

            //Changes the position of missile based on player's turn
            if (isP1Turn)
            {
                bounds.Location = new Point((int)player.position.X + player.frameSize.X, (int)player.position.Y);
            }
            else
            {
                bounds.Location = new Point((int)player.position.X, (int)player.position.Y);
            }

        }
    }
}
