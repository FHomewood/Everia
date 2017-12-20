using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameElement
{
    class ShipBlock
    {
        public Vector2 location, velocity, connectedLoc, UL, UR, LL, LR;
        public Color color;
        float rotation, angVelocity;
        int mass, blockSize = 1;
        public bool isConnected;
        public bool allowCollides = true;
        public ClipCenter Cclip;
        public List<ClipReach> Rclips = new List<ClipReach>();

        public void Initialise(Vector2 spawnLoc, Vector2 spawnVel, float spawnRot, float spawnAngVel, int spawnMass, Color spawnCol)
        {
            isConnected = false;
            location = spawnLoc;
            velocity = spawnVel;
            rotation = spawnRot;
            angVelocity = spawnAngVel;
            mass = spawnMass;
            color = spawnCol;
            for (int i = 0; i < 4; i++)
            {
                ClipReach Rclip = new ClipReach();
                Rclip.location = Vector2.Transform(new Vector2(0, 32), Matrix.CreateRotationZ((float)(i * Math.PI / 2)));
                Rclip.isInUse = false;
                Rclips.Add(Rclip);
            }
            Cclip = new ClipCenter();

        }
        public void Update(ClipCenter Clip)
        {
            if (isConnected)
            {
                rotation = Clip.rotation;
                location = Clip.location + Vector2.Transform(connectedLoc, Matrix.CreateRotationZ(-Clip.rotation));
            }
            else
            {

                location += velocity;
                rotation += angVelocity;
                if (rotation < 0) rotation += (float)(2 * Math.PI);
                rotation %= (float)(2 * Math.PI);
                UL = new Vector2((float)(location.X - 22.63 * Math.Cos(Math.PI / 4 - rotation)), (float)(location.Y - 22.63 * Math.Sin(Math.PI / 4 - rotation)));
                UR = new Vector2((float)(location.X + 22.63 * Math.Sin(Math.PI / 4 - rotation)), (float)(location.Y - 22.63 * Math.Cos(Math.PI / 4 - rotation)));
                LL = new Vector2((float)(location.X - 22.63 * Math.Sin(Math.PI / 4 - rotation)), (float)(location.Y + 22.63 * Math.Cos(Math.PI / 4 - rotation)));
                LR = new Vector2((float)(location.X + 22.63 * Math.Cos(Math.PI / 4 - rotation)), (float)(location.Y + 22.63 * Math.Sin(Math.PI / 4 - rotation)));
            }
        }
        public void Draw(SpriteBatch spritebatch, Texture2D texBox, Texture2D texClip)
        {
            spritebatch.Draw(texBox, new Rectangle((int)location.X, (int)location.Y, 32, 32), null, color, -rotation, new Vector2(texBox.Width / 2, texBox.Height / 2), SpriteEffects.None, 0);
            foreach (ClipReach Rclip in Rclips) { Rclip.Draw(spritebatch, texClip, location, rotation); }
            Cclip.Draw(spritebatch, texClip, location, rotation);
            if (isConnected == false)
            {
                spritebatch.Draw(texBox, new Rectangle((int)UL.X - 4, (int)UL.Y - 4, 8, 8), null, Color.Red);
                spritebatch.Draw(texBox, new Rectangle((int)UR.X - 4, (int)UR.Y - 4, 8, 8), null, Color.Green);
                spritebatch.Draw(texBox, new Rectangle((int)LL.X - 4, (int)LL.Y - 4, 8, 8), null, Color.Gray);
                spritebatch.Draw(texBox, new Rectangle((int)LR.X - 4, (int)LR.Y - 4, 8, 8), null, Color.White);
            }
        }

        public void Connect(ClipReach clip)
        {
            isConnected = true;
            connectedLoc = clip.location;
            location = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            rotation = clip.rotation;
            angVelocity = 0;
        }
        public void CheckClips(List<ShipBlock> List)
        {
            foreach (ShipBlock block in List)
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((Cclip.location - block.Rclips[i].location).Length() < 10)
                    {
                        Connect(block.Rclips[i]);
                    }
                }
            }
        }
        public void ResolveCollision(ShipBlock B, Vector2 MinTranslation)
        {
            location += MinTranslation / 2;
            B.location -= MinTranslation / 2;
            Vector2 oldvelocity = velocity;
            float oldangVelocity = angVelocity;
            velocity = new Vector2(((mass - B.mass) * velocity.X + 2 * B.mass * B.velocity.X) / (mass + B.mass), ((mass - B.mass) * velocity.Y + 2 * B.mass * B.velocity.Y) / (mass + B.mass));
            B.velocity = new Vector2(((B.mass - mass) * B.velocity.X + 2 * mass * oldvelocity.X) / (B.mass + mass), ((B.mass - mass) * B.velocity.Y + 2 * mass * oldvelocity.Y) / (B.mass + mass));
            angVelocity = ((mass - B.mass) * angVelocity + 2 * B.mass * B.angVelocity) / (mass + B.mass);
            B.angVelocity = ((B.mass - mass) * B.angVelocity + 2 * mass * oldangVelocity) / (B.mass + mass);
        }
        public Vector2 CollidesWith(ShipBlock B)
        {
            if (ShipBlock.ReferenceEquals(B, this) || B.allowCollides == false) { return Vector2.Zero; }
            if ((location - B.location).Length() > 64) { return Vector2.Zero; }
            Vector2 overlap = new Vector2(0, 40);
            Vector2[] axis = new Vector2[4];
            axis[0] = (UR - UL) / (UR - UL).Length();
            axis[1] = (UR - LR) / (UR - LR).Length();
            axis[2] = (B.UL - B.LL) / (B.UL - B.LL).Length();
            axis[3] = (B.UL - B.UR) / (B.UL - B.UR).Length();
            for (int i = 0; i < axis.Length; i++)
            {
                float projAUR = UR.X * axis[i].X + UR.Y * axis[i].Y;
                float projAUL = UL.X * axis[i].X + UL.Y * axis[i].Y;
                float projALR = LR.X * axis[i].X + LR.Y * axis[i].Y;
                float projALL = LL.X * axis[i].X + LL.Y * axis[i].Y;
                float projBUR = B.UR.X * axis[i].X + B.UR.Y * axis[i].Y;
                float projBUL = B.UL.X * axis[i].X + B.UL.Y * axis[i].Y;
                float projBLR = B.LR.X * axis[i].X + B.LR.Y * axis[i].Y;
                float projBLL = B.LL.X * axis[i].X + B.LL.Y * axis[i].Y;
                if (Minimum(projBLL, projBLR, projBUL, projBUR) > Maximum(projALL, projALR, projAUL, projAUR)) { return Vector2.Zero; }
                if (Math.Abs(Minimum(projBLL, projBLR, projBUL, projBUR) - Maximum(projALL, projALR, projAUL, projAUR)) < overlap.Length())
                {
                    overlap = axis[i] * (Minimum(projBLL, projBLR, projBUL, projBUR) - Maximum(projALL, projALR, projAUL, projAUR));
                }
                if (Minimum(projALL, projALR, projAUL, projAUR) > Maximum(projBLL, projBLR, projBUL, projBUR)) { return Vector2.Zero; }
                if (Math.Abs(Minimum(projALL, projALR, projAUL, projAUR) - Maximum(projBLL, projBLR, projBUL, projBUR)) < overlap.Length())
                {
                    overlap = axis[i] * (Minimum(projALL, projALR, projAUL, projAUR) - Maximum(projBLL, projBLR, projBUL, projBUR));
                }
            }
            return overlap;
        }
        public Vector2 CollidesWith(Vector2 ur, Vector2 ul, Vector2 lr, Vector2 ll)
        {
            Vector2 overlap = new Vector2(0, 40);
            Vector2[] axis = new Vector2[4];
            axis[0] = (UR - UL) / (UR - UL).Length();
            axis[1] = (UR - LR) / (UR - LR).Length();
            axis[2] = (ul - ll) / (ul - ll).Length();
            axis[3] = (ul - ur) / (ul - ur).Length();
            for (int i = 0; i < axis.Length; i++)
            {
                float projAUR = UR.X * axis[i].X + UR.Y * axis[i].Y;
                float projAUL = UL.X * axis[i].X + UL.Y * axis[i].Y;
                float projALR = LR.X * axis[i].X + LR.Y * axis[i].Y;
                float projALL = LL.X * axis[i].X + LL.Y * axis[i].Y;
                float projBUR = ur.X * axis[i].X + ur.Y * axis[i].Y;
                float projBUL = ul.X * axis[i].X + ul.Y * axis[i].Y;
                float projBLR = lr.X * axis[i].X + lr.Y * axis[i].Y;
                float projBLL = ll.X * axis[i].X + ll.Y * axis[i].Y;
                if (Minimum(projBLL, projBLR, projBUL, projBUR) > Maximum(projALL, projALR, projAUL, projAUR)) { return Vector2.Zero; }
                if (Math.Abs(Minimum(projBLL, projBLR, projBUL, projBUR) - Maximum(projALL, projALR, projAUL, projAUR)) < overlap.Length())
                {
                    overlap = axis[i] * (Minimum(projBLL, projBLR, projBUL, projBUR) - Maximum(projALL, projALR, projAUL, projAUR));
                }
                if (Minimum(projALL, projALR, projAUL, projAUR) > Maximum(projBLL, projBLR, projBUL, projBUR)) { return Vector2.Zero; }
                if (Math.Abs(Minimum(projALL, projALR, projAUL, projAUR) - Maximum(projBLL, projBLR, projBUL, projBUR)) < overlap.Length())
                {
                    overlap = axis[i] * (Minimum(projALL, projALR, projAUL, projAUR) - Maximum(projBLL, projBLR, projBUL, projBUR));
                }
            }
            return overlap;
        }
        public Vector2 DistanceTo(Vector2 Point)
        {
            return (Point - location);
        }
        public void SelectionAdjust(Vector2 Point, float shipRot)
        {
            if ((Point - location).Length() > 0.5)
                velocity = 0.2f * (Point - location) / (float)Math.Sqrt((Point - location).Length());
            else velocity = Vector2.Zero;
            if (Math.Abs((shipRot - rotation) % (Math.PI / 2)) < 0.05)
                angVelocity = 0;
            else if (((shipRot - rotation) % (Math.PI / 2)) - Math.PI / 4 > 0.00)
                angVelocity = 0.02f;
            else angVelocity = -0.02f;
        }
        private float Maximum(float One, float Two, float Three, float Four)
            { return Math.Max(Math.Max(One, Two), Math.Max(Three, Four)); }
        private float Minimum(float One, float Two, float Three, float Four)
            { return Math.Min(Math.Min(One, Two), Math.Min(Three, Four)); }
    }
}