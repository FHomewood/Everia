using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameElement
{
    class ClipReach
    {
        public Vector2 location;
        public float rotation;
        public bool isInUse;

        public Vector2 GetLocation(Vector2 shipLoc, float shipRot)
        {
            return shipLoc + Vector2.Transform(location,Matrix.CreateRotationZ(-shipRot));
        }
        public void Update()
        {
            
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D tex, Vector2 loc, float rot)
        {
            if (isInUse == false)
                spriteBatch.Draw(tex, GetLocation(loc, rot), null, Color.FromNonPremultiplied(255, 255, 0, 50), rot, new Vector2(tex.Width / 2, tex.Height / 2), 0.25f, SpriteEffects.None, 0);
        }
    }

}