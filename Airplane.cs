using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;


namespace AirTraffic
{
    public enum TurnDirection
    {
        Left,
        Right,
        None
    }

    public enum TurnKind
    {
        Normal,
        Small,
        None
    }

    public record PositionHist
    {
        public float X;
        public float Y;
        public float Opacity;
        public float Direction;
        public float Density;

        public PositionHist(float x, float y, float opacity, float direction, float density)
        {
            X = x;
            Y = y;
            Opacity = opacity;
            Direction = direction;
            Density = density;
        }
    }
    public abstract class Airplane

    {
        const int strobeLightsOnTime = 15;
        const int strobeLightsOffTime = 80;
        const float minimumTurnSpeed = 0.01f;
        const int startLevelOff = 28;


        public float Speed { get; set; }
        public TurnDirection TurnDirection { get; set; }

        float _heading;
        public float Heading
        {
            get
            {
                return _heading;
            }
            set
            {
                _heading = value % 360;
            }
        }

        public float TargetHeading;

        protected Texture2D TextureSelected;
        protected Texture2D TextureNotSelected;
        protected Texture2D TextureSelectedLights;
        protected Texture2D TextureNotSelectedLights;

        protected Texture2D TestTexture;

        public Vector2 Position;
        protected Vector2 Position2;
        protected Vector2 Origin;

        protected string Name;

        protected int Width;
        protected int Height;
        protected float Scale;

        protected float TurnSpeedAcceleration;
        protected float TurnSpeedDecceleration;
        protected float TargetTurnSpeed;
        protected float TargetSmallTurnSpeed;
        protected int TurnDelay;
        protected int Counter;
        float HeadingRadians => ToRadians(Heading);
        float HeadingRadiansCorrected => ToRadians(Heading - 90);
        float TurnSpeedIncrease;
        int TurnDelayCounter;
        bool StartTurn;
        bool EndTurn;
        bool LightsOn;
        bool IsTurning;
        bool Holding;
        int LightsCounter;
        TurnKind TurnKind;
        List<PositionHist> PositionHistory;
        List<PositionHist> PositionHistory2;

        public Airplane(GraphicsDevice graphicsDevice, Texture2D historyTexture, Texture2D textureSelected, Texture2D textureNotSelected, Texture2D textureSelectedLights, Texture2D textureNotSelectedLights, string name, int x, int y)
        {

            //TestTexture = new Texture2D(graphicsDevice, 1, 1);
            //TestTexture.SetData(new Color[] { Color.Red });

            TestTexture = historyTexture;
            TextureSelected = textureSelected;
            TextureNotSelected = textureNotSelected;
            TextureSelectedLights = textureSelectedLights;
            TextureNotSelectedLights = textureNotSelectedLights;
            Name = name;

            EndTurn = false;
            LightsCounter = 0;
            LightsOn = false;
            TurnDelayCounter = 0;
            TurnDelay = 200;
            TurnSpeedAcceleration = 0.0008f;
            TurnSpeedDecceleration = 0.001f;
            TurnDirection = TurnDirection.None;
            TargetTurnSpeed = 0.16f;
            Speed = 0.3f;
            Scale = 0.4f;
            Width = textureSelected.Width;
            Height = textureSelected.Height;
            Origin = new Vector2(Width / 2, Height / 2);
            Position = new Vector2(x, y);
            Position2 = new Vector2(x, y);
            PositionHistory = new List<PositionHist>();
            PositionHistory2 = new List<PositionHist>();
        }

        float ToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        public static TurnDirection GetTurnDirection(float sourceAngle, float otherAngle)
        {

            float difference = otherAngle - sourceAngle;

            if (difference < -180.0f)
                difference += 360.0f;
            if (difference > 180.0f)
                difference -= 360.0f;

            if (difference > 0.0f)
                return TurnDirection.Right;
            if (difference < 0.0f)
                return TurnDirection.Left;

            return TurnDirection.None;
        }

        public static float GetHeadingDifference(float currentHeading, float targetHeading)
        {
            float d = Math.Abs(targetHeading - currentHeading) % 360;

            if (d > 180)
                d = 360 - d;
            return d;
        }
        public void UseLights()
        {

            if (!LightsOn && LightsCounter == strobeLightsOffTime)
            {
                LightsOn = true;
                LightsCounter = 0;
            }

            if (LightsOn && LightsCounter == strobeLightsOnTime)
            {
                LightsOn = false;
                LightsCounter = 0;
            }

            LightsCounter++;
        }

        public void InitiateTurn()
        {
            StartTurn = true;
        }

        public void ResetTurn()
        {
            TurnKind = TurnKind.None;
            IsTurning = false;
            EndTurn = false;
            TurnDirection = TurnDirection.None;
            TurnSpeedIncrease = 0;
            TurnDelayCounter = 0;
        }

        public void Turn()
        {
            if (!StartTurn) return;

            if (GetHeadingDifference(Heading, TargetHeading) <= startLevelOff && TurnKind == TurnKind.None) TurnKind = TurnKind.Small;
            else if (TurnKind == TurnKind.None) TurnKind = TurnKind.Normal;



            if (TurnKind == TurnKind.Normal)
            {

                if (!Holding) TurnDirection = GetTurnDirection(Heading, TargetHeading);

                if (TurnDelayCounter == TurnDelay)
                {
                    if (Heading != TargetHeading)
                    {

                        if (TurnSpeedIncrease < TargetTurnSpeed && !EndTurn) TurnSpeedIncrease += TurnSpeedAcceleration;

                        if (TurnDirection == TurnDirection.Right)
                        {
                            Heading += TurnSpeedIncrease;


                            if (GetHeadingDifference(Heading, TargetHeading) < startLevelOff && Heading != TargetHeading)
                            {
                                if (TurnSpeedIncrease > minimumTurnSpeed) TurnSpeedIncrease -= TurnSpeedDecceleration;
                                EndTurn = true;

                            }

                            if (GetHeadingDifference(Heading, TargetHeading) < 0.1) Heading = TargetHeading;
                        }
                        else
                        {
                            Heading -= TurnSpeedIncrease;


                            if (GetHeadingDifference(Heading, TargetHeading) < startLevelOff && Heading != TargetHeading)
                            {
                                if (TurnSpeedIncrease > minimumTurnSpeed) TurnSpeedIncrease -= TurnSpeedDecceleration;
                                EndTurn = true;


                            }

                            if (GetHeadingDifference(Heading, TargetHeading) < 0.1) Heading = TargetHeading;
                        }
                    }
                    else
                    {
                        IsTurning = false;
                        EndTurn = false;
                        TurnKind = TurnKind.None;
                        TurnDirection = TurnDirection.None;
                        TurnSpeedIncrease = 0;
                        TurnDelayCounter = 0;
                    }
                }

                if (TurnDelayCounter < TurnDelay) TurnDelayCounter++;
            }

            if (TurnKind == TurnKind.Small)
            {
                if (!Holding) TurnDirection = GetTurnDirection(Heading, TargetHeading);

                if (TurnDelayCounter == TurnDelay)
                {
                    if (Heading != TargetHeading)
                    {



                        if (TurnDirection == TurnDirection.Right)
                        {
                            Heading += TargetSmallTurnSpeed;
                            if (GetHeadingDifference(Heading, TargetHeading) < 1) Heading = TargetHeading;
                        }
                        else
                        {
                            Heading -= TargetSmallTurnSpeed;
                            if (GetHeadingDifference(Heading, TargetHeading) < 1) Heading = TargetHeading;
                        }
                    }
                    else
                    {
                        IsTurning = false;
                        EndTurn = false;
                        TurnKind = TurnKind.None;
                        TurnDirection = TurnDirection.None;
                        TurnSpeedIncrease = 0;
                        TurnDelayCounter = 0;
                    }
                }
            }
            if (TurnDelayCounter < TurnDelay) TurnDelayCounter++;



        }


        public void Turn(bool ruenen)
        {
            if (StartTurn)
            {
                if (TurnDirection == TurnDirection.None) TurnDirection = GetTurnDirection(Heading, TargetHeading);


                if (TurnDelayCounter == TurnDelay)
                {
                    if (Heading != TargetHeading)
                    {

                        if (TurnSpeedIncrease < TargetTurnSpeed && !EndTurn) TurnSpeedIncrease += TurnSpeedAcceleration;

                        if (TurnDirection == TurnDirection.Right)
                        {
                            Heading += TurnSpeedIncrease;


                            if (GetHeadingDifference(Heading, TargetHeading) < startLevelOff && Heading != TargetHeading)
                            {
                                if (TurnSpeedIncrease > minimumTurnSpeed) TurnSpeedIncrease -= TurnSpeedDecceleration;
                                EndTurn = true;

                            }

                            if (GetHeadingDifference(Heading, TargetHeading) < 0.1) Heading = TargetHeading;
                        }
                        else
                        {
                            Heading -= TurnSpeedIncrease;
                            // Debug.WriteLine(GetHeadingDifference(Heading, TargetHeading));

                            if (GetHeadingDifference(Heading, TargetHeading) < startLevelOff && Heading != TargetHeading)
                            {
                                if (TurnSpeedIncrease > minimumTurnSpeed) TurnSpeedIncrease -= TurnSpeedDecceleration;
                                EndTurn = true;
                                Debug.WriteLine(EndTurn);

                            }

                            if (GetHeadingDifference(Heading, TargetHeading) < 0.1) Heading = TargetHeading;
                        }
                    }
                    else
                    {
                        StartTurn = false;
                        EndTurn = false;
                        TurnDirection = TurnDirection.None;
                        TurnSpeedIncrease = 0;
                        TurnDelayCounter = 0;
                    }
                }

                if (TurnDelayCounter < TurnDelay) TurnDelayCounter++;
            }
        }

        public void Move()
        {


            Vector2 direction = new Vector2((float)Math.Cos(HeadingRadiansCorrected), (float)Math.Sin(HeadingRadiansCorrected));
            Position += direction * Speed;




            if (Counter == 11)
            {
                PositionHistory.Add(new PositionHist(Position.X, Position.Y, 1f, HeadingRadiansCorrected, 0.14f));
                Counter = 0;
            }
            else Counter++;
            int ii = PositionHistory.Count;

            for (int i = 0; i < PositionHistory.Count; i++)
            {
                PositionHistory[i].Opacity -= 0.000003f * ii;

                PositionHistory[i].Density -= 0.0000003f * ii;
                if (PositionHistory[i].Density < 0f)
                {
                    PositionHistory.RemoveAt(i);
                    Debug.WriteLine("Removeing");
                }

                ii--;
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {



            foreach (PositionHist position in PositionHistory)
            {
                spriteBatch.Draw(TestTexture, new Vector2(position.X, position.Y), null, Color.White * position.Opacity, position.Direction, new Vector2(TestTexture.Width * position.Density - (100 * Scale), TestTexture.Height * position.Density - (270 * Scale)), position.Density, SpriteEffects.None, 0);
                spriteBatch.Draw(TestTexture, new Vector2(position.X, position.Y), null, Color.White * position.Opacity, position.Direction, new Vector2(TestTexture.Width * position.Density - (100 * Scale), TestTexture.Height * position.Density + (400 * Scale)), position.Density, SpriteEffects.None, 0);
                //  spriteBatch.Draw(TestTexture, new Vector2(position.X, position.Y), null, Color.White * position.Opacity, position.Direction, new Vector2(TestTexture.Width - 40, TestTexture.Height + 100), position.Density, SpriteEffects.None, 0);
                //  spriteBatch.Draw(TestTexture, new Vector2(position.X, position.Y), null, Color.White * position.Opacity, position.Direction, new Vector2(TestTexture.Width+100, TestTexture.Height-100), position.Opacity, SpriteEffects.None, 0);
                //spriteBatch.Draw(TestTexture, new Vector2(position.X, position.Y), null, Color.White * position.Opacity, position.Direction, new Vector2(400,-300), position.Density, SpriteEffects.None, 0);
                //   spriteBatch.Draw(TestTexture, new Vector2(position.X, position.Y), null, Color.White * position.Opacity, position.Direction, new Vector2(-400, -300), position.Density, SpriteEffects.None, 0);

            }

            // spriteBatch.Draw(TestTexture, new Vector2(0, 0), null, Color.White, 0, new Vector2(TestTexture.Width / 2, TestTexture.Height / 2), 1, SpriteEffects.None, 0);


            if (LightsOn) spriteBatch.Draw(TextureSelectedLights, Position, null, Color.White, HeadingRadians, Origin, Scale, SpriteEffects.None, 0);
            else spriteBatch.Draw(TextureSelected, Position, null, Color.White, HeadingRadians, Origin, Scale, SpriteEffects.None, 0);
            /*foreach (PositionHist position in PositionHistory2)
            {
                spriteBatch.Draw(TestTexture, new Vector2(position.X, position.Y), null, Color.White * position.Opacity, position.Direction, Vector2.Zero, 0.1f, SpriteEffects.None, 0);

            }*/
        }
    }
}
