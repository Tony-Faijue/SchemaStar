import { computed, inject, Injectable, signal } from '@angular/core';
import { MapDataService } from './map-data-service';
import { FCreateConnectionEvent, FMoveNodesEvent, FSelectionChangeEvent } from '@foblex/flow';
import { NodeRequest, NodeResponse, NodeService } from './schema/node-service';
import { EdgeRequest, EdgeResponse, EdgeService } from './schema/edge-service';
import { LoggerService } from './logger-service';
import { NodeAssetRequest, NodeAssetResponse, NodeAssetService } from './schema/node-asset-service';

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
   * Node(s) movement, Foblex emits fMoveNodes when the drag is finished
   * @param event 
   */
   handlesNodesMovement(event: FMoveNodesEvent){
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

        //Should look into bulk update for nodes for the API backend to improve peformance
        //instead of individual calls
        updates.forEach(node => {
          this.nodeService.patchNode(node).subscribe();
        })
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
    * Clear the selected node and edge ids
    */
   clearSelection(){
    this.selectedNodeIds.set([]);
    this.selectedEdgeIds.set([]);
   }

}
