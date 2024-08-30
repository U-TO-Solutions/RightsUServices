using AcqRightsValidation.AcqDealImportEngine.Repository;
using AcqRightsValidation.Entities;
using Autofac;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace AcqRightsValidation.AcqDealImportEngine
{
    public class ImportAcqDealJob : IJob
    {
        private IRepository<AcqDealRights> acqDealRightsRepository;

        public static IContainer _container;

        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            Title title = new Title();
            title.TitleCode = 0;
            ImportAcqDealRepository importAcquisitionRepository = new ImportAcqDealRepository();

            try
            {
                var startTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                                CultureInfo.InvariantCulture);

                Lib.LogService("ImportAcquisitionEngine execution started at " + startTime);

                ApplicationConfiguration appConfig = new ApplicationConfiguration();
                string titlestoprocessacqqueue = appConfig.GetConfigurationValue("musicacqtitlestoprocess");
                //string acqdealqueue = appConfig.GetConfigurationValue("acqdealqueue");

                #region Read title to process for acquisition from TitlesToProcessAcqQueue Queue
                var titleToProcessAcqQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", titlestoprocessacqqueue),
                                                        new NamedParameter("configuration", appConfig));

                //var acqDealQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", acqdealqueue),
                //new NamedParameter("configuration", appConfig));

                while (!titleToProcessAcqQueue.IsEmpty(titlestoprocessacqqueue))
                {
                    string response = titleToProcessAcqQueue.Dequeue();
                    title = JsonConvert.DeserializeObject<Title>(response);
                    #endregion

                    int musicTrackId = Convert.ToInt32(title.ID);
                    int musicTitleCode = Convert.ToInt32(title.TitleCode);
                    int musicRightsCode = Convert.ToInt32(title.RightsCode);
                    string DuplicateRightsCode = "";

                    Lib.LogService("Title Procesing Started for Title Code - " + title.TitleCode);

                    List<AcqDealTrackRight> rightsCodeList = importAcquisitionRepository.GetAcqDealRightsList(musicTitleCode);

                    List<AcqDealRightsEntity> acquisitionDealRightsEntityList = new List<AcqDealRightsEntity>();

                    foreach (AcqDealTrackRight rightsCode in rightsCodeList)
                    {
                        var acquisitionDealRightsEntity = importAcquisitionRepository.GetAcquisitionDealRight(rightsCode.RightsCode, rightsCode.TitleCode);
                        acquisitionDealRightsEntityList.Add(acquisitionDealRightsEntity);
                    }

                    if (acquisitionDealRightsEntityList != null && acquisitionDealRightsEntityList.Count > 1)
                    {
                        FindDuplicateFromAcqDealRights FindDuplicatesFromAcquisitionDealRights = new FindDuplicateFromAcqDealRights();
                        DuplicateRightsCode = FindDuplicatesFromAcquisitionDealRights.FindDuplicates(acquisitionDealRightsEntityList, musicRightsCode);

                        if (DuplicateRightsCode != "")
                        {
                            importAcquisitionRepository.UpdateTitle(musicTrackId.ToString(), "E", DuplicateRightsCode);
                        }
                        else
                        {
                            importAcquisitionRepository.UpdateTitle(musicTrackId.ToString(), "C");
                        }

                        Lib.LogService("Title Procesing Ended for Title Code - " + title.TitleCode);
                    }
                    else if (acquisitionDealRightsEntityList != null && acquisitionDealRightsEntityList.Count == 1)
                    {
                        importAcquisitionRepository.UpdateTitle(musicTrackId.ToString(), "C");
                    }
                    else
                    {
                        importAcquisitionRepository.UpdateTitle(musicTrackId.ToString(), "E", "Rights Not associated with this track");
                    }
                }

                var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Lib.LogService("ImportAcquisitionEngine execution finished at " + endTime);
            }
            catch (Exception ex)
            {
                Lib.LogService("Title Procesing Error for Title Code - " + title.TitleCode);

                //importAcquisitionRepository.InsertAvailError("Error API : => ImportAcquisitionEngine " + "Error Occured for Title_" + title.TitleCode + ex.Message + ex.StackTrace, 0);
            }
        }

        public void Initialize(IContainer container)
        {
            _container = container;
        }
    }
}
