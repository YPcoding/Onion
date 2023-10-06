using Domain.Entities;
using Domain.Enums;
using Domain.Entities.Departments;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Departments.DTOs
{
    [Map(typeof(Department))]
    public class DepartmentDto
    {    

        /// <summary>
        /// 部门名称
        /// </summary>
        [Description("部门名称")]
        public string DepartmentName { get; set; }    

        /// <summary>
        /// 排序
        /// </summary>
        [Description("排序")]
        public int Sort { get; set; }    

        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }    

        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public bool? IsActive { get; set; }    

        /// <summary>
        /// 上级节点
        /// </summary>
        [Description("上级节点")]
        public long? SuperiorId { get; set; }    

        /// <summary>
        /// 上级节点
        /// </summary>
        [Description("上级节点")]
        public Department? Superior { get; set; }     

        /// <summary>
        /// 唯一标识
        /// </summary>
        public long DepartmentId 
        {
            get 
            {
                return Id;
            }
        }    

        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public long Id { get; set; }    

        /// <summary>
        /// 乐观并发标记
        /// </summary>
        [Description("乐观并发标记")]
        public string ConcurrencyStamp { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public virtual DateTime? Created { get; set; }
    }
}