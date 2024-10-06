using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    [ZeroFormattable]
    public class SubTitle
    {
        [Index(0)]
        public virtual int SubTitleCode { get; set; }
    }
}
