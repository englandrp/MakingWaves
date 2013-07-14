using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace WindowsGame1
{
    class Camera
    {
        public Matrix view;
        public Matrix projection;
        Vector2 zoomRange = new Vector2(-400.0f, 10000.0f); //required levels for orthographic not to eat through world
        public bool cameraZoomIn; //queue zooming
        public bool cameraZoomOut; //queue zooming

        public int state = 2; //zoomed in or out 2/1
        //int amount = 130; //trial and error to get this base zoom level for orthographic

        int amount = 300; //trial and error to get this base zoom level for orthographic
        //float currentZoom = 1.0f; //current zoom level
        float currentZoom = 0.15f; //current zoom level

        //float zoomedOut = 1.0f; //level when zoomed out
        float zoomedOut = 0.70f; //level when zoomed out


        float zoomedIn = 0.25f; //level when zoomed in
        //int zoomNoFrames = 60; //how many frames it takes to zoom all the way in/out

        int zoomNoFrames = 100; //how many frames it takes to zoom all the way in/out
        int currentFrame; //which frame of animating are we on
        Vector2 defaultFocus = new Vector2(-50, 40); //where on the floow to centre the camera
        float camOffset = 20.0f; //offset when zoomed in?

        public Camera(Point gridOffset)
        {
            currentZoom = zoomedIn;
            projection =
                            Matrix.CreateRotationX(0.95f)//transform into birdseye view
                          * Matrix.CreateRotationZ(0.785398f)
                          * Matrix.CreateTranslation(new Vector3(defaultFocus.X, defaultFocus.Y, 0)) //move camera in the XY

                          * Matrix.CreateRotationZ(-0.785398f)//move back to isometric
                          * Matrix.CreateRotationX(-0.95f)
                          * Matrix.CreateOrthographicOffCenter(-amount * zoomedIn, 
                                                                  amount * zoomedIn,
                                                                 -amount * 0.5625f * zoomedIn, //o.56 is screen ratio
                                                                  amount * 0.5625f * zoomedIn,
                                                                  zoomRange.X,
                                                                  zoomRange.Y)
                            ;
            view = Matrix.CreateLookAt(new Vector3(1, 1, 1),
                                       new Vector3(0, 0, 0), Vector3.Up);//does really matter the scale of values, zooming/moving done with projection
        }

        public void Update(Vector2 borisPos)
        {
            if (cameraZoomIn == true && state == 1)
            {
                this.ZoomIn(borisPos);
            }
            else if (cameraZoomOut == true && state == 2)
            {
                this.ZoomOut(borisPos);
            }
            else if(state == 2)
            {
                float ratio = (currentFrame * (1.0f / zoomNoFrames));
                Vector2 desiredFocusPoint = new Vector2((-defaultFocus.X - borisPos.X + camOffset), (-defaultFocus.Y + borisPos.Y - camOffset));

                projection =
                Matrix.CreateRotationX(0.95f)
              * Matrix.CreateRotationZ(0.785398f)
              * Matrix.CreateTranslation(new Vector3(defaultFocus.X, defaultFocus.Y, 0))
              * Matrix.CreateTranslation(new Vector3(desiredFocusPoint.X, desiredFocusPoint.Y, 0))
              * Matrix.CreateRotationZ(-0.785398f)
              * Matrix.CreateRotationX(-0.95f)
              * Matrix.CreateOrthographicOffCenter(-amount * currentZoom,
                                                      amount * currentZoom,
                                                     -amount * 0.5625f * currentZoom,
                                                      amount * 0.5625f * currentZoom,
                                                      zoomRange.X,
                                                      zoomRange.Y)
                ;
            }

        }

        private void ZoomIn(Vector2 borisPos)
        {
            if (currentFrame < zoomNoFrames)
            {
                currentFrame++;
                currentZoom -= (zoomedOut - zoomedIn) * (1.0f / zoomNoFrames);

                float ratio = (currentFrame * (1.0f / zoomNoFrames));
                Vector2 desiredFocusPoint = new Vector2((-defaultFocus.X - borisPos.X + camOffset), (-defaultFocus.Y + borisPos.Y - camOffset));

                projection =
                Matrix.CreateRotationX(0.95f)
              * Matrix.CreateRotationZ(0.785398f)
               * Matrix.CreateTranslation(new Vector3(defaultFocus.X, defaultFocus.Y, 0))
              * Matrix.CreateTranslation(new Vector3(desiredFocusPoint.X * ratio, desiredFocusPoint.Y * ratio, 0))
              * Matrix.CreateRotationZ(-0.785398f)
              * Matrix.CreateRotationX(-0.95f)
              * Matrix.CreateOrthographicOffCenter(-amount * currentZoom,
                                                      amount * currentZoom,
                                                     -amount * 0.5625f * currentZoom,
                                                      amount * 0.5625f * currentZoom,
                                                      zoomRange.X,
                                                      zoomRange.Y)
                ;
            }
            else
            {
                currentFrame = 0;
                cameraZoomIn = false;
                state = 2;
                currentZoom = zoomedIn;
            }
        }

        private void ZoomOut(Vector2 borisPos)
        {
            if (currentFrame < zoomNoFrames)
            {
                currentFrame++;
                currentZoom += (zoomedOut - zoomedIn) * (1.0f / zoomNoFrames);
                projection =
                Matrix.CreateRotationX(0.95f)
              * Matrix.CreateRotationZ(0.785398f)
              * Matrix.CreateTranslation(new Vector3(defaultFocus.X, defaultFocus.Y, 0))
              * Matrix.CreateTranslation(new Vector3((-defaultFocus.X - borisPos.X + camOffset) - (-defaultFocus.X - borisPos.X + camOffset) * (currentFrame * (1.0f / zoomNoFrames)),
                                                     (-defaultFocus.Y + borisPos.Y - camOffset) - (-defaultFocus.Y + borisPos.Y - camOffset) * (currentFrame * (1.0f / zoomNoFrames)), 
                                                     0))
               * Matrix.CreateRotationZ(-0.785398f)
               * Matrix.CreateRotationX(-0.95f)
               * Matrix.CreateOrthographicOffCenter(-amount * currentZoom,
                                                      amount * currentZoom,
                                                     -amount * 0.5625f * currentZoom,
                                                      amount * 0.5625f * currentZoom,
                                                      zoomRange.X,
                                                      zoomRange.Y)
       ;
            }
            else
            {
                currentFrame = 0;
                state = 1;
                cameraZoomOut = false;
                currentZoom = zoomedOut;
            }
        }

        public void ZoomInCamera(Vector2 borisPos)
        {
            cameraZoomIn = true;
        }

        public void ZoomOutCamera()
        {
            cameraZoomOut = true;
        }
    }
}
