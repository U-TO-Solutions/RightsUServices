using ZeroFormatter;

namespace AcqRightsValidation.Entities
{
    [ZeroFormattable]
    public class AcqDealTrackRight
    {
        [Index(0)]
        public virtual int RightsCode { get; set; }

        [Index(1)]
        public virtual int TitleCode { get; set; }
    }
}
