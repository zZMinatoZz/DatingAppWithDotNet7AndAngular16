import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  constructor(private memberService: MembersService, private toastr: ToastrService) { }
  ngOnInit(): void { }
  @Input() member: Member | undefined;

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe(
      {
        next: () => this.toastr.success('You have liked ' + member.knownAs)
      }
    )
  }
}
