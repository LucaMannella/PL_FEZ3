using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FinalService
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IService1" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        AddressResponse getServerAddressWithPort(string myMacAddress);

        [OperationContract]
        Boolean keepAlive(string myMacAddress, long mycurrentTime, int port);

        [OperationContract]
        Boolean isValid(String mac, byte[] pin);

        // TODO: aggiungere qui le operazioni del servizio
    }

    // Per aggiungere tipi compositi alle operazioni del servizio utilizzare un contratto di dati come descritto nell'esempio seguente.
    // È possibile aggiungere file XSD nel progetto. Dopo la compilazione del progetto è possibile utilizzare direttamente i tipi di dati definiti qui con lo spazio dei nomi "FinalService.ContractType".

}
