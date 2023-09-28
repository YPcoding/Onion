using System.ComponentModel;

namespace Domain.Entities
{
    /// <summary>
    /// 测试表
    /// </summary>
    [Description("测试表")]
    public class TestTable : BaseAuditableSoftDeleteEntity, IAuditTrial
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        [Description("时间")]
        public DateTime DateTime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Description("Type")]
        public int Type { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public bool Stuts { get; set; }
    }
}
