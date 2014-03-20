﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ImgurNet.Exceptions;
using ImgurNet.Models;
using ImgurNet.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImgurNet.ApiEndpoints
{
	public class GalleryEndpoint : BaseEndpoint
	{
		#region Endpoints

		internal const string GallerySubRedditImageUrl =	"gallery/r/{0}/{1}";
		internal const string GalleryRandomUrl =			"gallery/random/{0}";
		internal const string GallerySubRedditUrl =			"gallery/r/{0}/{1}/{2}/{3}";
		internal const string GallerySearchUrl =			"gallery/search/{0}/{1}?q={2}";
		internal const string GalleryUrl =					"gallery/{0}/{1}/{2}/{3}?showViral={4}";

		#endregion

		public GalleryEndpoint(Imgur imgurClient)
		{
			ImgurClient = imgurClient;
		}

		/// <summary>
		/// Returns the images in the gallery
		/// </summary>
		/// <param name="page">The current page</param>
		/// <param name="section">The section of the site</param>
		/// <param name="sort">The sort method</param>
		/// <param name="window">Change the date range of the request if the section is "top"</param>
		/// <param name="showVirual">Show or hide viral images from the 'user' section.</param>
		public async Task<ImgurResponse<IGalleryObject[]>> GetGalleryImagesAsync(int page = 0,
			GallerySection section = GallerySection.Hot, GallerySort sort = GallerySort.Viral,
			GalleryWindow window = GalleryWindow.Day, bool showVirual = true)
		{
			if (ImgurClient.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var endpoint = String.Format(GalleryUrl, section.ToString().ToLowerInvariant(), sort.ToString().ToLowerInvariant(),
				window.ToString().ToLowerInvariant(), page, showVirual.ToString().ToLowerInvariant());

			return
				await
					Request.SubmitImgurRequestAsync(Request.HttpMethod.Get, endpoint, ImgurClient.Authentication,
						customParser: ParseGalleryObjectArrayResponse);
		}

		/// <summary>
		/// View gallery images for a sub-reddit
		/// </summary>
		/// <param name="subreddit">A valid sub-reddit name</param>
		/// <param name="page">The current page</param>
		/// <param name="sort">The sort method (can't be viral)</param>
		/// <param name="window">Change the date range of the request if the section is "top"</param>
		public async Task<ImgurResponse<IGalleryObject[]>> GetSubredditGalleryAsync(string subreddit, int page = 0,
			GallerySort sort = GallerySort.Time, GalleryWindow window = GalleryWindow.Week)
		{
			if (ImgurClient.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var endpoint = String.Format(GallerySubRedditUrl, subreddit, sort.ToString().ToLowerInvariant(),
				window.ToString().ToLowerInvariant(), page);

			return
				await
					Request.SubmitImgurRequestAsync(Request.HttpMethod.Get, endpoint, ImgurClient.Authentication,
						customParser: ParseGalleryObjectArrayResponse);
		}

		/// <summary>
		/// View a single image in the subreddit
		/// </summary>
		/// <param name="subreddit">A valid sub-reddit name</param>
		/// <param name="imageId">The ID for the image</param>
		public async Task<ImgurResponse<GalleryImage>> GetSubredditGalleryImageAsync(string subreddit, string imageId)
		{
			if (ImgurClient.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var endpoint = String.Format(GallerySubRedditImageUrl, subreddit, imageId);

			return
				await
					Request.SubmitImgurRequestAsync<GalleryImage>(Request.HttpMethod.Get, endpoint, ImgurClient.Authentication);
		}

		/// <summary>
		/// Search the gallery with a given query string
		/// </summary>
		/// <param name="searchQuery">A valid search query</param>
		/// <param name="page">The current page</param>
		/// <param name="sort">The sort method (can't be viral)</param>
		public async Task<ImgurResponse<IGalleryObject[]>> SearchGalleryAsync(string searchQuery, int page = 0,
			GallerySort sort = GallerySort.Time)
		{
			if (ImgurClient.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var endpoint = String.Format(GallerySearchUrl, sort.ToString().ToLowerInvariant(), page, searchQuery);

			return
				await
					Request.SubmitImgurRequestAsync(Request.HttpMethod.Get, endpoint, ImgurClient.Authentication,
						customParser: ParseGalleryObjectArrayResponse);
		}

		/// <summary>
		/// Returns a random set of gallery images.
		/// </summary>
		/// <param name="page">A page of random gallery images, from 0-50. Pages are regenerated every hour.</param>
		public async Task<ImgurResponse<IGalleryObject[]>> GetRandomGalleryImagesAsync(int page = 0)
		{
			if (ImgurClient.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			var endpoint = String.Format(GalleryRandomUrl, page);

			return
				await
					Request.SubmitImgurRequestAsync(Request.HttpMethod.Get, endpoint, ImgurClient.Authentication,
						customParser: ParseGalleryObjectArrayResponse);
		}

		#region Seralization Helpers

		/// <summary>
		/// Parses a list of Gallery Objects
		/// </summary>
		/// <param name="jsonObject">The <see cref="JObject"/> response from imgur</param>
		public ImgurResponse<IGalleryObject[]> ParseGalleryObjectArrayResponse(JObject jsonObject)
		{
			var imgurResponse = new ImgurResponse<IGalleryObject[]>
			{
				Success = jsonObject.Value<bool>("success"),
				Status = (HttpStatusCode) jsonObject.Value<Int32>("status")
			};

			var galleryObjects = new List<IGalleryObject>();
			foreach (var child in jsonObject.SelectToken("data").Children())
			{
				if (child.Value<bool>("is_album"))
					galleryObjects.Add(JsonConvert.DeserializeObject<GalleryAlbum>(child.ToString()));
				else
					galleryObjects.Add(JsonConvert.DeserializeObject<GalleryImage>(child.ToString()));
			}

			imgurResponse.Data = galleryObjects.ToArray();
			return imgurResponse;
		}

		#endregion
	}
}
