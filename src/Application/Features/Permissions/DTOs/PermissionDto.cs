﻿using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Permissions.DTOs
{
    [Map(typeof(Permission))]
    public class PermissionDto
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 父级节点
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 权限类型
        /// 菜  单 10
        /// 页  面 20
        /// 权限点 30
        /// </summary>
        public PermissionType Type { get; set; }

        /// <summary>
        /// 菜单访问地址
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 接口提交方法
        /// </summary>
        public string? HttpMethods { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 隐藏
        /// </summary>
        public bool? Hidden { get; set; } = false;

        /// <summary>
        /// 启用
        /// </summary>
        public bool? Enabled { get; set; } = true;

        /// <summary>
        /// 可关闭
        /// </summary>
        public bool? Closable { get; set; }

        /// <summary>
        /// 打开组
        /// </summary>
        public bool? Opened { get; set; }

        /// <summary>
        /// 打开新窗口
        /// </summary>
        public bool? NewWindow { get; set; }

        /// <summary>
        /// 链接外显
        /// </summary>
        public bool? External { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string? Group { get; set; }
    }
}
