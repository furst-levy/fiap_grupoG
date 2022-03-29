using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace Fiap.GrupoG.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadDocument(
            [FromForm] IFormFile file
        )
        {
            await AwsUtils.UploadFileToS3(file);
            return await Task.FromResult(Ok());
        }
    }
}
