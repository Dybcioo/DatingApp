import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AccountService } from "../_services/account.service";
import { ToastrService } from "ngx-toastr";
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Data, Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  maxDate: Data;
  validationErrors: string[] = [];

  registerForm: FormGroup;

  constructor(private accountServ: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm(){
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required ,this.matchValues("password")]]
    });
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }

  matchValues(matchTo: string): ValidatorFn{
    return (control: AbstractControl) => {
      const c = control?.parent?.controls as any;
      return (c)
        ? (control?.value === c[matchTo]?.value) ? null : { isMatching: true }
        : null;
    }
  }

  register() {
    this.accountServ.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl("/members");
    }, error => {
      this.validationErrors = error;
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
