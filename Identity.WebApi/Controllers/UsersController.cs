using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.Domain.Service;
using Identity.Domain.Service.Communication;
using Identity.WebApi.Controllers.Resources;
using Identity.WebApi.model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Route("Registration")]
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserCredentialsResource userCredentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateTime utcTime = userCredentials.Birthday; // convert it to Utc using timezone setting of server computer
            TimeZoneInfo localZone = TimeZoneInfo.Local;

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(localZone.Id);
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

            var user = _mapper.Map<UserCredentialsResource, AppIdentityUser>(userCredentials);

            //return CreateUserResponse
            var result = await _userService.CreateUserAsync(user, userCredentials.NewPassword, ApplicationRole.Common);

            if (!result.Succeeded)
            {
                return BadRequest(new ServiceResponse<object>
                {
                    isSuccess = result.Succeeded,
                    Message = result.Message,
                    ResultCode = result.ResultCode
                });
            }

            //var userResource = _mapper.Map<AppIdentityUser, UserResource>(response.User);
            return Ok(new ServiceResponse<object>
            {
                isSuccess = result.Succeeded,
                Message = result.Message,
                ResultCode = result.ResultCode
            });
        }

    }
}
