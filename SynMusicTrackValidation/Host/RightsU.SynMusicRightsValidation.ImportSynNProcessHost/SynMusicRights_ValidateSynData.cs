using Autofac;
using SynDealImportNProcessEngine;
using SynMusicRightsValidation.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UTO.Framework.Shared.Configuration;

namespace RightsU.SynMusicRightsValidation.ImportSynNProcessHost
{
    public partial class SynMusicRights_ValidateSynData : ServiceBase
    {
        Timer timer;
        int count;

        private static ContainerBuilder _builder;
        private static Autofac.IContainer _container;

        public SynMusicRights_ValidateSynData()
        {
            InitializeComponent();
        }

        protected void InitializeContainer()
        {
            Lib.LogService("Service inside InitializeContainer");

            #region initialize container
            _builder = new ContainerBuilder();
            Register.RegisterTypes(_builder);
            _container = _builder.Build();
            #endregion

            Lib.LogService("Service complete InitializeContainer");
        }

        protected override void OnStart(string[] args)
        {
            Lib.LogService("Service is Started 1");

            ApplicationConfiguration config = new ApplicationConfiguration();

            Lib.LogService("Service is Started 2");
            string Dubug = config.GetConfigurationValue("Dubug").ToString();

            Lib.LogService("Service is Started 3");
            AppDomain currentDomain = AppDomain.CurrentDomain;

            Lib.LogService("Service is Started 4");
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Lib.LogService("Service is Started 5");
            if (Dubug == "Y")
                System.Diagnostics.Debugger.Launch();

            Lib.LogService("Service Timer is set");

            StartTimer();
        }

        protected override void OnStop()
        {
            Lib.LogService("ImportAcquisitionEngineHostService is Stoped");

            StopTimer();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception execption = (Exception)e.ExceptionObject;
            Lib.LogService("Error on: {0} " + execption.Message + execption.StackTrace);

            //Restart Service
            StartTimer();
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

                SynDealImportNProcessJob objAcqDealJob = new SynDealImportNProcessJob();
                objAcqDealJob.Initialize(_container);
                objAcqDealJob.Execute();
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
