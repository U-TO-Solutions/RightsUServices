using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcqDealImportNProcessEngine.Repository;
using Autofac;
using Newtonsoft.Json;
using SynMusicRightsValidation.Entities;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace AcqDealImportNProcessEngine
{
    public class AcqDealImportNProcessJob : IJob
    {
        private IRepository<AcqDealRights> acqDealRightsRepository;

        public static IContainer _container;

        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            Title objTitle = new Title();
            objTitle.TitleCode = 0;
            AcqDealImportNProcessRepository objAcqDealImportNProcessRepos = new AcqDealImportNProcessRepository();

            try
            {
                var startTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                                                CultureInfo.InvariantCulture);

                Lib.LogService("ImportAcquisitionEngine execution started at " + startTime);

                ApplicationConfiguration appConfig = new ApplicationConfiguration();
                string MusicValidation_AcqDataQueue = appConfig.GetConfigurationValue("musicsyntitlestoprocess_acq");
                string MusicValidation_SynDataQueue = appConfig.GetConfigurationValue("musicsyntitlestoprocess_syn");
                //string acqdealqueue = appConfig.GetConfigurationValue("acqdealqueue");

                #region Read title to process for acquisition from TitlesToProcessAcqQueue Queue
                var titleToProcessAcqQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", MusicValidation_AcqDataQueue),
                                                        new NamedParameter("configuration", appConfig));

                //var acqDealQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", acqdealqueue),
                //new NamedParameter("configuration", appConfig));

                while (!titleToProcessAcqQueue.IsEmpty(MusicValidation_AcqDataQueue))
                {
                    string response = titleToProcessAcqQueue.Dequeue();
                    objTitle = JsonConvert.DeserializeObject<Title>(response);
                    #endregion

                    int synDealMusicTrackId = Convert.ToInt32(objTitle.ID);        //62258
                    int musicTitleCode = Convert.ToInt32(objTitle.TitleCode);      //13134
                    int synDealRightsCode = Convert.ToInt32(objTitle.RightsCode);  //9540
                    //List<Right> DuplicateRightsCode = new List<Right>();

                    SynDealRightsEntity objSynDealRight = objAcqDealImportNProcessRepos.GetSyndicationDealRight(synDealRightsCode, musicTitleCode);

                    Lib.LogService("Title Procesing Started for Title Code - " + objTitle.TitleCode);

                    List<AcqDealMusicTrackRight> rightsCodeList = objAcqDealImportNProcessRepos.GetAcqDealRightsList(musicTitleCode, synDealRightsCode);

                    List<AcqDealRightsEntity> lstAcqDealRightsEntity = new List<AcqDealRightsEntity>();
                    List<SynDealRightsEntity> lstSynRightsNotAcquired = new List<SynDealRightsEntity>();
                    FindRightsAcquired objFindRightsAcquired = new FindRightsAcquired();

                    foreach (AcqDealMusicTrackRight rightsCode in rightsCodeList)
                    {
                        var acquisitionDealRightsEntity = objAcqDealImportNProcessRepos.GetAcquisitionDealRight(rightsCode.RightsCode, rightsCode.TitleCode);
                        lstAcqDealRightsEntity.Add(acquisitionDealRightsEntity);
                    }

                    if (lstAcqDealRightsEntity != null && lstAcqDealRightsEntity.Count >= 1)
                    {
                        lstSynRightsNotAcquired = objFindRightsAcquired.CheckAcquisition(lstAcqDealRightsEntity, objSynDealRight);

                        Lib.LogService("Title Procesing Ended for Title Code - " + objTitle.TitleCode);
                    }
                    else
                    {
                        SynDealRightsEntity objSynRightsNA = new SynDealRightsEntity();
                        objSynRightsNA.OtherData = objSynDealRight.OtherData;
                        objSynRightsNA.ValidationStatus = "E";
                        objSynRightsNA.ErrorMessage = "Title Not Acquired";
                        lstSynRightsNotAcquired.Add(objSynRightsNA);
                    }

                    if (lstSynRightsNotAcquired.Count > 0)
                    {
                        List<MusicError> lstMusicError = new List<MusicError>();
                        lstMusicError = objFindRightsAcquired.GroupErrorMessage(synDealMusicTrackId, lstSynRightsNotAcquired);
                        objTitle.Validateion_Status = "E";

                        objAcqDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), "W", "Combination Not Acquired", lstMusicError);
                    }
                    else
                        objTitle.Validateion_Status = "C";
                    //else
                    //    objAcqDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), "C", "Success", null);

                    var titleSer = JsonConvert.SerializeObject(objTitle);
                    var acqQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", MusicValidation_SynDataQueue),
                                                                            new NamedParameter("configuration", appConfig));

                    acqQueue.Enqueue(titleSer);

                    //else
                    //{
                    //    objAcqDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), "C", "Success", null);
                    //}

                    //else if (lstAcqDealRightsEntity != null && lstAcqDealRightsEntity.Count == 1)
                    //{
                    //    objAcqDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), "C", "Success", null);
                    //}
                    //else
                    //{
                    //    objAcqDealImportNProcessRepos.UpdateTitle(synDealMusicTrackId.ToString(), "E", "Rights Not associated with this track", null);
                    //}
                }

                //var titleToProcessSynQueue = _container.BeginLifetimeScope().Resolve<IMessageQueue>(new NamedParameter("queueName", MusicValidation_SynDataQueue),
                //                                        new NamedParameter("configuration", appConfig));

                //if (titleToProcessAcqQueue.IsEmpty(MusicValidation_AcqDataQueue) && titleToProcessSynQueue.IsEmpty(MusicValidation_SynDataQueue))
                //{
                //    objAcqDealImportNProcessRepos.ReprocessWaitingRecords();
                //}

                var endTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Lib.LogService("ImportAcquisitionEngine execution finished at " + endTime);
            }
            catch (Exception ex)
            {
                Lib.LogService("Title Procesing Error for Title Code - " + objTitle.TitleCode);
                Lib.LogService("Error" + ex.InnerException);
                //objAcqDealImportNProcessRepos.InsertAvailError("Error API : => ImportAcquisitionEngine " + "Error Occured for Title_" + title.TitleCode + ex.Message + ex.StackTrace, 0);
            }
        }

        public void Initialize(IContainer container)
        {
            _container = container;
        }
    }
}
