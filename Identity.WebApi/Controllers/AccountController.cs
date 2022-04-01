using Identity.Domain.Security.Otp;
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
                //Todo: check if emailUser from token equal with Email request.
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
                //Todo: check if emailUser from token equal with Email request.
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

        [Route("otp/send")]
        [HttpPost]
        public async Task<IActionResult> Otp([FromBody] OtpResource otp)
        {
            //Todo: check if email/phone from user equal with Email/sms request.

            var result = new OtpResponse();
            result.description = "OK";
            result.code = 0;
            result.entities.sent.distributionType = "sms";
            result.entities.sent.status = 0;
            result.entities.sent.message = "OTP נשלח באמצעות sms";
            result.entities.sent.address = "0505586852";
            result.password.expiredTime = 1608579904476;
            result.id = "5fe0e091d937eb7397be6b5f";

            //Todo: Save in DB

            return Ok(new ServiceResponse<OtpResponse>(result.description, result.code, result.entities, result.password, result.id)
            {
                isSuccess = result.entities.sent.status == 0 ? true : false,
                Message = result.entities.sent.message,
                ResultCode = result.code
            });
        }

        [Route("confirm")]
        [HttpPost]
        public async Task<IActionResult> confirm([FromBody] OtpValidateResource otpValidate)
        {
            var result = new OtpValidateResponse();
            //Todo: check if otpValidate.id exsist and validation in DB
            //      and check what distribution Type it is
            //if fails = return BadResponse
            //else -> update user acording to distribution Type

            result.description = "אימות הצליח";
            result.code = 0;
            result.entities.expiredTime = 1608579904476;
            result.entities.attemptsNumber = 1;



            return Ok(new ServiceResponse<OtpValidateResponse>(result.code, result.entities.expiredTime, result.entities.attemptsNumber)
            {
                isSuccess = result.code == 0 ? true : false,
                Message = result.entities.sent.message,
                ResultCode = result.code
            });
        }

    }
}
