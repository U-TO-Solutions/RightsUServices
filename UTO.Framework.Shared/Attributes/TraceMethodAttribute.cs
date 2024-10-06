//using System;
//using System.Reflection;
//using UTO.Framework.Shared.Configuration;
//using UTO.Framework.Shared.Enums;
//using UTO.Framework.Shared.Logging;

//namespace UTO.Framework.Shared.Attributes
//{
//    //[module: LogMethod] // Atribute should be "registered" by adding as module or assembly custom attribute

//    // Any attribute which provides OnEntry/OnExit/OnException with proper args
//    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
//    public class TraceMethodAttribute : Attribute, IMethodDecorator
//    {
//        private MethodBase _method;
//        EventLogger eventLogger = new EventLogger("UTO", new ApplicationConfiguration());

//        ApplicationConfiguration config = new ApplicationConfiguration();
//        private int _appId;
//        private string _appName;

//        // instance, method and args can be captured here and stored in attribute instance fields
//        // for future usage in OnEntry/OnExit/OnException
//        public void Init(object instance, MethodBase method, object[] args)
//        {
//            ApplicationConfiguration config = new ApplicationConfiguration();
//            _appId = config.GetConfigurationValue<int>("AppId");
//            _appName = config.GetConfigurationValue<string>("AppName");
//            _method = method;
//        }

//        public void OnEntry()
//        {
//            //Console.WriteLine("Entering into {0}", _method.Name);
//            eventLogger.Log(LogLevel.Trace, _appId, _appName, string.Format("Entering into {0}", _method.Name));
//        }

//        public void OnExit()
//        {
//            //Console.WriteLine("Exiting into {0}", _method.Name);
//            eventLogger.Log(LogLevel.Trace, _appId, _appName, string.Format("Exiting into {0}", _method.Name));
//        }

//        public void OnException(Exception exception)
//        {
//            //Console.WriteLine("Exception {0} occured in {1}", exception.Message.ToString(), _method.Name);
//            eventLogger.Log(LogLevel.Error, _appId, _appName, string.Format("Exception {0} occured in {1}", exception.Message.ToString(), _method.Name));
//        }
//    }
//}
