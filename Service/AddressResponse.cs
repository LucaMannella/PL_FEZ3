using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [DataContractAttribute]
    public class AddressResponse
    {
        [DataMember]
        public String address;
        [DataMember]
        public String port;
        [DataMember]
        public String serverTime;

        
        public AddressResponse(String address, String port, String time)
        {
            this.address = address;
            this.port = port;
            this.serverTime = time;
        }



    }
}
