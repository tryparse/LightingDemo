using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Graphics;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;

namespace LightingDemo
{
    class PointLight
    {
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Radius { get; set; }

        public Effect Effect { get; set; }
    }

    class EndPoint
    {
        private Point2 _point;

        public bool IsBegin { get; set; }

        public Segment Segment { get; set; }

        public float Rotation { get; set; }

        public bool IsVisualized { get; set; }

        public EndPoint(Point2 point)
        {
            _point = point;
        }

        public EndPoint(float x, float y)
        {
            _point = new Point2(x, y);
        }
    }

    class Segment
    {
        public EndPoint P1 { get; set; }

        public EndPoint P2 { get; set; }

        public float D { get; set; }
    }

    class Visibility
    {
        LinkedList<Segment> _segments;
        LinkedList<EndPoint> _endPoints;
        Point2 _center;

        LinkedList<Segment> _open;
        List<Point2> _output;
        List<List<Point2>> _intersectionDetected;

        public Visibility()
        {
            _segments = new LinkedList<Segment>();
            _endPoints = new LinkedList<EndPoint>();
            _open = new LinkedList<Segment>();
            _center = new Point2();
            _output = new List<Point2>();
            _intersectionDetected = new List<List<Point2>>();
        }

        // Add a segment, where the first point shows up in the
        // visualization but the second one does not. (Every endpoint is
        // part of two segments, but we want to only show them once.)
        private void AddSegment(float x1, float y1, float x2, float y2)
        {
            Segment segment = new Segment();

            var p1 = new EndPoint(x1, y1)
            {
                Segment = segment,
                IsVisualized = true
            };

            var p2 = new EndPoint(x2, y2)
            {
                Segment = segment,
                IsVisualized = false
            };

            segment.P1 = p1;
            segment.P2 = p2;
            segment.D = 0.0f;

            _segments.AddLast(segment);
            _endPoints.AddLast(p1);
            _endPoints.AddLast(p2);
        }

        // Helper function to construct segments along the outside perimeter
        private void loadEdgeOfMap(int size, int margin)
        {
            AddSegment(margin, margin, margin, size - margin);
            AddSegment(margin, size - margin, size - margin, size - margin);
            AddSegment(size - margin, size - margin, size - margin, margin);
            AddSegment(size - margin, margin, margin, margin);
            // NOTE: if using the simpler distance function (a.d < b.d)
            // then we need segments to be similarly sized, so the edge of
            // the map needs to be broken up into smaller segments.
        }
    }


    class LighingDemoGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixelTexture;

        private RenderTarget2D _colorMapRenderTarget;
        private RenderTarget2D _lightMapRenderTarget;

        private Effect _grayscaleShader;
        private Effect _lightingEffect;
        private Effect _pointLightShader;

        private Matrix _worldMatrix;
        private Matrix _projectionMatrix;

        private PointLight _redLight;

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
            _redLight = new PointLight
            {
                Color = Color.Red,
                Position = new Vector2(200),
                Radius = 100f
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = "Content/bin";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _pixelTexture = Content.Load<Texture2D>("Pixel_grey");
            _pointLightShader = Content.Load<Effect>("PointLight");
            _grayscaleShader = Content.Load<Effect>("ShaderGrayscale");

            var parameters = GraphicsDevice.PresentationParameters;
            _colorMapRenderTarget = CreateRenderTarget(parameters);
            _lightMapRenderTarget = CreateRenderTarget(parameters);

            _worldMatrix = Matrix.CreateTranslation(new Vector3(-Vector2.Zero, 0f));
            _projectionMatrix = Matrix.Multiply(
                _worldMatrix,
                Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height, 0, -1, 0));

            base.LoadContent();
        }

        private RenderTarget2D CreateRenderTarget(PresentationParameters parameters)
        {
            return new RenderTarget2D(GraphicsDevice, parameters.BackBufferWidth, parameters.BackBufferHeight);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawToColorRenderTarget();
            DrawToLightRenderTarget();

            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);

            _spriteBatch.Draw(_colorMapRenderTarget, position: Vector2.Zero, color: Color.White);

            _spriteBatch.DrawCircle(_redLight.Position, _redLight.Radius, 32, _redLight.Color);

            _spriteBatch.End();

            DrawSeparatedRenderTargets();

            base.Draw(gameTime);
        }

        private void DrawToLightRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(_lightMapRenderTarget);

            _pointLightShader.TrySetParameter("ColorRenderTarget", _colorMapRenderTarget);
            _pointLightShader.TrySetParameter("LightColor", _redLight.Color.ToVector4());
            _pointLightShader.TrySetParameter("LightWorldPosition", _redLight.Position);
            _pointLightShader.TrySetParameter("LightRadius", _redLight.Radius);
            _pointLightShader.TrySetParameter("WorldMatrix", _worldMatrix);
            _pointLightShader.TrySetParameter("ViewProjection", _projectionMatrix);

            _spriteBatch.Begin(effect: _pointLightShader, blendState: BlendState.Additive);

            _spriteBatch.Draw(_colorMapRenderTarget, position: Vector2.Zero, color: Color.White);

            _spriteBatch.End();
        }

        private void DrawSeparatedRenderTargets()
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

        private void DrawToColorRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(_colorMapRenderTarget);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_pixelTexture, Vector2.Zero, Color.White);
            
            _spriteBatch.End();

            _pointLightShader.TrySetParameter("ColorRenderTarget", _colorMapRenderTarget);
            _pointLightShader.TrySetParameter("LightColor", _redLight.Color.ToVector4());
            _pointLightShader.TrySetParameter("LightWorldPosition", _redLight.Position);
            _pointLightShader.TrySetParameter("LightRadius", _redLight.Radius);
            _pointLightShader.TrySetParameter("WorldMatrix", _worldMatrix);
            _pointLightShader.TrySetParameter("ViewProjection", _projectionMatrix);

            _spriteBatch.Begin(effect: _pointLightShader, blendState: BlendState.Additive);

            _spriteBatch.Draw(_colorMapRenderTarget, position: Vector2.Zero, color: Color.White);

            _spriteBatch.End();
        }
    }
}
