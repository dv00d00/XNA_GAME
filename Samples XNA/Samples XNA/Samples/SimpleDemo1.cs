using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerPhysics.SamplesFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FarseerPhysics.Common;
    using FarseerPhysics.Common.Decomposition;

    using Microsoft.Xna.Framework.Input;

    struct BodySprite
    {
        public Body Body { get; set; }

        public Sprite Sprite { get; set; }
    }

    internal class SimpleDemo1 : PhysicsGameScreen, IDemoScreen
    {
        private Border _border;
        private Body _rectangle;
        private Sprite _rectangleSprite;

        private Body gamePad2;
        private Sprite gamePadSprite2;

        private Body gamePad1;

        private Sprite gamePadSprite1;

        private bool geometryAdded;

        private readonly List<BodySprite> dymanicShit = new List<BodySprite>();

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Body with a single fixture";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows a single body with one attached fixture and shape.");
            sb.AppendLine("A fixture binds a shape to a body and adds material");
            sb.AppendLine("properties such as density, friction, and restitution.");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Rotate object: left and right triggers");
            sb.AppendLine("  - Move object: right thumbstick");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Rotate Object: left and right arrows");
            sb.AppendLine("  - Move Object: A,S,D,W");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

        private void LoadObstacles()
        {
            this.gamePad2 = BodyFactory.CreateEllipse(World, 1f, 3f, 20, 1f);
            this.gamePad2.IsStatic = true;
            this.gamePad2.Restitution = 1f;
            this.gamePad2.Friction = 0f;

            this.gamePad2.Position = new Vector2(20f, 0f);

            this.gamePadSprite2 = new Sprite(ScreenManager.Assets.TextureFromShape(
                this.gamePad2.FixtureList[0].Shape,
                MaterialType.Dots,
                Color.SandyBrown, 0.8f));
            
            this.gamePad1 = BodyFactory.CreateEllipse(World, 1f, 3f, 20, 1f);
            this.gamePad1.IsStatic = true;
            this.gamePad1.Restitution = 1f;
            this.gamePad1.Friction = 0f;
                        
            this.gamePad1.Position = new Vector2(-20f, 0f);

            this.gamePadSprite1 = new Sprite(ScreenManager.Assets.TextureFromShape(
                this.gamePad2.FixtureList[0].Shape,
                MaterialType.Dots,
                Color.SandyBrown, 0.8f));
        }

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = Vector2.Zero;

            _border = new Border(World, this, ScreenManager.GraphicsDevice.Viewport);
            

            // Body circle = BodyFactory.CreateCircle(World, 5f, 0f, 1f);

            _rectangle = BodyFactory.CreateCircle(World, 1f, 0f, 1f); //BodyFactory.CreateRectangle(World, 5f, 5f, 1f);
            _rectangle.BodyType = BodyType.Dynamic;

            LoadObstacles();
            

            SetUserAgent(_rectangle, 10f, 0f);
            

            // create sprite based on body
            _rectangleSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_rectangle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.Orange, 1f));
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            ScreenManager.SpriteBatch.Draw(_rectangleSprite.Texture, ConvertUnits.ToDisplayUnits(_rectangle.Position),
                                           null,
                                           Color.White, _rectangle.Rotation, _rectangleSprite.Origin, 1f,
                                           SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(this.gamePadSprite2.Texture, ConvertUnits.ToDisplayUnits(this.gamePad2.Position),
                                               null,
                                               Color.White, this.gamePad2.Rotation, this.gamePadSprite2.Origin, 1f,
                                               SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(this.gamePadSprite1.Texture, ConvertUnits.ToDisplayUnits(this.gamePad1.Position),
                                               null,
                                               Color.White, this.gamePad1.Rotation, this.gamePadSprite1.Origin, 1f,
                                               SpriteEffects.None, 0f);

            foreach (var bodySprite in dymanicShit)
            {
                ScreenManager.SpriteBatch.Draw(bodySprite.Sprite.Texture, ConvertUnits.ToDisplayUnits(bodySprite.Body.Position),
                                               null,
                                               Color.White, bodySprite.Body.Rotation, bodySprite.Sprite.Origin, 1f,
                                               SpriteEffects.None, 0f);
            }

            ScreenManager.SpriteBatch.End();
            _border.Draw();
            base.Draw(gameTime);
        }
        
        const float MaxTop = (float)11;
        const float step = (float)0.75;

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            Keys[] pressedKeys = input.KeyboardState.GetPressedKeys();

            if(pressedKeys.Any(x => x == Keys.Up))
            {
                this.MoveVertical(gamePad2, -step);

                //this.gamePad2.Position = new Vector2((float)(this.gamePad2.Position.X), Math.Max(this.gamePad2.Position.Y - step, MinTop));
            }
            else if (pressedKeys.Any(x => x == Keys.Down))
            {
                //this.gamePad2.Position = new Vector2((float)(this.gamePad2.Position.X), Math.Min(this.gamePad2.Position.Y + step, MaxTop));

                this.MoveVertical(gamePad2, +step);
            }
            
            if(pressedKeys.Any(x => x == Keys.W))
            {
                this.MoveVertical(gamePad1, -step);
            }
            else if (pressedKeys.Any(x => x == Keys.S))
            {
                this.MoveVertical(gamePad1, +step);

                //this.gamePad1.Position = new Vector2((float)(this.gamePad1.Position.X), Math.Min(this.gamePad1.Position.Y + step, MaxTop));
            }

            if (pressedKeys.Any(x => x == Keys.Enter) && !geometryAdded)
            {
                geometryAdded = true;

                var verts = new Vertices { new Vector2(-2, -2), new Vector2(-2, 2), new Vector2(2, 2), new Vector2(2, -2) };
                var geo = BodyFactory.CreateCompoundPolygon(World, BayazitDecomposer.ConvexPartition(verts), 1f, new Vector2(-3));

                geo.IsStatic = true;
                geo.Restitution = 1f;
                geo.Friction = 0f;

                var sprite = new Sprite(ScreenManager.Assets.TextureFromShape(geo.FixtureList[0].Shape, MaterialType.Dots, Color.SandyBrown, 0.8f));

                dymanicShit.Add(new BodySprite { Body = geo, Sprite = sprite });
            }

            base.HandleInput(input, gameTime);
        }

        private void MoveVertical(Body body, float step)
        {
            float newPosition = body.Position.Y + step;

            if (Math.Abs(newPosition) > MaxTop)
            {
                newPosition = MaxTop * Math.Sign(step);
            }

            body.Position = new Vector2(body.Position.X, newPosition);
        }
    }
}