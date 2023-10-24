import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { AccountRoutingModule } from './account-routing.module';
<<<<<<< HEAD


=======
import { SharedModule } from '../shared/shared.module';
>>>>>>> upstream/main

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    CommonModule,
<<<<<<< HEAD
    AccountRoutingModule
=======
    AccountRoutingModule,
    SharedModule
>>>>>>> upstream/main
  ]
})
export class AccountModule { }
