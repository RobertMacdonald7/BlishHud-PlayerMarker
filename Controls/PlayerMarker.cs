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
	        if (!this.Visible || _geometryBuffer == null) return;

	        var x = GameService.Gw2Mumble.PlayerCharacter.Position.X;
	        var y = GameService.Gw2Mumble.PlayerCharacter.Position.Y;
	        var z = GameService.Gw2Mumble.PlayerCharacter.Position.Z + VerticalOffset;

	        // The camera 'pan' angle
	        var panAngleRad = (float)(Math.Atan2(GameService.Gw2Mumble.PlayerCamera.Forward.Y, GameService.Gw2Mumble.PlayerCamera.Forward.X));

	        // The camera 'pitch' angle
	        var pitchAngleRad = (float)(Math.Asin(GameService.Gw2Mumble.PlayerCamera.Forward.Z));

            // TODO - Add option to remove 'facing2' from the x rotation so that the marker doesn't 'face' the tilt of the camera.
	        var rotationMatrixX = Matrix.CreateRotationX((float)(pitchAngleRad + 2 * Math.PI / 4));
	        var rotationMatrixZ = Matrix.CreateRotationZ((float)(panAngleRad - 2 * Math.PI / 4));
	        var translationMatrix = Matrix.CreateTranslation(x, y, z);

	        // Rotate the marker to always face the camera, then translate it onto the character's position
	        var rotationMatrix = Matrix.Multiply(rotationMatrixX, rotationMatrixZ);
	        var worldMatrix = Matrix.Multiply(rotationMatrix, translationMatrix);

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

        public void Update(GameTime gameTime) { /* NOOP */ }
    }
}