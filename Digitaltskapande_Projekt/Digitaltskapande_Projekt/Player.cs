using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Digitaltskapande_Projekt
{
    public class Player : Entity
    {

        public override void LoadContent(ContentManager content, InputManager input)
        {
            base.LoadContent(content, input);
            fileManager = new FileManager();
            fileManager.LoadContent("Load/Player.cme", attributes, contents);
            moveAnimation = new SpriteAnimation();
            Vector2 tempFrames = Vector2.Zero;


            for (int i = 0; i < attributes.Count; i++)
            {
                for (int o = 0; o < attributes[i].Count; o++)
                {
                    switch(attributes[i][o])
                    {
                        case"Hp":
                            hp = int.Parse(contents[i][o]);
                            break;
                        case"Frames":
                            string[] frames = contents[i][o].Split(' ');
                            tempFrames = new Vector2(int.Parse(frames[0]), int.Parse(frames[1]));
                            break;
                        case"Image":
                            image = this.content.Load<Texture2D>(contents[i][o]);
                            break;
                        case"Position":
                            frames = contents[i][o].Split(' ');
                            position = new Vector2(int.Parse(frames[0]), int.Parse(frames[1]));
                            break;
                    }
                }
            }

            moveAnimation.LoadContent(content, image, "", position);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            moveAnimation.UnloadContent();
        }

        public override void Update(GameTime gameTime, InputManager input)
        {
            
            moveAnimation.IsActive = true;
            if (input.KeyDown(Keys.Right, Keys.D))
                moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 0);
            else if (input.KeyDown(Keys.Left, Keys.A))
                moveAnimation.CurrentFrame = new Vector2(moveAnimation.CurrentFrame.X, 1);
            else
                moveAnimation.IsActive = false;

            if (input.KeyReleased(Keys.Right, Keys.D))
                moveAnimation.CurrentFrame = new Vector2(0, 0);
            else if (input.KeyReleased(Keys.Left, Keys.A))
                moveAnimation.CurrentFrame = new Vector2(0, 1);

            moveAnimation.Update(gameTime);
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            moveAnimation.Draw(spriteBatch);
        }
    }
}
