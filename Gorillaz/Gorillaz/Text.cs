using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gorillaz
{
    class Text
    {
        //Stores the information regarding text
        private SpriteFont font;
        public string text;
        private Vector2 textLoc;
        private Color textColour;

        /// <summary>
        /// Intializes an instance of text
        /// </summary>
        /// <param name="Font"></param>
        /// <param name="Text"></param>
        /// <param name="TextLoc"></param>
        /// <param name="TextColour"></param>
        public Text(SpriteFont Font, string Text, Vector2 TextLoc, Color TextColour)
        {

            font = Font;
            text = Text;
            textLoc = TextLoc;
            textColour = TextColour;

        }

        /// <summary>
        /// Draws the text 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawString(SpriteBatch spriteBatch)
        {

            //Draws the text using information provided
            spriteBatch.DrawString(font, text, textLoc, textColour);

        }

    }
}
