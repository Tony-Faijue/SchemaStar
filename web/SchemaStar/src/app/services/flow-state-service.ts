import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FlowStateService {
  /**
   * ---Translation---
   * Foblex-Flow has its own requirement for nodes and edges
   * Q1: Should FlowState Service store its own visual-only version of the nodes,
   * or should it translate the MapData Service signals into foblex objects? Why?
   * 
   */

  /**
  * I think it makes sense to [translate the MapData service signals into foblex objects]
  * because it will make it easier to directly interact with those objects in the ui rather
  * trying to sync with a snapshot that will constantly be changing.
  * --Adapter Pattern--
  */

  /**
   * --State vs Persisted Data--
   * Q2: Should Map Data Service update every single time a pixel changes (Heavy Work)
   * or should FlowState Service hold temporary movement until the user lets go?
   * How should the "Live Dragging" state be managed?
   */

  /**
   * I think the FlowState service should hold temporary movement until the user lets go
   * since it would slow down performance trying to update every single pixel movement.
   * The pro of having it update for every pixel movement is that the data would be live one to one
   * but the con would be the performance.
   * The pro of having the data be sent after the drag ends it that it reduce the performance load.
   * The con would be that there could exist an inconsitency between the UI and the database but
   * this could be offset with holding a [temporary/latest state] that would be sent to the database.
   * I think overall waiting until the drag ends will be more effective performance wise and data persistence wise.
   * 
   */

  /**
   * --User Interactions (Selection)--
   * Ex: selecting multiple nodes, zooming the canvas
   * Q3:Does the backend/database care which node is currently highlighted?
   * If not, where should "Selection List" live?
   * How does FlowState Service handle a "Select All" command?
   */

  /**
   * I think the backend should not care which nodes are selected or the stat of the nodes related to the UI.
   * Because of that the flowstate service should care about the currently selected nodes.
   * The list should live inside the flowstate service that way actions can be directly to related nodes.
   */

  /**
   * --Library events (Event Flow Updates)--
   * Q4: What is the step by step hanshake between the library, the FlowState service and the MapData service
   * to ensure that new line on the screen actually results in an EdgeResponse being saved in the database?
   */

  /**
   * Optimistic UI vs Pessimistic UI
   * Issue with optimisitc is that we do work and have to undo the work when the api fails
   * Issue with pessimistic is that we have to wait for the API response to succeed to complete the work
   * I think it depends whether or not if speed is imporatant compared to data integrity.
   * I think it is better to have the UI display the new line/edge so that the applcation is reactive for UX.
   * So the new line is created and then API request is sent. If it fails then line should be removed because it will
   * reflect the actual database.
   */

  /**
   * --Component Communication--
   * Q5: Should components talk directly to the MapDataService or should they talk to FlowStateService?
   * How to keep components in sync without them knowing about each other?
   * Ex: Properties panel component need to show the node's name and assets for a clicked node in the Canvas
   */

  /**
   * I think the component should talk with the FlowStateService since it would reduce complexity trying to access the data directly from the
   * MapDataService. All the component would need is the id and from there it can access the selected node in the flowstate service
   * which the flowstate service will do the work with the given id to return info so that way data access is consistent.
   * --Shared State Service Pattern--
   */

}
