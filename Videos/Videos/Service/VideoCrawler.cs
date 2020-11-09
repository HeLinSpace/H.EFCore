using AngleSharp.Html.Parser;
using H.EF.Core;
using H.EF.Core.Extensions;
using H.EF.Core.Model;
using H.EF.Core.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using Videos.Entities;
using Videos.IService;
using Videos.Models;

namespace Videos.Service
{
    public class VideoCrawler: IVideoCrawler
    {
        private readonly IUnitRepository _repository;

        public VideoCrawler(IUnitRepository repository)
        {
            _repository = repository;
        }

        public OperateResult SynchronizationData(MoviesSyncQuery query)
        {
            var list = new List<string>();

            for (var i = query.PageFrom; i <= query.PageTo; i++)
            {
                if (query.Source == Const.SourcesType.Hao123)
                {
                    list.Add(Const.SourcesValue.Hao123 + "&pn=" + i);
                }
            }

            if (query.Source == Const.SourcesType.Hao123)
            {
                Hao123MoviesCrawler(list);
            }

            return new OperateResult() { Status = OperateStatus.Success};
        }

        /// <summary>
        /// 好123 电影爬虫
        /// </summary>
        /// <param name="urlList"></param>
        /// <param name="isDetial"></param>
        [TransactionHandler]
        private void Hao123MoviesCrawler(List<string> urlList, bool isDetial = false)
        {
            HtmlParser htmlParser = new HtmlParser();
            string resource = Const.SourcesType.Hao123;

            for (var i = 0; i < urlList.Count; i++)
            {
                var crawler = new SimpleCrawler();

                crawler.OnStart += (s, e) =>
                {
                    Console.WriteLine("爬虫开始抓取地址：" + e.Uri.ToString());
                };
                crawler.OnError += (s, e) =>
                {
                    Console.WriteLine("爬虫抓取出现错误：" + e.Uri.ToString() + "，异常消息：" + e.Exception.Message);
                };
                crawler.OnCompleted += (s, e) =>
                {
                    if (isDetial)
                    {
                        var dom = htmlParser.ParseDocument(e.PageSource);

                        var moviesInfo = new MoviesInfo();
                        var urlSourceList = new List<UrlSource>();

                        moviesInfo.Id = GuidExtend.NewGuid();
                        moviesInfo.Resource = resource;

                        moviesInfo.CreateTime = DateTime.Now;

                        var a = dom.QuerySelectorAll("div.poster>a");
                        if (a.Any())
                        {
                            moviesInfo.Name = a[0].GetAttribute("title"); //--电影名称
                        }
                        else
                        {
                            return;
                        }

                        var stars = dom.All.Where(sl => sl.GetAttribute("monkey") == "actor").ToList();

                        if (stars.Any())
                        {
                            moviesInfo.Stars = string.Join(",", stars[0].QuerySelectorAll("a").Select(X => X.InnerHtml).ToList().Distinct());
                        }

                        var type = dom.All.Where(sl => sl.GetAttribute("monkey") == "category").ToList();

                        if (type.Any())
                        {
                            moviesInfo.Type = string.Join(",", type[0].QuerySelectorAll("a").Select(X => X.InnerHtml).ToList().Distinct());
                        }


                        var area = dom.All.Where(sl => sl.GetAttribute("monkey") == "area").ToList();

                        if (area.Any())
                        {
                            moviesInfo.Area = string.Join(",", area[0].QuerySelectorAll("a").Select(X => X.InnerHtml).ToList().Distinct());
                        }

                        var year = dom.All.Where(sl => sl.GetAttribute("monkey") == "decade").ToList();

                        if (year.Any())
                        {
                            moviesInfo.Year = string.Join(",", year[0].QuerySelectorAll("a").Select(X => X.InnerHtml).ToList().Distinct());
                        }

                        var img = dom.QuerySelectorAll("div.poster>a>img");

                        if (img.Any())
                        {
                            moviesInfo.ImageUrl = img[0].GetAttribute("src"); //--图片
                        }

                        var des = dom.QuerySelectorAll("p.abstract>em");

                        if (des.Any())
                        {
                            moviesInfo.Description = des[0].InnerHtml;
                        }

                        var url = dom.QuerySelectorAll("div.source>a.play-btn");

                        if (url.Any())
                        {
                            var urlSource = new UrlSource();
                            urlSource.Url = url[0].GetAttribute("href");
                            urlSource.VideoSource = url[0].GetAttribute("alog-text");
                            urlSource.Id = GuidExtend.NewGuid();
                            urlSource.MovieId = moviesInfo.Id;
                            urlSource.Resource = resource;

                            urlSourceList.Add(urlSource);
                        }

                        var urls = dom.QuerySelectorAll("div.source")[0].QuerySelectorAll("ul>li>a").
                                    Select(x => new UrlSource
                                    {
                                        Id = GuidExtend.NewGuid(),
                                        MovieId = moviesInfo.Id,
                                        Url = x.GetAttribute("href"),
                                        VideoSource = x.TextContent,
                                        Resource = resource
                                    });

                        if (urls.Any())
                        {
                            urlSourceList.AddRange(urls);
                        }

                        if (!string.IsNullOrEmpty(moviesInfo.Name) && urlSourceList.Count > 0)
                        {

                            var oldData = _repository.All<MoviesInfo>(sl => sl.Name == moviesInfo.Name && sl.ImageUrl == moviesInfo.ImageUrl);

                            oldData.DeleteFromQuery();

                            _repository.DeleteByExpression<UrlSource>(sl => oldData.Select(m => m.Id).Contains(sl.MovieId));

                            _repository.Insert(moviesInfo, true);
                            _repository.BulkInsert<UrlSource>(urlSourceList);
                        }
                    }
                    else
                    {
                        var dom = htmlParser.ParseDocument(e.PageSource);

                        var MovieUrlList = dom.QuerySelectorAll("li.card>a").Select(a => a.GetAttribute("href")).ToList();

                        Hao123MoviesCrawler(MovieUrlList, true);
                    }
                };
                crawler.Start(new Uri(urlList[i])).Wait();
            }
        }
    }
}
