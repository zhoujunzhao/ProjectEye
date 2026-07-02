using ProjectEye.Core.Enums;
using System;

namespace ProjectEye.Core.Models.Options
{
    public class RadioBottonModel
    {
        private String _content;
        public String Content
        {
            get { return _content; }
            set { _content = value;}
        }

        private Boolean _isCheck;
        /// <summary>
        /// 单选框是否选中
        /// </summary>
        public Boolean IsCheck
        {
            get { return _isCheck; }
            set { _isCheck = value; }
        }

        private Position _position;
        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

    }

}
