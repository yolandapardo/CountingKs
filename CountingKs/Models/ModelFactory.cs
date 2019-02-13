using CountingKs.Data;
using CountingKs.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        private ICountingKsRepository _repo;
        private UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request, ICountingKsRepository repo)
        {
            _urlHelper = new UrlHelper(request);
            _repo = repo;
        }
        public FoodModel Create(Food food)
        {
            return new FoodModel()
            {
                Url = _urlHelper.Link("Food", new { foodid=food.Id}),
                Description = food.Description,
                Measures = food.Measures.Select(m => Create(m))
            };
        }

        public MeasureV2Model Create2(Measure m)
        {
            return new MeasureV2Model()
            {
                Url = _urlHelper.Link("Measures", new { foodid = m.Food.Id, id = m.Id, v=2 }),
                Description = m.Description,
                Calories = m.Calories,
                Carbohydrates=m.Carbohydrates,
                Cholestrol=m.Cholestrol,
                Fiber=m.Fiber,
                Iron=m.Iron,
                Protein=m.Protein,
                SaturatedFat=m.SaturatedFat,
                Sodium=m.Sodium,
                Sugar=m.Sugar,
                TotalFat=m.TotalFat
            };
        }

        public MeasureModel Create (Measure measure)
        {
            return new MeasureModel()
            {
                Url = _urlHelper.Link("Measures", new { foodid=measure.Food.Id, id = measure.Id }),
                Description = measure.Description,
                Calories = measure.Calories
            };
        }

        public DiarySummaryModel CreateSummary(Diary diary)
        {
            return new DiarySummaryModel()
            {
                DiaryDate = diary.CurrentDate,
                TotalCalories = diary.Entries.Sum(e => e.Measure.Calories * e.Quantity)
           };
        }

        public DiaryModel Create(Diary diary)
        {
            return new DiaryModel()
            {
                Links = new List<LinkModel>()
                {
                    CreateLink(_urlHelper.Link("Diaries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }),"self"),
                    CreateLink(_urlHelper.Link("DiaryEntries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }),"newDiaryEntry", "POST")
                },
                CurrentDate = diary.CurrentDate.Date,
                Entries = diary.Entries.Select(e => Create(e))
            };
        }

        public LinkModel CreateLink(string href, string rel, string method="GET", bool isTemplated=false)
        {
            return new LinkModel()
            {
                Href = href,
                Rel = rel,
                Method = method,
                IsTemplated = isTemplated
            };
        }

        public DiaryEntryModel Create(DiaryEntry diary)
        {
            return new DiaryEntryModel()
            {
                Url = _urlHelper.Link("DiaryEntries", new { diaryid = diary.Diary.CurrentDate.ToString("yyyy-MM-dd"), id=diary.Id }),
                FoodDescription = diary.FoodItem.Description,
                MeasureDescription=diary.Measure.Description,
                MeasureUrl= _urlHelper.Link("Measures", new { foodid = diary.Measure.Food.Id, id=diary.Measure.Id }),
                Quantity =diary.Quantity
            };
        }

        public DiaryEntry Parse(DiaryEntryModel model)
        {
            try
            {
                var entry = new DiaryEntry();
                if (model.Quantity != default(double))
                    entry.Quantity = model.Quantity;
                if (!string.IsNullOrWhiteSpace(model.MeasureUrl))
                {
                    var uri = new Uri(model.MeasureUrl);
                    var measureId = int.Parse(uri.Segments.Last());
                    var measure = _repo.GetMeasure(measureId);
                    entry.Measure = measure;
                    entry.FoodItem = measure.Food;
                }
                return entry;
            }
            catch
            {
                return null; 
            }

        }

        public Diary Parse(DiaryModel model)
        {
            try
            {
                var entity = new Diary();

                var selfLink = model.Links.Where(l=>l.Rel=="self").FirstOrDefault();

                if (selfLink != null && !string.IsNullOrWhiteSpace(selfLink.Href))
                {
                    var uri = new Uri(selfLink.Href);
                    entity.Id = int.Parse(uri.Segments.Last());
                }
                entity.CurrentDate = model.CurrentDate;

                if(model.Entries!=null)
                {
                    foreach (var entry in model.Entries)
                        entity.Entries.Add(Parse(entry));
                }
                return entity;
            }
            catch
            {
                return null;
            }

        }


        public AuthTokenModel Create(AuthToken auth)
        {
            return new AuthTokenModel()
            {
                Token = auth.Token,
                Expiration=auth.Expiration
            };
        }
    }
}