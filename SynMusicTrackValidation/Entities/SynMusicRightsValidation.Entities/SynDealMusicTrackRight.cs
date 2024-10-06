﻿using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    [ZeroFormattable]
    public class SynDealMusicTrackRight
    {
        [Index(0)]
        public virtual int RightsCode { get; set; }

        [Index(1)]
        public virtual int TitleCode { get; set; }
    }
}
