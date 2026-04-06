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
  edgeType: EdgeType
  fromNodeId: string,
  toNodeId: string
}

export interface EdgeResponse extends EdgeRequest{
  publicId: string
}

export interface UpdateEdge{
  publicId: string,
  edgeType?: EdgeType
  fromNodeId?: string,
  toNodeId?: string
}

@Injectable({
  providedIn: 'root',
})
export class EdgeService {
  
  private http = inject(HttpClient);
  private readonly edgeUrl = `${SecretData.baseuUrl}/api/Edges`;

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
   * @returns all the Edges
   */
  getEdges():Observable<EdgeResponse[]>{
    return this.http.get<EdgeResponse[]>(this.edgeUrl);
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
}
