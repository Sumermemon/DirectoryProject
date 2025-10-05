using DirectoryProject.Entity;
using Microsoft.EntityFrameworkCore;
using System;

namespace DirectoryProject.DBHelper
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        #region [Table]
        public DbSet<UsersMasters> UsersMasters { get; set; }
        public DbSet<DirectoryMaster> DirectoryMaster { get; set; }
        #endregion
    }

}
