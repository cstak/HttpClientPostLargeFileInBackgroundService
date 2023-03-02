using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpClientPostLargeFileInBackgroundService.Interfaces
{
    public interface IFileWatcherService
    {
        void Start();
    }
}