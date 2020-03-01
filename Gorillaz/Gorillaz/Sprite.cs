using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gorillaz
{
    abstract public class Sprite
    {
        //Stores the information about the sprite's dimensions
        protected Texture2D spriteSheet;
        public Vector2 position;
        public Point frameSize;
        public Point currentFrame;
        protected Point sheetSize;

        //Stores the information regarding animation speed
        protected int timeSinceLastFrame=0;
        protected const int milisecondsPerFrame =16;

        /// <summary>
        /// Constructor for the sprite class
        /// </summary>
        /// <param name="SpriteSheet"></param>
        /// <param name="Position"></param>
        /// <param name="FrameSize"></param>
        /// <param name="CurrentFrame"></param>
        /// <param name="SheetSize"></param>
        public Sprite(Texture2D SpriteSheet, Vector2 Position, Point FrameSize, Point CurrentFrame, Point SheetSize)
        {

            spriteSheet = SpriteSheet;
            position = Position;
            frameSize = FrameSize;
            currentFrame = CurrentFrame;
            sheetSize = SheetSize;

        }

        /// <summary>
        /// Method for animating sprites based on its sprite sheet
        /// </summary>
        /// <param name="gameTime"></param>
        public void Animate(GameTime gameTime)
        {

            //Stores the time that has passed prior to animation update
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;

            //Checks to see if time has passed the set animation time
            if (timeSinceLastFrame >= milisecondsPerFrame)
            {
                //Resets the time tracked
                timeSinceLastFrame = 0;

                //Increments the current frame on the x axis
                currentFrame.X++;

                //Chekcs to see if current frame is at the end of the sprite sheet
                if (currentFrame.X >= sheetSize.X)
                {
                    //Changes the current frame to move to next row on sprite sheet
                    currentFrame.X = 0;
                    currentFrame.Y++;

                    //Resets current frame on y axis back to zero after reaching end of sprite sheet
                    if (currentFrame.Y >= sheetSize.Y)
                    {
                        currentFrame.Y = 0;
                    }
                }
            }

        }

        /// <summary>
        /// Draws the sprites on screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            //Creates animation rectangle and draws the sprite
            Rectangle spriteRec = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
            spriteBatch.Draw(spriteSheet,position,spriteRec,Color.White);

        }

    }
}
