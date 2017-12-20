using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GameElement
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        MouseState mouState, oldMState;
        KeyboardState keyState, oldKState;
        Texture2D texBlock, texCapsule, texCursor;
        Vector2 location, velocity;
        Color[,] shipBlockColorArray;
        float rotation, angVel;
        List<ShipBlock> blockList = new List<ShipBlock>();
        List<ClipReach> ReachClips = new List<ClipReach>();
        ClipCenter ShipClip = new ClipCenter();
        ShipBlock movingBlock;
        int screenWidth, screenHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = screenHeight = 800;
            graphics.PreferredBackBufferWidth = screenWidth = 1400;
            Window.Title = "Evaria:PreDev GameElement V0.0";
            IsMouseVisible = false;
        }
        protected override void Initialize()
        {
            camera = new Camera(GraphicsDevice.Viewport);
            oldKState = Keyboard.GetState();
            oldMState = Mouse.GetState();

            ClipReach ONE = new ClipReach();
            ClipReach TWO = new ClipReach();
            ClipReach THREE = new ClipReach();
            ONE.location = new Vector2(32, 0);
            TWO.location = new Vector2(-32, 0);
            THREE.location = new Vector2(0, 32);
            ReachClips.Add(ONE);
            ReachClips.Add(TWO);
            ReachClips.Add(THREE);
            ShipClip.location = new Vector2(0, 0);

            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texBlock = Content.Load<Texture2D>("ShipBlock");
            texCapsule = Content.Load<Texture2D>("Capsule");
            texCursor = Content.Load<Texture2D>("cursor");

            shipBlockColorArray = GetColorArray(texBlock);
        }
        protected override void UnloadContent()
        { }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            keyState = Keyboard.GetState();
            mouState = Mouse.GetState();
            BoxSelection();
            CharMovement();
            ShipClip.Update(location,rotation);
            if (mouState.LeftButton == ButtonState.Pressed && oldMState.LeftButton == ButtonState.Released)
            {
                Random rand = new Random();
                ShipBlock newBlock = new ShipBlock();
                newBlock.Initialise(location + Vector2.Transform(new Vector2(mouState.X - screenWidth / 2 - texBlock.Width / 2, mouState.Y - screenHeight / 2 - texBlock.Height / 2) / camera.Zoom, Matrix.CreateRotationZ(-camera.Rotation)),
                                        Vector2.Transform((float)rand.NextDouble() * Vector2.UnitY, Matrix.CreateRotationZ(2*(float)Math.PI * (float)rand.NextDouble())), 2 * (float)Math.PI * (float)rand.NextDouble(), 0.04f * (float)rand.NextDouble() - 0.02f, 4, Color.Purple);
                blockList.Add(newBlock);
            }
            foreach (ShipBlock block in blockList)
            {
                if (ReferenceEquals(block, movingBlock))
                {
                    block.SelectionAdjust(location + Vector2.Transform(new Vector2(mouState.X - screenWidth / 2 - texBlock.Width / 2, mouState.Y - screenHeight / 2 - texBlock.Height / 2) / camera.Zoom, Matrix.CreateRotationZ(-camera.Rotation)), rotation);
                    
                }
                block.Update(ShipClip);
            }
            foreach (ShipBlock block in blockList)
            {
                int collisionCount = 0;
                foreach (ShipBlock block2 in blockList)
                {
                    Vector2 MinTranslation = block.CollidesWith(block2);
                    if (MinTranslation != Vector2.Zero)
                    {
                        collisionCount++;
                        block2.ResolveCollision(block, MinTranslation);
                    }
                }
                if (collisionCount > 0)
                {
                    block.allowCollides = false;
                }
                else block.allowCollides = true;
            }
            if (keyState.IsKeyDown(Keys.Enter))
            { }
            oldKState = keyState;
            oldMState = mouState;
            camera.Update(location);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
            CameraDraw();
            spriteBatch.End();

            spriteBatch.Begin();
            StaticDraw();
            spriteBatch.End();
            base.Draw(gameTime);
        }


        private void CharMovement()
        {
            if (keyState.IsKeyDown(Keys.Left) && angVel > -0.05) { angVel += 0.005f; }
            if (keyState.IsKeyDown(Keys.Right) && angVel < 0.05) { angVel -= 0.005f; }
            rotation += 0.3f * angVel;
            angVel /= 1.05f;
            if (keyState.IsKeyDown(Keys.Up)) { velocity += 0.1f*Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(-rotation)); }
            if (keyState.IsKeyDown(Keys.Down)) { velocity -= 0.1f *Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(-rotation)); }
            if (velocity.Length() > 2) { velocity *= 2 / velocity.Length(); }
            location += velocity;
            velocity /= 1.05f;
            if (rotation < 0) rotation += (float)(2 * Math.PI);
            rotation %= (float)(2 * Math.PI);
        }
        private void BoxSelection()
        {
            if (mouState.RightButton == ButtonState.Pressed && oldMState.RightButton == ButtonState.Released)
            {
                foreach (ShipBlock block in blockList)
                {
                    if (block.isConnected == false)
                    {
                        if (movingBlock == null) { movingBlock = block; }
                        if (block.DistanceTo(location + Vector2.Transform(new Vector2(mouState.X - screenWidth / 2 - texBlock.Width / 2, mouState.Y - screenHeight / 2 - texBlock.Height / 2) / camera.Zoom, Matrix.CreateRotationZ(-camera.Rotation))).Length() <
                            movingBlock.DistanceTo(location + Vector2.Transform(new Vector2(mouState.X - screenWidth / 2 - texBlock.Width / 2, mouState.Y - screenHeight / 2 - texBlock.Height / 2) / camera.Zoom, Matrix.CreateRotationZ(-camera.Rotation))).Length())
                        {
                            movingBlock = block;
                        }
                    }
                }
            }
            if (mouState.RightButton == ButtonState.Released && oldMState.RightButton == ButtonState.Pressed)
            {
                foreach (ShipBlock block in blockList)
                {
                    if (ReferenceEquals(block,movingBlock))
                    {
                        foreach (ClipReach clip in ReachClips)
                        {
                            if (block.DistanceTo(clip.GetLocation(location, rotation)).Length() < 10 && clip.isInUse == false)
                            {
                                block.Connect(clip);
                                clip.isInUse = true;
                            }
                        }
                        
                    }
                }
            }
            if (mouState.RightButton == ButtonState.Released)
            {
                movingBlock = null;
            }
            
        }
        private void CameraDraw()
        {
            foreach(ShipBlock block in blockList) { block.Draw(spriteBatch, texBlock, texCursor); }
            foreach (ClipReach clip in ReachClips) { clip.Draw(spriteBatch, texCursor, location, rotation); }
            ShipClip.Draw(spriteBatch, texCursor, location, rotation);
            spriteBatch.Draw(texCapsule, location, null, Color.LimeGreen, -rotation, new Vector2(texCapsule.Width-16, texCapsule.Height-16),1f, SpriteEffects.None, 0);
        }
        private void StaticDraw()
        {
            spriteBatch.Draw(texCursor, new Rectangle(mouState.X - texCursor.Width / 2, mouState.Y - texCursor.Height / 2, texCursor.Width / 2, texCursor.Height / 2), Color.White);
        }
        private Color[,] GetColorArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }
}