﻿using System;
using System.Threading.Tasks;
using PropertyBot.Provider.KSK.WebClient;

namespace PropertyBot.Provider.KSK
{
    public static class Program
    {
        public static async Task Main(string[] parameters)
        {
            var client = new KskClient();
            var webClient = new KskWebClient();
            var objects = await webClient.GetZvgObjects(new KskWebClientOptions());
        }
    }
}
