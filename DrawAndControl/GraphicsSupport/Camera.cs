using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace CarbonCube
{
    /// <summary>
    /// The camera class: a static class, meant to only allow one camera.
    /// </summary>
    static public class Camera
    {
        static private Vector2 sPosition = Vector2.Zero;     // Coordonnée dans le monde du centre de la caméra
        static private float sWidth;                        // Largeur de la camera
        static private float sRatio;                       // Ratio between camera window and pixel
        static private float sVitesse = 1f;

        static private float sMaxWidth = 0f;

        static public Vector2 Position
        {
            get { return sPosition; }
            set { sPosition = value; }
        }
        static public float Width
        {
            get { return sWidth; }
            set
            {
                if (sMaxWidth != 0f && value > sMaxWidth)
                    sWidth = sMaxWidth;
                else
                    sWidth = value;
                sRatio = (float)Game1.sGraphics.PreferredBackBufferWidth / sWidth;
            }
        }
        static public float Height
        {
            get { return ((float)(Game1.sGraphics.PreferredBackBufferHeight) / Game1.sGraphics.PreferredBackBufferWidth) * Width; }
            set
            {
                Width = (value * Game1.sGraphics.PreferredBackBufferWidth) / Game1.sGraphics.PreferredBackBufferHeight;
            }
        }

        static private Vector2 ScreenCenter = new Vector2(Game1.sGraphics.PreferredBackBufferWidth / 2, Game1.sGraphics.PreferredBackBufferHeight / 2);

        static private Vector2[] sCinematicArray;
        static private int? sCinematicIndex = null;


        static public void SetCameraWindow(float map_width, float map_height)
        {
            float marge = 1.3f;

            float width_ratio = Game1.sGraphics.PreferredBackBufferWidth / map_width;
            float height_ratio = Game1.sGraphics.PreferredBackBufferHeight / map_height;

            if (height_ratio < width_ratio)
            {
                Height = map_height * marge;
            }
            else
            {
                Width = map_width * marge;
            }
            sMaxWidth = Width;
        }

        static public void ComputePixelPosition(Vector2 objectPositionInWorld, out int x, out int y)
        {
            float ratio = sRatio;

            // Convert the position to pixel space
            x = (int)(((objectPositionInWorld.X - Camera.Position.X + Width/2) * ratio) + 0.5f);
            y = (int)(((objectPositionInWorld.Y - Camera.Position.Y + Height/2) * ratio) + 0.5f);

            y = Game1.sGraphics.PreferredBackBufferHeight - y;
        }

        static public Rectangle ComputePixelRectangleFromWorld(Vector2 position, Vector2 size, Vector2 relativeOrigin)
        {
            float ratio = sRatio;

            // Convert size from Camera Window Space to pixel space
            int width = (int)((size.X * ratio) + 0.5f);
            int height = (int)((size.Y * ratio) + 0.5f);

            // Convert the position to pixel space
            int x, y;
            ComputePixelPosition(position, out x, out y);

            // Reference position is the center
            y -= (int)(height * relativeOrigin.Y);
            x -= (int)(width * relativeOrigin.X);

            return new Rectangle(x, y, width, height);
        }

        static public Rectangle ComputePixelRectangleFromRelative(Vector2 relativePixelPosition, Vector2 relativePixelSize, Vector2 relativeOrigin)
        {
            float ratio = sRatio;

            int width = (int)(Game1.sGraphics.PreferredBackBufferWidth * relativePixelSize.X);
            int height = (int)(Game1.sGraphics.PreferredBackBufferHeight * relativePixelSize.Y);

            int x = (int)(Game1.sGraphics.PreferredBackBufferWidth * relativePixelPosition.X);
            int y = (int)(Game1.sGraphics.PreferredBackBufferHeight * (1 - relativePixelPosition.Y));

            x -= (int)(width * relativeOrigin.X);
            y -= (int)(height * relativeOrigin.Y);

            return new Rectangle(x, y, width, height);
        }

        static public void Update(Vector2 dirDeplacement)
        {
            if (sCinematicIndex == null)
            {
                if (dirDeplacement != Vector2.Zero)
                {
                    Position += Vector2.Normalize(dirDeplacement) * (Camera.sVitesse * Width / 100f);
                }

                if (ScrollWheel.difference > 0) { Width /= 1.1f; }
                if (ScrollWheel.difference < 0) { Width *= 1.1f; }
            }
            else
            {
                if (sCinematicIndex < sCinematicArray.Length)
                {
                    Position += sCinematicArray[(int)sCinematicIndex];
                    sCinematicIndex++;
                }
                else
                {
                    sCinematicIndex = null;
                }
            }
        }

        static public Vector2 ComputeWorldPosition(int x_pixel, int y_pixel)
        {
            float ratio = sRatio;

            Vector2 objectPositionInWorld = Vector2.Zero;

            y_pixel = Game1.sGraphics.PreferredBackBufferHeight - y_pixel;
            objectPositionInWorld.X = (x_pixel - 0.5f) / ratio - Width / 2 + Camera.Position.X;
            objectPositionInWorld.Y = (y_pixel - 0.5f) / ratio - Height / 2 + Camera.Position.Y;

            return objectPositionInWorld;
        }

        static public void MakeCinematic(Vector2 pos1, Vector2 pos2, int frame)
        {
            sCinematicArray = Calcul.GetVectorArrayForCinematic(pos1, pos2, frame);
            sCinematicIndex = 0;
            Position = pos1;
        }
    }

    static public class VectorOrigin
    {
        static private Vector2 mTopLeftCorner = Vector2.Zero;
        static private Vector2 mBottomLeftCorner = new Vector2(0f,1f);
        static private Vector2 mCenter = new Vector2(0.5f, 0.5f);

        static public Vector2 TopLeftCorner
        {
            get { return mTopLeftCorner; }
        }

        static public Vector2 BottomLeftCorner
        {
            get { return mBottomLeftCorner; }
        }

        static public Vector2 Center
        {
            get { return mCenter; }
        }
    }
}
