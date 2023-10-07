using Domain.ValueObjects;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [Description("菜单管理")]
    public class Menu : BaseAuditableSoftDeleteEntity, IAuditTrial
    {
        /// <summary>
        /// 父级节点
        /// </summary>
        [Description("父级节点")]
        public virtual long? ParentId { get; set; }
        [ForeignKey("SuperiorId")]
        public virtual Menu? Parent { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public virtual string? Path { get; set; }


        /// <summary>
        /// 重定向
        /// </summary>
        public virtual string? Redirect { get; set; }

        /// <summary>
        /// 菜单高亮
        /// </summary>
        public virtual string? Active { get; set; }

        /// <summary>
        /// 菜单高亮
        /// </summary>
        public virtual Meta? Meta { get; set; }

        [NotMapped]
        public List<Menu> Children { get; set; } = new List<Menu>();
    }
}
