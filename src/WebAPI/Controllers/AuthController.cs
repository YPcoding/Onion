using Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    public class AuthController : ApiControllerBase
    {
        /// <summary>
        /// 通过用户名和密码登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("LoginByUserNameAndPassword")]
        [AllowAnonymous]

        public async Task<Result<string>> LoginWithUserNameAndPassword(LoginByUserNameAndPasswordCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}
