import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GlobalErrorNotificationService {
  
  private errorMessageSignal = signal<string | null>(null);
  //prevent modification of this signal with set or update methods besides the custom methods here
  public errorMessage = this.errorMessageSignal.asReadonly();

  /**
   * Sets the global error message
   * @param message 
   */
  showError(message: string){
    this.errorMessageSignal.set(message);
  }

/**
 * Clears the global error message
 */
  clearError(){
    this.errorMessageSignal.set(null);
  }
}
