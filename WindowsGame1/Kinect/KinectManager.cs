//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using CppCLINativeDllWrapper;

namespace WindowsGame1
{
   public  class KinectManager:DrawableGameComponent
    {
        /// <summary>
        /// This control selects a sensor, and displays a notice if one is
        /// not connected.
        /// </summary>
         KinectChooser chooser;

        /// <summary>
        /// This manages the rendering of the color stream.
        /// </summary>
         readonly ColorStreamRenderer colorStream;

        /// <summary>
        /// This child responsible for rendering the color stream's skeleton.
        /// </summary>
        public SkeletonStreamRenderer skeletonStream;

        public KinectManager(Game1 game1)
            : base(game1)
        {
            // The Kinect sensor will use 640x480 for both streams
            // To make your app handle multiple Kinects and other scenarios,
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
            this.chooser = new KinectChooser(Game,ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30);
           // this.Services.AddService(typeof(KinectChooser), this.chooser);
            Game.Services.AddService(typeof(KinectChooser), this.chooser);

            //int[] resArray = new int[] { 640, 480 };
            //NativeMethods.setResolution(resArray);
            //NativeMethods.setBoneCount(29);



            // Default size is the full viewport
            this.colorStream = new ColorStreamRenderer(game1);
            this.skeletonStream = new SkeletonStreamRenderer(game1, colorStream.SkeletonToColorMap, game1.signPacks[game1.currentSignPack]);
    

            Game.Components.Add(this);
            Game.Components.Add(this.chooser);
            Game.Components.Add(this.colorStream);

           // colorStream.toHide = !colorStream.toHide;
           // skeletonStream.toHide = !skeletonStream.toHide;

            //colorStream.ToggleCameraSize();
        }

        public Texture2D getColorTexture()
        {
            return colorStream.ColorTexture;
        }

        public void ChangeCameraSize()
        {
            colorStream.ToggleCameraSize();
        }

        //Hide The Skeleton??
        public void toggleHide()
        {
            colorStream.toHide = !colorStream.toHide;
            skeletonStream.toHide = !skeletonStream.toHide;
        }

        public void UpdateParam(float theSmoothing, float theCorrection, float thePrediction, float theJitterRadius, float theMaxDeviationRadius)
        {

        }


        public int CheckForSign(int theSign, int precisionLevel, bool leftHanded)
        {
            int result = skeletonStream.TestForSign(theSign, precisionLevel, leftHanded);
            // result = -1;
            return result;
        }

        public int GetMostLikelySign()
        {
            return skeletonStream.GetMostLikelyResult();
        }


        public int GetCurrentSign(bool checkCorrect = false)
        {


            return skeletonStream.GetSign(checkCorrect);


        }

        /// <summary>
        /// Initializes class and components
        /// </summary>
        public override void Initialize()
        {
            //this.chooser.Sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
        //   imageTex = new Texture2D(Game.GraphicsDevice, 640, 480);
            base.Initialize();
   
        }

        public override void Update(GameTime gameTime)
        {


            this.skeletonStream.Update(gameTime);
               base.Update(gameTime);
        }

        /// <summary>
        /// This method renders the color and skeleton frame.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {

            // Draw the skeleton
            //this.skeletonStream.Draw(gameTime);
           // this.colorStream.Draw(gameTime);
           

           // this.skeletonStream.Draw(gameTime);
            base.Draw(gameTime);
        }

        /// <summary>
        /// This method is used to map the SkeletonPoint to the color frame.
        /// </summary>
        /// <param name="point">The SkeletonPoint to map.</param>
        /// <returns>A Vector2 of the location on the color frame.</returns>
        //private Vector2 SkeletonToColorMap(SkeletonPoint point)
        //{
        //    if ((null != chooser.Sensor) && (null != chooser.Sensor.DepthStream))
        //    {
        //        // This is used to map a skeleton point to the depth image location
        //        var depthPt = chooser.Sensor.MapSkeletonPointToColor(point, chooser.Sensor.ColorStream.Format);
        //        return new Vector2(depthPt.X, depthPt.Y);
        //    }

        //    return Vector2.Zero;
        //}

       public Vector2 getSkeletonPoint(Vector3 thePoint)
       {
           SkeletonPoint skelpo = new SkeletonPoint();
           skelpo.X = thePoint.X;
           skelpo.Y = thePoint.Y;
           skelpo.Z = thePoint.Z;

           return this.colorStream.SkeletonToColorMap(skelpo);


           return Vector2.Zero;
       }

     //   private Vector2 SkeletonToColorMap(SkeletonPoint point)
     //   {
     //       SkeletonPoint apoint = new SkeletonPoint();
     ////       (this.sensor != null && chooser.sensor.IsRunning())
     //       //if ((null != chooser.Sensor) && (null != chooser.Sensor.ColorStream)&& chooser.Sensor.IsRunning)
     //       //{
     //       //    // This is used to map a skeleton point to the color image location
     //       //    var colorPt = chooser.Sensor.MapSkeletonPointToColor(point, chooser.Sensor.ColorStream.Format);
     //       //    return new Vector2(colorPt.X, colorPt.Y);
     //       //}

     //       if ((null != chooser.Sensor) && (null != chooser.Sensor.ColorStream) && chooser.Sensor.IsRunning)
     //       {
     //           // This is used to map a skeleton point to the color image location
     //           var colorPt = chooser.Sensor.MapSkeletonPointToDepth(point, chooser.Sensor.DepthStream.Format);
     //           return new Vector2(colorPt.X, colorPt.Y);
     //       }

     //       return Vector2.Zero;
     //   }


    }
}
