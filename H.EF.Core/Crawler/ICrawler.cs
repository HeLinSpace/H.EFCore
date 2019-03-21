using H.EF.Core.Crawler.Event;
using System;
using System.Threading.Tasks;

namespace H.EF.Core.Crawler.Interface
{
    public interface ICrawler
    {
        event EventHandler<OnStartEventArgs> OnStart;//爬虫启动事件

        event EventHandler<OnCompletedEventArgs> OnCompleted;//爬虫完成事件

        event EventHandler<OnErrorEventArgs> OnError;//爬虫出错事件

        Task<string> Start(Uri uri, string proxy = null); //异步爬虫
    }
}
