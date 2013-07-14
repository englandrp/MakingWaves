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

    //Contains the model for Boris and the items he holds, plays animations for several events
    class BorisModel
    {
        bool animatingOnRequest; //has Boris been asked to animate
        SkinnedModel borisModel;
        SkinnedModel itemModel;
        private AnimationController animationController;
        AnimationController itemAnimationController;
        string anim; //name of the current animation
        public SpeechBubble speechBubble;

        public bool AnimatingOnRequest
        { get { return animatingOnRequest; } }

        public BorisModel(Game1 game1)
        {
            speechBubble = new SpeechBubble(game1);
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            borisModel = content.Load<SkinnedModel>("Models/Boris_isometric");
            itemModel = content.Load<SkinnedModel>("Models/item_isometric");

            animationController = new AnimationController(borisModel.SkeletonBones);
            ////   animationController.Speed = 0.5f;
            animationController.TranslationInterpolation = InterpolationMode.Linear;
            animationController.OrientationInterpolation = InterpolationMode.Linear;
            animationController.ScaleInterpolation = InterpolationMode.Linear;
            animationController.Speed = 2.0f;
            animationController.StartClip(borisModel.AnimationClips["standing"]);

            itemAnimationController = new AnimationController(itemModel.SkeletonBones);
            ////   animationController.Speed = 0.5f;
            itemAnimationController.TranslationInterpolation = InterpolationMode.Linear;
            itemAnimationController.OrientationInterpolation = InterpolationMode.Linear;
            itemAnimationController.ScaleInterpolation = InterpolationMode.Linear;
            itemAnimationController.Speed = 1.0f;
            itemAnimationController.LoopEnabled = false;
            itemAnimationController.StartClip(itemModel.AnimationClips["drink"]);
        }

        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (animationController.HasFinished == true)
            {
                animatingOnRequest = true;
            }
            if (animationController.IsPlaying == false && animationController.CrossFading == false) //if not animating, play default standing animation
            {
                animationController.Speed = 2.0f;
                animationController.LoopEnabled = true;
                animationController.CrossFade(borisModel.AnimationClips["standing"], TimeSpan.FromSeconds(0.01f));
                animatingOnRequest = false;
            }
            animationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
            itemAnimationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
            speechBubble.Update();
        }

        public void Lose()
        {
            animatingOnRequest = true;
            animationController.CrossFade(borisModel.AnimationClips["wrong"], TimeSpan.FromSeconds(0.05f));
            animationController.LoopEnabled = false;
            animationController.Speed = 1.0f;
        }

        public void Win(string anim)
        {
            if (anim != null)
            {
                animatingOnRequest = true;
                this.anim = anim;
                animationController.CrossFade(borisModel.AnimationClips[anim], TimeSpan.FromSeconds(0.05f));
                animationController.LoopEnabled = false;
                animationController.Speed = 1.0f;
                if (anim != "wrong" && anim != "wash" && anim != "sleep") //only used for eat and drink
                {
                        itemAnimationController.CrossFade(itemModel.AnimationClips[anim], TimeSpan.FromSeconds(0.05f));
                }
            }
        }

        //play any of Boris animations eg eat drink
        public void PlayAnim(string anim)
        {
            if (anim != null)
            {
                animatingOnRequest = true;
                this.anim = anim;
                animationController.CrossFade(borisModel.AnimationClips[anim], TimeSpan.FromSeconds(0.05f));
                animationController.LoopEnabled = false;
                animationController.Speed = 1.0f;
            }
        }

        public void ShowSpeechBubble(Texture2D texture)
        {
            speechBubble.Appear(texture);
        }

        //takes a little step when turning
        public void stepFromWalking(float angle)
        {
            if (angle < 0)
            {
                angle = -angle;
            }
            angle = 0.1f + angle * 0.4f ;

            animationController.CrossFade(borisModel.AnimationClips["standing"], TimeSpan.FromSeconds(angle));
            animationController.Speed = 2.0f;
            animationController.LoopEnabled = false;
        }

        //takes a little step when turning
        public void stepFromStanding(float angle)
        {
            if (angle < 0)
            {
                angle = -angle;
            }
            angle = 0.05f + angle * 0.7f ;
            Console.WriteLine("angle:  " + angle);
            animationController.PlayClip(borisModel.AnimationClips["walking"]);
            animationController.Speed = 2.0f;
            animationController.LoopEnabled = false;
            animationController.CrossFade(borisModel.AnimationClips["standing"], TimeSpan.FromSeconds(angle));
        }

        //play walk animation
        public void Walk()
        {
            animationController.LoopEnabled = true;
            animationController.CrossFade(borisModel.AnimationClips["walking"], TimeSpan.FromSeconds(0.05f));
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection, float borisRotation, Vector2 borisPos)
        {
            foreach (ModelMesh mesh in borisModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(animationController.SkinnedBoneTransforms);
                    effect.World = Matrix.CreateRotationY(borisRotation)
                                 * Matrix.CreateTranslation(new Vector3(borisPos.X, 0.5f, borisPos.Y));
                    effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f); // Add some overall ambient light.
                    effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            foreach (ModelMesh mesh in itemModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    if (mesh.Name == "Burger2" && anim == "eat" && animatingOnRequest == true)
                    {
                        effect.SetBoneTransforms(itemAnimationController.SkinnedBoneTransforms);
                        effect.World =
                             Matrix.CreateRotationY(borisRotation)
                            * Matrix.CreateTranslation(new Vector3(borisPos.X, 0.5f, borisPos.Y))
                            ;
                        effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f); // Add some overall ambient light.
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                        effect.View = view;
                        effect.Projection = projection;
                        mesh.Draw();
                    }

                    if (mesh.Name == "Can2" && anim == "drink" && animatingOnRequest == true)
                    {
                        effect.SetBoneTransforms(itemAnimationController.SkinnedBoneTransforms);
                        effect.World = Matrix.CreateRotationY(borisRotation)
                                        * Matrix.CreateTranslation(new Vector3(borisPos.X, 0.5f, borisPos.Y));
                        effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f); // Add some overall ambient light.
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                        effect.View = view;
                        effect.Projection = projection;
                        mesh.Draw();
                    }
                }
            }
            speechBubble.Draw(view, projection, borisPos);
        }
    }
}
