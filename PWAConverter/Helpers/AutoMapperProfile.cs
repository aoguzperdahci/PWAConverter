using AutoMapper;
using PWAConverter.Entities;
using PWAConverter.Models.Auth;
using PWAConverter.Models.Manifest;
using PWAConverter.Models.Project_;
using PWAConverter.Models.Source;

namespace PWAConverter.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // RegisterRequest -> User
            CreateMap<RegisterRequest, User>();

            CreateMap<CreateProjectModel, Project>();

            CreateMap<UpdateProjectModel, Project>();

            CreateMap<CreateManifestModel, Manifest>();

            CreateMap<UpdateManifestModel, Manifest>();

            CreateMap<CreateSourceModel, Source>();

        }
    }
}
