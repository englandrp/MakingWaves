using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNAnimation;
using XNAnimation.Controllers;

namespace WindowsGame1
{
    //contains all the models for the level and the hotspots for each bit of furniture
    class KitchenLevel
    {
        Game1 game1;


        public List<HotSpot> hotSpots; //the locations for Boris to walk to
        List<Model> roommodels;
        List<Model> foodModels;
        Model waterStack; //stck of cans
        AnimationController doorAnimationController; 
        SkinnedModel toiletdoor;
        TexturedQuad bgquad;
        BasicEffect quadEffect;
        Texture2D bgtex;

        public Point gridOffset = new Point(0, 0); //how much to offset all items by, not sure it still works
        public HotSpot toiletSpot;

        List<Model> awardModels;
        List<Vector2> awardPositions;
        int numAwards; //current number of awards
        public bool awardFade; //to fade award
        float alpha = 1.0f; //tramsparency of award
        float awardAlpha; //

        List<Vector3> colvecs; //list of colours for food
        float angleinc = ((float)Math.PI * 2.0f) / 6.0f / 20.0f; //amount to increase rotation of food by each frame//360degs / 6 food items / 20 frames
        bool rotate; //if the food should rotate
        int counter; //frame counter for rotation
        float totalangleinc; //total rotation

        public KitchenLevel(Game1 game1, Point gridOffset)
        {
            this.game1 = game1;
            this.gridOffset = gridOffset;
            quadEffect = new BasicEffect(game1.screenManager.GraphicsDevice);
            quadEffect.EnableDefaultLighting();

            quadEffect.World = Matrix.Identity;
            quadEffect.TextureEnabled = true;

            roommodels = new List<Model>();
            hotSpots = new List<HotSpot>();

            hotSpots.Add(new HotSpot(Offset(1, 2), new Point(-1, 0), "eat"));
            hotSpots.Add(new HotSpot(Offset(5, 1), new Point(0, -1), "drink"));
            hotSpots.Add(new HotSpot(Offset(2, 6), new Point(-1, 0), "toilet"));
            hotSpots.Add(new HotSpot(Offset(1, 7), new Point(0, 1), "wash"));
            hotSpots.Add(new HotSpot(Offset(5, 7), new Point(0, 1), "sleep"));

            colvecs = new List<Vector3>();
            colvecs.Add(new Vector3(0,0,1));
            colvecs.Add(new Vector3(1,0,0));
            colvecs.Add(new Vector3(0,1,0));
            colvecs.Add(new Vector3(1,1,0));
            colvecs.Add(new Vector3(1,0,1));
            colvecs.Add(new Vector3(0,1,1));

            toiletSpot = new HotSpot(Offset(-1, 6), new Point(1, 0), "toiletspot");
        }

        public void AddAward()
        {
            numAwards++;
            awardFade = true;
            awardAlpha = 0.0f;
        }

        public HotSpot getHotspotLocation(string name)
        {
            foreach (HotSpot aspot in hotSpots)
            {
                if(aspot.name == name)
                {
                    return aspot;
                }
            }
            return null;
        }

        //offset item by base amount
        public Point Offset(int x, int y)
        {
            return new Point(x + gridOffset.X, y + gridOffset.Y);
        }

        public void LoadContent(ContentManager content)
        {
            bgtex = content.Load<Texture2D>("Graphics/earth");
            bgquad = new TexturedQuad(Vector3.Zero, Vector3.Normalize(new Vector3(0, 0, 1)), Vector3.Up, 400, 200, new Vector3(-500,-500,-500), bgtex);
            foodModels = new List<Model>();

            for (int i = 0; i < 6; i++)
            {
                foodModels.Add(content.Load<Model>("Models/FoodStack1"));
            }
            waterStack = content.Load<Model>("Models/WaterStack1");

            Model table1 = content.Load<Model>("Models/table1");
            table1.Tag = "table1";
            Model table2 = content.Load<Model>("Models/table2");
            table2.Tag = "table2";
            Model table3 = content.Load<Model>("Models/table3");
            table3.Tag = "table3";
            Model drinkmachine = content.Load<Model>("Models/Water machine");
            drinkmachine.Tag = "drinkmachine";
            Model foodmachine = content.Load<Model>("Models/Food table");
            foodmachine.Tag = "foodmachine";
            Model toilet = content.Load<Model>("Models/Toilet");
            toilet.Tag = "toilet";
             toiletdoor = content.Load<SkinnedModel>("Models/toilet_door2");
            toiletdoor.Model.Tag = "toiletdoor";
            Model washbasin = content.Load<Model>("Models/basin");
            washbasin.Tag = "washbasin";
            Model roomModel = content.Load<Model>("Models/Kitchen_newh");
            roomModel.Tag = "kitchen";
            Model boxModel = content.Load<Model>("Models/box");
            boxModel.Tag = "box";
            Model bedModel = content.Load<Model>("Models/bed");
            bedModel.Tag = "bed";
            roommodels.Add(table1);
            roommodels.Add(table2);
            roommodels.Add(table3);
            roommodels.Add(drinkmachine);
            roommodels.Add(foodmachine);
            roommodels.Add(toilet);
            roommodels.Add(toiletdoor.Model);
            roommodels.Add(washbasin);
            roommodels.Add(roomModel);
            roommodels.Add(bedModel);

            doorAnimationController = new AnimationController(toiletdoor.SkeletonBones);
            doorAnimationController.Speed = 1.0f;
            doorAnimationController.TranslationInterpolation = InterpolationMode.Linear;
            doorAnimationController.OrientationInterpolation = InterpolationMode.Linear;
            doorAnimationController.ScaleInterpolation = InterpolationMode.Linear;
            doorAnimationController.PlayClip(toiletdoor.AnimationClips["dud"]); //had issues animating door, need to play this first to fix it
            doorAnimationController.LoopEnabled = false;

            awardModels = new List<Model>();
            awardPositions = new List<Vector2>();
            Model award1 = content.Load<Model>("Models/award1");
            award1.Tag = "award1";
            awardModels.Add(award1);
            Vector2 awardPos1 = new Vector2 (70 , 50);
            awardPositions.Add(awardPos1);
            Model award2 = content.Load<Model>("Models/award2");
            award2.Tag = "award2";
            awardModels.Add(award2);
            Vector2 awardPos2 = new Vector2(0, 10);
            awardPositions.Add(awardPos2);
            Model award3 = content.Load<Model>("Models/award3");
            award3.Tag = "award3";
            awardModels.Add(award3);
            Vector2 awardPos3 = new Vector2(60, 30);
            awardPositions.Add(awardPos3);
            Model award4 = content.Load<Model>("Models/award4");
            award4.Tag = "award4";
            awardModels.Add(award4);
            Vector2 awardPos4 = new Vector2(75, 30);
            awardPositions.Add(awardPos4);
            Model award5 = content.Load<Model>("Models/award5");
            award5.Tag = "award5";
            awardModels.Add(award5);
            Vector2 awardPos5 = new Vector2(90, 30);
            awardPositions.Add(awardPos5);
        }

        public void openDoor()
        {
            doorAnimationController.CrossFade(toiletdoor.AnimationClips["open"], TimeSpan.FromSeconds(0.00f));
        }

        public void closeDoor()
        {
            doorAnimationController.CrossFade(toiletdoor.AnimationClips["close"], TimeSpan.FromSeconds(0.00f));
        }

        public void SpinWheel()
        {
            rotate = true;
        }

        public void SwapFood()
        {
            rotate = true;
        }

        public void Update(GameTime gameTime)
        {
            if (rotate == true && counter < 20)
            {
                totalangleinc += angleinc;
                counter++;
            }
            else
            {
                rotate = false;
                counter = 0;
            }

            doorAnimationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
        }

        public void Draw(Matrix view, Matrix projection)
        {
             Matrix bgprojection= Matrix.CreateOrthographic(400, 160, -400.0f, 10000.0f);
             Matrix bgview = Matrix.CreateLookAt(new Vector3(1, 1, 1), new Vector3(0, 0, 0), Vector3.Up);
            game1.screenManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            quadEffect.Alpha = 1.0f;
            quadEffect.Texture = bgquad.tex;
            quadEffect.World = Matrix.CreateTranslation(new Vector3(0, 0, 0))
                             * Matrix.CreateRotationY((float)Math.PI / 4.0f)
                             * Matrix.CreateTranslation(new Vector3(-800.0f, -800.0f, -800.0f));
            quadEffect.View = bgview;
            quadEffect.Projection = bgprojection;
            quadEffect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);

            foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                game1.screenManager.GraphicsDevice.DrawUserIndexedPrimitives
                    <VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                     bgquad.Vertices, 0, 4,
                    bgquad.Indexes, 0, 2);
            }

            //draw anywhere from (0,0) to (6,6)
            foreach (Model theModel in roommodels)
            {
                alpha = 1.0f;
                if ((string)theModel.Tag == "table1")
                {
                    //this.DrawFurniture(false, theModel, Offset(0, 0), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if ((string)theModel.Tag == "table2")
                {
                    //this.DrawFurniture(false, theModel, Offset(0, 4), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if (theModel.Tag == "table3")
                {
                    //this.DrawFurniture(false, theModel, Offset(1, 0), 3, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if (theModel.Tag == "drinkmachine")
                {
                    this.DrawFurniture(false, theModel, Offset(5, 0), 4, new Vector3(0, 0, 4.0f), Vector3.One, view, projection, null);
                }
                else if (theModel.Tag == "foodmachine")
                {
                    //this.DrawFurniture(false, theModel, Offset(0, 2), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if (theModel.Tag == "toiletdoor")
                {
                    //this.DrawFurniture(true, theModel, Offset(-1, 0), 4, new Vector3(-5.0f, 0, -7.5f), Vector3.One, view, projection, doorAnimationController);
                }
                else if (theModel.Tag == "toilet")
                {
                    //this.DrawFurniture(false, theModel, Offset(-1, 6), 4, new Vector3(-5.0f, 0, 0), new Vector3(3.5f, 1.5f, 3.5f), view, projection, null);
                }
                else if (theModel.Tag == "washbasin")
                {
                    this.DrawFurniture(false, theModel, Offset(1, 8), 4, new Vector3(0.0f, 0, -4.0f), Vector3.One, view, projection, null);
                }
                else if (theModel.Tag == "kitchen")
                {
                    this.DrawFurniture(false, theModel, Offset(0, 0), 4, new Vector3(-7.5f, 0, -7.5f), Vector3.One, view, projection, null);
                }
                else if (theModel.Tag == "bed")
                {
                    this.DrawFurniture(false, theModel, Offset(4, 8), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
            }

            for (int i = 0; i < numAwards;i++)
            {
                if (i == numAwards-1 && awardFade == true) //then animate alpha of  new award
                {
                    
                    awardAlpha += 0.02f;
                    if (awardAlpha >= 1.0f)
                    {
                        awardAlpha = 1.0f;
                        awardFade = false;
                    }
                    alpha = awardAlpha;
                }else{
                    alpha = 1.0f;
                }
                if ((string)awardModels[i].Tag == "award1")
                {
                    this.DrawFurniture(false, awardModels[i], Offset(2, 2), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if ((string)awardModels[i].Tag == "award2")
                {
                    this.DrawFurniture(false, awardModels[i], Offset(3, 3), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if ((string)awardModels[i].Tag == "award3")
                {
                    this.DrawFurniture(false, awardModels[i], Offset(4, 4), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if ((string)awardModels[i].Tag == "award4")
                {
                    this.DrawFurniture(false, awardModels[i], Offset(5, 5), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
                else if ((string)awardModels[i].Tag == "award5")
                {
                    this.DrawFurniture(false, awardModels[i], Offset(6, 6), 4, Vector3.Zero, Vector3.One, view, projection, null);
                }
            }

            float numinc = 0;
            float rad = 10.0f;


            //BUrguers************************************************************
            foreach (Model theModel in foodModels)
            {
                alpha = 1.0f;
                                Matrix[] transforms = new Matrix[theModel.Bones.Count];
                theModel.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (ModelMesh mesh in theModel.Meshes)
                {
                    Vector2 anglenew = turn(totalangleinc + numinc * (float)(Math.PI * 2.0f) / 6.0f);
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World =  transforms[mesh.ParentBone.Index]
                                      * Matrix.CreateScale(0.9f)
                                      * Matrix.CreateTranslation(new Vector3(20.0f, 8.5f + (anglenew.Y * rad), 30 + anglenew.X * rad));
                        effect.View = view;
                        effect.Projection = projection;
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                        if (mesh.Name == "polySurface86"
                            || mesh.Name == "polySurface90"
                            || mesh.Name == "polySurface92"
                            || mesh.Name == "polySurface88"
                            || mesh.Name == "polySurface93"
                            || mesh.Name == "polySurface91"
                            || mesh.Name == "polySurface87"
                            || mesh.Name == "polySurface89")
                        {
                            effect.EmissiveColor = colvecs[(int)numinc];
                        }
                        effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f); // Add some overall ambient light.
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    //mesh.Draw();
                }
                numinc++;

                foreach (ModelMesh mesh in waterStack.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                                     * Matrix.CreateTranslation(new Vector3(20.0f, 500.0f, 20.0f));
                        effect.View = view;
                        effect.Projection = projection;
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                        effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f); // Add some overall ambient light.
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
            }
        }

        //rotate a vector by an angle
        public Vector2 turn(float angle)
        {
            float X = (float)Math.Sin(angle);
            float Y = (float)Math.Cos(angle);
            return new Vector2(X, Y);
        }

        public void DrawFurniture(bool skinned, Model theModel, Point pos, int direction, Vector3 adjustment, Vector3 scale, Matrix view, Matrix projection, AnimationController controller)
        {
            if (skinned == false)
            {
                Matrix[] transforms = new Matrix[theModel.Bones.Count];
                theModel.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (ModelMesh mesh in theModel.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.Alpha = alpha;
                        effect.World = transforms[mesh.ParentBone.Index]
                                     * Matrix.CreateTranslation(new Vector3(adjustment.X, adjustment.Y, adjustment.Z))
                                     * Matrix.CreateRotationY(direction * ((float)Math.PI / 2.0f))
                                     * Matrix.CreateScale(scale)
                                     * Matrix.CreateTranslation(new Vector3((15 * pos.X), 0, (15 * pos.Y)));
                        effect.View = view;
                        effect.Projection = projection;
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                        effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f); // Add some overall ambient light.
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
            }
            else
            {
                foreach (ModelMesh mesh in theModel.Meshes)
                {
                    foreach (SkinnedEffect effect in mesh.Effects)
                    {
                        effect.SetBoneTransforms(controller.SkinnedBoneTransforms);
                        effect.World = Matrix.CreateTranslation(new Vector3(adjustment.X, adjustment.Y, adjustment.Z))
                                     * Matrix.CreateRotationY(direction * ((float)Math.PI / 2.0f))
                                     * Matrix.CreateScale(scale)
                                     * Matrix.CreateTranslation(new Vector3((15 * pos.X), 0, (15 * pos.Y)));
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                        effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f); // Add some overall ambient light.
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}
