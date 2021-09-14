import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountService:AccountService,private toaster:ToastrService){}
  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(map(User=>{
      if(User)return true;
      this.toaster.error('You shall not pass!!!');
      return false;
    }))
  }
  
}
