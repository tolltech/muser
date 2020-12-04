using System;
using System.Diagnostics;
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
    public class StudyController : BaseController
    {
        private readonly IQueryExecutorFactory queryExecutorFactory;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IJsonSerializer jsonSerializer;

        private const string xmlContentType = "application/xml";
        private const string jsonContentType = "application/json";

        public StudyController(IQueryExecutorFactory queryExecutorFactory, IXmlSerializer xmlSerializer, IJsonSerializer jsonSerializer)
        {
            this.queryExecutorFactory = queryExecutorFactory;
            this.xmlSerializer = xmlSerializer;
            this.jsonSerializer = jsonSerializer;
        }

        public class Input
        {
            public int K { get; set; }
            public decimal[] Sums { get; set; }
            public int[] Muls { get; set; }
        }

        public class Output
        {
            public decimal SumResult { get; set; }
            public int MulResult { get; set; }
            public decimal[] SortedInputs { get; set; }
        }

        public void GetInputData()
        {
            WriteResponse(new Input
            {
                K = 10,
                Muls = new[] { 1, 4 },
                Sums = new[] { 1.01m, 2.02m }
            });
        }

        [HttpGet]
        public async Task Sleep(int milliseconds = 2000)
        {
            var sw = new Stopwatch();

            sw.Start();
            await Task.Delay(milliseconds).ConfigureAwait(true);
            sw.Stop();

            await WriteResponse(sw.ElapsedMilliseconds).ConfigureAwait(true);
        }

        public async Task WriteAnswer()
        {
            var answer = await GetFromBodyAsync<Output>().ConfigureAwait(true);
        }

        private static readonly Random random = new Random();
        private Random Random => random ?? new Random();

        public void RandomError(int probability)
        {
            ProcessErrorProbability(probability);
        }

        public void RandomError50()
        {
            ProcessErrorProbability(50);
        }

        public void RandomError90()
        {
            ProcessErrorProbability(90);
        }

        public void RandomError0()
        {
            ProcessErrorProbability(0);
        }

        public void RandomError10()
        {
            ProcessErrorProbability(10);
        }

        private void ProcessErrorProbability(int probability)
        {
            if (probability > 100)
            {
                probability = 100;
            }

            if (probability < 0)
            {
                probability = 0;
            }

            var chance = Random.Next(0, 100);
            if (chance <= probability)
            {
                throw new HttpException(429, $"It's error with {probability}% probability!");
            }

            WriteResponse($"Ok! Error with {probability}% has not occured.");
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
            using var queryExecutor = queryExecutorFactory.Create<KeyValueHandler, KeyValue>();

            var keyValue = await queryExecutor.ExecuteAsync(h => h.FindAsync(key)).ConfigureAwait(true);
            await WriteResponse(keyValue).ConfigureAwait(true);
        }

        [HttpGet]
        public async Task SelectAsync(string[] keys)
        {
            using var queryExecutor = queryExecutorFactory.Create<KeyValueHandler, KeyValue> ();

            var keyValue = await queryExecutor.ExecuteAsync(h => h.SelectAsync(keys)).ConfigureAwait(true);
            await WriteResponse(keyValue).ConfigureAwait(true);
        }

        [HttpPost]
        public async Task Create()
        {
            var keyValue = await GetFromBodyAsync<KeyValue>().ConfigureAwait(true);
            using var queryExecutor = queryExecutorFactory.Create<KeyValueHandler, KeyValue> ();

            if (await queryExecutor.ExecuteAsync(h => h.FindAsync(keyValue.Key)).ConfigureAwait(true) != null)
                throw new HttpException((int)HttpStatusCode.BadRequest,
                    $"Key {keyValue.Key} is already presented in store.");
            await queryExecutor.ExecuteAsync(h => h.CreateAsync(keyValue)).ConfigureAwait(true);
        }

        [HttpPost]
        public async Task CreateAll()
        {
            var keyValues = await GetFromBodyAsync<KeyValue[]>().ConfigureAwait(true);
            using var queryExecutor = queryExecutorFactory.Create<KeyValueHandler, KeyValue> ();

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
            using var queryExecutor = queryExecutorFactory.Create<KeyValueHandler, KeyValue> ();

            var existed = await queryExecutor.ExecuteAsync(h => h.FindAsync(key)).ConfigureAwait(true);
            if (existed == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, $"Key {key} is not presented in store.");
            existed.Value = value;
            await queryExecutor.ExecuteAsync(h => h.UpdateAsync(existed)).ConfigureAwait(true);
        }

        [HttpPost]
        public async Task UpdateFromBody()
        {
            var keyValue = await GetFromBodyAsync<KeyValue>().ConfigureAwait(true);
            using var queryExecutor = queryExecutorFactory.Create<KeyValueHandler, KeyValue> ();

            var key = keyValue.Key;

            var existed = await queryExecutor.ExecuteAsync(h => h.FindAsync(key)).ConfigureAwait(true);
            if (existed == null)
                throw new HttpException((int)HttpStatusCode.BadRequest, $"Key {key} is not presented in store.");
            existed.Value = keyValue.Value;
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
                //Request.Body.Seek(0, SeekOrigin.Begin);

                byte[] body;
                using (var streamReader = new StreamReader(Request.Body))
                {
                    body = Encoding.UTF8.GetBytes(await streamReader.ReadToEndAsync().ConfigureAwait(true));
                }

                var contentType = Request.ContentType;
                return contentType == xmlContentType
                    ? xmlSerializer.Deserialize<T>(body)
                    : jsonSerializer.Deserialize<T>(body);
            }
            catch (Exception ex)
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, $"Wrong parameters. {ex.Message}");
            }
        }

        private Task WriteResponse<T>(T response)
        {
            var realContentType = Request.GetAcceptHeaders().Contains(xmlContentType)
                ? xmlContentType
                : jsonContentType;
            var bytes = realContentType == xmlContentType
                ? xmlSerializer.Serialize(response)
                : jsonSerializer.Serialize(response);
            Response.Headers.Add("Content-Type", realContentType);
            return Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}