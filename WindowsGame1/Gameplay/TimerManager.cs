//using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    //stores a bunch of timers and updates/destroys them
    public class TimerManager
    {
        List<Timer> ToRemove; //finished timers
        List<Timer> Timers; //timers currently ticking

        public TimerManager()
        {
            ToRemove = new List<Timer>(); 
            Timers = new List<Timer>();
        }

        public void Add(Timer aTimer)
        {
            Timers.Add(aTimer);
        }
        public void Remove(Timer aTimer)
        {
            ToRemove.Add(aTimer);
        }
        public bool testfunc(int test)
        {
            return true;
        }

        public void CancelAllTimers()
        {
            ToRemove.Clear();
            Timers.Clear();
        }

        public void Update(GameTime gametime)
        {
            foreach (Timer theTimer in ToRemove)
            {
                Timers.Remove(theTimer);
            }
            ToRemove.Clear();
            foreach (Timer theTimer in Timers.ToArray())
            {
                theTimer.Update((float)gametime.ElapsedGameTime.TotalSeconds);
            }
        }
    }
}
