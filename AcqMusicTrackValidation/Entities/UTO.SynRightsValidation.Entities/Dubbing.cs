using ZeroFormatter;

namespace AcqRightsValidation.Entities
{
    [ZeroFormattable]
    public class Dubbing
    {
        [Index(0)]
        public virtual int DubbingCode { get; set; }
    }
}
