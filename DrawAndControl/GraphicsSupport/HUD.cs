using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace CarbonCube
{
    static public class HUD
    {
        static private GameManager GM;
        static private string mMessagePermanent = " ";
        static private Color mColorMsgPermanent = Color.Black;
        static private string mMessageTmp = " ";
        static private int mMessageTmpDisplayTime = 250;
        static private int mMessageTmpFrame = 0;

        static public string MessageTemporaire{
            get
            {
                return mMessageTmp;
            }
            set
            {
                mMessageTmpFrame = 0;
                mMessageTmp = value;
            }
        }

        static public void SetGameManager(GameManager gameManager)
        {
            GM = gameManager;
        }

        static public void SetMessagePermanent(string msg, Color color)
        {
            mMessagePermanent = msg;
            mColorMsgPermanent = color;
        }

        static public void Draw()
        {
            if (GM == null)
            {
                throw new Exception("HUD: La fonction SetGameManager doit avoir été appelée au moins au fois avant la fonction Draw");
            }

            DrawableRectangle.DrawRectangle(Vector2.Zero, new Vector2(1f, 0.07f), Color.Black);

            #region affichage des messages

            FontSupport.PrintStatus(25, "  " + mMessagePermanent, mColorMsgPermanent);
            if (mMessageTmpFrame < mMessageTmpDisplayTime)
            {
                mMessageTmpFrame++;
                FontSupport.PrintStatus(24, "  " + mMessageTmp, Color.Black);
            }
            #endregion

            if (GM.mState == GameState.CreationDeLaMap)
            {
                FontSupport.PrintStatus(2, "          -- Creation de la map --", Color.DarkBlue);

                FontSupport.PrintStatusPixelPosition(40, (int)(Game1.sGraphics.PreferredBackBufferHeight * 0.95f),
                    "[Clic gauche] Placer un mur    [Clic droit] Placer un teleporteur    [Entree] Terminer", Color.White);
            }

            if (GM.mState == GameState.PlacementPlayers)
            {
                FontSupport.PrintStatus(2, "          -- Placement des joueurs -- (Distance de securite : " + GameRules.Sécurité + " cases)", Color.DarkRed);

                FontSupport.PrintStatusPixelPosition(40, (int)(Game1.sGraphics.PreferredBackBufferHeight * 0.95f),
                    "[Clic gauche] Placer un joueur sur une case rose      [Entree] Terminer / Commencer le jeu", Color.White);
            }

            if (GM.mState == GameState.Jeu || GM.mState == GameState.FinDuJeu)
            {
                if (GM.mState == GameState.Jeu)
                {
                    FontSupport.PrintStatus(2, "          -- Partie en cours --", Color.Black);
                }
                else
                {
                    FontSupport.PrintStatus(2, "          -- PARTIE TERMINEE --", Color.Black);
                }
                int espacement = Game1.sGraphics.PreferredBackBufferWidth / (GM.Players.Count + 1);

                for (int i = 0; i < GM.Players.Count; i++)
                {
                    string vies = " ";
                    if (GM.Players[i].mVies >= 0)
                    {
                        for (int v = GameRules.Vies; v > 0; v--)
                        {
                            if (v > GM.Players[i].mVies)
                            {
                                vies += "[X]";
                            }
                            else
                            {
                                vies += "[O]";
                            }
                        }
                    }
                    else
                    {
                        vies += "est mort !";
                    }

                    FontSupport.PrintStatusPixelPosition((int)(espacement * (i + 0.5f)),
                                                        (int)(Game1.sGraphics.PreferredBackBufferHeight * 0.95f),
                                                        GM.Players[i].mNom + vies,
                                                        GM.Players[i].Couleur);
                }
            }
        }
    }
}
