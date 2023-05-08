using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Tortle.PlayerMarker.Models;

namespace Tortle.PlayerMarker.Services
{
	/// <summary>
	/// A cache for textures.
	/// </summary>
	internal class MarkerTextureManager : IDisposable
	{
		private static readonly Logger Logger = Logger.GetLogger(typeof(MarkerTextureManager));
		private readonly Dictionary<string, ITexture> _textures;
		private readonly string _imageDirectory;

		public IEnumerable<ITexture> DefaultTextures { get; }

		private readonly string[] _supportedFileFormats = { ".bmp", ".gif", ".jpg", ".png", ".tif", ".dds" };

		public MarkerTextureManager(DirectoriesManager directoriesManager, ContentsManager contentsManager)
		{
			_imageDirectory = Path.Combine(directoriesManager.GetFullDirectoryPath("tortle"), "playermarkers");

			DefaultTextures = new ITexture[]
			{
				new RefTexture(contentsManager, _imageDirectory, "gw2PersonalTarget.png"),
				new RefTexture(contentsManager, _imageDirectory, "circleFill.png"),
				new Gw2DatTexture(1335145, "gw2CommanderArrow.png"),
				new Gw2DatTexture(1335146, "gw2CommanderCircle.png"),
				new Gw2DatTexture(1335147, "gw2CommanderHeart.png"),
				new Gw2DatTexture(1335148, "gw2CommanderSquare.png"),
				new Gw2DatTexture(1335149, "gw2CommanderStar.png"),
				new Gw2DatTexture(1335150, "gw2CommanderSpiral.png"),
				new Gw2DatTexture(1335151, "gw2CommanderTriangle.png"),
				new Gw2DatTexture(1335152, "gw2CommanderX.png"),
			};

			_textures = DefaultTextures.ToDictionary(a => a.Id);
		}

		/// <summary>
		/// Loads all marker images for use.
		/// </summary>
		public void Load()
		{
			if (!Directory.Exists(_imageDirectory))
			{
				try
				{
					Directory.CreateDirectory(_imageDirectory);
				}
				catch (UnauthorizedAccessException e)
				{
					Logger.Error(e, "Unable to create {directory}", _imageDirectory);
					Blish_HUD.Debug.Contingency.NotifyFileSaveAccessDenied(_imageDirectory,
						"creating playermarkers directory");
					return;
				}
				catch (Exception e)
				{
					Logger.Error(e, "Unable to copy default markers into {directory}", _imageDirectory);
					return;
				}
			}

			LoadCustomMarkers(_imageDirectory);
		}

		/// <summary>
		/// Gets <paramref name="key"/>'s corresponding <see cref="ITexture"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>An <see cref="ITexture"/> for the given <paramref name="key"/>.</returns>
		public ITexture Get(string key)
		{
			return _textures.TryGetValue(key, out var value) ? value : null;
		}

		/// <summary>
		/// Determines whether <see cref="MarkerTextureManager"/> contains the <paramref name="key"/> key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns><see langword="true" /> if the <see cref="MarkerTextureManager" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
		public bool ContainsKey(string key)
		{
			return _textures.ContainsKey(key);
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
		/// Loads all marker images in <paramref name="imageDirectory"/>.
		/// </summary>
		/// <param name="imageDirectory"></param>
		private void LoadCustomMarkers(string imageDirectory)
		{
			try
			{
				foreach (var filePath in Directory.GetFiles(imageDirectory))
				{
					if (!_supportedFileFormats.Contains(Path.GetExtension(filePath)))
					{
						continue;
					}

					if (!AddFile(filePath))
					{
						// TODO - Should we do this? Feels bad to delete files without confirmation from the user.
						// TODO - If we don't do this, the worst case is the duplicate protection kicks in for existing users
						TryDelete(filePath);
					}
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

		private void TryDelete(string filePath)
		{
			try
			{
				Logger.Debug("Removing {filePath}", filePath);
				File.Delete(filePath);
			}
			catch (Exception e)
			{
				Logger.Warn(e, "Failed to remove duplicate file");
			}
		}

		/// <summary>
		/// Adds a <see cref="CustomTexture"/> for the given <paramref name="filePath"/>.
		/// </summary>
		/// <param name="filePath"></param>
		private bool AddFile(string filePath)
		{
			var fileName = Path.GetFileName(filePath);
			if (ContainsKey(fileName))
			{
				Logger.Info("{filename} already loaded", fileName);
				return false;
			}

			Logger.Debug("Adding {filePath}", filePath);
			_textures.Add(fileName, new CustomTexture(filePath, fileName));
			return true;
		}
	}
}
