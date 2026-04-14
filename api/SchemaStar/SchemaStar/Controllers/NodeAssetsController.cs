using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchemaStar.DataRepositories;
using SchemaStar.DTOs.NodeAsset_DTOs;
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
    public class NodeAssetsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<NodeAssetsController> _logger;
        private readonly INodeAssetRepository _repository;
        private readonly INodeRepository _nodeRepository;

        public NodeAssetsController(IUserService userService, ILogger<NodeAssetsController> logger, INodeAssetRepository repository, INodeRepository nodeRepository)
        {
            _userService = userService;
            _logger = logger;
            _repository = repository;
            _nodeRepository = nodeRepository;
        }

        // GET: api/nodes/{nodeId}/nodeassets
        [HttpGet("/api/nodes/{nodeId}/nodeassets")]
        public async Task<ActionResult<IEnumerable<NodeAssetResponseDTO>>> GetNodeAssets(Guid nodeId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access these NodeAssets");

            byte[] publicIdBytes = nodeId.ToMySqlBinary();

            var nodeAssets = await _repository.GetNodeAssetsByNodeIdAsync(publicIdBytes, (ulong)userId);
            var response = nodeAssets.Select(n => n.ToResponseDTO()).ToList();

            return response;
        }

        // GET: api/nodeassets/{nodeAssetId}
        [HttpGet("{nodeAssetId}")]
        public async Task<ActionResult<NodeAssetResponseDTO>> GetNodeAsset(Guid nodeAssetId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to access this NodeAsset");

            byte[] publicIdBytes = nodeAssetId.ToMySqlBinary();

            var nodeAsset = await _repository.GetNodeAssetByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (nodeAsset == null)
            {
                _logger.LogWarning("NodeAsset {PublicId} not found for User {UserId}", nodeAssetId, userId);
                throw new NotFoundException("NodeAsset does not exists for the user");
            }

            var response = nodeAsset.ToResponseDTO();
            return response;
        }

        // PATCH: api/nodeassets/{nodeAssetId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{nodeAssetId}")]
        public async Task<IActionResult> UpdateNodeAsset(Guid nodeAssetId, NodeAssetUpdateRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to modify this NodeAsset");

            byte[] publicIdBytes = nodeAssetId.ToMySqlBinary();
            var nodeAsset = await _repository.GetNodeAssetByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (nodeAsset == null)
            {
                _logger.LogWarning("NodeAsset {PublicId} for User {UserId} not found", nodeAssetId, userId);
                throw new NotFoundException("NodeAsset does not exist for this user");
            }

            //Update the node asset
            if(request.NodeAssetName != null) nodeAsset.NodeAssetName = request.NodeAssetName;
            if (request.NodeAssetType != null) nodeAsset.NodeAssetType = request.NodeAssetType.Value;
            if (request.NodeAssetSource != null) nodeAsset.NodeAssetSource = request.NodeAssetSource.Value;
            if (request.Url != null) nodeAsset.Url = request.Url;
            if (request.MimeType != null) nodeAsset.MimeType = request.MimeType;
            if (request.FileSize.HasValue) nodeAsset.FileSize = request.FileSize.Value;
            if (request.BlobPath != null) nodeAsset.BlobPath = request.BlobPath;

            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/nodeassets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NodeAssetResponseDTO>> PostNodeAsset(NodeAssetRequestDTO request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to create NodeAsset");

            //Get node internal id and assign it for the node asset
            var nodeId = await _nodeRepository.GetInternalIdByPublicIdAsync(request.NodeId.ToMySqlBinary(), (ulong)userId);

            if (nodeId == null)
            {
                _logger.LogWarning("Node {PublicId} not found for User {UserId}", request.NodeId, userId);
                throw new NotFoundException("Node does not exists for the user");
            }

            var newGuid = Guid.NewGuid();

            var nodeAsset = new NodeAsset
            {
                PublicId = newGuid.ToMySqlBinary(),
                NodeAssetName = request.NodeAssetName,
                NodeAssetType = request.NodeAssetType,
                NodeAssetSource = request.NodeAssetSource,
                Url = request.Url,
                MimeType = request.MimeType,
                FileSize = request.FileSize,
                BlobPath = request.BlobPath,
                NodeId = (ulong)nodeId
            };

            _repository.Add(nodeAsset);
            await _repository.SaveChangesAsync();

            var response = nodeAsset.ToResponseDTO(request.NodeId); //pass the NodeId from the request to the mapper

            return CreatedAtAction(nameof(GetNodeAsset), new { nodeAssetId = response.PublicId }, response);
        }

        // DELETE: api/nodeassets/{publicId}
        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteNodeAsset(Guid publicId)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null) throw new UnauthorizedException("User does not have permission to delete NodeAsset");

            byte[] publicIdBytes = publicId.ToMySqlBinary();

            var nodeAsset = await _repository.GetNodeAssetByPublicIdAsync(publicIdBytes, (ulong)userId);

            if (nodeAsset == null)
            {
                _logger.LogWarning("NodeAsset {PublicId} not found for User {UserId}", publicId, userId);
                throw new NotFoundException("NodeAsset does not exists");
            }

            _repository.Delete(nodeAsset);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("NodeAsset {PublicId} has been deleted by User {UserId}", nodeAsset.PublicId, userId);
            
            return NoContent();
        }

    }
}
