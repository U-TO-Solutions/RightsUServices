using ZeroFormatter;

namespace AcqRightsValidation.Entities
{
    [ZeroFormattable]
    public class Platform
    {
        [Index(0)]
        public virtual int PlatformCode { get; set; }
    }
}
