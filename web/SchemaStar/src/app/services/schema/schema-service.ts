import { inject, Injectable, signal } from '@angular/core';
import { SecretData } from '../../../../environment';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';


/**
 * Data to create new Schema
 */
export interface RegisterSchema {
  nodeWebName: string
}

/**
 * Data to update an exisiting Schema
 */
export interface UpdateSchema{
  nodeWebName: string
  publicId: string
}

/**
 * Data response for a Schema
 */
export interface SchemaResponse{
  publicId: string,
  nodeWebName: string,
  lastLayoutAt: string,
  createdAt: string,
  updatedAt: string
}

@Injectable({
  providedIn: 'root',
})
export class SchemaService {
 
  public currentSchema = signal<SchemaResponse | null>(null);

  private http = inject(HttpClient);
  private readonly schemaUrl = `${SecretData.baseuUrl}/api/Nodewebs`;

  /**
   * 
   * @param id 
   * @returns the schema url with the schema id for HTTP methods
   */
  private getUrl (id: string): string{
    return `${this.schemaUrl}/${id}`;
  }

  /**
   * 
   * @returns all the Schemas/NodeWebs
   */
  getSchemas():Observable<SchemaResponse[]>{
    return this.http.get<SchemaResponse[]>(this.schemaUrl);
  }

  /**
   * 
   * @param id 
   * @returns the SchemaRespone for the specified id
   */
  getSchema(id: string):Observable<SchemaResponse>{
    const url = this.getUrl(id);
    return this.http.get<SchemaResponse>(url).pipe(
      tap((schema : SchemaResponse) => 
        this.currentSchema.set(schema)
      )
    );
  }

  /**
   * 
   * @param schema 
   * @returns the SchemaResponse for the Schema that was updated
   */
  patchSchema(schema: UpdateSchema):Observable<SchemaResponse>{
    //separate id to url and name to body
    const url = this.getUrl(schema.publicId);
    const body = {nodeWebName: schema.nodeWebName};
    return this.http.patch<SchemaResponse>(url, body).pipe(
      tap((updatedSchema: SchemaResponse) => 
        this.currentSchema.set(updatedSchema)
      )
    );
  }

  /**
   * 
   * @param schema 
   * @returns the SchemaResponse for the created Schema
   */
  createSchema(schema: RegisterSchema):Observable<SchemaResponse>{
    return this.http.post<SchemaResponse>(this.schemaUrl, schema);
  }

  /**
   * Deletes an existing Schema with the corresponding id
   * @param id 
   * @returns void
   */
  deleteSchema(id: string): Observable<void>{
    const url = this.getUrl(id);
    return this.http.delete<void>(url);
  }

  /**
   * Sets the current Schema to null
   */
  clearCurrentSchema(): void{
    this.currentSchema.set(null);
  }
}
