using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionGateway;
using TransactionGateway.Enums;
using System.Threading.Tasks;
using TransactionGateway.ApiModels;

namespace TransactionGateway
{
    public interface IApiClient
    {
        ApiClient SetContent(object objectToSerializeAndSendToApi);
        ApiClient Init(string relativeUrl, string description = "");
        ApiClient AddParam(string key, object value);
        Task<T> Execute<T>() where T : BaseResponse;
        void SetBearerToken(string token);
        ApiClient SetMethod(RequestMethodTypes methodType);       
    }
}
