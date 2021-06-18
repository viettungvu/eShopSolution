using eShopSolution.Application.System.Languages;
using eShopSolution.ViewModels.System.Languages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var languages = await _languageService.GetAll();
            if (languages == null)
                return NotFound("Empty");
            return Ok(languages);
        }

        [HttpPost("new")]
        public async Task<IActionResult> Create([FromForm] LanguageCreateRequest request)
        {
            var languageId = await _languageService.Create(request);
            if (languageId == null)
                return BadRequest("Failed");
            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] string languageId)
        {
            var result = await _languageService.Delete(languageId);
            if (!result)
                return BadRequest("Failed");
            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] LanguageUpdateRequest request)
        {
            var result = await _languageService.Update(request);
            if (!result)
                return BadRequest("Failed");
            return Ok();
        }
    }
}