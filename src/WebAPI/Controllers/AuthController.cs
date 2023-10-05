using Application.Common.Configurations;
using Application.Common.Interfaces;
using Application.Constants.ClaimTypes;
using Application.Features.Auth.Commands;
using Application.Features.Auth.DTOs;
using Application.Features.Permissions.DTOs;
using Application.Features.Permissions.Queries.GetByUserId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Dynamic;
using System.Text;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 授权管理
    /// </summary>
    public class AuthController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IOptions<JwtSettings> _optJwtSettings;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(
            ITokenService tokenService,
            IOptions<JwtSettings> optJwtSettings,
            ICurrentUserService currentUserService)
        {
            _tokenService = tokenService;
            _optJwtSettings = optJwtSettings;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("LoginByUserNameAndPassword")]
        [AllowAnonymous]

        public async Task<Result<LoginResultDto>> LoginWithUserNameAndPassword(LoginByUserNameAndPasswordCommand command)
        {
            return await Mediator.Send(command);
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        [HttpPost("RefreshToken/{refreshToken}")]
        [AllowAnonymous]
        public async Task<Result<RefreshTokenDto>> RefreshToken(string refreshToken)
        {
            var jwtSecurityToken = await _tokenService.DecodeAsync(refreshToken);
            var claims = jwtSecurityToken?.Claims?.ToArray();
            if (!claims!.Any())
                throw new ArgumentException("无法解析token");

            var refreshExpires = claims!.FirstOrDefault(a => a.Type == ApplicationClaimTypes.RefreshExpires)?.Value;
            if (refreshExpires!.IsNullOrEmpty() || refreshExpires!.ToInt64OrDefault() <= DateTime.Now.ToUnixTimestampSeconds())
                throw new Exception("登录信息已过期");

            //验签
            var securityKey = _optJwtSettings.Value.SecurityKey;
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey)), SecurityAlgorithms.HmacSha256);
            var input = jwtSecurityToken!.RawHeader + "." + jwtSecurityToken.RawPayload;
            if (jwtSecurityToken.RawSignature != JwtTokenUtilities.CreateEncodedSignature(input, signingCredentials))
            {
                throw new Exception("验签失败");
            }

            var userId = claims![0].Value.ToInt64OrDefault();
            var userName = claims[1].Value;

            var newClaims = await _tokenService.CreateClaimsAsync(userId, userName);
            var newToken = await _tokenService.BuildAsync(newClaims, _optJwtSettings.Value);

            var data = new RefreshTokenDto()
            {
                AccessToken = newToken,
                RefreshToken = newToken,
                Expires = DateTime.Now.AddSeconds(_optJwtSettings.Value.ExpireSeconds)
                                      .ToString("yyyy/MM/dd HH:mm:ss")
            };

            return await Result<RefreshTokenDto>.SuccessAsync(data);
        }

        /// <summary>
        /// 获取登录者权限路由
        /// </summary>
        /// <returns></returns>
        [HttpGet("LoginerPermissionRouter")]

        public async Task<Result<IEnumerable<PermissionRouterDto>>> GetLoginerPermissionRouter()
        {
            return await Mediator.Send(new GetLoginerPermissionRouterQuery()
            {
                UserId = _currentUserService.CurrentUserId
            });
        }


        /// <summary>
        /// 获取登录者菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet("Loginer/Muens")]

        public async Task<Result<MuenDto>> GetLoginertMuens()
        {
            return await Result<MuenDto>.SuccessAsync(new MuenDto
            {
                Menu = new List<PermissionMuenDto>()
                 {
                     new PermissionMuenDto
                     {
                         Name= "home",
                         Path="/home",
                         Meta=new MuenMeta() { Title="首页",Icon="el-icon-eleme-filled", Type="menu" },
                         Children=new List<PermissionMuenDto>()
                         {
                             new PermissionMuenDto
                             {
                                 Name= "dashboard",
                                 Path= "/dashboard",
                                 Meta=new MuenMeta() { Title="控制台",Icon="el-icon-menu", Type="menu", Affix=true },
                                 Component = "home"
                             },
                             new PermissionMuenDto
                             {
                                 Name= "userCenter",
                                 Path= "/userCenter",
                                 Meta=new MuenMeta() { Title="帐号信息",Icon="el-icon-user", Type="menu", Tag="NEW" },
                                 Component = "userCenter"
                             }
                         }
                     },
                     new PermissionMuenDto
                     {
                         Name= "vab",
                         Path="/vab",
                         Meta=new MuenMeta() { Title="组件",Icon="el-icon-takeaway-box", Type="menu" },
                         Children=new List<PermissionMuenDto>()
                         {
                             new PermissionMuenDto
                             {
                                 Name= "minivab",
                                 Path= "/vab/mini",
                                 Meta=new MuenMeta() { Title="原子组件",Icon="el-icon-magic-stick", Type="menu" },
                                 Component = "vab/mini"
                             },
                             new PermissionMuenDto
                             {
                                 Name= "iconfont",
                                 Path= "/vab/iconfont",
                                 Meta=new MuenMeta() { Title="扩展图标",Icon="el-icon-orange", Type="menu" },
                                 Component = "vab/iconfont"
                             },
                             new PermissionMuenDto
                             {
                                 Name= "vabdata",
                                 Path= "/vab/data",
                                 Meta=new MuenMeta() { Title="Data 数据展示",Icon="el-icon-histogram", Type="menu" },
                                 Children=new List<PermissionMuenDto>()
                                 {
                                     new PermissionMuenDto
                                     {
                                         Path = "/vab/chart",
                                         Name = "chart",
                                         Meta = new MuenMeta() { Title="图表 Echarts",Type="menu" },
                                         Component = "vab/chart"
                                     },
                                     new PermissionMuenDto
                                     {
                                         Path = "/vab/statistic",
                                         Name = "statistic",
                                         Meta = new MuenMeta() { Title="统计数值",Type="menu" },
                                         Component = "vab/statistic"
                                     },
                                     new PermissionMuenDto
                                     {
                                         Path = "/vab/video",
                                         Name = "scvideo",
                                         Meta = new MuenMeta() { Title="视频播放器",Type="menu" },
                                         Component = "vab/video"
                                     },
                                     new PermissionMuenDto
                                     {
                                         Path = "/vab/qrcode",
                                         Name = "qrcode",
                                         Meta = new MuenMeta() { Title="二维码",Type="menu" },
                                         Component = "vab/qrcode"
                                     }
                                 }
                             }
                         }
                     },
                     new PermissionMuenDto
                     {
                         Name= "template",
                         Path="/template",
                         Meta=new MuenMeta() { Title="模板",Icon="el-icon-files", Type="menu" },
                         Children=new List<PermissionMuenDto>()
                         {
                             new PermissionMuenDto
                             {
                                 Path= "/template/layout",
                                 Name= "layoutTemplate",
                                 Meta=new MuenMeta() { Title="布局",Icon="el-icon-grid", Type="menu" },
                                 Children= new List<PermissionMuenDto>()
                                 {
                                     new PermissionMuenDto
                                     {
                                         Path="/template/layout/blank",
                                         Name="blank",
                                         Meta=new MuenMeta() {Title="空白模板",Type="menu" },
                                         Component="template/layout/blank"
                                     },
                                     new PermissionMuenDto
                                     {
                                         Path="/template/layout/layoutTCB",
                                         Name="layoutTCB",
                                         Meta=new MuenMeta() {Title="上中下布局",Type="menu" },
                                         Component="template/layout/layoutTCB"
                                     },
                                     new PermissionMuenDto
                                     {
                                         Path="/template/layout/layoutLCR",
                                         Name="layoutLCR",
                                         Meta=new MuenMeta() {Title="左中右布局",Type="menu" },
                                         Component="template/layout/layoutLCR"
                                     }
                                 },
                             },
                             new PermissionMuenDto
                             {
                                 Path= "/template/list",
                                 Name= "list",
                                 Meta=new MuenMeta() { Title="列表",Icon="el-icon-document", Type="menu" },
                                 Children=new List<PermissionMuenDto>
                                 {
                                     new PermissionMuenDto
                                     {
                                          Path= "/template/list/crud",
                                          Name= "listCrud",
                                          Meta=new MuenMeta() { Title="CRUD", Type="menu" },
                                          Component="template/list/crud",
                                          Children=new List<PermissionMuenDto>
                                          {
                                              new PermissionMuenDto
                                              {
                                                  Path="/template/list/crud/detail/:id?",
                                                  Name="listCrud-detail",
                                                  Meta=new MuenMeta() { Title="新增/编辑",Hidden=true,Active="/template/list/crud", Type="menu" },
                                                  Component="template/list/crud/detail"
                                              }
                                          }
                                     }
                                 }
                             },
                             new PermissionMuenDto
                             {
                                 Path="/template/other",
                                 Name = "other",
                                 Meta=new MuenMeta() { Title="其他",Icon="el-icon-folder", Type="menu" },
                                 Children=new List<PermissionMuenDto> 
                                 {
                                     new PermissionMuenDto 
                                     {
                                         Path="/template/other/stepform",
                                         Name="stepform",
                                         Meta=new MuenMeta() { Title="分步表单", Type="menu" },
                                         Component = "template/other/stepform",
                                     }
                                 }
                             }
                         }
                     },
                 },
                Permissions = new List<string> {""},
            }) ;
        }
    }
}
