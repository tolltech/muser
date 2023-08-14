using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Study;
using Tolltech.MuserUI.UICore;
using Tolltech.Serialization;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Controllers
{
    [AllowAnonymous]
    [Route("kntr")]
    [EnableCors(Constants.MuserCorsPolicy)]
    public class KntrController : BaseController
    {
        private readonly IQueryExecutorFactory queryExecutorFactory;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IJsonSerializer jsonSerializer;

        private const string xmlContentType = "application/xml";
        private const string jsonContentType = "application/json";

        public KntrController(IQueryExecutorFactory queryExecutorFactory, IXmlSerializer xmlSerializer, IJsonSerializer jsonSerializer)
        {
            this.queryExecutorFactory = queryExecutorFactory;
            this.xmlSerializer = xmlSerializer;
            this.jsonSerializer = jsonSerializer;
        }

        private static readonly Random random = new Random();
        private Random Random => random ?? new Random();

        public void Error(int? statusCode)
        {
            WriteResponse($"Error, isn't it?", statusCode);
        }

        [HttpGet("dateticks/{input}")]
        public JsonResult ConvertDateTicks(string input)
        {
            if (long.TryParse(input, out var l))
            {
                return Json(new DateTime(l).ToString("s"));
            }
            else if (DateTime.TryParse(input, out var d))
            {
                return Json(d.Ticks);                
            }

            return Json(input);
        }
        
        public void Ping()
        {
        }

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

        private Task WriteResponse<T>(T response, int? statusCode = null)
        {
            var realContentType = Request.GetAcceptHeaders().Contains(xmlContentType)
                ? xmlContentType
                : jsonContentType;
            var bytes = realContentType == xmlContentType
                ? xmlSerializer.Serialize(response)
                : jsonSerializer.Serialize(response);
            Response.Headers.Add("Content-Type", realContentType);

            if (statusCode.HasValue)
            {
                Response.StatusCode = statusCode.Value;
            }
            
            return Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}