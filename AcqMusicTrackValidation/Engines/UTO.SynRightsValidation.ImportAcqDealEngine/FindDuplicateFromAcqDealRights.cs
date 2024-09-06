using AcqRightsValidation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AcqRightsValidation.AcqDealImportEngine
{
    public class FindDuplicateFromAcqDealRights
    {
        #region public variables
        public static List<Right> objRightsList;
        public static bool IsSubsetOrEqualCountryVar = false;
        public static bool IsSubsetOrEqualPlatformvar = false;
        public static bool IsSubsetOrEqualLicensePeriodvar = false;
        public static bool IsSubsetOrEqualSubTitlevar = false;
        public static bool IsSubsetOrEqualDubbingvar = false;
        #endregion
        #region public methods
        public List<Right> FindDuplicates(List<AcqDealRightsEntity> acquisitionDealRightsEntities, int musicRightsCode)
        {
            objRightsList = new List<Right>();

            List<AcqDealRightsEntity> acquisitionDealRightsEntityNotDistinct = acquisitionDealRightsEntities.Where(a => a.OtherData.RightCode != musicRightsCode).ToList();

            AcqDealRightsEntity currentRight = acquisitionDealRightsEntities.Where(a => a.OtherData.RightCode == musicRightsCode).FirstOrDefault();

            foreach (var acquisitionDealRightsEntity in acquisitionDealRightsEntityNotDistinct)
            {
                Right objRight = new Right();
                objRight.RightCode = 0;

                IsSubsetOrEqualCountryVar = IsSubsetOrEqualCountry(currentRight.CountryList, acquisitionDealRightsEntity.CountryList, objRight);
                IsSubsetOrEqualPlatformvar = IsSubsetOrEqualPlatform(currentRight.PlatformList, acquisitionDealRightsEntity.PlatformList, objRight);
                IsSubsetOrEqualLicensePeriodvar = IsSubsetOrEqualLicensePeriod(currentRight.OtherData.ActualRightStartDate, currentRight.OtherData.ActualRightEndDate,
                    acquisitionDealRightsEntity.OtherData.ActualRightStartDate, acquisitionDealRightsEntity.OtherData.ActualRightEndDate, objRight);
                IsSubsetOrEqualSubTitlevar = IsSubsetOrEqualSubTitle(currentRight.SubTitleList, acquisitionDealRightsEntity.SubTitleList, objRight);
                IsSubsetOrEqualDubbingvar = IsSubsetOrEqualDubbing(currentRight.DubbingList, acquisitionDealRightsEntity.DubbingList, objRight);

                //Platform, Country, Exclusivity, License_Period, Title_Language_Rights
                //Platform, Country, Exclusivity, License_Period, Subtitling_Language
                //Platform, Country, Exclusivity, License_Period, Dubbing_Language
                if ((IsSubsetOrEqualCountryVar && IsSubsetOrEqualPlatformvar && IsSubsetOrEqualLicensePeriodvar &&
                    acquisitionDealRightsEntity.OtherData.IsTitleLanguageRight == currentRight.OtherData.IsTitleLanguageRight &&
                    acquisitionDealRightsEntity.OtherData.IsExclusive == currentRight.OtherData.IsExclusive ) || 
                    (IsSubsetOrEqualCountryVar && IsSubsetOrEqualPlatformvar && IsSubsetOrEqualLicensePeriodvar && IsSubsetOrEqualSubTitlevar &&
                    acquisitionDealRightsEntity.OtherData.IsExclusive == currentRight.OtherData.IsExclusive) ||
                    (IsSubsetOrEqualCountryVar && IsSubsetOrEqualPlatformvar && IsSubsetOrEqualLicensePeriodvar && IsSubsetOrEqualDubbingvar &&
                    acquisitionDealRightsEntity.OtherData.IsExclusive == currentRight.OtherData.IsExclusive))
                {
                    objRight.RightCode = acquisitionDealRightsEntity.OtherData.RightCode;
                    objRight.IsExclusive = acquisitionDealRightsEntity.OtherData.IsExclusive == "Y"? true:false;
                    objRight.IsTitleLanguage = acquisitionDealRightsEntity.OtherData.IsTitleLanguageRight == "Y" ? true : false;
                    objRight.IsSubLicensing = acquisitionDealRightsEntity.OtherData.IsSubLicense == "Y" ? true : false;
                    objRightsList.Add(objRight);
                }
                objRight = null;
            }
            return objRightsList;
        }
        #endregion

        #region private methods
        private static bool IsSubsetOrEqualCountry(List<Country> currentList, List<Country> checkingWithList, Right objRight)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = checkingWithList.All(x => currentList.Exists(y => y.CountryCode == x.CountryCode));

            if (isSubSet)
            {
                //objRight.CountryList = currentList.Intersect(checkingWithList).ToList();
                var ids = currentList.Select(x => x.CountryCode).Intersect(checkingWithList.Select(x => x.CountryCode));
                objRight.CountryList = currentList.Where(x => ids.Contains(x.CountryCode)).ToList();

                flag = true;
            }

            return flag;
        }

        private static bool IsSubsetOrEqualEpisode(List<Episode> currentList, List<Episode> checkingWithList)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = checkingWithList.All(x => currentList.Exists(y => y.EpisodeFrom <= x.EpisodeFrom && y.EpisodeTo >= x.EpisodeTo));

            if (isSubSet)
                flag = true;

            return flag;
        }

        private static bool IsSubsetOrEqualPlatform(List<Platform> currentList, List<Platform> checkingWithList, Right objRight)
        {
            bool flag = false;
            bool isSubSet = false;
            bool isEqual = false;

            isSubSet = checkingWithList.All(x => currentList.Contains(x));
            isEqual = checkingWithList.All(x => currentList.Exists(y => y.PlatformCode == x.PlatformCode));

            if (isSubSet || isEqual)
            {
                //objRight.PlatformList = currentList.Intersect(checkingWithList).ToList();
                var ids = currentList.Select(x => x.PlatformCode).Intersect(checkingWithList.Select(x => x.PlatformCode));
                objRight.PlatformList = currentList.Where(x => ids.Contains(x.PlatformCode)).ToList();
                flag = true;
            }

            return flag;
        }

        private static bool IsSubsetOrEqualLicensePeriod(DateTime currentListStartFrom, DateTime? currentListEndTo, DateTime checkingWithStartFrom, DateTime? checkingWithEndTo,Right objRight)
        {
            bool flag = false;
            bool isSubSet = false;
            bool isEqual = false;
            LicensePeriod lc = new LicensePeriod();

            if (currentListEndTo != null && checkingWithEndTo != null)
            {
                if (currentListStartFrom <= checkingWithStartFrom && currentListEndTo >= checkingWithEndTo)
                {
                    lc.LicensePeriodStartFrom = checkingWithStartFrom;
                    lc.LicensePeriodEndTo = checkingWithEndTo;
                    objRight.LicensePeriod = lc;
                    isSubSet = true;
                }
                if (currentListStartFrom <= checkingWithStartFrom && currentListEndTo <= checkingWithEndTo)
                {
                    lc.LicensePeriodStartFrom = checkingWithStartFrom;
                    lc.LicensePeriodEndTo = checkingWithEndTo;
                    objRight.LicensePeriod = lc;
                    isSubSet = true;
                }
                if (currentListStartFrom >= checkingWithStartFrom && currentListEndTo >= checkingWithEndTo)
                {
                    lc.LicensePeriodStartFrom = checkingWithStartFrom;
                    lc.LicensePeriodEndTo = checkingWithEndTo;
                    objRight.LicensePeriod = lc;
                    isSubSet = true;
                }
                if (currentListStartFrom >= checkingWithStartFrom && currentListEndTo <= checkingWithEndTo)
                {
                    lc.LicensePeriodStartFrom = checkingWithStartFrom;
                    lc.LicensePeriodEndTo = checkingWithEndTo;
                    objRight.LicensePeriod = lc;
                    isSubSet = true;
                }
            }
            else
            {
                if (currentListStartFrom <= checkingWithStartFrom && currentListEndTo == null)
                {
                    lc.LicensePeriodStartFrom = checkingWithStartFrom;
                    lc.LicensePeriodEndTo = checkingWithEndTo;
                    objRight.LicensePeriod = lc;
                    isSubSet = true;
                }
            }

            if (currentListStartFrom == checkingWithStartFrom && currentListEndTo == checkingWithEndTo)
            {
                lc.LicensePeriodStartFrom = checkingWithStartFrom;
                lc.LicensePeriodEndTo = checkingWithEndTo;
                objRight.LicensePeriod = lc;
                isEqual = true;
            }

            if (isSubSet || isEqual)
                flag = true;

            return flag;
        }

        private static bool IsSubsetOrEqualSubTitle(List<SubTitle> currentList, List<SubTitle> checkingWithList, Right objRight)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = checkingWithList.All(x => currentList.Exists(y => y.SubTitleCode == x.SubTitleCode));

            if (isSubSet && checkingWithList.Count > 0 && currentList.Count > 0)
            {
                //objRight.SubTitleList = currentList.Intersect(checkingWithList).ToList();
                var ids = currentList.Select(x => x.SubTitleCode).Intersect(checkingWithList.Select(x => x.SubTitleCode));
                objRight.SubTitleList = currentList.Where(x => ids.Contains(x.SubTitleCode)).ToList();
                flag = true;
            }
            return flag;
        }

        private static bool IsSubsetOrEqualDubbing(List<Dubbing> currentList, List<Dubbing> checkingWithList, Right objRight)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = checkingWithList.All(x => currentList.Exists(y => y.DubbingCode == x.DubbingCode));

            if (isSubSet && checkingWithList.Count > 0 && currentList.Count > 0)
            {
                //objRight.DubbingList = currentList.Intersect(checkingWithList).ToList();
                var ids = currentList.Select(x => x.DubbingCode).Intersect(checkingWithList.Select(x => x.DubbingCode));
                objRight.DubbingList = currentList.Where(x => ids.Contains(x.DubbingCode)).ToList();
                flag = true;
            }

            return flag;
        }
        #endregion
    }
}
