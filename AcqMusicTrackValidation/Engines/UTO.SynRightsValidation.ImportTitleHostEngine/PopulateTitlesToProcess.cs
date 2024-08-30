using Autofac;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UTO.Framework.Shared.Interfaces;
using AcqRightsValidation.Entities;
using AcqRightsValidation.MusicTrackImportEngine.Repository;

namespace UTO.AcqRightsValidation.MusicTrackImportEngine
{
    public class PopulateTitlesToProcess
    {
        private static IContainer _container;

        public PopulateTitlesToProcess(IContainer container)
        {
            _container = container;
        }

        public void populateTitlesToProcess()
        {
            ImportTitleToProcessRepository importTitlesToProcessRepository = new ImportTitleToProcessRepository();

            try
            {
                var appConfig = _container.BeginLifetimeScope().Resolve<IConfiguration>();

                string titlestoprocessacqqueue = appConfig.GetConfigurationValue("musicacqtitlestoprocess");
                //string titlestoprocesssynqueue = appConfig.GetConfigurationValue("titlestoprocesssynqueue");

                string strIds = "";
                List<Title> allTitlesToProcess = importTitlesToProcessRepository.GetTitlesToProcessList();

                foreach (Title titleCodeName in allTitlesToProcess)
                {
                    #region add Title to process to TitlesToProcessQueue of MSMQ
                    Title titleToProcess = new Title();
                    titleToProcess.TitleCode = titleCodeName.TitleCode;
                    titleToProcess.ID = titleCodeName.ID;
                    titleToProcess.RightsCode = titleCodeName.RightsCode;

                    var title = JsonConvert.SerializeObject(titleToProcess);

                    var acqQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", titlestoprocessacqqueue),
                                                        new NamedParameter("configuration", appConfig));

                    acqQueue.Enqueue(title);

                    if (strIds.Length < 10000)
                        strIds += titleCodeName.ID.ToString() + ",";
                    else
                    {
                        strIds += titleCodeName.ID.ToString();
                        importTitlesToProcessRepository.UpdateTitle(strIds, "W");
                        strIds = "";
                    }
                    #endregion
                    Lib.LogService("Queued:" + titleCodeName.ID);
                }
                if (strIds.Length < 10000 && strIds.Length > 0) importTitlesToProcessRepository.UpdateTitle(strIds, "W");
            }
            catch (Exception ex)
            {
                Lib.LogService("Error API : => ImportTitlesToProcessEngine " + ex.Message + ex.StackTrace);
            }
        }
    }
}
