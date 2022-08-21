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
    public class StudentsController: ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentsController> _logger;
        private readonly IMapper _mapper;

        public StudentsController(IUnitOfWork unitOfWork, ILogger<StudentsController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await _unitOfWork.Students.GetAll();
                var results = _mapper.Map<IList<StudentDTO>>(students);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetStudents)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [HttpGet("{id:int}", Name = "GetStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudent(int id)
        {
            try
            {
                var student = await _unitOfWork.Students.Get(q => q.Id == id, new List<string> { "Country" });
                var result = _mapper.Map<StudentDTO>(student);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(GetStudent)}");
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentDTO studentDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateStudent)}");
                return BadRequest(ModelState);
            }

            try
            {
                var student = _mapper.Map<Student>(studentDTO);
                await _unitOfWork.Students.Insert(student);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetStudent", new { id = student.Id }, student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(CreateStudent)}");
                return StatusCode(500, $"Something Went Wrong in the {nameof(CreateStudent)}");
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDTO studentDTO)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateStudent)}");
                return BadRequest(ModelState);
            }

            try
            {
                var student = await _unitOfWork.Students.Get(q => q.Id == id);
                if (student == null)
                {
                    _logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateStudent)}");
                    return BadRequest(ModelState);
                }

                _mapper.Map(studentDTO, student);
                _unitOfWork.Students.Update(student);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateStudent)}");
                return StatusCode(500, $"Internal Server Error. Please try again later.");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (!ModelState.IsValid || id < 1)
            {
                _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteStudent)}");
                return BadRequest(ModelState);
            }

            try
            {
                var student = await _unitOfWork.Students.Get(q => q.Id == id);
                if (student == null)
                {
                    _logger.LogError($"Invalid DELETE attempt in {nameof(DeleteStudent)}");
                    return BadRequest(ModelState);
                }

                await _unitOfWork.Students.Delete(id);
                await _unitOfWork.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(DeleteStudent)}");
                return StatusCode(500, $"Internal Server Error. Please try again later.");
            }
        }
    }
}
