using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CarbonCube
{
    static public class DrawableRectangle
    {
        static private Dictionary<Color, Texture2D> Images = new Dictionary<Color, Texture2D>();

        static public void DrawCase(Case c, Color couleur)
        {
            AddCouleur(couleur);
            Rectangle rect = Camera.ComputePixelRectangleFromWorld(c.ToVector() + new Vector2(0.05f, 0.05f), new Vector2(0.9f, 0.9f), VectorOrigin.BottomLeftCorner);
            Game1.sSpriteBatch.Draw(Images[couleur], rect, Color.White);
        }

        static public void DrawCase(Case c, Color couleur1, Color couleur2)
        {
            Rectangle rect;
            AddCouleur(couleur1);
            AddCouleur(couleur2);

            rect = Camera.ComputePixelRectangleFromWorld(c.ToVector() + new Vector2(0.05f, 0.05f), new Vector2(0.9f, 0.9f), VectorOrigin.BottomLeftCorner);
            Game1.sSpriteBatch.Draw(Images[couleur1], rect, Color.White);

            rect = Camera.ComputePixelRectangleFromWorld(c.ToVector() + new Vector2(0.15f, 0.15f), new Vector2(0.7f, 0.7f), VectorOrigin.BottomLeftCorner);
            Game1.sSpriteBatch.Draw(Images[couleur2], rect, Color.White);

            rect = Camera.ComputePixelRectangleFromWorld(c.ToVector() + new Vector2(0.25f, 0.25f), new Vector2(0.5f, 0.5f), VectorOrigin.BottomLeftCorner);
            Game1.sSpriteBatch.Draw(Images[couleur1], rect, Color.White);
        }

        static public void DrawRectangle(Vector2 positionRelative, Vector2 tailleRelative, Color couleur)
        {
            AddCouleur(couleur);
            Rectangle rect = Camera.ComputePixelRectangleFromRelative(positionRelative, tailleRelative, VectorOrigin.BottomLeftCorner);
            Game1.sSpriteBatch.Draw(Images[couleur], rect, Color.White);
        }

        static private void AddCouleur(Color couleur)
        {
            if (!(Images.ContainsKey(couleur)))
            {
                Texture2D imageGénéré = new Texture2D(Game1.sGraphics.GraphicsDevice, 1, 1);
                imageGénéré.SetData<Color>(new Color[] { couleur });
                Images.Add(couleur, imageGénéré);
            }
        }
    }
}
