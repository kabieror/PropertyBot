﻿using PropertyBot.Common;
using PropertyBot.Common.Settings;
using PropertyBot.Interface;
using PropertyBot.Provider.ImmoscoutLists.Converter;
using PropertyBot.Provider.ImmoscoutLists.WebClient;

namespace PropertyBot.Provider.ImmoscoutLists
{
    public class ImmoscoutListProviderFactory
    {
        public static IPropertyProvider CreateProvider()
        {
            var webClient = new ImmoscoutListWebClient();
            var converter = new ImmoscoutListConverter();
            var settingsReader = new SettingsReader<ImmoscoutListWebClientOptions>();

            return new ImmoscoutListClient(webClient, converter, settingsReader);
        }
    }
}
