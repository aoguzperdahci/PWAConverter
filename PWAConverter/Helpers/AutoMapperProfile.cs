using AutoMapper;
using PWAConverter.Entities;
using PWAConverter.Models.Auth;

namespace PWAConverter.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // RegisterRequest -> User
            CreateMap<RegisterRequest, User>();
        }
    }
}
