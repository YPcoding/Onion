namespace Application.Common.Models;

/// <summary>
/// 返回结果
/// </summary>
public class Result : IResult
{
    internal Result()
    {
        Errors = new string[] { };
    }
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// 错误信息数组
    /// </summary>
    public string[] Errors { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string Error => string.Join(", ", Errors ?? new string[] { });

    /// <summary>
    /// 状态码 200成功 0失败
    /// </summary>
    public int Code { get { return Succeeded == true ? 200 : 0; } }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get { return Succeeded == true ? "请求成功" : Error; } }



    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }
    public static Task<Result> SuccessAsync()
    {
        return Task.FromResult(new Result(true, Array.Empty<string>()));
    }
    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
    public static Task<Result> FailureAsync(IEnumerable<string> errors)
    {
        return Task.FromResult(new Result(false, errors));
    }
}

/// <summary>
/// 返回结果
/// </summary>
/// <typeparam name="T">泛型结果</typeparam>
public class Result<T> : Result, IResult<T>
{
    /// <summary>
    /// 返回数据
    /// </summary>
    public T? Data { get; set; }

    public static new Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T> { Succeeded = false, Errors = errors.ToArray() };
    }
    public static new async Task<Result<T>> FailureAsync(IEnumerable<string> errors)
    {
        return await Task.FromResult(Failure(errors));
    }
    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }
    public static async Task<Result<T>> SuccessAsync(T data)
    {
        return await Task.FromResult(Success(data));
    }
    public static async Task<Result<T>> SuccessOrFailureAsync(T data, bool isSuccess,IEnumerable<string> errors)
    {
        if (isSuccess)
            return await Task.FromResult(Success(data));
        else
            return await Task.FromResult(Failure(errors));
    }
}
