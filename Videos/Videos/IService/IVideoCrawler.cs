using H.EF.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Videos.Models;

namespace Videos.IService
{
    public interface IVideoCrawler
    {
        OperateResult SynchronizationData(MoviesSyncQuery query);
    }
}
