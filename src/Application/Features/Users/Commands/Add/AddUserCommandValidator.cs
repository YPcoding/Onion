namespace Application.Features.Users.Commands.AddEdit;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(v => v.UserName)
             .MaximumLength(20).WithMessage("超出最大长度")
             .NotEmpty().WithMessage("用户名不能为空");
    }
}
