using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpTest
{
    class platforms
    {
        public Texture2D texture { get; set; }
        public float platformX { get; set; }
        public float platformY { get; set; }


        public platforms(Texture2D Texture, float PlatformX, float PlatformY)
        {
            texture = Texture;
            platformX = PlatformX;
            platformY = PlatformY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture,new Vector2(platformX,platformY),Color.White);
            spriteBatch.End();
        }

    }
}
