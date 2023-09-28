using Domain.Entities;
using Domain.Enums;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.TestTables.DTOs
{
    [Map(typeof(TestTable))]
    public class TestTableDto
    {
     
        /// <summary>
        /// 唯一标识
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public long? TestTableId 
        {
            get 
            {
                return Id;
            }
        }
        
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string? Name { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string? Description { get; set; }
        
        /// <summary>
        /// 时间
        /// </summary>
        [Description("时间")]
        public DateTime? DateTime { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public int? Type { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public bool? Stuts { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// 并发标记
        /// </summary>
        [Description("并发标记")]
        public string? ConcurrencyStamp { get; set; }
    }
}