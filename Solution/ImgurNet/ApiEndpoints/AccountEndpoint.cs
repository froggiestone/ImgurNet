﻿using System;
using System.Threading.Tasks;
using ImgurNet.Authentication;
using ImgurNet.Exceptions;
using ImgurNet.Models;
using ImgurNet.Web;

namespace ImgurNet.ApiEndpoints
{
	public class AccountEndpoint : BaseEndpoint
	{
		#region EndPoints

		internal const string AccountUrl =					"account/{0}";

		// Albums
		internal const string AccountAlbumDetailsUrl =		"account/{0}/album/{1}";
		internal const string AccountAlbumsUrl =			"account/{0}/albums/{1}";
		internal const string AccountAlbumIdsUrl =			"account/{0}/albums/ids";
		internal const string AccountAlbumCountUrl =		"account/{0}/albums/count";

		// Images
		internal const string AccountDeleteImageUrl =		"account/{0}/image/{1}";
		internal const string AccountImageDetailsUrl =		"account/{0}/image/{1}";
		internal const string AccountImagesUrl =			"account/{0}/images/{1}";
		internal const string AccountImageIdsUrl =			"account/{0}/images/ids";
		internal const string AccountImageCountUrl =		"account/{0}/images/count";

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="imgur"></param>
		public AccountEndpoint(Imgur imgur)
		{
			Imgur = imgur;
		}

		/// <summary>
		/// Request standard account information
		/// </summary>
		/// <param name="username">The username of the account you want information of.</param>
		/// <returns>The account data</returns>
		public async Task<ImgurResponse<Account>> GetAccountDetailsAsync(string username)
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			return await Request.SubmitImgurRequestAsync<Account>(Request.HttpMethod.Get, String.Format(AccountUrl, username), Imgur.Authentication);
		}

		#region Album Specific Endpoints

		/// <summary>
		/// Gets all albums uploaded by an account. This endpoint is pagniated.
		/// </summary>
		/// <param name="page">The page of albums to load.</param>
		/// <param name="username">The username to get the album from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<Album[]>> GetAccountAlbums(int page = 0, string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<Album[]>(Request.HttpMethod.Get, String.Format(AccountAlbumsUrl, username, page),
						Imgur.Authentication);
		}

		/// <summary>
		/// Gets the Details of an album uploaded by an account.
		/// </summary>
		/// <param name="albumId">The Id of the album to get details from</param>
		/// <param name="username">The username to get the album from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<Album>> GetAccountAlbumDetails(string albumId, string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<Album>(Request.HttpMethod.Get, String.Format(AccountAlbumDetailsUrl, username, albumId),
						Imgur.Authentication);
		}

		/// <summary>
		/// Gets a list of AlbumId's that the account has uploaded
		/// </summary>
		/// <param name="username">The username to get album ids from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<String[]>> GetAccountAlbumIds(string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<String[]>(Request.HttpMethod.Get, String.Format(AccountAlbumIdsUrl, username),
						Imgur.Authentication);
		}

		/// <summary>
		/// Gets the number of albums the user has uploaded.
		/// </summary>
		/// <param name="username">The username to get album count from. Can be ignored if using OAuth2, and it will use that account.</param>
		/// <remarks>This tests throws a "Imgur over capacity error right now.. No idea why</remarks>
		public async Task<ImgurResponse<int>> GetAccountAlbumCount(string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<int>(Request.HttpMethod.Get, String.Format(AccountAlbumCountUrl, username),
						Imgur.Authentication);
		}

		#endregion

		#region Image Specific Endpoints

		/// <summary>
		/// Gets all images uploaded by an account. This endpoint is pagniated.
		/// </summary>
		/// <param name="page">The page of images to load.</param>
		/// <param name="username">The username to get the images from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<Image[]>> GetAccountImages(int page = 0, string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<Image[]>(Request.HttpMethod.Get, String.Format(AccountImagesUrl, username, page),
						Imgur.Authentication);
		}

		/// <summary>
		/// Gets the Details of an image uploaded by an account.
		/// </summary>
		/// <param name="imageId">The Id of the image to get details from</param>
		/// <param name="username">The username to get the image from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<Image>> GetAccountImageDetails(string imageId, string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<Image>(Request.HttpMethod.Get, String.Format(AccountImageDetailsUrl, username, imageId),
						Imgur.Authentication);
		}

		/// <summary>
		/// Gets a list of ImageId's that the account has uploaded
		/// </summary>
		/// <param name="username">The username to get image count from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<String[]>> GetAccountImageIds(string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<String[]>(Request.HttpMethod.Get, String.Format(AccountImageIdsUrl, username),
						Imgur.Authentication);
		}

		/// <summary>
		/// Gets the number of images the user has uploaded.
		/// </summary>
		/// <param name="username">The username to get image count from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<int>> GetAccountImageCount(string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<int>(Request.HttpMethod.Get, String.Format(AccountImageCountUrl, username),
						Imgur.Authentication);
		}

		/// <summary>
		/// Deletes an image from a user's account. This always requires a deletion hash.
		/// </summary>
		/// <param name="deletionHash">The deletion hash for the image.</param>
		/// <param name="username">The username of the account to delete from. Can be ignored if using OAuth2, and it will use that account.</param>
		public async Task<ImgurResponse<Boolean>> DeleteAccountImage(string deletionHash, string username = "me")
		{
			if (Imgur.Authentication == null)
				throw new InvalidAuthenticationException("Authentication can not be null. Set it in the main Imgur class.");

			if (username == "me" && !(Imgur.Authentication is OAuth2Authentication))
				throw new InvalidAuthenticationException("You need to use OAuth2Authentication to call this Endpoint.");

			return
				await
					Request.SubmitImgurRequestAsync<Boolean>(Request.HttpMethod.Delete, String.Format(AccountDeleteImageUrl, username, deletionHash),
						Imgur.Authentication);
		}

		#endregion
	}
}
