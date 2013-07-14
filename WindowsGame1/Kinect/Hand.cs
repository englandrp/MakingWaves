//------------------------------------------------------------------------------
// simple class for tracking hand positions
//------------------------------------------------------------------------------

namespace WindowsGame1
{
    using Microsoft.Xna.Framework;

    public class Hand
    {
        /// <summary>
        /// Gets or sets the position of the object.
        /// </summary>
        public Vector3 Position { get; set; }

        public void update()
        {
            // TODO
        }

        public Vector3 getVelocity()
        {
            // TODO - get velocity
            return Position;
        }
    }
}
