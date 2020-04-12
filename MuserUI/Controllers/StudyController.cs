using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tolltech.MuserUI.Study;
using Tolltech.MuserUI.UICore;
using Tolltech.Serialization;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Controllers
{
    [AllowAnonymous]
    public class StudyController : Controller
    {
        private readonly IQueryExecutorFactory<KeyValueHandler, KeyValue> queryExecutorFactory;
        private readonly IXmlSerialiazer xmlSerialiazer;
        private readonly IJsonSerializer jsonSerializer;

        private const string xmlContentType = "application/xml";
        private const string jsonContentType = "application/json";

        public StudyController(IQueryExecutorFactory<KeyValueHandler, KeyValue> queryExecutorFactory, IXmlSerialiazer xmlSerialiazer, IJsonSerializer jsonSerializer)
        {
            this.queryExecutorFactory = queryExecutorFactory;
            this.xmlSerialiazer = xmlSerialiazer;
            this.jsonSerializer = jsonSerializer;
        }

        public void Ping()
        {
        }

        [HttpGet]
        public IActionResult Help()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task Find(string key)
        {
            using var queryExecutor = queryExecutorFactory.Create();

            var keyValue = await queryExecutor.ExecuteAsync(h => h.FindAsync(key)).ConfigureAwait(true);
            await WriteResponse(keyValue).ConfigureAwait(true);
        }

        [HttpGet]
        public async Task SelectAsync(string[] keys)
        {
            using var queryExecutor = queryExecutorFactory.Create();

            var keyValue = await queryExecutor.ExecuteAsync(h => h.SelectAsync(keys)).ConfigureAwait(true);
            await WriteResponse(keyValue).ConfigureAwait(true);
        }

        [HttpPost]
        public async Task Create()
        {
            var keyValue = await GetFromBodyAsync<KeyValue>().ConfigureAwait(true);
            using var queryExecutor = queryExecutorFactory.Create();

            if (await queryExecutor.ExecuteAsync(h => h.FindAsync(keyValue.Key)).ConfigureAwait(true) != null)
                throw new HttpException((int)HttpStatusCode.BadRequest,
                    $"Key {keyValue.Key} is already presented in store.");
            await queryExecutor.ExecuteAsync(h => h.CreateAsync(keyValue)).ConfigureAwait(true);
        }

        [HttpPost]
        public async Task CreateAll()
        {
            var keyValues = await GetFromBodyAsync<KeyValue[]>().ConfigureAwait(true);
            using var queryExecutor = queryExecutorFactory.Create();

            foreach (var keyValue in keyValues)
            {
                if (await queryExecutor.ExecuteAsync(h => h.FindAsync(keyValue.Key)).ConfigureAwait(true) != null)
                    throw new HttpException((int)HttpStatusCode.BadRequest,
                        $"Key {keyValue.Key} is already presented in store.");
            }

            await queryExecutor.ExecuteAsync(h => h.CreateAsync(keyValues)).ConfigureAwait(true);
        }

        [HttpPost]
        public async Task Update(string key, string value)
        {
            using var queryExecutor = queryExecutorFactory.Create();

            var existed = await queryExecutor.ExecuteAsync<KeyValue>(h => h.FindAsync(key)).ConfigureAwait(true);
            if (existed == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, $"Key {key} is not presented in store.");
            existed.Value = value;
            await queryExecutor.ExecuteAsync(h => h.UpdateAsync(existed)).ConfigureAwait(true);
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    log.Error(filterContext.Exception?.Message);

        //    if (filterContext.ExceptionHandled)
        //        return;

        //    try
        //    {
        //        if (!(filterContext.Exception is HttpException))
        //        {
        //            throw new HttpException((int)HttpStatusCode.InternalServerError, "Oops...Somthing was wrong.",
        //                filterContext.Exception);
        //        }

        //        throw filterContext.Exception;
        //    }
        //    catch (HttpException ex)
        //    {
        //        HttpContext.Response.StatusCode = ex.GetHttpCode();
        //        var bytes = Encoding.UTF8.GetBytes(ex.Message);
        //        Response.OutputStream.Write(bytes, 0, bytes.Length);
        //        log.Error(ex.Message, ex);
        //    }

        //    filterContext.ExceptionHandled = true;
        //    ControllerContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        //}


        private async Task<T> GetFromBodyAsync<T>()
        {
            try
            {
                Request.Body.Seek(0, SeekOrigin.Begin);

                byte[] body;
                using (var streamReader = new StreamReader(Request.Body))
                {
                    body = Encoding.UTF8.GetBytes(await streamReader.ReadToEndAsync().ConfigureAwait(true));
                }

                var contentType = Request.ContentType;
                return contentType == xmlContentType
                    ? xmlSerialiazer.Deserialize<T>(body)
                    : jsonSerializer.Deserialize<T>(body);
            }
            catch (Exception)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Wrong parameters.");
            }
        }

        private Task WriteResponse<T>(T response)
        {
            var realContentType = Request.GetAcceptHeaders().Contains(xmlContentType)
                ? xmlContentType
                : jsonContentType;
            var bytes = realContentType == xmlContentType
                ? xmlSerialiazer.Serialize(response)
                : jsonSerializer.Serialize(response);
            Response.Headers.Add("Content-Type", realContentType);
            return Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}