﻿using System.Collections.Generic;

namespace PropertyBot.Common
{
    public class SettingsContainer<TSetting>
    {
        public IEnumerable<TSetting> Settings { get; set; }
    }
}