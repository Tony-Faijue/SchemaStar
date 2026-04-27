import { computed, inject, Injectable, signal } from '@angular/core';
import { NodeResponse, NodeResponseFull, NodeService } from './schema/node-service';
import { EdgeResponse, EdgeService } from './schema/edge-service';
import { SchemaResponse, SchemaResponseFull, SchemaService } from './schema/schema-service';
import { NodeAssetResponse, NodeAssetService } from './schema/node-asset-service';
import { forkJoin, take } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class MapDataService {
  //Data Layer
  private schemaService = inject(SchemaService);
  private nodeService = inject(NodeService);
  private nodeAssetService = inject(NodeAssetService);
  private edgeService = inject(EdgeService);

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
   *checks if the current active schema is loaded and ready 
   */
  readonly isSchemaLoaded = computed(() => {
    return !this.isLoading() && this.currentSchema() !== null;
  });

  /**
   * check if the there is no schemas and sub resources for the data layer
   */
  readonly isEmpty = computed (() => this.schemas().length === 0 && !this.isLoading());

  /**
  * Computed Full Nodes (Nodes + Assets)
  */
  readonly nodesFull = computed<NodeResponseFull[]>(() => {
    const allNodes = this.nodes();
    const allAssets = this.assets();

    //Create buckets for each nodeId that contains the assets for it
    const assetsByNodes = Map.groupBy(allAssets, a => a.nodeId);

    return allNodes.map(node => ({
      ...node, //spread operator to copy node response properties
      NodeAssets: assetsByNodes.get(node.publicId) ?? []
    }))
    //Sort the nodes in alphabetical order
    .sort((a, b) => a.nodeName.localeCompare(b.nodeName));
  });

  /**
  * Computed Full Schemas (Schemas + Nodes + Edges)
  */
  readonly schemasFull = computed<SchemaResponseFull[]>(() => {
    const allSchemas = this.schemas();
    const allNodesFull = this.nodesFull();
    const allEdges = this.edges();
    
    //Map groupBy function to create an array/bucket for the value with the nodewebId
    //{nodewebId, [node, node, node, ...]}
    //Essentially creates a bucket with related object matching the nodewebId
    const nodesBySchema = Map.groupBy(allNodesFull, n => n.nodeWebId);
    const edgesBySchema = Map.groupBy(allEdges, e => e.nodeWebId);

    return allSchemas.map(schema => {
      //Sort the bucket
      const sortedNodes = (nodesBySchema.get(schema.publicId) ?? [])
        .sort((a , b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
      
        return{
          ...schema,
        //Get the bucket of items with the matching nodewebId/schemaPublicId and attach them
        nodes: sortedNodes,
        edges: edgesBySchema.get(schema.publicId) ?? []
      };
    });
  });
  
  /**
   * The current schema the user interacts with
   */
  readonly currentSchema = computed(() => {
    const id = this.selectedSchemaId();
    return this.schemasFull().find(s => s.publicId === id) ?? null;
  });


  //-----------Functions------

  /**
   * Upsert function to add/update schemas
   */
  upsertSchemas(newSchemas: SchemaResponse[]){
    //update the schemas signal
    this.schemas.update((current: SchemaResponse[]) => {
      //create schema map with key(schemaresponse publicId), value (schemaresponse)
      const schemaMap = new Map<string, SchemaResponse>(current.filter(s => !!s).map(s =>[s.publicId, s])); //filter before mapping for null values
      //Add a schema or update and exisiting schema
      newSchemas.filter(n => !!n).forEach(n => schemaMap.set(n.publicId, n));
      //Converts the Map iterator type to an Array
      return Array.from(schemaMap.values());
    });
  }
  
  /***
   * Delete function remove an exisiting schema
   */
  deleteSchema(publicId: string){
    //Collect all node ids that belong to the given schema to be removed
    const nodeIdsToRemove = this.nodes()
      .filter(n => n.nodeWebId === publicId)
      .map(n => n.publicId);
    //Set for Snapshot of nodes, O(n) look up, prevent weak cascade delete
    const nodeIdsSet = new Set(nodeIdsToRemove);

    //update the schemas signal to not include schema that includes the publicId
    this.schemas.update(current => current.filter(s => s.publicId !== publicId));
    
    //Remove all nodes belonging to this schema
    this.nodes.update(current => current.filter(n =>  n.nodeWebId !== publicId));

    //Remove all edges belonging to this schema
    this.edges.update(current => current.filter(e => e.nodeWebId !== publicId));

    //Remove assets belonging to nodes of this schema
    this.assets.update(current => current.filter(a => !nodeIdsSet.has(a.nodeId))); //if the set does not contain the id keep it 

    //If the deleted schema is current schema set to null
    if (this.selectedSchemaId() === publicId){
      this.selectedSchemaId.set(null);
    }
  }

  /**
   * Upsert function to add/update nodes
   */
  upsertNodes(newNodes: NodeResponse[]){
    //update the nodes signal
    this.nodes.update((current: NodeResponse[]) => {
      //create node map with key(noderesponse publicId), value (noderesponse)
      const nodeMap = new Map<string, NodeResponse>(current.filter(n => !!n).map(n => [n.publicId, n])); //filter before mapping for null values
      //Add a new node or update and exisiting node
      newNodes.filter(n => !!n).forEach(n => nodeMap.set(n.publicId, n));
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
    
    //Remove assets connected to the node
    this.assets.update(current => current.filter(a => a.nodeId !== publicId));

    //Remove any edges connected to the node
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
      const assetMap = new Map<string, NodeAssetResponse>(current.filter(a => !!a).map(a =>[a.publicId, a])); //filter current assets before mapping for null
      //Add a new asset or update and exisiting asset
      newAssets.filter(a => !!a).forEach(a => assetMap.set(a.publicId, a)); //filter before mapping
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
      const edgeMap = new Map<string, EdgeResponse>(current.filter(e => !!e).map(e => [e.publicId, e])); //filter before mapping for null values
      //Add a new edge or update and exisiting edge
      newEdges.filter(e => !!e).forEach(e => edgeMap.set(e.publicId, e));
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

  /**
   * Loads the schemas for the logged-in user
   */
  loadUserWorkspace(){
    this.isLoading.set(true);
    this.error.set(null);

    //Get all schemas and update local data here
    this.schemaService.getSchemas().pipe(take(1)).subscribe({ //right now gets the first schema; modify for selection
      next: (schemas) => {
        //Update fresh schemas
        this.upsertSchemas(schemas);

        if(schemas.length > 0){
          const firstSchema = schemas[0];
          this.loadSchemaResources(firstSchema.publicId);
        } else {
          //no schemas found
          this.isLoading.set(false);
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.error.set("Failed to load user workspace");
      }
    });
  }

  /**
   * Loads the specific nodes, edges and assets for a selected schema
   */
  loadSchemaResources(schemaId: string){
    this.selectedSchemaId.set(schemaId);
    this.isLoading.set(true);

    //Use forkjoin to fetch all sub-resources in parallel for better performance
    
    //Get Basic node info and edges
    forkJoin({
      nodes: this.nodeService.getNodes(schemaId),
      edges: this.edgeService.getEdges(schemaId)
    }).subscribe({
      next: (data) => {
        this.upsertEdges(data.edges);

        if (data.nodes.length === 0){
          this.isLoading.set(false);
          return;
        }

        const fullNodeRequests = data.nodes.map((node) =>
          this.nodeService.getNodeFull(node.publicId)
        );

        //Update nodes with full nodes and get all assets
        forkJoin(fullNodeRequests).subscribe({
          next: (fullNodes: NodeResponseFull[]) => {
            this.upsertNodes(fullNodes);
            const allAssets = fullNodes.flatMap(n => n.NodeAssets);
            this.upsertAssets(allAssets);
            this.isLoading.set(false);
          },
          error: () => {
            this.isLoading.set(false);
            this.error.set("Failed to load node details");
          }
        });
      }, 
      error: () =>{ 
        this.isLoading.set(false);
        this.error.set("Failed loading schema resources");
      }
    });
  }

}
