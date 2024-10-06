using System;
using System.Collections.Generic;
using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    [ZeroFormattable]
    public class Right
    {
        [Index(0)]
        public virtual List<Country> CountryList { get; set; }
        [Index(1)]
        public virtual List<Platform> PlatformList { get; set; }
        [Index(2)]
        public virtual LicensePeriod LicensePeriod { get; set; }
        [Index(3)]
        public virtual List<SubTitle> SubTitleList { get; set; }
        [Index(4)]
        public virtual List<Dubbing> DubbingList { get; set; }
        [Index(5)]
        public virtual bool IsTitleLanguage { get; set; }
        [Index(6)]
        public virtual bool IsSubLicensing { get; set; }
        [Index(7)]
        public virtual bool IsSelfConsumption { get; set; }
        [Index(8)]
        public virtual bool IsExclusive { get; set; }
        [Index(9)]
        public virtual bool IsNonExclusive { get; set; }
        [Index(10)]
        public virtual int SubLicensing { get; set; }
        [Index(11)]
        public virtual bool IsTheatrical { get; set; }
        [Index(12)]
        public virtual DateTime? ROFR { get; set; }
        [Index(13)]
        public virtual string SelfUtilizationGroup { get; set; }
        [Index(14)]
        public virtual string SelfUtilizationRemarks { get; set; }
        [Index(15)]
        public virtual List<Episode> EpisodeList { get; set; }
        [Index(16)]
        public virtual long RightCode { get; set; }
        [Index(17)]
        public virtual bool IsCoExclusive { get; set; }
    }
}
