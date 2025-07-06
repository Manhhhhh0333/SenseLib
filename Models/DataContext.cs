using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DOAN.Areas.Admin.Controllers;
using DOAN.Areas.Admin.Models;
using Microsoft.EntityFrameworkCore;

namespace DOAN.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<AdminMenu> AdminMenus { get; set; }
        public DbSet<tblMenu> Menus { get; set; }
		public DbSet<tblSliders> Sliders { get; set; }
        public DbSet<tblDocuments> Documents { get; set; }
        public DbSet<tblPublishers> Publishers { get; set; }
        public DbSet<tblAuthors> Authors { get; set; }
        public DbSet<tblCategories> Categories { get; set; }
        public DbSet<tblUsers> Users { get; set; }
        public DbSet<tblComments> Comments { get; set; }
        public DbSet<tblDownload> Downloads { get; set; }
        public DbSet<tblFavorites> Favorites { get; set; }
        public DbSet<tblWebLink> WebLinks { get; set; }
        public DbSet<tblRating> Ratings { get; set; }
        public DbSet<tblTransactions> Transactions { get; set; }
        public DbSet<MomoOrderInfo> MomoOrderInfos { get; set; }
    }
}