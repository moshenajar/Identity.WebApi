using AutoMapper;
using Identity.Domain.AggregatesModel.IdentityAggregate;
using Identity.WebApi.Controllers.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<UserCredentialsResource, AppIdentityUser>();
            //.ForMember(a => a.PasswordHash, opt => opt.MapFrom(a => a.NewPassword));
            //CreateMap<ForgotPasswordResource, AppIdentityUser>();
        }
    }
}
