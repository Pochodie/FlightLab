using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTraffic
{
    public class SmallPlane: Airplane
    {
        public SmallPlane(GraphicsDevice graphicsDevice, Texture2D historySelected, Texture2D textureSelected, Texture2D textureNotSelected, Texture2D textureSelectedLights, Texture2D textureNotSelectedLights, string name, int x, int y)
           : base(graphicsDevice, historySelected, textureSelected, textureNotSelected, textureSelectedLights, textureNotSelectedLights, name, x, y)
        {


            Heading = 90;
            TargetHeading = 90;
        }

    
    }
}
