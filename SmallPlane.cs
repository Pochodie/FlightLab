using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AirTraffic
{
    public class SmallPlane: Airplane
    {
        
        public static int SmallPlaneCount { get; set; } = 22;
        public static SmallPlane smallPlane { get; set; }
        public SmallPlane(GraphicsDevice graphicsDevice, Texture2D historySelected, Texture2D textureSelected, Texture2D textureNotSelected, Texture2D textureSelectedLights, Texture2D textureNotSelectedLights, string name, int x, int y)
           : base(graphicsDevice, historySelected, textureSelected, textureNotSelected, textureSelectedLights, textureNotSelectedLights, name, x, y)
        {


            Heading = 90;
            TargetHeading = 90;
        }

    
    }
}
