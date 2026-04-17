using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaStar.DataRepositories;
using SchemaStar.DTOs.Node_DTOs;
using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.Exceptions;
using SchemaStar.Mappers;
using SchemaStar.Models;
using SchemaStar.Services;

namespace SchemaStar.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<NodesController> _logger;
        private readonly INodeRepository _repository;
        private readonly INodewebRepository _nodeWebRepository;

        public NodesController(IUserService userService, ILogger<NodesController> logger, INodeRepository repository, INodewebRepository nodeWebRepository)
        {
            _userService = userService;
            _logger = logger;
            _repository = repository;
            _nodeWebRepository = nodeWebRepository;
        }

        // GET: api/nodewebs/{nodeWebId}/nodes
        [HttpGet("/api/nodewebs/{nodeWebId}/nodes")]
        public async Task<ActionResult<IEnumerable<NodeResponseDTO>>> GetNodes(Guid nodeWebId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access these Nodes");

            byte[] publicIdBytes = nodeWebId.ToMySqlBinary();

            var nodes = await _repository.GetNodesByNodeWebIdAsync(publicIdBytes, (ulong)userId);
            var response = nodes.Select(n => n.ToResponseDTO()).ToList();

            return response;
        }

        // GET: api/nodes/{nodeId}
        [HttpGet("{nodeId}")]
        public async Task<ActionResult<NodeResponseDTO>> GetNode(Guid nodeId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access this Node");

            byte[] publicIdBytes = nodeId.ToMySqlBinary();

            var node = await _repository.GetNodeByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (node == null)
            {
                _logger.LogWarning("Node {PublicId} not found for User {UserId}", nodeId, userId);
                throw new NotFoundException("Node does not exists for the user");
            }

            var response = node.ToResponseDTO();

            return response;
        }

        // PATCH: api/nodes/{nodeId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{nodeId}")]
        public async Task<IActionResult> UpdateNode(Guid nodeId, NodeUpdateRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to modify this Node");

            byte[] publicIdBytes = nodeId.ToMySqlBinary();
            var node = await _repository.GetNodeByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (node == null)
            {
                _logger.LogWarning("Node {PublicId} for User {UserId} not found", nodeId, userId);
                throw new NotFoundException("Node does not exist for this user");
            }

            //Update the node
            if (request.NodeName != null) node.NodeName = request.NodeName;
            if (request.NodeDescription != null) node.NodeDescription = request.NodeDescription;
            if (request.PositionX.HasValue) node.PositionX = request.PositionX.Value;
            if (request.PositionY.HasValue) node.PositionY = request.PositionY.Value;
            if (request.Width.HasValue) node.Width = request.Width.Value;
            if (request.Height.HasValue) node.Height = request.Height.Value;
            if (request.State != null) node.State = request.State.Value;

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/nodes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NodeResponseDTO>> PostNode(NodeRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to create Node");

            //Get nodeweb internal id and assign it for the node
            var nodeWebId = await _nodeWebRepository.GetInternalIdByPublicIdAsync(request.NodeWebId.ToMySqlBinary(), (ulong)userId);

            if (nodeWebId == null)
            {
                _logger.LogWarning("NodeWeb {PublicId} not found for User {UserId}", request.NodeWebId, userId);
                throw new NotFoundException("NodeWeb does not exists for the user");
            }

            var newGuid = Guid.NewGuid();

            var node = new Node
            {
                PublicId = newGuid.ToMySqlBinary(),
                NodeName = request.NodeName,
                NodeDescription = request.NodeDescription,
                PositionX = request.PositionX,
                PositionY = request.PositionY,
                Width = request.Width,
                Height = request.Height,
                State = request.State,
                NodeWebId = (ulong)nodeWebId
            };

            _repository.Add(node);
            await _repository.SaveChangesAsync();

            var response = node.ToResponseDTO(request.NodeWebId); //pass the NodeWebId from the request to the mapper

            return CreatedAtAction(nameof(GetNode), new { nodeId = response.PublicId }, response);
        }

        // DELETE: api/nodes/{publicId}
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteNode(Guid publicId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to delete Node");

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var node = await _repository.GetNodeByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (node == null)
            {
                _logger.LogWarning("Node {PublicId} not found for User {UserId}", publicId, userId);
                throw new NotFoundException("Node does not exists");
            }

            _repository.Delete(node);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Node {PublicId} has been deleted by User {UserId}", node.PublicId, userId);

            return NoContent();
        }

        // GET: api/nodes/{publicId}/full
        [HttpGet("{publicId}/full")]
        public async Task<ActionResult<NodeFullResponseDTO>> GetNodeFull(Guid publicId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access the Node");

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            //Eager load the node
            var node = await _repository.GetFullNodeByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (node == null)
            {
                _logger.LogWarning("Node {PublicId} not found for User {UserId}", publicId, userId);
                throw new NotFoundException("Node does not exists for the user");
            }

            var response = node.ToFullResponseDTO();
            return response; //use FullResponseDTO to get eager loaded Node
        }

        // PATCH: api/nodewebs/{nodewebId}/nodes/bulk
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("/api/nodewebs/{nodewebId}/nodes/bulk")]
        public async Task<IActionResult> BulkUpdateNodes(Guid nodewebId, [FromBody] List<NodeBulkUpdateRequestDTO> request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to modify these Nodes");

            byte[] publicIdBytes = nodewebId.ToMySqlBinary();

            //Update the nodes
            var nodeUpdates = request.Select(n => new Node { 
                PublicId = n.PublicId.ToMySqlBinary(),
                NodeName = n.NodeName!,
                NodeDescription = n.NodeDescription,
                PositionX = n.PositionX ?? 0,
                PositionY = n.PositionY ?? 0,
                Width = n.Width ?? 0,
                Height = n.Height ?? 0,
                State = n.State ?? Models.Enums.NodeState.Unlocked
            }).ToList();
            
            await _repository.UpdateNodesBulkAsync(nodeUpdates, publicIdBytes, (ulong)userId);

            return NoContent();
        }

        // DELETE: api/nodewebs/{nodewebId}/nodes/bulk
        [HttpDelete("/api/nodewebs/{nodewebId}/nodes/bulk")]
        public async Task<IActionResult> BulkDeleteNodes(Guid nodewebId, [FromBody] List<Guid> nodeIds)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to delete Nodes");

            if (nodeIds == null | nodeIds!.Count == 0) throw new ArgumentException("List of node ids cannot be null or empty");

            byte[] nodewebIdBytes = nodewebId.ToMySqlBinary();
            var publicIdsToBytes = nodeIds.Select(id => id.ToMySqlBinary()).ToList();

            //Returns the rows affected
            int deletedCount = await _repository.DeleteNodesBulkAsync(publicIdsToBytes, nodewebIdBytes, (ulong)userId);

            _logger.LogInformation("{Count} nodes deleted from Schema {SchemaId} by {UserId}", deletedCount, nodewebId, userId);
            return NoContent();
        }
    }
}
