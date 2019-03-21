using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace H.EF.Core.Filters
{
    [DataContract]
    public class ExceptionResult
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        [DataMember(Name = "message_type")]
        public string MessageType { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [DataMember(Name = "messagecontent")]
        public string Messagecontent { get; set; }

        /// <summary>
        /// 引发异常的方法
        /// </summary>
        [DataMember(Name = "exception_method")]
        public string ExceptionMethod { get; set; }

        /// <summary>
        /// 引发异常源
        /// </summary>
        [DataMember(Name = "exception_source")]
        public string ExceptionSource { get; set; }


    }
}
