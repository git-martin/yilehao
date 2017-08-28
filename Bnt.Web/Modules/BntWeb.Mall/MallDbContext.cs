/* 
    ======================================================================== 
        File name：        MallDbContext
        Module:                
        Author：            罗嗣宝
        Create Time：    2016/7/1 12:43:45
        Modify By:        
        Modify Date:    
    ======================================================================== 
*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BntWeb.ContentMarkup.Models;
using BntWeb.Data;
using BntWeb.Mall.Models;

namespace BntWeb.Mall
{
    public class MallDbContext : BaseDbContext
    {
        public DbSet<GoodsAttribute> GoodsAttributes { set; get; }
        public DbSet<Goods> Goods { get; set; }

        public DbSet<SingleGoods> SingleGoods { get; set; }

        public DbSet<SingleGoodsAttribute> SingleGoodsAttributes { get; set; }
        public DbSet<GoodsCategory> GoodsCategories { get; set; }

        public DbSet<GoodsCategoryRelation> GoodsCategoryRelations { set; get; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<Markup> Markups { get; set; }
        public DbSet<GoodsView> GoodsViews { get; set; }

        public DbSet<Evaluate.Models.Evaluate> Evaluates { get; set; }
        /// <summary>
        /// 商品分类查询视图
        /// </summary>
        public DbSet<GoodsCategoryView> GoodsCategoryView { get; set; }
        /// <summary>
        /// 单品商品分类查询视图
        /// </summary>
        public DbSet<SingleGoodsCategoryView> SingleGoodsCategoryView { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Goods>().HasMany(g => g.SingleGoods).WithOptional().HasForeignKey(g => g.GoodsId);
            //modelBuilder.Entity<SingleGoods>().HasMany(g => g.Attributes).WithOptional().HasForeignKey(g => g.SingleGoodsId);
        }
    }
}