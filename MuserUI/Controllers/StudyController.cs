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
        private readonly IQueryExecutorFactory queryExecutorFactory;
        private readonly IXmlSerialiazer xmlSerialiazer;
        private readonly IJsonSerializer jsonSerializer;

        private const string xmlContentType = "application/xml";
        private const string jsonContentType = "application/json";

        public StudyController(IQueryExecutorFactory queryExecutorFactory, IXmlSerialiazer xmlSerialiazer, IJsonSerializer jsonSerializer)
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
        public void Find(string key)
        {
            using var queryExecutor = queryExecutorFactory.Create();

            var keyValue = queryExecutor.ExecuteAsync<KeyValueHandler, KeyValue>(h => h.FindAsync(key));
            WriteResponse(keyValue);
        }

        [HttpGet]
        public void SelectAsync(string[] keys)
        {
            using var queryExecutor = queryExecutorFactory.Create();

            var keyValue = queryExecutor.ExecuteAsync<KeyValueHandler, KeyValue[]>(h => h.SelectAsync(keys));
            WriteResponse(keyValue);
        }

        [HttpPost]
        public void Create()
        {
            var keyValue = GetFromBody<KeyValue>();
            using var queryExecutor = queryExecutorFactory.Create();

            if (queryExecutor.ExecuteAsync<KeyValueHandler, KeyValue>(h => h.FindAsync(keyValue.Key)) != null)
                throw new HttpException((int)HttpStatusCode.BadRequest,
                    $"Key {keyValue.Key} is already presented in store.");
            queryExecutor.ExecuteAsync<KeyValueHandler>(h => h.CreateAsync(keyValue));
        }

        [HttpPost]
        public void CreateAll()
        {
            var keyValues = GetFromBody<KeyValue[]>();
            using var queryExecutor = queryExecutorFactory.Create();

            foreach (var keyValue in keyValues)
            {
                if (queryExecutor.ExecuteAsync<KeyValueHandler, KeyValue>(h => h.FindAsync(keyValue.Key)) != null)
                    throw new HttpException((int)HttpStatusCode.BadRequest,
                        $"Key {keyValue.Key} is already presented in store.");
            }

            queryExecutor.ExecuteAsync<KeyValueHandler>(h => h.CreateAsync(keyValues));
        }

        [HttpPost]
        public async Task Update(string key, string value)
        {
            using var queryExecutor = queryExecutorFactory.Create();

            var existed = await queryExecutor.ExecuteAsync<KeyValueHandler, KeyValue>(h => h.FindAsync(key)).ConfigureAwait(true);
            if (existed == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, $"Key {key} is not presented in store.");
            existed.Value = value;
            await queryExecutor.ExecuteAsync<KeyValueHandler>(h => h.UpdateAsync(existed)).ConfigureAwait(true);
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


        private T GetFromBody<T>()
        {
            try
            {
                Request.Body.Seek(0, SeekOrigin.Begin);

                byte[] body;
                using (var streamReader = new StreamReader(Request.Body))
                {
                    body = Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
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

        private void WriteResponse<T>(T response)
        {
            var realContentType = Request.GetAcceptHeaders().Contains(xmlContentType)
                ? xmlContentType
                : jsonContentType;
            var bytes = realContentType == xmlContentType
                ? xmlSerialiazer.Serialize(response)
                : jsonSerializer.Serialize(response);
            Response.Headers.Add("Content-Type", realContentType);
            Response.Body.Write(bytes, 0, bytes.Length);
        }
    }
}