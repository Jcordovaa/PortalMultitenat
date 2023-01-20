import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InboxRoutingModule } from './inbox-routing.module';
import { MessagesComponent } from './messages/messages.component';
import { ComposeDialogComponent } from './compose-dialog/compose-dialog.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { QuillModule } from 'ngx-quill';
import { SharedDirectivesModule } from 'src/app/shared/directives/shared-directives.module';

@NgModule({
  imports: [
    CommonModule,
    NgbModule,
    QuillModule,
    SharedDirectivesModule,
    InboxRoutingModule
  ],
  declarations: [MessagesComponent, ComposeDialogComponent],
  entryComponents: [ComposeDialogComponent]
})
export class InboxModule { }
