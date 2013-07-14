using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class SpeechBubble
    {
        Game1 game1;

        //main bubble and two smaller ones
        List<TexturedQuad> quadtexs;
        TexturedQuad texquad1;
        TexturedQuad texquad2;
        TexturedQuad texquad3;

        BasicEffect quadEffect;
        Texture2D bubtex1;
        Texture2D bubtex2;
        Texture2D bubtex3;
        Texture2D contenttex; //current image showing in the speech bubble

       public bool visible; //is speech bubble visible

        //numbers for animating the speech bubble bits separately
        float count = 0;
        float count2 = 0;
        float count3 = 0;
        float alpha = 0;
        float duration1 = 10;
        float duration2 = 5;
        float duration3 = 20;

        Vector2 size1 = new Vector2(20.0f, 15.0f);
        Vector2 size2 = new Vector2(4.0f, 3.0f);
        Vector2 size3 = new Vector2(2.0f, 1.5f);
        Vector3 bubblepos1 = new Vector3(0, 32, 0);
        Vector3 bubblepos2 = new Vector3(-7, 24, 0);
        Vector3 bubblepos3 = new Vector3(-5, 21, 0);
        
        int animstate = 0; //current stage of animation
        bool appear = false; //queue appearing
        bool disappear = false; //queue disappearing

        public SpeechBubble(Game1 game1)
        {
            quadtexs = new List<TexturedQuad>();
            bubtex1 = game1.Content.Load<Texture2D>("bubble");
            bubtex2 = game1.Content.Load<Texture2D>("bubble2");
            bubtex3 = game1.Content.Load<Texture2D>("bubble3");
            // texquad = new TexturedQuad(Vector3.Zero, Vector3.Backward, Vector3.Up, 4.8f, 6.4f);
            texquad1 = new TexturedQuad(Vector3.Zero, Vector3.Normalize(new Vector3(0, 0, 1)), Vector3.Up, count2, count3, bubblepos1, bubtex1);
            texquad2 = new TexturedQuad(Vector3.Zero, Vector3.Normalize(new Vector3(0, 0, 1)), Vector3.Up, count2, count3, bubblepos2, bubtex2);
            texquad3 = new TexturedQuad(Vector3.Zero, Vector3.Normalize(new Vector3(0, 0, 1)), Vector3.Up, count2, count3, bubblepos3, bubtex3);

            quadtexs.Add(texquad1);
            quadtexs.Add(texquad2);
            quadtexs.Add(texquad3);

            quadEffect = new BasicEffect(game1.screenManager.GraphicsDevice);
            quadEffect.EnableDefaultLighting();

            quadEffect.World = Matrix.Identity;
            quadEffect.TextureEnabled = true;
            this.game1 = game1;
        }

        public void Update()
        {
            if (appear == true)
            {
                if (animstate == 0)
                {
                    if (count < duration2)
                    {
                        if (count != 0)
                        {
                            count2 = size3.X * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            count3 = size3.Y * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            texquad3.updateVertices(count2, count3);
                        }
                        count++;
                    }
                    else
                    {
                        animstate = 1;
                        count = 0;
                    }
                }
                else if (animstate == 1)
                {
                    if (count < duration2)
                    {
                        if (count != 0)
                        {
                            count2 = size2.X * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            count3 = size2.Y * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            texquad2.updateVertices(count2, count3);
                        }
                        count++;
                    }
                    else
                    {
                        animstate = 2;
                        count = 0;
                    }
                }
                else if (animstate == 2)
                {
                    if (count < duration1)
                    {
                        if (count != 0)
                        {
                            count2 = size1.X * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration1 / count)));
                            count3 = size1.Y * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration1 / count)));
                            texquad1.updateVertices(count2, count3);
                        }
                        count++;
                    }
                    else
                    {
                        animstate = 3;
                        count = 0;
                    }
                }
                else if (animstate == 3)
                {
                    if (count < duration3)
                    {
                        alpha += 1 / duration3;
                        count++;
                    }
                    else
                    {
                        animstate = 0;
                        count = 0;
                        appear = false;
                    }
                }
            }

            if (disappear == true)
            {
                if (animstate == 0)
                {
                    if (count < duration2)
                    {
                        if (count != 0)
                        {
                            count2 = size3.X - size3.X * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            count3 = size3.Y - size3.Y * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            texquad3.updateVertices(count2, count3);
                        }
                        count++;
                    }
                    else
                    {
                        animstate = 1;
                       
                        count = 0;
                        count2 = 0;
                        count3 = 0;
                        texquad3.updateVertices(count2, count3);
                    }
                }
                else if (animstate == 1)
                {
                    if (count < duration2)
                    {
                        if (count != 0)
                        {
                            count2 = size2.X - size2.X * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            count3 = size2.Y - size2.Y * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration2 / count)));
                            texquad2.updateVertices(count2, count3);
                        }
                        count++;
                    }
                    else
                    {
                        animstate = 2;
                        count = 0;
                        count2 = 0;
                        count3 = 0;
                        texquad2.updateVertices(count2, count3);
                    }
                }
                else if (animstate == 2)
                {
                    if (count < duration1)
                    {
                        if (count != 0)
                        {
                            count2 = size1.X - size1.X * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration1 / count)));
                            count3 = size1.Y - size1.Y * ((float)Math.Sin(((Math.PI / 2.0f)) / (duration1 / count)));
                            texquad1.updateVertices(count2, count3);
                        }
                        count++;
                    }
                    else
                    {
                        count = 0;
                        animstate = 0;
                        count = 0;
                        disappear = false;
                        count2 = 0;
                        count3 = 0;
                        texquad1.updateVertices(count2, count3);
                    }
                }
            }
        }

        public void Appear(Texture2D newtex)
        {
            appear = true;
            contenttex = newtex;
            visible = true;
        }

        public void Disappear()
        {
            disappear = true;
        }

        public void Draw(Matrix view, Matrix projection, Vector2 position)
        {
            if(visible == true)
            {
                game1.screenManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
                quadEffect.Alpha = 1.0f;

                foreach (TexturedQuad quadtex in quadtexs)
                {
                    quadEffect.Texture = quadtex.tex;
                    quadEffect.World = 
                          Matrix.CreateTranslation(new Vector3( quadtex.pos.X, quadtex.pos.Y, quadtex.pos.Z ))
                       *  Matrix.CreateRotationY((float)Math.PI/4.0f)
                       *  Matrix.CreateTranslation(new Vector3(position.X, 0, position.Y))
                          ;
                    quadEffect.View = view;
                    quadEffect.Projection = projection;
                    quadEffect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

                    foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        game1.screenManager.GraphicsDevice.DrawUserIndexedPrimitives
                            <VertexPositionNormalTexture>(
                            PrimitiveType.TriangleList,
                             quadtex.Vertices, 0, 4,
                            quadtex.Indexes, 0, 2);
                    }
                }
                quadEffect.Texture = texquad1.tex;
                quadEffect.Alpha = alpha;
                quadEffect.World =
                    Matrix.CreateTranslation(new Vector3(texquad1.pos.X, texquad1.pos.Y, texquad1.pos.Z))
                  * Matrix.CreateRotationY((float)Math.PI / 4.0f)
                  * Matrix.CreateTranslation(new Vector3(position.X, 0, position.Y))
                    ;
                quadEffect.View = view;
                quadEffect.Projection = projection;
                quadEffect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

                    foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
                    {
                        quadEffect.Texture = contenttex;
                        pass.Apply();
                        game1.screenManager.GraphicsDevice.DrawUserIndexedPrimitives
                            <VertexPositionNormalTexture>(
                            PrimitiveType.TriangleList,
                             texquad1.Vertices, 0, 4,
                            texquad1.Indexes, 0, 2);
                    }
            }
        }
    }
}
