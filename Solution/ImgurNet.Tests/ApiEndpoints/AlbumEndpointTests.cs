﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ImgurNet.ApiEndpoints;
using ImgurNet.Models;
using ImgurNet.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ImgurNet.Tests.ApiEndpoints
{
	[TestClass]
	public class AlbumEndpointTests
	{
		[TestMethod]
		public async Task TestGetAlbumDetails()
		{
			var imgurClient = AuthenticationHelpers.CreateClientAuthenticatedImgurClient();
			var albumEndpoint = new AlbumEndpoint(imgurClient);
			var response = await albumEndpoint.GetAlbumDetailsAsync("IPPAY");

			// Assert the Reponse
			Assert.IsNotNull(response.Data);
			Assert.AreEqual(response.Success, true);
			Assert.AreEqual(response.Status, HttpStatusCode.OK);

			// Asset the Data
			Assert.AreEqual(response.Data.Title, "Grassy Snowbound");
			Assert.AreEqual(response.Data.Layout, "blog");
			Assert.AreEqual(response.Data.Privacy, "public");
		}

		[TestMethod]
		public async Task TestGetAllImagesFromAlbum()
		{
			var imgurClient = AuthenticationHelpers.CreateClientAuthenticatedImgurClient();
			var albumEndpoint = new AlbumEndpoint(imgurClient);
			var albumDetails = await albumEndpoint.GetAlbumDetailsAsync("IPPAY");
			var albumImages = await albumEndpoint.GetAllImagesFromAlbumAsync(albumDetails.Data.Id);

			// Assert the Reponse
			Assert.IsNotNull(albumImages.Data);
			Assert.AreEqual(albumImages.Success, true);
			Assert.AreEqual(albumImages.Status, HttpStatusCode.OK);

			// Asset the Data
			Assert.AreEqual(albumImages.Data.Length, albumDetails.Data.ImagesCount);
		}

		[TestMethod]
		public async Task TestCreateAlbum()
		{
			var imgurClient = await AuthenticationHelpers.CreateOAuth2AuthenticatedImgurClient();
			var albumEndpoint = new AlbumEndpoint(imgurClient);
			var imageEndpoint = new ImageEndpoint(imgurClient);

			var filePath = VariousFunctions.GetTestsAssetDirectory() + @"\upload-image-example.jpg";
			var imageBinary = File.ReadAllBytes(filePath);

			var title = String.Format("dicks-{0}", new Random().Next(100, 1337));
			var description = String.Format("black dicks, yo-{0}", new Random().Next(100, 1337));

			var uploadedImages = new List<Image>();
			for (var i = 0; i < 2; i++)
				uploadedImages.Add((await imageEndpoint.UploadImageFromBinaryAsync(imageBinary)).Data);
			var createdAlbum = await albumEndpoint.CreateAlbumAsync(uploadedImages.ToArray(), uploadedImages[0], title, description);

			// Assert the Reponse
			Assert.IsNotNull(createdAlbum.Data);
			Assert.AreEqual(createdAlbum.Success, true);
			Assert.AreEqual(createdAlbum.Status, HttpStatusCode.OK);

			// Assert the data
			Assert.AreEqual(createdAlbum.Data.Title, title);
			Assert.AreEqual(createdAlbum.Data.Description, description);
		}

		[TestMethod]
		public async Task TestDeleteAlbum()
		{
			var imgurClient = await AuthenticationHelpers.CreateOAuth2AuthenticatedImgurClient();
			var albumEndpoint = new AlbumEndpoint(imgurClient);
			var createdAlbum = await albumEndpoint.CreateAlbumAsync();
			var deletedAlbum = await albumEndpoint.DeleteAlbumAsync(createdAlbum.Data.DeleteHash);

			// Assert the Reponse
			Assert.IsNotNull(deletedAlbum.Data);
			Assert.AreEqual(deletedAlbum.Success, true);
			Assert.AreEqual(deletedAlbum.Status, HttpStatusCode.OK);

			// Assert the data
			Assert.AreEqual(deletedAlbum.Data, true);
		}

		[TestMethod]
		public async Task TestFavouriteAlbum()
		{
			var imgurClient = await AuthenticationHelpers.CreateOAuth2AuthenticatedImgurClient();
			var albumEndpoint = new AlbumEndpoint(imgurClient);
			var createdAlbum = await albumEndpoint.CreateAlbumAsync();
			var favourited = await albumEndpoint.FavouriteAlbumAsync(createdAlbum.Data.Id);

			// Assert the Reponse
			Assert.IsNotNull(favourited.Data);
			Assert.AreEqual(favourited.Success, true);
			Assert.AreEqual(favourited.Status, HttpStatusCode.OK);

			// Assert the data
			Assert.AreEqual(favourited.Data, true);
		}

		[TestMethod]
		public async Task TestAddImagesToAlbum()
		{
			var imgurClient = await AuthenticationHelpers.CreateOAuth2AuthenticatedImgurClient();
			var albumEndpoint = new AlbumEndpoint(imgurClient);
			var imageEndpoint = new ImageEndpoint(imgurClient);

			var filePath = VariousFunctions.GetTestsAssetDirectory() + @"\upload-image-example.jpg";
			var imageBinary = File.ReadAllBytes(filePath);
			var createdAlbum = await albumEndpoint.CreateAlbumAsync();
			await
				albumEndpoint.AddImageToAlbumAsync(createdAlbum.Data.Id,
					(await imageEndpoint.UploadImageFromBinaryAsync(imageBinary)).Data.Id);
			var updatedAlbum = await albumEndpoint.GetAlbumDetailsAsync(createdAlbum.Data.Id);

			// Assert the Reponse
			Assert.IsNotNull(updatedAlbum.Data);
			Assert.AreEqual(updatedAlbum.Success, true);
			Assert.AreEqual(updatedAlbum.Status, HttpStatusCode.OK);

			// Assert the data
			Assert.AreEqual(createdAlbum.Data.ImagesCount + 1, updatedAlbum.Data.ImagesCount);
		}

		[TestMethod]
		public async Task TestRemoveImagesFromAlbum()
		{
			var imgurClient = await AuthenticationHelpers.CreateOAuth2AuthenticatedImgurClient();
			var albumEndpoint = new AlbumEndpoint(imgurClient);
			var imageEndpoint = new ImageEndpoint(imgurClient);

			var filePath = VariousFunctions.GetTestsAssetDirectory() + @"\upload-image-example.jpg";
			var imageBinary = File.ReadAllBytes(filePath);
			var createdAlbum = await albumEndpoint.CreateAlbumAsync(new[] {(await imageEndpoint.UploadImageFromBinaryAsync(imageBinary)).Data});
			await albumEndpoint.RemoveImageFromAlbumAsync(createdAlbum.Data.Id, createdAlbum.Data.Images[0].Id);
			var updatedAlbum = await albumEndpoint.GetAlbumDetailsAsync(createdAlbum.Data.Id);

			// Assert the Reponse
			Assert.IsNotNull(updatedAlbum.Data);
			Assert.AreEqual(updatedAlbum.Success, true);
			Assert.AreEqual(updatedAlbum.Status, HttpStatusCode.OK);
		}
	}
}
