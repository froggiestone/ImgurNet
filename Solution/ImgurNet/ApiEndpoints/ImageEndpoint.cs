﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImgurNet.Authentication;
using ImgurNet.Exceptions;
using ImgurNet.Helpers;
using ImgurNet.Models;
using ImgurNet.Web;

namespace ImgurNet.ApiEndpoints
{
	public class ImageEndpoint
	{
		internal const int MaxUriLength = 32766;

		#region EndPoints

		internal const string UploadImageUrl =		"image";
		internal const string ImageUrl =			"image/{0}";
		internal const string FavouriteImageUrl =	"image/{0}/favorite";

		#endregion

		/// <summary>
		/// 
		/// </summary>
		public Imgur Imgur { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="imgur"></param>
		public ImageEndpoint(Imgur imgur)
		{
			Imgur = imgur;
		}

		/// <summary>
		/// Get information about an image.
		/// </summary>
		/// <param name="imageId">The Id of the image you want details of.</param>
		/// <returns>The image data.</returns>
		public async Task<ImgurResponse<Image>> GetImageDetailsAsync(string imageId)
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			return await Request.SubmitImgurRequestAsync<Image>(Request.HttpMethod.Get, String.Format(ImageUrl, imageId), Imgur.Authentication);
		}

		/// <summary>
		/// Deletes an image from imgur. You get the deletion hash from the initial image response when you upload an image, or 
		/// from <see cref="GetImageDetailsAsync"/> if you are signed in and own that image;
		/// </summary>
		/// <param name="imageDeletionHash">The image deletion hash</param>
		public async Task<ImgurResponse<Boolean>> DeleteImageAsync(string imageDeletionHash)
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			return await Request.SubmitImgurRequestAsync<Boolean>(Request.HttpMethod.Delete, String.Format(ImageUrl, imageDeletionHash), Imgur.Authentication);
		}

		/// <summary>
		/// Updates an image that was previously uploaded. ImageId can be the Image Id, if you're signed in as the uploader, or the DeleteHash if you are not.
		/// </summary>
		/// <param name="imageId">The ImageId (or deletion hash) of the image to be edited.</param>
		/// <param name="title">The string you want to set as the title of image.</param>
		/// <param name="description">The string you want to set as the description of image.</param>
		/// <returns>A boolean indicating if the transaction was successful.</returns>
		public async Task<ImgurResponse<Boolean>> UpdateImageDetailsAsync(string imageId, string title = null, string description = null)
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var keyPairs = new List<KeyValuePair<string, string>>();
			if (title != null) keyPairs.Add(new KeyValuePair<string, string>("title", title));
			if (description != null) keyPairs.Add(new KeyValuePair<string, string>("description", description));
			var multi = new FormUrlEncodedContent(keyPairs.ToArray());

			return
				await
					Request.SubmitImgurRequestAsync<Boolean>(Request.HttpMethod.Post, String.Format(ImageUrl, imageId),
						Imgur.Authentication, content: multi);
		}

		/// <summary>
		/// Adds/Removes an image from the authenticated user's favourites. Must be authenticated using <see cref="OAuth2Authentication"/> to call this Endpoint.
		/// </summary>
		/// <param name="imageId">The ImageId of the image you want to favourite.</param>
		/// <returns>An enum declaring the state of the favourite.</returns>
		public async Task<ImgurResponse<Boolean>> FavouriteImageAsync(string imageId)
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (!(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			var response =
				await
					Request.SubmitImgurRequestAsync<String>(Request.HttpMethod.Post, String.Format(FavouriteImageUrl, imageId),
						Imgur.Authentication);

			return new ImgurResponse<Boolean>
			{
				Data = (response.Data.ToLowerInvariant() == "favorited"),
				Status = response.Status,
				Success = response.Success
			};
		}


		#region Upload Base64 Image

		public async Task<ImgurResponse<Image>> UploadImageFromBase64Async(string base64ImageData,
			string albumId = null, string name = null, string title = null, string description = null)
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var sb = new StringBuilder();
			for (var i = 0; i < base64ImageData.Length; i += MaxUriLength)
				sb.Append(base64ImageData.Substring(i, Math.Min(MaxUriLength, base64ImageData.Length - i)));

			var keyPairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("image", base64ImageData),
				new KeyValuePair<string, string>("type", "base64")
			};
			if (albumId != null) keyPairs.Add(new KeyValuePair<string, string>(albumId, albumId));
			if (name != null) keyPairs.Add(new KeyValuePair<string, string>("name", name));
			if (title != null) keyPairs.Add(new KeyValuePair<string, string>("title", title));
			if (description != null) keyPairs.Add(new KeyValuePair<string, string>("description", description));
			var multi = new FormUrlEncodedContent(keyPairs.ToArray());

			return await Request.SubmitImgurRequestAsync<Image>(Request.HttpMethod.Post, UploadImageUrl, Imgur.Authentication, content: multi);
		}

		#endregion

		#region Upload Image From Url

		public async Task<ImgurResponse<Image>> UploadImageFromUrlAsync(string url,
			string albumId = null, string name = null, string title = null, string description = null)
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var keyPairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("image", url),
				new KeyValuePair<string, string>("type", "url")
			};
			if (albumId != null) keyPairs.Add(new KeyValuePair<string, string>(albumId, albumId));
			if (name != null) keyPairs.Add(new KeyValuePair<string, string>("name", name));
			if (title != null) keyPairs.Add(new KeyValuePair<string, string>("title", title));
			if (description != null) keyPairs.Add(new KeyValuePair<string, string>("description", description));
			var multi = new FormUrlEncodedContent(keyPairs.ToArray());

			return await Request.SubmitImgurRequestAsync<Image>(Request.HttpMethod.Post, UploadImageUrl, Imgur.Authentication, content: multi);
		}

		public async Task<ImgurResponse<Image>> UploadImageFromUrlAsync(Uri uri,
			string albumId = null, string name = null, string title = null, string description = null)
		{
			return await UploadImageFromUrlAsync(uri.ToString(), albumId, name, title, description);
		}

		#endregion

		#region Upload Image From Binary

		public async Task<ImgurResponse<Image>> UploadImageFromBinaryAsync(byte[] imageBinary,
			string albumId = null, string name = null, string title = null, string description = null)
		{
			return await UploadImageFromBase64Async(Convert.ToBase64String(imageBinary), albumId, name, title, description);
		}

		#endregion
	}
}
