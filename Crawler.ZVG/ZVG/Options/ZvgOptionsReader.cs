﻿using System;
using Crawler.ZVG.ZVG.WebClient;

namespace Crawler.ZVG.ZVG.Options
{
    public class ZvgOptionsReader : IZvgOptionsReader
    {
        public ZvgWebClientOptions GetWebClientOptions()
        {
            var stateId = Environment.GetEnvironmentVariable("PROVIDER_ZVG_STATE_ID");
            var courtIds = Environment.GetEnvironmentVariable("PROVIDER_ZVG_COURT_IDS");
            var objectKindIds = Environment.GetEnvironmentVariable("PROVIDER_ZVG_OBJECT_KIND_ID");

            return new ZvgWebClientOptions(stateId, courtIds, objectKindIds);
        }
    }
}
