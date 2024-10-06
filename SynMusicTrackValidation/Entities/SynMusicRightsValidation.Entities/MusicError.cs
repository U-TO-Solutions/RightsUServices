using System;
using System.Collections.Generic;
using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    //[ZeroFormattable]
    public class MusicError
    {
        //[Index(0)]
        public virtual int Syn_Music_Track_Code { get; set; }
        //[Index(1)]
        public virtual string PlatformIDs { get; set; }
        //[Index(2)]
        public virtual string CountryIDs { get; set; }
        //[Index(3)]
        public virtual string SubtitlingIDs { get; set; }
        //[Index(4)]
        public virtual string DubbingIDs { get; set; }
        //[Index(5)]
        public virtual string StartDate { get; set; }
        //[Index(6)]
        public virtual string EndDate { get; set; }
        //[Index(7)]
        public virtual string IsSubLicensing { get; set; }
        //[Index(8)]
        public virtual string IsTitleLanguageRight { get; set; }
        //[Index(9)]
        public virtual string ErrorMessage { get; set; }
    }
}
