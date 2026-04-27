import { computed, inject, Injectable, signal } from '@angular/core';
import { FCanvasComponent, FZoomDirective } from '@foblex/flow';
import { LoggerService } from './logger-service';

@Injectable({
  providedIn: 'root',
})
export class SchemaUiStateService {
  
  private loggerService = inject(LoggerService);
  //--------------UI state Properties------------
  /**
   * FCanvas reference for the schema component
   */
  private fCanvas?: FCanvasComponent;

  /**
   * Fzoom directive to interact directly with the zoom functionality
   */
  private fZoom?: FZoomDirective;

  /**
   * Zoom level of the schema
   */
  public zoomLevel = signal<number>(1);

  /***
   *Percentage of zoomLevel 
   */
  public zoomPercentage = computed(() => {
    return (this.zoomLevel() * 100).toFixed(0) + '%';
  });

  /**
   * Toggle state of SchemaToolBar
   */
  public isToolBarOpen = signal(false);

  /**
   * Toggle state of SchemaQuickToolBar
   */
  public isQuickToolBarOpen = signal(false);

  //--------------Functions------------

  /**
   * Set the canvas to reference the given FCanvasComponent and FZoomDirective
   * @param canvas 
   */
  public setCanvas(canvas: FCanvasComponent, zoom: FZoomDirective){
    this.fCanvas = canvas;
    this.fZoom = zoom;
    this.zoomLevel.set(this.fCanvas.getScale());
  }

  //--------------Zoom and View Functions---------------

  /**
   * increases the zoom level by 10% to maximum of 400%
   */
  public zoomIn(){
    if(this.fZoom){
      this.fZoom.zoomIn();

      setTimeout(() => {
        if(this.fCanvas){
          const currentScale = this.fCanvas.getScale(); //get the scale from the canvas directly
          this.onZoomChange(currentScale);
        }
      }, 10);
    }
  }

  /**
   * decreases the zoom level by 10% to minimum of 10%
   */
  public zoomOut(){
    if(this.fZoom){
      this.fZoom.zoomOut();
     
      setTimeout(() => {
        if(this.fCanvas){
          const currentScale = this.fCanvas.getScale(); //get scale from the canvas directly
          this.onZoomChange(currentScale);
        }
      }, 10);
    }    
  }

  /**
   * Updates the zoom level to the newScale value passing this event to the fZoomChange event
   * @param newScale 
   */
  public onZoomChange(newScale: number | any): void {
    if (typeof newScale === 'number'){
      this.loggerService.log(newScale);
      this.zoomLevel.set(newScale);
    }
  }
  /**
   * Resets the view of the canvas
   */
  public onResetView(){
    if(this.fCanvas){
      this.fCanvas.resetScaleAndCenter(false);
      this.zoomLevel.set(1);
    }
  }
  /**
   * Makes the content fit the screen
   */
  public onFitScreenView(){
    if(this.fCanvas){
      this.fCanvas.fitToScreen();
      this.zoomLevel.set(this.fCanvas.getScale()); //update value to the new scale
    }
  }

  /**
   * Triggers the native browswer full-screen mode
   * @param element the HTML Element that contains the f-flow root
   */
  public async toggleBrowserFullScreen(element: HTMLElement){
    if(!document.fullscreenElement){
      await element.requestFullscreen()
      .then(() => {
        this.onFitScreenView(); //Resize for elements to fit the screen
      })
      .catch((err) => {
        this.loggerService.error(`Error enabling full screen view:${err.message}`);
      });
    } else {
      document.exitFullscreen();
    }
  }
}
