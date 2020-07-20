using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Graphics;
using System.Linq;

namespace LightingDemo
{
    class LightSource
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Radius { get; set; }
    }

    class LighingDemoGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixelTexture;
        private Texture2D _lightmask;

        private RenderTarget2D _colorMapRenderTarget;
        private RenderTarget2D _lightMapRenderTarget;

        private Effect _lightingEffect;

        private LightSource _redLight;
        private LightSource _greenLight;
        private LightSource _blueLight;

        public LighingDemoGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1500,
                PreferredBackBufferHeight = 1024,
            };

            IsMouseVisible = true;

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _redLight = new LightSource
            {
                Color = Color.Red,
                Position = new Vector2(200),
                Radius = 1200f
            };

            _greenLight = new LightSource
            {
                Color = Color.Green,
                Position = new Vector2(400),
                Radius = 1200f
            };

            _blueLight = new LightSource
            {
                Color = Color.Blue,
                Position = new Vector2(600),
                Radius = 1200f
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = "Content/bin";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _pixelTexture = Content.Load<Texture2D>("Pixel_grey");
            _lightmask = Content.Load<Texture2D>("lightmask");
            _lightingEffect = Content.Load<Effect>("SimpleEffect");

            var parameters = GraphicsDevice.PresentationParameters;
            _colorMapRenderTarget = CreateRenderTarget(parameters);
            _lightMapRenderTarget = CreateRenderTarget(parameters);

            base.LoadContent();
        }

        private RenderTarget2D CreateRenderTarget(PresentationParameters parameters)
        {
            return new RenderTarget2D(GraphicsDevice, parameters.BackBufferWidth, parameters.BackBufferHeight);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawColorRenderTarget();

            DrawLightRenderTarget();

            GraphicsDevice.SetRenderTarget(null);

            if (_lightingEffect.Parameters.Any(x => x.Name == "colorTexture"))
            {
                _lightingEffect.Parameters["colorTexture"].SetValue(_colorMapRenderTarget);
            }

            if (_lightingEffect.Parameters.Any(x => x.Name == "lightTexture"))
            {
                _lightingEffect.Parameters["lightTexture"].SetValue(_lightMapRenderTarget);
            }

            _spriteBatch.Begin(effect: _lightingEffect, sortMode: SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied);

            _spriteBatch.Draw(_colorMapRenderTarget, position: Vector2.Zero, color: Color.White);

            _spriteBatch.End();
            
            DrawRenderTargets();

            base.Draw(gameTime);
        }

        private void DrawRenderTargets()
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(
                texture: _colorMapRenderTarget,
                position: new Vector2(1024, 0),
                sourceRectangle: null,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: new Vector2(.3f),
                effects: SpriteEffects.None,
                layerDepth: 0f);

            _spriteBatch.Draw(
                texture: _lightMapRenderTarget, 
                position: new Vector2(1024, 350),
                sourceRectangle: null,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero, 
                scale: new Vector2(.3f),
                effects: SpriteEffects.None,
                layerDepth: 0f);

            _spriteBatch.End();
        }

        private void DrawLightRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(_lightMapRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(blendState: BlendState.Additive);

            _spriteBatch.Draw(
                texture: _lightmask,
                position: Mouse.GetState().Position.ToVector2(),
                sourceRectangle: null,
                color: _redLight.Color,
                rotation: 0f,
                origin: _lightmask.Bounds.Center.ToVector2(),
                scale: _redLight.Radius / (float)_lightmask.Width,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            _spriteBatch.Draw(
                texture: _lightmask,
                position: _greenLight.Position,
                sourceRectangle: null,
                color: _greenLight.Color,
                rotation: 0f,
                origin: _lightmask.Bounds.Center.ToVector2(),
                scale: _greenLight.Radius / (float)_lightmask.Width,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            _spriteBatch.Draw(
                texture: _lightmask,
                position: _blueLight.Position,
                sourceRectangle: null,
                color: _blueLight.Color,
                rotation: 0f,
                origin: _lightmask.Bounds.Center.ToVector2(),
                scale: _blueLight.Radius / (float)_lightmask.Width,
                effects: SpriteEffects.None,
                layerDepth: 0f);

            _spriteBatch.End();
        }

        private void DrawColorRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(_colorMapRenderTarget);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_pixelTexture, Vector2.Zero, Color.White);

            _spriteBatch.End();
        }
    }
}
