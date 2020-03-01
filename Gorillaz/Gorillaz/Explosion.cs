using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gorillaz
{
    class Explosion: Sprite
    {
        /// <summary>
        /// Initializes a new instance of explosion
        /// </summary>
        /// <param name="SpriteSheet"></param>
        /// <param name="Position"></param>
        /// <param name="FrameSize"></param>
        /// <param name="CurrentFrame"></param>
        /// <param name="SheetSize"></param>
        public Explosion(Texture2D SpriteSheet, Vector2 Position, Point FrameSize, Point CurrentFrame, Point SheetSize):
        base(SpriteSheet,Position,FrameSize,CurrentFrame,SheetSize)
        {

            spriteSheet = SpriteSheet;
            position = Position;
            frameSize = FrameSize;
            currentFrame = CurrentFrame;
            sheetSize = SheetSize;

        }
    }
}
