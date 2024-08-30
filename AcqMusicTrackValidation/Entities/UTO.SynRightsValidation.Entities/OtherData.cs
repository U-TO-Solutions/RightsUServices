using System;
using ZeroFormatter;
namespace AcqRightsValidation.Entities
{
    [ZeroFormattable]
    public class OtherData
    {
        [Index(0)]
        public virtual string IsExclusive { get; set; }
        [Index(1)]
        public virtual string IsTitleLanguageRight { get; set; }
        [Index(2)]
        public virtual string IsSubLicense { get; set; }
        [Index(3)]
        public virtual int SubLicensing { get; set; }
        [Index(4)]
        public virtual string IsTheatricalRight { get; set; }
        [Index(5)]
        public virtual string IsTentative { get; set; }
        [Index(6)]
        public virtual DateTime ActualRightStartDate { get; set; }
        [Index(7)]
        public virtual DateTime? ActualRightEndDate { get; set; }
        [Index(8)]
        public virtual DateTime? ROFR { get; set; }
        [Index(9)]
        public virtual string SelfUtilizationGroup { get; set; }
        [Index(10)]
        public virtual string SelfUtilizationRemarks { get; set; }
        [Index(11)]
        public virtual long RightCode { get; set; }
    }
}