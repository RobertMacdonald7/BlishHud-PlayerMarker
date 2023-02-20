using Blish_HUD;
using Blish_HUD.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Tortle.PlayerMarker.Control
{
    public class PlayerMarker : IEntity
    {
        public Texture2D MarkerTexture;
        public float MarkerOpacity;
        public bool Visible;
        public Color MarkerColor;
        private VertexPositionColorTexture[] vertex;

        private VertexBuffer _geometryBuffer;

        public Vector3 Size { get; set; }

        public float VerticalOffset { get; set; }

        public float DrawOrder => 0;

        private static BasicEffect _renderEffect;

        public PlayerMarker()
        {
            _renderEffect = _renderEffect ?? new BasicEffect(GameService.Graphics.GraphicsDevice);

            this.Size = Vector3.Zero;
            this.MarkerOpacity = 1f;
            this.MarkerColor = Color.White;
            this.Visible = false;

            _renderEffect.TextureEnabled = true;
            _renderEffect.VertexColorEnabled = true;

            vertex = new VertexPositionColorTexture[4];
        }

        public void UpdateRings()
        {
            vertex[0].Position = new Vector3(-1, 1, 1) * Size;
            vertex[0].TextureCoordinate = new Vector2(0, 0);
            vertex[0].Color = MarkerColor;
            vertex[1].Position = new Vector3(1, 1, 1) * Size;
            vertex[1].TextureCoordinate = new Vector2(1, 0);
            vertex[1].Color = MarkerColor;
            vertex[2].Position = new Vector3(-1, -1, 1) * Size;
            vertex[2].TextureCoordinate = new Vector2(0, 1);
            vertex[2].Color = MarkerColor;
            vertex[3].Position = new Vector3(1, -1, 1) * Size;
            vertex[3].TextureCoordinate = new Vector2(1, 1);
            vertex[3].Color = MarkerColor;

            _geometryBuffer = new VertexBuffer(GameService.Graphics.GraphicsDevice, VertexPositionColorTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _geometryBuffer.SetData(vertex);
        }



        public void Render(GraphicsDevice graphicsDevice, IWorld world, ICamera camera)
        {
            if (this.Visible && _geometryBuffer != null)
            {
                float x = GameService.Gw2Mumble.PlayerCharacter.Position.X;
                float y = GameService.Gw2Mumble.PlayerCharacter.Position.Y;
                float z = GameService.Gw2Mumble.PlayerCharacter.Position.Z + VerticalOffset;

                float facing = (float)(Math.Atan2(GameService.Gw2Mumble.PlayerCamera.Forward.Y, GameService.Gw2Mumble.PlayerCamera.Forward.X));
                float facing2 = (float)(Math.Tan(GameService.Gw2Mumble.PlayerCamera.Forward.Z));
                Matrix worldMatrix = Matrix.CreateTranslation(x, y, z);

                var rotationMatrixX = Matrix.CreateRotationX((float)(facing2 - 2 * Math.PI / 4));
                var rotationMatrixZ = Matrix.CreateRotationZ((float)(facing - 2 * Math.PI / 4));

                var rotationMatrix = Matrix.Multiply(rotationMatrixX, rotationMatrixZ);
                worldMatrix = Matrix.Multiply(rotationMatrix, worldMatrix);

                _renderEffect.View = GameService.Gw2Mumble.PlayerCamera.View;
                _renderEffect.Projection = GameService.Gw2Mumble.PlayerCamera.Projection;
                _renderEffect.World = worldMatrix;
                _renderEffect.Texture = MarkerTexture;
                _renderEffect.Alpha = MarkerOpacity;

                graphicsDevice.SetVertexBuffer(_geometryBuffer, 0);

                foreach (var pass in _renderEffect.CurrentTechnique.Passes)
                    pass.Apply();

                graphicsDevice.DrawPrimitives(
                    PrimitiveType.TriangleStrip,
                    0,
                    2);
            }
        }

        public void Update(GameTime gameTime) { /* NOOP */ }
    }
}