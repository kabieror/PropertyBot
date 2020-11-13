﻿namespace PropertyBot.Provider.KSK.WebClient
{
    public class KskWebClientOptions
    {
        public int Zip { get; set; }

        public int RadiusInKm { get; set; }

        public int Limit { get; set; }

        public long RegioClientId { get; set; }

        public string MarketingUsageObjectType { get; set; }
    }
}
