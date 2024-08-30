using Autofac;
using Autofac.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.SharedInfrastructure.Infrastructure;
using UTO.Framework.SharedInfrastructure.PostgreSQL;
using UTO.Framework.SharedInfrastructure.MSSQL;

namespace AcqRightsValidation.AcqDealImportEngine
{
    public static class Register
    {
        public static void RegisterTypes(ContainerBuilder builder)
        {
            ApplicationConfiguration applicationConfiguration = new ApplicationConfiguration();
            string jobType = applicationConfiguration.GetConfigurationValue("JobType");
            string configurationType = applicationConfiguration.GetConfigurationValue("ConfigurationType");
            string messageQueueType = applicationConfiguration.GetConfigurationValue("MessageQueueType");
            string noSqlDBType = applicationConfiguration.GetConfigurationValue("NoSqlDBType");

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var assemblies = Directory.GetFiles(path, "UTO*.dll", SearchOption.TopDirectoryOnly)
                                      .Select(Assembly.LoadFrom);

            List<Parameter> allParams = new List<Parameter>();
            allParams.Add(new NamedParameter("queueName", string.Empty));
            allParams.Add(new ResolvedParameter(
                                            (pi, ctx) => pi.ParameterType == typeof(IConfiguration) && pi.Name == "configuration",
                                            (pi, ctx) => null));

            foreach (var assembly in assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                       .Where(t => t.Name.EndsWith(jobType))
                       .As<IJob>().InstancePerLifetimeScope();

                builder.RegisterAssemblyTypes(assembly)
                       .Where(t => t.Name.EndsWith(messageQueueType))
                       .As<IMessageQueue>().WithParameters(allParams);

                builder.RegisterAssemblyTypes(assembly)
                       .Where(t => t.Name.EndsWith(configurationType))
                       .As<IConfiguration>().InstancePerLifetimeScope();

                if (noSqlDBType.ToUpper().Contains("POSTGRES"))
                    builder.RegisterGeneric(typeof(PostgreSQLGenericRepository<>)).As(typeof(IRepository<>));

                if (noSqlDBType.ToUpper().Contains("MSSQL"))
                    builder.RegisterGeneric(typeof(SQLGenericRepository<>)).As(typeof(IRepository<>));

                if (noSqlDBType.ToUpper().Contains("MONGO"))
                    builder.RegisterGeneric(typeof(MongoDBGenericRepository<>)).As(typeof(IRepository<>));
            }
        }
    }
}