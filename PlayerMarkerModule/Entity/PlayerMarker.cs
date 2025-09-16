using System;
using Blish_HUD;
using Blish_HUD.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tortle.PlayerMarker.Models;

namespace Tortle.PlayerMarker.Entity
{
	/// <summary>
	/// An entity that is rendered above the player's head.
	/// </summary>
	internal sealed class PlayerMarker : IEntity, IDisposable
	{
		private static readonly Logger Logger = Logger.GetLogger(typeof(PlayerMarker));

		private readonly VertexPositionColorTexture[] _vertex;

		private ITexture _markerTexture;
		private BasicEffect _effect;
		private VertexBuffer _buffer;

		/// <summary>
		/// The <see cref="PlayerMarker"/>'s texture.
		/// </summary>
		public ITexture MarkerTexture
		{
			get => _markerTexture;
			set
			{
				// Dispose the previous asset
				_markerTexture?.Dispose();
				_markerTexture = value;
			}
		}

		/// <summary>
		/// The <see cref="PlayerMarker"/>'s opacity.
		/// </summary>
		public float MarkerOpacity { get; set; }

		/// <summary>
		/// The <see cref="PlayerMarker"/>'s visibility.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// The <see cref="PlayerMarker"/>'s color.
		/// </summary>
		public Color MarkerColor { get; set; }

		/// <summary>
		/// The <see cref="PlayerMarker"/>'s size.
		/// </summary>
		public Vector3 Size { get; set; }

		/// <summary>
		/// The <see cref="PlayerMarker"/>'s vertical offset.
		/// </summary>
		public float VerticalOffset { get; set; }

		public float DrawOrder
		{
			get
			{
				var position = GameService.Gw2Mumble.PlayerCharacter.Position;
				position.Z += VerticalOffset;
				return Vector3.DistanceSquared(position, GameService.Graphics.World.Camera.Position);
			}
		}

		public PlayerMarker()
		{
			Size = Vector3.Zero;
			MarkerOpacity = 1f;
			MarkerColor = Color.White;
			Visible = false;
			VerticalOffset = 0f;

			_vertex = new VertexPositionColorTexture[4];
		}

		public void Dispose()
		{
			Logger.Debug("Disposing");
			_buffer?.Dispose();
			_buffer = null;
			_effect?.Dispose();
			_effect = null;
			_markerTexture?.Dispose();
			_markerTexture = null;
		}

		/// <summary>
		/// Updates the marker's Size and Color.
		/// </summary>
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
			if (!Visible || MarkerTexture == null)
			{
				return;
			}

			var x = GameService.Gw2Mumble.PlayerCharacter.Position.X;
			var y = GameService.Gw2Mumble.PlayerCharacter.Position.Y;
			var z = GameService.Gw2Mumble.PlayerCharacter.Position.Z + VerticalOffset;

			var panAngleRad = (float)Math.Atan2(camera.Forward.Y, camera.Forward.X);
			var pitchAngleRad = (float)Math.Asin(camera.Forward.Z);

			var rotationMatrixX = Matrix.CreateRotationX((float)(pitchAngleRad + Math.PI / 2));
			var rotationMatrixZ = Matrix.CreateRotationZ((float)(panAngleRad - Math.PI / 2));
			var translationMatrix = Matrix.CreateTranslation(x, y, z);

			// Rotate the marker to always face the camera, then translate it onto the character's position
			var rotationMatrix = Matrix.Multiply(rotationMatrixX, rotationMatrixZ);
			var worldMatrix = Matrix.Multiply(rotationMatrix, translationMatrix);


			_effect ??= new BasicEffect(graphicsDevice)
			{
				VertexColorEnabled = true,
				TextureEnabled = true,
			};

			_effect.View = GameService.Gw2Mumble.PlayerCamera.View;
			_effect.Projection = GameService.Gw2Mumble.PlayerCamera.Projection;
			_effect.World = worldMatrix;
			_effect.Alpha = MarkerOpacity;
			_effect.Texture = MarkerTexture.Get();


			_buffer ??= new VertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration, 4,
				BufferUsage.WriteOnly);
			_buffer.SetData(_vertex);

			graphicsDevice.SetVertexBuffer(_buffer, 0);

			foreach (var pass in _effect.CurrentTechnique.Passes)
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
