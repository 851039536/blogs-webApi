﻿using Snblog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Snblog.IService
{
    public interface ISnSortService
    {

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        List<SnSort> GetSort();

        /// <summary>
        /// 异步查询
        /// </summary>
        /// <returns></returns>
        Task<List<SnSort>> AsyGetSort();

        /// <summary>
        /// 主键id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<SnSort>> AsyGetSortId(int id);


        /// <summary>
        /// 查询用户总数
        /// </summary>
        /// <returns></returns>
        int GetSortCount();


        /// <summary>
        /// 异步添加数据
        /// </summary>
        /// <returns></returns>
        Task<SnSort> AsyInsSort(SnSort test);

        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        Task<string> AysUpSort(SnSort test);

        /// <summary>
        /// 异步按id删除
        /// </summary>
        Task<string> AsyDetSort(int id);
    }
}
