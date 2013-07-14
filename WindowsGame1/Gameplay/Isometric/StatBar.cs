using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    //used for Boris' decision making and previously for star logic. 
    class StatBar
    {
        public enum State
        {
            Empty,
            Normal,
            Full
        }

        public string name;
        public int size;  //how big the statbar is
        public int baseValue; //default value at start
        public int currentValue;
        bool full;
        public State state;
        
        public StatBar(string name, int size, int baseValue)
        {
            this.name = name;
            this.size = size;
            this.baseValue = baseValue;
            this.currentValue = this.baseValue;
            this.UpdateState();
        }

        public void Increase()
        {
            currentValue++;
            this.UpdateState();
        }

        public void Decrease()
        {
            currentValue--;
            this.UpdateState();
        }

        public void Reset()
        {
            this.currentValue = baseValue;
            this.UpdateState();
        }

        public void Empty()
        {
            this.currentValue = 0;
            this.UpdateState();
        }

        //checks whether the state needs to change based on currentvalue
        public void UpdateState()
        {
            if (currentValue <= 0)
            {
                state = State.Empty;
                currentValue = 0;
            }
            else if (currentValue >= size)
            {
                state = State.Full;
                currentValue = size;
            }
            else
            {
                state = State.Normal;
            }
        }

        //percentage is used to decide what is a priority for Boris
        public float Percentage()
        {
            if(this.size != 0)
            {
                return (float)this.currentValue / (float)this.size;
            }
            return 0;
        }
    }
}
