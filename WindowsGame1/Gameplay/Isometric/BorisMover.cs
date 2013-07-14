using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    //changes Boris rotation and position as he moves around the room, gets the paths from the path finder
    class BorisMover
    {
        public QuadManager quadManager; //handles the path finding and drawing the path on the floor in debug mode

        public List<Vector2> thePath;  //cuurent path that Boris is following - world coordinates
        public Vector2 borisPos;       //Boris' current position - world coordinates
        public int currentHotSpot = -1; 
        public float borisRotation = (float)Math.PI / 2; //Boris' current rotation
        float rotationSpeed = 0.1f; 
        float rotationToGo = 0.0f; //how much Boris has to turn to be facing the correct direction
        float targetRotation; //the direction Boris is trying to turn to
        public bool rotating; //whether Boris is rotating
        float walkspeed = 0.1555f * 2.5f;

        public BorisMover(Game1 game1, Point gridOffset)
        {
            quadManager = new QuadManager(game1, gridOffset);
            borisPos = quadManager.ReturnTilePos(quadManager.CurrentTilePos);
            thePath = quadManager.ReturnPath(true);
        }

        //takes a grid coordinate and find a new path to it, either around walls or through them
        public void positionNewEndPoint(Point newEndPoint, bool ignorewalls)
        {
            quadManager.CreateEndTile(borisPos, newEndPoint, ignorewalls); //tell the quad manager there is a new end point
            thePath = quadManager.ReturnPath(ignorewalls);

            if (thePath.Count > 1)
            {
                //So that Boris doesn't always go to the first tile if it involves going backwards,
                //check whether to take a shortcut to tile[1], as long as it's closer to Boris that it is to tile[0].
                //unlikely to be needed in game as Boris won't be interrupted while walking
                double distBetweenTiles01 = Math.Sqrt(Math.Pow(thePath[1].X - thePath[0].X, 2.0f) + Math.Pow(thePath[1].Y - thePath[0].Y, 2.0f));
                double distBetweenBorisTile1 = Math.Sqrt(Math.Pow(thePath[1].X - borisPos.X, 2.0f) + Math.Pow(thePath[1].Y - borisPos.Y, 2.0f));
                if (distBetweenTiles01 > distBetweenBorisTile1)
                {
                    double route1dist = distBetweenTiles01 +
                            Math.Sqrt(Math.Pow(borisPos.X - thePath[0].X, 2.0f) + Math.Pow(borisPos.Y - thePath[0].Y, 2.0f));
                    double route2dist = Math.Sqrt(Math.Pow(borisPos.X - thePath[1].X, 2.0f) + Math.Pow(borisPos.Y - thePath[1].Y, 2.0f));
                    if (route1dist > route2dist)
                    {
                        thePath.RemoveAt(0);
                    }
                }
            }
        }

        public void Update()
        {
            if (rotating == true)
            {
                RotateBoris();
            }
        }

        //update Boris' rotation
        public void RotateBoris()
        {
            rotationToGo = targetRotation - borisRotation;
            rotationToGo = constrainangle(rotationToGo);
            if (rotationToGo > rotationSpeed)
            {
                borisRotation += rotationSpeed;
            }
            else if (rotationToGo < -rotationSpeed)
            {
                borisRotation -= rotationSpeed;
            }
            else
            {
                this.rotating = false;
            }
            borisRotation = constrainangle(borisRotation);
        }

        //update Boris' location
        public void MoveBoris()
        {
            rotating = true;
            Vector2 currentDirection = new Vector2(thePath[0].X - borisPos.X, thePath[0].Y - borisPos.Y);
            currentDirection = Vector2.Normalize(currentDirection);
            this.RotateToVector(currentDirection);
            float distanceToNextTile = (float)Math.Sqrt(Math.Pow(thePath[0].X - borisPos.X, 2.0f) + Math.Pow(thePath[0].Y - borisPos.Y, 2));
            if (distanceToNextTile > walkspeed)
            {
                borisPos = Vector2.Add(Vector2.Multiply(currentDirection, walkspeed), borisPos);
            }
            else
            {
                thePath.Remove(thePath[0]);
            }
        }

        //get new path?
        public void FindPath(bool ignorewalls)
        {
            thePath = quadManager.ReturnPath(ignorewalls);
        }

        //what boris should do at the end point
        public  Vector2 BorisAtEndPoint(Action<float> myMethodName)
        {
            this.RotateOnSpot(new Point(1, 1));
            return (borisPos);
        }

        public float RotateOnSpot(Point theAngle)
        {
            this.RotateToVector(new Vector2(theAngle.X,theAngle.Y));
           return rotationToGo;
        }

        public void RotateToVector(Vector2 theAngle)
        {
            rotating = true;
            Vector2 directvec = Vector2.Normalize(theAngle);
            targetRotation = (float)Math.Atan2(directvec.X, directvec.Y);
            targetRotation = constrainangle(targetRotation);
            this.RotateBoris();
        }

        public float constrainangle(float angle)
        {
            if (angle > Math.PI)
            {
                angle = -(float)(Math.PI * 2) + angle; ;
            }
            else if (angle < -Math.PI)
            {
                angle = (float)(Math.PI * 2) + angle;
            }
            return angle;
        }
    }
}
