using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntryPoints.ServiceBus.Base.Gateway
{
    public interface ISubscription
    {
        Task SubscribeAsync();
    }
}
