using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GeoSketch;

namespace GeoSketch2023
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            //_spriteBatch.DrawArc(400, 200, 100, Color.Red, Color.Blue, 5, 0, MathHelper.PiOver2);

            _spriteBatch.DrawLine(100, 96, 200, 96, Color.Red, 8);
            //_spriteBatch.DrawLine(92, 100, 92, 200, Color.Red, 8);
            //_spriteBatch.DrawRectangle(100, 104, 100, 104, Color.RoyalBlue, Color.RoyalBlue, 0);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}