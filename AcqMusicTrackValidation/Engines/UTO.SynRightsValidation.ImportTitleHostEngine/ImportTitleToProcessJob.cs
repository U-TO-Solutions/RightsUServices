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
            var startTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
           
            Lib.LogService("ImportTitlesToProcessEngine execution started at " + startTime);

            PopulateTitlesToProcess populateTitlesToProcess = new PopulateTitlesToProcess(_container);

            populateTitlesToProcess.populateTitlesToProcess();

            var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            Lib.LogService("ImportTitlesToProcessEngine execution finished at " + endTime);
        }

        public void Initialize(IContainer container)
        {
            _container = container;
        }
    }
}
