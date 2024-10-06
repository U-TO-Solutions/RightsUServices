using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Autofac;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Enums;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.Shared.Logging;
using AcqRightsValidation.Entities;

namespace UTO.AcqRightsValidation.MusicTrackImportEngine
{
    public class ImportTitleToProcessJob : IJob
    {
        private static IContainer _container;
        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            //var startTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
           
            Lib.LogService("Import Acq Music Tracks To Process Engine Execution Started at " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));

            PopulateTitlesToProcess populateTitlesToProcess = new PopulateTitlesToProcess(_container);

            populateTitlesToProcess.populateTitlesToProcess("A");

            //var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            Lib.LogService("Import Acq Music Tracks To Process Engine Execution Finished at " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));

            //var startSynTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            Lib.LogService("Import Syn Music Tracks To Process Engine Execution Started at " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));

            populateTitlesToProcess.populateTitlesToProcess("S");

            //var endSynTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            Lib.LogService("Import Syn Music Tracks To Process Engine Execution Finished at " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));


        }

        public void Initialize(IContainer container)
        {
            _container = container;
        }
    }
}
