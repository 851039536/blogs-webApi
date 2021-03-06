using Microsoft.EntityFrameworkCore;
using Snblog.IRepository;
using Snblog.IService;
using Snblog.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Snblog.Repository.Repository;

namespace Snblog.Service
{
    public class SnVideoTypeService : BaseService, ISnVideoTypeService
    {
        private readonly SnblogContext _coreDbContext;//DB
        public SnVideoTypeService(IRepositoryFactory repositoryFactory, IConcardContext mydbcontext,SnblogContext coreDbContext) : base(repositoryFactory, mydbcontext)
        {
             _coreDbContext = coreDbContext;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public async Task<bool> AddAsync(SnVideoType Entity)
        {
            await _coreDbContext.SnVideoType.AddAsync(Entity);
            return await _coreDbContext.SaveChangesAsync()>0;
        }

        public async Task<List<SnVideoType>> AsyGetTest()
        {
            return await CreateService<SnVideoType>().GetAll().ToListAsync();
        }

        public async Task<int> CountAsync()
        {
           return await _coreDbContext.SnVideoType.CountAsync();
        }

        public async Task<bool> DeleteAsync(SnVideoType Entity)
        {
             _coreDbContext.SnVideoType.Remove(Entity);
            return await _coreDbContext.SaveChangesAsync()>0;
        }

        public async Task<List<SnVideoType>> GetAllAsync(int id)
        {
            //var data = from s in _coreDbContext.SnVideoType
            //           where s.VId == id
            //           select s;
           return await _coreDbContext.SnVideoType.Where(s => s.VId == id).ToListAsync();
        }

        public async Task<bool> UpdateAsync(SnVideoType Entity)
        {
             _coreDbContext.SnVideoType.Update(Entity);
            return await _coreDbContext.SaveChangesAsync()>0;
        }
    }
}
