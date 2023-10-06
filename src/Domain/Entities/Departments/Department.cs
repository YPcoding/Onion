using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Departments
{
    /// <summary>
    /// 部门管理
    /// </summary>
    [Description("部门管理")]
    public class Department : BaseAuditableSoftDeleteEntity, IAuditTrial
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [Description("部门名称")]
        public virtual string? DepartmentName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public virtual int Sort { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public virtual string? Description { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// 上级节点
        /// </summary>
        [Description("上级节点")]
        public virtual long? SuperiorId { get; set; } = null;

        /// <summary>
        /// 上级节点
        /// </summary>
        [Description("上级节点")]
        [ForeignKey("SuperiorId")]
        public Department? Superior { get; set; } = null;
    }
}
