import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SecretData } from '../../../../environment';
import { Observable } from 'rxjs';


/**
 * Edge Types
 */
export enum EdgeType {
  Directed,
  Undirected
}

export interface EdgeRequest{
  edgeType: EdgeType,
  uiMetadata: string,
  fromNodeId: string,
  toNodeId: string,
  nodewebId: string
}

export interface EdgeResponse{
  publicId: string,
  edgeType?: EdgeType,
  uiMetadata?: string,
  fromNodeId: string,
  toNodeId: string,
  nodeWebId: string
}

export interface UpdateEdge{
  publicId: string,
  edgeType?: EdgeType,
  uiMetadata?: string,
  fromNodeId?: string,
  toNodeId?: string
}

@Injectable({
  providedIn: 'root',
})
export class EdgeService {
  
  private http = inject(HttpClient);
  private readonly edgeUrl = `${SecretData.baseuUrl}/api/edges`;

  /**
   * 
   * @param id 
   * @returns the edge url with the edge id for HTTP methods
   */
  private getUrl (id: string): string{
    return `${this.edgeUrl}/${id}`;
  }

   /**
   * 
   * @param id 
   * @returns the edge url with the nodeweb id for HTTP GET method
   */
  private getEdgesUrl (id: string): string{
    return `${SecretData.baseuUrl}/api/nodewebs/${id}/edges`;
  }

  /**
   * @param nodewebId
   * @returns all the Edges for the given nodewebId for GET Method
   */
  getEdges(nodewebId: string):Observable<EdgeResponse[]>{
    const url = this.getEdgesUrl(nodewebId);
    return this.http.get<EdgeResponse[]>(url);
  }

  /**
   * 
   * @param id 
   * @returns the EdgeResponse for the specified id
   */
  getEdge(id: string):Observable<EdgeResponse>{
    const url = this.getUrl(id);
    return this.http.get<EdgeResponse>(url);
  }

  /**
   * 
   * @param edge 
   * @returns the EdgeResponse for the edge that was updated
   */
  patchEdge(edge: UpdateEdge):Observable<EdgeResponse>{
    //separate id to url and name to body
    const url = this.getUrl(edge.publicId);
    const {publicId, ...updates} = edge; //destructuring the UpdateEdge object and using the spread operator to assign the remaing values
    return this.http.patch<EdgeResponse>(url, updates);
  }

  /**
   * 
   * @param edge 
   * @returns the EdgeResponse for the created edge
   */
  createEdge(edge: EdgeRequest):Observable<EdgeResponse>{
    return this.http.post<EdgeResponse>(this.edgeUrl, edge);
  }

  /**
   * Deletes an existing Edge with the corresponding id
   * @param id 
   * @returns void
   */
  deleteEdge(id: string): Observable<void>{
    const url = this.getUrl(id);
    return this.http.delete<void>(url);
  }

  /**
   * Bulk deletes edges
   * @param id nodeweb id
   * @param edgeIds string of edge ids
   * @returns 
   */
  bulkDeleteEdges(id: string, edgeIds: string[]):Observable<void>{
    const baseUrl = this.getEdgesUrl(id);
    const url = `${baseUrl}/bulk`;
    return this.http.delete<void>(url, {body: edgeIds});
  }
}
