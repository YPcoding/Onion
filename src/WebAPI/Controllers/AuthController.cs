using Application.Common.Configurations;
using Application.Common.Interfaces;
using Application.Constants.ClaimTypes;
using Application.Features.Auth.Commands;
using Application.Features.Auth.DTOs;
using Application.Features.Menus.DTOs;
using Application.Features.Menus.Queries.GetByUserId;
using Application.Features.Permissions.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
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
                         Id =1710837769615577088,
                         Name= "home",
                         Path="/home",
                         Meta=new MuenMeta() { Title="首页",Icon="el-icon-eleme-filled", Type="menu" },
                         Children=new List<PermissionMuenDto>()
                         {
                             new PermissionMuenDto
                             {
                                 ParentId = 1710837769615577088,
                                 Name= "dashboard",
                                 Path= "/dashboard",
                                 Meta=new MuenMeta() { Title="控制台",Icon="el-icon-menu", Type="menu", Affix=true },
                                 Component = "home"
                             },
                             new PermissionMuenDto
                             {
                                 ParentId = 1710837769615577088,
                                 Name= "userCenter",
                                 Path= "/userCenter",
                                 Meta=new MuenMeta() { Title="帐号信息",Icon="el-icon-user", Type="menu", Tag="NEW" },
                                 Component = "userCenter"
                             }
                         }
                     },
                     //new PermissionMuenDto
                     //{
                     //    Name= "vab",
                     //    Path="/vab",
                     //    Meta=new MuenMeta() { Title="组件",Icon="el-icon-takeaway-box", Type="menu" },
                     //    Children=new List<PermissionMuenDto>()
                     //    {
                     //        new PermissionMuenDto
                     //        {
                     //            Name= "minivab",
                     //            Path= "/vab/mini",
                     //            Meta=new MuenMeta() { Title="原子组件",Icon="el-icon-magic-stick", Type="menu" },
                     //            Component = "vab/mini"
                     //        },
                     //        new PermissionMuenDto
                     //        {
                     //            Name= "iconfont",
                     //            Path= "/vab/iconfont",
                     //            Meta=new MuenMeta() { Title="扩展图标",Icon="el-icon-orange", Type="menu" },
                     //            Component = "vab/iconfont"
                     //        },
                     //        new PermissionMuenDto
                     //        {
                     //            Name= "vabdata",
                     //            Path= "/vab/data",
                     //            Meta=new MuenMeta() { Title="Data 数据展示",Icon="el-icon-histogram", Type="menu" },
                     //            Children=new List<PermissionMuenDto>()
                     //            {
                     //                new PermissionMuenDto
                     //                {
                     //                    Path = "/vab/chart",
                     //                    Name = "chart",
                     //                    Meta = new MuenMeta() { Title="图表 Echarts",Type="menu" },
                     //                    Component = "vab/chart"
                     //                },
                     //                new PermissionMuenDto
                     //                {
                     //                    Path = "/vab/statistic",
                     //                    Name = "statistic",
                     //                    Meta = new MuenMeta() { Title="统计数值",Type="menu" },
                     //                    Component = "vab/statistic"
                     //                },
                     //                new PermissionMuenDto
                     //                {
                     //                    Path = "/vab/video",
                     //                    Name = "scvideo",
                     //                    Meta = new MuenMeta() { Title="视频播放器",Type="menu" },
                     //                    Component = "vab/video"
                     //                },
                     //                new PermissionMuenDto
                     //                {
                     //                    Path = "/vab/qrcode",
                     //                    Name = "qrcode",
                     //                    Meta = new MuenMeta() { Title="二维码",Type="menu" },
                     //                    Component = "vab/qrcode"
                     //                }
                     //            }
                     //        }
                     //    }
                     //},
                     //new PermissionMuenDto
                     //{
                     //    Name= "template",
                     //    Path="/template",
                     //    Meta=new MuenMeta() { Title="模板",Icon="el-icon-files", Type="menu" },
                     //    Children=new List<PermissionMuenDto>()
                     //    {
                     //        new PermissionMuenDto
                     //        {
                     //            Path= "/template/layout",
                     //            Name= "layoutTemplate",
                     //            Meta=new MuenMeta() { Title="布局",Icon="el-icon-grid", Type="menu" },
                     //            Children= new List<PermissionMuenDto>()
                     //            {
                     //                new PermissionMuenDto
                     //                {
                     //                    Path="/template/layout/blank",
                     //                    Name="blank",
                     //                    Meta=new MuenMeta() {Title="空白模板",Type="menu" },
                     //                    Component="template/layout/blank"
                     //                },
                     //                new PermissionMuenDto
                     //                {
                     //                    Path="/template/layout/layoutTCB",
                     //                    Name="layoutTCB",
                     //                    Meta=new MuenMeta() {Title="上中下布局",Type="menu" },
                     //                    Component="template/layout/layoutTCB"
                     //                },
                     //                new PermissionMuenDto
                     //                {
                     //                    Path="/template/layout/layoutLCR",
                     //                    Name="layoutLCR",
                     //                    Meta=new MuenMeta() {Title="左中右布局",Type="menu" },
                     //                    Component="template/layout/layoutLCR"
                     //                }
                     //            },
                     //        },
                     //        new PermissionMuenDto
                     //        {
                     //            Path= "/template/list",
                     //            Name= "list",
                     //            Meta=new MuenMeta() { Title="列表",Icon="el-icon-document", Type="menu" },
                     //            Children=new List<PermissionMuenDto>
                     //            {
                     //                new PermissionMuenDto
                     //                {
                     //                     Path= "/template/list/crud",
                     //                     Name= "listCrud",
                     //                     Meta=new MuenMeta() { Title="CRUD", Type="menu" },
                     //                     Component="template/list/crud",
                     //                     Children=new List<PermissionMuenDto>
                     //                     {
                     //                         new PermissionMuenDto
                     //                         {
                     //                             Path="/template/list/crud/detail/:id?",
                     //                             Name="listCrud-detail",
                     //                             Meta=new MuenMeta() { Title="新增/编辑",Hidden=true,Active="/template/list/crud", Type="menu" },
                     //                             Component="template/list/crud/detail"
                     //                         }
                     //                     }
                     //                }
                     //            }
                     //        },
                     //        new PermissionMuenDto
                     //        {
                     //            Path="/template/other",
                     //            Name = "other",
                     //            Meta=new MuenMeta() { Title="其他",Icon="el-icon-folder", Type="menu" },
                     //            Children=new List<PermissionMuenDto> 
                     //            {
                     //                new PermissionMuenDto 
                     //                {
                     //                    Path="/template/other/stepform",
                     //                    Name="stepform",
                     //                    Meta=new MuenMeta() { Title="分步表单", Type="menu" },
                     //                    Component = "template/other/stepform",
                     //                }
                     //            }
                     //        }
                     //    }
                     //},
                     //new PermissionMuenDto()
                     //{
                     //    Name = "other",
                     //    Path= "/other",
                     //    Meta=new MuenMeta()
                     //    {
                     //        Title="其他",
                     //        Icon="el-icon-more-filled",
                     //        Type="menu",
                     //    },
                     //    Children=new List<PermissionMuenDto>()
                     //    {
                     //        new PermissionMuenDto()
                     //        {
                     //            Path = "/other/directive",
                     //            Name= "directive",
                     //            Meta=new MuenMeta()
                     //            {
                     //                Title="指令",
                     //                Icon="el-icon-price-tag",
                     //                Type="menu",
                     //            },
                     //            Component="other/directive"
                     //        },
                     //        new PermissionMuenDto()
                     //        {
                     //            Path = "/other/viewTags",
                     //            Name= "viewTags",
                     //            Meta=new MuenMeta()
                     //            {
                     //                Title="标签操作",
                     //                Icon="el-icon-files",
                     //                Type="menu",
                     //            },
                     //            Component="other/viewTags",
                     //            Children=new List<PermissionMuenDto>()
                     //            {
                     //                new PermissionMuenDto()
                     //                {
                     //                    Path = "/other/fullpage",
                     //                    Name= "fullpage",
                     //                    Meta=new MuenMeta()
                     //                    {
                     //                        Title="整页路由",
                     //                        Icon="el-icon-monitor",
                     //                        Fullpage=true,
                     //                        Hidden=true,
                     //                        Type="menu",
                     //                    },
                     //                    Component="other/fullpage"
                     //                }    
                     //            }
                     //        },
                     //        new PermissionMuenDto()
                     //        {
                     //            Path="/other/verificate",
                     //            Name="verificate",
                     //            Meta=new MuenMeta()
                     //            {
                     //                Title="表单验证",
                     //                Icon="el-icon-open",
                     //                Type="menu",
                     //            },
                     //            Component="other/verificate"
                     //        },
                     //        new PermissionMuenDto()
                     //        {
                     //            Path="/other/loadJS",
                     //            Name="loadJS",
                     //            Meta=new MuenMeta()
                     //            {
                     //                Title="异步加载JS",
                     //                Icon="el-icon-open",
                     //                Type="menu",
                     //            },
                     //            Component="other/loadJS"
                     //        },
                     //        new PermissionMuenDto()
                     //        {
                     //            Path="/link",
                     //            Name="link",
                     //            Meta=new MuenMeta()
                     //            {
                     //                Title="外部链接",
                     //                Icon="el-icon-link",
                     //                Type="menu",
                     //            },
                     //            Children=new List<PermissionMuenDto>()
                     //            {
                     //                new PermissionMuenDto()
                     //                {
                     //                    Path="https://baidu.com",
                     //                    Name = "百度",
                     //                    Meta=new MuenMeta()
                     //                    {
                     //                        Title="百度",
                     //                        Icon="el-icon-position",
                     //                        Type="link",
                     //                    }
                     //                },
                     //                new PermissionMuenDto()
                     //                {
                     //                    Path="https://www.google.cn",
                     //                    Name = "谷歌",
                     //                    Meta=new MuenMeta()
                     //                    {
                     //                        Title="谷歌",
                     //                        Type="link",
                     //                    }
                     //                }
                     //            }
                     //        },
                     //        new PermissionMuenDto()
                     //        {
                     //            Path="/iframe",
                     //            Name="iframe",
                     //            Meta=new MuenMeta()
                     //            {
                     //                Title="Iframe",
                     //                Icon="el-icon-position",
                     //                Type="menu",
                     //            },
                     //            Children=new List<PermissionMuenDto>()
                     //            {
                     //                new PermissionMuenDto()
                     //                {
                     //                    Path="https://v3.cn.vuejs.org",
                     //                    Name = "vue3",
                     //                    Meta=new MuenMeta()
                     //                    {
                     //                        Title="VUE 3",
                     //                        Type="iframe"
                     //                    }
                     //                },
                     //                new PermissionMuenDto()
                     //                {
                     //                    Path="https://element-plus.gitee.io",
                     //                    Name = "elementplus",
                     //                    Meta=new MuenMeta()
                     //                    {
                     //                        Title="Element Plus",
                     //                        Type="iframe",
                     //                    }
                     //                },
                     //                new PermissionMuenDto()
                     //                {
                     //                    Path="https://lolicode.gitee.io/scui-doc",
                     //                    Name = "scuidoc",
                     //                    Meta=new MuenMeta()
                     //                    {
                     //                        Title="SCUI文档",
                     //                        Type="iframe",
                     //                    }
                     //                }
                     //            }
                     //        }
                     //    }
                     //},
                     //new PermissionMuenDto()
                     //{
                     //    Name="test",
                     //    Path="/test",
                     //    Meta=new MuenMeta()
                     //    {
                     //        Title = "实验室",
                     //        Icon="el-icon-mouse",
                     //        Type="menu",
                     //    },
                     //    Children= new List<PermissionMuenDto>()
                     //    {
                     //        new PermissionMuenDto()
                     //        {
                     //            Path="test/autocode",
                     //            Name="autocode",
                     //            Meta=new MuenMeta()
                     //            {
                     //                Title="代码生成器",
                     //                Icon="sc-icon-code",
                     //                Type="menu",
                     //            },
                     //            Component="test/autocode/index",
                     //            Children= new List<PermissionMuenDto>()
                     //            {
                     //                 new PermissionMuenDto()
                     //                 {
                     //                     Path="/test/autocode/table",
                     //                     Name="autocode-table",
                     //                     Meta=new MuenMeta()
                     //                     {
                     //                         Title="CRUD代码生成",
                     //                         Hidden=true,
                     //                         Active="/test/autocode",
                     //                         Type="menu"
                     //                     },
                     //                     Component="test/autocode/table"
                     //                 },
                     //                 new PermissionMuenDto()
                     //                 {
                     //                     Path="/test/codebug",
                     //                     Name="codebug",
                     //                     Meta=new MuenMeta()
                     //                     {
                     //                         Title="异常处理",
                     //                         Icon="sc-icon-bug-line",
                     //                         Type="menu"
                     //                     },
                     //                     Component="test/codebug"
                     //                 }
                     //            }
                     //        }
                     //    }

                     //},
                     new PermissionMuenDto()
                     {
                         Id =1710837786413764608,
                         Name="setting",
                         Path="/setting",
                         Meta=new MuenMeta()
                         {
                             Title="配置",
                             Icon="el-icon-setting",
                             Type="menu",
                         },
                         Children=new List<PermissionMuenDto>()
                         {
                             new PermissionMuenDto()
                             {
                                 ParentId =1710837786413764608,
                                 Path="/setting/system",
                                 Name="system",
                                 Meta=new MuenMeta()
                                 {
                                     Title="系统设置",
                                     Icon="el-icon-tools",
                                     Type="menu",
                                 },
                                 Component="setting/system",
                             },
                             new PermissionMuenDto()
                             {
                                 ParentId =1710837786413764608,
                                 Path="/setting/user",
                                 Name="user",
                                 Meta=new MuenMeta()
                                 {
                                     Title="用户管理",
                                     Icon="el-icon-user-filled",
                                     Type="menu",
                                 },
                                 Component="setting/user",
                             },
                             new PermissionMuenDto()
                             {
                                 ParentId =1710837786413764608,
                                 Path="/setting/role",
                                 Name="role",
                                 Meta=new MuenMeta()
                                 {
                                     Title="角色管理",
                                     Icon="el-icon-notebook",
                                     Type="menu",
                                 },
                                 Component="setting/role",
                             },
                             new PermissionMuenDto()
                             {

                                 ParentId =1710837786413764608,
                                 Path="/setting/dept",
                                 Name="dept",
                                 Meta=new MuenMeta()
                                 {
                                     Title="部门管理",
                                     Icon="sc-icon-organization",
                                     Type="menu",
                                 },
                                 Component="setting/dept",
                             },
                             new PermissionMuenDto()
                             {ParentId = 1710837786413764608,
                                 Path="/setting/dic",
                                 Name="dic",
                                 Meta=new MuenMeta()
                                 {
                                     Title="字典管理",
                                     Icon="el-icon-document",
                                     Type="menu",
                                 },
                                 Component="setting/dic",
                             },
                             new PermissionMuenDto()
                             {
                                 ParentId =1710837786413764608,
                                 Path="/setting/table",
                                 Name="tableSetting",
                                 Meta=new MuenMeta()
                                 {
                                     Title="表格列管理",
                                     Icon="el-icon-scale-to-original",
                                     Type="menu",
                                 },
                                 Component="setting/table",
                             },
                             new PermissionMuenDto()
                             {ParentId = 1710837786413764608,
                                 Path="/setting/menu",
                                 Name="settingMenu",
                                 Meta=new MuenMeta()
                                 {
                                     Title="菜单管理",
                                     Icon="el-icon-fold",
                                     Type="menu",
                                 },
                                 Component="setting/menu",
                             },
                             new PermissionMuenDto()
                             {
                                 ParentId =1710837786413764608,
                                 Path="/setting/task",
                                 Name="task",
                                 Meta=new MuenMeta()
                                 {
                                     Title="计划任务",
                                     Icon="el-icon-alarm-clock",
                                     Type="menu",
                                 },
                                 Component="setting/task",
                             },
                             new PermissionMuenDto()
                             {ParentId = 1710837786413764608,
                                 Path="/setting/client",
                                 Name="client",
                                 Meta=new MuenMeta()
                                 {
                                     Title="应用管理",
                                     Icon="el-icon-help-filled",
                                     Type="menu",
                                 },
                                 Component="setting/client",
                             },
                             new PermissionMuenDto()
                             {
                                 ParentId =1710837786413764608,
                                 Path="/setting/log",
                                 Name="log",
                                 Meta=new MuenMeta()
                                 {
                                     Title="系统日志",
                                     Icon="el-icon-warning",
                                     Type="menu",
                                 },
                                 Component="setting/log",
                             },
                         }
                     }
                     //,
                     //new PermissionMuenDto()
                     //{
                     //    Path="/other/about",
                     //    Name="about",
                     //    Meta=new MuenMeta()
                     //    {
                     //        Title="关于",
                     //        Icon="el-icon-info-filled",
                     //        Type="menu",
                     //    },
                     //    Component="other/about"
                     //}
                 },
                Permissions = new List<string> {""},
            }) ;
        }


        /// <summary>
        /// 用户菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet("User/Menu")]
        [AllowAnonymous]
        public async Task<Result<UserMenuDto>> GetUserMenus()
        {
            return await Mediator.Send(new GetMenusQueryByUserId()
            {
                UserId = _currentUserService.CurrentUserId
            });
        }
    }
}
