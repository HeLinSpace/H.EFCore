using H.EF.Core.Model;
using H.EF.Core.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Videos.Entities;
using Videos.IService;
using Videos.Models;

namespace Videos.Service
{
    public class VideoManageService: IVideoManageService
    {
        private readonly IUnitRepository _repository;

        public VideoManageService(IUnitRepository repository)
        {
            _repository = repository;
        }

        public MoviesListView GetMovies(PageQuery query)
        {
            var moviesInfo = new OperateResultPage<IQueryable<MoviesInfo>>();

            if (string.IsNullOrEmpty(query.Key))
            {
                moviesInfo = _repository.QueryWithPage<MoviesInfo>(query);
            }
            else
            {
                moviesInfo = _repository.QueryWithPage<MoviesInfo>(query, s => s.Name.Contains(query.Key) ||
                                s.Stars.Contains(query.Key) || s.Area.Contains(query.Key)
                                || s.Type.Contains(query.Key) || s.Year.Contains(query.Key));
            }

            var result = new List<string>();

            var data = moviesInfo.Rows.ToList();

            foreach (var item in data)
            {
                result.Add(GetMovieDiv(item));
            }

            return new MoviesListView()
            {
                MoviesContent = string.Join("", result),
                Page = query.Page,
                Total = moviesInfo.Total
            };
        }

        public MoviesView GetMovie(string Id)
        {
            var data = _repository.FindById<MoviesInfo>(Id);

            var detial = _repository.All<UrlSource>().Where(s => s.MovieId == Id).
                Select(s => new SelectListItem { Text = s.VideoSource, Value = s.Url }).ToList();

            var line = new List<SelectListItem>();
            line.Add(new SelectListItem { Text = "A", Value = "http://www.wq114.org/x2/tong.php?url=" });
            line.Add(new SelectListItem { Text = "B", Value = "https://jx.618g.com/?url=" });
            line.Add(new SelectListItem { Text = "C", Value = "http://www.wmxz.wang/video.php?url=" });
            line.Add(new SelectListItem { Text = "D", Value = "http://www.a305.org/weixin.php?url=" });
            line.Add(new SelectListItem { Text = "E", Value = "http://www.vipjiexi.com/tong.php?url=" });

            return new MoviesView()
            {
                Id = data.Id,
                Name = data.Name,
                Stars = data.Stars,
                Description = data.Description,
                Image = data.ImageUrl,
                Sources = detial,
                Line = line,
                PlayLine = line[0],
                PlaySources = detial[0],
                PlayUrl = "https://jx.618g.com/?url=" + detial[0].Value
            };
        }

        private string GetMovieDiv(MoviesInfo view)
        {
            return string.Format("<div class=\"col-xs-6 col-sm-4 col-md-2\">\r\n" +
                "<div class=\"video-div\">\r\n" +
                "<div class=\"video-div-item\">\r\n" +
                "<div class=\"item-overlay opacity r r-2x bg-black\">\r\n" +
                "<div class=\"center text-center m-t-n\">\r\n" +
                "<a href=\"VideoDetail?id={0}\"><i class=\"fa fa-play-circle i-2x\"></i></a>\r\n" +
                "</div>" +
                "</div>" +
                "<a href=\"VideoDetail?id={0}\"><img class=\"video-div-img\" src=\"{1}\" alt={2} class=\"r r-2x img-full\"></a>\r\n" +
                "</div>\r\n" +
                "<div class=\"padder-v\">\r\n" +
                "<a href=\"VideoDetail?id={0}\" class=\"text-ellipsis\">{2}</a>\r\n" +
                "</div>" +
                "</div>" +
                "</div>", view.Id, view.ImageUrl, view.Name);
        }
    }
}
