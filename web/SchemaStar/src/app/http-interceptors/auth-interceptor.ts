import { HttpInterceptorFn } from "@angular/common/http";

//Http Interceptor Middleware

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    //Add with credentials
    const authReq = req.clone({
        withCredentials: true
    });

    return next(authReq);
}