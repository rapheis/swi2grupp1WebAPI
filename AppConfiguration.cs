namespace swi2grupp1WebAPI
{
    public static class AppConfiguration
    {
        public static string AzureServiceBusConnectionString { get; set; }
        public static string AzureServiceBusCommand { get; set; }
        public static string AzureServiceBusQuery { get; set; }
        public static string AzureServiceBusResponse { get; set; }

        // Konstanten für CosomosDB
        public static string CosmosEndpointUri { get; set; }
        public static string CosmosPrimaryKey { get; set; }

    }
}
