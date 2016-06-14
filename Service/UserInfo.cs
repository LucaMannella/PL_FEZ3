using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Service
{
    [DataContractAttribute]
    public class UserInfo
    {
        [DataMember]
        public String email;
        [DataMember]
        public String pin;
        [DataMember]
        public Boolean exist;

        public UserInfo(String email, String pin, Boolean exist)
        {
            this.email = email;
            this.pin = pin;
            this.exist = exist;
            if (!exist)
            {
                this.email = null;
                this.pin = null;
            }
        }

    }
}
