using AutoMapper;
using Songify.Simple.Dtos;
using Songify.Simple.Models;

namespace Songify.Simple.Mappings
{
    public class ArtistsProfiles : Profile
    {
        public ArtistsProfiles()
        {
            CreateMap<CreateArtistResource, Artist>();
            CreateMap<UpdateArtistResource, Artist>();

            CreateMap<Artist, CreateArtistResource>();
            CreateMap<Artist, UpdateArtistResource>();
        }
    }
}