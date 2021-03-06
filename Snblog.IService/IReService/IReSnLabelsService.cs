using Snblog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snblog.IService.IReService
{
    public interface IReSnLabelsService
    {
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        Task<List<SnLabels>> GetAllAsync();
        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SnLabels> GetByIdAsync(int id);

        /// <summary>
        /// 条件分页查询 - 支持排序
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录条数</param>
        /// <param name="isDesc">是否倒序</param>
        Task<List<SnLabels>> GetfyAllAsync(int pageIndex, int pageSize, bool isDesc);
        /// <summary>
        /// 查询总数
        /// </summary>
        /// <returns></returns>
        Task<int> GetCountAsync();
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <returns></returns>
        Task<SnLabels> AddAsync(SnLabels entity);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<string> UpdateAsync(SnLabels entity);

        /// <summary>
        /// 按id删除
        /// </summary>
        Task<string> DeleteAsync(int id);
    }
}
