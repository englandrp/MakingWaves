//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    class DebugScreen : MenuScreen
    {
        MenuEntry Seated;
        MenuEntry CameraSize;
        MenuEntry back;
        MenuEntry Smoothing;
        MenuEntry Correction;
        MenuEntry Prediction;
        MenuEntry JitterRadius;
        MenuEntry MaxDeviationRadius;
    
        KinectManager kinectManager;
        

        float theSmoothing = 0.5f;
         float    theCorrection = 0.5f;
         float    thePrediction = 0.5f;
          float   theJitterRadius = 0.05f;
            float theMaxDeviationRadius = 0.04f;

            bool cameraFullSize = true;

            public DebugScreen(KinectManager kinectManager, ScreenManager screenManager)
            : base("Debug options", screenManager)
        {
             Seated = new MenuEntry("SeatedSelect");
             back = new MenuEntry("OnCancel");
             Smoothing = new MenuEntry("SmoothingSelect");
             Correction = new MenuEntry("CorrectionSelect");
             Prediction = new MenuEntry("PredictionSelect");
             JitterRadius = new MenuEntry("JitterRadiusSelect");
             MaxDeviationRadius = new MenuEntry("MaxDeviationRadiusSelect");
             CameraSize = new MenuEntry("Camera FullSize");

             this.kinectManager = kinectManager;

            back = new MenuEntry("Back");

            Seated.Selected += SeatedSelect;
            back.Selected +=  OnCancel;
            CameraSize.Selected += ChangeCamera;

            Smoothing.Right += SmoothingRight;
            Correction.Right += CorrectionRight;
            Prediction.Right += PredictionRight;
            JitterRadius.Right += JitterRadiusRight;
            MaxDeviationRadius.Right += MaxDeviationRadiusRight;


            Smoothing.Left += SmoothingLeft;
            Correction.Left += CorrectionLeft;
            Prediction.Left += PredictionLeft;
            JitterRadius.Left += JitterRadiusLeft;
            MaxDeviationRadius.Left += MaxDeviationRadiusLeft;

            MenuEntries.Add(Seated);
            MenuEntries.Add(CameraSize);
            
            MenuEntries.Add(Smoothing);
            MenuEntries.Add(Correction);
            MenuEntries.Add(Prediction);
            MenuEntries.Add(JitterRadius);
            MenuEntries.Add(MaxDeviationRadius);
            MenuEntries.Add(back);

            int i = 1;
            foreach (MenuEntry theEntry in MenuEntries)
            {
                theEntry.Position = new Vector2(50, 50 * i);
                i++;

            }
        }

        public override void LoadContent()
        {
           // JitterRadius.Position
            //theSmoothing = screenManager.kinectManager.chooser.Sensor.SkeletonStream.SmoothParameters.Smoothing;
            //theCorrection = screenManager.kinectManager.chooser.Sensor.SkeletonStream.SmoothParameters.Correction;
            //thePrediction = screenManager.kinectManager.chooser.Sensor.SkeletonStream.SmoothParameters.Prediction;
            //theJitterRadius = screenManager.kinectManager.chooser.Sensor.SkeletonStream.SmoothParameters.JitterRadius;
            //theMaxDeviationRadius = screenManager.kinectManager.chooser.Sensor.SkeletonStream.SmoothParameters.MaxDeviationRadius;

            SetMenuEntryText();
        }

        void SetMenuEntryText()
        {
            Seated.Text = "Seated: " + ("");
            CameraSize.Text = "Camera FullSize: " + (cameraFullSize.ToString());
            Smoothing.Text = "Smoothing: " + (theSmoothing.ToString());
            Correction.Text = "Correction: " + (theCorrection.ToString());
            Prediction.Text = "Prediction: " + (thePrediction.ToString());
            JitterRadius.Text = "JitterRadius: " + (theJitterRadius.ToString());
            MaxDeviationRadius.Text = "MaxDeviation: " + (theMaxDeviationRadius.ToString());

            kinectManager.UpdateParam(theSmoothing, theCorrection, thePrediction, theJitterRadius, theMaxDeviationRadius);
        }


        void SeatedSelect(object sender, PlayerIndexEventArgs e)
        {
          //  the += 0.5f;

            
        }

        void ChangeCamera(object sender, PlayerIndexEventArgs e)
        {
            //  the += 0.5f;
            cameraFullSize = !cameraFullSize;
          //123  this.screenManager.kinectManager.colorStream.ToggleCameraSize();
            SetMenuEntryText();
        }

        void SmoothingRight(object sender, PlayerIndexEventArgs e)
        {
        //    theSmoothing = 1.0f;
            theSmoothing += 0.1f;
            if (theSmoothing >= 1.1f)
                theSmoothing = 0.0f;
            SetMenuEntryText();
        }

        void CorrectionRight(object sender, PlayerIndexEventArgs e)
        {
            theCorrection += 0.1f;
            if (theCorrection >= 1.1f)
            {
                theCorrection = 0.0f;
            }

            SetMenuEntryText();
        }

        void PredictionRight(object sender, PlayerIndexEventArgs e)
        {
            thePrediction += 0.1f;
            if (thePrediction >= 1.5)
            {
                thePrediction = 0;
            }

            SetMenuEntryText();
        }

        void JitterRadiusRight(object sender, PlayerIndexEventArgs e)
        {
            theJitterRadius += 0.01f;
            if (theJitterRadius >= 10.1f)
            {
                theJitterRadius = 0;
            }
            SetMenuEntryText();
        }

        void MaxDeviationRadiusRight(object sender, PlayerIndexEventArgs e)
        {
            theMaxDeviationRadius += 0.01f;
            if (theMaxDeviationRadius >= 10.1f)
            {
                theMaxDeviationRadius = 0;
            }
            SetMenuEntryText();
        }


        protected override void UpdateMenuEntryLocations()
        {

        }




        void SmoothingLeft(object sender, PlayerIndexEventArgs e)
        {
            theSmoothing -= 0.1f;
            if (theSmoothing < 0.0f)
            {
                theSmoothing = 1.0f;
            }
            SetMenuEntryText();
        }

        void CorrectionLeft(object sender, PlayerIndexEventArgs e)
        {
            theCorrection -= 0.1f;
            if (theCorrection < 0.0f)
            {
                theCorrection = 1.0f;
            }

            SetMenuEntryText();
        }

        void PredictionLeft(object sender, PlayerIndexEventArgs e)
        {
            thePrediction -= 0.1f;
            if (thePrediction < 0.0f)
            {
                thePrediction = 1.0f;
            }

            SetMenuEntryText();
        }

        void JitterRadiusLeft(object sender, PlayerIndexEventArgs e)
        {
            theJitterRadius -= 0.01f;
            if (theJitterRadius < 0.0f)
            {
                theJitterRadius = 1.0f;
            }
            SetMenuEntryText();
        }

        void MaxDeviationRadiusLeft(object sender, PlayerIndexEventArgs e)
        {
            theMaxDeviationRadius -= 0.01f;
            if (theMaxDeviationRadius < 0.0f)
            {
                theMaxDeviationRadius = 1.0f;
            }
            SetMenuEntryText();
        }
    }
}
