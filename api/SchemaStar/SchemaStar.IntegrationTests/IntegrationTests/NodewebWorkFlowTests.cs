using Microsoft.AspNetCore.Mvc.Testing;
using SchemaStar.DTOs;
using SchemaStar.DTOs.Authentication_DTOs;
using SchemaStar.DTOs.Edge_DTOs;
using SchemaStar.DTOs.Node_DTOs;
using SchemaStar.DTOs.NodeAsset_DTOs;
using SchemaStar.DTOs.Nodeweb_DTOs;
using SchemaStar.IntegrationTests.Factory;
using System.Net;

namespace SchemaStar.IntegrationTests.IntegrationTests
{
    /// <summary>
    /// Test the work flows of Nodewebs and depending resources
    /// </summary>
    public class NodewebWorkFlowTests : IClassFixture<CustomizeWebApplicationFactory<Program>>
    {
        private readonly CustomizeWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        private const string SeedEmail = "test@example.com";
        private const string SeedPassword = "Password123!";
        public NodewebWorkFlowTests(CustomizeWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
            _output = output;
        }

        [Fact]
        public async Task Nodeweb_Basic_CRUD_WorkFlow_Succeeds() 
        {
            //Create Nodeweb -> Read Nodeweb -> Update Nodeweb -> Delete Nodeweb -> Confirm deletion

            //Login with test(seed) user
            var loginRequest = new TokenRequestModel { Email = SeedEmail, Password = SeedPassword };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/token", loginRequest);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginBody = await loginResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
            Assert.NotNull(loginBody);
            Assert.Equal(SeedEmail, loginBody.Email);

            var userPublicId = loginBody.PublicId;

            //Create Nodeweb

            var nodeWebRequest = new NodewebRequestDTO { NodeWebName = "my_test_nodeweb" };
            var nodeWebResponse = await _client.PostAsJsonAsync("/api/nodewebs", nodeWebRequest);
            Assert.Equal(HttpStatusCode.Created, nodeWebResponse.StatusCode);

            var nodeWebBody = await nodeWebResponse.Content.ReadFromJsonAsync<NodewebResponseDTO>();
            Assert.NotNull(nodeWebBody);
            Assert.Equal("my_test_nodeweb", nodeWebBody.NodeWebName);

            var nodeWebPublicId = nodeWebBody.PublicId;

            //Read Nodeweb

            var readNodewebResponse = await _client.GetAsync($"/api/nodewebs/{nodeWebPublicId}");
            Assert.Equal(HttpStatusCode.OK, readNodewebResponse.StatusCode);

            var readNodewebBody = await readNodewebResponse.Content.ReadFromJsonAsync<NodewebResponseDTO>();
            Assert.NotNull(readNodewebBody);
            Assert.Equal(nodeWebPublicId, readNodewebBody.PublicId);

            //Update Nodeweb

            var updateNodewebRequest = new NodewebRequestDTO { NodeWebName = "my_test_nodeweb_updated" };
            var updateNodewebResponse = await _client.PatchAsJsonAsync($"/api/nodewebs/{nodeWebPublicId}", updateNodewebRequest);
            Assert.Equal(HttpStatusCode.NoContent, updateNodewebResponse.StatusCode);

            var checkUpdatedNodewebResponse = await _client.GetAsync($"/api/nodewebs/{nodeWebPublicId}");
            var checkUpdatedNodewebBody = await checkUpdatedNodewebResponse.Content.ReadFromJsonAsync<NodewebResponseDTO>();

            Assert.NotNull(checkUpdatedNodewebBody);
            Assert.Equal("my_test_nodeweb_updated", checkUpdatedNodewebBody.NodeWebName);

            //Delete Nodeweb
            var deleteNodewebResponse = await _client.DeleteAsync($"/api/nodewebs/{nodeWebPublicId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteNodewebResponse.StatusCode);

            //Confirm Deletion
            var confirmDeletionResponse = await _client.GetAsync($"/api/nodewebs/{nodeWebPublicId}");
            Assert.Equal(HttpStatusCode.NotFound, confirmDeletionResponse.StatusCode);
        }

        [Fact]
        public async Task Nodeweb_Relational_WorkFlow_Succeeds() 
        {
            /*
             * Create Nodeweb -> Create 2 nodes -> Create Node Assets for each Node 
             * -> Create Edge Connecting the 2 nodes 
             * -> Bulk Update positions of Nodes -> Read Full Nodeweb -> Read Full Node
             * -> Delete Nodeweb -> Confirm cascade deletions on Nodes, NodeAssets and Edges
             */

            //Login with test(seed) user
            var loginRequest = new TokenRequestModel { Email = SeedEmail, Password = SeedPassword };
            var loginResponse = await _client.PostAsJsonAsync("/api/users/token", loginRequest);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginBody = await loginResponse.Content.ReadFromJsonAsync<UserResponseDTO>();
            Assert.NotNull(loginBody);
            Assert.Equal(SeedEmail, loginBody.Email);

            var userPublicId = loginBody.PublicId;

            //Create Nodeweb

            var nodeWebRequest = new NodewebRequestDTO { NodeWebName = "my_test_nodeweb" };
            var nodeWebResponse = await _client.PostAsJsonAsync("/api/nodewebs", nodeWebRequest);
            Assert.Equal(HttpStatusCode.Created, nodeWebResponse.StatusCode);

            var nodeWebBody = await nodeWebResponse.Content.ReadFromJsonAsync<NodewebResponseDTO>();
            Assert.NotNull(nodeWebBody);
            Assert.Equal("my_test_nodeweb", nodeWebBody.NodeWebName);

            var nodeWebPublicId = nodeWebBody.PublicId;

            //Create Nodes

            var nodeRequest_A = new NodeRequestDTO
            {
                NodeName = "node_a",
                PositionX = -500,
                PositionY = 1000,
                State = Models.Enums.NodeState.Unlocked,
                NodeWebId = nodeWebPublicId,
            };

            var nodeRequest_B = new NodeRequestDTO
            {
                NodeName = "node_b",
                PositionX = 650,
                PositionY = -123,
                State = Models.Enums.NodeState.Unlocked,
                NodeWebId = nodeWebPublicId,
            };

            var nodeResponse_A = await _client.PostAsJsonAsync("/api/nodes", nodeRequest_A);
            var nodeResponse_B = await _client.PostAsJsonAsync("/api/nodes", nodeRequest_B);

            Assert.Equal(HttpStatusCode.Created, nodeResponse_A.StatusCode);
            Assert.Equal(HttpStatusCode.Created, nodeResponse_B.StatusCode);

            var nodeBody_A = await nodeResponse_A.Content.ReadFromJsonAsync<NodeResponseDTO>();
            Assert.NotNull(nodeBody_A);

            var nodeBody_B = await nodeResponse_B.Content.ReadFromJsonAsync<NodeResponseDTO>();
            Assert.NotNull(nodeBody_B);

            var nodePublicId_A = nodeBody_A.PublicId;
            var nodePublicId_B = nodeBody_B.PublicId;

            //Create NodeAssets for Node

            var nodeAssetRequest_A = new NodeAssetRequestDTO
            {
                NodeAssetName = "node_asset_a",
                NodeAssetType = Models.Enums.NodeAssetEnums.NodeAssetType.Link,
                NodeAssetSource = Models.Enums.NodeAssetEnums.NodeAssetSource.External,
                Url = "welcome@example.com",
                NodeId = nodePublicId_A,
            };

            var nodeAssetRequest_B = new NodeAssetRequestDTO
            {
                NodeAssetName = "node_asset_b",
                NodeAssetType = Models.Enums.NodeAssetEnums.NodeAssetType.Image,
                NodeAssetSource = Models.Enums.NodeAssetEnums.NodeAssetSource.Upload,
                FileSize = 86,
                BlobPath = "example/path",
                NodeId = nodePublicId_B,
            };

            var nodeAssetResponse_A = await _client.PostAsJsonAsync("/api/nodeassets", nodeAssetRequest_A);
            var nodeAssetResponse_B = await _client.PostAsJsonAsync("/api/nodeassets", nodeAssetRequest_B);

            Assert.Equal(HttpStatusCode.Created, nodeAssetResponse_A.StatusCode);
            Assert.Equal(HttpStatusCode.Created, nodeAssetResponse_B.StatusCode);

            var nodeAssetBody_A = await nodeAssetResponse_A.Content.ReadFromJsonAsync<NodeAssetResponseDTO>();
            Assert.NotNull(nodeAssetBody_A);

            var nodeAssetBody_B = await nodeAssetResponse_B.Content.ReadFromJsonAsync<NodeAssetResponseDTO>();
            Assert.NotNull(nodeAssetBody_B);

            var nodeAssetPublicId_A = nodeAssetBody_A.PublicId;
            var nodeAssetPublicId_B = nodeAssetBody_B.PublicId;

            //Create an edge connecting both nodes

            var edgeRequest = new EdgeRequestDTO 
            {
                FromNodeId = nodePublicId_A,
                ToNodeId = nodePublicId_B,
                NodeWebId = nodeWebPublicId
            };

            var edgeResponse = await _client.PostAsJsonAsync("/api/edges", edgeRequest);
            Assert.Equal(HttpStatusCode.Created, edgeResponse.StatusCode);

            var edgeBody = await edgeResponse.Content.ReadFromJsonAsync<EdgeResponseDTO>();
            Assert.NotNull(edgeBody);

            var edgePublicId = edgeBody.PublicId;

            //Bulk Update Position of Nodes
            var updateNodeRequest_A = new NodeBulkUpdateRequestDTO { PositionX = 242, PositionY = -5353, PublicId = nodePublicId_A};
            var updateNodeRequest_B = new NodeBulkUpdateRequestDTO { PositionX = -7878, PositionY = 9807, PublicId = nodePublicId_B };

            var listOfNodes = new List<NodeBulkUpdateRequestDTO> { updateNodeRequest_A, updateNodeRequest_B };

            var bulkUpdateNodesResponse = await _client.PatchAsJsonAsync($"/api/nodewebs/{nodeWebPublicId}/nodes/bulk", listOfNodes);
            Assert.Equal(HttpStatusCode.NoContent, bulkUpdateNodesResponse.StatusCode);

            var checkNodeUpdateResponse = await _client.GetAsync($"/api/nodes/{nodePublicId_A}");
            Assert.Equal(HttpStatusCode.OK, checkNodeUpdateResponse.StatusCode);
            var checkNodeUpdateBody = await checkNodeUpdateResponse.Content.ReadFromJsonAsync<NodeResponseDTO>();
            Assert.NotNull(checkNodeUpdateBody);
            Assert.Equal(242, checkNodeUpdateBody.PositionX);
            Assert.Equal(-5353, checkNodeUpdateBody.PositionY);

            //Read Full Nodeweb

            var nodeWebFullResponse = await _client.GetAsync($"/api/nodewebs/{nodeWebPublicId}/full");
            Assert.Equal(HttpStatusCode.OK, nodeWebFullResponse.StatusCode);

            var nodeWebFullBody = await nodeWebFullResponse.Content.ReadFromJsonAsync<NodewebFullResponseDTO>();
            Assert.NotNull(nodeWebFullBody);

            Assert.Equal(2, nodeWebFullBody.Nodes.Count);
            Assert.Equal("node_a", nodeWebFullBody.Nodes[0].NodeName);
            Assert.Equal(nodePublicId_A, nodeWebFullBody.Edges[0].FromNodeId);
            Assert.Equal(nodePublicId_B, nodeWebFullBody.Edges[0].ToNodeId);

            //Read Full Node
            var nodeFullResponse = await _client.GetAsync($"/api/nodes/{nodePublicId_A}/full");
            Assert.Equal(HttpStatusCode.OK, nodeFullResponse.StatusCode);

            var nodeFullBody = await nodeFullResponse.Content.ReadFromJsonAsync<NodeFullResponseDTO>();
            Assert.NotNull(nodeFullBody);

            Assert.Equal("node_asset_a", nodeFullBody.NodeAssets[0].NodeAssetName);
            Assert.Equal(nodePublicId_A, nodeFullBody.NodeAssets[0].NodeId);

            //Delete Nodeweb
            var deleteNodewebResponse = await _client.DeleteAsync($"/api/nodewebs/{nodeWebPublicId}");
            Assert.Equal(HttpStatusCode.NoContent, deleteNodewebResponse.StatusCode);

            var checkNodewebDelete = await _client.GetAsync($"/api/nodewebs/{nodeWebPublicId}");
            Assert.Equal(HttpStatusCode.NotFound, checkNodewebDelete.StatusCode);

            //Cacade Delete Checks

            // Nodes, NodeAssets, Edges

            //Confirm Nodes deleted
            var checkNodeDelete_A = await _client.GetAsync($"/api/nodes/{nodePublicId_A}");
            Assert.Equal(HttpStatusCode.NotFound, checkNodeDelete_A.StatusCode);

            var checkNodeDelete_B = await _client.GetAsync($"/api/nodes/{nodePublicId_B}");
            Assert.Equal(HttpStatusCode.NotFound, checkNodeDelete_B.StatusCode);

            //Confirm NodeAssets deleted
            var checkNodeAssetDelete_A = await _client.GetAsync($"/api/nodeassets/{nodeAssetPublicId_A}");
            Assert.Equal(HttpStatusCode.NotFound, checkNodeAssetDelete_A.StatusCode);

            var checkNodeAssetDelete_B = await _client.GetAsync($"/api/nodeassets/{nodeAssetPublicId_B}");
            Assert.Equal(HttpStatusCode.NotFound, checkNodeAssetDelete_B.StatusCode);

            //Confirm Edge deleted
            var checkEdgeDelete = await _client.GetAsync($"/api/edges/{edgePublicId}");
            Assert.Equal(HttpStatusCode.NotFound, checkEdgeDelete.StatusCode);
        }
    }
}
