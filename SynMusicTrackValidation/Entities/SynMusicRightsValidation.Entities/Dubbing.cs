using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    [ZeroFormattable]
    public class Dubbing
    {
        [Index(0)]
        public virtual int DubbingCode { get; set; }
    }
}
