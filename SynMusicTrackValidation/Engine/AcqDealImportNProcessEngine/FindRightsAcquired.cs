using SynMusicRightsValidation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcqDealImportNProcessEngine
{
    class FindRightsAcquired
    {
        public List<SynDealRightsEntity> CheckAcquisition(List<AcqDealRightsEntity> lstAcqDealRightsEntity, SynDealRightsEntity objSynDealRight)
        {
            List<SynDealRightsEntity> lstSynRightsNotAcquired = new List<SynDealRightsEntity>();
            List<SynDealRightsEntity> lstSynRights = new List<SynDealRightsEntity>();
            List<DateRange> lstDateRange = new List<DateRange>();

            //lstSynRights.Add(objSynDealRight);
            //int ProcessCount = 1;

            List<DateTime> lstDate = new List<DateTime>();
            lstDate.Add(objSynDealRight.OtherData.ActualRightStartDate);
            lstDate.Add(objSynDealRight.OtherData.ActualRightEndDate.Value.AddDays(1));

            foreach (AcqDealRightsEntity objAcqDealRightsEntity in lstAcqDealRightsEntity)
            {
                if (!lstDate.Contains(objAcqDealRightsEntity.OtherData.ActualRightStartDate) && objAcqDealRightsEntity.OtherData.ActualRightStartDate > objSynDealRight.OtherData.ActualRightStartDate)
                {
                    lstDate.Add(objAcqDealRightsEntity.OtherData.ActualRightStartDate);
                }
                if (!lstDate.Contains(objAcqDealRightsEntity.OtherData.ActualRightEndDate.Value) && objAcqDealRightsEntity.OtherData.ActualRightEndDate.Value < objSynDealRight.OtherData.ActualRightEndDate.Value)
                {
                    lstDate.Add(objAcqDealRightsEntity.OtherData.ActualRightEndDate.Value.AddDays(1));
                }
            }

            lstDate = lstDate.OrderBy(item => item.Date).ToList();
            for (int i = 0; i < (lstDate.Count - 1); i++)
            {
                DateRange objRange = new DateRange();
                objRange.StartDate = lstDate[i];
                objRange.EndDate = lstDate[i + 1].AddDays(-1);
                lstDateRange.Add(objRange);
            }

            objSynDealRight.IsProcessed = "N";
            objSynDealRight.ValidationStatus = "P";
            foreach (DateRange objDateRange in lstDateRange)
            {
                SynDealRightsEntity objSynDealRightNeo = objSynDealRight.CreateCopy();
                objSynDealRightNeo.OtherData.ActualRightStartDate = objDateRange.StartDate;
                objSynDealRightNeo.OtherData.ActualRightEndDate = objDateRange.EndDate;
                lstSynRights.Add(objSynDealRightNeo);
            }

            foreach (SynDealRightsEntity objSynDealRightNeo in lstSynRights)
            {
                OtherData objSynData = objSynDealRightNeo.OtherData;
                objSynDealRightNeo.IsProcessed = "Y";
                objSynDealRightNeo.ValidationStatus = "N";

                List<Platform> lstNAPlatforms = objSynDealRightNeo.PlatformList;
                List<Country> lstNACountry = objSynDealRightNeo.CountryList;
                List<SubTitle> lstNASubtitling = objSynDealRightNeo.SubTitleList;
                List<Dubbing> lstNADubbing = objSynDealRightNeo.DubbingList;

                int counter = 0;
                foreach (var objAcqDealRightsEntity in lstAcqDealRightsEntity)
                {
                    if (objAcqDealRightsEntity.OtherData.ActualRightStartDate <= objSynData.ActualRightStartDate                //01-Jan-24 (A) <= 01-Sep-24 (S)
                       && objAcqDealRightsEntity.OtherData.ActualRightEndDate.Value >= objSynData.ActualRightEndDate.Value)     //31-Dec-25 (A) >= 31-Aug-25 (S)
                    {
                        lstNAPlatforms = CheckPlatforms(objSynDealRightNeo.PlatformList, objAcqDealRightsEntity.PlatformList);
                        lstNACountry = CheckCountries(objSynDealRightNeo.CountryList, objAcqDealRightsEntity.CountryList);
                        lstNASubtitling = CheckSubtitlings(objSynDealRightNeo.SubTitleList, objAcqDealRightsEntity.SubTitleList);
                        lstNADubbing = CheckDubbings(objSynDealRightNeo.DubbingList, objAcqDealRightsEntity.DubbingList);
                        counter += 1;
                    }
                }

                if (counter == 0)
                {
                    SynDealRightsEntity objSynRightsNA = new SynDealRightsEntity();
                    objSynRightsNA.PlatformList = lstNAPlatforms;
                    objSynRightsNA.CountryList = lstNACountry;
                    objSynRightsNA.SubTitleList = lstNASubtitling;
                    objSynRightsNA.DubbingList = lstNADubbing;
                    objSynRightsNA.OtherData = objSynDealRightNeo.OtherData;
                    //objSynRightsNA.IsProcessed = "N";
                    objSynRightsNA.ValidationStatus = "E";
                    objSynRightsNA.ErrorMessage = "Combination is not Acquired for the License Period";
                    lstSynRightsNotAcquired.Add(objSynRightsNA);
                }
                else
                {
                    if (lstNAPlatforms.Count > 0)
                    {
                        SynDealRightsEntity objSynRightsNA = new SynDealRightsEntity();
                        objSynRightsNA.PlatformList = lstNAPlatforms;
                        objSynRightsNA.OtherData = objSynDealRightNeo.OtherData;
                        objSynRightsNA.ValidationStatus = "E";
                        objSynRightsNA.ErrorMessage = "Platform Not Acquired";
                        lstSynRightsNotAcquired.Add(objSynRightsNA);
                    }

                    if (lstNACountry.Count > 0)
                    {
                        SynDealRightsEntity objSynRightsNA = new SynDealRightsEntity();
                        objSynRightsNA.PlatformList = CheckPlatforms(objSynDealRightNeo.PlatformList, lstNAPlatforms);
                        objSynRightsNA.CountryList = lstNACountry;
                        objSynRightsNA.OtherData = objSynDealRightNeo.OtherData;
                        objSynRightsNA.ValidationStatus = "E";
                        objSynRightsNA.ErrorMessage = "Country Not Acquired";
                        lstSynRightsNotAcquired.Add(objSynRightsNA);
                    }

                    if (lstNASubtitling.Count > 0 || lstNADubbing.Count > 0)
                    {
                        SynDealRightsEntity objSynRightsNA = new SynDealRightsEntity();
                        objSynRightsNA.PlatformList = CheckPlatforms(objSynDealRightNeo.PlatformList, lstNAPlatforms);
                        objSynRightsNA.CountryList = CheckCountries(objSynDealRightNeo.CountryList, lstNACountry);
                        objSynRightsNA.SubTitleList = lstNASubtitling;
                        objSynRightsNA.DubbingList = lstNADubbing;
                        objSynRightsNA.OtherData = objSynDealRightNeo.OtherData;
                        //objSynRightsNA.IsProcessed = "N";
                        objSynRightsNA.ValidationStatus = "E";
                        objSynRightsNA.ErrorMessage = "Subtitling or Dubbing Language Not Acquired";
                        lstSynRightsNotAcquired.Add(objSynRightsNA);
                    }

                    if (objSynData.IsTitleLanguageRight == "Y")
                    {
                        lstNAPlatforms = CheckPlatforms(objSynDealRightNeo.PlatformList, lstNAPlatforms);
                        lstNACountry = CheckCountries(objSynDealRightNeo.CountryList, lstNACountry);
                        foreach (var objAcqDealRightsEntity in lstAcqDealRightsEntity)
                        {
                            if (objAcqDealRightsEntity.OtherData.ActualRightStartDate <= objSynData.ActualRightStartDate                //01-Jan-24 (A) <= 01-Sep-24 (S)
                               && objAcqDealRightsEntity.OtherData.ActualRightEndDate.Value >= objSynData.ActualRightEndDate.Value      //31-Dec-25 (A) >= 31-Aug-25 (S)
                               && objAcqDealRightsEntity.OtherData.IsTitleLanguageRight == "Y")
                            {
                                lstNAPlatforms = CheckPlatforms(lstNAPlatforms, objAcqDealRightsEntity.PlatformList);
                                lstNACountry = CheckCountries(lstNACountry, objAcqDealRightsEntity.CountryList);
                            }
                        }
                        if (lstNAPlatforms.Count > 0 || lstNACountry.Count > 0)
                        {
                            SynDealRightsEntity objSynRightsNA = new SynDealRightsEntity();
                            objSynRightsNA.PlatformList = lstNAPlatforms;
                            objSynRightsNA.CountryList = lstNACountry;
                            objSynRightsNA.OtherData = objSynDealRightNeo.OtherData;
                            //objSynRightsNA.OtherData.IsTitleLanguageRight = "N";
                            //objSynRightsNA.IsProcessed = "N";
                            objSynRightsNA.ValidationStatus = "E";
                            objSynRightsNA.ErrorMessage = "Title Language Not Acquired";
                            lstSynRightsNotAcquired.Add(objSynRightsNA);
                        }
                    }
                }
            }

            return lstSynRightsNotAcquired;
        }

        #region private methods

        public List<MusicError> GroupErrorMessage(int synDealMusicTrackId, List<SynDealRightsEntity> lstSynRightsNotAcquired)
        {
            List<MusicError> lstMusicErrorFinal = new List<MusicError>();
            List<MusicError> lstMusicError = new List<MusicError>();
            lstSynRightsNotAcquired = lstSynRightsNotAcquired.OrderBy(item => item.ErrorMessage).ThenBy(item => item.OtherData.ActualRightStartDate).ToList();

            foreach (SynDealRightsEntity objSynRightsEntity in lstSynRightsNotAcquired)
            {
                MusicError objMusicError = new MusicError();
                objMusicError.Syn_Music_Track_Code = synDealMusicTrackId;
                if (objSynRightsEntity.PlatformList != null)
                    objMusicError.PlatformIDs = string.Join(",", objSynRightsEntity.PlatformList.Select(x => x.PlatformCode));
                else
                    objMusicError.PlatformIDs = "";

                if (objSynRightsEntity.CountryList != null)
                    objMusicError.CountryIDs = string.Join(",", objSynRightsEntity.CountryList.Select(x => x.CountryCode));
                else
                    objMusicError.CountryIDs = "";

                if (objSynRightsEntity.SubTitleList != null)
                    objMusicError.SubtitlingIDs = string.Join(",", objSynRightsEntity.SubTitleList.Select(x => x.SubTitleCode));
                else
                    objMusicError.SubtitlingIDs = "";

                if (objSynRightsEntity.DubbingList != null)
                    objMusicError.DubbingIDs = string.Join(",", objSynRightsEntity.DubbingList.Select(x => x.DubbingCode));
                else
                    objMusicError.DubbingIDs = "";

                if (objSynRightsEntity.OtherData.ActualRightStartDate == DateTime.MinValue)
                    objMusicError.StartDate = "";
                else
                    objMusicError.StartDate = objSynRightsEntity.OtherData.ActualRightStartDate.ToString("dd-MMM-yyyy");

                if (objSynRightsEntity.OtherData.ActualRightEndDate == null)
                    objMusicError.EndDate = "";
                else
                    objMusicError.EndDate = objSynRightsEntity.OtherData.ActualRightEndDate.Value.ToString("dd-MMM-yyyy");

                objMusicError.IsSubLicensing = objSynRightsEntity.OtherData.IsSubLicense;
                objMusicError.IsTitleLanguageRight = objSynRightsEntity.OtherData.IsTitleLanguageRight;
                objMusicError.ErrorMessage = objSynRightsEntity.ErrorMessage;

                lstMusicError.Add(objMusicError);
            }

            var lstGrp = lstMusicError.GroupBy(gb => new { gb.StartDate, gb.EndDate, gb.ErrorMessage })
                                      .Select(mer => new MusicError()
                                      {
                                          StartDate = mer.Key.StartDate,
                                          EndDate = mer.Key.EndDate,
                                          ErrorMessage = mer.Key.ErrorMessage
                                      });

            foreach (MusicError objMEGrp in lstGrp)
            {
                var lstResult = lstMusicError.Where(x => x.StartDate == objMEGrp.StartDate && x.EndDate == objMEGrp.EndDate && x.ErrorMessage == objMEGrp.ErrorMessage)
                                             .Select(mer => new MusicError()
                                             {
                                                 PlatformIDs = mer.PlatformIDs,
                                                 CountryIDs = mer.CountryIDs,
                                                 SubtitlingIDs = mer.SubtitlingIDs,
                                                 DubbingIDs = mer.DubbingIDs,
                                                 IsSubLicensing = mer.IsSubLicensing,
                                                 IsTitleLanguageRight = mer.IsTitleLanguageRight,
                                             });

                MusicError objMusicError = new MusicError();
                objMusicError.Syn_Music_Track_Code = synDealMusicTrackId;
                objMusicError.StartDate = objMEGrp.StartDate;
                objMusicError.EndDate = objMEGrp.EndDate;
                objMusicError.ErrorMessage = objMEGrp.ErrorMessage;

                objMusicError.PlatformIDs = string.Join(",", lstResult.ToList<MusicError>().Select(x => x.PlatformIDs));
                objMusicError.CountryIDs = string.Join(",", lstResult.ToList<MusicError>().Select(x => x.CountryIDs));
                objMusicError.SubtitlingIDs = string.Join(",", lstResult.ToList<MusicError>().Select(x => x.SubtitlingIDs));
                objMusicError.DubbingIDs = string.Join(",", lstResult.ToList<MusicError>().Select(x => x.DubbingIDs));
                objMusicError.IsSubLicensing = lstResult.ToList<MusicError>().Select(x => x.IsSubLicensing).FirstOrDefault();
                objMusicError.IsTitleLanguageRight = lstResult.ToList<MusicError>().Select(x => x.IsTitleLanguageRight).FirstOrDefault();

                lstMusicErrorFinal.Add(objMusicError);
            }

            return lstMusicErrorFinal;
        }

        private List<Platform> CheckPlatforms(List<Platform> lstSynSource, List<Platform> lstAcqTarget)
        {
            //lstSynSource.Except(lstAcqTarget, (lstSynSource, lstAcqTarget) => lstSynSource.key.CompareTo(b.key));

            var ids = lstSynSource.Select(x => x.PlatformCode).Intersect(lstAcqTarget.Select(x => x.PlatformCode));
            return lstSynSource.Where(x => !ids.Contains(x.PlatformCode)).ToList();

            //return lstSynSource.Except(lstAcqTarget).ToList();

            //bool flag = false;
            //bool isSubSet = false;
            //bool isEqual = false;

            //isSubSet = lstTarget.All(x => lstSource.Contains(x));
            //isEqual = lstTarget.All(x => lstSource.Exists(y => y.PlatformCode == x.PlatformCode));

            //if (isSubSet || isEqual)
            //{
            //    //objRight.PlatformList = lstSource.Intersect(lstTarget).ToList();
            //    var ids = lstSource.Select(x => x.PlatformCode).Intersect(lstTarget.Select(x => x.PlatformCode));
            //    objRight.PlatformList = lstSource.Where(x => ids.Contains(x.PlatformCode)).ToList();
            //    flag = true;
            //}

            //return flag;
        }

        private List<Country> CheckCountries(List<Country> lstSynSource, List<Country> lstAcqTarget)
        {
            //bool flag = false;
            //bool isSubSet = false;

            //isSubSet = lstTarget.All(x => lstSource.Exists(y => y.CountryCode == x.CountryCode));

            //if (isSubSet)
            //{
            //    //objRight.CountryList = lstSource.Intersect(lstTarget).ToList();
            //    var ids = lstSource.Select(x => x.CountryCode).Intersect(lstTarget.Select(x => x.CountryCode));
            //    objRight.CountryList = lstSource.Where(x => ids.Contains(x.CountryCode)).ToList();

            //    flag = true;
            //}

            //return flag;

            //return lstSynSource.Except(lstAcqTarget).ToList();
            var ids = lstSynSource.Select(x => x.CountryCode).Intersect(lstAcqTarget.Select(x => x.CountryCode));
            return lstSynSource.Where(x => !ids.Contains(x.CountryCode)).ToList();

        }

        private static bool IsSubsetOrEqualEpisode(List<Episode> lstSource, List<Episode> lstTarget)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = lstTarget.All(x => lstSource.Exists(y => y.EpisodeFrom <= x.EpisodeFrom && y.EpisodeTo >= x.EpisodeTo));

            if (isSubSet)
                flag = true;

            return flag;
        }

        private List<SubTitle> CheckSubtitlings(List<SubTitle> lstSynSource, List<SubTitle> lstAcqTarget)
        {
            //bool flag = false;
            //bool isSubSet = false;

            //isSubSet = lstTarget.All(x => lstSource.Exists(y => y.SubTitleCode == x.SubTitleCode));

            //if (isSubSet && lstTarget.Count > 0 && lstSource.Count > 0)
            //{
            //    //objRight.SubTitleList = lstSource.Intersect(lstTarget).ToList();
            //    var ids = lstSource.Select(x => x.SubTitleCode).Intersect(lstTarget.Select(x => x.SubTitleCode));
            //    objRight.SubTitleList = lstSource.Where(x => ids.Contains(x.SubTitleCode)).ToList();
            //    flag = true;
            //}
            //return flag;

            //return lstSynSource.Except(lstAcqTarget).ToList();

            var ids = lstSynSource.Select(x => x.SubTitleCode).Intersect(lstAcqTarget.Select(x => x.SubTitleCode));
            return lstSynSource.Where(x => !ids.Contains(x.SubTitleCode)).ToList();
        }

        private List<Dubbing> CheckDubbings(List<Dubbing> lstSynSource, List<Dubbing> lstAcqTarget)
        {
            //bool flag = false;
            //bool isSubSet = false;

            //isSubSet = lstTarget.All(x => lstSource.Exists(y => y.DubbingCode == x.DubbingCode));

            //if (isSubSet && lstTarget.Count > 0 && lstSource.Count > 0)
            //{
            //    //objRight.DubbingList = lstSource.Intersect(lstTarget).ToList();
            //    var ids = lstSource.Select(x => x.DubbingCode).Intersect(lstTarget.Select(x => x.DubbingCode));
            //    objRight.DubbingList = lstSource.Where(x => ids.Contains(x.DubbingCode)).ToList();
            //    flag = true;
            //}

            //return flag;

            //return lstSynSource.Except(lstAcqTarget).ToList();

            var ids = lstSynSource.Select(x => x.DubbingCode).Intersect(lstAcqTarget.Select(x => x.DubbingCode));
            return lstSynSource.Where(x => !ids.Contains(x.DubbingCode)).ToList();
        }

        //private static bool IsSubsetOrEqualLicensePeriod(DateTime lstSourceStartFrom, DateTime? lstSourceEndTo, DateTime checkingWithStartFrom, DateTime? checkingWithEndTo, Right objRight)
        //{
        //    bool flag = false;
        //    bool isSubSet = false;
        //    bool isEqual = false;
        //    LicensePeriod lc = new LicensePeriod();

        //    if (lstSourceEndTo != null && checkingWithEndTo != null)
        //    {
        //        if (lstSourceStartFrom <= checkingWithStartFrom && lstSourceEndTo >= checkingWithEndTo)
        //        {
        //            lc.LicensePeriodStartFrom = checkingWithStartFrom;
        //            lc.LicensePeriodEndTo = checkingWithEndTo;
        //            objRight.LicensePeriod = lc;
        //            isSubSet = true;
        //        }
        //        if (lstSourceStartFrom <= checkingWithStartFrom && lstSourceEndTo <= checkingWithEndTo && lstSourceEndTo >= checkingWithStartFrom)
        //        {
        //            lc.LicensePeriodStartFrom = checkingWithStartFrom;
        //            lc.LicensePeriodEndTo = checkingWithEndTo;
        //            objRight.LicensePeriod = lc;
        //            isSubSet = true;
        //        }
        //        if (lstSourceStartFrom >= checkingWithStartFrom && lstSourceEndTo >= checkingWithEndTo)
        //        {
        //            lc.LicensePeriodStartFrom = checkingWithStartFrom;
        //            lc.LicensePeriodEndTo = checkingWithEndTo;
        //            objRight.LicensePeriod = lc;
        //            isSubSet = true;
        //        }
        //        if (lstSourceStartFrom <= checkingWithStartFrom && lstSourceEndTo <= checkingWithEndTo && checkingWithStartFrom <= lstSourceEndTo)
        //        {
        //            lc.LicensePeriodStartFrom = checkingWithStartFrom;
        //            lc.LicensePeriodEndTo = checkingWithEndTo;
        //            objRight.LicensePeriod = lc;
        //            isSubSet = true;
        //        }
        //        if (checkingWithStartFrom < lstSourceStartFrom && lstSourceEndTo < checkingWithEndTo)
        //        {
        //            lc.LicensePeriodStartFrom = checkingWithStartFrom;
        //            lc.LicensePeriodEndTo = checkingWithEndTo;
        //            objRight.LicensePeriod = lc;
        //            isSubSet = true;
        //        }
        //    }
        //    else
        //    {
        //        if (lstSourceStartFrom <= checkingWithStartFrom && lstSourceEndTo == null)
        //        {
        //            lc.LicensePeriodStartFrom = checkingWithStartFrom;
        //            lc.LicensePeriodEndTo = checkingWithEndTo;
        //            objRight.LicensePeriod = lc;
        //            isSubSet = true;
        //        }
        //    }

        //    if (lstSourceStartFrom == checkingWithStartFrom && lstSourceEndTo == checkingWithEndTo)
        //    {
        //        lc.LicensePeriodStartFrom = checkingWithStartFrom;
        //        lc.LicensePeriodEndTo = checkingWithEndTo;
        //        objRight.LicensePeriod = lc;
        //        isEqual = true;
        //    }

        //    if (isSubSet || isEqual)
        //        flag = true;

        //    return flag;
        //}

        #endregion

    }

    class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
