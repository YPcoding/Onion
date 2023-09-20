﻿namespace Application.Features.Auth.DTOs
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public class LoginResultDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// 角色组
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 访问令牌过期时间
        /// </summary>
        public string Expires 
        { 
            get 
            {
                return DateTime.Now.AddHours(2).ToString("yyyy/MM/dd HH:mm:ss");
            } 
        }
    }
}
