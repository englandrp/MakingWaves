//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    //stores the location and rotation of a point for Boris to walk to, eg in front of a table
    class HotSpot
    {
        public string name;  //hotspot name
        public Point location;  //location in grid coordinates
        public Point direction;  //direction to face
        public HotSpot(Point theloc, Point thedir, string name)
        {
            this.location = theloc;
            this.direction = thedir;
            this.name = name;
        }
    }
}
