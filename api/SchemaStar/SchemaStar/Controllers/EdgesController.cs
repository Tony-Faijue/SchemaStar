using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaStar.DataRepositories;
using SchemaStar.DTOs;
using SchemaStar.DTOs.Edge_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Mappers;
using SchemaStar.Models;
using SchemaStar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchemaStar.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EdgesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<EdgesController> _logger;
        private readonly IEdgeRepository _repository;
        private readonly INodewebRepository _nodewebRepository;
        private readonly INodeRepository _nodeRepository;

        public EdgesController(IUserService userService, ILogger<EdgesController> logger, IEdgeRepository repository, INodewebRepository nodewebRepository, INodeRepository nodeRepository)
        {
            _userService = userService;
            _logger = logger;
            _repository = repository;
            _nodewebRepository = nodewebRepository;
            _nodeRepository = nodeRepository;
        }

        // GET: api/nodewebs/{nodeWebId}/edges
        [HttpGet("/api/nodewebs/{nodeWebId}/edges")]
        public async Task<ActionResult<IEnumerable<EdgeResponseDTO>>> GetEdges(Guid nodeWebId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access these Edges");

            byte[] publicIdBytes = nodeWebId.ToMySqlBinary();

            var edges = await _repository.GetEdgesByNodeWebIdAsync(publicIdBytes, (ulong)userId);
            var response = edges.Select(e => e.ToResponseDTO()).ToList();

            return response;
        }

        // GET: api/edges/{edgeId}
        [HttpGet("{edgeId}")]
        public async Task<ActionResult<EdgeResponseDTO>> GetEdge(Guid edgeId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access this Edge");

            byte[] publicIdBytes = edgeId.ToMySqlBinary();

            var edge = await _repository.GetEdgeByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (edge == null)
            {
                _logger.LogWarning("Edge {PublicId} not found for User {UserId}", edgeId, userId);
                throw new NotFoundException("Edge does not exists for the user");
            }

            var response = edge.ToResponseDTO();
            return response;
        }

        // PATCH: api/edges/{edgeId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{edgeId}")]
        public async Task<IActionResult> UpdateEdge(Guid edgeId, EdgeUpdateRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to modify this Edge");

            byte[] publicIdBytes = edgeId.ToMySqlBinary();
            var edge = await _repository.GetEdgeByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (edge == null)
            {
                _logger.LogWarning("Edge {PublicId} not found for User {UserId}", edgeId, userId);
                throw new NotFoundException("Edge does not exists for the user");
            }

            //Update the edge
            if (request.EdgeType.HasValue) edge.EdgeType = request.EdgeType.Value;
            if (request.UiMetadata != null) edge.UiMetadata = request.UiMetadata;
            
            //Check FromNodeId and ToNodeId
            if (request.FromNodeId.HasValue)
            {
                var fromNodeId = await _nodeRepository.GetInternalIdByPublicIdAsync(request.FromNodeId.Value.ToMySqlBinary(), (ulong)userId);
                if (fromNodeId == null)
                {
                    _logger.LogWarning("FromNode {PublicId} not found for User {UserId}", request.FromNodeId.Value, userId);
                    throw new NotFoundException("FromNode does not exists for the Edge");
                }

                //Update FromNodeId
                edge.FromNodeId = (ulong)fromNodeId;
            }

            if (request.ToNodeId.HasValue)
            {
                var toNodeId = await _nodeRepository.GetInternalIdByPublicIdAsync(request.ToNodeId.Value.ToMySqlBinary(), (ulong)userId);
                if (toNodeId == null)
                {
                    _logger.LogWarning("ToNode {PublicId} not found for User {UserId}", request.ToNodeId.Value, userId);
                    throw new NotFoundException("ToNode does not exists for the Edge");
                }
                //Update ToNodeId
                edge.ToNodeId = (ulong)toNodeId;
            }

            //Check for self-looping with nodes
            if (edge.FromNodeId == edge.ToNodeId)
            {
                _logger.LogWarning("User {UserId} attempted to create a self-loop on Edge {EdgeId}", userId, edgeId);
                throw new ValidationException("An edge cannot connect a node to itself.");
            }

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/edges
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EdgeResponseDTO>> PostEdge(EdgeRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to create Edge");

            //Check if FromNode and ToNode are the same
            if (request.FromNodeId == request.ToNodeId)
            {
                throw new ValidationException("A connection cannot start and end at the same node");
            }

            //Validate the nodeweb and get nodeweb internal id and assign it for the edge
            var nodeWebId = await _nodewebRepository.GetInternalIdByPublicIdAsync(request.NodeWebId.ToMySqlBinary(), (ulong)userId);

            if (nodeWebId == null)
            {
                _logger.LogWarning("Nodeweb {PublicId} not found for User {UserId}", request.NodeWebId, userId);
                throw new NotFoundException("NodeWeb does not exists for the user");
            }

            //Validate the FromNode and get fromnode internal id and assign it for the edge
            var fromNodeId = await _nodeRepository.GetInternalIdByPublicIdAsync(request.FromNodeId.ToMySqlBinary(), (ulong)userId);
            if (fromNodeId == null)
            {
                _logger.LogWarning("FromNode {PublicId} not found for User {UserId}", request.FromNodeId, userId);
                throw new NotFoundException("FromNode does not exists for the Edge");
            }

            //Validate the ToNode and get tomnode internal id and assign it for the edge
            var toNodeId = await _nodeRepository.GetInternalIdByPublicIdAsync(request.ToNodeId.ToMySqlBinary(), (ulong)userId);
            if (toNodeId == null)
            {
                _logger.LogWarning("ToNode {PublicId} not found for User {UserId}", request.ToNodeId, userId);
                throw new NotFoundException("ToNode does not exists for the Edge");
            }

            //Create the new Edge

            var newGuid = Guid.NewGuid();
            var edge = new Edge
            {
                PublicId = newGuid.ToMySqlBinary(),
                NodeWebId = (ulong)nodeWebId,
                FromNodeId = (ulong)fromNodeId,
                ToNodeId = (ulong)toNodeId,
                EdgeType = request.EdgeType ?? Models.Enums.EdgeType.Undirected,
                UiMetadata = request.UiMetadata,
            };

            _repository.Add(edge);
            await _repository.SaveChangesAsync();

            var response = edge.ToResponseDTO(request.NodeWebId); //pass the NodeWebId from the request to the mapper

            return CreatedAtAction(nameof(GetEdge), new { edgeId = response.PublicId }, response);
        }

        // DELETE: api/edges/{publicId}
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteEdge(Guid publicId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to delete Edge");

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var edge = await _repository.GetEdgeByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (edge == null)
            {
                _logger.LogWarning("Edge {PublicId} not found for User {UserId}", publicId, userId);
                throw new NotFoundException("Edge does not exists");
            }

            _repository.Delete(edge);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Edge {PublicId} has been deleted by User {UserId}", edge.PublicId, userId);

            return NoContent();
        }

        // DELETE: api/nodewebs/{nodewebId}/edges/bulk
        [HttpDelete("/api/nodewebs/{nodewebId}/edges/bulk")]
        public async Task<IActionResult> BulkDeleteEdges(Guid nodewebId, [FromBody] List<Guid> edgeIds)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to delete Edges");

            if (edgeIds == null || edgeIds.Count == 0) throw new ArgumentException("List of edge ids cannot be null or empty");

            byte[] nodewebIdBytes = nodewebId.ToMySqlBinary();
            var publicIdsToBytes = edgeIds.Select(id => id.ToMySqlBinary()).ToList();

            //Returns the rows affected
            int deletedCount = await _repository.DeleteEdgesBulkAsync(publicIdsToBytes, nodewebIdBytes, (ulong)userId);

            _logger.LogInformation("{Count} edges deleted from Schema {SchemaId} by {UserId}", deletedCount, nodewebId, userId);
            return NoContent();
        }
    }
}
