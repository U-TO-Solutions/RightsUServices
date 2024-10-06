using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RightsU.SynMusicRightsValidation.ImportAcqNProcessHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SynMusicRights_ValidateAcqData()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
