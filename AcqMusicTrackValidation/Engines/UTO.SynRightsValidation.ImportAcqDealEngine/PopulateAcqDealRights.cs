using System.Collections.Generic;
using AcqRightsValidation.Entities;

namespace AcqRightsValidation.AcqDealImportEngine
{
    public class PopulateAcqDealRights
    {
        public static AcqDealRights PopulateAcquisitionDealRights_From_AcquisitionDealRightsEntity(int titleCode, List<AcqDealRightsEntity> acquisitionDealRightsEntityList)//, TitleInfo titleInfo)
        {
            AcqDealRights acquisitionDealRights = new AcqDealRights();
            List<Right> rightList = new List<Right>();
            foreach (var acquisitionDealRightsEntity in acquisitionDealRightsEntityList)
            {
                Right right = new Right();
                right.RightCode = acquisitionDealRightsEntity.OtherData.RightCode;
                right.CountryList = acquisitionDealRightsEntity.CountryList;
                right.PlatformList = acquisitionDealRightsEntity.PlatformList;
                LicensePeriod licensePeriod = new LicensePeriod();
                licensePeriod.LicensePeriodStartFrom = acquisitionDealRightsEntity.OtherData.ActualRightStartDate;
                licensePeriod.LicensePeriodEndTo = acquisitionDealRightsEntity.OtherData.ActualRightEndDate;
                licensePeriod.IsPerpetualLicense = acquisitionDealRightsEntity.OtherData.ActualRightEndDate == null ? true : false;
                right.LicensePeriod = licensePeriod;
                right.SubTitleList = acquisitionDealRightsEntity.SubTitleList;
                right.DubbingList = acquisitionDealRightsEntity.DubbingList;
                right.EpisodeList = acquisitionDealRightsEntity.EpisodeList;// Added By Sachin
                right.IsTitleLanguage = acquisitionDealRightsEntity.OtherData.IsTitleLanguageRight == "Y" ? true : false;
                right.IsSubLicensing = acquisitionDealRightsEntity.OtherData.IsSubLicense == "Y" ? true : false;
                right.IsSelfConsumption = acquisitionDealRightsEntity.OtherData.IsSubLicense == "Y" ? false : true;
                right.IsExclusive = acquisitionDealRightsEntity.OtherData.IsExclusive == "Y" ? true : false;
                right.IsNonExclusive = acquisitionDealRightsEntity.OtherData.IsExclusive == "Y" ? false : true;
                right.SubLicensing = acquisitionDealRightsEntity.OtherData.SubLicensing;
                right.IsTheatrical = acquisitionDealRightsEntity.OtherData.IsTheatricalRight == "Y" ? true : false;
                right.ROFR = acquisitionDealRightsEntity.OtherData.ROFR;
                // right.RestrictionRemarks = acquisitionDealRightsEntity.OtherData.RestrictionRemarks;
                //  right.Remarks = acquisitionDealRightsEntity.OtherData.Remarks;
                // right.RightsRemarks = acquisitionDealRightsEntity.OtherData.RightsRemarks;
                right.SelfUtilizationGroup = acquisitionDealRightsEntity.OtherData.SelfUtilizationGroup;
                right.SelfUtilizationRemarks = acquisitionDealRightsEntity.OtherData.SelfUtilizationRemarks;

                rightList.Add(right);
            }

            acquisitionDealRights.RightList = rightList;

            acquisitionDealRights.TitleCode = titleCode;

            return acquisitionDealRights;
        }
    }
}

