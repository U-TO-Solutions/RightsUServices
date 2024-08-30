using ZeroFormatter;

namespace AcqRightsValidation.Entities
{
    [ZeroFormattable]
    public class Episode
    {
        [Index(0)]
        public virtual int EpisodeFrom { get; set; }
        [Index(1)]
        public virtual int EpisodeTo { get; set; }

    }
}
