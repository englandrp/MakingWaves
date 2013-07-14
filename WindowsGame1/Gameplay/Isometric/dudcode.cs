//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace WindowsGame1.Gameplay.Isometric
//{
//    class dudcode
//    {

//        enum State
//        {
//            Starting,
//            Losing,
//            TurningToPos,
//            TurningToCamera,
//            TurningToCamera2,
//            Walking,
//            WaitingForInput,
//            WaitingForSign,
//            WaitingBeforePerforming,
//            PlayWash,
//            PlayToilet,
//            PlayEat,
//            PlayDrink
//        }

//        State currentState = State.Starting;

//        StatBar happiness;
//        StatBar frustration;
//        StatBar hungryness;
//        StatBar thirstiness;
//        StatBar boredom;
//        List<StatBar> statBars;


//         happiness = new StatBar("Happiness", 10, 5);
//            hungryness = new StatBar("Hunger",3, 0);
//            thirstiness = new StatBar("Thirst",3, 0);
//            boredom = new StatBar("Boredom", 5, 0);
//            frustration = new StatBar("Frustration", 5, 0);
//            statBars = new List<StatBar>();
//            statBars.Add(happiness);
//            statBars.Add(hungryness);
//            statBars.Add(thirstiness);
//            statBars.Add(boredom);
//            statBars.Add(frustration);

        
//            if (borisIso.AnimatingOnRequest == false)
//            {
//                if (currentState == State.Starting)
//                {
//                    currentState = State.Walking;
//                }
//                else if (currentState == State.Walking)
//                {
//                    if (borisIso.toMove == true)
//                    {
//                        thePath = quadManager.ReturnPath();
//                        borisIso.FindPathAndWalk(thePath);
//                    }

//                    borisIso.RotateBoris();


//                    if (borisIso.moving == true && thePath.Count > 0)
//                    {
//                        borisIso.MoveBoris(thePath);
//                    }
//                    else
//                    {
//                        quadManager.makeStartPos(borisIso.BorisAtEndPoint(thePath));
//                        //   currentState = State.TurningToCamera;

//                        if (thePath.Count < 2)
//                        {
//                            currentState = State.TurningToCamera;
//                        }

//                        //   
//                    }
//                }
//                else if (currentState == State.TurningToCamera)
//                {
//                    if (quadManager.endPos != new Point(3, 0))
//                    {
//                        if (NeedWash == false)
//                        {
//                            this.ZoomInCamera();
//                        }
//                        else
//                        {
//                            if (quadManager.endPos == new Point(1, 3))
//                            {
//                                this.ZoomInCamera();
//                            }
//                        }

//                    }

//                    borisIso.RotateBoris();
//                    borisIso.BorisAtEndPoint(thePath);
//                    if (borisIso.rotating == false)
//                    {
//                        currentState = State.WaitingForSign;
//                    }
//                }
//                else if (currentState == State.WaitingForSign)
//                {
//                    wrongtimerstarted = false;


//                    if (NeedWash == true)
//                    {
//                        if (quadManager.endPos == new Point(5, 5) || quadManager.endPos == new Point(6, 1))
//                        {
//                            alarmOn = true;
//                            Timer aTimer = new Timer(timerManager, () => this.CancelAlarm(), 5.0f);
//                            currentState = State.Walking;
//                            this.positionNewEndPoint(hotSpots[3].location);
//                            frustration.Increase();
//                            currentSign = 3;
//                            currentAnim = "wash";
//                        }
//                    }
//                    if (quadManager.endPos == new Point(5, 5))
//                    {
//                        currentSign = 4;
//                        currentAnim = "eat";

//                        //  borisIso.playAnimation("eat");
//                        //  currentState = State.Starting;
//                    }
//                    else if (quadManager.endPos == new Point(6, 1))
//                    {
//                        currentSign = 1;
//                        currentAnim = "drink";

//                        //  borisIso.playAnimation("drink");
//                    }
//                    else if (quadManager.endPos == new Point(1, 3))
//                    {
//                        currentSign = 3;
//                        currentAnim = "wash";

//                        // borisIso.playAnimation("wash");
//                    }
//                    else if (quadManager.endPos == new Point(1, 6))
//                    {
//                        currentSign = 2;
//                        currentAnim = "wrong";

//                        //  borisIso.playAnimation("wrong");
//                    }
//                    else
//                    {
//                        currentState = State.WaitingForInput;

//                    }
//                }
//                else if (currentState == State.TurningToPos)
//                {
//                    if (currentAnim == "drink")
//                    {
//                        borisIso.RotateOnSpot(new Vector2(0, -1));
//                    }
//                    else if (currentAnim == "eat")
//                    {
//                        borisIso.RotateOnSpot(new Vector2(1, 0));
//                    }
//                    else if (currentAnim == "wash")
//                    {
//                        borisIso.RotateOnSpot(new Vector2(-1, 0));
//                    }
//                    else if (currentAnim == "wrong")
//                    {
//                        borisIso.RotateOnSpot(new Vector2(-1, 0));
//                    }

//                    borisIso.RotateBoris();

//                    if (borisIso.rotating == false)
//                    {
//                        currentState = State.WaitingBeforePerforming;
//                    }
//                }
//                else if (currentState == State.WaitingBeforePerforming)
//                {
//                    borisIso.Win(this.currentAnim);

//                    if (currentSign == 4)//eat
//                    {
//                        if (hungryness.state == StatBar.State.Full)
//                        {
//                            frustration.Increase();
//                            // happiness.Decrease();
//                        }
//                        else
//                        {
//                            hungryness.Increase();
//                            happiness.Increase();
//                            frustration.Reset();
//                        }
//                    }
//                    else if (currentSign == 1)//drink
//                    {
//                        if (thirstiness.state == StatBar.State.Full)
//                        {
//                            frustration.Increase();
//                            //happiness.Decrease();
//                        }
//                        else
//                        {
//                            thirstiness.Increase();
//                            happiness.Increase();
//                            frustration.Reset();
//                        }
//                    }
//                    else if (currentSign == 3)//wash
//                    {
//                        if (NeedWash == true)
//                        {
//                            frustration.Reset();
//                            happiness.Increase();
//                            NeedWash = false;
//                        }
//                    }
//                    else if (currentSign == 2)//toilet
//                    {
//                        if (thirstiness.currentValue > 0 || hungryness.currentValue > 0)
//                        {
//                            thirstiness.Reset();
//                            hungryness.Reset();
//                            happiness.Increase();
//                        }
//                        else
//                        {
//                            frustration.Reset();
//                            //happiness.Decrease();
//                        }
//                        NeedWash = true;
//                    }

//                    currentState = State.TurningToCamera2;
//                }
//                else if (currentState == State.TurningToCamera2)
//                {
//                    ZoomOutCamera();
//                    borisIso.BorisAtEndPoint(thePath);
//                    borisIso.RotateBoris();

//                    if (borisIso.rotating == false)
//                    {
//                        currentState = State.WaitingForInput;
//                    }
//                }
//                else if (currentState == State.Losing)
//                {
//                    borisIso.Lose();
//                    currentState = State.WaitingForSign;
//                }
//                else if (currentState == State.PlayWash)
//                {

//                }

//            }

//            if (frustration.state == StatBar.State.Full)
//            {
//                happiness.Decrease();
//            }

//           // this.ProcessHandMovement();

//                if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D1, null, out index))
//                {
//                    currentState = State.Walking;
//                    this.positionNewEndPoint(hotSpots[0].location);
//                }
//                else
//                    if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D2, null, out index))
//                    {
//                        currentState = State.Walking;
//                        this.positionNewEndPoint(hotSpots[1].location);
//                    }
//                    else
//                        if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D3, null, out index))
//                        {
//                            currentState = State.Walking;
//                            this.positionNewEndPoint(hotSpots[2].location);
//                        }
//                        else
//                            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D4, null, out index))
//                            {
//                                currentState = State.Walking;
//                                this.positionNewEndPoint(hotSpots[3].location);
//                            }
    
    

//            if (input.isWinning(null))
//            {
//                this.Win();
//          //      zoomRange.X += 200.0f;
//            }


//            if (input.isLosing(null))
//            {
    
//         //       zoomRange.X -= 200.0f;
//            }

        //public void CancelAlarm()
        //{
        //    cancelAlarm = true;
        //}


//     public void Win()
//        {
//            if (currentState == State.WaitingForSign)
//            {
             
//                currentState = State.TurningToPos;

//                if (audioYN == true)
//                {
//                    correctSound.Play();
//                }

//                if (scoreYN == true)
//                {
//                    correcttimerstarted = true;
//                }
//            }
//        }

//        public void Lose()
//        {
//            if (currentState == State.WaitingForSign)
//            {
//                if (audioYN == true)
//                {
//                    wrongSound.Play();
//                }

//                if (scoreYN == true)
//                {
//                    wrongtimerstarted = true;
//                }
//                attemptcount++;
//                if (attemptcount == 5)
//                {
//                    currentState = State.WaitingForInput;
//                    attemptcount = 0;
//                }
//                else
//                {

//                    currentState = State.Losing;
//                }
//            }
//        }

            //foreach (StatBar statBar in statBars)
            //{
            //    this.spriteBatch.DrawString(
            //     this.font,
            //     statBar.name + ": " + statBar.currentValue + " / " + statBar.size + ".",
            //     new Vector2(20, inc * 100),
            //     Color.Red);
            //    inc++;
            //}

////BORISCODE

//       public void playAnimation(string theClip)
//        {
//            animatingOnRequest = true;

//            if (theClip == "eat")
//            {

            
//            }
//            else if (theClip == "drink")
//            {
                
//            }
//            else if (theClip == "toilet")
//            {

//            }
//            else
//            {


           
//                //   activeAnimationClip = theAnimation;
//                animationController.CrossFade(borisModel.AnimationClips[theClip], TimeSpan.FromSeconds(0.05f));
//                itemAnimationController.CrossFade(itemModel.AnimationClips[theClip], TimeSpan.FromSeconds(0.05f));
//                Trace.WriteLine("PLAY: " + theClip);
//                animationController.LoopEnabled = false;
//                animationController.Speed = 1.0f;
//                itemAnimationController.LoopEnabled = false;
//            }
//        }

//        //public void Wave()
//        //{
//        //    animatingOnRequest = true;
//        //    activeAnimationClip = 5;
//        //    animationController.CrossFade(currentModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
//        ////    animationController2.CrossFade(burgerModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
//        //}



//        public void Lose()
//        {
//            animatingOnRequest = true;
//            activeAnimationClip = 7;
//            animationController.CrossFade(borisModel.AnimationClips["wrong"], TimeSpan.FromSeconds(0.05f));
//           // itemAnimationController.CrossFade(itemModel.AnimationClips["wrong"], TimeSpan.FromSeconds(0.05f));
//            animationController.LoopEnabled = false;
//            animationController.Speed = 1.0f;
//        }

//        public void Win(string anim)
//        {

//            animatingOnRequest = true;
//            activeAnimationClip = 3;
//            this.anim = anim;
//            animationController.CrossFade(borisModel.AnimationClips[anim], TimeSpan.FromSeconds(0.05f));
//            animationController.LoopEnabled = false;
//            animationController.Speed = 1.0f;
//            if (anim != "wrong" && anim != "wash")
//            {
//                itemAnimationController.CrossFade(itemModel.AnimationClips[anim], TimeSpan.FromSeconds(0.05f));

//            }
//            //throwCard = true;
//            Trace.WriteLine("WIN");
//        }



//    }
//}
