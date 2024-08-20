import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import ValidateForm from 'src/app/helper/validateform';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {
  type: string = "password";
  control: string = "show"
  isclick: boolean = false
  signupForm:FormGroup;

  constructor(private fb: FormBuilder){}

  ngOnInit(): void {
    console.log(this.type);
    this.signupForm=this.fb.group({
      firstname:['',Validators.required],
      lastname:['', Validators.required],
      email:['', Validators.required],
      username:['',Validators.required],
      password:['', Validators.required]
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
    if(this.signupForm.valid){
      //send the object to data base
      console.log(this.signupForm.value);
    }
    else{
      ValidateForm.validateAllFormFileds(this.signupForm);

      //throw the error using toaster and with required fields
    }
  }
}
