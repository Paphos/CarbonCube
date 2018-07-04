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
    public class Map
    {
        public int mHeight;
        public int mWidth;

        private List<Case> mCasesDeLaMap = null;
        private Texture2D mImage;
        private Color lineColor = Color.Gray;

        private Element[,] mapArray;
        public List<Case> Téléporteurs = new List<Case>();

        public Map(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mImage = GenerateImage();

            mapArray = new Element[mWidth, mHeight];
            for (int y = 0; y < mHeight; y++)
            {
                for (int x = 0; x < mWidth; x++)
                {
                    mapArray[x, y] = new Element(TypeElement.Vide);
                }
            }
        }

        public Element this[int x, int y]
        {
            get
            {
                if (!IsInMap(x, y))
                {
                    return null;
                }
                return mapArray[x, y];
            }
            set
            {
                mapArray[x, y] = value;
            }
        }

        public Element this[Case c]
        {
            get
            {
                return this[c.X, c.Y];
            }
            set
            {
                this[c.X, c.Y] = value;
            }
        }

        private List<Case> CasesDeLaMap
        {
            get
            {
                if (mCasesDeLaMap == null)
                {
                    mCasesDeLaMap = new List<Case>();
                    for (int y = 0; y < mHeight; y++)
                    {
                        for (int x = 0; x < mWidth; x++)
                        {
                            CasesDeLaMap.Add(new Case(x, y));
                        }
                    }
                }
                return mCasesDeLaMap;
            }
        }

        public void Draw(List<Player> players)
        {
            Rectangle destRect = Camera.ComputePixelRectangleFromWorld(Vector2.Zero, new Vector2(mWidth, mHeight), new Vector2(0f, 1f));
            Game1.sSpriteBatch.Draw(mImage, destRect, Color.White);

            for (int y = 0; y < mHeight; y++)
            {
                for (int x = 0; x < mWidth; x++)
                {
                    switch (mapArray[x, y].Type)
                    {
                        case TypeElement.Mur:
                            DrawableRectangle.DrawCase(new Case(x, y), Color.Black);
                            break;
                        case TypeElement.Téléporteur:
                            DrawableRectangle.DrawCase(new Case(x, y), GameManager.CouleurTéléporteur, Color.White);
                            break;
                        case TypeElement.MurPlayer:
                            foreach (Player p in players)
                            {
                                if (p.mNumero == mapArray[x, y].Numero)
                                {
                                    DrawableRectangle.DrawCase(new Case(x, y), p.mCouleurMur);
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        public Texture2D GenerateImage()
        {
            Texture2D texture = new Texture2D(Game1.sGraphics.GraphicsDevice, mWidth * 20 + 1, mHeight * 20 + 1);// some Texture2D
            Color[,] pixels = new Color[texture.Width, texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (x % 20 == 0 || y % 20 == 0)
                    {
                        pixels[x, y] = lineColor;
                    }
                    else
                    {
                        pixels[x, y] = Color.Transparent;
                    }
                }
            }

            Color[] data = new Color[texture.Width * texture.Height];
            int compteur = 0;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    data[compteur] = pixels[x, y];
                    compteur++;
                }
            }

            texture.SetData(data);
            return texture;
        }

        public void Add(Element e, Case c)
        {
            if (IsInMap(c.X, c.Y) && this[c].Type != e.Type)
            {
                if (this[c].Type == TypeElement.Téléporteur)
                {
                    if (e.Type == TypeElement.Téléporteur) { return; }
                    else { Téléporteurs.Remove(c); }
                }

                mapArray[c.X, c.Y] = e;
                if (e.Type == TypeElement.Téléporteur)
                {
                    Téléporteurs.Add(c);
                }
            }
        }

        public bool IsInMap(int x, int y)
        {
            if (x < 0 || y < 0 || x >= mWidth || y >= mHeight)
            {
                return false;
            }
            return true;
        }

        public void DrawAccessible(Player p)
        {
            if (p.Mode == PlayerMode.Deplacement && p.CasesAccessibles != null)
            {
                List<Case> listeCasesAccessibles = p.CasesAccessibles.ToList();
                listeCasesAccessibles.Remove(p.Case);

                foreach (Case c in listeCasesAccessibles)
                {
                    if (this[c].Type != TypeElement.Téléporteur)
                    {
                        DrawableRectangle.DrawCase(c, GameManager.CouleurAccessibles);
                    }
                    else
                    {
                        DrawableRectangle.DrawCase(c, GameManager.CouleurTéléporteur, GameManager.CouleurAccessibles);
                    }
                }
            }

            if (p.Mode == PlayerMode.Coincé && p.CasesPourTeleportation != null)
            {
                DrawPlacementPlayer(p.CasesPourTeleportation);
            }
        }

        public void DrawPlacementPlayer(List<Case> CasesPlacementPlayers)
        {
            if (CasesPlacementPlayers != null)
            {
                foreach (Case c in CasesPlacementPlayers)
                {
                    if (this[c].Type != TypeElement.Téléporteur)
                    {
                        DrawableRectangle.DrawCase(c, GameManager.CouleurPlacement);
                    }
                    else
                    {
                        throw new Exception("La liste CasesPlacementPlayers ne doit pas contenir de téléporteur (Téléporteur:"+c+")");
                    }
                }
            }
        }

        public void RemplirCasesAccessibles(Case playerPosition, out Tree<Case> CasesAccessibles, int déplacementRestant)
        {
            CasesAccessibles = new Tree<Case>();
            CasesAccessibles.Add(playerPosition, null);

            List<Case> CasesEnAttentes = new List<Case>();
            List<Case> CasesAdjacentes = new List<Case>();
            CasesEnAttentes.Add(playerPosition);

            int? téléporteurLevel = null;

            while (CasesEnAttentes.Count > 0)
            {
                Case current = CasesEnAttentes[0];
                int currentLevel = CasesAccessibles[current].Level;

                if (CasesAccessibles[current].Level < déplacementRestant)
                {
                    CasesAdjacentes.Clear();
                    CasesAdjacentes.Add(new Case(current.X + 1, current.Y));
                    CasesAdjacentes.Add(new Case(current.X - 1, current.Y));
                    CasesAdjacentes.Add(new Case(current.X, current.Y + 1));
                    CasesAdjacentes.Add(new Case(current.X, current.Y - 1));

                    foreach (Case c in CasesAdjacentes)
                    {
                        if (this[c] != null && this[c].Traversable)
                        {
                            if (CasesAccessibles.Add(c, current) && !(CasesEnAttentes.Contains(c)))
                                CasesEnAttentes.Add(c);
                        }
                    }
                }

                if ((téléporteurLevel == null || currentLevel < téléporteurLevel) && this[current].Type == TypeElement.Téléporteur)
                {
                    bool parentNull;
                    Case currentParent = CasesAccessibles.GetParent(current, out parentNull);
                    if (parentNull) currentParent = null;

                    foreach (Case téléporteur in this.Téléporteurs)
                    {
                        if (CasesAccessibles.Add(téléporteur, currentParent) && !(CasesEnAttentes.Contains(téléporteur)))
                            CasesEnAttentes.Add(téléporteur);
                    }
                    téléporteurLevel = currentLevel;
                }

                CasesEnAttentes.Remove(current);
            }
        }

        public bool RemplirCasesPlacementPlayer(List<Player> Players, out List<Case> CasesPlacementPlayer, int distanceSecurité)
        {
            Tree<Case> CasesPlacementPlayerInversé = new Tree<Case>();

            List<Case> CasesEnAttentes = new List<Case>();
            List<Case> CasesAdjacentes = new List<Case>();
            foreach (Player p in Players)
            {
                CasesPlacementPlayerInversé.Add(p.Case, null);
                CasesEnAttentes.Add(p.Case);
            }

            if (this.Téléporteurs.Count > 0)
            {
                CasesPlacementPlayerInversé.Add(this.Téléporteurs[0], null);
                CasesEnAttentes.Add(this.Téléporteurs[0]);
            }

            bool téléporteurPrisEnCompte = false;

            while (CasesEnAttentes.Count > 0)
            {
                Case current = CasesEnAttentes[0];
                int currentLevel = CasesPlacementPlayerInversé[current].Level;

                if (CasesPlacementPlayerInversé[current].Level < distanceSecurité)
                {
                    CasesAdjacentes.Clear();
                    CasesAdjacentes.Add(new Case(current.X + 1, current.Y));
                    CasesAdjacentes.Add(new Case(current.X - 1, current.Y));
                    CasesAdjacentes.Add(new Case(current.X, current.Y + 1));
                    CasesAdjacentes.Add(new Case(current.X, current.Y - 1));

                    foreach (Case c in CasesAdjacentes)
                    {
                        if (this[c] != null && this[c].Traversable)
                        {
                            if (CasesPlacementPlayerInversé.Add(c, current) && !(CasesEnAttentes.Contains(c)))
                                CasesEnAttentes.Add(c);
                        }
                    }
                }

                if (!téléporteurPrisEnCompte && this[current].Type == TypeElement.Téléporteur)
                {
                    bool parentNull;
                    Case currentParent = CasesPlacementPlayerInversé.GetParent(current, out parentNull);
                    if (parentNull) currentParent = null;

                    foreach (Case téléporteur in this.Téléporteurs)
                    {
                        if (CasesPlacementPlayerInversé.Add(téléporteur, currentParent) && !(CasesEnAttentes.Contains(téléporteur)))
                            CasesEnAttentes.Add(téléporteur);
                    }
                    téléporteurPrisEnCompte = true;
                }

                CasesEnAttentes.Remove(current);
            }

            CasesPlacementPlayer = new List<Case>();

            foreach (Case c in CasesDeLaMap)
            {
                if (!(CasesPlacementPlayerInversé.Contains(c)) && this[c].Traversable)
                {
                    if (this[c].Type == TypeElement.MurPlayer)
                    {
                        throw new Exception("La case " + c + " est un MurPlayer.");
                    }

                    CasesPlacementPlayer.Add(c);
                }
            }

            return (CasesPlacementPlayer.Count > 0);
        }
    }

    public enum TypeElement
    {
        Vide,
        Mur,
        MurPlayer,
        Téléporteur
    }

    public class Element
    {
        public TypeElement Type;
        public int? Numero;
        public bool Traversable
        {
            get { return Type == TypeElement.Vide || Type == TypeElement.Téléporteur; }
        }

        public Element(TypeElement type)
        {
            Type = type;
            Numero = null;
        }

        public Element(int numeroPlayer)
        {
            Type = TypeElement.MurPlayer;
            Numero = numeroPlayer;
        }
    }
}
