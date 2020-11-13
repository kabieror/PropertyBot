﻿using System.Collections.Generic;
using PropertyBot.Interface;
using PropertyBot.Provider.KSK.Entity;

namespace PropertyBot.Provider.KSK.Converter
{
    public interface IKskEstateConverter
    {
        public IEnumerable<Property> ToProperties(IEnumerable<Estate> estates);
    }
}
