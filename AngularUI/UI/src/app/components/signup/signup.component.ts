import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {
  type: string = "password";
  control: string = "show"
  isclick: boolean = false

  ngOnInit(): void {
    console.log(this.type);
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
}
