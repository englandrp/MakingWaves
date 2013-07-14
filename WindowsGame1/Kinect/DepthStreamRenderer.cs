//------------------------------------------------------------------------------
// <copyright file="ColorStreamRenderer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace WindowsGame1
{
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    //  using CppCLINativeDllWrapper;

    /// <summary>
    /// This class renders the current color stream frame.
    /// </summary>
    public class DepthStreamRenderer : Object2D
    {
        /// <summary>
        /// This child responsible for rendering the color stream's skeleton.
        /// </summary>
        readonly SkeletonStreamRenderer skeletonStream;

        /// <summary>
        /// The last frame of color data.
        /// </summary>
        private short[] colorData;

        /// <summary>
        /// The color frame as a texture.
        /// </summary>
        Texture2D colorTexture;

        /// <summary>
        /// The back buffer where color frame is scaled as requested by the Size.
        /// </summary>
        private RenderTarget2D backBuffer;

        /// <summary>
        /// This Xna effect is used to swap the Red and Blue bytes of the color stream data.
        /// </summary>
        private Effect kinectColorVisualizer;

        /// <summary>
        /// Whether or not the back buffer needs updating.
        /// </summary>
        private bool needToRedrawBackBuffer = true;

        public bool toHide = false;
        Texture2D redPoint;
        Game1 game1;
        public Texture2D ColorTexture
        { get { return colorTexture; } }

        /// <summary>
        /// Initializes a new instance of the ColorStreamRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        public DepthStreamRenderer(Game1 game1)
            : base(game1)
        {
            game1 = this.game1;
            this.DrawOrder = 1;
        }

        /// <summary>
        /// This method loads the Xna effect.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // This effect is necessary to remap the BGRX byte data we get
            // to the XNA color RGBA format.
            this.kinectColorVisualizer = Game.Content.Load<Effect>("KinectDepthVisualizer");
            redPoint = Game.Content.Load<Texture2D>("Graphics/redPoint");
        }

        /// <summary>
        /// Initializes the necessary children.
        /// </summary>
        /// 
        //Initialize the skeleton
        public override void Initialize()
        {
            base.Initialize();
            Size = new Vector2(320, 240);
            //Position = new Vector2(1280 - 320, 0);
            Position = new Vector2(500 - 320, 0);
        }

        //Togle Camera....***********************************************************************************************
        public void ToggleCameraSize()
        {
            if (Size == new Vector2(640, 480))
            {
                Size = new Vector2(320, 240);
                Position = new Vector2(1280 - 320, 0);
            }
            else
            {
                Size = new Vector2(640, 480);
                Position = new Vector2(1280 - 640, 0);
            }
        }

        /// <summary>
        /// The update method where the new color frame is retrieved.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If the sensor is not found, not running, or not connected, stop now
            if (null == this.Chooser.Sensor ||
                false == this.Chooser.Sensor.IsRunning ||
                KinectStatus.Connected != this.Chooser.Sensor.Status)
            {
                return;
            }

            using (var frame = this.Chooser.Sensor.DepthStream.OpenNextFrame(0))
            {
                // Sometimes we get a null frame back if no data is ready
                if (frame == null)
                {
                    return;
                }

                // Reallocate values if necessary
                if (this.colorData == null || this.colorData.Length != frame.PixelDataLength)
                {
                    this.colorData = new short[frame.PixelDataLength];

                     this.colorTexture = new Texture2D(
                        Game.GraphicsDevice,
                        frame.Width,
                        frame.Height,
                        false,
                        SurfaceFormat.Bgra4444);

                    this.backBuffer = new RenderTarget2D(
                        Game.GraphicsDevice,
                        frame.Width,
                        frame.Height,
                        false,
                        SurfaceFormat.Color,
                        DepthFormat.None,
                        this.Game.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.PreserveContents);
                }

                frame.CopyPixelDataTo(this.colorData);
                this.needToRedrawBackBuffer = true;
            }

            // Update the skeleton renderer
            this.skeletonStream.Update(gameTime);
        }

        /// <summary>
        /// This method renders the color and skeleton frame.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If we don't have the effect load, load it
            if (null == this.kinectColorVisualizer)
            {
                this.LoadContent();
            }

            // If we don't have a target, don't try to render
            if (null == this.colorTexture)
            {
                return;
            }

            if (toHide == false)
            {

                if (this.needToRedrawBackBuffer)
                {
                    // Set the backbuffer and clear
                    this.Game.GraphicsDevice.SetRenderTarget(this.backBuffer);
                    this.Game.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
                    // this.colorTexture.
                    this.Game.GraphicsDevice.Textures[0] = null;

                    this.colorTexture.SetData<short>(this.colorData);

     

                    // Draw the color image
                    this.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, this.kinectColorVisualizer);
                    this.SharedSpriteBatch.Draw(this.colorTexture, Vector2.Zero, Color.White);

                    this.SharedSpriteBatch.End();

                    // Draw the skeleton
                    game1.kinectManager.skeletonStream.Draw(gameTime);


                    this.SharedSpriteBatch.Begin();
                    Rectangle mouseRect = IsometricWorld.mouseRect;

                    SharedSpriteBatch.Draw(redPoint, new Rectangle(mouseRect.X, mouseRect.Y, mouseRect.Width, 1), Color.White);
                    SharedSpriteBatch.Draw(redPoint, new Rectangle(mouseRect.X, mouseRect.Y + mouseRect.Height, mouseRect.Width, 1), Color.White);
                    SharedSpriteBatch.Draw(redPoint, new Rectangle(mouseRect.X, mouseRect.Y, 1, mouseRect.Height), Color.White);
                    SharedSpriteBatch.Draw(redPoint, new Rectangle(mouseRect.X + mouseRect.Width, mouseRect.Y, 1, mouseRect.Height), Color.White);
                    this.SharedSpriteBatch.End();


                    // Reset the render target and prepare to draw scaled image
                    this.Game.GraphicsDevice.SetRenderTargets(null);

                    // No need to re-render the back buffer until we get new data
                    this.needToRedrawBackBuffer = false;
                }

                // Draw the scaled texture
                this.SharedSpriteBatch.Begin();
                this.SharedSpriteBatch.Draw(
                    this.backBuffer,
                    new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y),
                    null,
                    Color.White);


                this.SharedSpriteBatch.End();
            }
            else
            {
                if (this.needToRedrawBackBuffer)
                {
                    this.Game.GraphicsDevice.Textures[0] = null;

                    this.colorTexture.SetData<short>(this.colorData);
                }
                  //this.skeletonStream.Draw(gameTime);
            }
            base.Draw(gameTime);
        }



        /// <summary>
        /// This method is used to map the SkeletonPoint to the color frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the color frame.</returns>
        public Vector2 SkeletonToColorMap(SkeletonPoint point)
        {
            if ((null != Chooser.Sensor) && (null != Chooser.Sensor.DepthStream))
            {
                // This is used to map a skeleton point to the color image location
                var colorPt = Chooser.Sensor.MapSkeletonPointToDepth(point, Chooser.Sensor.DepthStream.Format);
                return new Vector2(colorPt.X, colorPt.Y);
            }

            return Vector2.Zero;
        }
    }
}
