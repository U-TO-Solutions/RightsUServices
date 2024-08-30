using System;
using System.Collections.Generic;
using System.Linq;
using AcqRightsValidation.Entities;

namespace AcqRightsValidation.AcqDealImportEngine
{
    public class FindDuplicateFromAcqDealRights
    {
        #region public methods
        public string FindDuplicates(List<AcqDealRightsEntity> acquisitionDealRightsEntities,int musicRightsCode)
        {
            string DuplicateRightsCodeList = "";
            List<AcqDealRightsEntity> acquisitionDealRightsEntityNotDistinct = acquisitionDealRightsEntities.Where(a=> a.OtherData.RightCode != musicRightsCode).ToList();

            AcqDealRightsEntity currentRight  = acquisitionDealRightsEntities.Where(a => a.OtherData.RightCode == musicRightsCode).FirstOrDefault();

            foreach (var acquisitionDealRightsEntity in acquisitionDealRightsEntityNotDistinct)
            {
                
                if (
                    IsSubsetOrEqualCountry(currentRight.CountryList, acquisitionDealRightsEntity.CountryList) &&
                    IsSubsetOrEqualPlatform(currentRight.PlatformList, acquisitionDealRightsEntity.PlatformList) &&
                    IsSubsetOrEqualLicensePeriod(currentRight.OtherData.ActualRightStartDate, currentRight.OtherData.ActualRightEndDate, 
                    acquisitionDealRightsEntity.OtherData.ActualRightStartDate, acquisitionDealRightsEntity.OtherData.ActualRightEndDate) &&
                    IsSubsetOrEqualSubTitle(currentRight.SubTitleList, acquisitionDealRightsEntity.SubTitleList) &&
                    IsSubsetOrEqualDubbing(currentRight.DubbingList, acquisitionDealRightsEntity.DubbingList) &&
                    IsSubsetOrEqualEpisode(currentRight.EpisodeList, acquisitionDealRightsEntity.EpisodeList) &&
                    acquisitionDealRightsEntity.OtherData.IsExclusive == currentRight.OtherData.IsExclusive &&
                    acquisitionDealRightsEntity.OtherData.IsTitleLanguageRight == currentRight.OtherData.IsTitleLanguageRight &&
                    acquisitionDealRightsEntity.OtherData.IsTheatricalRight == currentRight.OtherData.IsTheatricalRight)
                {
                    DuplicateRightsCodeList += acquisitionDealRightsEntity.OtherData.RightCode.ToString() + ",";
                }
            }
            return DuplicateRightsCodeList;
        }
        #endregion

        #region private methods
        private static bool IsSubsetOrEqualCountry(List<Country> currentList, List<Country> checkingWithList)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = checkingWithList.All(x => currentList.Exists(y => y.CountryCode == x.CountryCode));

            if (isSubSet)
                flag = true;

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

        private static bool IsSubsetOrEqualPlatform(List<Platform> currentList, List<Platform> checkingWithList)
        {
            bool flag = false;
            bool isSubSet = false;
            bool isEqual = false;

            isSubSet = checkingWithList.All(x => currentList.Contains(x));
            isEqual = checkingWithList.All(x => currentList.Exists(y => y.PlatformCode == x.PlatformCode));

            if (isSubSet || isEqual)
                flag = true;

            return flag;
        }

        private static bool IsSubsetOrEqualLicensePeriod(DateTime currentListStartFrom, DateTime? currentListEndTo, DateTime checkingWithStartFrom, DateTime? checkingWithEndTo)
        {
            bool flag = false;
            bool isSubSet = false;
            bool isEqual = false;

            if (currentListEndTo != null && checkingWithEndTo != null)
            {
                if (currentListStartFrom <= checkingWithStartFrom && currentListEndTo >= checkingWithEndTo)
                    isSubSet = true;
            }
            else
            {
                if (currentListStartFrom <= checkingWithStartFrom && currentListEndTo == null)
                    isSubSet = true;
            }

            if (currentListStartFrom == checkingWithStartFrom && currentListEndTo == checkingWithEndTo)
                isEqual = true;

            if (isSubSet || isEqual)
                flag = true;

            return flag;
        }

        private static bool IsSubsetOrEqualSubTitle(List<SubTitle> currentList, List<SubTitle> checkingWithList)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = checkingWithList.All(x => currentList.Exists(y => y.SubTitleCode == x.SubTitleCode));

            if (isSubSet)
                flag = true;

            return flag;
        }

        private static bool IsSubsetOrEqualDubbing(List<Dubbing> currentList, List<Dubbing> checkingWithList)
        {
            bool flag = false;
            bool isSubSet = false;

            isSubSet = checkingWithList.All(x => currentList.Exists(y => y.DubbingCode == x.DubbingCode));

            if (isSubSet)
                flag = true;

            return flag;
        }
        #endregion
    }
}
