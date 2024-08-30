using Autofac;
using System;
using System.ServiceProcess;
using System.Timers;
using UTO.Framework.Shared.Configuration;
using UTO.AcqRightsValidation.MusicTrackImportEngine;
using AcqRightsValidation.Entities;
using AcqRightsValidation.MusicTrackImportEngine.Repository;

namespace UTO.AcqRightsValidation.ImportTitleHost
{
    public partial class ImportTitleToProcessHostEngineService : ServiceBase
    {
        Timer timer;
        int count;

        private static ContainerBuilder _builder;
        private static IContainer _container;
        public ImportTitleToProcessHostEngineService()
        {
            #region initialize component
            InitializeComponent();
            #endregion
        }
        protected void InitializeContainer()
        {
            #region initialize container
            _builder = new ContainerBuilder();
            Register.RegisterTypes(_builder);
            _container = _builder.Build();
            #endregion
        }
        protected override void OnStart(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Diagnostics.Debugger.Launch();
            Lib.LogService("ImportTitleToProcessEngineHostService is Started");

            StartTimer();
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception execption = (Exception)e.ExceptionObject;
            Lib.LogService("Error on: {0} " + execption.Message + execption.StackTrace);

            //Restart Service
            StartTimer();
        }

        protected override void OnStop()
        {
            Lib.LogService("ImportTitlesToProcessEngineHostService is Stoped");

            StopTimer();
        }

        void StartTimer()
        {
            ApplicationConfiguration config = new ApplicationConfiguration();
            int runAtInterval = config.GetConfigurationValue<int>("RunAtInterval");

            timer = new Timer(runAtInterval);
            timer.AutoReset = true;
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            timer.Enabled = true;
            timer.Start();
        }

        void StopTimer()
        {
            timer.Stop();
            timer.Enabled = false;
        }

        public void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            StopTimer();

            string process = "Timer Tick " + count + DateTime.Now;
            Lib.LogService(process);
            count++;

            ImportTitleToProcessRepository importTitlesToProcessRepository = new ImportTitleToProcessRepository();

            try
            {
                InitializeContainer();

                ImportTitleToProcessJob importTitlesToProcessJob = new ImportTitleToProcessJob();
                importTitlesToProcessJob.Initialize(_container);
                importTitlesToProcessJob.Execute();
            }
            catch (Exception ex)
            {
                Lib.LogService(ex.Message + " " + ex.StackTrace);
                //throw;
            }
            finally
            {
                ApplicationConfiguration config = new ApplicationConfiguration();
                bool runOnce = config.GetConfigurationValue<bool>("RunOnce", false);

                if (!runOnce)
                    StartTimer();
            }
        }
    }
}
