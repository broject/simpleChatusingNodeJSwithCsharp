using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    public class Model
    {
        public string model { get; set; }
        public Model()
        {
            this.model = GetType().Name;
        }
    }

    public class UserFullModel : Model
    {
        public UserAccount user { get; set; }
        public UserSetting setting { get; set; }
    }

    public class Prompt : Model
    {
        public int status { get; set; }
        public string description { get; set; }
        public string time { get; set; }
    }

    public class Message : Model
    {
        public string from { get; set; }
        public string to { get; set; }
        public int type { get; set; }
        public string data { get; set; }
    }

    public class UserBlock : Model
    {
        public string iam { get; set; }
        public string them { get; set; }
        public int block { get; set; }
    }

    public class UserAccount : Model
    {
        public string id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public int role { get; set; }
        public string password { get; set; }
    }

    public class UserSetting : Model
    {
        public string user_id { get;set;}
        public string mobile { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string address { get; set; }
        public string position { get; set; }
        public string avatar { get; set; }
        public string facebook { get; set; }
        public string twitter { get; set; }
    }
}
