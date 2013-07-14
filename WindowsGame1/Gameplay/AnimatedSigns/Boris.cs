using System;
using Microsoft.Xna.Framework;
using XNAnimation;
using XNAnimation.Controllers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class Boris
    {
        ContentManager content;
        private Matrix view;
        private Matrix projection;

        Texture2D blueTex;
        Texture2D pinkTex;
        Texture2D currentTex; //which texture is Boris currently using, either pink or blue
        Texture2D headTex;

        private SkinnedModel currentModel; //which model is currently in use, either borismodel or facelessmodel
        private SkinnedModel borisModel;
        private SkinnedModel borisFacelessModel;
        private SkinnedModel burgerModel;
        private AnimationController borisAnimController;
        private AnimationController burgerAnimController;

        private int activeAnimationClip; //which one is currently being played/to be played
        bool animatingOnRequest; // has Boris been sent a request to animate, and is he still animating

         public bool AnimatingOnRequest
         { get { return animatingOnRequest; } }

        public Boris(Game game)
        {
            content = game.Content;
            view = Matrix.CreateLookAt(new Vector3(0, 12.5f, 36), new Vector3(0, 12.5f, 0), Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1.77f, 1, 1000);
        }
        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent()
        {
            borisModel = content.Load<SkinnedModel>("Models/Hero_test");
            borisFacelessModel = content.Load<SkinnedModel>("Models/boris_noface");
            currentModel = borisModel;

            burgerModel = content.Load<SkinnedModel>("Models/padding1/FussyBurger");
            blueTex = content.Load<Texture2D>("Textures/blue");
            pinkTex = content.Load<Texture2D>("Textures/pink");
            currentTex = blueTex;
            headTex = content.Load<Texture2D>("Textures/head");

            // Create an animation controller and start a clip
            borisAnimController = new AnimationController(currentModel.SkeletonBones);
            borisAnimController.TranslationInterpolation = InterpolationMode.Linear;
            borisAnimController.OrientationInterpolation = InterpolationMode.Linear;
            borisAnimController.ScaleInterpolation = InterpolationMode.Linear;
            borisAnimController.StartClip(currentModel.AnimationClips["standing"]);
            borisAnimController.LoopEnabled = false;
            activeAnimationClip = 1;

            burgerAnimController = new AnimationController(burgerModel.SkeletonBones);
            burgerAnimController.TranslationInterpolation = InterpolationMode.Linear;
            burgerAnimController.OrientationInterpolation = InterpolationMode.Linear;
            burgerAnimController.ScaleInterpolation = InterpolationMode.Linear;
            burgerAnimController.StartClip(burgerModel.AnimationClips["standing"]);
            burgerAnimController.LoopEnabled = false;
            burgerAnimController.TranslationInterpolation = InterpolationMode.None;
            burgerAnimController.OrientationInterpolation = InterpolationMode.None;
            burgerAnimController.ScaleInterpolation = InterpolationMode.None;

        }
                /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public  void Update(GameTime gameTime)
        {
            //if Boris isn't animating, then play the standing animation
            if (borisAnimController.IsPlaying == false)
            {
                playAnimation(1);
                animatingOnRequest = false;
            }

            //animation controllers must be updated each frame
            borisAnimController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
            burgerAnimController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
        }

        //tells boris to play a clip
        public void playAnimation(int theAnimation)
        {
            animatingOnRequest = true;
                activeAnimationClip = theAnimation;
                borisAnimController.CrossFade(currentModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
                burgerAnimController.CrossFade(burgerModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
        }

        //tells Boris to Wave
        public void Wave()
        {
            animatingOnRequest = true;
            activeAnimationClip = 10;
            borisAnimController.CrossFade(currentModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
        }

        //tells Boris to play losing animation
        public void Lose()
        {
            animatingOnRequest = true;
            activeAnimationClip = 11;
            borisAnimController.CrossFade(currentModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
        }

        //toggles the colour of Boris' outfit 
        public void ChangeColor(bool defaultColor)
        {
            if(defaultColor)
                currentTex = blueTex;
            else
                currentTex = pinkTex;
        }

        //toggles which model is used for Boris
        public void ChangeFace(bool faceYN)
        {
            if (faceYN)
                currentModel = borisModel;
            else
                currentModel = borisFacelessModel;
        }
                /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in currentModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    if (mesh.Name == "BODY") 
                    {
                        effect.Texture = currentTex;
                    }
                    else if (mesh.Name == "HEAD1")
                    {
                        effect.Texture = headTex;
                    }
                    effect.SetBoneTransforms(borisAnimController.SkinnedBoneTransforms);
                    effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f); // Add some overall ambient light.
                    effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            foreach (ModelMesh mesh in burgerModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(burgerAnimController.SkinnedBoneTransforms);
                    effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f); // Add some overall ambient light.
                    effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }
}
