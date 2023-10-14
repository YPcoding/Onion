using Quartz;
namespace Application.Common.Jobs;

/// <summary>
/// 定时任务参数抽象类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class JobParameterAbstractBase<T> where T : class
{
    public virtual T? GetParameters<T>(IJobExecutionContext context) where T : class
    {
        if (context.JobDetail == null || context.JobDetail.JobDataMap == null)
        {
            return null;
        }
        var parameter = context.JobDetail.JobDataMap.GetString("parameter");
        if (parameter == null) return null;

        return parameter.FromJson<T>();
    }
}
