namespace Application.Common.Interfaces;

public interface IResult
{
    string[] Errors { get; init; }

    bool Succeeded { get; init; }
    int Code { get; init; }
}
public interface IResult<out T> : IResult
{
    T? Data { get; }
}
