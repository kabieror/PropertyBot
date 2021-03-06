﻿using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PropertyBot.Common.Settings
{
    public class SettingsReader<TSetting> : ISettingsReader<TSetting>
    {
        public async Task<SettingsContainer<TSetting>> ReadSettings(string path)
        {
            var fileContent = await File.ReadAllTextAsync($"settings/{path}");
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            return deserializer.Deserialize<SettingsContainer<TSetting>>(fileContent);
        }
    }
}
