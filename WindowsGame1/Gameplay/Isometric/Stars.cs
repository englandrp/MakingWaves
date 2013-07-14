using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio; 

namespace WindowsGame1
{
    class Stars
    {

        SoundEffect starSound;


        Texture2D red;
        Texture2D gold;
        Texture2D outline;

        int Count = 0;
       public bool Animating = false;
        float Rotation;
        int Counter;
        public int Stage;
        Vector2 Movement;
        
        Rectangle movingStarRect;
        Point smallPos = new Point(500, 20);
        Point bigPos = new Point(1280 / 2, 720 / 4);
        int Gap = 100;
        int bigSize = 200;
        int smallSize = 60;
        bool animateWithAward;
        public bool clearStars;
        int offset;
        int numStars;
        float starAlpha = 1.0f;

        public void LoadContent(ContentManager content)
        {
            gold = content.Load<Texture2D>("Graphics/stargold");
            red = content.Load<Texture2D>("Graphics/starred");
            outline = content.Load<Texture2D>("Graphics/staroutline");

            starSound = content.Load<SoundEffect>("done");
        }

        public void Animate()
        {
            Animating = true;
        }

        public void ClearStars()
        {
            clearStars = true;
        }

        public void Update( StatBar barGold)
        {
            if (Animating == true)
            {
                Animating = false;
                Stage = 1;
                Counter = 0;
                movingStarRect = new Rectangle(bigPos.X, bigPos.Y, 0,0);
                starSound.Play();/////////////////////*****************************************************************
            }
            if (Stage == 1)
            {
                Counter++;

                


                if (Counter < 15)
                {
                    movingStarRect.Width += bigSize / 15;
                    movingStarRect.Height += bigSize / 15;
                }
                else if (Counter < 59)
                {

                }
                else if (Counter < 60)
                {
                    float dif = bigSize / 2 - smallSize / 2;
                    dif = smallSize / 2;
                    Movement = new Vector2((smallPos.X - bigPos.X + dif ) / 20.0f,
                                           (smallPos.Y - bigPos.Y + dif + (Gap * (barGold.currentValue-1))) / 20.0f
                        );
                }
                else if (Counter < 81)
                {
                    movingStarRect.X = bigPos.X +  (int)(Movement.X * ( Counter - 60));
                    movingStarRect.Y =bigPos.Y +  (int)(Movement.Y * ( Counter - 60));
                    movingStarRect.Width -= ((bigSize - smallSize) / 21);
                    movingStarRect.Height -= ((bigSize - smallSize) / 21); 
                }
                else
                {
                    Stage = 0;
                    numStars++;
                }
            }
            if (clearStars == true)
            {
                if (offset < 800)
                {
                    
                    offset += 20;
                    starAlpha -= 0.05f;
                }
                else
                {
                    clearStars = false;
                    offset = 0;
                    numStars = 0;
                    starAlpha = 1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, StatBar barGold)
        {
            if (Stage != 0)
            {
                spriteBatch.Draw(gold,
                    movingStarRect, gold.Bounds, Color.White, Rotation, new Vector2(gold.Width / 2, gold.Width / 2), SpriteEffects.None, 1.0f);
            }
            for (int i = 0; i < barGold.size; i++)
            {
                spriteBatch.Draw(outline, new Rectangle(smallPos.X + (Gap * i), smallPos.Y, smallSize, smallSize), outline.Bounds, Color.White, Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);

            for (int j = 0; j < numStars; j++)
            {

                spriteBatch.Draw(gold, new Rectangle(smallPos.X + (Gap * j), smallPos.Y + offset, smallSize, smallSize), gold.Bounds, Color.White * starAlpha, Rotation, Vector2.Zero, SpriteEffects.None, 1.0f);

            }
       
            }
        }
    }
}
