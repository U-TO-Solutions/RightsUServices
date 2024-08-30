using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Enums;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.Shared.Logging;
using UTO.Framework.SharedInfrastructure.Infrastructure;
using AcqRightsValidation.Entities;

namespace AcqRightsValidation.MusicTrackImportEngine.Repository
{
    public class ImportTitleToProcessRepository
    {
        private readonly DBConnection dbConnection;

        public ImportTitleToProcessRepository()
        {
            this.dbConnection = new DBConnection();
        }

        public ImportTitleToProcessRepository(string DBConnection)
        {
            this.dbConnection = new DBConnection(DBConnection);
        }

        public List<Title> GetTitlesToProcessList()
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_AcqMusicRightsValidationGetTitlesList",
                    new { },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

                var titlesCodeList = result.Read<Title>();
                
                return titlesCodeList.ToList();
            }
        }

        public void UpdateTitle(string strIDs, string strStatus)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USP_AcqMusicRightsValidationUpdateTitle",
                    new { @strIDs = strIDs, @strStatus = strStatus, @strErrRightsCode =""},
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);
            }
        }
        public void InsertAvailError(string Error, int code)
        {
            var appConfig = new ApplicationConfiguration();
            int? queryTimeoutInSeconds = Convert.ToInt32(appConfig.GetConfigurationValue("QueryTimeoutInSeconds"));
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "USPInsertAvailError",
                    new { @ErrorMsg = Error, @intCode = code },
                    null, queryTimeoutInSeconds, CommandType.StoredProcedure);

            }
        }
    }
}
