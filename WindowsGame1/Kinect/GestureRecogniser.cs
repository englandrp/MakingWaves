//------------------------------------------------------------------------------
// really simplified gesture recognition based on hand/limb locations and movement between frames
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace WindowsGame1
{
    using Microsoft.Xna.Framework;
    using Microsoft.Kinect;

    public class GestureRecogniser
    {
        public int lastProbableSign { get; set; }

        private Vector3         _leftHand;
        private Vector3         _rightHand;
        private int             _holdCount;
        private JointCollection _lastJoints;

        public List<int> probabilityList;

        public GestureRecogniser()
        {
            probabilityList  = new List<int>(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        }

        public void update( JointCollection skelJoints)
        {
            if(null == _lastJoints)
            {
                Debug.Write("No joints");
                _lastJoints = skelJoints;
            }
            //what are we looking for?

            //1) either hand to head / mouth (eat)
            //2) " " with head tipped back (drink)
            //3) two hands together in centre of torso in washing movement (wash)
            //4) either hand to opposite shoulder area (toilet)
            //5) wave away from body (hello)

            for(int i = 0; i < probabilityList.Count; i++)
            {
                if (probabilityList[i] > 0) probabilityList[i]--;
                
            }

            // stores a countdown for each detected gesture
            
            /*
             * NB THESE ARE THE SIGNS MAPPED TO IDS - this needs to be reflected in the game!
                0 Come
                1 Go
                2 Toilet
                3 Wash
                4 Eat
                5 Drink
                6 Stop
                7 Yes
                8 No
                
                9 Hello
            */

            //first get head, shoulder, hand, spine values
            Vector3 head            = SkeletonPointToVector3(skelJoints[JointType.Head].Position);
            Vector3 neck            = SkeletonPointToVector3(skelJoints[JointType.ShoulderCenter].Position);
            Vector3 leftHand        = SkeletonPointToVector3(skelJoints[JointType.HandLeft].Position);
            Vector3 rightHand       = SkeletonPointToVector3(skelJoints[JointType.HandRight].Position);
            Vector3 leftShoulder    = SkeletonPointToVector3(skelJoints[JointType.ShoulderLeft].Position);
            Vector3 rightShoulder   = SkeletonPointToVector3(skelJoints[JointType.ShoulderRight].Position);
            Vector3 leftElbow       = SkeletonPointToVector3(skelJoints[JointType.ElbowLeft].Position);
            Vector3 rightElbow      = SkeletonPointToVector3(skelJoints[JointType.ElbowRight].Position);
            Vector3 spine           = SkeletonPointToVector3(skelJoints[JointType.Spine].Position);

            if (head.Z < 1.22) return;

            // easily check at any point that one hand is down
            bool oneHandDown = (leftHand.Y < spine.Y || rightHand.Y < spine.Y);
            bool leftHandDown = (leftHand.Y < spine.Y);
            bool rightHandDown = (rightHand.Y < spine.Y);

            // 0 Come
            bool comePosition = (Vector3.Distance(rightHand, rightShoulder) < 0.3);
            if (comePosition)
            {
                if (rightHand.Z - _lastJoints[JointType.HandRight].Position.Z < 0.3 && leftHandDown)
                {
                    probabilityList[0] = 100;
                }
                if (leftHand.Z - _lastJoints[JointType.HandLeft].Position.Z < 0.3 && rightHandDown)
                {
                    probabilityList[0] = 100;
                }
                
            }

            // 1 Go
            //right hand (pointing) from left to right
            bool goPosition = (Vector3.Distance(leftHand, leftElbow) < 0.18
                                      || Vector3.Distance(rightHand, rightElbow) < 0.18);
            if (goPosition)
            {
                float rDist = rightHand.X - _rightHand.X;
                float lDist = leftHand.X - _leftHand.X;
                if (lDist < -0.03
                    || rDist > 0.03)
                probabilityList[2] = 100;
            }

            // 2 Toilet
            bool toiletPosition = (Vector3.Distance(leftHand, rightShoulder) < 0.2
                                      || Vector3.Distance(rightHand, leftShoulder) < 0.2);
            if (toiletPosition)
            {
                if (leftHand.Y < spine.Y || rightHand.Y < spine.Y)
           //     Debug.WriteLine(" !!! Toilet !!! ");
                probabilityList[2] = 100;
            }

            // 3 Wash
            // two hands together in centre of torso in washing movement (wash)
            bool washPosition = (
                Vector3.Distance(leftHand, rightHand) < 0.2
                && Vector3.Distance(leftHand, spine) < 0.3
                && Vector3.Distance(rightHand, spine) < 0.3
                );
            if (washPosition)
            {
          //      Debug.WriteLine(" === Wash === ");
                probabilityList[3] = 100;
            }

            // 4 EAT
            //mouth is halfway between neck and head
            Vector3 mouth = Vector3.SmoothStep(head, neck, 0.5f);
            bool nearMouth = (Vector3.Distance(mouth, rightHand) < 0.28 || Vector3.Distance(mouth, leftHand) < 0.28);

            // 5 DRINK
            if (nearMouth)
            {
                probabilityList[2] = 0;
                //check for drink (head tipped back)
                if (oneHandDown)
                {
                    float hZ = head.Z - neck.Z;
                    if (hZ > 0.032)
                    {
                  //      Debug.WriteLine("----- DRINK -----");
                        probabilityList[5] = 100;
                    }
                    else
                    {
                   //     Debug.WriteLine("+++ EAT +++");
                        probabilityList[4] = 100;
                    }
                }
            }

            // 6 STOP


            // 7 YES


            // 8 NO

            // 9 HELLO (right or left)
            bool righthandRaised = (rightHand.Y >= rightShoulder.Y);
            bool lefthandRaised = (leftHand.Y >= leftShoulder.Y);
            if (righthandRaised)
            {
                Vector3 destination = rightHand;
                Vector3 origin = _rightHand;
                Vector3 direction = destination - origin;
                direction.Normalize();
                //Debug.WriteLine(direction.X);
                if (direction.X > 0.4) probabilityList[9] = 100;
            }
            if (lefthandRaised)
            {
                Vector3 destination = leftHand;
                Vector3 origin = _leftHand;
                Vector3 direction = destination - origin;
                direction.Normalize();
                //Debug.WriteLine(direction.X);
                if (direction.X < -0.4) probabilityList[9] = 100;
            }

            //remember the last position of all joints
            _lastJoints = skelJoints;
            _leftHand = leftHand;
            _rightHand = rightHand;
        }

        /// <summary>
        /// SkeletonPointToVector3 converts Skeleton Point to Vector3.  
        /// </summary>
        /// <param name="pt">The Skeleton Point position.</param>
        /// <returns>Returns the Vector3 position.</returns>
        public static Vector3 SkeletonPointToVector3(SkeletonPoint pt)
        {
            return new Vector3(pt.X, pt.Y, pt.Z);
        }


    }
}
