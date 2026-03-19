import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environment';

export enum LogLevel {
  //Severity based
  none = 0,
  fatal = 1, //application cannot continue
  error = 2, //operation failed need investigation
  warn = 3, //unexpected but recoverable
  info = 4, //normal operations
  debug = 5, //developer diagnostics
}

@Injectable({
  providedIn: 'root',
})
export class LoggerService {
  
    //Need HttpClient to log information to backend be careful of circular dependency & log bloat

    /**
     * Only logs the level that are equal to it and below
     * @param level the log level
     * @returns true if the log level is less than the environment log level and does not equal a log level of none
     */
    private shouldLog(level: LogLevel): boolean {
      return environment.logLevel >= level && environment.logLevel !== LogLevel.none;
    }

  /**
   * Fatal Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  fatal(message?: any, ...optionalParams: any[]){
    if(this.shouldLog(LogLevel.fatal)){
      console.error(
        "%c FATAL ",
        "background: #8b0000; color: #ffffff; font-weight: bold; padding: 2px 5px;",
        message,
        ...optionalParams
      );
    }
  }

  /**
   * Error Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  error(message?: any, ...optionalParams: any[]){
    if (this.shouldLog(LogLevel.error)){
      console.error(message, ...optionalParams);
    }
  }

    /**
   * Warn Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  warn(message?: any, ...optionalParams: any[]){
    if (this.shouldLog(LogLevel.warn)){
      console.info(message, ...optionalParams);
    }
  }

    /**
   * Debug Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  debug(message?: any, ...optionalParams: any[]){
    if (this.shouldLog(LogLevel.debug)){
      console.debug(message, ...optionalParams);
    }
  }

    /**
   * Generic log 'info' Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  log(message?: any, ...optionalParams: any[]){
    if (this.shouldLog(LogLevel.info)){
      console.log(message, ...optionalParams);
    }
  }

    /**
   * Display Debug Level Message as Table Format
   * @param data data to be displayed in the table
   * @param properties optional properties to be passed in the table
   */
  table(data?: any, properties?: string[]){
    if (this.shouldLog(LogLevel.debug)){
      console.table(data, properties);
    }
  }

    /**
   * Trace 'debug' Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  trace(message?: any, ...optionalParams: any[]){
    if (this.shouldLog(LogLevel.debug)){
      console.trace(message, ...optionalParams);
    }
  }
}
