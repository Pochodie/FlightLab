using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace AirTraffic
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int PreviousScrollWheelValue = 0;
        private int ScrollWheelValue = 0;
        Texture2D _smoke;

        BigPlane _bigPlane;
        SmallPlane _smallPlane;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _smoke = Content.Load<Texture2D>("Graphics/smoke2");
            _bigPlane = new BigPlane(GraphicsDevice, _smoke, Content.Load<Texture2D>("Graphics/737_normal"), Content.Load<Texture2D>("Graphics/737_normal"),
                Content.Load<Texture2D>("Graphics/737_strobe"), Content.Load<Texture2D>("Graphics/737_strobe"), "A350", 200, 400);
            _smallPlane = new SmallPlane(GraphicsDevice, _smoke, Content.Load<Texture2D>("Graphics/plane_selected"), Content.Load<Texture2D>("Graphics/plane_not_selected"),
                Content.Load<Texture2D>("Graphics/plane_selected_lights"), Content.Load<Texture2D>("Graphics/plane_not_selected_lights"), "A350", 200, 600);


        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (mouseState.ScrollWheelValue != PreviousScrollWheelValue)
            {
                if (PreviousScrollWheelValue < mouseState.ScrollWheelValue) ScrollWheelValue += 5;
                else ScrollWheelValue -= 5;
                if (ScrollWheelValue < 0) ScrollWheelValue += 360;
                if (ScrollWheelValue > 359) ScrollWheelValue -= 360;
                PreviousScrollWheelValue = Mouse.GetState().ScrollWheelValue;
                Debug.WriteLine(ScrollWheelValue);
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _bigPlane.TargetHeading = ScrollWheelValue;
                _bigPlane.ResetTurn();
                _bigPlane.InitiateTurn();

            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _bigPlane.TargetHeading = 280.22f;
                //_bigPlane.TurnDirection = TurnDirection.Right;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _bigPlane.TargetHeading = 2.22f;
                //_bigPlane.TurnDirection = TurnDirection.Left;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _bigPlane.Position = new Vector2(500, 500);

            }

            _bigPlane.Turn();
            _bigPlane.Move();
            _bigPlane.UseLights();


            //Debug.WriteLine($"{_bigPlane.GetHeading()} {_bigPlane.TargetHeading}");


        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);

            _spriteBatch.Begin();

            _bigPlane.Draw(_spriteBatch);


            _spriteBatch.End();



        }
    }
}
