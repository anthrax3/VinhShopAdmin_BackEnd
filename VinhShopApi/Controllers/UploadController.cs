using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using VinhShopApi.Infrastructure.Core;
using VinhShopApi.Service;

namespace VinhShopApi.Controllers
{
    [RoutePrefix("api/upload")]
    [Authorize]
    public class UploadController : ApiControllerBase
    {
        public UploadController(IErrorService errorService) : base(errorService)
        {
        }

        [HttpPost]
        [Route("saveImage")]
        public HttpResponseMessage SaveImage(HttpRequestMessage request, string type = "")
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                Dictionary<string, object> dict = new Dictionary<string, object>();
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        response = request.CreateResponse(HttpStatusCode.Created);
                        var postedFile = httpRequest.Files[file];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(extension))
                            {
                                dict.Add("error", string.Format("Please upload images of type .jpg, .gif, .png"));
                                return request.CreateResponse(HttpStatusCode.BadRequest, dict);
                            }
                            else if (postedFile.ContentLength > MaxContentLength)
                            {
                                var message = string.Format("Please Upload a file upto 1 mb.");

                                dict.Add("error", message);
                                return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                            }
                            else
                            {
                                string directory = string.Empty;
                                if (type == "avatar")
                                {
                                    directory = "/UploadedFiles/Avatars/";
                                }
                                else if (type == "product")
                                {
                                    directory = "/UploadedFiles/Products/";
                                }
                                else if (type == "news")
                                {
                                    directory = "/UploadedFiles/News/";
                                }
                                else if (type == "banner")
                                {
                                    directory = "/UploadedFiles/Banners/";
                                }
                                else
                                {
                                    directory = "/UploadedFiles/";
                                }
                                if (!Directory.Exists(HttpContext.Current.Server.MapPath(directory)))
                                {
                                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(directory));
                                }

                                string path = Path.Combine(HttpContext.Current.Server.MapPath(directory), postedFile.FileName);
                                //Userimage myfolder name where i want to save my image
                                postedFile.SaveAs(path);
                                return request.CreateResponse(HttpStatusCode.OK, Path.Combine(directory, postedFile.FileName));
                            }
                        }
                    }
                }
                else
                {
                    var res = string.Format("Please Upload an image.");
                    dict.Add("error", res);
                    response = request.CreateResponse(HttpStatusCode.NotFound, dict);
                }
                return response;
            });
        }
    }
}
