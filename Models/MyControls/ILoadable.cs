using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.MyControls
{
    public interface ILoadable
    {
        System.Threading.Tasks.Task EnsureLoadedAsync(System.Threading.CancellationToken ct);
    }

}
