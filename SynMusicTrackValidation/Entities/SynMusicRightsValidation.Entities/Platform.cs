using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    [ZeroFormattable]
    public class Platform
    {
        [Index(0)]
        public virtual int PlatformCode { get; set; }
    }
}
