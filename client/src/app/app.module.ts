import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CoreModule } from './core/core.module';
import { HomeModule } from './home/home.module';
import { LoadingInterceptor } from './core/interceptors/loading.interceptor';
import { JwtInterceptor } from './core/interceptors/jwt.interceptor';
import { OrderDetailedComponent } from './order-detailed/order-detailed.component';
import { SharedModule } from "./shared/shared.module";

@NgModule({
    declarations: [
        AppComponent,
        OrderDetailedComponent,
    ],
    providers: [
        { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        HttpClientModule,
        CoreModule,
        HomeModule,
        SharedModule
    ]
})
export class AppModule { }
