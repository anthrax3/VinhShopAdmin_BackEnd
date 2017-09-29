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
using VinhShopApi.Providers;
using VinhShopApi.Service;

namespace VinhShopApi.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiControllerBase
    {
        private IOrderService _orderService;

        public OrderController(IOrderService orderService, IErrorService errorService) : base(errorService)
        {
            _orderService = orderService;
        }

        [Route("getlistpaging")]
        [HttpGet]
        [Permission(Action = "Read", Function = "ORDER")]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, string startDate, string endDate,
            string customerName, string paymentStatus, int page, int pageSize, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var model = _orderService.GetList(startDate, endDate, customerName, paymentStatus, page, pageSize, out totalRow);
                List<OrderViewModel> modelVm = Mapper.Map<List<Order>, List<OrderViewModel>>(model);

                var paginationSet = new PaginationSet<OrderViewModel>()
                {
                    Items = modelVm,
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow
                };
                response = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return response;
            });
        }

        [Route("detail/{id}")]
        [HttpGet]
        [Permission(Action = "Read", Function = "ORDER")]
        public HttpResponseMessage Details(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (id == 0)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị");
                }
                var order = _orderService.GetDetail(id);
                if (order == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "Không có dữ liệu");
                }
                else
                {
                    var orderVm = Mapper.Map<Order, OrderViewModel>(order);
                    response = request.CreateResponse(HttpStatusCode.OK, orderVm);
                    return response;
                }
            });
        }

        [Route("getalldetails/{id}")]
        [HttpGet]
        [Permission(Action = "Read", Function = "ORDER")]
        public HttpResponseMessage GetOrderDetails(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (id == 0)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị");
                }
                var order = _orderService.GetOrderDetails(id);
                if (order == null)
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "Không có dữ liệu");
                }
                else
                {
                    var orderVm = Mapper.Map<List<OrderDetail>, List<OrderDetailViewModel>>(order);
                    response = request.CreateResponse(HttpStatusCode.OK, orderVm);
                    return response;
                }
            });
        }

        [HttpPost]
        [Route("add")]
        [Permission(Action = "Create", Function = "USER")]
        public HttpResponseMessage Create(HttpRequestMessage request, OrderViewModel orderVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (ModelState.IsValid)
                {
                    var newOrder = new Order();
                    newOrder.UpdateOrder(orderVm);
                    var listOrderDetails = new List<OrderDetail>();
                    foreach (var item in orderVm.OrderDetails)
                    {
                        listOrderDetails.Add(new OrderDetail()
                        {
                            ColorId = item.ColorId,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            SizeId = item.SizeId
                        });
                    }
                    newOrder.OrderDetails = listOrderDetails;
                    var result = _orderService.Create(newOrder);
                    var model = Mapper.Map<Order, OrderViewModel>(result);
                    response = request.CreateResponse(HttpStatusCode.OK, model);
                }
                else
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                return response;
            });
        }


    }
}
