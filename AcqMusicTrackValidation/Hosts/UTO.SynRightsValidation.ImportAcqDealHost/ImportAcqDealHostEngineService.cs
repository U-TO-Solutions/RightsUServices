using System;
using System.Security.Permissions;
using System.ServiceProcess;
using Autofac;
using System.Timers;
using UTO.Framework.Shared.Configuration;
using AcqRightsValidation.AcqDealImportEngine;
using AcqRightsValidation.Entities;

namespace AcqRightsValidation.ImportAcqDealHost
{
    public partial class ImportAcquisitionEngineHostService : ServiceBase
    {
        Timer timer;
        int count;

        private static ContainerBuilder _builder;
        private static Autofac.IContainer _container;

        public ImportAcquisitionEngineHostService()
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

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        protected override void OnStart(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;

            ApplicationConfiguration config = new ApplicationConfiguration();
            string Dubug = config.GetConfigurationValue("Dubug").ToString();
            if (Dubug == "Y")
                System.Diagnostics.Debugger.Launch();

            Lib.LogService("ImportAcquisitionEngineHostService is Started");

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
            Lib.LogService("ImportAcquisitionEngineHostService is Stoped");

            StopTimer();
        }

        void StartTimer()
        {
            ApplicationConfiguration config = new ApplicationConfiguration();
            int runAtInterval = config.GetConfigurationValue<int>("RunAtInterval");

            timer = new Timer(runAtInterval);
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

            try
            {
                InitializeContainer();

                ImportAcqDealJob ImportAcqDealJob = new ImportAcqDealJob();
                ImportAcqDealJob.Initialize(_container);
                ImportAcqDealJob.Execute();
            }
            catch (Exception ex)
            {
                Lib.LogService(ex.Message + " " + ex.StackTrace);
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