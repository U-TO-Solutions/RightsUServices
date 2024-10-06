using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynDealImportNProcessEngine.Repository;
using Autofac;
using Newtonsoft.Json;
using SynMusicRightsValidation.Entities;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace SynDealImportNProcessEngine
{
    public class SynDealImportNProcessJob : IJob
    {
        private IRepository<SynDealRights> objSynDealRightsRepos;

        public static IContainer _container;

        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            Title title = new Title();
            title.TitleCode = 0;
            SynDealImportNProcessRepository objSynDealImportNProcessRepos = new SynDealImportNProcessRepository();

            try
            {
                var startTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                Lib.LogService("ImportSyndicationEngine execution started at " + startTime);

                ApplicationConfiguration appConfig = new ApplicationConfiguration();
                string titlestoprocessacqqueue = appConfig.GetConfigurationValue("musicsyntitlestoprocess_syn");
                //string acqdealqueue = appConfig.GetConfigurationValue("acqdealqueue");

                #region Read title to process for acquisition from TitlesToProcessSynQueue Queue
                var titleToProcessSynQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", titlestoprocessacqqueue),
                                                        new NamedParameter("configuration", appConfig));

                while (!titleToProcessSynQueue.IsEmpty(titlestoprocessacqqueue))
                {
                    string response = titleToProcessSynQueue.Dequeue();
                    title = JsonConvert.DeserializeObject<Title>(response);
                    #endregion

                    int synDealMusicTrackId = Convert.ToInt32(title.ID);                    //62258
                    int musicTitleCode = Convert.ToInt32(title.TitleCode);                  //13134
                    int synDealRightsCode = Convert.ToInt32(title.RightsCode);              //9540
                    string Validateion_Status = Convert.ToString(title.Validateion_Status); //"E"

                    SynDealRightsEntity objSynDealRight = objSynDealImportNProcessRepos.GetSyndicationDealRight(synDealRightsCode, musicTitleCode);

                    Lib.LogService("Title Procesing Started for Title Code - " + title.TitleCode);

                    List<SynDealMusicTrackRight> rightsCodeList = objSynDealImportNProcessRepos.GetSynDealRightsList(musicTitleCode, synDealRightsCode);

                    List<SynDealRightsEntity> lstSynDealRightsEntity = new List<SynDealRightsEntity>();
                    //List<SynDealRightsEntity> lstSynRightsNotSynuired = new List<SynDealRightsEntity>();
                    FindRightsDuplication objFindRightsDuplication = new FindRightsDuplication();
                    List<Right> lstDuplicateRights = new List<Right>();

                    foreach (SynDealMusicTrackRight rightsCode in rightsCodeList)
                    {
                        var acquisitionDealRightsEntity = objSynDealImportNProcessRepos.GetSyndicationDealRight(rightsCode.RightsCode, rightsCode.TitleCode);
                        lstSynDealRightsEntity.Add(acquisitionDealRightsEntity);
                    }

                    if (lstSynDealRightsEntity != null && lstSynDealRightsEntity.Count >= 1)
                    {
                        lstDuplicateRights = objFindRightsDuplication.CheckRightsDuplication(lstSynDealRightsEntity, objSynDealRight);

                        if (lstDuplicateRights.Count > 0)
                        {
                            objSynDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), "E", "Duplicate/Overlapping Rights", lstDuplicateRights);
                        }
                        else
                        {
                            objSynDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), Validateion_Status, "Success", null);
                        }

                        Lib.LogService("Title Procesing Ended for Title Code - " + title.TitleCode);
                    }
                    else
                        objSynDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), Validateion_Status, "Success", null);
                }

                //string MusicValidation_AcqDataQueue = appConfig.GetConfigurationValue("musicsyntitlestoprocess_acq");
                //var titleToProcessAcqQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", MusicValidation_AcqDataQueue),
                //                                        new NamedParameter("configuration", appConfig));

                //if (titleToProcessAcqQueue.IsEmpty(MusicValidation_AcqDataQueue) && titleToProcessSynQueue.IsEmpty(titlestoprocessacqqueue))
                //{
                //    objSynDealImportNProcessRepos.ReprocessWaitingRecords();
                //}

                var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Lib.LogService("ImportSyndicationEngine execution finished at " + endTime);
            }
            catch (Exception ex)
            {
                Lib.LogService("Title Procesing Error for Title Code - " + title.TitleCode);
                Lib.LogService("Error" + ex.InnerException);
                //objSynDealImportNProcessRepos.InsertAvailError("Error API : => ImportSyndicationEngine " + "Error Occured for Title_" + title.TitleCode + ex.Message + ex.StackTrace, 0);
            }
        }

        public void Initialize(IContainer container)
        {
            _container = container;
        }
    }
}
