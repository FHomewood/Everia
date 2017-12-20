using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameElement
{
    class ClipCenter
    {
        public Vector2 location;
        public float rotation;
        public bool isInUse;
        public List<ShipBlock> connectionlist = new List<ShipBlock>();

        public Vector2 GetLocation(Vector2 conLoc, float conRot)
        {
            return conLoc + Vector2.Transform(location, Matrix.CreateRotationZ(-conRot));
        }
        public void Update(Vector2 loc, float rot)
        {
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D tex, Vector2 loc, float rot)
        {
            if (isInUse == false)
                spriteBatch.Draw(tex, GetLocation(loc, rot), null, Color.Blue, rot, new Vector2(tex.Width / 2, tex.Height / 2), 0.25f, SpriteEffects.None, 0);
        }
    }
}
