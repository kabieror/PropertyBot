﻿using System;
using System.Collections.Generic;
using System.Linq;
using PropertyBot.Common;
using PropertyBot.Interface;
using PropertyBot.Provider.VolksbankEnz.Entity;

namespace PropertyBot.Provider.VolksbankEnz.Converter
{
    internal class VolksbankConverter : IVolksbankConverter
    {
        public IEnumerable<Property> ToProperties(IEnumerable<VolksbankProperty> volksbankProperties)
        {
            return volksbankProperties.Select(ToProperty);
        }

        private Property ToProperty(VolksbankProperty volksbankProperty)
        {
            var details = new Dictionary<string, string>
            {
                {"Ort", volksbankProperty.Location},
                {"Zimmer", volksbankProperty.RoomCount.Format()},
                {"Wohnfläche", $"{volksbankProperty.LivingArea.Format()} m²"}
            };

            return new Property(
                volksbankProperty.Id,
                volksbankProperty.Description,
                volksbankProperty.ImageUri,
                DateTime.Now, 
                volksbankProperty.Price,
                details,
                new Uri($"https://60491430.flowfact-webparts.net/index.php/estates/{volksbankProperty.Id}"), 
                MessageFormat.Html,
                "Volksbank Neckar-Enz");
        }
    }
}
