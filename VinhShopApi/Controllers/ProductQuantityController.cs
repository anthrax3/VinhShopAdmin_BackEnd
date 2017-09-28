using AutoMapper;
using System.Collections.Generic;
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
    [RoutePrefix("api/productQuantity")]
    public class ProductQuantityController : ApiControllerBase
    {
        private IProductQuantityService _productQuantityService;

        public ProductQuantityController(IProductQuantityService productQuantityService,
            IErrorService errorService) : base(errorService)
        {
            _productQuantityService = productQuantityService;
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, int productId, int? sizeId, int? colorId)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productQuantityService.GetAll(productId, sizeId, colorId);
                var responseData = Mapper.Map<IEnumerable<ProductQuantity>, IEnumerable<ProductQuantityViewModel>>(model);
                var response = request.CreateResponse(HttpStatusCode.OK, responseData);
                return response;
            });
        }

        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductQuantityViewModel productQuantityVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var newQuantity = new ProductQuantity();
                    if (_productQuantityService.CheckExist(productQuantityVm.ProductId, productQuantityVm.SizeId, productQuantityVm.ColorId))
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Màu sắc kích thước cho sản phẩm này đã tồn tại");
                    }
                    else
                    {
                        newQuantity.UpdateProductQuantity(productQuantityVm);
                        _productQuantityService.Add(newQuantity);
                        _productQuantityService.Save();
                    }
                }
                return response;
            });
        }

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int productId, int colorId, int sizeId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                _productQuantityService.Delete(productId, colorId, sizeId);
                _productQuantityService.Save();
                response = request.CreateResponse(HttpStatusCode.OK, "Xóa thành công");
                return response;
            });
        }

        [Route("getcolors")]
        [HttpGet]
        public HttpResponseMessage GetColors(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productQuantityService.GetListColor();

                IEnumerable<ColorViewModel> modelVm = Mapper.Map<IEnumerable<Color>, IEnumerable<ColorViewModel>>(model);

                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }

        [Route("getsizes")]
        [HttpGet]
        public HttpResponseMessage GetSizes(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = _productQuantityService.GetListSize();

                List<SizeViewModel> modelVm = Mapper.Map<List<Size>, List<SizeViewModel>>(model);

                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }
    }
}