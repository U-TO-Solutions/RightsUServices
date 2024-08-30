using ZeroFormatter;

namespace AcqRightsValidation.Entities
{
    [ZeroFormattable]
    public class SubTitle
    {
        [Index(0)]
        public virtual int SubTitleCode { get; set; }
    }
}
