using Snblog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snblog.IService
{
    public interface ISnSortService
    {

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="cache">是否开启缓存</param>
        /// <returns></returns>
        Task<List<SnSort>> GetAllAsync(bool cache);

        /// <summary>
        /// 异步查询
        /// </summary>
        /// <returns></returns>
        Task<List<SnSort>> AsyGetSort();

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="cache">是否开启缓存</param>
        /// <returns></returns>
        Task<SnSort> GetByIdAsync(int id, bool cache);


        /// <summary>
        /// 分页查询 
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录条数</param>
        /// <param name="isDesc">是否倒序</param>
        /// <param name="cache">是否开启缓存</param>
        Task<List<SnSort>> GetFyAllAsync(int pageIndex, int pageSize, bool isDesc, bool cache);

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="cache">是否开启缓存</param>
        /// <returns></returns>
        Task<int> GetCountAsync(bool cache);


        /// <summary>
        /// 异步添加数据
        /// </summary>
        /// <returns></returns>
        Task<bool> AddAsync(SnSort entity);

        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(SnSort test);

        /// <summary>
        /// 异步按id删除
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}
