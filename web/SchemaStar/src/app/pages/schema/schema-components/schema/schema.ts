import { Component, inject, signal } from '@angular/core';
import {FCreateConnectionEvent, FFlowModule, FZoomDirective} from '@foblex/flow';
import { FlowStateService } from '../../../../services/flow-state-service';
import { EdgeRequest, EdgeType } from '../../../../services/schema/edge-service';
import { MapDataService } from '../../../../services/map-data-service';
import { LoggerService } from '../../../../services/logger-service';

@Component({
  selector: 'app-schema',
  imports: [FFlowModule],
  templateUrl: './schema.html',
  styleUrl: './schema.css',
})
export class Schema {

  /**
   * Zoom level of the schema
   */
  zoomLevel = signal<number>(1);

  public flowStateService = inject(FlowStateService);
  private mapDataService = inject(MapDataService);
  private loggerService = inject(LoggerService);

  /**
   * updates the zoom level to the given newScale value
   * @param newScale 
   */
  public updateZoom(newScale: number){
    this.zoomLevel.set(newScale);
  }  
  
  /**
   * increases the zoom level by 10% to maximum of 200%
   */
  public zoomIn(){
    this.zoomLevel.update(z => Math.min(z + 0.1, 2));
  }

  /**
   * decreases the zoom level by 10% to minimum of 10%
   */
  public zoomOut(){
    this.zoomLevel.update(z => Math.max(z - 0.1, 0.1));
  }

  /**
   * Updates the zoom level to the newScale value passing this event to the fZoomChange event
   * @param newScale 
   */
  onZoomChange(newScale: number | any): void {
    if (typeof newScale === 'number'){
      this.zoomLevel.set(newScale);
      // this.loggerService.log('Zoom Level:', this.zoomLevel());
    }
  }

  /**
   * Function to handle passing Edge Request data to handleConnectionCreation method in flowstate service
   * @param event 
   */
  public onCreateConnection(event: FCreateConnectionEvent): void {
    const defaultRequest: Partial<EdgeRequest> = {
      edgeType: EdgeType.Undirected,
      uiMetadata: JSON.stringify({color: '#FFFFFF'})
    };
    //Call handlesCreateConnection with default values
    this.flowStateService.handlesCreateConnection(event, defaultRequest as EdgeRequest);
  }

  /**
   * Load the schema and resources for the user 
   */
  ngOnInit(){
    this.mapDataService.loadUserWorkspace();
  }
}
