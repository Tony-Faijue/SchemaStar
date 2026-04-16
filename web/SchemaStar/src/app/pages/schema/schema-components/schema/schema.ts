import { Component, inject } from '@angular/core';
import {FCreateConnectionEvent, FFlowModule} from '@foblex/flow';
import { FlowStateService } from '../../../../services/flow-state-service';
import { EdgeRequest, EdgeType } from '../../../../services/schema/edge-service';
import { MapDataService } from '../../../../services/map-data-service';

@Component({
  selector: 'app-schema',
  imports: [FFlowModule],
  templateUrl: './schema.html',
  styleUrl: './schema.css',
})
export class Schema {

  public flowStateService = inject(FlowStateService);
  private mapDataService = inject(MapDataService);

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
