import { TestBed } from "@angular/core/testing";
import { HttpClient, provideHttpClient, withInterceptors } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { authInterceptor } from "./auth-interceptor";

describe('AuthInterceptor', () => {
    let httpTestingController: HttpTestingController;
    let httpClient: HttpClient;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provideHttpClient(withInterceptors([authInterceptor])),
                provideHttpClientTesting(),
            ],
        });

        httpClient = TestBed.inject(HttpClient);
        httpTestingController = TestBed.inject(HttpTestingController);
    });

    it('should add withCredentials=true to every outgoing request', () => {
        //Make a dummy call to test url
        httpClient.get('/test-endpoint').subscribe();
        //Expect it to be intercepted
        const req = httpTestingController.expectOne('/test-endpoint');
        //Assert that the request was interceptor
        expect(req.request.withCredentials).toBeTrue();
        req.flush({}); //Close the request
    });
});