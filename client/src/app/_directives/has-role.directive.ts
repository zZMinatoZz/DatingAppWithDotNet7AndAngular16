import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]' // *appHasRole='["Admin", "Moderator"]'
})
export class HasRoleDirective implements OnInit {

  @Input() appHasRole: string[] = [];
  user: User = {} as User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(
      {
        next: user => {
          if (user) this.user = user
        }
      }
    )
  }

  ngOnInit(): void {
    // check whether user roles have any role that declared inside appHasRole
    if (this.user.roles.some(r => this.appHasRole.includes(r))) {
      // display element
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      // no display element
      this.viewContainerRef.clear();
    }
  }

}
