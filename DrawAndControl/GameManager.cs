using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace CarbonCube
{
    static public class GameRules
    {
        static public int Déplacement = 3;
        static public int Sécurité = 6;
        static public int Vies = 3;
    }

    public class GameManager
    {
        public List<Player> Players = new List<Player>();
        public List<Player> PlayersEnJeu = new List<Player>();
        public List<Player> PlayersParDefaut = new List<Player>();
        public int mIndexPlayer;
        public int mIndexPlayerParDefaut = 0;
        public bool mLimiteDePlayerAtteinte = false;

        public Map mMap;
        public GameState mState;

        public Case CaseSelectionnée;
        public List<Case> CasesPlacementPlayers;

        public int map_width = 40; //40
        public int map_height = 30; //30

        #region Couleurs statiques
        static private Color mCouleurPath = Color.Green;
        static private Color mCouleurAccessibles = new Color(91, 255, 91);
        static private Color mCouleurPlacement = new Color(255, 153, 255);
        static private Color mCouleurTéléporteur = new Color(255, 0, 127);

        static public Color CouleurPath { get { return mCouleurPath; } }
        static public Color CouleurAccessibles { get { return mCouleurAccessibles; } }
        static public Color CouleurTéléporteur { get { return mCouleurTéléporteur; } }
        static public Color CouleurPlacement { get { return mCouleurPlacement; } }

        #endregion

        public GameManager()
        {
            mState = GameState.CreationDeLaMap;
            mMap = new Map(map_width, map_height);

            Camera.SetCameraWindow((float)map_width, (float)map_height);
            Camera.Position = new Vector2(map_width / 2f, map_height / 2f);
            HUD.SetGameManager(this);

            PlayersParDefaut.Add(new Player(0, "Rouge", "R", null, new Color(255, 51, 51), new Color(150, 0, 0)));
            PlayersParDefaut.Add(new Player(0, "Bleu", "B", null, Color.CornflowerBlue, new Color(0, 0, 150)));
            PlayersParDefaut.Add(new Player(0, "Violet", "V", null, new Color(178, 102, 255), new Color(125, 0, 175)));
            PlayersParDefaut.Add(new Player(0, "Jaune", "J", null, Color.Yellow, new Color(120, 120, 0)));
            PlayersParDefaut.Add(new Player(0, "Orange", "O", null, Color.Orange, Color.DarkOrange));
        }

        public void UpdateGame()
        {
            #region Déplacement de la caméra
            Vector2 dirDeplacement = new Vector2(0f, 0f);

            if (Game1.sControles["Gauche"].Etat2 == ButtonState.Pressed)
                dirDeplacement.X -= 1f;
            if (Game1.sControles["Droite"].Etat2 == ButtonState.Pressed)
                dirDeplacement.X += 1f;
            if (Game1.sControles["Haut"].Etat2 == ButtonState.Pressed)
                dirDeplacement.Y += 1f;
            if (Game1.sControles["Bas"].Etat2 == ButtonState.Pressed)
                dirDeplacement.Y -= 1f;

            Camera.Update(dirDeplacement);
            #endregion

            #region Update Souris
            Point mousePos = Mouse.GetState().Position;
            Vector2 tmp = Camera.ComputeWorldPosition(mousePos.X, mousePos.Y);
            CaseSelectionnée = new Case((int)tmp.X, (int)tmp.Y);
            #endregion

            if (mState == GameState.Erreur)
            {
                return;
            }

            if (mState == GameState.FinDuJeu)
            {
                HUD.SetMessagePermanent("> " + PlayersEnJeu[0].mNom + " a gagne !!", PlayersEnJeu[0].CouleurFoncé);
                HUD.MessageTemporaire = " ";
                return;
            }

            if (mState == GameState.CreationDeLaMap)
            {
                if (Game1.sControles["LeftButton"].Etat2 == ButtonState.Pressed)
                {
                    mMap.Add(new Element(TypeElement.Mur), CaseSelectionnée);
                }
                if (Game1.sControles["RightButton"].Etat2 == ButtonState.Pressed)
                {
                    mMap.Add(new Element(TypeElement.Téléporteur), CaseSelectionnée);
                }
                if (Game1.sControles["Entrée"].Etat4 == KeyState.JustPressed)
                {
                    if (!(mMap.RemplirCasesPlacementPlayer(Players, out CasesPlacementPlayers, GameRules.Sécurité)))
                    {
                        HUD.SetMessagePermanent("Erreur : la map est trop petite et ne peut accueillir aucun joueur. Veuillez redemarrer le jeu.", Color.Red);
                        mState = GameState.Erreur;
                        return;
                    }
                    mState = GameState.PlacementPlayers;
                    Game1.sControles.Update();
                }
            }

            if (mState == GameState.PlacementPlayers)
            {
                if (Game1.sControles["LeftButton"].Etat4 == KeyState.JustPressed && !mLimiteDePlayerAtteinte && CasesPlacementPlayers.Contains(CaseSelectionnée))
                {
                    Player pdd = PlayersParDefaut[mIndexPlayerParDefaut];
                    mIndexPlayerParDefaut = (mIndexPlayerParDefaut + 1) % PlayersParDefaut.Count;

                    Players.Add(new Player(Players.Count - 1, pdd.mNom, pdd.mInitiales, CaseSelectionnée, pdd.Couleur, pdd.CouleurFoncé));

                    HUD.MessageTemporaire = Players[Players.Count - 1].mNom + " est pret !";

                    if (!(mMap.RemplirCasesPlacementPlayer(Players, out CasesPlacementPlayers, GameRules.Sécurité)))
                    {
                        mLimiteDePlayerAtteinte = true;
                        HUD.SetMessagePermanent("Il n'y a plus de place sur la map pour un nouveau joueur. Appuyez sur [Entree] pour commencer le jeu.", Color.Black);
                    }
                }

                if (Game1.sControles["Entrée"].Etat4 == KeyState.JustPressed)
                {
                    LaunchGame();
                }
            }

            if (mState == GameState.Jeu)
            {
                if (PlayersEnJeu.Count == 1) //verification condition victoire
                {
                    mState = GameState.FinDuJeu;
                }
                else
                {
                    mIndexPlayer %= PlayersEnJeu.Count;
                }

                if (PlayersEnJeu[mIndexPlayer].Mode == PlayerMode.Inactif)
                {
                    Camera.MakeCinematic(Camera.Position, PlayersEnJeu[mIndexPlayer].Position, 90);
                    PlayersEnJeu[mIndexPlayer].ActiverModeDeplacement(mMap);
                }
                else
                {
                    PlayersEnJeu[mIndexPlayer].Update(mMap);
                }

                if (PlayersEnJeu[mIndexPlayer].Mode == PlayerMode.Coincé)
                {
                    if (PlayersEnJeu[mIndexPlayer].mVies < 0)
                    {
                        HUD.MessageTemporaire = PlayersEnJeu[mIndexPlayer].mNom + " est mort ! =(";
                        PlayersEnJeu.RemoveAt(mIndexPlayer);
                        return;
                    }
                    else
                    {
                        if (PlayersEnJeu[mIndexPlayer].CasesPourTeleportation == null && !(PlayersEnJeu[mIndexPlayer].RemplirCasesPourTeleportation(mMap, PlayersEnJeu)))
                        {
                            HUD.MessageTemporaire = PlayersEnJeu[mIndexPlayer].mNom + " est mort car il n'y a plus de place sur la map.";
                            PlayersEnJeu[mIndexPlayer].mVies = -1;
                            PlayersEnJeu.RemoveAt(mIndexPlayer);
                            return;
                        }
                    }
                }

                if (Game1.sControles["LeftButton"].Etat4 == KeyState.JustPressed)
                {
                    PlayersEnJeu[mIndexPlayer].MoveTo(CaseSelectionnée, mMap);
                }

                if (Game1.sControles["F"].Etat4 == KeyState.JustPressed)
                {
                    Camera.MakeCinematic(Camera.Position, PlayersEnJeu[mIndexPlayer].Position, 90);
                }

                if (PlayersEnJeu[mIndexPlayer].Mode == PlayerMode.FinDeTour)
                {
                    PlayersEnJeu[mIndexPlayer].Mode = PlayerMode.Inactif;
                    mIndexPlayer = (mIndexPlayer + 1) % PlayersEnJeu.Count;
                }

                HUD.SetMessagePermanent("> Tour de " + PlayersEnJeu[mIndexPlayer].mNom, PlayersEnJeu[mIndexPlayer].CouleurFoncé);
            }
        }

        public void DrawGame()
        {
            mMap.Draw(Players);

            if (mState == GameState.CreationDeLaMap)
            {
                if (mMap[CaseSelectionnée] != null && mMap[CaseSelectionnée].Type == TypeElement.Vide)
                {
                    DrawableRectangle.DrawCase(CaseSelectionnée, Color.Gray);
                }
            }

            if (mState == GameState.PlacementPlayers)
            {
                mMap.DrawPlacementPlayer(CasesPlacementPlayers);

                foreach (Player p in Players)
                {
                    p.DrawPlayer();
                    FontSupport.PrintStatusAt(p.Position + new Vector2(0.3f, 0.9f), p.mInitiales, Color.Black);
                }

                if (mMap[CaseSelectionnée] != null && mMap[CaseSelectionnée].Type == TypeElement.Vide)
                {
                    DrawableRectangle.DrawCase(CaseSelectionnée, Color.Gray);
                }

            }

            if (mState == GameState.Jeu)
            {
                mMap.DrawAccessible(PlayersEnJeu[mIndexPlayer]);

                foreach (Player p in PlayersEnJeu)
                {
                    p.DrawPlayer();
                    FontSupport.PrintStatusAt(p.Position + new Vector2(0.3f, 0.9f), p.mInitiales, Color.Black);
                }

                #region Draw Path (en vert foncé)
                if (PlayersEnJeu[mIndexPlayer].Mode == PlayerMode.Deplacement)
                {
                    List<Case> path = new List<Case>();
                    if (PlayersEnJeu[mIndexPlayer].CasesAccessibles != null && PlayersEnJeu[mIndexPlayer].CasesAccessibles.Contains(CaseSelectionnée))
                    {
                        path = PlayersEnJeu[mIndexPlayer].CasesAccessibles.PathTo(CaseSelectionnée);
                        foreach (Case c in path)
                        {
                            if (c != PlayersEnJeu[mIndexPlayer].Case)
                            {
                                if (mMap[c].Type == TypeElement.Téléporteur)
                                {
                                    DrawableRectangle.DrawCase(c, CouleurPath, CouleurAccessibles);
                                }
                                else
                                {
                                    DrawableRectangle.DrawCase(c, CouleurPath);
                                }
                            }
                        }
                    }
                }
                if (PlayersEnJeu[mIndexPlayer].Mode == PlayerMode.Coincé && mMap[CaseSelectionnée] != null && PlayersEnJeu[mIndexPlayer].CasesPourTeleportation.Contains(CaseSelectionnée))
                {
                    DrawableRectangle.DrawCase(CaseSelectionnée, Color.DarkSalmon);
                }
                #endregion
            }


            if (mState == GameState.FinDuJeu)
            {
                PlayersEnJeu[0].DrawPlayer();
                FontSupport.PrintStatusAt(PlayersEnJeu[0].Position + new Vector2(0.3f, 0.9f), PlayersEnJeu[0].mInitiales, Color.Black);
            }

            HUD.Draw();

            #region du debug
            string isMurPlayer = (mMap[CaseSelectionnée] != null) ? (mMap[CaseSelectionnée].Type == TypeElement.MurPlayer).ToString() : "null";
            string dansList = "nope";
            if (PlayersEnJeu != null && PlayersEnJeu.Count > 0 && mIndexPlayer < PlayersEnJeu.Count)
            {
                dansList = (PlayersEnJeu[mIndexPlayer].CasesPourTeleportation != null) ? PlayersEnJeu[mIndexPlayer].CasesPourTeleportation.Contains(CaseSelectionnée).ToString() : "null";
            }
            #endregion
            FontSupport.PrintStatus(1, " [ZQSD/Molette] Bouger camera     | Case = " + CaseSelectionnée + " | isMurPlayer? " + isMurPlayer + " | dansList? " + dansList, Color.Gray);
            //FontSupport.PrintStatus(1, "Camera.Width : " + Camera.Width + " | Camera.Height : " + Camera.Height);
        }

        public void LaunchGame()
        {
            foreach (Player p in Players)
            {
                PlayersEnJeu.Add(p);
            }
            mIndexPlayer = 0;
            mState = GameState.Jeu;
        }
    }

    public enum GameState
    {
        CreationDeLaMap,
        PlacementPlayers,
        Jeu,
        FinDuJeu,
        Erreur
    }
}
