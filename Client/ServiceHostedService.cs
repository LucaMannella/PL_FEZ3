//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     .NET Micro Framework MFSvcUtil.Exe
//     Runtime Version:2.0.00001.0001
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Text;
using System.Xml;
using Dpws.Device;
using Dpws.Device.Services;
using Ws.Services;
using Ws.Services.WsaAddressing;
using Ws.Services.Xml;
using Ws.Services.Binding;
using Ws.Services.Soap;

namespace tempuri.org
{
    
    
    public class IService1 : DpwsHostedService
    {
        
        private IIService1 m_service;
        
        public IService1(IIService1 service, ProtocolVersion version) : 
                base(version)
        {
            // Set the service implementation properties
            m_service = service;

            // Set base service properties
            ServiceNamespace = new WsXmlNamespace("ise", "http://tempuri.org/");
            ServiceID = "urn:uuid:4f54ab6d-6094-4a73-b485-cdf6ee8aa116";
            ServiceTypeName = "IService1";

            // Add service types here
            ServiceOperations.Add(new WsServiceOperation("http://tempuri.org/IService1", "getServerAddressWithPort"));
            ServiceOperations.Add(new WsServiceOperation("http://tempuri.org/IService1", "keepAlive"));
            ServiceOperations.Add(new WsServiceOperation("http://tempuri.org/IService1", "isValid"));

            // Add event sources here
        }
        
        public IService1(IIService1 service) : 
                this(service, new ProtocolVersion10())
        {
        }
        
        public virtual WsMessage getServerAddressWithPort(WsMessage request)
        {
            // Build request object
            getServerAddressWithPortDataContractSerializer reqDcs;
            reqDcs = new getServerAddressWithPortDataContractSerializer("getServerAddressWithPort", "http://tempuri.org/");
            getServerAddressWithPort req;
            req = ((getServerAddressWithPort)(reqDcs.ReadObject(request.Reader)));
            request.Reader.Dispose();
            request.Reader = null;

            // Create response object
            // Call service operation to process request and return response.
            getServerAddressWithPortResponse resp;
            resp = m_service.getServerAddressWithPort(req);

            // Create response header
            WsWsaHeader respHeader = new WsWsaHeader("http://tempuri.org/IService1/getServerAddressWithPortResponse", request.Header.MessageID, m_version.AnonymousUri, null, null, null);
            WsMessage response = new WsMessage(respHeader, resp, WsPrefix.Wsdp);

            // Create response serializer
            getServerAddressWithPortResponseDataContractSerializer respDcs;
            respDcs = new getServerAddressWithPortResponseDataContractSerializer("getServerAddressWithPortResponse", "http://tempuri.org/");
            response.Serializer = respDcs;
            return response;
        }
        
        public virtual WsMessage keepAlive(WsMessage request)
        {
            // Build request object
            keepAliveDataContractSerializer reqDcs;
            reqDcs = new keepAliveDataContractSerializer("keepAlive", "http://tempuri.org/");
            keepAlive req;
            req = ((keepAlive)(reqDcs.ReadObject(request.Reader)));
            request.Reader.Dispose();
            request.Reader = null;

            // Create response object
            // Call service operation to process request and return response.
            keepAliveResponse resp;
            resp = m_service.keepAlive(req);

            // Create response header
            WsWsaHeader respHeader = new WsWsaHeader("http://tempuri.org/IService1/keepAliveResponse", request.Header.MessageID, m_version.AnonymousUri, null, null, null);
            WsMessage response = new WsMessage(respHeader, resp, WsPrefix.Wsdp);

            // Create response serializer
            keepAliveResponseDataContractSerializer respDcs;
            respDcs = new keepAliveResponseDataContractSerializer("keepAliveResponse", "http://tempuri.org/");
            response.Serializer = respDcs;
            return response;
        }
        
        public virtual WsMessage isValid(WsMessage request)
        {
            // Build request object
            isValidDataContractSerializer reqDcs;
            reqDcs = new isValidDataContractSerializer("isValid", "http://tempuri.org/");
            isValid req;
            req = ((isValid)(reqDcs.ReadObject(request.Reader)));
            request.Reader.Dispose();
            request.Reader = null;

            // Create response object
            // Call service operation to process request and return response.
            isValidResponse resp;
            resp = m_service.isValid(req);

            // Create response header
            WsWsaHeader respHeader = new WsWsaHeader("http://tempuri.org/IService1/isValidResponse", request.Header.MessageID, m_version.AnonymousUri, null, null, null);
            WsMessage response = new WsMessage(respHeader, resp, WsPrefix.Wsdp);

            // Create response serializer
            isValidResponseDataContractSerializer respDcs;
            respDcs = new isValidResponseDataContractSerializer("isValidResponse", "http://tempuri.org/");
            response.Serializer = respDcs;
            return response;
        }
    }
}
