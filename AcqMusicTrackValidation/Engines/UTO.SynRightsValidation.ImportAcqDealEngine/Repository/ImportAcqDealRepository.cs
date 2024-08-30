using Dapper;
using Autofac;
using System;
using System.Data;
using UTO.Framework.SharedInfrastructure.Infrastructure;
using UTO.Framework.Shared.Configuration;
using System.Collections.Generic;
using AcqRightsValidation.Entities;
using System.Linq;

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
        public void UpdateTitle(string strIDs, string strStatus, string strErrRightsCode="")
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_AcqMusicRightsValidationUpdateTitle",
                    new { @strIDs = strIDs, @strStatus = strStatus, @strErrRightsCode = strErrRightsCode },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);
            }
        }
    }
}
