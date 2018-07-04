using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CarbonCube
{
    public class InputWrapper
    {
        private Dictionary<string, Touche> mTouches = new Dictionary<string, Touche>();

        public InputWrapper()
        {
            mTouches.Add("LeftButton", new Touche(MouseButton.LeftButton));
            mTouches.Add("RightButton", new Touche(MouseButton.RightButton));

            mTouches.Add("Haut", new Touche(Keys.W));
            mTouches.Add("Bas", new Touche(Keys.S));
            mTouches.Add("Gauche", new Touche(Keys.A));
            mTouches.Add("Droite", new Touche(Keys.D));
            mTouches.Add("Echap", new Touche(Keys.Escape));
            mTouches.Add("Tab", new Touche(Keys.Tab));
            mTouches.Add("F", new Touche(Keys.F));
            mTouches.Add("Entrée", new Touche(Keys.Enter));

            mTouches.Add("FlècheHaut", new Touche(Keys.Up));
            mTouches.Add("FlècheBas", new Touche(Keys.Down));
            mTouches.Add("FlècheDroite", new Touche(Keys.Right));
            mTouches.Add("FlècheGauche", new Touche(Keys.Left));
        }

        public Touche this[string nom]
        {
            get
            {
                return mTouches[nom];
            }
        }

        public void Update()
        {
            foreach (KeyValuePair<string,Touche> kvp in mTouches)
            {
                kvp.Value.Update();
            }
            ScrollWheel.Update();
        }
    }

    public class Touche
    {
        private Keys? mKey;
        private MouseButton mMouseButton;

        private ButtonState mEtatPrecedent;
        private ButtonState mEtatActuel;

        public Touche(Keys key)
        {
            mKey = key;
        }

        public Touche(MouseButton button)
        {
            mMouseButton = button;
            mKey = null;
        }

        public ButtonState Etat2
        {
            get { return mEtatActuel; }
        }

        public KeyState Etat4
        {
            get
            {
                if (mEtatActuel == ButtonState.Released)
                {
                    if (mEtatPrecedent == ButtonState.Released)
                        return KeyState.Released;
                    else
                        return KeyState.JustReleased;
                }
                else
                {
                    if (mEtatPrecedent == ButtonState.Released)
                        return KeyState.JustPressed;
                    else
                        return KeyState.Pressed;
                }
            }
        }

        public void Update()
        {
            mEtatPrecedent = mEtatActuel;

            if (mKey != null)
            {
                Keys mKeyNotNull = (Keys)mKey;
                if (Keyboard.GetState().IsKeyDown(mKeyNotNull))
                {
                    mEtatActuel = ButtonState.Pressed;
                }
                else
                {
                    mEtatActuel = ButtonState.Released;
                }
            }
            else
            {
                MouseState mMouseState = Mouse.GetState();
                switch (mMouseButton)
                {
                    case MouseButton.LeftButton:
                        mEtatActuel = mMouseState.LeftButton;
                        break;
                    case MouseButton.MiddleButton:
                        mEtatActuel = mMouseState.MiddleButton;
                        break;
                    case MouseButton.RightButton:
                        mEtatActuel = mMouseState.RightButton;
                        break;
                }
            }
        }
    }

    public enum KeyState
    {
        Released,
        Pressed,
        JustPressed,
        JustReleased
    }

    public enum MouseButton
    {
        LeftButton,
        RightButton,
        MiddleButton
    }

    static public class ScrollWheel
    {
        static private int ScrollWheelValue_prev;
        static private int ScrollWheelValue_actu;
        static public float difference = 0;

        static public void Update()
        {
            ScrollWheelValue_prev = ScrollWheelValue_actu;
            ScrollWheelValue_actu = Mouse.GetState().ScrollWheelValue;

            difference = ScrollWheelValue_actu - ScrollWheelValue_prev;
        }
    }
}
