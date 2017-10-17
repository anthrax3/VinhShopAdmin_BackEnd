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
    [RoutePrefix("api/appRole")]
    [Authorize]
    public class AppRoleController : ApiControllerBase
    {
        private IPermissionService _permissionService;
        private IFunctionService _functionService;
        public AppRoleController(IErrorService errorService, IFunctionService functionService, IPermissionService permissionService) : base(errorService)
        {
            _functionService = functionService;
            _permissionService = permissionService;
        }

        [Route("getlistpaging")]
        [HttpGet]
        public HttpResponseMessage GetListPaging(HttpRequestMessage request, int page, int pageSize, string filter = null)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                int totalRow = 0;
                var query = AppRoleManager.Roles;
                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(r => r.Description == filter);
                }
                totalRow = query.Count();

                var model = query.OrderBy(x => x.Name).Skip((page - 1) * pageSize).Take(pageSize);
                IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(model);
                PaginationSet<ApplicationRoleViewModel> pagedSet = new PaginationSet<ApplicationRoleViewModel>()
                {
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalRows = totalRow,
                    Items = modelVm
                };

                response = request.CreateResponse(HttpStatusCode.OK, pagedSet);
                return response;
            });
        }

        [Route("getlistall")]
        [HttpGet]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                var model = AppRoleManager.Roles.ToList();
                IEnumerable<ApplicationRoleViewModel> modelVm = Mapper.Map<IEnumerable<AppRole>, IEnumerable<ApplicationRoleViewModel>>(model);

                response = request.CreateResponse(HttpStatusCode.OK, modelVm);

                return response;
            });
        }

        [Route("getAllPermission")]
        [HttpGet]
        public HttpResponseMessage GetAllPermission(HttpRequestMessage request, string functionId)
        {
            return CreateHttpResponse(request, () =>
            {
                List<PermissionViewModel> permissions = new List<PermissionViewModel>();
                HttpResponseMessage response = null;
                var roles = AppRoleManager.Roles.Where(r => r.Name != "Admin").ToList();
                var listPermission = _permissionService.GetByFunctionId(functionId).ToList();
                if (listPermission.Count == 0)
                {
                    foreach (var item in roles)
                    {
                        permissions.Add(new PermissionViewModel()
                        {
                            RoleId = item.Id,
                            CanCreate = false,
                            CanDelete = false,
                            CanRead = false,
                            CanUpdate = false,
                            AppRole = new ApplicationRoleViewModel()
                            {
                                Id = item.Id,
                                Description = item.Description,
                                Name = item.Name
                            }
                        });
                    }
                }
                else
                {
                    foreach (var item in roles)
                    {
                        if (!listPermission.Any(x => x.RoleId == item.Id))
                        {
                            permissions.Add(new PermissionViewModel()
                            {
                                RoleId = item.Id,
                                CanCreate = false,
                                CanDelete = false,
                                CanRead = false,
                                CanUpdate = false,
                                AppRole = new ApplicationRoleViewModel()
                                {
                                    Id = item.Id,
                                    Description = item.Description,
                                    Name = item.Name
                                }
                            });
                        }
                        permissions = Mapper.Map<List<Permission>, List<PermissionViewModel>>(listPermission);
                    }
                }
                response = request.CreateResponse(HttpStatusCode.OK, permissions);

                return response;
            });
        }

        [Route("detail/{id:int}")]
        [HttpGet]
        public HttpResponseMessage Detail(HttpRequestMessage request, string id)
        {
            return CreateHttpResponse(request, () =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị");
                }
                var model = AppRoleManager.FindById(id);

                var responseData = Mapper.Map<AppRole, ApplicationRoleViewModel>(model);

                var response = request.CreateResponse(HttpStatusCode.OK, responseData);

                return response;
            });
        }

        [Route("add")]
        [HttpPost]
        public HttpResponseMessage Create(HttpRequestMessage request, ApplicationRoleViewModel applicationRoleViewModel)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                } else
                {
                    var newAppRole = new AppRole();
                    newAppRole.UpdateApplicationRole(applicationRoleViewModel);
                    AppRoleManager.Create(newAppRole);
                    response = request.CreateResponse(HttpStatusCode.OK, applicationRoleViewModel);
                }
                return response;
            });
        }

        [HttpPut]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, ApplicationRoleViewModel applicationRoleViewModel)
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
                    var AppRole = AppRoleManager.FindById(applicationRoleViewModel.Id);
                    AppRole.UpdateApplicationRole(applicationRoleViewModel, "update");
                    AppRoleManager.Update(AppRole);
                    response = request.CreateResponse(HttpStatusCode.OK, applicationRoleViewModel);
                }
                return response;
            });
        }

        [HttpDelete]
        [Route("delete")]
        public HttpResponseMessage Delete(HttpRequestMessage request, string id)
        {
            return CreateHttpResponse(request, () =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, nameof(id) + " không có giá trị");
                }
                var model = AppRoleManager.FindById(id);
                AppRoleManager.Delete(model);
                var response = request.CreateResponse(HttpStatusCode.OK, id);
                return response;
            });
        }
    }
}
