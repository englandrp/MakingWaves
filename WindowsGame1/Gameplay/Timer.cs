using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    //timer waits a set period of time then triggers an action, used with the timermanager
    public class Timer
    {
        public Action Trigger;
        public float Interval;
        float Elapsed;
        TimerManager theManager;

        public Timer(TimerManager theManager, Action theTrigger, float theInterval)
        {
            Trigger = theTrigger;
            Interval = theInterval;
            this.theManager = theManager;
            theManager.Add(this);
        }

        public void Update(float Seconds)
        {
            Elapsed += Seconds;
            if (Elapsed >= Interval)
            {
                Trigger.Invoke();
                Destroy();
            }
        }

        public void Destroy()
        {
            theManager.Remove(this);
        }
    }
}
