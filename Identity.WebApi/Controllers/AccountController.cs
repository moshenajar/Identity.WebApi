using Identity.Domain.Security.Tokens;
using Identity.Domain.Service;
using Identity.WebApi.Controllers.Resources;
using Identity.WebApi.model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(
            IAuthenticationService authenticationService
            ) 
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserResource userCredentials)
        {
            if (ModelState.IsValid)
            {
                //return TokenResponse
                var result = await _authenticationService.CreateAccessTokenAsync(userCredentials.UserName, userCredentials.Password);
                if (result.Succeeded && (result.ResultCode == 0))
                {
                    return Ok(new ServiceResponse<AccessToken>(result.response.Token, result.response.Expiration, result.response.RefreshToken)
                    {
                        isSuccess = result.Succeeded,
                        Message = result.Message,
                        ResultCode = result.ResultCode
                    });
                }
                return BadRequest(new ServiceResponse<object>
                {
                    isSuccess = result.Succeeded,
                    Message = result.Message,
                    Response = null,
                    ResultCode = result.ResultCode
                });
            }

            return BadRequest();
        }

        [Route("token/refresh")]
        [HttpPost]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenResource refreshTokenResource)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.RefreshTokenAsync(refreshTokenResource.Token, refreshTokenResource.UserEmail);
                if (result.Succeeded)
                {
                    return Ok(new ServiceResponse<AccessToken>(result.response.Token, result.response.Expiration, result.response.RefreshToken)
                    {
                        isSuccess = result.Succeeded,
                        Message = result.Message,
                        ResultCode = result.ResultCode
                    });
                }

                return BadRequest(new ServiceResponse<object>
                {
                    isSuccess = result.Succeeded,
                    Message = result.Message,
                    Response = null,
                    ResultCode = result.ResultCode
                });
            }

            return BadRequest(new ServiceResponse<object>
            {
                isSuccess = false,
                Message = "Ended in failure",
                Response = null,
                ResultCode = -1
            });
        }

        [Route("token/revoke")]
        [HttpPost]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenResource revokeTokenResource)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.RevokeToken(revokeTokenResource.Email);
                if (result.Succeeded)
                {
                    return Ok(new ServiceResponse<object>
                    {
                        isSuccess = result.Succeeded,
                        Message = "Successfully completed",
                        ResultCode = 0
                    });
                }
            }

            return BadRequest(new ServiceResponse<object>
            {
                isSuccess = false,
                Message = "Ended in failure",
                Response = null,
                ResultCode = -1
            });
        }

        [Route("token/ValidateToken")]
        [HttpPost]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {

            return Ok(new ServiceResponse<object>
            {
                isSuccess = true,
                Message = "Successfully completed",
                ResultCode = 0
            });
        }
    }
}
