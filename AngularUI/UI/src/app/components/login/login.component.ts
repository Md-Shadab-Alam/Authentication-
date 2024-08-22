import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helper/validateform';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  type: string = "password";
  control: string = "show"
  isclick: boolean = false
  loginForm: FormGroup;

  constructor(private fb:FormBuilder,private auth: AuthService){}

  ngOnInit(): void {
    console.log(this.type);
    this.loginForm = this.fb.group({
      username:['',Validators.required],
      password:['',Validators.required]
    })
  }
  ShowHidepassword() {
    if (this.isclick == false) {
      this.isclick = true
      this.type = "text";
      this.control = "hide"
    }
    else{
      this.isclick=false
      this.type="password"
      this.control = "show"
    }
  }
  onSubmit(){
    if(this.loginForm.valid){
      //send the object to data base
      this.auth.login(this.loginForm.value)
        .subscribe({
          next:(res)=>{
            console.log(res)
            this.loginForm.reset();
          },
          error:(err)=>{
            console.log(err.message)
          }
          
        })
      console.log(this.loginForm.value);
    }
    else{
      ValidateForm.validateAllFormFileds(this.loginForm);

      //throw the error using toaster and with required fields
    }
  }
  

}