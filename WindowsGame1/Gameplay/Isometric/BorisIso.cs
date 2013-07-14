using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XNAnimation;
using XNAnimation.Controllers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio; 
using System.Diagnostics;

namespace WindowsGame1
{
    //contains most of the isometric game logic 
    class BorisIso
    {
        public enum Desire //what Boris most wants to do next
        {
            None,
            Hungry,
            Thirsty,
            Toilet,
            Clean,
            Sleep,
        }

        enum State //current state of the gameplay
        {
            ThumbsUp,
            WaitingBeforePerforming,
            Walking,
            TurningToPos,
            TurningToCamera,
            TurningToCamera2,
            WaitingForInput,
            PerformingAction,
            WaitingForZoomIn,
            WaitingForZoomOut,
            WipingStars,
            ZoomingIn,
            ZoomingOut,
            Turning,
            goingtotoilet,
            toiletturn,
            leavingtoilet,
            FoodTimer,
            ShowingAward,

        }

        Game1 game1;
        ContentManager content;
        KitchenLevel level; //holds the hotspots and level models
        IsometricWorld world; 
        public BorisMover borisMover; //contains the logic for moving Boris around and path finding
        public BorisModel borisModel; //contains the 3d model for Boris and items and animationcontrollers
      
        State currentState = State.Walking;
        public Desire currentDesire;

        public List<StatBar> statBars;
        StatBar barHappiness;
        StatBar barFrustration;
        StatBar barHunger;
        StatBar barThirst;
        StatBar barBoredom;
        StatBar barToilet;
        StatBar barClean;
        StatBar barSleep;
        public StatBar barRedstar;
        public StatBar barGoldstar;
        StatBar barAwards; 
        public int numAwards; //size of barAwards

        public bool toMove = true; //queues up Boris starting to walk
        public bool moving = false; // Boris has started walking

        int hungerCounter; //stops his hunger going up with every action, not sure if needed
        int pausecounter; //used for timing toilet animations
        int foodcounter; //timer for turning the food wheel
        int foodTurnsToMake; //which food item to take (rand 1 - 6)
        int foodTurnCounter; //how many food items have been cycled
        public bool needWash; //has boris been to the toilet but not washed his hands

        //images for the speechbubble
        Texture2D hungryTex;
        Texture2D thirstyTex;
        Texture2D toiletTex;
        Texture2D cleanTex;
        Texture2D sleepTex;

        //sound effects, need to be udpated with correct files
        SoundEffect correctSound;
        SoundEffect wrongSound;
        SoundEffect eatSound;
        SoundEffect drinkSound;
        SoundEffect flushSound;
        SoundEffect washSound;
        SoundEffect sleepSound;

        
        SoundEffect starSound;
        SoundEffect bubleSound;





        SoundEffect soundTrack;/////////////////////*********
        //Set the sound effects to use

        SoundEffectInstance soundEngineInstance;






        bool queueGoldAwards; //set to show star next update if correct answer

        public BorisIso(Game1 game1, IsometricWorld world, KitchenLevel level)
        {
            this.game1 = game1;
            this.world = world;
            this.level = level;
            content = game1.Content;
            borisModel = new BorisModel(game1);
            borisMover = new BorisMover(game1, level.gridOffset);


            //Old Stuff
            /*barHappiness = new StatBar("Happiness", 5, 0);
            barHunger = new StatBar("Hunger", 4, 2);
            barThirst = new StatBar("Thirst", 4, 3);
            barBoredom = new StatBar("Boredom", 2, 0);
            barFrustration = new StatBar("Frustration", 3, 0);
            barToilet = new StatBar("Toilet", 4, 0);
            barClean = new StatBar("Clean", 1, 0);
            barRedstar = new StatBar("Red Stars", 3, 0);
            barGoldstar = new StatBar("Gold stars", 3, 0);
            barAwards = new StatBar("Awards Unlocked", 5, 0);
            barSleep = new StatBar("Tiredness", 8, 0);
            statBars = new List<StatBar>();*/



            //New Stuff
            barHappiness = new StatBar("Happiness", 5, 0);
            barHunger = new StatBar("Hunger", 4, 2);
            barThirst = new StatBar("Thirst", 4, 3);
            barBoredom = new StatBar("Boredom", 2, 0);
            barFrustration = new StatBar("Frustration", 3, 0);
            barToilet = new StatBar("Toilet", 4, 0);
            barClean = new StatBar("Clean", 1, 0);
            barRedstar = new StatBar("Red Stars", 3, 0);
            barGoldstar = new StatBar("Gold stars", 3, 0);
            barAwards = new StatBar("Awards Unlocked", 5, 0);
            barSleep = new StatBar("Tiredness", 8, 0);
            statBars = new List<StatBar>();

            //statBars.Add(barHunger);
            //statBars.Add(barThirst);
           // statBars.Add(barToilet);
           // statBars.Add(barClean);
           // statBars.Add(barHappiness);
           // statBars.Add(barBoredom);
            //statBars.Add(barFrustration);
            //statBars.Add(barRedstar);
            //statBars.Add(barGoldstar);
            //statBars.Add(barAwards);
           // statBars.Add(barSleep);
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent()
        {
            hungryTex = game1.Content.Load<Texture2D>("Graphics/thoughtfood1");
            thirstyTex = game1.Content.Load<Texture2D>("Graphics/thoughtdrink");
            toiletTex = game1.Content.Load<Texture2D>("Graphics/thoughttoilet");
            cleanTex = game1.Content.Load<Texture2D>("Graphics/thoughtwash");
            sleepTex = game1.Content.Load<Texture2D>("Graphics/thoughtsleep");

            correctSound = content.Load<SoundEffect>("home");
            wrongSound = content.Load<SoundEffect>("wrong");
            eatSound = content.Load<SoundEffect>("eat");
            drinkSound = content.Load<SoundEffect>("drink");
            flushSound = content.Load<SoundEffect>("toilet");
            washSound = content.Load<SoundEffect>("wash");
            sleepSound = content.Load<SoundEffect>("sleep");



        


            soundTrack =content.Load<SoundEffect>("soundTrack");

            soundEngineInstance = soundTrack.CreateInstance();





            borisModel.LoadContent(game1.Content);
        }

        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public  void Update(GameTime gameTime)
        {
            hungerCounter++; //should be moved into the 


            //SoundTrack

            int seconds = (int)gameTime.TotalGameTime.TotalSeconds;

            if (seconds > 3000)
            //if (hungerCounter <= 0)

            {
                if ( soundEngineInstance.State != SoundState.Stopped)
                    soundEngineInstance.Stop();
            }
            else
                if (soundEngineInstance.State == SoundState.Stopped)
                {
                    soundEngineInstance.Volume = 0.30f;
                    soundEngineInstance.IsLooped = true;
                    soundEngineInstance.Play();
                }


            
            
            
            
            //Checks through each possible state 
            switch (currentState)
            {
                case State.ThumbsUp: //Sign detected, start the process of moving to the hotspot
                    borisModel.PlayAnim("ThumbsUp");
                    if (borisMover.thePath.Count > 0) //checks length of path
                    {
                        moving = false;//reset this
                        toMove = true;//queue the need to start moving



                    }
                    currentState = State.Walking;
                    break;


                    //WALKING
                case State.Walking: //move to location
                    if (borisModel.AnimatingOnRequest == false) //check Boris isn't half way through an animation like eating drinking
                    {
                        if (borisModel.speechBubble.visible == true && moving == false) //hide speech bubble 
                        {
                            borisModel.speechBubble.Disappear();
                        }
                        if (toMove == true)
                        {
                            borisMover.FindPath(true); //get the new path from the quadmanager
                            borisModel.Walk(); //play walkcycle animation
                            toMove = false; //reset
                            moving = true;

                            //walkingSound.Play();//********************************************************waling sound
                        }

                        if (moving == true && borisMover.thePath.Count > 0)
                        {
                            borisMover.MoveBoris(); //updates his position
                        }
                        else
                        {
                            if (borisMover.thePath.Count < 1) //while there is still a path...
                            {
                                Point theAngle;
                                if (borisMover.currentHotSpot != -1) //if an actual hotspot
                                {
                                     theAngle = level.hotSpots[borisMover.currentHotSpot].direction; //direction Boris has to run
                                    float ang = borisMover.RotateOnSpot(theAngle);
                                    borisModel.stepFromWalking(ang); //Animate a slight step when turning, using ang to change duration
                                    currentState = State.Turning;
                                    break;
                                }
                                else //if not at a hotspot (eg at middle at start)
                                {
                                    float ang = borisMover.RotateOnSpot(new Point(1, 1));//looks at camera
                                    borisModel.stepFromWalking(ang);
                                    currentState = State.TurningToCamera2;
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case State.Turning: //keep doing this until boris isn't turning any more
                    if (borisMover.rotating == false)
                    {
                        if (borisMover.currentHotSpot != -1)
                        {
                            currentState = State.WaitingBeforePerforming; //queue up an animation
                        }
                        else
                        {
                            currentState = State.WaitingForInput; //wait for kinect
                        }
                    }
                    break;



                //WAITING
                case State.WaitingForInput: //allows the kinect to make signs as long as we are zoomed in
                    if (currentDesire == Desire.None && world.camera.state == 2)
                    {
                        this.checkDesire();
                    }
                    break;

                case State.WaitingBeforePerforming: //checks it's not a sign that requres special action (toilet or eat)
                    if (borisMover.currentHotSpot != -1)
                    {
                        string theName = level.hotSpots[borisMover.currentHotSpot].name;
                        if (theName == "toilet")
                        {
                            world.kitchenLevel.openDoor();
                            borisMover.positionNewEndPoint(level.toiletSpot.location, true); //move to the toilet, ignore walls in quadmanager
                            borisMover.FindPath(true);
                            borisModel.Walk();
                            toMove = false;
                            moving = true;
                            currentState = State.goingtotoilet;
                        }
                        else if (theName == "eat")
                        {
                            if (foodTurnsToMake > foodTurnCounter) //
                            {
                                if (foodcounter < 30) 
                                {
                                    foodcounter++;
                                }
                                else
                                {
                                    foodcounter = 0;
                                    foodTurnCounter++; 
                                    level.SwapFood();
                                }
                            }
                            else
                            {
                                foodTurnCounter = 0;
                                currentState = State.PerformingAction;
                            }
                        }
                        else
                        {
                            currentState = State.PerformingAction;
                        }
                    }
                    break;



                //TOILET
                case State.goingtotoilet: //moving into toilet
                    if (moving == true && borisMover.thePath.Count > 0) //repeat until at location
                    {
                        borisMover.MoveBoris();
                    }
                    else
                    {
                        if (borisMover.thePath.Count < 1) //once there, close door and turn boris
                        {
                            world.kitchenLevel.closeDoor();
                            currentState = State.toiletturn; 
                        }
                    }
                    break;

                case State.toiletturn: //turn Boris while in toilet, then queue actions based on pausecounter
                    if (pausecounter < 150)
                    {
                        borisMover.RotateBoris();
                        pausecounter++;
                    }
                    else
                    {
                        if (pausecounter < 151)
                        {
                            world.kitchenLevel.openDoor();
                            pausecounter++;
                        }
                        else if (pausecounter < 160)
                        {
                            pausecounter++;
                        }
                        else
                        {
                            pausecounter = 0;
                            borisMover.positionNewEndPoint(level.getHotspotLocation("toilet").location, true); //move back to initial spot outside toilet
                            borisMover.FindPath(true);

                            toMove = false;
                            moving = true;
                            currentState = State.leavingtoilet; ;
                            flushSound.Play();
                        }
                    }
                    break;

                case State.leavingtoilet:
                    if (moving == true && borisMover.thePath.Count > 0) //keep moving until back at toilet hotspot
                    {
                        borisMover.MoveBoris();
                    }
                    else
                    {
                        if (borisMover.thePath.Count < 1)
                        {
                            world.kitchenLevel.closeDoor();
                            currentState = State.TurningToCamera;
                            float ang = borisMover.RotateOnSpot(new Point(1, 1));
                            borisModel.stepFromWalking(ang);
                            borisMover.quadManager.ReadWalls();
                        }
                    }
                    break;


                    //???
                case State.PerformingAction: //perform one of the main animation eg eating
                    string theName2 = level.hotSpots[borisMover.currentHotSpot].name;
                    this.PlaySound(theName2);
                    borisModel.Win(theName2);


                    currentState = State.TurningToCamera;
                    break;



                case State.TurningToCamera:
                    if (borisModel.AnimatingOnRequest == false) //wait until Boris has finished animating
                    {
                        float ang =  borisMover.RotateOnSpot(new Point(1, 1));
                        borisModel.stepFromStanding(ang);
                        currentState = State.TurningToCamera2;

                        if (queueGoldAwards == true) //If the answer matched the speechbubble
                        {
                            queueGoldAwards = false;
                            barGoldstar.Increase(); //give a new star
                            world.ShowStar(); //animate the star
                            if (barGoldstar.state == StatBar.State.Full) //if full, we need to zoom out to show the award being given and stars being cleared
                            {
                                currentState = State.ZoomingOut;
                            }
                        }
                    }
                    break;



                case State.TurningToCamera2: //second stage of turning to camera, waiting for everything else to finish before moving on
                    if (borisMover.rotating == false && currentDesire == Desire.None && world.stars.Stage == 0)
                    {
                        currentState = State.WaitingForInput;
                    }
                    break;


                    //ZOOM OUT
                case State.ZoomingOut:
                    if (world.stars.Stage == 0) //if the star animation has stopped
                    {
                        world.camera.ZoomOutCamera();
                        currentState = State.ShowingAward;
                    }
                    break;



                case State.ShowingAward: 
                    if (world.camera.state == 1 && world.stars.clearStars == false) //wait until camera is fully zoomed out (state 1)
                    {
                        world.ClearStars();
                        currentState = State.WipingStars;
                    }
                    break;



                case State.WipingStars:
                    if (world.camera.state == 1 && world.stars.clearStars == false) //wait until stars are off the sreen, then animate in the award
                    {
                        barAwards.Increase();
                        numAwards = barAwards.currentValue;
                        world.kitchenLevel.AddAward();
                        barGoldstar.Empty();
                        currentState = State.ZoomingIn;
                    }
                    break;

                case State.ZoomingIn:
                    if (world.stars.clearStars == false && world.kitchenLevel.awardFade == false ) //zoom back in once the stars and awards are both finished animaeing
                    {
                        world.camera.ZoomInCamera(borisMover.borisPos);
                        currentState = State.WaitingForInput;
                    }
                    break;

                default:
                    break;
            }

            borisModel.Update(gameTime);
            borisMover.Update();
        }

        public void PlaySound(string theName2)
        {
            if (theName2 == "eat")
            {
                eatSound.Play();
            }
            else if (theName2 == "drink")
            {
                drinkSound.Play();
            }
            else if (theName2 == "wash")
            {
                washSound.Play();
            }
            else if (theName2 == "sleep")
            {
                sleepSound.Play();
            }
        }

        //Takes the percentage of each bar based on how full it is, and uses this to decide which sign Boris wants next
        public void checkDesire()
        {
            float percentage = 0;
            currentDesire = Desire.None;

            if (barToilet.Percentage() > percentage)
            {
                percentage = barToilet.Percentage();
                currentDesire = Desire.Toilet;
            }
            if (barClean.Percentage() > percentage)
            {
                percentage = barClean.Percentage();
                currentDesire = Desire.Clean;
            }
            if (barHunger.Percentage() > percentage)
            {
                percentage = barHunger.Percentage();
                currentDesire = Desire.Hungry;
            }
            if (barThirst.Percentage() >= percentage)
            {
                percentage = barThirst.Percentage();
                currentDesire = Desire.Thirsty;
            }
            if (barSleep.Percentage() >= percentage)
            {
                percentage = barSleep.Percentage();
                currentDesire = Desire.Sleep;
            }

            this.ShowDesire();
        }

        //Launch a new speech bubble
        public void ShowDesire()
        {
            if (currentDesire != Desire.None)
            {
                if (currentDesire == Desire.Clean)
                {
                       borisModel.ShowSpeechBubble(cleanTex);
                }
                else if (currentDesire == Desire.Hungry)
                {
                    borisModel.ShowSpeechBubble(hungryTex);
                }
                else if (currentDesire == Desire.Thirsty)
                {
                    borisModel.ShowSpeechBubble(thirstyTex);
                }
                else if (currentDesire == Desire.Toilet)
                {
                    borisModel.ShowSpeechBubble(toiletTex);
                }
                else if (currentDesire == Desire.Sleep)
                {
                    borisModel.ShowSpeechBubble(sleepTex);
                }
            }
            else
            {
                //speechBubble.Disappear();
            }
        }






        //waits for input from the kinect then calls the sign's method///////****************************************************************
        public void DoSignAction(int thesign)
        {
            if (currentState == State.WaitingForInput)
            {
                if (thesign == 4)
                {
                    this.Eat();
                }
                else if (thesign == 5)
                {
                    this.Drink();
                }
                else if (thesign == 2)
                {
                    this.Toilet();
                }
                else if (thesign == 3)
                {
                    this.Clean();
                }
               // else if (thesign == 6)
                //{
                  //  this.Sleep();
               // }
            }
            //else if (eating == true)
            //{
            //    this.Yes();
            //}
        }



        public void Eat()
        {
            if (needWash == false) //check he hasn't just been to the toilet
            {
                correctSound.Play();
                System.Random rand = new System.Random();
                foodTurnsToMake = rand.Next(0, 5); //choose which bit of food on the food wheel to animate to
                borisMover.currentHotSpot = 0;
                borisMover.positionNewEndPoint(level.hotSpots[0].location, false); //set up move to the food wheel
                currentState = State.ThumbsUp;
                barHunger.Decrease();
                barThirst.Increase();
                barToilet.Increase();
                barSleep.Increase();
                if (currentDesire == Desire.Hungry)
                {
                    Happy();
                }
                currentDesire = Desire.None;
             //   Yes();
            }
            else
            {
                world.SoundAlarm(); //if needs to wash hands
                wrongSound.Play();
            }
        }



        public void Drink()
        {
            if (needWash == false)
            {
                correctSound.Play();
                borisMover.currentHotSpot = 1;
                borisMover.positionNewEndPoint(level.hotSpots[1].location, false);

                if (barThirst.state == StatBar.State.Empty)
                {
                    Frustrate();
                }
                else
                {
                    if (currentDesire == Desire.Thirsty)
                    {
                        Happy();
                    }
                }
                barThirst.Decrease();
                barToilet.Increase();
                barSleep.Increase();
                currentDesire = Desire.None;
                this.Increment();
                currentState = State.ThumbsUp;
            }
            else
            {
                world.SoundAlarm();
                wrongSound.Play();
            }
        }



        public void Toilet()
        {
            correctSound.Play();
            borisMover.currentHotSpot = 2;
        
            borisMover.positionNewEndPoint(level.hotSpots[2].location, false);

            if (barToilet.state == StatBar.State.Empty)
            {
                Bored();
            }
            else
            {
                if (currentDesire == Desire.Toilet)
                {
                    Happy();
                }
            }
            needWash = true;
            barToilet.Empty();
            barClean.Increase();
            barSleep.Increase();
            currentDesire = Desire.None;
            this.Increment();
            currentState = State.ThumbsUp;
        }

        public void Clean()
        {
            correctSound.Play();
            borisMover.currentHotSpot = 3;
         
            borisMover.positionNewEndPoint(level.hotSpots[3].location, false);

            if (barClean.state == StatBar.State.Empty)
            {
                Bored();
            }
            else
            {
                if (currentDesire == Desire.Clean)
                {
                    Happy();
                }
            }
            needWash = false;
            world.CancelAlarm();
            barClean.Empty();
            barSleep.Increase();
            currentDesire = Desire.None;
            this.Increment();
            currentState = State.ThumbsUp;
        }



        public void Sleep()
        {
            if (needWash == false)
            {
                correctSound.Play();
                borisMover.currentHotSpot = 4;

                borisMover.positionNewEndPoint(level.hotSpots[4].location, false);
                currentState = State.ThumbsUp;

                if (barSleep.state == StatBar.State.Empty)
                {
                    Frustrate();
                }
                else
                {
                    if (currentDesire == Desire.Sleep)
                    {
                        Happy();
                    }
                }
                barSleep.Empty();
                currentDesire = Desire.None;
                this.Increment();
                currentState = State.ThumbsUp;
            }
            else
            {
                world.SoundAlarm();
                wrongSound.Play();
            }
        }

        //no longer used for emotions, just adds gold straigh away
        public void Happy()
        {
            Golds();
            //No longer required happiness to be full to get star, just does it straight away
            //barHappiness.Increase();
            //if (barHappiness.state == StatBar.State.Full)
            //{
            //    barHappiness.Empty();
            //    Golds();
            //}
        }

        //queue up a star animation
        public void Golds()
        {
            queueGoldAwards = true;
            //world.ShowStar();
            //barGoldstar.Increase();
            //if (barGoldstar.state == StatBar.State.Full)
            //{
            //    barAwards.Increase();
            //    numAwards = barAwards.currentValue;
            //    barGoldstar.Empty();
            //}
        }

        //not sure this is correctly implemented, but works ok so left for now
        public void Increment()
        {
            if (hungerCounter % 3 == 0)
            {
                barHunger.Increase();
            }
        }

        //no longer used
        public void Frustrate()
        {
            barFrustration.Increase();
            if (barFrustration.state == StatBar.State.Full)
            {
                barFrustration.Empty();
                barRedstar.Increase();
            }
        }

        //no longer used
        public void Bored()
        {
            barBoredom.Increase();
            if (barBoredom.state == StatBar.State.Full)
            {
                barBoredom.Empty();
                Frustrate();
            }
        }

        //no longer used, was for emotions
        public void Yes()
        {
            //if (barHunger.state == StatBar.State.Empty)
            //{
            //    Frustrate();
            //}
            //else
            //{
            //    Happy();
            //}

            //barHunger.Decrease();
            //barThirst.Increase();
            //barToilet.Increase();
            //barSleep.Increase();

            //currentDesire = Desire.None;
            ////currentState = State.PerformingAction;
            //this.Increment();
            //eating = false;
        }
                /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            borisModel.Draw( gameTime,  view,  projection, borisMover.borisRotation, borisMover.borisPos);
       //     borisMover.quadManager.Draw(game1.GraphicsDevice, view, projection);
        }
    }
}
