using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;
using Tortle.PlayerMarker.Models;

namespace Tortle.PlayerMarker.Services
{
	/// <summary>
	/// A cache for textures.
	/// </summary>
	internal class TextureCache : IDisposable
	{
		private static readonly Logger Logger = Logger.GetLogger(typeof(TextureCache));
		private readonly Dictionary<string, TextureInfo> _textures = new Dictionary<string, TextureInfo>();
		private readonly DirectoriesManager _directoriesManager;
		private readonly ContentsManager _contentsManager;

		private readonly string[] _supportedFileFormats = { ".bmp", ".gif", ".jpg", ".png", ".tif", ".dds" };

		public TextureCache(DirectoriesManager directoriesManager, ContentsManager contentsManager)
		{
			_directoriesManager = directoriesManager;
			_contentsManager = contentsManager;
		}

		/// <summary>
		/// Loads all marker images for use.
		/// </summary>
		/// <param name="defaultImageFileNames"></param>
		public async Task Load(IEnumerable<string> defaultImageFileNames)
		{
			var imageDirectory = _directoriesManager.GetFullDirectoryPath("tortle/playermarkers");
			await CopyDefaultMarkerImages(defaultImageFileNames, imageDirectory);
			LoadMarkerImages(imageDirectory);
		}

		/// <summary>
		/// Gets <paramref name="name"/>'s corresponding <see cref="Texture2D"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>A <see cref="Texture2D"/> for the given <paramref name="name"/>.</returns>
		public Texture2D Get(string name)
		{
			return _textures.TryGetValue(name, out var textureInfo) ? textureInfo.Texture : null;
		}

		/// <summary>
		/// Determines whether <see cref="TextureCache"/> contains the <paramref name="name"/> key.
		/// </summary>
		/// <param name="name"></param>
		/// <returns><see langword="true" /> if the <see cref="TextureCache" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
		public bool ContainsKey(string name)
		{
			return _textures.ContainsKey(name);
		}

		/// <summary>
		/// Gets all image file names.
		/// </summary>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="string"/>'s containing image file names.</returns>
		public IEnumerable<string> GetNames()
		{
			return _textures.Keys;
		}

		public void Dispose()
		{
			Logger.Debug("Disposing {textureCount} entries", _textures.Count);
			foreach (var item in _textures)
			{
				item.Value.Dispose();
			}
		}

		/// <summary>
		/// Copies <paramref name="defaultImageFiles"/> into <paramref name="imageDirectory"/>.
		/// </summary>
		/// <param name="defaultImageFiles"></param>
		/// <param name="imageDirectory"></param>
		private async Task CopyDefaultMarkerImages(IEnumerable<string> defaultImageFiles, string imageDirectory)
		{
			try
			{
				foreach (var fileName in defaultImageFiles)
				{
					using var fs = _contentsManager.GetFileStream(fileName);
					using var wfs = File.Create(Path.Combine(imageDirectory, fileName));
					await fs.CopyToAsync(wfs);
				}
			}
			catch (UnauthorizedAccessException e)
			{
				Logger.Error(e, "Unable to access {directory}", imageDirectory);
				Blish_HUD.Debug.Contingency.NotifyFileSaveAccessDenied(imageDirectory, "copying default images");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Unable to copy default markers into {directory}", imageDirectory);
			}
		}

		/// <summary>
		/// Loads all marker images in <paramref name="imageDirectory"/>.
		/// </summary>
		/// <param name="imageDirectory"></param>
		private void LoadMarkerImages(string imageDirectory)
		{
			try
			{
				foreach (var filePath in Directory.GetFiles(imageDirectory))
				{
					if (!_supportedFileFormats.Contains(Path.GetExtension(filePath)))
					{
						continue;
					}

					Add(Path.GetFileName(filePath), filePath);
				}
			}
			catch (UnauthorizedAccessException e)
			{
				Logger.Error(e, "Unable to access markers in {directory}", imageDirectory);
				Blish_HUD.Debug.Contingency.NotifyFileSaveAccessDenied(imageDirectory, "listing images");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Unable to load markers in {directory}", imageDirectory);
			}
		}

		/// <summary>
		/// Adds a <see cref="TextureInfo"/> for the given <paramref name="name"/> and <paramref name="filePath"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="filePath"></param>
		private void Add(string name, string filePath)
		{
			if (_textures.ContainsKey(name))
			{
				return;
			}

			Logger.Debug("Adding {name} located at {filePath}", name, filePath);
			_textures.Add(name, new TextureInfo(filePath));
		}
	}
}
