import { Component } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { debounce, debounceTime, finalize, map, switchMap, take } from 'rxjs';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {

  errors : string [] | null = null;

  constructor(private fb: FormBuilder, private accountService: AccountService,  private router: Router) {}

 
  // This regular expression enforces a strong password that must be at least 8 characters long, containing at least one digit, one lowercase letter, one uppercase letter, and one special character from the specified set, with a combination of digits, letters, and special characters.

  complexPassword = "^(?=.*[0-9]+.*)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{\":;'?/>.<,]+).*[0-9a-zA-Z!@#$%^&*()_+}{\":;'?/>.<,]{8,}$"

  //Samples
  //complexPassword = "^(?=.*[0-9]+.*)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{8,}$"
  //complexPassword = "^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$"
  //complexPassword = "(?=^.{6,10}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$" 

  registerForm = this.fb.group({
    displayName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email], [this.validateEmailNotTaken()]],
    password: ['', [Validators.required, Validators.pattern(this.complexPassword)]]

  }) 

  onSubmit() {
    this.accountService.register(this.registerForm.value).subscribe({
      next: () => this.router.navigateByUrl('/shop'), 
      error: error => this.errors = error.errors
    })
  }

  validateEmailNotTaken(): AsyncValidatorFn {
    return (control: AbstractControl) => {
      return control.valueChanges.pipe(
        debounceTime(1800),
        take(1),
        switchMap(() => {
          return this.accountService.checkEmailExists(control.value).pipe(
            map(result => result ? {emailExists: true} : null),
            finalize(() => control.markAsTouched())
          )
        })
      )
    }
  }


}
