using System;
using Blish_HUD;
using Blish_HUD.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tortle.PlayerMarker.Entity
{
	public class PlayerMarker : IEntity
	{
		public Texture2D MarkerTexture { get; set; }
		public float MarkerOpacity { get; set; }
		public bool Visible { get; set; }
		public Color MarkerColor { get; set; }

		public Vector3 Size { get; set; }

		public float VerticalOffset { get; set; }

		public float DrawOrder => 0;

		private readonly VertexPositionColorTexture[] _vertex;

		public PlayerMarker()
		{
			Size = Vector3.Zero;
			MarkerOpacity = 1f;
			MarkerColor = Color.White;
			Visible = false;

			_vertex = new VertexPositionColorTexture[4];
		}

		public void UpdateMarker()
		{
			_vertex[0].Position = new Vector3(-1, 1, 1) * Size;
			_vertex[0].TextureCoordinate = new Vector2(0, 0);
			_vertex[0].Color = MarkerColor;
			_vertex[1].Position = new Vector3(1, 1, 1) * Size;
			_vertex[1].TextureCoordinate = new Vector2(1, 0);
			_vertex[1].Color = MarkerColor;
			_vertex[2].Position = new Vector3(-1, -1, 1) * Size;
			_vertex[2].TextureCoordinate = new Vector2(0, 1);
			_vertex[2].Color = MarkerColor;
			_vertex[3].Position = new Vector3(1, -1, 1) * Size;
			_vertex[3].TextureCoordinate = new Vector2(1, 1);
			_vertex[3].Color = MarkerColor;
		}

		public void Render(GraphicsDevice graphicsDevice, IWorld world, ICamera camera)
		{
			if (!Visible)
			{
				return;
			}

			var x = GameService.Gw2Mumble.PlayerCharacter.Position.X;
			var y = GameService.Gw2Mumble.PlayerCharacter.Position.Y;
			var z = GameService.Gw2Mumble.PlayerCharacter.Position.Z + VerticalOffset;

			var panAngleRad = (float)(Math.Atan2(GameService.Gw2Mumble.PlayerCamera.Forward.Y,
				GameService.Gw2Mumble.PlayerCamera.Forward.X));
			var pitchAngleRad = (float)(Math.Asin(GameService.Gw2Mumble.PlayerCamera.Forward.Z));

			// TODO - Add option to remove 'facing2' from the x rotation so that the marker doesn't 'face' the tilt of the camera.
			var rotationMatrixX = Matrix.CreateRotationX((float)(pitchAngleRad + 2 * Math.PI / 4));
			var rotationMatrixZ = Matrix.CreateRotationZ((float)(panAngleRad - 2 * Math.PI / 4));
			var translationMatrix = Matrix.CreateTranslation(x, y, z);

			// Rotate the marker to always face the camera, then translate it onto the character's position
			var rotationMatrix = Matrix.Multiply(rotationMatrixX, rotationMatrixZ);
			var worldMatrix = Matrix.Multiply(rotationMatrix, translationMatrix);

			var renderEffect = new BasicEffect(graphicsDevice)
			{
				VertexColorEnabled = true,
				TextureEnabled = true,
				View = GameService.Gw2Mumble.PlayerCamera.View,
				Projection = GameService.Gw2Mumble.PlayerCamera.Projection,
				World = worldMatrix,
				Texture = MarkerTexture,
				Alpha = MarkerOpacity
			};

			var geometryBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, 4,
				BufferUsage.WriteOnly);
			geometryBuffer.SetData(_vertex);

			graphicsDevice.SetVertexBuffer(geometryBuffer, 0);

			foreach (var pass in renderEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
			}

			graphicsDevice.DrawPrimitives(
				PrimitiveType.TriangleStrip,
				0,
				2);
		}

		public void Update(GameTime gameTime)
		{
			/* NOOP */
		}
	}
}
