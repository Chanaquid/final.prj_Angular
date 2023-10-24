import { Component } from '@angular/core';
<<<<<<< HEAD
=======
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
>>>>>>> upstream/main

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
<<<<<<< HEAD

=======
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password : new FormControl('', Validators.required)
  })

  constructor(private accountService: AccountService, private router: Router) { }

  onSubmit(){
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => this.router.navigateByUrl('/shop')
    })
  }
>>>>>>> upstream/main
}
