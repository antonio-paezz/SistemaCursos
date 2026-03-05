using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SistemaDeCursos.Models;
using SistemaDeCursos.ViewModels;
using System;

public class CloudinaryService
{
	private readonly Cloudinary _cloudinary;

	public CloudinaryService(Cloudinary cloudinary)
	{
		_cloudinary = cloudinary;
	}

	public async Task<CloudinaryViewModel> SubirArchivoAsync(IFormFile archivo)
	{
		if (archivo == null || archivo.Length == 0)
			return null;

		var nombreArchivo = Guid.NewGuid().ToString();
		var mime = archivo.ContentType;

		UploadResult result = null;

		// 🎥 Si es video
		if (mime.StartsWith("video"))
		{
			var uploadParams = new VideoUploadParams
			{
				File = new FileDescription(nombreArchivo, archivo.OpenReadStream()),
				Folder = "cursos/videos"
			};

			result = await _cloudinary.UploadAsync(uploadParams);
		}
		// 🖼 Si es imagen
		else if (mime.StartsWith("image"))
		{
			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(nombreArchivo, archivo.OpenReadStream()),
				Folder = "cursos/imagenes"
			};

			result = await _cloudinary.UploadAsync(uploadParams);
		}
        // 📄 Para todos los otros tipos (PDF, Word, Excel, ZIP…)
        else
		{
			var uploadParams = new RawUploadParams
			{
				File = new FileDescription(archivo.FileName, archivo.OpenReadStream()),
				Folder = "cursos/archivos"
			};

			result = await _cloudinary.UploadAsync(uploadParams);
		}
		
		return new CloudinaryViewModel
		{
			NombreArchivo = nombreArchivo,
			UrlArchivo = result.SecureUrl.ToString(),
			PublicId = result.PublicId,
			Tipo = mime   // 💡 guardá el mime real o un tipo simplificado
		};
	}

	public async Task<bool> EliminarArchivo(string publicId, string mime)
	{
		ResourceType resourceType;

		if (mime.StartsWith("image"))
			resourceType = ResourceType.Image;
		else if (mime.StartsWith("video"))
			resourceType = ResourceType.Video;
		else
			resourceType = ResourceType.Raw;

		var deletionParams = new DeletionParams(publicId)
		{
			ResourceType = resourceType
		};

		var result = await _cloudinary.DestroyAsync(deletionParams);
		return result.Result == "ok";
	}
}
