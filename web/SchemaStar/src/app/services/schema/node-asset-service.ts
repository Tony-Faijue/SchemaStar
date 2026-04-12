import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SecretData } from '../../../../environment';
import { Observable } from 'rxjs';

export enum NodeAssetType {
  Image,
  Audio,
  Video,
  Link
}

export enum NodeAssetSource {
  Upload,
  External
}

export interface NodeAssetRequest {
  nodeAssetName?: string,
  nodeAssetType: NodeAssetType,
  nodeAssetSource: NodeAssetSource,
  url?: string,
  mimeType?: string,
  fileSize?: number,
  blobPath?: string,
  nodeId: string
}

export interface NodeAssetResponse{
  publicId: string,
  nodeAssetName?: string,
  nodeAssetType: NodeAssetType,
  nodeAssetSource: NodeAssetSource,
  url?: string,
  mimeType?: string,
  fileSize?: number,
  blobPath?: string,
}

export interface UpdateNodeAsset{
  publicId: string, 
  nodeAssetName?: string,
  assetType?: NodeAssetType,
  assetSource?: NodeAssetSource,
  url?: string,
  mimeType?: string,
  fileSize?: number,
  blobPath?: string
}

@Injectable({
  providedIn: 'root',
})
export class NodeAssetService {
  
  private http = inject(HttpClient);
  private readonly nodeAssetUrl = `${SecretData.baseuUrl}/api/NodeAssets`;

  /**
   * 
   * @param id 
   * @returns the node asset url with the node asset id for HTTP methods
   */
  private getUrl (id: string): string{
    return `${this.nodeAssetUrl}/${id}`;
  }

   /**
   * 
   * @param id 
   * @returns the node asset url with the node id for HTTP GET method
   */
  private getNodeAssetsUrl (id: string): string{
    return `${SecretData.baseuUrl}/api/Nodes/${id}/NodeAssets`;
  }

  /**
   * @param nodeId
   * @returns all the Node Assets for the node id
   */
  getNodeAssets(nodeId: string):Observable<NodeAssetResponse[]>{
    const url = this.getNodeAssetsUrl(nodeId);
    return this.http.get<NodeAssetResponse[]>(url);
  }

  /**
   * 
   * @param id 
   * @returns the NodeResponse for the specified id
   */
  getNodeAsset(id: string):Observable<NodeAssetResponse>{
    const url = this.getUrl(id);
    return this.http.get<NodeAssetResponse>(url);
  }

  /**
   * 
   * @param nodeAsset 
   * @returns the NodeAssetResponse for the node asset that was updated
   */
  patchNodeAsset(nodeAsset: UpdateNodeAsset):Observable<NodeAssetResponse>{
    //separate id to url and name to body
    const url = this.getUrl(nodeAsset.publicId);
    const {publicId, ...updates} = nodeAsset; //destructuring the UpdateNodeAsset object and using the spread operator to assign the remaing values
    return this.http.patch<NodeAssetResponse>(url, updates);
  }

  /**
   * 
   * @param nodeAsset 
   * @returns the NodeAssetResponse for the created node asset
   */
  createNodeAsset(nodeAsset: NodeAssetRequest):Observable<NodeAssetResponse>{
    return this.http.post<NodeAssetResponse>(this.nodeAssetUrl, nodeAsset);
  }

  /**
   * Deletes an existing Node Asset with the corresponding id
   * @param id 
   * @returns void
   */
  deleteNodeAsset(id: string): Observable<void>{
    const url = this.getUrl(id);
    return this.http.delete<void>(url);
  }
}
