using AcqRightsValidation.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace AcqRightsValidation.AcqDealImportEngine.Repository
{
    public class ImportAcqDealRepository
    {
        private readonly DBConnection dbConnection;

        public ImportAcqDealRepository()
        {
            this.dbConnection = new DBConnection();
        }
        public ImportAcqDealRepository(string DBConnection)
        {
            this.dbConnection = new DBConnection(DBConnection);
        }

        public List<AcqDealTrackRight> GetAcqDealRightsList(int musicTrackId)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_AcqMusicRightsValidationGetRightsList",
                    new { @Music_Title_Code = musicTrackId },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                var rightsCodeList = result.Read<AcqDealTrackRight>();
                //Need to change to object as there are two coulmns in output
                return rightsCodeList.ToList();
            }
        }

        public AcqDealRightsEntity GetAcquisitionDealRight(int acqDealRightsCode, int ContainerCode)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));

            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_AcqMusicRightsValidationGetRight",
                    new { @Acq_Deal_Rights_Code = acqDealRightsCode, @Container_Code = ContainerCode },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                var CountryList = result.Read<Country>();
                var PlatformList = result.Read<Platform>();
                var SubTitleList = result.Read<SubTitle>();
                var DubbingList = result.Read<Dubbing>();
                var OtherData = result.Read<OtherData>().FirstOrDefault();
                var EpisodeList = result.Read<Episode>();

                AcqDealRightsEntity acquisitionDealRightsEntity = new AcqDealRightsEntity();
                acquisitionDealRightsEntity.CountryList = CountryList.ToList();
                acquisitionDealRightsEntity.PlatformList = PlatformList.ToList();
                acquisitionDealRightsEntity.SubTitleList = SubTitleList.ToList();
                acquisitionDealRightsEntity.DubbingList = DubbingList.ToList();
                acquisitionDealRightsEntity.OtherData = OtherData;
                acquisitionDealRightsEntity.EpisodeList = EpisodeList.ToList();
                return acquisitionDealRightsEntity;
            }
        }

        //push following code library to avoid duplication of code
        public void UpdateTitle(string strIDs, string strStatus, string strErr, List<Right> rights)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_AcqMusicRightsValidationUpdateTitle",
                    new { @strIDs = strIDs, @strStatus = strStatus, @strErrRightsCode = strErr },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                result.Dispose();

                if (rights != null)
                {
                    foreach (Right rt in rights)
                    {
                        string Ctr = "", pltfm = "", startDt = "", endDt = "", subTitle = "", dubbing = "";

                        if (rt.CountryList.Count > 0)
                            Ctr = string.Join(",", rt.CountryList.Select(x => x.CountryCode));
                        if (rt.PlatformList.Count > 0)
                            pltfm = string.Join(",", rt.PlatformList.Select(x => x.PlatformCode));
                        if (rt.SubTitleList != null)
                            if (rt.SubTitleList.Count > 0)
                                subTitle = string.Join(",", rt.SubTitleList.Select(x => x.SubTitleCode));
                        if (rt.DubbingList != null)
                            if (rt.DubbingList.Count > 0)
                                dubbing = string.Join(",", rt.DubbingList.Select(x => x.DubbingCode));

                        startDt = rt.LicensePeriod.LicensePeriodStartFrom.ToString("dd-MMM-yyyy");
                        endDt = Convert.ToDateTime(rt.LicensePeriod.LicensePeriodEndTo).ToString("dd-MMM-yyyy");

                        var subresult = connection.QueryMultiple(
                        "USP_AcqMusicRightsValidationAddError",
                        new
                        {
                            @Acq_Deal_Music_Track_Code = strIDs,
                            @Platform_ids = pltfm,
                            @Right_Start_Date = startDt,
                            @Right_End_Date = endDt,
                            @Right_Type = "",
                            @Is_Sub_License = rt.IsSubLicensing.ToString(),
                            @Is_Title_Language_Right = rt.IsTitleLanguage.ToString(),
                            @Country_Name = Ctr,
                            @Subtitling_Language = subTitle,
                            @Dubbing_Language = dubbing,
                            @Acq_Deal_Rights_Code = rt.RightCode.ToString()
                        },
                        null, queryTimeoutInSeconds, CommandType.StoredProcedure);
                        subresult.Dispose();
                    }
                }
            }
        }
    }
}
