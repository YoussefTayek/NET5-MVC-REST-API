using System.Linq;
using System.Collections.Generic;
using Commander.Data;
using Microsoft.AspNetCore.Mvc;
using Commander.Dtos;
using AutoMapper;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Commander.Controllers
{
    //api/commands
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepo _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommanderRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        //GET api/commands
        [HttpGet]
        public ActionResult <IEnumerable<CommandReadDto>> GetAllCommands()
        {  
           var commandItems = _repo.GetAllCommands().ToList();
               return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
            
        }

        //GET api/commands/{id}
        [HttpGet("{id}", Name="GetCommandById")]
        public ActionResult <CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repo.GetCommandById(id);
            if(commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(commandItem));
            }
            return NotFound();
        }

        //POST api/commands
        [HttpPost]
        public ActionResult <CommandReadDto> CreateCommand(CommandCreateDto cmdCreateDto)
        {
            var commandModel = _mapper.Map<Command>(cmdCreateDto);
               _repo.CreateCommand(commandModel);
               _repo.SaveChanges();

               var cmdReadDto = _mapper.Map<CommandReadDto>(commandModel);
               
               return CreatedAtRoute(nameof(GetCommandById), new {Id= cmdReadDto.Id}, cmdReadDto);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto cmdUpdateDto)
        {
            var cmdModelFromRepo = _repo.GetCommandById(id);
            if(cmdUpdateDto == null)
            {
                return NotFound();
            }
            _mapper.Map(cmdUpdateDto,cmdModelFromRepo);
            _repo.UpdateCommand(cmdModelFromRepo);
            _repo.SaveChanges();
            return NoContent();
        }

        //PATCH api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc) 
        {
            var cmdModelFromRepo = _repo.GetCommandById(id);
            if(cmdModelFromRepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDto>(cmdModelFromRepo);
            patchDoc.ApplyTo(commandToPatch, ModelState);
            
            if(!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(commandToPatch, cmdModelFromRepo);

            _repo.UpdateCommand(cmdModelFromRepo);
            _repo.SaveChanges();

            return NoContent();
        }

         //DELETE api/commands/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var cmdModelFromRepo = _repo.GetCommandById(id);
            if(cmdModelFromRepo == null)
            {
                return NotFound();
            }
        
            _repo.DeleteCommand(cmdModelFromRepo);
            _repo.SaveChanges();
            return NoContent();
        }
    }
}