using AutoMapper;
using Microsoft.AspNet.Identity;
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
    [Authorize]
    [RoutePrefix("api/function")]
    public class FunctionController : ApiControllerBase
    {
        private IFunctionService _functionService;

        public FunctionController(IErrorService errorService, IFunctionService functionService) : base(errorService)
        {
            this._functionService = functionService;
        }

        [Route("getlisthierarchy")]
        [HttpGet]
        public HttpResponseMessage GetAllHierachy(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                IEnumerable<Function> model;
                if (User.IsInRole("Admin"))
                {
                    model = _functionService.GetAll(string.Empty);
                }
                else
                {
                    model = _functionService.GetAllWithPermission(User.Identity.GetUserId());
                }

                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);
                var parents = modelVm.Where(x => x.Parent == null);
                foreach (var parent in parents)
                {
                    parent.ChildFunctions = modelVm.Where(x => x.ParentId == parent.ID).ToList();
                }
                var response = request.CreateResponse(HttpStatusCode.OK, parents);
                return response;
            });
        }

        [Route("getall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request, string filter = "")
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _functionService.GetAll(filter);
                IEnumerable<FunctionViewModel> modelVm = Mapper.Map<IEnumerable<Function>, IEnumerable<FunctionViewModel>>(model);
                var response = request.CreateResponse(HttpStatusCode.OK, modelVm);
                return response;
            });
        }

        [Route("detail/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetById(HttpRequestMessage request, string id)
        {
            return CreateHttpResponse(request, () =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị");
                }
                var model = _functionService.Get(id);

                var responseData = Mapper.Map<Function, FunctionViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("add")]
        [HttpPost]
        public HttpResponseMessage Create(HttpRequestMessage request, FunctionViewModel functionVm)
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
                    var newFunction = new Function();
                    if (_functionService.CheckExistedId(functionVm.ID))
                    {
                        return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id đã tồn tại");
                    }
                    else
                    {
                        if (functionVm.ParentId == "")
                        {
                            functionVm.ParentId = null;
                        }
                        newFunction.UpdateFunction(functionVm);
                        _functionService.Create(newFunction);
                        _functionService.Save();
                        response = request.CreateResponse(HttpStatusCode.OK, functionVm);
                    }
                    
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        public HttpResponseMessage Update(HttpRequestMessage request, FunctionViewModel functionVm)
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
                    var function = _functionService.Get(functionVm.ID);
                    if (functionVm.ParentId == "")
                    {
                        functionVm.ParentId = null;
                    }
                    function.UpdateFunction(functionVm);
                    _functionService.Update(function);
                    _functionService.Save();
                }

                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        public HttpResponseMessage Delete(HttpRequestMessage request, string id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (string.IsNullOrEmpty(id))
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị");
                }
                _functionService.Delete(id);
                _functionService.Save();

                return response;
            });

        }
    }
}
