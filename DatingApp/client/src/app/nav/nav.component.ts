import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";
import { AccountService } from '../_services/account.service';
import { User } from "../_models/user";
import { Observable } from "rxjs";
import { ToastrService } from "ngx-toastr";


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public accountServ: AccountService, private router: Router,
    private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  login() {
    this.accountServ.login(this.model).subscribe(response => {
      this.router.navigateByUrl("/members");
    }, error => {
        console.log(error);
        this.toastr.error(error.error);
    });
  }

  logout() {
    this.accountServ.logout();
    this.router.navigateByUrl("/");
  }
}
