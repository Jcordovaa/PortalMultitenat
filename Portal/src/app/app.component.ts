import { Component, HostListener } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'bootDash';

  // Agrega un evento para escuchar los cambios en el LocalStorage
  @HostListener('window:storage', ['$event'])
  onStorageChange(event: StorageEvent) {
    if (event.key === 'currentUserPortal') {
      // Forzar recarga para actualizar los datos del último cliente que inició sesión
      window.location.reload();
    }
  }
}
