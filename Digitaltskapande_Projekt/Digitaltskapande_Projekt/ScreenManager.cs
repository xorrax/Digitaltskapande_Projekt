using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digitaltskapande_Projekt
{
    public class ScreenManager
    {
        #region Vari

        private static ScreenManager instance;

        ContentManager content;

        Stack<GameScreen> screenStack = new Stack<GameScreen>();

        GameScreen currentScreen;
        GameScreen newScreen;

        Vector2 screenSize;

        bool transition;

        FadeAnimation fade;

        Texture2D fadeTexture, nullImage;

        InputManager inputManager;

        #endregion

        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();

                return instance;
            }

        }

        public Vector2 ScreenSize
        {
            get { return screenSize; }
            set { screenSize = value; }
        }

        public Texture2D NullImage
        {
            get { return nullImage; }
        }

        public void AddScreen(GameScreen gameScreen, InputManager inputManager)
        {
            transition = true;
            newScreen = gameScreen;
            fade.IsActive = true;
            fade.Alpha = 0.0f;
            fade.ActivateNumber = 1.0f;
            this.inputManager = inputManager;
        }

        public void Initialize()
        {
            currentScreen = new IntroScreen();
            fade = new FadeAnimation();
            inputManager = new InputManager();
        }
        public void LoadContent(ContentManager Content)
        {
            content = new ContentManager(Content.ServiceProvider, "Content");
            currentScreen.LoadContent(Content, inputManager);

            nullImage = this.content.Load<Texture2D>("null");
            fadeTexture = this.content.Load<Texture2D>("fade");
            fade.LoadContent(content, fadeTexture, "", Vector2.Zero);
            fade.Scale = screenSize.X;
        }
        public void Update(GameTime gameTime)
        {
            if (!transition)
                currentScreen.Update(gameTime);
            else
                Transition(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
            if (transition)
                fade.Draw(spriteBatch);
        }

        private void Transition(GameTime gameTime)
        {
            fade.Update(gameTime);

            if(fade.Alpha == 1.0f && fade.Timer.TotalSeconds == 1.0f)
            {
                screenStack.Push(newScreen);
                currentScreen.UnloadContent();
                currentScreen = newScreen;
                currentScreen.LoadContent(content, this.inputManager);
            }
            else if(fade.Alpha == 0.0f)
            {
                transition = false;
                fade.IsActive = false;
            }

        }
    }
}
