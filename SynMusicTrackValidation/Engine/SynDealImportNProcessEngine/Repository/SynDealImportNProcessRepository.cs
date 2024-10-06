using Dapper;
using SynMusicRightsValidation.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace SynDealImportNProcessEngine.Repository
{
    class SynDealImportNProcessRepository
    {
        private readonly DBConnection dbConnection;

        public SynDealImportNProcessRepository()
        {
            this.dbConnection = new DBConnection();
        }
        public SynDealImportNProcessRepository(string DBConnection)
        {
            this.dbConnection = new DBConnection(DBConnection);
        }

        public List<SynDealMusicTrackRight> GetSynDealRightsList(int musicTrackId, int synDealRightsCode)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_SynMusicRightsValidation_GetSynRightsList",
                    new { @Music_Title_Code = musicTrackId, @Syn_Deal_Rights_Code = synDealRightsCode },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                var rightsCodeList = result.Read<SynDealMusicTrackRight>();
                //Need to change to object as there are two coulmns in output
                return rightsCodeList.ToList();
            }
        }

        //public AcqDealRightsEntity GetAcquisitionDealRight(int acqDealRightsCode, int ContainerCode)
        //{
        //    var appConfig = new ApplicationConfiguration();
        //    int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));

        //    using (var connection = dbConnection.Connection())
        //    {
        //        connection.Open();
        //        var result = connection.QueryMultiple(
        //            "USP_AcqMusicRightsValidationGetRight",
        //            new { @Acq_Deal_Rights_Code = acqDealRightsCode, @Container_Code = ContainerCode },
        //            null, queryTimeoutInSeconds, CommandType.StoredProcedure);

        //        var CountryList = result.Read<Country>();
        //        var PlatformList = result.Read<Platform>();
        //        var SubTitleList = result.Read<SubTitle>();
        //        var DubbingList = result.Read<Dubbing>();
        //        var OtherData = result.Read<OtherData>().FirstOrDefault();
        //        var EpisodeList = result.Read<Episode>();

        //        AcqDealRightsEntity acquisitionDealRightsEntity = new AcqDealRightsEntity();
        //        acquisitionDealRightsEntity.CountryList = CountryList.ToList();
        //        acquisitionDealRightsEntity.PlatformList = PlatformList.ToList();
        //        acquisitionDealRightsEntity.SubTitleList = SubTitleList.ToList();
        //        acquisitionDealRightsEntity.DubbingList = DubbingList.ToList();
        //        acquisitionDealRightsEntity.OtherData = OtherData;
        //        acquisitionDealRightsEntity.EpisodeList = EpisodeList.ToList();
        //        return acquisitionDealRightsEntity;
        //    }
        //}

        public SynDealRightsEntity GetSyndicationDealRight(int synDealRightsCode, int ContainerCode)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));

            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_SynMusicRightsValidationGetRight",
                    new { @Syn_Deal_Rights_Code = synDealRightsCode, @Container_Code = ContainerCode },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                var CountryList = result.Read<Country>();
                var PlatformList = result.Read<Platform>();
                var SubTitleList = result.Read<SubTitle>();
                var DubbingList = result.Read<Dubbing>();
                var OtherData = result.Read<OtherData>().FirstOrDefault();
                var EpisodeList = result.Read<Episode>();

                SynDealRightsEntity objSynDealRightsEntity = new SynDealRightsEntity();
                objSynDealRightsEntity.CountryList = CountryList.ToList();
                objSynDealRightsEntity.PlatformList = PlatformList.ToList();
                objSynDealRightsEntity.SubTitleList = SubTitleList.ToList();
                objSynDealRightsEntity.DubbingList = DubbingList.ToList();
                objSynDealRightsEntity.OtherData = OtherData;
                objSynDealRightsEntity.EpisodeList = EpisodeList.ToList();
                return objSynDealRightsEntity;
            }
        }

        internal void ReprocessWaitingRecords()
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_AcqSynMusicRights_ReprocessWaitingTracks",
                    new { @Request_Type = "S" },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                result.Dispose();
            }
        }

        //push following code library to avoid duplication of code
        //public void UpdateTitle(string strIDs, string strStatus, string strErr, List<MusicError> rights)
        //{
        //    var appConfig = new ApplicationConfiguration();
        //    int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
        //    using (var connection = dbConnection.Connection())
        //    {
        //        connection.Open();
        //        var result = connection.QueryMultiple(
        //            "USP_SynMusicRightsValidationUpdateTitle",
        //            new { @strIDs = strIDs, @strStatus = strStatus, @strErrRightsCode = strErr },
        //            null, queryTimeoutInSeconds, CommandType.StoredProcedure);

        //        result.Dispose();

        //        if (rights != null)
        //        {
        //            foreach (MusicError objMusicError in rights)
        //            {
        //                var subresult = connection.QueryMultiple(
        //                "USP_SynMusicRightsValidationAddError",
        //                new
        //                {
        //                    @Syn_Deal_Music_Track_Code = objMusicError.Syn_Music_Track_Code,
        //                    @Platform_IDs = objMusicError.PlatformIDs,
        //                    @Country_IDs = objMusicError.CountryIDs,
        //                    @Subtitling_IDs = objMusicError.SubtitlingIDs,
        //                    @Dubbing_IDs = objMusicError.DubbingIDs,
        //                    @Right_Start_Date = objMusicError.StartDate,
        //                    @Right_End_Date = objMusicError.EndDate,
        //                    @Is_Sub_License = objMusicError.IsSubLicensing,
        //                    @Is_Title_Language_Right = objMusicError.IsTitleLanguageRight,
        //                    @Agreement_No = "",
        //                    @ErrorMessage = objMusicError.ErrorMessage
        //                },
        //                null, queryTimeoutInSeconds, CommandType.StoredProcedure);
        //                subresult.Dispose();
        //            }
        //        }
        //    }
        //}

        public void UpdateTitle(string strIDs, string strStatus, string strErr, List<Right> rights)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_SynMusicRightsValidationUpdateTitle",
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
                        "USP_SynMusicRightsValidationAddError",
                        new
                        {
                            @Syn_Deal_Music_Track_Code = strIDs,
                            @Platform_IDs = pltfm,
                            @Country_IDs = Ctr,
                            @Subtitling_IDs = subTitle,
                            @Dubbing_IDs = dubbing,
                            @Right_Start_Date = startDt,
                            @Right_End_Date = endDt,
                            @Is_Sub_License = rt.IsSubLicensing.ToString(),
                            @Is_Title_Language_Right = rt.IsTitleLanguage.ToString(),
                            @Agreement_No = rt.RightCode.ToString(),
                            @ErrorMessage = strErr
                        },
                        null, queryTimeoutInSeconds, CommandType.StoredProcedure);
                        subresult.Dispose();
                    }
                }
            }
        }
    }
}
