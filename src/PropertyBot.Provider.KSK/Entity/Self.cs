﻿using System.Text.Json.Serialization;

namespace PropertyBot.Provider.KSK.Entity
{
    public class Self    {
        [JsonPropertyName("href")]
        public string Href { get; set; } 
    }
}
