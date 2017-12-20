using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Everia
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Camera camHub;
        KeyboardState oldKState, keyState;
        MouseState oldMState, mouState;
        Texture2D texBackground, texChar, texWhite, texCursor, texCircleButton;
        public Vector2 location;
        Vector2 velocity;
        Color[,] ColArrBackground;
        bool flipChar, allowJump = true, platformDrop;
        public int screenWidth, screenHeight;
        int contCount, elevateFloor = 100;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = screenHeight = 800;
            graphics.PreferredBackBufferWidth = screenWidth = 1400;
            Window.Title = "Evaria:PreDev Hub V0.1";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            camHub = new Camera(GraphicsDevice.Viewport);
            oldKState = Keyboard.GetState();
            oldMState = Mouse.GetState();
            location = new Vector2(350, 1200);
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadTextures();
            LoadOthers();
        }
        protected override void UnloadContent()
        {

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            keyState = Keyboard.GetState();
            mouState = Mouse.GetState();

            if (elevateFloor == 100)
            {
                CharMovement();
                CollisionCheck();
            Vendors();
            }
            else velocity -= velocity;
            Elevate(elevateFloor);
            if (velocity.X > 0) flipChar = false;
            if (velocity.X < 0) flipChar = true;
            location += velocity;
            contCount++;

            oldKState = keyState;
            oldMState = mouState;

            camHub.Zoom = 2f;
            camHub.Update(location);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.GhostWhite);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,null,null, null, null,camHub.Transform);
            CamDraw();
            spriteBatch.End();



            spriteBatch.Begin();
            StatDraw();
            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void LoadTextures()
        {
            texBackground = Content.Load<Texture2D>("StationLayoutV0.0");
            texChar = Content.Load<Texture2D>("TempChar");
            texWhite = Content.Load<Texture2D>("WhiteSquare");
            texCursor = Content.Load<Texture2D>("cursor");
            texCircleButton = Content.Load<Texture2D>("CircleButton");
        }
        private void LoadOthers()
        {
            ColArrBackground = GetColorArray(texBackground);
            spriteFont = Content.Load<SpriteFont>("Font");
        }
        private void CharMovement()
        {
            if (keyState.IsKeyDown(Keys.Left)  && velocity.X >= -3) velocity.X--;
            if (keyState.IsKeyDown(Keys.Right) && velocity.X <=  3) velocity.X++;
            if (keyState.IsKeyDown(Keys.Up) && oldKState.IsKeyUp(Keys.Up) && allowJump)
            { velocity.Y = -8; allowJump = false; }
            if (keyState.IsKeyDown(Keys.Down) && oldKState.IsKeyUp(Keys.Down)) { platformDrop = true; }
            if (mouState.MiddleButton == ButtonState.Pressed && oldMState.MiddleButton == ButtonState.Released) { location += new Vector2(mouState.X - screenWidth / 2, mouState.Y - screenHeight / 2) / camHub.Zoom; }
            velocity.X -= velocity.X / 10;
            velocity.Y -= velocity.Y / 100;
            velocity.Y += 0.3f;
        }
        private void CollisionCheck()
        {
            try
            {
                for (int Bottom = -7; Bottom < 7; Bottom++)
                {
                    if (ColArrBackground[(int)(location.X + Bottom),
                                            (int)(location.Y + 24)] == Color.Black && velocity.Y > 0)
                    {velocity.Y = 0; allowJump = true; platformDrop = false; } else { }
                }
                for (int Bottom = -7; Bottom < 7; Bottom++)
                {
                    if (ColArrBackground[(int)(location.X + Bottom),
                                            (int)(location.Y + 24)] == Color.Yellow
                                            && velocity.Y > 0 && platformDrop == false)
                    { velocity.Y = 0; allowJump = true;
                        if(ColArrBackground[(int)(location.X + Bottom),
                                            (int)(location.Y + 23)] == Color.Yellow
                                            && velocity.Y > 0 && platformDrop == false)location.Y--; }
                }
                for (int Right = 17; Right < 22; Right++)
                {
                    if (ColArrBackground[   (int)(location.X + 7),
                                            (int)(location.Y + Right)] == Color.Black)
                    { location.Y--;}
                }
                for (int Left = 17; Left < 22; Left++)
                {
                    if (ColArrBackground[   (int)(location.X - 7),
                                            (int)(location.Y + Left)] == Color.Black)
                    { location.Y--; }
                }
                for (int Top = -7; Top < 7; Top++)
                {
                    if (ColArrBackground[(int)(location.X + Top),
                                            (int)(location.Y - 23)] == Color.Black && velocity.Y < 0)
                    {velocity.Y = 0; }
                }
                for (int RightBlock = -21; RightBlock < 16; RightBlock++)
                {
                    if (ColArrBackground[(int)(location.X + 11),
                                            (int)(location.Y + RightBlock)] == Color.Black && velocity.X > 0)
                    {velocity.X = 0; }
                }
                for (int LeftBlock = -21; LeftBlock < 16; LeftBlock++)
                {
                    if (ColArrBackground[(int)(location.X - 11),
                                            (int)(location.Y + LeftBlock)] == Color.Black && velocity.X < 0)
                    { velocity.X = 0; }
                }
            }
            catch{ }

        }
        private void Vendors()
        {

            if (new Rectangle(1764, 1031, 345, 175).Contains(location)) //Shop Rectangle  
            { }
            if (new Rectangle(1912, 627, 520, 290).Contains(location)) //Refinery Rectangle
            { }
            if (new Rectangle(853, 768, 341, 149).Contains(location)) //Laboratory Rectangle
            { }
            if (new Rectangle(815, 464, 366, 183).Contains(location)) //Workshop Rectangle
            { }
            if (new Rectangle(1439, 1411, 273, 146).Contains(location)) //Storage Rectangle
            { }
            if (new Rectangle(1748, 1298, 129, 115).Contains(location)) //Backstreet Rectangle
            { }
            if (new Rectangle(426, 1168, 94, 94).Contains(location)) //HangarLeft Rectangle
            { }
            if (new Rectangle(604, 1168, 94, 94).Contains(location)) //HangarCentre Rectangle
            { }
            if (new Rectangle(809, 1168, 94, 94).Contains(location)) //HangarRight Rectangle
            { }
            if (new Rectangle(271, 1124, 87, 138).Contains(location)) //Airlock Rectangle
            { }
            if (new Rectangle(1425, 1106, 254, 100).Contains(location)) //GroundFloor Rectangle
            {
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.25f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 2;
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.35f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 1;
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.45f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 0;
            }
            if (new Rectangle(1425, 817, 254, 100).Contains(location)) //FirstFloor Rectangle
            {
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.25f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 2;
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.35f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 1;
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.45f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 0;
            }
            if (new Rectangle(1425, 404, 254, 100).Contains(location)) //SecondFloor Rectangle
            {
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.25f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 2;
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.35f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 1;
                if (new Vector2(mouState.X - screenWidth * 0.1f, mouState.Y - screenHeight * 0.45f).Length() < 75 && mouState.LeftButton == ButtonState.Pressed) elevateFloor = 0;
            }
        }
        private void Elevate(int floor)
        {
            if (floor < 100)
            {
                int elevation = 0;
                if (floor == 0) elevation = 1106 + 76;
                if (floor == 1) elevation = 817 + 76;
                if (floor == 2) elevation = 404 + 76;
                if (elevation == (int)location.Y) elevateFloor = 100;
                try
                {
                    location.Y += (elevation - (int)location.Y) / Math.Abs(elevation - (int)location.Y);
                }
                catch { }
            }
        }
        private void CamDraw()
        {
            spriteBatch.Draw(texBackground, new Rectangle(0, 0, texBackground.Width, texBackground.Height), Color.White);
            if (flipChar)
                spriteBatch.Draw(texChar, new Rectangle((int)location.X, (int)location.Y, 20, 47), null, Color.White, 0.0f,
                                    new Vector2(texChar.Width / 2, texChar.Height / 2), SpriteEffects.FlipHorizontally, 0);
            else
                spriteBatch.Draw(texChar, new Rectangle((int)location.X, (int)location.Y, 20, 47), null, Color.White, 0.0f,
                                    new Vector2(texChar.Width / 2, texChar.Height / 2), SpriteEffects.None, 0);
        }
        private void StatDraw()
        {
            DrawMenus();
            if (mouState.LeftButton == ButtonState.Pressed || mouState.RightButton == ButtonState.Pressed)
                spriteBatch.Draw(texCursor, new Rectangle(mouState.X, mouState.Y, (int)(0.5 * texCursor.Width), (int)(0.5 * texCursor.Height)), null, Color.Violet, 0f, new Vector2(texCursor.Width / 2, texCursor.Height / 2), SpriteEffects.None, 0);
            else
                spriteBatch.Draw(texCursor, new Rectangle(mouState.X, mouState.Y, (int)(0.6 * texCursor.Width), (int)(0.6 * texCursor.Height)), null, Color.White, 0f, new Vector2(texCursor.Width / 2, texCursor.Height / 2), SpriteEffects.None, 0);
        }
        private void DrawMenus()
        {

            if (new Rectangle(1764, 1031, 345, 175) .Contains(location)) //Shop Rectangle  
            { }
            if (new Rectangle(1912, 627, 520, 290)  .Contains(location)) //Refinery Rectangle
            { }
            if (new Rectangle(853, 768, 341, 149)   .Contains(location)) //Laboratory Rectangle
            { }
            if (new Rectangle(815, 464, 366, 183)   .Contains(location)) //Workshop Rectangle
            { }
            if (new Rectangle(1439, 1411, 273, 146) .Contains(location)) //Storage Rectangle
            { }
            if (new Rectangle(1748, 1298, 129, 115) .Contains(location)) //Backstreet Rectangle
            { }
            if (new Rectangle(426, 1168, 94, 94)    .Contains(location)) //HangarLeft Rectangle
            { }
            if (new Rectangle(604, 1168, 94, 94)    .Contains(location)) //HangarCentre Rectangle
            { }
            if (new Rectangle(809, 1168, 94, 94)    .Contains(location)) //HangarRight Rectangle
            { }
            if (new Rectangle(271, 1124, 87, 138)   .Contains(location)) //Airlock Rectangle
            { }
            if (new Rectangle(1425, 1106, 254, 100) .Contains(location)) //GroundFloor Rectangle
            {
                spriteBatch.Draw(texWhite, new Rectangle((int)(screenWidth * 0.05), (int)(screenHeight * 0.18), 150, 280), Color.Gray);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.25), 75, 75), null, Color.White,       0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.35), 75, 75), null, Color.White,       0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.45), 75, 75), null, Color.SlateGray,   0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "2", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("2").X / 2, (int)(screenHeight * 0.25) - spriteFont.MeasureString("2").Y / 2), Color.DarkGray);
                spriteBatch.DrawString(spriteFont, "1", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("1").X / 2, (int)(screenHeight * 0.35) - spriteFont.MeasureString("1").Y / 2), Color.DarkGray);
                spriteBatch.DrawString(spriteFont, "G", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("G").X / 2, (int)(screenHeight * 0.45) - spriteFont.MeasureString("G").Y / 2), Color.DarkGray);
            }
            if (new Rectangle(1425, 817, 254, 100)  .Contains(location)) //FirstFloor Rectangle
            {
                spriteBatch.Draw(texWhite, new Rectangle((int)(screenWidth * 0.05), (int)(screenHeight * 0.18), 150, 280), Color.Gray);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.25), 75, 75), null, Color.White,       0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.35), 75, 75), null, Color.SlateGray,   0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.45), 75, 75), null, Color.White,       0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "2", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("2").X / 2, (int)(screenHeight * 0.25) - spriteFont.MeasureString("2").Y / 2), Color.DarkGray);
                spriteBatch.DrawString(spriteFont, "1", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("1").X / 2, (int)(screenHeight * 0.35) - spriteFont.MeasureString("1").Y / 2), Color.DarkGray);
                spriteBatch.DrawString(spriteFont, "G", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("G").X / 2, (int)(screenHeight * 0.45) - spriteFont.MeasureString("G").Y / 2), Color.DarkGray);
            }
            if (new Rectangle(1425, 404, 254, 100)  .Contains(location)) //SecondFloor Rectangle
            {
                spriteBatch.Draw(texWhite, new Rectangle((int)(screenWidth * 0.05), (int)(screenHeight * 0.18), 150, 280), Color.Gray);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.25), 75, 75), null, Color.SlateGray,   0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.35), 75, 75), null, Color.White,       0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.Draw(texCircleButton, new Rectangle((int)(screenWidth * 0.1), (int)(screenHeight * 0.45), 75, 75), null, Color.White,       0, new Vector2(206, 206), SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "2", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("2").X / 2, (int)(screenHeight * 0.25) - spriteFont.MeasureString("2").Y / 2), Color.DarkGray);
                spriteBatch.DrawString(spriteFont, "1", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("1").X / 2, (int)(screenHeight * 0.35) - spriteFont.MeasureString("1").Y / 2), Color.DarkGray);
                spriteBatch.DrawString(spriteFont, "G", new Vector2((int)(screenWidth * 0.1) - spriteFont.MeasureString("G").X / 2, (int)(screenHeight * 0.45) - spriteFont.MeasureString("G").Y / 2), Color.DarkGray);
            }
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