using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BntWeb.Discovery.Models;
using BntWeb.Data;
using BntWeb.Data.Services;
using BntWeb.FileSystems.Media;
using BntWeb.Logging;
using BntWeb.Security;

namespace BntWeb.Discovery.Services
{
    public class DiscoveryService : IDiscoveryService
    {
        private readonly IUserContainer _userContainer;

        public ILogger Logger { get; set; }
        public DiscoveryService(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public List<Models.Discovery> GetDiscoveryByPage(int pageIndex, int pageSize, out int totalCount)
        {
            using (var dbContext = new DiscoveryDbContext())
            {
                var query = dbContext.Discoveries.Where(me => me.Status == DiscoveryStatus.Normal).OrderByDescending(me => me.CreateTime);
                totalCount = query.Count();
                return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public Guid SaveDiscovery(Models.Discovery model,List<Guid> goodsIds)
        {
            bool result;
            using (var dbContext = new DiscoveryDbContext())
            {
                model.LastUpdateTime = DateTime.Now;

                //新增
                if (model.Id == Guid.Empty)
                {
                    model.Id = KeyGenerator.GetGuidKey();
                    model.CreateTime = DateTime.Now;
                    var currentUser = _userContainer.CurrentUser;
                    model.CreateUserId = currentUser.Id;
                    model.CreateName = currentUser.UserName;
                    model.Status = DiscoveryStatus.Normal;
                    dbContext.Discoveries.Add(model);
                }
                else//编辑
                {
                    //删除商品关联
                    var goods = dbContext.DiscoveryGoodsRelations.Where(x => x.DiscoveryId==model.Id).ToList();
                    goods.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);

                    dbContext.Discoveries.Attach(model);
                    dbContext.Entry(model).State = EntityState.Modified;
                }

                //关联商品
                foreach (var goodsId in goodsIds)
                {
                    DiscoveryGoodsRelation relation = new DiscoveryGoodsRelation
                    {
                        Id = KeyGenerator.GetGuidKey(),
                        DiscoveryId = model.Id,
                        GoodsId = goodsId
                    };
                    dbContext.DiscoveryGoodsRelations.Add(relation);
                }

                result = dbContext.SaveChanges() > 0;
            }

            if (!result)
                return Guid.Empty;
            
            return model.Id;
        }

        public List<DiscoveryGoodsRelation> GetDiscoveryRelationGoods(Guid discoveryId)
        {
            using (var dbContext = new DiscoveryDbContext())
            {
                var goods = dbContext.DiscoveryGoodsRelations.Include(dg => dg.Goods).Where(g => g.DiscoveryId.Equals(discoveryId));
                return goods.ToList();
            }
        }

        public void DeleteDiscovery(Guid id)
        {
            using (var dbContext = new DiscoveryDbContext())
            {
                //删除发现文章
                var discovery= dbContext.Discoveries.FirstOrDefault(x => x.Id == id);
                dbContext.Entry(discovery).State = EntityState.Deleted;
                //删除关联商品
                var goods = dbContext.DiscoveryGoodsRelations.Where(x => x.DiscoveryId == id).ToList();
                goods.ForEach(m => dbContext.Entry(m).State = EntityState.Deleted);

                dbContext.SaveChanges();
            }
        }
    }
}
 