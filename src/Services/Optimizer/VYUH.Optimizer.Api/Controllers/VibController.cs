using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VYUH.Optimizer.Application.Commands;
using VYUH.Optimizer.Application.Queries;

namespace VYUH.Optimizer.Api.Controllers;

[ApiController]
[Route("api/v1/optimizer/vib")]
public class VibController : ControllerBase
{
    private readonly IMediator _mediator;

    public VibController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("proposals/queue")]
    public async Task<IActionResult> QueueProposal([FromBody] QueueProposalCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("proposals/pending")]
    public async Task<IActionResult> GetPendingProposals()
    {
        var result = await _mediator.Send(new GetPendingProposalsQuery());
        return Ok(result);
    }

    [HttpPost("proposals/{proposalId}/respond")]
    public async Task<IActionResult> RespondToProposal(Guid proposalId, [FromBody] ProposalResponseDto response)
    {
        var command = new RespondToProposalCommand(proposalId, response.Approved);
        var result = await _mediator.Send(command);
        if (result == null)
        {
            return NotFound($"Proposal with ID {proposalId} not found or not in PENDING status.");
        }
        return Ok(result);
    }
}

public class ProposalResponseDto
{
    public bool Approved { get; set; }
}
