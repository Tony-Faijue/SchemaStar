import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environment';

export enum LogLevel {
  //Severity based
  all = 6, 
  fatal = 5, //application cannot continue
  error = 4, //operation failed need investigation
  warn = 3, //unexpected but recoverable
  info = 2, //normal operations
  debug = 1, //developer diagnostics
  none = 0,
}

@Injectable({
  providedIn: 'root',
})
export class LoggerService {
  
    //Need HttpClient to log information to backend be careful of circular dependency & log bloat

  /**
   * Fatal Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  fatal(message?: any, ...optionalParams: any[]){
    if(environment.logLevel >= LogLevel.fatal){
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
    if (environment.logLevel >= LogLevel.error){
      console.error(message, ...optionalParams);
    }
  }

    /**
   * Info Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  info(message?: any, ...optionalParams: any[]){
    if (environment.logLevel >= LogLevel.info){
      console.info(message, ...optionalParams);
    }
  }

    /**
   * Debug Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  debug(message?: any, ...optionalParams: any[]){
    if (environment.logLevel >= LogLevel.debug){
      console.debug(message, ...optionalParams);
    }
  }

    /**
   * Generic log 'info' Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  log(message?: any, ...optionalParams: any[]){
    if (environment.logLevel >= LogLevel.info){
      console.log(message, ...optionalParams);
    }
  }

    /**
   * Display Debug Level Message as Table Format
   * @param data data to be displayed in the table
   * @param properties optional properties to be passed in the table
   */
  table(data?: any, properties?: string[]){
    if (environment.logLevel >= LogLevel.debug){
      console.table(data, properties);
    }
  }

    /**
   * Trace 'debug' Level Message
   * @param message error message to display
   * @param optionalParams optional parameters to be passed
   */
  trace(message?: any, ...optionalParams: any[]){
    if (environment.logLevel >= LogLevel.debug){
      console.trace(message, ...optionalParams);
    }
  }
}
