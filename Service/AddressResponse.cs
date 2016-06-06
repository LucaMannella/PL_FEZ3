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
        public long serverTime;

        
        public AddressResponse(String address, String port, long time)
        {
            this.address = address;
            this.port = port;
            this.serverTime = time;
        }



    }
}
