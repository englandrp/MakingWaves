//------------------------------------------------------------------------------
// <copyright file="SkeletonStreamRenderer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace WindowsGame1
{
    using System;
    using Microsoft.Kinect;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using CppCLINativeDllWrapper;
   // using System.Diagnostics;

    /// <summary>
    /// A delegate method explaining how to map a SkeletonPoint from one space to another.
    /// </summary>
    /// <param name="point">The SkeletonPoint to map.</param>
    /// <returns>The Vector2 representing the target location.</returns>
    public delegate Vector2 SkeletonPointMap(SkeletonPoint point);

    /// <summary>
    /// This class is responsible for rendering a skeleton stream.
    /// </summary>
    public class SkeletonStreamRenderer : Object2D
    {
        //  public static bool drawSkeleton = true;
        /// <summary>
        /// This is the map method called when mapping from
        /// skeleton space to the target space.
        /// </summary>
        private readonly SkeletonPointMap mapMethod;

        /// <summary>
        /// The last frames skeleton data.
        /// </summary>
        private static Skeleton[] skeletonData;
        

        /// <summary>
        /// This flag ensures only request a frame once per update call
        /// across the entire application.
        /// </summary>
        private static bool skeletonDrawn = true;

        /// <summary>
        /// The origin (center) location of the joint texture.
        /// </summary>
        private Vector2 jointOrigin;

        /// <summary>
        /// The joint texture.
        /// </summary>
        private Texture2D jointTexture;

        /// <summary>
        /// The origin (center) location of the bone texture.
        /// </summary>
        private Vector2 boneOrigin;

        /// <summary>
        /// The bone texture.
        /// </summary>
        private Texture2D boneTexture;

        /// <summary>
        /// Whether the rendering has been initialized.
        /// </summary>
        private bool initialized;

        private Hand leftHand;
        private Hand rightHand;
        private GestureRecogniser gestureRecogniser;
        private bool isMoving;
        private int _theSign = 0;
        private int _theResult = -1;
        private int leawayCounter = 0;
        private int motionCounter = 0;
        private TimeSpan lastSignTime;
        private bool _checkCorrect;

        private float[] skeletonArray;

        private int activeSkeleton = 0;
        SignPack signPack;

  
        public  bool toHide;
        private JointType[] joints;
        Skeleton theActiveSkeleton;

        public Skeleton TheActiveSkeleton
        {
            get { return theActiveSkeleton; }
        }
        public static Vector3 rightHandPos = Vector3.Zero;
        public static Vector3 neckPos = Vector3.Zero;

        public Vector3 RightHandPos
        {
            get { return rightHandPos; }
        }

        /// <summary>
        /// Initializes a new instance of the SkeletonStreamRenderer class.
        /// </summary>
        /// <param name="game">The related game object.</param>
        /// <param name="map">The method used to map the SkeletonPoint to the target space.</param>
        public SkeletonStreamRenderer(Game1 game1, SkeletonPointMap map, SignPack signPack)
            : base(game1)
        {
            this.mapMethod = map;
            this.signPack = signPack;
        }

        /// <summary>
        /// This method loads the textures and sets the origin values.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.jointTexture = Game.Content.Load<Texture2D>("Graphics/Joint");
            this.jointOrigin = new Vector2(this.jointTexture.Width/2, this.jointTexture.Height/2);
            this.boneTexture = Game.Content.Load<Texture2D>("Graphics/Bone");
            this.boneOrigin = new Vector2(0.5f, 0.0f);

         //   contenttex = game1.Content.Load<Texture2D>("Graphics/star");

        }
        

        public void SetSignPack( SignPack signPack )
        {
            this.signPack = signPack;
            NativeMethods.setSignPack(this.signPack.classFile, this.signPack.nameFile);
        } 
        /// <summary>
        /// This method initializes necessary values.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            skeletonArray = new float[40];
            this.Size = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            this.initialized = true;

            this.leftHand = new Hand();
            this.rightHand = new Hand();

            this.gestureRecogniser = new GestureRecogniser();

            isMoving = false;

            lastSignTime = new GameTime().TotalGameTime;


            NativeMethods.setSignPack(this.signPack.classFile, this.signPack.nameFile);
         

            joints = new JointType[13]
                {
                    JointType.Head, // HIP_CENTER0
                    JointType.ShoulderCenter, // SPINE1
                    JointType.Spine, // SHOULDER_CENTER2
                    JointType.ShoulderLeft, // HEAD3
                    JointType.ElbowLeft, // SHOULDER_LEFT4
                    JointType.HandLeft, // ELBOW_LEFT5
                    JointType.HipLeft, // WRIST_LEFT6
                    JointType.ShoulderRight, // HAND_LEFT7
                    JointType.ElbowRight, // SHOULDER_RIGHT8
                    JointType.HandRight, // ELBOW_RIGHT9
                    JointType.HipRight, // WRIST_RIGHT10
                    JointType.WristLeft, // HAND_RIGHT11
                    JointType.WristRight // HIP_LEFT12
                };
        }

        
        public int GetSign( bool checkCorrect )
        {
            _checkCorrect = checkCorrect; //if iso we need the actual sign ID
            //Console.WriteLine("-- GetSign() -- " + _theResult + ", " + signPack.GetSignNum(_theResult));
            int r = _theResult;
            if (r > -1) _theResult = -1;
            //return signPack.GetSignNum(_theResult); ??????????????????????????
            return r;

            
        }


        public int TestForSign(int theSign, int precisionLevel, bool leftHanded)
        {
           _theSign = signPack.GetSignNum(theSign);
                int tResult = _theResult;
                if (_theResult > -1)
                {
                    _theResult = -1;
                }
                return tResult;

            //// IDs are out of whack so let's sort that...
            //switch (theSign)
            //{
            //    case 0:
            //        _theSign = 4;
            //        break;
            //    case 1:
            //        _theSign = 5;
            //        break;
            //    case 2:
            //        _theSign = 2;
            //        break;
            //    case 3:
            //        _theSign = 3;
            //        break;
            //    case 4:
            //        _theSign = 9;
            //        break;
            //    case 5:
            //        _theSign = 4;
            //        break;
            //}
           // _theSign = theSign;
            //Debug.Write(theSign _theSign);
 
        }

        /// <summary>
        /// This method retrieves a new skeleton frame if necessary.
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

            // If we have already drawn this skeleton, then we should retrieve a new frame
            // This prevents us from calling the next frame more than once per update
            if (skeletonDrawn)
            {
                using (var skeletonFrame = this.Chooser.Sensor.SkeletonStream.OpenNextFrame(0))
                {
                    // Sometimes we get a null frame back if no data is ready
                    if (null == skeletonFrame)
                    {
                        return;
                    }

                    // Reallocate if necessary
                    if (null == skeletonData || skeletonData.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        
                    }

               

                    if (null != skeletonData[0])
                    {
                        // get active skeleton
                        activeSkeleton = -1;
                        float nearestZ = 999999999999; //?
                        for (int i = 0; i < skeletonData.Length; i++)
                        {
                            float skelHeadZ = skeletonData[i].Joints[JointType.Head].Position.Z;
                            if (skelHeadZ > 0) //ignore if 0
                            {
                                if (skelHeadZ < nearestZ)
                                {
                                    //the nearest skeleton is the active one
                                    /*TODO - make sure this is also the most central 
                                    (and maybe track to avoid occlusion issues)*/
                                    nearestZ = skelHeadZ;
                                    activeSkeleton = i;
                                }
                            }
                        }

                        if (activeSkeleton > -1)
                        {

                            
                            //     theActiveSkeleton = skeletonData[activeSkeleton];
                            JointCollection skelJoints = skeletonData[activeSkeleton].Joints;
                            this.gestureRecogniser.update(skelJoints);

                            rightHandPos = new Vector3(skelJoints[JointType.HandRight].Position.X,
                                                       skelJoints[JointType.HandRight].Position.Y,
                                                       skelJoints[JointType.HandRight].Position.Z);


                            neckPos = new Vector3(skelJoints[JointType.Head].Position.X,
                                                       skelJoints[JointType.Head].Position.Y,
                                                       skelJoints[JointType.Head].Position.Z);



                            // store info about both hands
                            SkeletonPoint pos = skelJoints[JointType.HandLeft].Position;
                            Vector3 handPos = new Vector3();
                            handPos.X = pos.X;
                            handPos.Y = pos.Y;
                            handPos.Z = pos.Z;

                            //get the velocity here?
                            float leftVel = Vector3.Distance(handPos, this.leftHand.Position);
                            this.leftHand.Position = handPos;

                            pos = skelJoints[JointType.HandRight].Position;
                            handPos = new Vector3();
                            handPos.X = pos.X;
                            handPos.Y = pos.Y;
                            handPos.Z = pos.Z;
                            float rightVel = Vector3.Distance(handPos, this.rightHand.Position);
                            this.rightHand.Position = handPos;

                            //this checks if the hands are stupidly close to the feet (if so I think we can assume kinect hasn't picked up valid skel)
                            pos = skelJoints[JointType.FootRight].Position;
                            Vector3 feetPos = new Vector3();
                            feetPos.X = pos.X;
                            feetPos.Y = pos.Y;
                            feetPos.Z = pos.Z;
                            float handsToFeetDist = Vector3.Distance(this.rightHand.Position, feetPos);
                            
                            // NB: we now simply check if either right or left hand is raised
                            // EITHER hand must be raised above waist to signal start of sign
                            // BOTH hands must be dropped below waist to signal end of sign - only then will we send the trigger (or if max frames reached)
                            bool leftRaised = false;
                            bool rightRaised = false;

                            if(handsToFeetDist > 0.1)
                            {
                                //Console.WriteLine(" activeSkeleton: " + activeSkeleton);
                                //Console.WriteLine(handPos.Y);
                                leftRaised = (handPos.Y > skelJoints[JointType.HipRight].Position.Y);
                                rightRaised = (this.leftHand.Position.Y > skelJoints[JointType.HipLeft].Position.Y);
                            } else
                            {
                                //Console.WriteLine(" NOT A VALID SKELETON " + handsToFeetDist);
                            }

                            //make sure two signs not immediately after one another (e.g. from hands returning to rest position)
                            TimeSpan timeDiff = gameTime.TotalGameTime.Subtract(lastSignTime);

                            //check if we think a sign is happening / either hand has been raised above waist
                            float minVelThreshold = 0.09f;
                            if (leftRaised || rightRaised)
                            {
                                //only do this if NOT already known to be moving
                                if (!isMoving)
                                {
                                    isMoving = true;

                                    //Console.WriteLine(" ++ STARTED MOVING ++ " + activeSkeleton + " " + gameTime.TotalGameTime);
                                    NativeMethods.startRecording();
                                }
                                leawayCounter = 0;
                            }
                            else if (isMoving)
                            {
                                isMoving = false;

                                //Console.WriteLine(" ------ STOPPED SIGN ------ " + gameTime.TotalGameTime);
                                leawayCounter = 0; //TODO check if this is not needed...?

                                //do different things depending on whether we're in the iso game
                                if (_checkCorrect)
                                {
                                    _theResult = GetMostLikelyResult();
                                }
                                else
                                {
                                    _theResult = GetResult(_theSign);
                                }

                                lastSignTime = gameTime.TotalGameTime;
                            }

                            if (isMoving)
                            {
                                motionCounter++;
                            }

                            /*
                            if (motionCounter > 50)
                            {
                                //Console.WriteLine(" SIGN TAKING LONG TIME - sending to blackbox anyway...");

                                _theResult = GetResult(_theSign);


                                lastSignTime = gameTime.TotalGameTime;
                                motionCounter = 0;
                                leawayCounter = 0;
                                isMoving = false;
                            }
                             * */

                            skeletonArray.SetValue(40, 0); // ??
                            int c = 0;

                            for (int i = 0; i < joints.Length; i++)
                            {
                                Joint joint = skelJoints[joints[i]];
                                SkeletonPoint posJ = joint.Position;
                                c++;
                                skeletonArray[c] = joint.Position.X;
                                c++;
                                skeletonArray[c] = joint.Position.Y;
                                c++;
                                skeletonArray[c] = joint.Position.Z;
                            }

                            int length = NativeMethods.processSkeleton(skeletonArray);
                        }
                    }

                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    skeletonDrawn = false;
                }
            }
        }


        /// <summary>
        /// This method tells us the most likely sign to be correct
        /// </summary>
        /// <param name="result">The sign ID</param>
        public int GetMostLikelyResult()
        {
            int res = -1;
            res = NativeMethods.getMostLikelySignResults();
            Console.WriteLine("GetMostLikelyResult:" + res);
            return res;
        }

        /// <summary>
        /// This method tells us if the "sign" was correct
        /// </summary>
        /// <param name="result">The result - 1 (correct), 0 (incorrect), or -1.</param>
        private int GetResult( int signID )
        {
            int res = -1;
            //@hello@ lives outside of this classifier
            //Console.WriteLine(" THE SIGN is : " + signID);
            if(signID < 9)  res = NativeMethods.getSignResults(signID);

            //Trace.WriteLine("Chance of correctioness for... " + signID + " - " + gestureRecogniser.probabilityList[signID]);
            //crosscheck the result against the hard-coded signs
            // HARD CODED SIGNS by RE below
            Console.WriteLine(" THE SIGNPACK name is : " + signPack.nameFile);
            if (signPack.nameFile == "sptree_sd.200.30.2_Game.name")
                 if (gestureRecogniser.probabilityList[signID] > 20) res = 1;


            Console.WriteLine("signID: " + signID + ", Result = " + res);
            return res;
        }

        /// <summary>
        /// This method draws the skeleton frame data.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(GameTime gameTime)
        {
            // If the joint texture isn't loaded, load it now
            if (null == this.jointTexture)
            {
                this.LoadContent();
            }

            // If we don't have data, lets leave
            if (null == skeletonData || null == this.mapMethod)
            {
                return;
            }

            if (false == this.initialized)
            {
                this.Initialize();
            }

            this.SharedSpriteBatch.Begin();
            this.SharedSpriteBatch.Draw(this.boneTexture, Vector2.Zero, null, Color.Red, 0, this.boneOrigin, 1,
                                        SpriteEffects.None, 1.0f);
            if (activeSkeleton != -1)
            {
                // Draw Bones
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.Head, JointType.ShoulderCenter);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.ShoulderCenter, JointType.ShoulderLeft);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.ShoulderCenter, JointType.ShoulderRight);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.ShoulderCenter, JointType.Spine);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.Spine, JointType.HipCenter);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.HipCenter, JointType.HipLeft);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.HipCenter, JointType.HipRight);

                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.ShoulderLeft, JointType.ElbowLeft);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.ElbowLeft, JointType.WristLeft);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.WristLeft, JointType.HandLeft);

                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.ShoulderRight, JointType.ElbowRight);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.ElbowRight, JointType.WristRight);
                this.DrawBone(skeletonData[activeSkeleton].Joints, JointType.WristRight, JointType.HandRight);


                // Now draw the joints
                foreach (Joint j in skeletonData[activeSkeleton].Joints)
                {
                    if (toHide == false)
                    {
                        Color jointColor = Color.Green;
                        if (j.TrackingState != JointTrackingState.Tracked)
                        {
                            jointColor = Color.Yellow;
                        }

                        this.SharedSpriteBatch.Draw(
                            this.jointTexture,
                            this.mapMethod(j.Position),
                            null,
                            jointColor,
                            0.0f,
                            this.jointOrigin,
                            1.0f,
                            SpriteEffects.None,
                            0.0f);
                    }
                }
                this.SharedSpriteBatch.Draw(this.boneTexture, Vector2.Zero, null, Color.Lime, 0, this.boneOrigin, 1,
                                            SpriteEffects.None, 1.0f);
            }
            this.SharedSpriteBatch.End();
            skeletonDrawn = true;

            base.Draw(gameTime);
        }


        /// <summary>
        /// This method draws a bone.
        /// </summary>
        /// <param name="joints">The joint data.</param>
        /// <param name="startJoint">The starting joint.</param>
        /// <param name="endJoint">The ending joint.</param>
        private void DrawBone(JointCollection joints, JointType startJoint, JointType endJoint)
        {
            if (toHide == false)
            {
                Vector2 start = this.mapMethod(joints[startJoint].Position);
                Vector2 end = this.mapMethod(joints[endJoint].Position);
                Vector2 diff = end - start;
                Vector2 scale = new Vector2(1.0f, diff.Length()/this.boneTexture.Height);

                float angle = (float) Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;

                Color color = Color.LightGreen;
                if (joints[startJoint].TrackingState != JointTrackingState.Tracked ||
                    joints[endJoint].TrackingState != JointTrackingState.Tracked)
                {
                    color = Color.Gray;
                }

                this.SharedSpriteBatch.Draw(this.boneTexture, start, null, color, angle, this.boneOrigin, scale,
                                            SpriteEffects.None, 1.0f);
            }
        }
    }
}