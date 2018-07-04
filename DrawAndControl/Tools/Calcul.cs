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
    public static class Calcul
    {
        static public Vector2 RotateVectorByAngle(Vector2 v, float angleInRadian)
        {
            float sinTheta = (float)(Math.Sin((double)angleInRadian));
            float cosTheta = (float)(Math.Cos((double)angleInRadian));
            float x, y;
            x = cosTheta * v.X + sinTheta * v.Y;
            y = -sinTheta * v.X + cosTheta * v.Y;
            return new Vector2(x, y);
        }

        static public Vector2 GetVectorFromAngle(float angleInRadian)
        {
            return RotateVectorByAngle(new Vector2(0f, 1f), angleInRadian);
        }

        static public Vector2[] GetVectorArrayForCinematic(Vector2 pos1, Vector2 pos2, int frame)
        {
            Vector2 translation = new Vector2(pos2.X - pos1.X, pos2.Y - pos1.Y);
            float distance = translation.Length();

            float x;
            float[] tab = new float[frame + 1];
            for (int i = 0; i < tab.Length; i++)
            {
                x = (i * 4) / (float)frame - 2;
                tab[i] = (float)(Math.Exp(Math.Pow(x, 2) * -1) * 2.266597 / frame) * distance;
            }

            Vector2 transNormalized;

            if (pos1 != pos2) { transNormalized = Vector2.Normalize(translation); }
            else { transNormalized = Vector2.Zero; }

            Vector2[] tabVect = new Vector2[frame + 1];
            for (int j = 0; j < tab.Length; j++)
            {
                tabVect[j] = transNormalized * tab[j];
            }

            return tabVect;
        }
    }
}
