import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SecretData } from '../../../../environment';
import { Observable } from 'rxjs';
import { NodeAssetResponse } from './node-asset-service';

/**
 * State of the node
 */
export enum NodeState {
  Locked,
  Unlocked,
  Pinned
}

export interface NodeRequest{
  nodeName: string,
  nodeDescription?: string,
  positionX: number,
  positionY: number,
  width: number,
  height: number,
  state: NodeState,
  nodewebId: string,
}

export interface NodeResponse{
  publicId: string,
  nodeName: string,
  nodeDescription?: string,
  positionX: number,
  positionY: number,
  width: number,
  height: number,
  state: NodeState,
  createdAt: string,
  updatedAt?: string,
}

export interface NodeResponseFull{
  publicId: string,
  nodeName: string,
  nodeDescription?: string,
  positionX: number,
  positionY: number,
  width: number,
  height: number,
  state: NodeState,
  createdAt: string,
  updatedAt?: string,
  NodeAssets: NodeAssetResponse[]
}

export interface UpdateNode {
  publicId: string, 
  nodeName?: string,
  nodeDescription?: string,
  positionX?: number,
  positionY?: number,
  width?: number,
  height?: number,
  state?: NodeState
}

@Injectable({
  providedIn: 'root',
})
export class NodeService {

  private http = inject(HttpClient);
  private readonly nodeUrl = `${SecretData.baseuUrl}/api/nodes`;

  /**
   * 
   * @param id 
   * @returns the node url with the node id for HTTP methods
   */
  private getUrl (id: string): string{
    return `${this.nodeUrl}/${id}`;
  }

   /**
   * 
   * @param id 
   * @returns the node url with the nodeweb id for HTTP GET method
   */
  private getNodesUrl (id: string): string{
    return `${SecretData.baseuUrl}/api/nodewebs/${id}/nodes`;
  }

  /**
   * @param nodewebId
   * @returns all the Nodes for the nodeweb
   */
  getNodes(nodewebId:string):Observable<NodeResponse[]>{
    const url = this.getNodesUrl(nodewebId);
    return this.http.get<NodeResponse[]>(url);
  }

  /**
   * 
   * @param id 
   * @returns the NodeResponse for the specified id
   */
  getNode(id: string):Observable<NodeResponse>{
    const url = this.getUrl(id);
    return this.http.get<NodeResponse>(url);
  }

  /**
   * 
   * @param node 
   * @returns the NodeResponse for the node that was updated
   */
  patchNode(node: UpdateNode):Observable<NodeResponse>{
    //separate id to url and name to body
    const url = this.getUrl(node.publicId);
    const {publicId, ...updates} = node; //destructuring the UpdateNode object and using the spread operator to assign the remaing values
    return this.http.patch<NodeResponse>(url, updates);
  }

  /**
   * 
   * @param node 
   * @returns the NodeResponse for the created node
   */
  createNode(node: NodeRequest):Observable<NodeResponse>{
    return this.http.post<NodeResponse>(this.nodeUrl, node);
  }

  /**
   * Deletes an existing Node with the corresponding id
   * @param id 
   * @returns void
   */
  deleteNode(id: string): Observable<void>{
    const url = this.getUrl(id);
    return this.http.delete<void>(url);
  }

    /**
   * 
   * @param id 
   * @returns the NodeResponseFull for the specified id
   */
  getNodeFull(id: string):Observable<NodeResponseFull>{
    const baseUrl = this.getUrl(id);
    const url = `${baseUrl}/full`;
    return this.http.get<NodeResponseFull>(url);
  }

}
