using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using swi2grupp1WebAPI.Data;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace swi2grupp1WebAPI.DataAccessLayer
{
    public class CosmosDAC

    {

        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = AppConfiguration.CosmosEndpointUri;

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = AppConfiguration.CosmosPrimaryKey;

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The Cosmos Database

        private Database database;
        // The Cosmos Container 
        private Container container;

        // The name of the database and container we will create
        private string dBName = "ostdb";
        private string containerName = "swi2fs22grp1";

        // Konstruktor
        public CosmosDAC()
        {
            // Instanz von Cosmos Client erstellen
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            database = cosmosClient.GetDatabase(dBName);
            container = database.GetContainer(containerName);
        }

        // <GetImage>
        /// <summary>
        /// Read Image
        /// </summary>
        public async Task<FilmImage> GetImage(String id)
        {
            FilmImage movie = new FilmImage();
            var partitionKey = new PartitionKey(id);
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<FilmImage> movieiResponse =
                    await this.container.ReadItemAsync<FilmImage>(id, partitionKey);
                Console.WriteLine("Item in database with id: {0} found\n", movieiResponse.Resource.Id);
                movie.Id = movieiResponse.Resource.Id;
                movie.Image = movieiResponse.Resource.Image;
                movie.Preview = movieiResponse.Resource.Preview;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                movie.Id = "0";
                Console.WriteLine("Entry with id {0} not found!\n", id);
            }
            return movie;
        }

        // <AddItemsToContainerAsync>
        /// <summary>
        /// Add an Image-Entry
        /// </summary>
        public async Task AddImage(FilmImage moviei)
        {
            Console.WriteLine("Cosmos DB AddImage: " + moviei.Id);
            if (moviei.Id != "0")
            {
                var partitionKey = new PartitionKey(moviei.Id.ToString());
                try
                {
                    // Create an item in the container representing some subgroup. Note we can provide the value of the partition key for this item
                    ItemResponse<FilmImage> movieiResponse =
                        await this.container.CreateItemAsync<FilmImage>(moviei, partitionKey);

                    // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                    Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", movieiResponse.Resource.Id, movieiResponse.RequestCharge);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    Console.WriteLine("Record already exists " + moviei.Id);
                }
            }
        }

        // <ReplaceImage>
        /// <summary>
        /// Replace an item in the container
        /// </summary>
        public async Task ReplaceImage(FilmImage moviei)
        {
            Console.WriteLine("Cosmos DB ReplaceImage: " + moviei.Id);
            if (moviei.Id != "0")
            {
                var partitionKey = new PartitionKey(moviei.Id.ToString());
                try
                {
                    ItemResponse<FilmImage> movieiResponse =
                        await this.container.ReadItemAsync<FilmImage>(moviei.Id, partitionKey);
                    var itemBody = movieiResponse.Resource;
                    // update Fiels
                    itemBody.Image = moviei.Image;
                    itemBody.Preview = moviei.Preview;
                    // replace the item with the updated content
                    movieiResponse = await this.container.ReplaceItemAsync<FilmImage>(itemBody, itemBody.Id, partitionKey);
                    Console.WriteLine("Image updated\n");

                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.BadRequest ||
                    ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await AddImage(moviei);
                }
            }
        }
        // </ReplaceFamilyItemAsync>

        // <DeleteImageAsync>
        /// <summary>
        /// Delete an item in the container
        /// </summary>
        public async Task DeleteImage(string id)
        {
            Console.WriteLine("Cosmos DB DeleteImage: " + id);
            if (id != "0")
            {
                var partitionKey = new PartitionKey(id.ToString());
                try
                {
                    // Delete an item. Note we must provide the partition key value and id of the item to delete
                    ItemResponse<FilmImage> movieiResponse =
                        await this.container.DeleteItemAsync<FilmImage>(id, partitionKey);
                    Console.WriteLine("Deleted id {0}\n", id);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("{0} was yet deleted!", id);
                }
            }
        }
        // </DeleteFamilyItemAsync>
    }
}
        
