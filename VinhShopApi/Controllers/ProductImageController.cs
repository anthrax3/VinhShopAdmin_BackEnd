using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VinhShopApi.Infrastructure.Core;
using VinhShopApi.Infrastructure.Extensions;
using VinhShopApi.Model.Models;
using VinhShopApi.Models;
using VinhShopApi.Service;

namespace VinhShopApi.Controllers
{
    [RoutePrefix("api/productImage")]
    public class ProductImageController : ApiControllerBase
    {
        private IProductImageService _productImageService;

        public ProductImageController(IProductImageService productImageService, IErrorService errorService) : base(errorService)
        {
            _productImageService = productImageService;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int productId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productImageService.GetAll(productId);

                IEnumerable<ProductImageViewModel> modelVm = Mapper.Map<IEnumerable<ProductImage>, IEnumerable<ProductImageViewModel>>(model);

                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductImageViewModel productImageVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (ModelState.IsValid)
                {
                    var newImage = new ProductImage();
                    
                    newImage.UpdateProductImage(productImageVm);

                    _productImageService.Add(newImage);
                    _productImageService.Save();
                    response = request.CreateResponse(HttpStatusCode.OK, productImageVm);
                    return response;
                }
                else
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    return response;
                }
            });
        }

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                _productImageService.Delete(id);
                _productImageService.Save();
                response = request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
                return response;
            });
        }
    }
}
