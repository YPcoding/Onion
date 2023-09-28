using System.ComponentModel;

namespace Common.Enums;

/// <summary>
/// 代码生成类型
/// </summary>
public enum GenerateCodeType : byte
{
    /// <summary>
    /// 生成所有代码
    /// </summary>
    [Description(@"生成所有代码")]
    GenerateAll,
    /// <summary>
    /// 缓存键值
    /// </summary>
    [Description(@"缓存键值")]
    Caching,
    /// <summary>
    /// 新增
    /// </summary>
    [Description(@"新增")]
    Add,
    /// <summary>
    /// 修改
    /// </summary>
    [Description(@"修改")]
    Update,
    /// <summary>
    /// 删除
    /// </summary>
    [Description(@"删除")]
    Delete,
    /// <summary>
    /// 传输数据对象
    /// </summary>
    [Description(@"传输数据对象")]
    DTOs,
    /// <summary>
    /// 处理事件
    /// </summary>
    [Description(@"处理事件")]
    EventHandlers,
    /// <summary>
    /// 查询所有
    /// </summary>
    [Description(@"查询所有")]
    GetAll,
    /// <summary>
    /// 修改
    /// </summary>
    [Description(@"查询单条")]
    GetById,
    /// <summary>
    /// 分页查询
    /// </summary>
    [Description(@"分页查询")]
    Pagination,
    /// <summary>
    /// 高级过滤
    /// </summary>
    [Description(@"高级过滤")]
    AdvancedFilter,
    /// <summary>
    /// 高级分页规格
    /// </summary>
    [Description(@"高级分页规格")]
    AdvancedPaginationSpec,
    /// <summary>
    /// 单条查询规格
    /// </summary>
    [Description(@"单条查询规格")]
    ByIdSpec,
    /// <summary>
    /// 控制器
    /// </summary>
    [Description(@"控制器")]
    Controller,
    /// <summary>
    /// Api
    /// </summary>
    [Description(@"Api")]
    Api,
    /// <summary>
    /// Hook
    /// </summary>
    [Description(@"Hook")]
    Hook,
    /// <summary>
    /// Rule
    /// </summary>
    [Description(@"Rule")]
    Rule,
    /// <summary>
    /// Types
    /// </summary>
    [Description(@"Types")]
    Types,
    /// <summary>
    /// Form
    /// </summary>
    [Description(@"Form")]
    Form,
    /// <summary>
    /// Index
    /// </summary>
    [Description(@"Index")]
    Index,
}
