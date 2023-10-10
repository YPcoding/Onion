using Domain.Entities.Loggers;

namespace Application.Features.Loggers.DTOs
{
    [Map(typeof(Logger))]
    public class LoggerDto
    {     

        /// <summary>
        /// 
        /// </summary>
        public int LoggerId 
        {
            get 
            {
                return Id;
            }
        }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public int Id { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Timestamp { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Level { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Template { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Message { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Exception { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Properties { get; set; }    

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public DateTime? TS { get; set; }
    }
}