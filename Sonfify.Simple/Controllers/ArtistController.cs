using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Songify.Simple.DAL;
using Songify.Simple.Dtos;
using Songify.Simple.Models;

namespace Songify.Simple.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistsRepository _repository;
        private readonly IMapper _mapper;

        public ArtistController(IArtistsRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateArtistResource request)
        {
            var artist = _mapper.Map<CreateArtistResource, Artist>(request);

            _repository.Add(artist);
            await _repository.UnitOfWork.SaveChangesAsync();

            return Created($"api/artists/{artist.Id}", request);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var artist = await _repository.GetArtist(id);

            if (artist is null)
            {
                return NotFound();
            }

            return Ok(artist);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            _repository.Delete(id);
            await _repository.UnitOfWork.SaveChangesAsync();

            //good practice to return NoContent();
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateArtistResource request)
        {
            var artist = await _repository.GetArtist(request.Id);

            if (artist is null)
            {
                return NotFound();
            }

            _mapper.Map(request, artist);

            _repository.Update(artist);

            await _repository.UnitOfWork.SaveChangesAsync();

            return Ok();
        }
    }
}