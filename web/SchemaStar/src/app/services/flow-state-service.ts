import { computed, inject, Injectable, signal } from '@angular/core';
import { MapDataService } from './map-data-service';
import { FCreateConnectionEvent, FMoveNodesEvent, FSelectionChangeEvent } from '@foblex/flow';
import { NodeRequest, NodeResponse, NodeService, UpdateNode } from './schema/node-service';
import { EdgeRequest, EdgeResponse, EdgeService, UpdateEdge } from './schema/edge-service';
import { LoggerService } from './logger-service';
import { NodeAssetRequest, NodeAssetResponse, NodeAssetService, UpdateNodeAsset } from './schema/node-asset-service';

@Injectable({
  providedIn: 'root',
})
export class FlowStateService {

   private loggerService = inject(LoggerService);
   private mapData = inject(MapDataService);
   private edgeService = inject(EdgeService);
   private nodeService = inject(NodeService);
   private nodeAssetService = inject(NodeAssetService);
   

   /**
    * Selected Node Ids
    */
   selectedNodeIds = signal<string[]>([]);
   /**
    * Selected Edge Ids
    */
   selectedEdgeIds = signal<string[]>([]);

   /**
    * Transform domain nodes into Flobex compatible node objects
    */
   readonly flowNodes = computed(() => {
    const domainNodes = this.mapData.nodesFull();
    return domainNodes.map(node => ({
      id: node.publicId,
      position : { x: node.positionX, y: node.positionY},
      data: node
    }));
   });

   /**
    * Transform domain edges into Floblex compatible edge objects
    */
   readonly flowEdges = computed(() => {
    const domainEdges = this.mapData.edges();
    return domainEdges.map(edge => ({
      id: edge.publicId,
      source: edge.fromNodeId,
      target: edge.toNodeId,
      data: edge
    }));
   });

  /**
   * Node(s) movement, Foblex emits fMoveNodes when the drag is finished.
   * Updates the postions of the node(s)
   * @param event 
   */
   handlesNodesMovement(event: FMoveNodesEvent){
      const currentSchemaId = this.mapData.selectedSchemaId();
      if (!currentSchemaId) return;

      const updates = event.nodes.map(fNode => {
      const domainNode = this.mapData.nodes().find(n => n.publicId === fNode.id);
        
        if(!domainNode) return null;
        return {
          ...domainNode,
          positionX: fNode.position.x,
          positionY: fNode.position.y
        };
      }).filter((n): n is NodeResponse => n !== null);

      //Update the postions of the node(s)
      if (updates.length > 0){
        this.mapData.upsertNodes(updates);

        //bulk updates for nodes
        this.nodeService.bulkUpdateNodes(currentSchemaId, updates).subscribe({
          error: (err) => {
            this.loggerService.error('Failed to sync node positions', err);
          }
        });
      }
   }

   /**
    * Sync UI selection state of nodes and edges
    * @param event 
    */
   handleSelectionChange(event: FSelectionChangeEvent){
      this.selectedNodeIds.set(event.nodeIds);
      this.selectedEdgeIds.set(event.connectionIds);
   }

   /**
    * Creates a edge/connection between two nodes when user completes the action
    * @param event 
    */
   handlesCreateConnection(event: FCreateConnectionEvent, edgeRequest: EdgeRequest){
    const time = Date.now();
    const tempId = `temp-edge-${time}`;

    const currentSchemaId = this.mapData.selectedSchemaId();
    if (!currentSchemaId) return;

    //Placeholder/Temp edge for UI 
    const placeHolderEdge: EdgeResponse = {
      publicId: tempId,
      nodeWebId: currentSchemaId,
      fromNodeId: event.sourceId,
      toNodeId: event.targetId!
    }

    const request: EdgeRequest = {
      ...edgeRequest,
      nodewebId: currentSchemaId,
      fromNodeId: event.sourceId,
      toNodeId: event.targetId!,
    }

    //Optimistically update UI
    this.mapData.upsertEdges([placeHolderEdge]);

    //Call API to create the edge if successful
    this.edgeService.createEdge(request).subscribe({
      next: (realEdge) => {
        //Success: remove the temp and add the real edge
        this.mapData.deleteEdge(tempId);
        this.mapData.upsertEdges([realEdge]);
      },
      error: (err) => {
        this.loggerService.error('Failed to create edge:', err);
        this.mapData.deleteEdge(tempId);
      } 
    });
   }

   /**
    * Creates a new Node
    */
   handlesNodeCreation(nodeRequest: NodeRequest){
      const time = Date.now();
      const tempId = `temp-node-${time}`;

      const currenSchemaId = this.mapData.selectedSchemaId();
      if (!currenSchemaId) return;

      //placeholder node for UI
      const placeholderNode: NodeResponse = {
        ...nodeRequest,
        publicId: tempId,
        createdAt: new Date().toISOString(),
        nodeWebId: currenSchemaId
      }

      const request: NodeRequest = {
        ...nodeRequest,
        nodewebId: currenSchemaId
      }

      //Optimistically update the UI
      this.mapData.upsertNodes([placeholderNode]);

      //Call API to create node if successful
      this.nodeService.createNode(request).subscribe({
        next: (realNode) => {
          //Success: remove the temp and add the real node
          this.mapData.deleteNode(tempId);
          this.mapData.upsertNodes([realNode]);
        },
        error: (err) => {
          this.loggerService.error('Failed to create node:', err);
          this.mapData.deleteNode(tempId);
        }
      });
   }

   /**
    * Creates node asset
    * @param assetRequest 
    * @returns 
    */
   handlesAssetCreation(assetRequest: NodeAssetRequest){
      const time = Date.now();
      const tempId = `temp-asset-${time}`;

      //placeholder node asset for UI
      const placeholderAsset: NodeAssetResponse = {
        ...assetRequest,
        publicId: tempId
      }

      //Optimistically update the UI
      this.mapData.upsertAssets([placeholderAsset]);

      //Call API to create node asset if successful
      this.nodeAssetService.createNodeAsset(assetRequest).subscribe({
        next: (realAsset) => {
          //Success: remove the temp and add the real node asset
          this.mapData.deleteAsset(tempId);
          this.mapData.upsertAssets([realAsset]);
        },
        error: (err) => {
          this.loggerService.error('Failed to create node asset:', err);
          this.mapData.deleteAsset(tempId);
        }
      });
   }

  /**
   * Deletes the node
   * @param id the node id
   * @returns 
   */
   handlesNodeDeletion(id: string){
    if (id == null) return;
    const currentSchemaId = this.mapData.selectedSchemaId();
    if (!currentSchemaId) return;
    
    //Optimistically remove from UI
    this.mapData.deleteNode(id);

    this.nodeService.deleteNode(id).subscribe({
      error: (err) => {
        this.loggerService.error('Failed to delete node:', err);
      }
    });
   }

  /**
   * Deletes the node asset
   * @param id the node asset id
   * @returns 
   */
   handlesAssetDeletion(id: string){
    if (id == null) return;
    const currentSchemaId = this.mapData.selectedSchemaId();
    if (!currentSchemaId) return;

    //Optimistically remove from UI
    this.mapData.deleteAsset(id);

    this.nodeAssetService.deleteNodeAsset(id).subscribe({
      error: (err) => {
        this.loggerService.error('Failed to delete node asset:', err);
      }
    });
   }

   /**
   * Deletes the edge 
   * @param id the edge id
   * @returns 
   */
   handlesEdgeDeletion(id: string){
    if (id == null) return;
    const currentSchemaId = this.mapData.selectedSchemaId();
    if (!currentSchemaId) return;
    
    //Optimistically remove from UI
    this.mapData.deleteEdge(id);

    this.edgeService.deleteEdge(id).subscribe({
      error: (err) => {
        this.loggerService.error('Failed to delete edge:', err);
      }
    });
   }   

    /**
    * Updates the node
    * @param update data to update the node
    * @returns 
    */
   handlesNodeUpdate(update: UpdateNode){
    if (!update || !update.publicId) return;

    //Get the existing node to update
    const existingNode = this.mapData.nodes().find(n => n.publicId === update.publicId);
    if (!existingNode) return;

    //Merges the updated data into the existing node
    const updatedNode: NodeResponse = {
      ...existingNode,
      ...update,
      publicId: existingNode.publicId //ensure public id is not overwrite for data layer
    }

    //Optimistically update the UI
    this.mapData.upsertNodes([updatedNode]);

    this.nodeService.patchNode(update).subscribe({
      error: (err) => {
        this.loggerService.error('Failed to update node:', err);
      }
    });
   }

   /**
    * Updates the node asset
    * @param update data to update the node asset
    * @returns 
    */
   handlesAssetUpdate(update: UpdateNodeAsset){
    if (!update || !update.publicId) return;

    //Get the existing node to update
    const existingAsset = this.mapData.assets().find(a => a.publicId === update.publicId);
    if (!existingAsset) return;

    //Merges the updated data into the existing node asset
    const updatedAsset: NodeAssetResponse = {
      ...existingAsset,
      ...update,
      publicId: existingAsset.publicId //ensure public id is not overwrite for data layer
    }

    //Optimistically update the UI
    this.mapData.upsertAssets([updatedAsset]);

    this.nodeAssetService.patchNodeAsset(update).subscribe({
      error: (err) => {
        this.loggerService.error('Failed to update node asset:', err);
      }
    });
   }

   /**
    * Updates the edge
    * @param update data to update the edge
    * @returns 
    */
   handlesEdgeUpdate(update: UpdateEdge){
    if (!update || !update.publicId) return;

    //Get the existing edge to update
    const existingEdge = this.mapData.edges().find(e => e.publicId === update.publicId);
    if (!existingEdge) return;

    //Merges the updated data into the existing edge
    const updatedEdge: EdgeResponse = {
      ...existingEdge,
      ...update,
      publicId: existingEdge.publicId //ensure public id is not overwrite for data layer
    }

    //Optimistically update the UI
    this.mapData.upsertEdges([updatedEdge]);

    this.edgeService.patchEdge(update).subscribe({
      error: (err) => {
        this.loggerService.error('Failed to update edge:', err);
      }
    });
   }   
   
   /**
    * Deletes all currently selected nodes and edges in bulk
    */
   handlesBulkDeletion(){
    const currentSchemaId = this.mapData.selectedSchemaId();
    const nodeIds = this.selectedNodeIds();
    const edgeIds = this.selectedEdgeIds();

    if (!currentSchemaId || (nodeIds.length === 0 && edgeIds.length === 0)) return;

    //Optimistically update the UI state
    nodeIds.forEach(id => this.mapData.deleteNode(id)); //handles cascade deletion of edges
    edgeIds.forEach(id => this.mapData.deleteEdge(id)); //delete oprhan edges just in case

    this.clearSelection();
    
    //Bulk delete edges  
    if (edgeIds.length > 0) {
      this.edgeService.bulkDeleteEdges(currentSchemaId, edgeIds).subscribe({
        error: (err) => this.loggerService.error('Bulk edge deletion failed: ', err)
      });
    }

    //Bulk delete nodes; also cascade deletes depending edges
    if (nodeIds.length > 0) {
      this.nodeService.bulkDeleteNodes(currentSchemaId, nodeIds).subscribe({
        error: (err) => this.loggerService.error('Bulk node deletion failed: ', err)
      });
    }
   }


   /**
    * Clear the selected node and edge ids
    */
   clearSelection(){
    this.selectedNodeIds.set([]);
    this.selectedEdgeIds.set([]);
   }

}
