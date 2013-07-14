
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    //a version of the quad that represents a floor tile that can be drawn to show the path finding routes
    class QuadTile
    {
        public Quad theQuad;
        public BasicEffect basicEffect;
        public Vector3 tilePosition;
        public float theAlpha = 1.0f;

        public QuadTile(Vector3 origin, Vector3 normal, Vector3 up,
           float width, float height, Vector3 position, GraphicsDevice theDevice)
        {
            theQuad = new Quad(Vector3.Zero, Vector3.Up, Vector3.Backward, width, height);
            basicEffect = new BasicEffect(theDevice);
            tilePosition = position;
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            basicEffect.World = Matrix.CreateTranslation(tilePosition);
            basicEffect.Alpha = theAlpha;
            basicEffect.VertexColorEnabled = true;
            basicEffect.View = view;
            basicEffect.Projection = projection;

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives
                    <VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    theQuad.Vertices,0,4,theQuad.Indexes,0,2);
            }
        }
    }
}
