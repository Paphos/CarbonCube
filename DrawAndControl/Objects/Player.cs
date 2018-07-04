using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


namespace CarbonCube
{
    public class Player
    {
        public int mNumero;
        public string mNom;
        public string mInitiales;

        private Case mPosition_previous = new Case(-5865, -3784);
        private Case mPosition;

        #region Graphic-related members
        private Color mCouleur;
        private Color mCouleurBlink;
        static private int sBlinkDelay0 = 25;         //temps entre 2 clignotements en nb de frames
        static private int sBlinkDelay1 = 5;
        private int mBlinkState = 1;
        private int mBlinkTime = 0;

        public Color mCouleurMur;
        #endregion

        public PlayerMode Mode = PlayerMode.Inactif;
        public Tree<Case> CasesAccessibles;
        public List<Case> CasesPourTeleportation = null;
        public int mDéplacementRestant;
        public int mVies;

        public Vector2 Position
        {
            get { return mPosition.ToVector(); }
        }
        public Case Case
        {
            get { return mPosition; }
        }

        public Color Couleur
        {
            get { return mCouleur; }
        }

        public Color CouleurFoncé
        {
            get { return mCouleurBlink; }
        }



        public Player(int numero, string nom, string initiales, Case position, Color couleur, Color couleurFoncé)
        {
            mNumero = numero;
            mNom = nom;
            mVies = GameRules.Vies;
            mInitiales = initiales;
            mPosition = position;
            mCouleur = couleur;
            mCouleurBlink = couleurFoncé;
            mCouleurMur = couleurFoncé;
        }

        public void Update(Map map)
        {
            if (Mode == PlayerMode.Deplacement)
            {
                if (mPosition != mPosition_previous)
                {
                    map.RemplirCasesAccessibles(mPosition, out CasesAccessibles, mDéplacementRestant);
                }
                if (CasesAccessibles.IsEmpty() && mDéplacementRestant > 0)
                {
                    HUD.MessageTemporaire = this.mNom + " a perdu une vie.";
                    mVies--;
                    Mode = PlayerMode.Coincé;
                }
            }

            mPosition_previous.X = mPosition.X;
            mPosition_previous.Y = mPosition.Y;
        }

        public void MoveTo(Case destination, Map map)
        {
            if (Mode == PlayerMode.Deplacement && CasesAccessibles.Contains(destination))
            {
                List<Case> path = CasesAccessibles.PathTo(destination);
                foreach (Case c in path)
                {
                    if (map[c].Type != TypeElement.Téléporteur)
                    {
                        map.Add(new Element(mNumero), c);
                    }
                }

                this.mPosition = destination;

                mDéplacementRestant -= CasesAccessibles[destination].Level;

                if (mDéplacementRestant == 0)
                {
                    Mode = PlayerMode.FinDeTour;
                }
            }

            if (Mode == PlayerMode.Coincé && CasesPourTeleportation.Contains(destination))
            {
                this.mPosition = destination;
                mDéplacementRestant--;

                CasesPourTeleportation = null;

                if (mDéplacementRestant == 0)
                {
                    Mode = PlayerMode.FinDeTour;
                }
                else
                {
                    Mode = PlayerMode.Deplacement;
                }
            }
        }

        public void DrawPlayer()
        {
            if (mBlinkTime == 0)
            {
                mBlinkState = (mBlinkState + 1) % 2;
            }

            if (mBlinkState == 0)
            {
                mBlinkTime = (mBlinkTime + 1) % sBlinkDelay0;
                DrawableRectangle.DrawCase(Case, mCouleur);
            }
            else
            {
                mBlinkTime = (mBlinkTime + 1) % sBlinkDelay1;
                DrawableRectangle.DrawCase(Case, mCouleurBlink);
            }
        }

        public bool RemplirCasesPourTeleportation(Map map, List<Player> playersEnJeu)
        {
            return map.RemplirCasesPlacementPlayer(playersEnJeu, out CasesPourTeleportation, GameRules.Sécurité);
        }

        public void ActiverModeDeplacement(Map map)
        {
            Mode = PlayerMode.Deplacement;
            mDéplacementRestant = 3;
            this.Update(map);
        }
    }

    public enum PlayerMode
    {
        Inactif,
        Deplacement,
        Coincé,
        FinDeTour
    }

    public class Case : IEquatable<Case>
    {
        public int X;
        public int Y;

        public Case(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2 ToVector()
        {
            return new Vector2(X, Y);
        }

        public override bool Equals(object right)
        {
            if (object.ReferenceEquals(right, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, right))
            {
                return true;
            }

            if (this.GetType() != right.GetType())
            {
                return false;
            }

            return this.Equals(right as Case);
        }

        public bool Equals(Case other)
        {
            return (this.X == other.X && this.Y == other.Y);
        }

        public override int GetHashCode()
        {
            return X * 1997 + Y;
        }

        public override string ToString()
        {
            return "{X:" + X + ",Y:" + Y + "}";
        }

        public static bool operator ==(Case a, Case b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b as object);
        }

        public static bool operator !=(Case a, Case b)
        {
            return !(a == b);
        }
    }
}
