using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace AirTraffic
{
    public class BigPlane : Airplane
    {

        float ToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        
        public BigPlane(GraphicsDevice graphicsDevice, Texture2D historySelected, Texture2D textureSelected, Texture2D textureNotSelected, Texture2D textureSelectedLights, Texture2D textureNotSelectedLights, string name, int x, int y)
           : base(graphicsDevice, historySelected, textureSelected, textureNotSelected, textureSelectedLights, textureNotSelectedLights, name, x, y)
        {

            TargetTurnSpeed = 0.1f;
            TargetSmallTurnSpeed = 0.03f;
            TurnSpeedAcceleration = 0.0003f;
            TurnSpeedDecceleration = 0.001f;
            Heading = 90;
            TargetHeading = 90;
        }


    }
}
