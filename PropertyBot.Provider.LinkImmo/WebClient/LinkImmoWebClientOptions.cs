﻿using System.Collections.Generic;

namespace PropertyBot.Provider.LinkImmo.WebClient
{
    internal class LinkImmoWebClientOptions
    {
        internal LinkImmoWebClientOptions(int zipRadiusSearch, int perimeterInKm, int limit, long regioClientId, IEnumerable<string> marketingUsageObjectType)
        {
            ZipRadiusSearch = zipRadiusSearch;
            PerimeterInKm = perimeterInKm;
            Limit = limit;
            RegioClientId = regioClientId;
            MarketingUsageObjectType = marketingUsageObjectType;
        }

        public int ZipRadiusSearch { get; }

        public int PerimeterInKm { get; }

        public int Limit { get; }

        public long RegioClientId { get; }

        public IEnumerable<string> MarketingUsageObjectType { get; }


    }
}
