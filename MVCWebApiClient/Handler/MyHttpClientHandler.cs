﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MVCWebApiClient.Handler
{
    public class MyHttpClientHandler1 : DelegatingHandler
    {
        public MyHttpClientHandler1(HttpMessageHandler httpMessageHandler)
        {
            InnerHandler = httpMessageHandler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> responseTask = base.SendAsync(request, cancellationToken).ContinueWith(
                (requestTask) =>
                {
                    Debug.WriteLine(">>> Process MyHttpClientHandler1 Request");
                    var responseMsg = requestTask.Result;
                    responseMsg.Content = new StringContent($"{responseMsg.Content.ReadAsStringAsync().Result} .... Hello from HttpClient Message Handler 1");
                    Debug.WriteLine(">>> Process MyHttpClientHandler1 Response");
                    return responseMsg;
                },
                TaskContinuationOptions.OnlyOnRanToCompletion);

            return responseTask;
        }
    }

    public class MyHttpClientHandler2 : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {            
            var task = new Task<HttpResponseMessage>(
                () => {
                    Debug.WriteLine(">>> Process MyHttpClientHandler2 Request");
                    var responseMsg = new HttpResponseMessage();
                    responseMsg.Content = new StringContent("Hello from HttpClient Message Handler 2");
                    Debug.WriteLine(">>> Process MyHttpClientHandler2 Response");
                    return responseMsg;
                });
            task.Start();
            return task;            
        }
    }
}