using Autofac;
using Microsoft.Azure.WebJobs.Host;

namespace RomMaster.Server.WebJob
{
    public class AutofacJobActivator : IJobActivator
    {
        private readonly IContainer container;

        public AutofacJobActivator(IContainer container)
        {
            this.container = container;
        }

        public T CreateInstance<T>()
        {
            return container.Resolve<T>();
        }
    }
}
