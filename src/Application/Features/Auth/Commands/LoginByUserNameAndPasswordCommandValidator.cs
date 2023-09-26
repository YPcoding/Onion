namespace Application.Features.Auth.Commands;

public class LoginByUserNameAndPasswordCommandValidator : AbstractValidator<LoginByUserNameAndPasswordCommand>
{
    public LoginByUserNameAndPasswordCommandValidator()
    {
        RuleFor(v => v.UserName)
             .NotEmpty().WithMessage("用户名不能为空");

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("密码不能为空");
    }
}
