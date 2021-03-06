using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blog.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Snblog.IService;
using Snblog.Jwt;
using Snblog.Models;


//默认的约定集将应用于程序集中的所有操作：
[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Snblog.Controllers
{

    /// <summary>
    /// 用户SnUserController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SnUserController : Controller
    {
        private readonly SnblogContext _coreDbContext;
        private readonly ISnUserService _service; //IOC依赖注入
        private readonly DbSet<SnUser> user;
        private readonly JwtConfig jwtModel = null;

     
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="service"></param>
        /// <param name="coreDbContext"></param>
        /// <param name="_jwtModel"></param>
        public SnUserController(ISnUserService service, SnblogContext coreDbContext, IOptions<JwtConfig> _jwtModel)
        {
            _service = service;
            _coreDbContext = coreDbContext;
            user = coreDbContext.SnUser;
            jwtModel = _jwtModel.Value;
        }


        [HttpGet("Login")]
        public IActionResult Login(string users, string pwd)
        {
            if (string.IsNullOrEmpty(users) && string.IsNullOrEmpty(pwd))
            {
                return Ok("用户密码不能为空");
            }
            var data = from u in user
                       where u.Name == users && u.Pwd == pwd
                       select u.Name;
            if (data.Count() == 0)
            {
                return Ok("用户或密码错误");
            }
            else
            {
                var claims = new List<Claim>();
                claims.AddRange(new[]
                {
                new Claim("UserName", users),
                new Claim(ClaimTypes.Role,users),
                new Claim(JwtRegisteredClaimNames.Sub,users),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });
                DateTime now = DateTime.UtcNow;
                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: jwtModel.Issuer,
                    audience: jwtModel.Audience,
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromMinutes(jwtModel.Expiration)),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtModel.SecurityKey)), SecurityAlgorithms.HmacSha256)
                );
                string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                return Ok(data.Count()+","+token);
            }

        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="cache">缓存</param>
        /// <returns></returns>
        [HttpGet("GetAllAsync")]
        public async Task<IActionResult> GetAllAsync(bool cache)
        {
            return Ok(await _service.GetAllAsync(cache));
        }

        /// <summary>
        /// 主键查询
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="cache">缓存</param>
        /// <returns></returns>
        [HttpGet("GetByIdAsync")]
        public async Task<IActionResult> GetByIdAsync(int id ,bool cache)
        {
            return Ok(await _service.GetByIdAsync(id,cache));
        }

        /// <summary>
        /// 查询总数
        /// </summary>
        /// <param name="cache">缓存</param>
        /// <returns></returns>
        [HttpGet("GetCountAsync")]
        public async Task<IActionResult> GetCountAsync(bool cache)
        {
            return Ok(await _service.GetCountAsync(cache));
        }

        /// <summary>
        /// 条件分页查询 - 支持排序
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录条数</param>
        /// <param name="isDesc">是否倒序</param>
        [HttpGet("GetPagingUser")]
        public IActionResult GetPagingUser(int pageIndex, int pageSize, bool isDesc)
        {
            return Ok(_service.GetPagingUser(1, pageIndex, pageSize, out _, isDesc));
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Permissions.Name)] 
        [HttpPost("AsyInsArticle")]
        public async Task<ActionResult<SnUser>> AsyInsArticle(SnUser user)
        {
            return Ok(await _service.AsyInsUser(user));
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = Permissions.Name)]
        [HttpDelete("AsyDetUserId")]
        public async Task<IActionResult> AsyDetUserId(int userId)
        {
            return Ok(await _service.AsyDetUserId(userId));
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize(Roles = Permissions.Name)]
        [HttpPut("AysUpUser")]
        public async Task<IActionResult> AysUpUser(SnUserDto user)
        {
            var data = await _service.AysUpUser(user);
            return Ok(data);
        }
        
    }
}
