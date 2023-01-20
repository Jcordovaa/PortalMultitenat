import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { User, ChatService } from '../chat.service';

@Component({
  selector: 'app-chat-left-sidebar',
  templateUrl: './chat-left-sidebar.component.html',
  styleUrls: ['./chat-left-sidebar.component.scss']
})
export class ChatLeftSidebarComponent implements OnInit {

  userUpdateSub: Subscription;
  loadDataSub: Subscription;

  isSidenavOpen = true;

  currentUserAd: User = new User();
  contacts: any[];

  constructor(private chatService: ChatService) {}

  ngOnInit() {
    // this.chatService.onChatsUpdated
    //   .subscribe(updatedChats => {
    //     this.chats = updatedChats;
    //   });

    this.userUpdateSub = this.chatService.onUserUpdated
      .subscribe(updatedUser => {
        this.currentUserAd = updatedUser;
      });

    this.loadDataSub = this.chatService.loadChatData()
      .subscribe(res => {
        this.currentUserAd = this.chatService.user;
        // this.chats = this.chatService.chats;
        this.contacts = this.chatService.contacts;
      });
  }
  ngOnDestroy() {
    if ( this.userUpdateSub ) { this.userUpdateSub.unsubscribe(); }
    if ( this.loadDataSub ) { this.loadDataSub.unsubscribe(); }
  }

  getChatByContact(contactId) {
    this.chatService.getChatByContact(contactId)
      .subscribe(res => {
        console.log('from sub', res);
      }, err => {
        console.log(err);
      });
  }
}
