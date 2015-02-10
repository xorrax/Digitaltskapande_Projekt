using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Digitaltskapande_Projekt
{
    public class MenuManager
    {
        SpriteFont font;

        List<string> menuItems;
        List<string> animationTypes , linkType, linkID;
        List<List<string>> attributes, contents;
        List<List<Animation>> animation;
        List<Animation> tempAnimation;
        List<Texture2D> menuImages;

        ContentManager content;
        FileManager fileManager;

        Rectangle source;
        Vector2 position;

        string align;

        int axis, itemNumber;

        private void SetMenuItems()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (menuImages.Count == i)
                    menuImages.Add(ScreenManager.Instance.NullImage);
            }

            for (int i = 0; i < menuImages.Count; i++)
            {
                if (menuItems.Count == i)
                    menuItems.Add(null);
            }
        }
        private void SetAnimations()
        {
            Vector2 dimensions = Vector2.Zero;
            Vector2 addPosition = Vector2.Zero;
            
            if(align.Contains("Center"))
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    dimensions.X += font.MeasureString(menuItems[i]).X + menuImages[i].Width;
                    dimensions.Y += font.MeasureString(menuItems[i]).Y + menuImages[i].Height;
                }
                
                if(axis == 1)
                {
                    addPosition.X = (ScreenManager.Instance.ScreenSize.X - dimensions.X) / 2;
                }
                else if(axis == 2)
                {
                    addPosition.Y = (ScreenManager.Instance.ScreenSize.Y - dimensions.Y) / 2;
                }
            }
            else
            {
                    addPosition = position;
            }

            tempAnimation = new List<Animation>();

            for (int i = 0; i < menuImages.Count; i++)
            {
                dimensions = new Vector2(font.MeasureString(menuItems[i]).X - menuImages[i].Width, font.MeasureString(menuItems[i]).Y - menuImages[i].Height);

                if (axis == 1)
                    addPosition.Y = (ScreenManager.Instance.ScreenSize.Y - dimensions.Y) / 2;
                else
                    addPosition.X = (ScreenManager.Instance.ScreenSize.X - dimensions.X) / 2;

                for (int o = 0; o < animationTypes.Count; o++)
                {
                    switch(animationTypes[o])
                    {
                        case "Fade":
                            tempAnimation.Add(new FadeAnimation());
                            tempAnimation[tempAnimation.Count - 1].LoadContent(content, menuImages[i], menuItems[i], addPosition);
                            tempAnimation[tempAnimation.Count - 1].Font = font;
                            break;
                            
                    }
                }
                if(tempAnimation.Count > 0)
                    animation.Add(tempAnimation);

                tempAnimation = new List<Animation>();

                
                if(axis == 1)
                {
                    addPosition.X += dimensions.X;
                }
                else
                {
                    addPosition.Y += dimensions.Y;
                }
            }
        }
        public void LoadContent(ContentManager content, string id)
        {
            this.content = new ContentManager(content.ServiceProvider, "Content");
            menuItems = new List<string>();
            contents = new List<List<string>>();
            attributes = new List<List<string>>();
            animationTypes = new List<string>();
            menuImages = new List<Texture2D>();
            animation = new List<List<Animation>>();
            linkID = new List<string>();
            linkType = new List<string>();
            position = Vector2.Zero;
            itemNumber = 0;

            fileManager = new FileManager();
            fileManager.LoadContent("Load/Menus.cme", attributes, contents);

            for (int i = 0; i < attributes.Count; i++)
            {
                for (int o = 0; o < attributes[i].Count; o++)
                {
                    switch(attributes[i][o])
                    {
                        case "Font":
                            font = this.content.Load<SpriteFont>("font");
                            break;
                        case "Item":
                            menuItems.Add(contents[i][o]);
                            break;
                        case "Image":
                            menuImages.Add(this.content.Load<Texture2D>(contents[i][o]));
                            break;
                        case "Axis":
                            axis = int.Parse(contents[i][o]);
                            break;
                        case "Position":
                            string[] temp = contents[i][o].Split(' ');
                            position = new Vector2(float.Parse(temp[0]), float.Parse(temp[1]));
                            break;
                        case "Source":
                            temp = contents[i][o].Split(' ');
                            source = new Rectangle(int.Parse(temp[0]), int.Parse(temp[1]), int.Parse(temp[2]), int.Parse(temp[3]));
                            break;
                        case "Animation":
                            animationTypes.Add(contents[i][o]);
                            break;
                        case "Align":
                            align = contents[i][o];
                            break;
                        case "LinkType":
                            linkType.Add(contents[i][o]);
                                break;
                        case"LinkID":
                            linkID.Add(contents[i][o]);
                            break;
                    }
                }
            }

            SetMenuItems();
            SetAnimations();
        }

        public void UnloadContent()
        {
            content.Unload();
            position = Vector2.Zero;
            animationTypes.Clear();
            animation.Clear();
            menuImages.Clear();
            menuItems.Clear();
        }

        public void Update(GameTime gameTime, InputManager inputManager)
        {
            if(axis == 1)
            {
                if (inputManager.KeyPressed(Keys.D, Keys.Right))
                    itemNumber++;
                else if (inputManager.KeyPressed(Keys.A, Keys.Left))
                    itemNumber--;

            }
            else
            {
                if (inputManager.KeyPressed(Keys.S, Keys.Down))
                    itemNumber++;
                else if (inputManager.KeyPressed(Keys.W, Keys.Up))
                    itemNumber--;
            }

            if(inputManager.KeyPressed(Keys.Enter, Keys.X))
            {
                if (linkType[itemNumber] == "Screen")
                {
                    Type newClass = Type.GetType("Digitaltskapande_Projekt." + linkID[itemNumber]);
                    ScreenManager.Instance.AddScreen((GameScreen)Activator.CreateInstance(newClass), inputManager);
                }
            }

            if (itemNumber < 0)
                itemNumber = 0;
            else if (itemNumber > menuItems.Count - 1)
                itemNumber = menuItems.Count - 1;

            for (int i = 0; i < animation.Count; i++)
            {
                for (int o = 0; o < animation[i].Count; o++)
                {
                    if (itemNumber == i)
                        animation[i][o].IsActive = true;
                    else
                        animation[i][o].IsActive = false;

                    animation[i][o].Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < animation.Count; i++)
            {
                for (int o = 0; o < animation[i].Count; o++)
                {
                    animation[i][o].Draw(spriteBatch);
                }
            }
        }
    }
}
