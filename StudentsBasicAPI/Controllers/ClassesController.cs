using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentsBasicAPI.DTO;
using StudentsBasicAPI.IRepository;
using StudentsBasicAPI.Models;

namespace StudentsBasicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClassesController> _logger;
        private readonly IMapper _mapper;

        public ClassesController(IUnitOfWork unitOfWork, ILogger<ClassesController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetClasses()
        {
            try
            {
                var classes = await _unitOfWork.Classes.GetAll();
                var results = _mapper.Map<IList<ClassStudentDTO>>(classes);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetClasses)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("{id:int}", Name = "GetClass")]
        public async Task<IActionResult> GetClass(int id)
        {
            try
            {
                var classStudent = await _unitOfWork.Classes.Get(q => q.Id == id, new List<string> { "Students" });
                var result = _mapper.Map<ClassStudentDTO>(classStudent);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetClass)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassStudentDTO classDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateClass)}");
                return BadRequest(ModelState);
            }

            try
            {
                var classStudent = _mapper.Map<ClassStudent>(classDTO);
                await _unitOfWork.Classes.Insert(classStudent);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetClass", new { id = classStudent.Id }, classStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(CreateClass)}");
                return StatusCode(500, $"Something Went Wrong in the {nameof(CreateClass)}");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] UpdateClassStudentDTO classDTO)
        {
            if(!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateClass)}");
                return BadRequest(ModelState);
            }

            try
            {
                var classStudents = await _unitOfWork.Classes.Get(q => q.Id == id);
                if(classStudents == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateClass)}");
                    return BadRequest(ModelState);
                }

                _mapper.Map(classDTO, classStudents);
                _unitOfWork.Classes.Update(classStudents);
                _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateClass)}");
                return StatusCode(500, $"Internal Server Error. Please try again later.");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteClass(int id)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteClass)}");
                return BadRequest(ModelState);
            }

            try
            {
                var classStudents = await _unitOfWork.Classes.Get(q => q.Id == id);
                if (classStudents == null)
                {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteClass)}");
                    return BadRequest(ModelState);
                }

                await _unitOfWork.Classes.Delete(id);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(DeleteClass)}");
                return StatusCode(500, $"Internal Server Error. Please try again later.");
            }
        }

    }
}
