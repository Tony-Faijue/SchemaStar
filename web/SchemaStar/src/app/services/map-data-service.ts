import { computed, Injectable, signal } from '@angular/core';
import { NodeResponse, NodeResponseFull } from './schema/node-service';
import { EdgeResponse } from './schema/edge-service';
import { SchemaResponse, SchemaResponseFull } from './schema/schema-service';
import { NodeAssetResponse } from './schema/node-asset-service';


@Injectable({
  providedIn: 'root',
})
export class MapDataService {
  //Data Layer
  //All resource data for the user
  schemas = signal<SchemaResponse[]>([]);
  nodes = signal<NodeResponse[]>([]);
  edges = signal<EdgeResponse[]>([]);
  assets = signal<NodeAssetResponse[]>([]);

  //Current Schema Selected
  selectedSchemaId = signal<string | null>(null);

  //Data state and Error
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);


  //-----Computed Values-----

  /**
  * Computed Full Nodes (Nodes + Assets)
  */
  readonly nodesFull = computed<NodeResponseFull[]>(() => {
    const allNodes = this.nodes();
    const allAssets = this.assets();
    //Map the node assets for each node filtering with the matching nodeId to the node's publicId
    return allNodes.map(node => ({
      ...node, //spread operator to copy node response properties
      NodeAssets: allAssets.filter(asset => asset.nodeId == node.publicId)
    }));
  });

  /**
  * Computed Full Schemas (Schemas + Nodes + Edges)
  */
  readonly schemasFull = computed<SchemaResponseFull[]>(() => {
    const allSchemas = this.schemas();
    const allNodesFull = this.nodesFull();
    const allEdges = this.edges();
    //Map the nodes and edges for each schema filtering with matching schema.publicId
    return allSchemas.map(schema => ({
      ...schema,
      nodes: allNodesFull.filter(node => node.nodeWebId == schema.publicId),
      edges: allEdges.filter(edge => edge.nodeWebId == schema.publicId)
    }));
  });
  
  /**
   * The current schema the user interacts with
   */
  readonly currentSchema = computed(() => {
    const id = this.selectedSchemaId();
    return this.schemasFull().find(s => s.publicId == id) ?? null;
  });


  //-----------Functions------

  /**
   * Upsert function to add/update schemas
   */
  upsertSchemas(newSchemas: SchemaResponse[]){
    //update the schemas signal
    this.schemas.update((current: SchemaResponse[]) => {
      //create schema map with key(schemaresponse publicId), value (schemaresponse)
      const schemaMap = new Map<string, SchemaResponse>(current.map(s => [s.publicId, s]));
      //Add a schema or update and exisiting schema
      newSchemas.forEach(n => schemaMap.set(n.publicId, n));
      //Converts the Map iterator type to an Array
      return Array.from(schemaMap.values());
    });
  }
  
  /***
   * Delete function remove an exisiting schema
   */
  deleteSchema(publicId: string){
    //update the schemas signal to not include schema that includes the publicId
    this.schemas.update(current => current.filter(s => s.publicId !== publicId));
    
    //Remove all nodes belonging to this schema
    this.nodes.update(current => current.filter(n =>  n.nodeWebId !== publicId));

    //Remove all edges belonging to this schema
    this.edges.update(current => current.filter(e => e.nodeWebId !== publicId));

    //Remove assets belonging to nodes of this schema
    this.assets.update(current => { //Filter by matching the remaining nodes that do contain a node id
      const remainingNodeIds = new Set(this.nodes().map(n => n.publicId));
      return current.filter(a => remainingNodeIds.has(a.nodeId));
    });
  }

  /**
   * Upsert function to add/update nodes
   */
  upsertNodes(newNodes: NodeResponse[]){
    //update the nodes signal
    this.nodes.update((current: NodeResponse[]) => {
      //create node map with key(noderesponse publicId), value (noderesponse)
      const nodeMap = new Map<string, NodeResponse>(current.map(n => [n.publicId, n]));
      //Add a new node or update and exisiting node
      newNodes.forEach(n => nodeMap.set(n.publicId, n));
      //Converts the Map iterator type to an Array
      return Array.from(nodeMap.values());
    });
  }
  
  /***
   * Delete function remove an exisiting node
   */
  deleteNode(publicId: string){
    //update the nodes signal to not include node that include the publicId
    this.nodes.update(current => current.filter(n => n.publicId !== publicId));
    
    //Remove any edges connect to the node
    this.edges.update(current => current.filter(e => 
      e.fromNodeId !== publicId && e.toNodeId !== publicId
    ));
  }

  /**
   * Upsert function to add/update node asset
   */
  upsertAssets(newAssets: NodeAssetResponse[]){
    //update the assets signal
    this.assets.update((current: NodeAssetResponse[]) => {
      //create asset map with key(nodeassetresponse publicId), value (nodeassetresponse)
      const assetMap = new Map<string, NodeAssetResponse>(current.map(a => [a.publicId, a]));
      //Add a new asset or update and exisiting asset
      newAssets.forEach(a => assetMap.set(a.publicId, a));
      //Converts the Map iterator type to an Array
      return Array.from(assetMap.values());
    });
  }
  
  /***
   * Delete function remove an exisiting node asset
   */
  deleteAsset(publicId: string){
    //update the assets signal to not include asset that include the publicId
    this.assets.update(current => current.filter(a => a.publicId !== publicId));
  }

  
  /**
   * Upsert function to add/update edge
   */
  upsertEdges(newEdges: EdgeResponse[]){
    //update the edges signal
    this.edges.update((current: EdgeResponse[]) => {
      //create edge map with key(edgeresponse publicId), value (edgeresponse)
      const edgeMap = new Map<string, EdgeResponse>(current.map(e => [e.publicId, e]));
      //Add a new edge or update and exisiting edge
      newEdges.forEach(e => edgeMap.set(e.publicId, e));
      //Converts the Map iterator type to an Array
      return Array.from(edgeMap.values());
    });
  }
  
  /***
   * Delete function remove an exisiting edge
   */
  deleteEdge(publicId: string){
    //update the edges signal to not include edge that include the publicId
    this.edges.update(current => current.filter(e => e.publicId !== publicId));
  }

  /**
   * Function clear data 
   */
  clearData(){
    this.schemas.set([]);
    this.nodes.set([]);
    this.assets.set([]);
    this.edges.set([]);
    this.selectedSchemaId.set(null);
  }

}
