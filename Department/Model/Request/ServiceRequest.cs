//
//  ServiceRequest.cs
//  PureMVC CSharp Demo - EmployeeAdmin Microservice
//
//  Copyright(c) 2019 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Department.Model.Request
{
    public class ServiceRequest
    {
        public ServiceRequest(HttpContext context)
        {
            Context = context;
        }

        public string GetBody()
        {
            if (RequestData != null) return (string) RequestData;
            using (var reader = new StreamReader(Context.Request.Body))
            {
                RequestData = reader.ReadToEnd();
            }
            return (string) RequestData;
        }

        public T GetJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(GetBody());
        }

        public void SetResultData(int status, object resultData)
        {
            Status = status;
            ResultData = resultData;
        }
        
        public HttpContext Context { get; }
        
        public int Status { get; private set; }

        public object ResultData { get; private set; }
        
        public object RequestData { get; private set; }

        public TaskCompletionSource<object> TaskCompletionSource { get; } = new TaskCompletionSource<object>();
    }
}