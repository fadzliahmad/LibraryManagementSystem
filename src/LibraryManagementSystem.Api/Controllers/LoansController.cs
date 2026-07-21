using LibraryManagementSystem.Core.DTOs;
using LibraryManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly ILoanService _service;
    public LoansController(ILoanService service) => _service = service;

    /// <summary>Get all loans.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LoanDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var loans = await _service.GetAllAsync();
        return Ok(loans);
    }

    /// <summary>Get a single loan by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var loan = await _service.GetByIdAsync(id);
        return loan is null ? NotFound(new { error = $"Loan with id {id} not found." }) : Ok(loan);
    }

    /// <summary>Borrow a book: creates a loan and decrements available copies.</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(LoanDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateLoanDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (loan, error) = await _service.CreateAsync(dto);
        if (error is not null) return BadRequest(new { error });

        return CreatedAtAction(nameof(GetById), new { id = loan!.Id }, loan);
    }

    /// <summary>Return a book: closes the loan and increments available copies.</summary>
    [HttpPut("{id:int}/return")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Return(int id)
    {
        var (success, error) = await _service.ReturnAsync(id);
        if (success) return NoContent();

        return error == "Loan not found."
            ? NotFound(new { error })
            : BadRequest(new { error });
    }

    /// <summary>Delete a loan record.</summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound(new { error = $"Loan with id {id} not found." });
    }
}
