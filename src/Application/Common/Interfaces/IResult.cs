namespace Application.Common.Interfaces;

public interface IResult
{
    /// <summary>
    /// 错误信息数组
    /// </summary>
    string[] Errors { get; init; }
    /// <summary>
    /// 是否成功
    /// </summary>
    bool Succeeded { get; init; }
    /// <summary>
    /// 状态码
    /// </summary>
    int Code { get; init; }
    /// <summary>
    /// 错误信息
    /// </summary>
    string ErrorMessage { get; }
}
public interface IResult<out T> : IResult
{
    /// <summary>
    /// 返回数据
    /// </summary>
    T? Data { get; }
}
