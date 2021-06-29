using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Songify.Simple.DAL;
using Songify.Simple.Dtos;
using Songify.Simple.Models;

namespace Songify.Simple.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]
    [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
    public class ArtistController : ControllerBase
    {
        private readonly IArtistsRepository _repository;
        private readonly IMapper _mapper;

        public ArtistController(IArtistsRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        /// <summary>
        /// Creates an Artist resource
        /// </summary>
        /// <param name="request">Request body</param>
        /// <returns>Created Artist entity</returns>
        /// <response code="201">Created Artist entity</response>
        [HttpPost]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Artist), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateArtistResource request)
        {
            var artist = _mapper.Map<CreateArtistResource, Artist>(request);

            _repository.Add(artist);
            await _repository.UnitOfWork.SaveChangesAsync();

            return Created($"api/artists/{artist.Id}", request);
        }

        /// <summary>
        /// Gets a specific artist
        /// </summary>
        /// <param name="id">Artist's id</param>
        /// <returns>Existing Artist entity</returns>
        /// <response code="200">Returns existing Artist entity</response>
        [HttpGet]
        [Route("{id}")]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Artist), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var artist = await _repository.GetArtist(id);

            if (artist is null)
            {
                return NotFound();
            }

            return Ok(artist);
        }

        /// <summary>
        /// Removes existing Artist entity
        /// </summary>
        /// <param name="id">Artist's id</param>
        /// <returns>No content response status</returns>
        /// <response code="204">Request processed - status No content</response>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remove(int id)
        {
            _repository.Delete(id);
            await _repository.UnitOfWork.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Updates existing Artist entity
        /// </summary>
        /// <param name="request">Request body</param>
        /// <returns>OK response status</returns>
        /// <response code="200">Request processed - status OK</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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