using System;
using System.Collections.Generic;
using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    [ZeroFormattable]
    public class AcqDealRightsEntity
    {
            [Index(0)]
            public virtual List<Country> CountryList { get; set; }
            [Index(1)]
            public virtual List<Platform> PlatformList { get; set; }
            [Index(2)]
            public virtual List<SubTitle> SubTitleList { get; set; }
            [Index(3)]
            public virtual List<Dubbing> DubbingList { get; set; }
            [Index(4)]
            public virtual OtherData OtherData { get; set; }
            [Index(5)]
            public virtual List<Episode> EpisodeList { get; set; }
    }
}
