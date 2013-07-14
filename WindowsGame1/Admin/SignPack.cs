using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
////using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    //stores signs used for the Card game, includes card images and videos, 
    public class SignPack
    {
        public int noOfSigns; //how many signs are in the pack

        public String classFile; //name of classifier file for black box
        public String nameFile; //name of the file which stores the names of each sign in the signpack, used for the black box
        public String packName;

        List<int> signOrder = new List<int>();  //Override the default sign pack order
         List<int>       cardOrder = new List<int>(); //Override the default cards order (normally based on alphabetical order of the images)
         List<Video>     videos    = new List<Video>(); //videos for cards
         List<Texture2D> photos    = new List<Texture2D>(); //pictures for cards
         List<Texture2D> diagrams  = new List<Texture2D>(); //diagrams for cards

         public SignPack(
             int noOfSigns,
             String classFile,
             String nameFile,
             List<int> signOrder,
             List<int> cardOrder,
             List<Video> videos,
             List<Texture2D>photos,
             List<Texture2D>diagrams,
             String packName
             )
         {
             this.noOfSigns = noOfSigns;
             this.classFile = classFile;
             this.nameFile = nameFile;
             this.signOrder = signOrder;
             this.cardOrder = cardOrder;
             this.videos = videos;
             this.photos = photos;
             this.diagrams = diagrams;
             this.packName = packName;
         }

        
         public Video GetVideo(int vidNum)
         {
             if (vidNum <= videos.Count)
             {
                 int test = vidNum;
                 test = cardOrder[test];
                 return videos[test];
             }
             return null;
         }

         public Texture2D GetPhotos(int photoNum)
         {
             if (photoNum <= photos.Count)
             {
                 return photos[cardOrder[photoNum]];
             }
             return null;
         }

         public Texture2D GetDiagram(int diagNum)
         {
             if (diagNum <= diagrams.Count)
             {
                 return diagrams[cardOrder[diagNum]];
             }
             return null;
         }

         public int GetSignNum(int signNum)
         {
             if (signNum <= signOrder.Count && signNum >= 0)
             {
                 return signOrder[signNum];
             }
             return -1;
         }
    }
}
