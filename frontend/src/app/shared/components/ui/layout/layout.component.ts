import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, SidebarComponent],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss'
})
export class LayoutComponent {
  protected readonly sidebarOpen = signal(false);

  openSidebar(): void  { this.sidebarOpen.set(true); }
  closeSidebar(): void { this.sidebarOpen.set(false); }
}
