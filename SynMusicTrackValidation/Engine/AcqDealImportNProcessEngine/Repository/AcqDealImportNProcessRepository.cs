using SynMusicRightsValidation.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace AcqDealImportNProcessEngine.Repository
{
    public class AcqDealImportNProcessRepository
    {
        private readonly DBConnection dbConnection;

        public AcqDealImportNProcessRepository()
        {
            this.dbConnection = new DBConnection();
        }
        public AcqDealImportNProcessRepository(string DBConnection)
        {
            this.dbConnection = new DBConnection(DBConnection);
        }

        public List<AcqDealMusicTrackRight> GetAcqDealRightsList(int musicTrackId, int synDealRightsCode)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_SynMusicRightsValidation_GetAcqRightsList",
                    new { @Music_Title_Code = musicTrackId, @Syn_Deal_Rights_Code = synDealRightsCode },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                var rightsCodeList = result.Read<AcqDealMusicTrackRight>();
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

        //push following code library to avoid duplication of code
        public void UpdateTitle(string strIDs, string strStatus, string strErr, List<MusicError> rights)
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
                    foreach (MusicError objMusicError in rights)
                    {
                        var subresult = connection.QueryMultiple(
                        "USP_SynMusicRightsValidationAddError",
                        new
                        {
                            @Syn_Deal_Music_Track_Code = objMusicError.Syn_Music_Track_Code,
                            @Platform_IDs = objMusicError.PlatformIDs,
                            @Country_IDs = objMusicError.CountryIDs,
                            @Subtitling_IDs = objMusicError.SubtitlingIDs,
                            @Dubbing_IDs = objMusicError.DubbingIDs,
                            @Right_Start_Date = objMusicError.StartDate,
                            @Right_End_Date = objMusicError.EndDate,
                            @Is_Sub_License = objMusicError.IsSubLicensing,
                            @Is_Title_Language_Right = objMusicError.IsTitleLanguageRight,
                            @Agreement_No = "",
                            @ErrorMessage = objMusicError.ErrorMessage
                        },
                        null, queryTimeoutInSeconds, CommandType.StoredProcedure);
                        subresult.Dispose();
                    }
                }
            }
        }
    }
}
